using System.Globalization;

namespace AndroidSuite.Core.Localization;

/// <summary>Meldet einen zur Laufzeit durchgefuehrten Sprachwechsel (§46).</summary>
/// <param name="previousCulture">Bis dahin aktive Sprache.</param>
/// <param name="currentCulture">Neu aktivierte Sprache.</param>
public sealed class CultureChangedEventArgs(CultureInfo previousCulture, CultureInfo currentCulture)
    : EventArgs
{
    /// <summary>Bis zum Wechsel aktive Sprache.</summary>
    public CultureInfo PreviousCulture { get; } = previousCulture;

    /// <summary>Nach dem Wechsel aktive Sprache.</summary>
    public CultureInfo CurrentCulture { get; } = currentCulture;
}
