using System.ComponentModel;
using System.Globalization;
using AndroidSuite.Core.Localization;

namespace AndroidSuite.Localization;

/// <summary>
/// Standardimplementierung von <see cref="ILocalizationService"/> (§2, §46).
/// Deutsch ist Standard- und Rueckfallsprache; der Wechsel erfolgt zur Laufzeit ohne Neustart.
/// </summary>
public sealed class JsonLocalizationService : ILocalizationService
{
    /// <summary>Name der Indexer-Eigenschaft, auf die Oberflaechen-Bindungen horchen.</summary>
    public const string IndexerName = "Item[]";

    /// <summary>Standardsprache beim Erststart (§2: "Standard beim ersten Start: DEUTSCH").</summary>
    public static CultureInfo GermanCulture => CultureInfo.GetCultureInfo("de");

    private readonly ITranslationCatalog _catalog;
    private readonly ILanguagePreferenceStore _preferenceStore;
    private readonly CultureInfo _fallbackCulture;
    private readonly object _gate = new();

    private CultureInfo _currentCulture;

    private JsonLocalizationService(
        ITranslationCatalog catalog,
        ILanguagePreferenceStore preferenceStore,
        CultureInfo fallbackCulture,
        CultureInfo initialCulture)
    {
        _catalog = catalog;
        _preferenceStore = preferenceStore;
        _fallbackCulture = fallbackCulture;
        _currentCulture = initialCulture;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    /// <inheritdoc />
    public CultureInfo CurrentCulture
    {
        get
        {
            lock (_gate)
            {
                return _currentCulture;
            }
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<CultureInfo> AvailableCultures => _catalog.AvailableCultures;

    /// <inheritdoc />
    public string this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            var culture = CurrentCulture;
            if (_catalog.TryGetString(culture, key, out var value))
            {
                return value;
            }

            if (!string.Equals(culture.Name, _fallbackCulture.Name, StringComparison.OrdinalIgnoreCase) &&
                _catalog.TryGetString(_fallbackCulture, key, out var fallbackValue))
            {
                return fallbackValue;
            }

            // Bewusst der Schluesselname statt leerem Text: ein fehlender Schluessel muss in der
            // Oberflaeche sichtbar sein, darf aber niemals zum Absturz fuehren.
            return key;
        }
    }

    /// <summary>
    /// Erzeugt den Dienst und stellt die zuletzt gespeicherte Sprache wieder her.
    /// Liegt keine gespeicherte Auswahl vor oder ist sie unbekannt, wird Deutsch aktiviert.
    /// </summary>
    /// <param name="catalog">Quelle der Uebersetzungen.</param>
    /// <param name="preferenceStore">Dauerhafte Ablage der Sprachwahl.</param>
    /// <param name="fallbackCulture">Rueckfallsprache; Vorgabe ist Deutsch.</param>
    /// <param name="cancellationToken">Abbruchsteuerung.</param>
    /// <exception cref="ArgumentNullException">Wenn Katalog oder Ablage <c>null</c> sind.</exception>
    /// <exception cref="TranslationCatalogException">Wenn der Katalog die Rueckfallsprache nicht kennt.</exception>
    public static async Task<JsonLocalizationService> CreateAsync(
        ITranslationCatalog catalog,
        ILanguagePreferenceStore preferenceStore,
        CultureInfo? fallbackCulture = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(preferenceStore);

        var fallback = fallbackCulture ?? GermanCulture;

        if (!ContainsCulture(catalog.AvailableCultures, fallback, out var resolvedFallback))
        {
            throw new TranslationCatalogException(
                $"Der Katalog enthaelt die Rueckfallsprache '{fallback.Name}' nicht. " +
                "Ohne Rueckfallsprache kann keine Oberflaeche garantiert angezeigt werden.");
        }

        var initial = resolvedFallback;

        var storedName = await preferenceStore.LoadAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(storedName))
        {
            CultureInfo? stored = null;
            try
            {
                stored = CultureInfo.GetCultureInfo(storedName);
            }
            catch (CultureNotFoundException)
            {
                // Beschaedigte Einstellung darf den Start nicht verhindern -> Standardsprache.
                stored = null;
            }

            if (stored is not null && ContainsCulture(catalog.AvailableCultures, stored, out var resolvedStored))
            {
                initial = resolvedStored;
            }
        }

        return new JsonLocalizationService(catalog, preferenceStore, resolvedFallback, initial);
    }

    /// <inheritdoc />
    public bool TryGetString(string key, out string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            value = string.Empty;
            return false;
        }

        return _catalog.TryGetString(CurrentCulture, key, out value);
    }

    /// <inheritdoc />
    public string Format(string key, params object?[] args)
    {
        var template = this[key];

        if (args is null || args.Length == 0)
        {
            return template;
        }

        try
        {
            return string.Format(CurrentCulture, template, args);
        }
        catch (FormatException)
        {
            // Eine fehlerhafte Vorlage darf die Oberflaeche nicht zum Absturz bringen (§75).
            // Der Vollstaendigkeitstest der Kataloge faengt solche Faelle vor dem Release ab.
            return template;
        }
    }

    /// <inheritdoc />
    public Task<bool> SetCultureAsync(CultureInfo culture, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return SetCultureCoreAsync(culture, cancellationToken);
    }

    private static bool ContainsCulture(
        IReadOnlyList<CultureInfo> cultures,
        CultureInfo wanted,
        out CultureInfo resolved)
    {
        foreach (var candidate in cultures)
        {
            if (string.Equals(candidate.Name, wanted.Name, StringComparison.OrdinalIgnoreCase))
            {
                resolved = candidate;
                return true;
            }
        }

        // "de-DE" darf den neutralen Katalog "de" verwenden.
        foreach (var candidate in cultures)
        {
            if (string.Equals(candidate.Name, wanted.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
            {
                resolved = candidate;
                return true;
            }
        }

        resolved = wanted;
        return false;
    }

    private async Task<bool> SetCultureCoreAsync(CultureInfo culture, CancellationToken cancellationToken)
    {
        if (!ContainsCulture(_catalog.AvailableCultures, culture, out var resolved))
        {
            // Unbekannte Sprache: bisherige Sprache bleibt aktiv, kein stiller Wechsel auf Englisch.
            return false;
        }

        CultureInfo previous;
        lock (_gate)
        {
            previous = _currentCulture;
            if (string.Equals(previous.Name, resolved.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            _currentCulture = resolved;
        }

        // Fehlschlagendes Speichern darf den Wechsel in der laufenden Sitzung nicht rueckgaengig machen;
        // es wird als Rueckgabewert gemeldet und vom Aufrufer protokolliert.
        var persisted = await _preferenceStore.SaveAsync(resolved.Name, cancellationToken).ConfigureAwait(false);

        RaisePropertyChanged(nameof(CurrentCulture));
        RaisePropertyChanged(IndexerName);
        CultureChanged?.Invoke(this, new CultureChangedEventArgs(previous, resolved));

        return persisted;
    }

    private void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
