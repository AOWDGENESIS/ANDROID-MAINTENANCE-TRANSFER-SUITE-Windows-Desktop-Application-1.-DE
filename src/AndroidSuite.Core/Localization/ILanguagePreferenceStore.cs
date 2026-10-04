namespace AndroidSuite.Core.Localization;

/// <summary>
/// Dauerhafte Ablage der Sprachwahl (§2: "Die Spracheinstellung muss dauerhaft gespeichert werden").
/// Die Ablage darf niemals eine Ausnahme nach aussen werfen - eine nicht lesbare
/// Einstellungsdatei fuehrt zur Standardsprache, nicht zum Programmabbruch.
/// </summary>
public interface ILanguagePreferenceStore
{
    /// <summary>Liest den gespeicherten Sprachnamen (z. B. "de"), oder <c>null</c> beim Erststart.</summary>
    Task<string?> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>Speichert den Sprachnamen dauerhaft.</summary>
    /// <returns><c>true</c>, wenn gespeichert werden konnte.</returns>
    Task<bool> SaveAsync(string cultureName, CancellationToken cancellationToken = default);
}
