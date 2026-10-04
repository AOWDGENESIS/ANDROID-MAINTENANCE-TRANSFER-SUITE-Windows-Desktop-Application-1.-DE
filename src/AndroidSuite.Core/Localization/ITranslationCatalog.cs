using System.Globalization;

namespace AndroidSuite.Core.Localization;

/// <summary>
/// Quelle der Uebersetzungstexte. Trennt den Lokalisierungsdienst von der Frage,
/// woher die Texte stammen (eingebettete Ressource, Datei, Test-Attrappe).
/// </summary>
public interface ITranslationCatalog
{
    /// <summary>Sprachen, fuer die dieser Katalog Texte liefert.</summary>
    IReadOnlyList<CultureInfo> AvailableCultures { get; }

    /// <summary>Alle Schluessel einer Sprache. Wird fuer die Vollstaendigkeitspruefung benoetigt.</summary>
    IReadOnlyCollection<string> GetKeys(CultureInfo culture);

    /// <summary>Sucht einen Text. Liefert <c>false</c>, wenn Sprache oder Schluessel fehlen.</summary>
    bool TryGetString(CultureInfo culture, string key, out string value);
}
