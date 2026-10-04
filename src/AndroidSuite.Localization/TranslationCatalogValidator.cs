using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using AndroidSuite.Core.Localization;

namespace AndroidSuite.Localization;

/// <summary>
/// Prueft Sprachkataloge auf Vollstaendigkeit und Konsistenz (§46, Testplan §5).
/// Wird von den Tests als Release-Gate und beim Programmstart als Selbstpruefung verwendet.
/// </summary>
public static partial class TranslationCatalogValidator
{
    /// <summary>Prueft alle Sprachen eines Katalogs gegeneinander.</summary>
    /// <param name="catalog">Zu pruefender Katalog.</param>
    /// <param name="referenceCulture">Sprache, die als Mass fuer Vollstaendigkeit gilt (Vorgabe: Deutsch).</param>
    /// <returns>Bericht mit allen Abweichungen.</returns>
    /// <exception cref="ArgumentNullException">Wenn <paramref name="catalog"/> <c>null</c> ist.</exception>
    public static CatalogValidationReport Validate(ITranslationCatalog catalog, CultureInfo? referenceCulture = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var reference = referenceCulture ?? JsonLocalizationService.GermanCulture;
        var referenceKeys = catalog.GetKeys(reference).ToHashSet(StringComparer.Ordinal);

        var missing = new List<CatalogIssue>();
        var extra = new List<CatalogIssue>();
        var empty = new List<CatalogIssue>();
        var placeholders = new List<CatalogIssue>();

        foreach (var culture in catalog.AvailableCultures)
        {
            var keys = catalog.GetKeys(culture).ToHashSet(StringComparer.Ordinal);

            foreach (var key in referenceKeys.Except(keys, StringComparer.Ordinal).OrderBy(k => k, StringComparer.Ordinal))
            {
                missing.Add(new CatalogIssue(culture.Name, key, "Schluessel fehlt in dieser Sprache."));
            }

            foreach (var key in keys.Except(referenceKeys, StringComparer.Ordinal).OrderBy(k => k, StringComparer.Ordinal))
            {
                extra.Add(new CatalogIssue(culture.Name, key, "Schluessel existiert nicht in der Referenzsprache."));
            }

            foreach (var key in keys.OrderBy(k => k, StringComparer.Ordinal))
            {
                if (!catalog.TryGetString(culture, key, out var value) || string.IsNullOrWhiteSpace(value))
                {
                    empty.Add(new CatalogIssue(culture.Name, key, "Uebersetzung ist leer."));
                    continue;
                }

                if (!referenceKeys.Contains(key) ||
                    !catalog.TryGetString(reference, key, out var referenceValue))
                {
                    continue;
                }

                var expected = ExtractPlaceholders(referenceValue);
                var actual = ExtractPlaceholders(value);
                if (!expected.SetEquals(actual))
                {
                    placeholders.Add(new CatalogIssue(
                        culture.Name,
                        key,
                        $"Platzhalter weichen ab: erwartet [{string.Join(", ", expected.Order())}], " +
                        $"gefunden [{string.Join(", ", actual.Order())}]."));
                }
            }
        }

        return new CatalogValidationReport(
            new ReadOnlyCollection<CatalogIssue>(missing),
            new ReadOnlyCollection<CatalogIssue>(extra),
            new ReadOnlyCollection<CatalogIssue>(empty),
            new ReadOnlyCollection<CatalogIssue>(placeholders));
    }

    /// <summary>Ermittelt die verwendeten Formatplatzhalter einer Textvorlage.</summary>
    /// <param name="template">Textvorlage, z. B. "{0} von {1} Dateien".</param>
    /// <returns>Menge der Platzhalterindizes.</returns>
    public static HashSet<int> ExtractPlaceholders(string template)
    {
        var result = new HashSet<int>();
        if (string.IsNullOrEmpty(template))
        {
            return result;
        }

        foreach (var match in PlaceholderRegex().Matches(template).Cast<Match>())
        {
            if (int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
            {
                result.Add(index);
            }
        }

        return result;
    }

    [GeneratedRegex(@"\{(\d+)(?:[,:][^}]*)?\}", RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();
}

/// <summary>Einzelner Befund der Katalogpruefung.</summary>
/// <param name="Culture">Betroffene Sprache.</param>
/// <param name="Key">Betroffener Schluessel.</param>
/// <param name="Description">Beschreibung des Befunds.</param>
public sealed record CatalogIssue(string Culture, string Key, string Description);

/// <summary>Ergebnis der Katalogpruefung.</summary>
/// <param name="MissingKeys">Schluessel, die in einer Sprache fehlen.</param>
/// <param name="ExtraKeys">Schluessel ohne Entsprechung in der Referenzsprache.</param>
/// <param name="EmptyValues">Leere Uebersetzungen.</param>
/// <param name="PlaceholderMismatches">Abweichende Formatplatzhalter.</param>
public sealed record CatalogValidationReport(
    IReadOnlyList<CatalogIssue> MissingKeys,
    IReadOnlyList<CatalogIssue> ExtraKeys,
    IReadOnlyList<CatalogIssue> EmptyValues,
    IReadOnlyList<CatalogIssue> PlaceholderMismatches)
{
    /// <summary>Alle Befunde zusammen.</summary>
    public IEnumerable<CatalogIssue> AllIssues
        => MissingKeys.Concat(ExtraKeys).Concat(EmptyValues).Concat(PlaceholderMismatches);

    /// <summary><c>true</c>, wenn keine Abweichung gefunden wurde.</summary>
    public bool IsValid => !AllIssues.Any();

    /// <summary>Lesbare Zusammenfassung fuer Testausgaben und Protokolle.</summary>
    /// <returns>Mehrzeilige Beschreibung aller Befunde.</returns>
    public string Describe()
        => IsValid
            ? "Katalog vollstaendig und konsistent."
            : string.Join(
                Environment.NewLine,
                AllIssues.Select(i => $"[{i.Culture}] {i.Key}: {i.Description}"));
}
