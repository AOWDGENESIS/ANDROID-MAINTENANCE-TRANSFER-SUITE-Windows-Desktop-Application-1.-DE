using System.Collections.Frozen;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using AndroidSuite.Core.Localization;

namespace AndroidSuite.Localization;

/// <summary>
/// Laedt Uebersetzungen aus eingebetteten JSON-Ressourcen (<c>Resources/de.json</c>, <c>Resources/en.json</c>).
/// </summary>
/// <remarks>
/// Bewusst JSON statt RESX: diff-freundlich, ohne Werkzeug editierbar und zur Laufzeit
/// auf Vollstaendigkeit pruefbar (siehe <see cref="TranslationCatalogValidator"/>).
/// Die Kataloge werden beim Erzeugen einmalig geladen und sind danach unveraenderlich,
/// damit Lesezugriffe ohne Sperre threadsicher sind.
/// </remarks>
public sealed class EmbeddedJsonTranslationCatalog : ITranslationCatalog
{
    private const string ResourcePrefix = "AndroidSuite.Localization.Resources.";
    private const string ResourceSuffix = ".json";

    private readonly FrozenDictionary<string, FrozenDictionary<string, string>> _byCulture;

    private EmbeddedJsonTranslationCatalog(
        FrozenDictionary<string, FrozenDictionary<string, string>> byCulture,
        IReadOnlyList<CultureInfo> cultures)
    {
        _byCulture = byCulture;
        AvailableCultures = cultures;
    }

    /// <inheritdoc />
    public IReadOnlyList<CultureInfo> AvailableCultures { get; }

    /// <summary>Laedt alle eingebetteten Sprachkataloge der Lokalisierungs-Assembly.</summary>
    /// <exception cref="TranslationCatalogException">
    /// Wenn keine Ressource gefunden wird oder eine Ressource kein flaches
    /// JSON-Objekt aus Zeichenketten ist. Das ist ein Build-Fehler, kein Laufzeitzustand.
    /// </exception>
    public static EmbeddedJsonTranslationCatalog Load()
        => Load(typeof(EmbeddedJsonTranslationCatalog).Assembly);

    /// <summary>Laedt alle eingebetteten Sprachkataloge der angegebenen Assembly.</summary>
    /// <param name="assembly">Assembly mit den eingebetteten Ressourcen.</param>
    /// <exception cref="ArgumentNullException">Wenn <paramref name="assembly"/> <c>null</c> ist.</exception>
    /// <exception cref="TranslationCatalogException">Wenn keine oder ungueltige Ressourcen vorliegen.</exception>
    public static EmbeddedJsonTranslationCatalog Load(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var cultures = new List<CultureInfo>();
        var catalogs = new Dictionary<string, FrozenDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var resourceName in assembly.GetManifestResourceNames().OrderBy(n => n, StringComparer.Ordinal))
        {
            if (!resourceName.StartsWith(ResourcePrefix, StringComparison.Ordinal) ||
                !resourceName.EndsWith(ResourceSuffix, StringComparison.Ordinal))
            {
                continue;
            }

            var cultureName = resourceName.Substring(
                ResourcePrefix.Length,
                resourceName.Length - ResourcePrefix.Length - ResourceSuffix.Length);

            CultureInfo culture;
            try
            {
                culture = CultureInfo.GetCultureInfo(cultureName);
            }
            catch (CultureNotFoundException ex)
            {
                throw new TranslationCatalogException(
                    $"Ressource '{resourceName}' enthaelt keinen gueltigen Sprachnamen.", ex);
            }

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new TranslationCatalogException($"Ressource '{resourceName}' konnte nicht geoeffnet werden.");

            catalogs[culture.Name] = ReadFlatJson(stream, resourceName);
            cultures.Add(culture);
        }

        if (catalogs.Count == 0)
        {
            throw new TranslationCatalogException(
                $"In Assembly '{assembly.GetName().Name}' wurde keine Sprachressource mit dem Praefix '{ResourcePrefix}' gefunden.");
        }

        return new EmbeddedJsonTranslationCatalog(
            catalogs.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase),
            cultures);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> GetKeys(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return _byCulture.TryGetValue(culture.Name, out var map)
            ? map.Keys
            : Array.Empty<string>();
    }

    /// <inheritdoc />
    public bool TryGetString(CultureInfo culture, string key, out string value)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if (string.IsNullOrEmpty(key))
        {
            value = string.Empty;
            return false;
        }

        if (_byCulture.TryGetValue(culture.Name, out var map) && map.TryGetValue(key, out var found))
        {
            value = found;
            return true;
        }

        // Regionale Sprache (z. B. "de-DE") faellt auf die neutrale Sprache ("de") zurueck.
        var parent = culture.Parent;
        if (!string.IsNullOrEmpty(parent.Name) &&
            _byCulture.TryGetValue(parent.Name, out var parentMap) &&
            parentMap.TryGetValue(key, out var parentFound))
        {
            value = parentFound;
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static FrozenDictionary<string, string> ReadFlatJson(Stream stream, string resourceName)
    {
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(stream, new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });
        }
        catch (JsonException ex)
        {
            throw new TranslationCatalogException($"Ressource '{resourceName}' enthaelt kein gueltiges JSON.", ex);
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new TranslationCatalogException($"Ressource '{resourceName}' muss ein JSON-Objekt sein.");
            }

            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.String)
                {
                    throw new TranslationCatalogException(
                        $"Schluessel '{property.Name}' in '{resourceName}' ist keine Zeichenkette. " +
                        "Der Katalog muss flach und rein textuell sein.");
                }

                if (!map.TryAdd(property.Name, property.Value.GetString() ?? string.Empty))
                {
                    throw new TranslationCatalogException(
                        $"Schluessel '{property.Name}' kommt in '{resourceName}' mehrfach vor.");
                }
            }

            return map.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }
}
