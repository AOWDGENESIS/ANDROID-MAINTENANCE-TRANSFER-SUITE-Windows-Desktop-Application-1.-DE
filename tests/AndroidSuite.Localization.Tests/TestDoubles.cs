using System.Globalization;
using AndroidSuite.Core.Localization;

namespace AndroidSuite.Localization.Tests;

/// <summary>Katalog-Attrappe fuer Tests mit kontrollierten Inhalten (§62: keine echten Daten).</summary>
internal sealed class FakeCatalog : ITranslationCatalog
{
    private readonly Dictionary<string, Dictionary<string, string>> _data = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<CultureInfo> AvailableCultures { get; private set; } = [];

    public FakeCatalog With(string culture, params (string Key, string Value)[] entries)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in entries)
        {
            map[key] = value;
        }

        _data[culture] = map;
        AvailableCultures = [.. _data.Keys.Select(CultureInfo.GetCultureInfo)];
        return this;
    }

    public IReadOnlyCollection<string> GetKeys(CultureInfo culture)
        => _data.TryGetValue(culture.Name, out var map) ? map.Keys : [];

    public bool TryGetString(CultureInfo culture, string key, out string value)
    {
        if (_data.TryGetValue(culture.Name, out var map) && map.TryGetValue(key, out var found))
        {
            value = found;
            return true;
        }

        value = string.Empty;
        return false;
    }
}

/// <summary>Speicher-Attrappe: haelt die Sprachwahl im Arbeitsspeicher.</summary>
internal sealed class FakePreferenceStore(string? initial = null) : ILanguagePreferenceStore
{
    public string? Stored { get; private set; } = initial;

    public int SaveCount { get; private set; }

    public bool FailOnSave { get; set; }

    public Task<string?> LoadAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Stored);

    public Task<bool> SaveAsync(string cultureName, CancellationToken cancellationToken = default)
    {
        SaveCount++;
        if (FailOnSave)
        {
            return Task.FromResult(false);
        }

        Stored = cultureName;
        return Task.FromResult(true);
    }
}

/// <summary>Temporaerer Ordner, der nach dem Test wieder entfernt wird (§67 Workspace-Schutz).</summary>
internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "androidsuite-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public string File(string name) => System.IO.Path.Combine(Path, name);

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch (IOException)
        {
            // Aufraeumen darf einen Testlauf nicht zum Scheitern bringen.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
