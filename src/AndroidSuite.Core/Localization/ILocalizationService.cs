using System.ComponentModel;
using System.Globalization;

namespace AndroidSuite.Core.Localization;

/// <summary>
/// Zentraler Lokalisierungsdienst (Master-Prompt §2, §46).
/// Jeder sichtbare Text der Anwendung MUSS ueber diesen Dienst aufgeloest werden.
/// Hartcodierte Benutzertexte sind unzulaessig.
/// </summary>
/// <remarks>
/// Der Dienst implementiert <see cref="INotifyPropertyChanged"/> und meldet bei einem
/// Sprachwechsel eine Aenderung des Indexers ("Item[]"). Dadurch aktualisieren sich
/// datengebundene Oberflaechen ohne Neustart der Anwendung.
/// </remarks>
public interface ILocalizationService : INotifyPropertyChanged
{
    /// <summary>Aktuell aktive Sprache. Standard beim Erststart ist Deutsch.</summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>Alle Sprachen, fuer die ein vollstaendiger Katalog vorliegt.</summary>
    IReadOnlyList<CultureInfo> AvailableCultures { get; }

    /// <summary>
    /// Liefert den uebersetzten Text zu <paramref name="key"/>.
    /// Fehlt der Schluessel, wird die Rueckfallsprache benutzt; fehlt er auch dort,
    /// wird der Schluesselname zurueckgegeben. Diese Eigenschaft wirft niemals.
    /// </summary>
    string this[string key] { get; }

    /// <summary>Liefert den uebersetzten Text ohne Rueckfall-Ersatz.</summary>
    /// <returns><c>true</c>, wenn der Schluessel in der aktuellen Sprache existiert.</returns>
    bool TryGetString(string key, out string value);

    /// <summary>
    /// Liefert den uebersetzten und mit <paramref name="args"/> formatierten Text.
    /// Passt die Platzhalteranzahl nicht, wird die unformatierte Vorlage zurueckgegeben,
    /// statt eine Ausnahme auszuloesen (eine Oberflaeche darf an einem Text nicht abstuerzen).
    /// </summary>
    string Format(string key, params object?[] args);

    /// <summary>
    /// Wechselt die Sprache zur Laufzeit und speichert die Auswahl dauerhaft (§2).
    /// Ist die Sprache unbekannt, bleibt die bisherige aktiv und es wird <c>false</c> geliefert.
    /// </summary>
    Task<bool> SetCultureAsync(CultureInfo culture, CancellationToken cancellationToken = default);

    /// <summary>Wird nach einem erfolgreichen Sprachwechsel ausgeloest.</summary>
    event EventHandler<CultureChangedEventArgs>? CultureChanged;
}
