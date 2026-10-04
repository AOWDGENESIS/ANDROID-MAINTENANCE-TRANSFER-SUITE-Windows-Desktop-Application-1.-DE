namespace AndroidSuite.Localization;

/// <summary>
/// Fehler beim Laden eines Sprachkatalogs. Tritt nur bei fehlerhaften Build-Artefakten auf
/// und wird deshalb bewusst als Ausnahme gemeldet statt still verschluckt zu werden.
/// </summary>
public sealed class TranslationCatalogException : Exception
{
    /// <summary>Erzeugt eine Ausnahme ohne naehere Angabe.</summary>
    public TranslationCatalogException()
        : base("Der Sprachkatalog konnte nicht geladen werden.")
    {
    }

    /// <summary>Erzeugt eine Ausnahme mit Beschreibung.</summary>
    /// <param name="message">Beschreibung des Fehlers.</param>
    public TranslationCatalogException(string message)
        : base(message)
    {
    }

    /// <summary>Erzeugt eine Ausnahme mit Beschreibung und Ursache.</summary>
    /// <param name="message">Beschreibung des Fehlers.</param>
    /// <param name="innerException">Ausloesende Ausnahme.</param>
    public TranslationCatalogException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
