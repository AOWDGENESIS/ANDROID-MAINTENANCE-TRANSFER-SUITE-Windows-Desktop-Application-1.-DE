using System.Globalization;
using System.Reflection;

namespace AndroidSuite.Localization.Tests;

/// <summary>
/// Release-blockierende Pruefungen der ausgelieferten Sprachkataloge
/// (TEST_PLAN §5, RELEASE_CHECKLIST Nr. 12).
/// </summary>
public sealed class CatalogCompletenessTests
{
    private static readonly EmbeddedJsonTranslationCatalog Catalog = EmbeddedJsonTranslationCatalog.Load();

    [Fact]
    public void Catalog_contains_german_and_english()
    {
        var names = Catalog.AvailableCultures.Select(c => c.Name).ToArray();

        Assert.Contains("de", names);
        Assert.Contains("en", names);
    }

    [Fact]
    public void All_keys_exist_in_all_cultures()
    {
        var report = TranslationCatalogValidator.Validate(Catalog);

        Assert.True(report.MissingKeys.Count == 0, report.Describe());
        Assert.True(report.ExtraKeys.Count == 0, report.Describe());
    }

    [Fact]
    public void No_empty_translations()
    {
        var report = TranslationCatalogValidator.Validate(Catalog);

        Assert.True(report.EmptyValues.Count == 0, report.Describe());
    }

    [Fact]
    public void Placeholders_match_between_cultures()
    {
        var report = TranslationCatalogValidator.Validate(Catalog);

        Assert.True(report.PlaceholderMismatches.Count == 0, report.Describe());
    }

    [Fact]
    public void Catalog_is_valid_overall()
    {
        var report = TranslationCatalogValidator.Validate(Catalog);

        Assert.True(report.IsValid, report.Describe());
    }

    [Fact]
    public void Every_translation_key_constant_resolves_in_every_culture()
    {
        var constants = GetKeyConstants();

        Assert.NotEmpty(constants);

        var failures = new List<string>();
        foreach (var culture in Catalog.AvailableCultures)
        {
            foreach (var key in constants)
            {
                if (!Catalog.TryGetString(culture, key, out var value) || string.IsNullOrWhiteSpace(value))
                {
                    failures.Add($"[{culture.Name}] {key}");
                }
            }
        }

        Assert.True(failures.Count == 0, "Nicht aufloesbare Konstanten: " + string.Join(", ", failures));
    }

    [Fact]
    public void Every_catalog_key_has_a_constant()
    {
        // Verhindert verwaiste Texte, die niemand mehr verwendet, und erzwingt,
        // dass neue Texte ueber TranslationKeys angesprochen werden.
        var constants = GetKeyConstants().ToHashSet(StringComparer.Ordinal);
        var catalogKeys = Catalog.GetKeys(CultureInfo.GetCultureInfo("de"));

        var orphans = catalogKeys.Where(k => !constants.Contains(k)).Order(StringComparer.Ordinal).ToArray();

        Assert.True(orphans.Length == 0, "Schluessel ohne Konstante: " + string.Join(", ", orphans));
    }

    [Fact]
    public void Keys_follow_the_naming_convention()
    {
        var invalid = Catalog.GetKeys(CultureInfo.GetCultureInfo("de"))
            .Where(k => !k.Contains('.', StringComparison.Ordinal) || k != k.Trim())
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(invalid.Length == 0, "Schluessel ohne Bereichspraefix: " + string.Join(", ", invalid));
    }

    [Fact]
    public void Loading_from_an_assembly_without_resources_reports_a_clear_error()
    {
        var exception = Assert.Throws<TranslationCatalogException>(
            () => EmbeddedJsonTranslationCatalog.Load(typeof(object).Assembly));

        Assert.Contains("Sprachressource", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Unknown_culture_returns_no_keys_instead_of_throwing()
    {
        var keys = Catalog.GetKeys(CultureInfo.GetCultureInfo("fr"));

        Assert.Empty(keys);
    }

    [Fact]
    public void Regional_culture_falls_back_to_neutral_catalog()
    {
        var found = Catalog.TryGetString(CultureInfo.GetCultureInfo("de-AT"), TranslationKeys.DeviceConnected, out var value);

        Assert.True(found);
        Assert.Equal("Gerät verbunden", value);
    }

    [Fact]
    public void German_and_english_core_texts_differ()
    {
        // Schutz gegen versehentlich kopierte Kataloge: die Uebersetzung muss echt sein.
        Catalog.TryGetString(CultureInfo.GetCultureInfo("de"), TranslationKeys.DeviceConnected, out var german);
        Catalog.TryGetString(CultureInfo.GetCultureInfo("en"), TranslationKeys.DeviceConnected, out var english);

        Assert.Equal("Gerät verbunden", german);
        Assert.Equal("Device connected", english);
    }

    private static string[] GetKeyConstants()
        => [.. typeof(TranslationKeys)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)];
}
