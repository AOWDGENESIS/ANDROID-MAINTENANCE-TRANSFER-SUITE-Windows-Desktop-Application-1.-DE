using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using AndroidSuite.Core.Localization;

namespace AndroidSuite.Localization;

/// <summary>
/// Speichert die Sprachwahl in der Einstellungsdatei der Anwendung
/// (Vorgabe <c>%APPDATA%\AndroidSuite\settings.json</c>).
/// </summary>
/// <remarks>
/// Eigenschaften anderer Module in derselben Datei bleiben erhalten; es wird ausschliesslich
/// der Schluessel <c>language</c> geschrieben. Das Schreiben erfolgt ueber eine temporaere
/// Datei und anschliessendes Ersetzen, damit ein Absturz waehrend des Schreibens keine
/// zerstoerte Einstellungsdatei hinterlaesst.
/// </remarks>
public sealed class JsonFileLanguagePreferenceStore : ILanguagePreferenceStore, IDisposable
{
    private const string LanguagePropertyName = "language";

    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    private readonly string _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    /// <summary>Erzeugt die Ablage fuer einen festen Dateipfad.</summary>
    /// <param name="filePath">Vollstaendiger Pfad zur Einstellungsdatei.</param>
    /// <exception cref="ArgumentException">Wenn der Pfad leer ist.</exception>
    public JsonFileLanguagePreferenceStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    /// <summary>Letzter aufgetretener Fehler beim Lesen oder Schreiben, sonst <c>null</c>.</summary>
    /// <remarks>
    /// Wird vom Aufrufer protokolliert. Die Ablage wirft nicht nach aussen, damit eine
    /// beschaedigte Einstellungsdatei nie den Programmstart verhindert.
    /// </remarks>
    public Exception? LastError { get; private set; }

    /// <summary>Ermittelt den Standardpfad der Einstellungsdatei im Benutzerprofil.</summary>
    /// <returns>Pfad unterhalb von <c>ApplicationData</c>.</returns>
    public static string GetDefaultPath()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
            "AndroidSuite",
            "settings.json");

    /// <summary>
    /// Pfad der Sicherungskopie, die beim Ueberschreiben einer beschaedigten
    /// Einstellungsdatei angelegt wurde, sonst <c>null</c>.
    /// </summary>
    public string? LastRecoveredBackupPath { get; private set; }

    /// <summary>Gibt die interne Zugriffssperre frei.</summary>
    public void Dispose() => _fileLock.Dispose();

    /// <inheritdoc />
    public async Task<string?> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(_filePath, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var root = JsonNode.Parse(json) as JsonObject;
            var value = root?[LanguagePropertyName]?.GetValue<string>();
            LastError = null;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException or FormatException)
        {
            // Beschaedigte oder nicht lesbare Einstellung -> Standardsprache, kein Startabbruch.
            LastError = ex;
            return null;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> SaveAsync(string cultureName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cultureName);

        await _fileLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var root = new JsonObject();
            if (File.Exists(_filePath))
            {
                var existing = await File.ReadAllTextAsync(_filePath, cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(existing))
                {
                    try
                    {
                        root = JsonNode.Parse(existing) as JsonObject ?? new JsonObject();
                    }
                    catch (JsonException)
                    {
                        // Eine beschaedigte Einstellungsdatei darf das Speichern nicht dauerhaft
                        // blockieren. Der alte Inhalt wird jedoch nicht stillschweigend verworfen,
                        // sondern als Sicherungskopie abgelegt (Grundsatz: kein Datenverlust).
                        root = new JsonObject();
                        LastRecoveredBackupPath = await TryBackupCorruptFileAsync(existing, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }

            root[LanguagePropertyName] = cultureName;

            var temporaryPath = _filePath + ".tmp";
            await File.WriteAllTextAsync(temporaryPath, root.ToJsonString(WriteOptions), cancellationToken)
                .ConfigureAwait(false);
            File.Move(temporaryPath, _filePath, overwrite: true);

            LastError = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or NotSupportedException)
        {
            LastError = ex;
            return false;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task<string?> TryBackupCorruptFileAsync(string content, CancellationToken cancellationToken)
    {
        try
        {
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            var backupPath = $"{_filePath}.corrupt-{stamp}.bak";
            await File.WriteAllTextAsync(backupPath, content, cancellationToken).ConfigureAwait(false);
            return backupPath;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Eine fehlgeschlagene Sicherungskopie darf das Speichern der Sprache nicht verhindern.
            return null;
        }
    }
}
