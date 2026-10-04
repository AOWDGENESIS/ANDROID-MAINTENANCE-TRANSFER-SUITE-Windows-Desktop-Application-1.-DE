using System.Globalization;
using AndroidSuite.Core.Localization;
using AndroidSuite.Localization;

namespace AndroidSuite.SmokeRunner;

/// <summary>
/// Laufzeitpruefung der Komponente "Localization" (§60: RUNTIME TEST, isolierte Umgebung).
/// Spielt den echten Ablauf durch: Erststart, Live-Sprachwechsel, dauerhaftes Speichern,
/// Neustart mit wiederhergestellter Sprache. Beendet sich mit Code 1, sobald eine
/// Erwartung verletzt wird - damit ist das Gate automatisierbar.
/// </summary>
internal static class Program
{
    private static int _failures;

    private static async Task<int> Main()
    {
        var sandbox = Path.Combine(Path.GetTempPath(), "androidsuite-smoke", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        var settingsPath = Path.Combine(sandbox, "settings.json");

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== RUNTIME-TEST: Lokalisierung ===");
        Console.WriteLine(Text($"Testumgebung: {sandbox}"));
        Console.WriteLine();

        try
        {
            var catalog = EmbeddedJsonTranslationCatalog.Load();
            Console.WriteLine(Text($"Kataloge geladen: {string.Join(", ", catalog.AvailableCultures.Select(c => c.Name))}"));

            var report = TranslationCatalogValidator.Validate(catalog);
            Check("Selbstpruefung der Kataloge", report.IsValid, report.Describe());

            // 1. Erststart ohne gespeicherte Einstellung -> Deutsch
            using var store = new JsonFileLanguagePreferenceStore(settingsPath);
            ILocalizationService service = await JsonLocalizationService.CreateAsync(catalog, store).ConfigureAwait(false);
            Check("Erststart ist Deutsch", service.CurrentCulture.Name == "de", service.CurrentCulture.Name);
            Print(service);

            // 2. Live-Wechsel waehrend des Betriebs, ohne Neuaufbau des Dienstes
            var indexerNotified = false;
            service.PropertyChanged += (_, e) =>
                indexerNotified |= e.PropertyName == JsonLocalizationService.IndexerName;

            var switched = await service.SetCultureAsync(CultureInfo.GetCultureInfo("en")).ConfigureAwait(false);
            Check("Wechsel nach Englisch gespeichert", switched, "SetCultureAsync lieferte false");
            Check("Oberflaeche wird zur Neubindung aufgefordert", indexerNotified, "keine Indexer-Meldung");
            Check("Text ist englisch",
                service[TranslationKeys.DeviceConnected] == "Device connected",
                service[TranslationKeys.DeviceConnected]);
            Print(service);

            // 3. Neustart simulieren -> gespeicherte Sprache wird wiederhergestellt
            using var store2 = new JsonFileLanguagePreferenceStore(settingsPath);
            ILocalizationService restarted = await JsonLocalizationService.CreateAsync(catalog, store2).ConfigureAwait(false);
            Check("Sprache ueberlebt den Neustart", restarted.CurrentCulture.Name == "en", restarted.CurrentCulture.Name);

            // 4. Zurueck auf Deutsch
            await restarted.SetCultureAsync(CultureInfo.GetCultureInfo("de")).ConfigureAwait(false);
            Check("Rueckwechsel nach Deutsch",
                restarted[TranslationKeys.DeviceConnected] == "Gerät verbunden",
                restarted[TranslationKeys.DeviceConnected]);

            // 5. Unbekannte Sprache darf nichts kaputt machen
            var rejected = await restarted.SetCultureAsync(CultureInfo.GetCultureInfo("fr")).ConfigureAwait(false);
            Check("Unbekannte Sprache wird abgelehnt", !rejected, "wurde faelschlich akzeptiert");
            Check("Sprache bleibt Deutsch", restarted.CurrentCulture.Name == "de", restarted.CurrentCulture.Name);

            // 6. Fehlender Schluessel fuehrt nicht zum Absturz
            Check("Fehlender Schluessel liefert den Schluesselnamen",
                restarted["kein.schluessel.vorhanden"] == "kein.schluessel.vorhanden",
                restarted["kein.schluessel.vorhanden"]);
        }
#pragma warning disable CA1031 // Der Laufzeittest meldet bewusst JEDEN Fehler als Gate-Verstoss (AW-002).
        catch (Exception ex)
#pragma warning restore CA1031
        {
            Console.WriteLine(Text($"UNERWARTETE AUSNAHME: {ex}"));
            _failures++;
        }
        finally
        {
            TryCleanup(sandbox);
        }

        Console.WriteLine();
        Console.WriteLine(_failures == 0 ? "RUNTIME TEST: PASS" : $"RUNTIME TEST: FAIL ({_failures})");
        return _failures == 0 ? 0 : 1;
    }

    private static string Text(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);

    private static void Print(ILocalizationService service)
    {
        Console.WriteLine(
            Text($"  [{service.CurrentCulture.Name}] {service[TranslationKeys.NavDashboard]}")
            + Text($" | {service[TranslationKeys.NavCleanup]}")
            + Text($" | {service[TranslationKeys.DeviceConnected]}")
            + Text($" | {service.Format(TranslationKeys.TransferProgressFiles, 12, 340)}"));
    }

    private static void Check(string description, bool condition, string detail)
    {
        if (condition)
        {
            Console.WriteLine(Text($"  PASS  {description}"));
            return;
        }

        _failures++;
        Console.WriteLine(Text($"  FAIL  {description} -> {detail}"));
    }

    private static void TryCleanup(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.WriteLine(Text($"Hinweis: Testordner blieb bestehen ({path})."));
        }
    }
}
