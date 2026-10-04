using System.ComponentModel;
using System.Globalization;
using AndroidSuite.Core.Localization;

namespace AndroidSuite.Localization.Tests;

/// <summary>
/// Verhaltenstests des Lokalisierungsdienstes, inklusive der Fehler- und Randfaelle
/// aus der Testmatrix (§63).
/// </summary>
public sealed class JsonLocalizationServiceTests
{
    private static Task<JsonLocalizationService> CreateAsync(
        ILanguagePreferenceStore? store = null,
        ITranslationCatalog? catalog = null)
        => JsonLocalizationService.CreateAsync(
            catalog ?? EmbeddedJsonTranslationCatalog.Load(),
            store ?? new FakePreferenceStore());

    // --- NORMAL ---------------------------------------------------------

    [Fact]
    public async Task First_start_uses_german()
    {
        var service = await CreateAsync();

        Assert.Equal("de", service.CurrentCulture.Name);
        Assert.Equal("Gerät verbunden", service[TranslationKeys.DeviceConnected]);
    }

    [Fact]
    public async Task Switching_to_english_changes_texts_immediately()
    {
        var service = await CreateAsync();

        var changed = await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        Assert.True(changed);
        Assert.Equal("en", service.CurrentCulture.Name);
        Assert.Equal("Device connected", service[TranslationKeys.DeviceConnected]);
    }

