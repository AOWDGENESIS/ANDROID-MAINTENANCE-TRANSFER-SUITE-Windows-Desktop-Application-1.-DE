using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AndroidSuite.Localization.Tests;

/// <summary>
/// Tests der dauerhaften Sprachablage, inklusive der Fehlerfaelle aus der Testmatrix (§63):
/// Datei fehlt, Datei beschaedigt, Ziel nicht beschreibbar, paralleler Zugriff.
/// </summary>
public sealed class JsonFileLanguagePreferenceStoreTests
{
    private static readonly string[] SupportedLanguages = ["de", "en"];

    [Fact]
    public async Task Missing_file_returns_null_instead_of_failing()
    {
        using var dir = new TempDirectory();
        var store = new JsonFileLanguagePreferenceStore(dir.File("settings.json"));

        Assert.Null(await store.LoadAsync());
        Assert.Null(store.LastError);
    }

    [Fact]
    public async Task Save_and_load_round_trip()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        var store = new JsonFileLanguagePreferenceStore(path);

        Assert.True(await store.SaveAsync("en"));
        Assert.Equal("en", await new JsonFileLanguagePreferenceStore(path).LoadAsync());
    }

    [Fact]
    public async Task Save_preserves_unrelated_settings_of_other_modules()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        await File.WriteAllTextAsync(path, """{ "theme": "dark", "parallelTransfers": 4 }""");
        var store = new JsonFileLanguagePreferenceStore(path);

        await store.SaveAsync("de");

        var root = JsonNode.Parse(await File.ReadAllTextAsync(path))!.AsObject();
        Assert.Equal("dark", root["theme"]!.GetValue<string>());
        Assert.Equal(4, root["parallelTransfers"]!.GetValue<int>());
        Assert.Equal("de", root["language"]!.GetValue<string>());
    }

    [Fact]
    public async Task Corrupt_file_is_tolerated_and_reported()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        await File.WriteAllTextAsync(path, "{ das ist kein JSON ");
        var store = new JsonFileLanguagePreferenceStore(path);

        var result = await store.LoadAsync();

        Assert.Null(result);
        Assert.IsAssignableFrom<JsonException>(store.LastError);
    }

    [Fact]
    public async Task Empty_file_is_treated_as_first_start()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        await File.WriteAllTextAsync(path, string.Empty);

        Assert.Null(await new JsonFileLanguagePreferenceStore(path).LoadAsync());
    }

    [Fact]
    public async Task File_without_language_property_is_treated_as_first_start()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        await File.WriteAllTextAsync(path, """{ "theme": "dark" }""");

        Assert.Null(await new JsonFileLanguagePreferenceStore(path).LoadAsync());
    }

    [Fact]
    public async Task Corrupt_file_is_replaced_on_save_without_data_loss_of_the_language()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        await File.WriteAllTextAsync(path, "kaputt");
        var store = new JsonFileLanguagePreferenceStore(path);

        Assert.True(await store.SaveAsync("en"));
        Assert.Equal("en", await store.LoadAsync());
    }

    [Fact]
    public async Task Corrupt_file_is_backed_up_before_being_replaced()
    {
        // Regressionstest zu BUG-001: Speichern scheiterte dauerhaft an einer kaputten Datei.
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        await File.WriteAllTextAsync(path, "kaputt");
        using var store = new JsonFileLanguagePreferenceStore(path);

        Assert.True(await store.SaveAsync("de"));

        Assert.NotNull(store.LastRecoveredBackupPath);
        Assert.True(File.Exists(store.LastRecoveredBackupPath));
        Assert.Equal("kaputt", await File.ReadAllTextAsync(store.LastRecoveredBackupPath!));
    }

    [Fact]
    public async Task Save_creates_missing_directories()
    {
        using var dir = new TempDirectory();
        var path = Path.Combine(dir.Path, "a", "b", "settings.json");
        var store = new JsonFileLanguagePreferenceStore(path);

        Assert.True(await store.SaveAsync("de"));
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Save_leaves_no_temporary_file_behind()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");

        await new JsonFileLanguagePreferenceStore(path).SaveAsync("de");

        Assert.False(File.Exists(path + ".tmp"));
    }

    [SkippableFact]
    public async Task Unwritable_target_reports_false_instead_of_throwing()
    {
        Skip.If(OperatingSystem.IsWindows(), "Rechtemodell wird unter Windows separat geprueft (PENDING-WINDOWS).");

        // Zusaetzliche Plattformabfrage: sie erfuellt die Analyse von CA1416,
        // die das Skip-Muster nicht erkennen kann.
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        using var dir = new TempDirectory();
        var locked = Path.Combine(dir.Path, "locked");
        Directory.CreateDirectory(locked);
        File.SetUnixFileMode(locked, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        try
        {
            var store = new JsonFileLanguagePreferenceStore(Path.Combine(locked, "settings.json"));

            Assert.False(await store.SaveAsync("de"));
            Assert.IsAssignableFrom<UnauthorizedAccessException>(store.LastError);
        }
        finally
        {
            File.SetUnixFileMode(locked, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }
    }

    [Fact]
    public async Task Concurrent_saves_do_not_corrupt_the_file()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        var store = new JsonFileLanguagePreferenceStore(path);

        await Task.WhenAll(Enumerable.Range(0, 20).Select(i =>
            store.SaveAsync(i % 2 == 0 ? "de" : "en")));

        var value = await store.LoadAsync();
        Assert.Contains(value, SupportedLanguages);
    }

    [Fact]
    public void Empty_path_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new JsonFileLanguagePreferenceStore("   "));
        Assert.Throws<ArgumentNullException>(() => new JsonFileLanguagePreferenceStore(null!));
    }

    [Fact]
    public async Task Save_rejects_an_empty_culture_name()
    {
        using var dir = new TempDirectory();
        var store = new JsonFileLanguagePreferenceStore(dir.File("settings.json"));

        await Assert.ThrowsAsync<ArgumentException>(() => store.SaveAsync(" "));
    }

    [Fact]
    public void Default_path_is_inside_the_user_profile_and_not_hardcoded_to_drive_c()
    {
        // §21/§49: keine feste Annahme auf C:\
        var path = JsonFileLanguagePreferenceStore.GetDefaultPath();

        Assert.Contains("AndroidSuite", path, StringComparison.Ordinal);
        Assert.EndsWith("settings.json", path, StringComparison.Ordinal);
        Assert.StartsWith(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            path,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task Full_round_trip_with_the_localization_service()
    {
        using var dir = new TempDirectory();
        var path = dir.File("settings.json");
        var catalog = EmbeddedJsonTranslationCatalog.Load();

        var first = await JsonLocalizationService.CreateAsync(catalog, new JsonFileLanguagePreferenceStore(path));
        Assert.Equal("de", first.CurrentCulture.Name);
        await first.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        var second = await JsonLocalizationService.CreateAsync(catalog, new JsonFileLanguagePreferenceStore(path));
        Assert.Equal("en", second.CurrentCulture.Name);
        Assert.Equal("Device connected", second[TranslationKeys.DeviceConnected]);
    }
}