    [Fact]
    public async Task Switching_back_to_german_restores_texts()
    {
        var service = await CreateAsync();

        await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));
        await service.SetCultureAsync(CultureInfo.GetCultureInfo("de"));

        Assert.Equal("Gerät verbunden", service[TranslationKeys.DeviceConnected]);
    }

    [Fact]
    public async Task Culture_change_raises_indexer_notification_for_live_rebinding()
    {
        var service = await CreateAsync();
        var properties = new List<string?>();
        ((INotifyPropertyChanged)service).PropertyChanged += (_, e) => properties.Add(e.PropertyName);

        await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        Assert.Contains(JsonLocalizationService.IndexerName, properties);
        Assert.Contains(nameof(ILocalizationService.CurrentCulture), properties);
    }

    [Fact]
    public async Task Culture_change_raises_culture_changed_event_with_both_cultures()
    {
        var service = await CreateAsync();
        CultureChangedEventArgs? captured = null;
        service.CultureChanged += (_, e) => captured = e;

        await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        Assert.NotNull(captured);
        Assert.Equal("de", captured.PreviousCulture.Name);
        Assert.Equal("en", captured.CurrentCulture.Name);
    }

    [Fact]
    public async Task Selected_language_is_persisted()
    {
        var store = new FakePreferenceStore();
        var service = await CreateAsync(store);

        await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        Assert.Equal("en", store.Stored);
    }

    [Fact]
    public async Task Persisted_language_is_restored_on_next_start()
    {
        var store = new FakePreferenceStore("en");

        var service = await CreateAsync(store);

        Assert.Equal("en", service.CurrentCulture.Name);
        Assert.Equal("Device connected", service[TranslationKeys.DeviceConnected]);
    }

    [Fact]
    public async Task Available_cultures_are_reported()
    {
        var service = await CreateAsync();

        var names = service.AvailableCultures.Select(c => c.Name).ToArray();

        Assert.Contains("de", names);
        Assert.Contains("en", names);
    }

    [Fact]
    public async Task Format_inserts_arguments()
    {
        var service = await CreateAsync();

        var text = service.Format(TranslationKeys.TransferProgressFiles, 7, 42);

        Assert.Equal("7 von 42 Dateien", text);
    }

    [Fact]
    public async Task Format_uses_the_active_culture_for_numbers()
    {
        var service = await CreateAsync();

        var german = service.Format(TranslationKeys.CommonFilesCount, 1234.5);
        await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));
        var english = service.Format(TranslationKeys.CommonFilesCount, 1234.5);

        Assert.Equal("1234,5 Dateien", german);
        Assert.Equal("1234.5 files", english);
    }

    // --- FEHLER / RANDFAELLE --------------------------------------------

    [Fact]
    public async Task Unknown_key_returns_the_key_and_does_not_throw()
    {
        var service = await CreateAsync();

        Assert.Equal("gibt.es.nicht", service["gibt.es.nicht"]);
    }

    [Fact]
    public async Task Empty_or_whitespace_key_returns_empty_text()
    {
        var service = await CreateAsync();

        Assert.Equal(string.Empty, service[string.Empty]);
        Assert.Equal(string.Empty, service["   "]);
    }

    [Fact]
    public async Task Missing_translation_falls_back_to_german()
    {
        var catalog = new FakeCatalog()
            .With("de", ("a.b", "Deutscher Text"))
            .With("en", ("x.y", "only english"));
        var service = await CreateAsync(catalog: catalog);
        await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        Assert.Equal("Deutscher Text", service["a.b"]);
    }

    [Fact]
    public async Task TryGetString_reports_missing_keys_without_fallback()
    {
        var service = await CreateAsync();

        Assert.False(service.TryGetString("gibt.es.nicht", out var missing));
        Assert.Equal(string.Empty, missing);
        Assert.True(service.TryGetString(TranslationKeys.CommonOk, out var ok));
        Assert.Equal("OK", ok);
    }

    [Fact]
    public async Task Format_with_wrong_argument_count_returns_template_instead_of_throwing()
    {
        var catalog = new FakeCatalog().With("de", ("broken", "{0} und {1}"));
        var service = await CreateAsync(catalog: catalog);

        var result = service.Format("broken", "nur eins");

        Assert.Equal("{0} und {1}", result);
    }

    [Fact]
    public async Task Format_without_arguments_returns_template()
    {
        var service = await CreateAsync();

        Assert.Equal("Version {0}", service.Format(TranslationKeys.AppVersion));
    }

    [Fact]
    public async Task Unknown_culture_is_rejected_and_keeps_the_current_language()
    {
        var service = await CreateAsync();

        var result = await service.SetCultureAsync(CultureInfo.GetCultureInfo("fr"));

        Assert.False(result);
        Assert.Equal("de", service.CurrentCulture.Name);
    }

    [Fact]
    public async Task Regional_culture_resolves_to_the_neutral_catalog()
    {
        var service = await CreateAsync();

        var result = await service.SetCultureAsync(CultureInfo.GetCultureInfo("en-GB"));

        Assert.True(result);
        Assert.Equal("en", service.CurrentCulture.Name);
    }

    [Fact]
    public async Task Corrupt_stored_language_falls_back_to_german_instead_of_failing()
    {
        var store = new FakePreferenceStore("nicht-existente-sprache");

        var service = await CreateAsync(store);

        Assert.Equal("de", service.CurrentCulture.Name);
    }

    [Fact]
    public async Task Stored_language_outside_the_catalog_falls_back_to_german()
    {
        var store = new FakePreferenceStore("fr");

        var service = await CreateAsync(store);

        Assert.Equal("de", service.CurrentCulture.Name);
    }

    [Fact]
    public async Task Failed_persistence_keeps_the_language_active_but_reports_false()
    {
        var store = new FakePreferenceStore { FailOnSave = true };
        var service = await CreateAsync(store);

        var result = await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));

        Assert.False(result);
        Assert.Equal("en", service.CurrentCulture.Name);
    }

    [Fact]
    public async Task Setting_the_same_culture_twice_does_not_raise_events_or_save_again()
    {
        var store = new FakePreferenceStore();
        var service = await CreateAsync(store);
        var raised = 0;
        service.CultureChanged += (_, _) => raised++;

        var result = await service.SetCultureAsync(CultureInfo.GetCultureInfo("de"));

        Assert.True(result);
        Assert.Equal(0, raised);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task Null_culture_is_rejected()
    {
        var service = await CreateAsync();

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.SetCultureAsync(null!));
    }

    [Fact]
    public async Task Catalog_without_fallback_language_is_refused_at_startup()
    {
        var catalog = new FakeCatalog().With("en", ("a.b", "text"));

        var exception = await Assert.ThrowsAsync<TranslationCatalogException>(
            () => JsonLocalizationService.CreateAsync(catalog, new FakePreferenceStore()));

        Assert.Contains("Rueckfallsprache", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Null_arguments_are_rejected()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => JsonLocalizationService.CreateAsync(null!, new FakePreferenceStore()));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => JsonLocalizationService.CreateAsync(new FakeCatalog().With("de"), null!));
    }

    // --- LEERER DATENSATZ -----------------------------------------------

    [Fact]
    public async Task Empty_german_catalog_still_starts_and_returns_keys()
    {
        var catalog = new FakeCatalog().With("de");
        var service = await CreateAsync(catalog: catalog);

        Assert.Equal("de", service.CurrentCulture.Name);
        Assert.Equal("irgendein.schluessel", service["irgendein.schluessel"]);
    }

    // --- ABBRUCH ---------------------------------------------------------

    [Fact]
    public async Task Cancellation_during_startup_is_propagated()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => JsonLocalizationService.CreateAsync(
                EmbeddedJsonTranslationCatalog.Load(),
                new CancellingPreferenceStore(),
                cancellationToken: cts.Token));
    }

    // --- PARALLELITAET / GROSSE LAST -------------------------------------

    [Fact]
    public async Task Concurrent_reads_during_a_language_switch_never_throw()
    {
        var service = await CreateAsync();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        var readers = Enumerable.Range(0, 4).Select(_ => Task.Run(() =>
        {
            while (!cts.IsCancellationRequested)
            {
                var text = service[TranslationKeys.DeviceConnected];
                Assert.False(string.IsNullOrEmpty(text));
            }
        })).ToArray();

        while (!cts.IsCancellationRequested)
        {
            await service.SetCultureAsync(CultureInfo.GetCultureInfo("en"));
            await service.SetCultureAsync(CultureInfo.GetCultureInfo("de"));
        }

        await Task.WhenAll(readers);
    }

    [Fact]
    public async Task Resolving_many_texts_stays_fast()
    {
        // §73: Die Oberflaeche darf durch Textaufloesung nicht ausgebremst werden.
        var service = await CreateAsync();
        var keys = EmbeddedJsonTranslationCatalog.Load().GetKeys(CultureInfo.GetCultureInfo("de")).ToArray();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (var round = 0; round < 1_000; round++)
        {
            foreach (var key in keys)
            {
                _ = service[key];
            }
        }

        stopwatch.Stop();
        Assert.True(
            stopwatch.ElapsedMilliseconds < 5_000,
            $"{keys.Length * 1_000} Aufloesungen dauerten {stopwatch.ElapsedMilliseconds} ms.");
    }

    private sealed class CancellingPreferenceStore : ILanguagePreferenceStore
    {
        public Task<string?> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<string?>(null);
        }

        public Task<bool> SaveAsync(string cultureName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(true);
        }
    }
}
