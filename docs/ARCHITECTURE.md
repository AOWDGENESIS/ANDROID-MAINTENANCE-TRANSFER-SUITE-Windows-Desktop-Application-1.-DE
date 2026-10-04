# ARCHITECTURE — Android Maintenance & Transfer Suite

Stand: 2026-10-04 · Version 0.1.0 · Zielplattform Windows 10/11 x64

---

## 1. Leitsätze

1. **Offline-First.** Keine Kernfunktion benötigt Internet. Keine Telemetrie. Keine Übertragung
   privater Inhalte an Dritte (§3).
2. **Nichts Destruktives ohne Evidenz, Vorschau und Zustimmung** (§4, §5).
3. **Stabilität vor Funktionsmenge** (§80). Lieber ein vollständiges Modul als fünf angefangene.
4. **Die UI enthält keine Logik.** Alles Fachliche ist headless testbar — sonst wäre das
   §60-Gate nicht beweisbar.
5. **Keine hartcodierten Texte.** Jede sichtbare Zeichenkette kommt aus `ILocalizationService`.
6. **Ehrlichkeit über Fähigkeiten.** Was Android nicht erlaubt, wird gemeldet, nicht umgangen (§6).

---

## 2. Schichten

```
┌──────────────────────────────────────────────────────────────┐
│  AndroidSuite.App            Avalonia 11, Views (XAML)       │  UI
├──────────────────────────────────────────────────────────────┤
│  AndroidSuite.ViewModels     MVVM, keine Fachlogik           │  Präsentation
├──────────────────────────────────────────────────────────────┤
│  AndroidSuite.Core           Domänenmodell, Interfaces,      │  Kern
│                              Regeln, Klassifikation          │  (100 % testbar,
│                              KEINE Plattform-/IO-Abhängigkeit)│   keine Win-API)
├──────────────────────────────────────────────────────────────┤
│  AndroidSuite.Infrastructure ADB, MTP/WPD, SQLite, Dateisystem│ Adapter
├──────────────────────────────────────────────────────────────┤
│  AndroidSuite.Localization   Ressourcen DE/EN, Live-Wechsel  │  Querschnitt
│  AndroidSuite.Diagnostics    Logging (Serilog), Health       │  Querschnitt
└──────────────────────────────────────────────────────────────┘
```

**Abhängigkeitsregel (per Test erzwungen):** Pfeile zeigen nur nach unten.
`Core` referenziert **kein** Infrastructure-Projekt, **kein** Avalonia, **kein** `System.IO`
für Gerätezugriffe. Verstöße werden durch einen Architekturtest erkannt, nicht durch Review.

### Projektstruktur
```
AndroidSuite.sln
├─ src/
│  ├─ AndroidSuite.Core/            netstandard-kompatibel, net8.0
│  ├─ AndroidSuite.Localization/    net8.0
│  ├─ AndroidSuite.Infrastructure/  net8.0-windows (ADB/WPD) + net8.0 (portabel)
│  ├─ AndroidSuite.ViewModels/      net8.0
│  └─ AndroidSuite.App/             net8.0, Avalonia, win-x64 publish
├─ tests/
│  ├─ AndroidSuite.Core.Tests/
│  ├─ AndroidSuite.Localization.Tests/
│  ├─ AndroidSuite.Infrastructure.Tests/
│  └─ AndroidSuite.Architecture.Tests/
├─ installer/   AndroidSuite.iss (Inno Setup)
├─ docs/
└─ scripts/
```

---

## 3. Komponenten (§55) und ihr MVP-Status

| Komponente | Interface | MVP | Phase |
|---|---|---|---|
| Localization | `ILocalizationService` | ✅ voll | 0 |
| Logging | `IAppLogger` | ✅ voll | 0 |
| Settings | `ISettingsStore` | ✅ voll | 0 |
| DeviceDiscovery | `IDeviceProvider` | ✅ voll | 1 |
| AdbEngine | `IAdbEngine` | ✅ voll | 1 |
| MtpEngine | `IMtpEngine` | ⚠ nur Erkennung | 1 / 4b |
| DeviceMap | `IDeviceMapStore` | ✅ voll | 2 |
| FileSystem | `IStorageProvider` | ✅ voll | 2 |
| MediaAnalyzer | `IMediaAnalyzer` | ✅ voll | 6 |
| MetadataEngine | `IMetadataProvider` | ✅ voll | 6 |
| TransferEngine / Queue | `ITransferBackend`, `ITransferQueue` | ✅ voll | 4 |
| BackupEngine | `IBackupProvider` | ✅ voll | 5 |
| RestoreEngine | `IRestoreProvider` | ✅ voll | 5 |
| StorageAnalyzer | `IStorageAnalyzer` | ✅ voll | 7 |
| CleanupEngine | `ICleanupProvider` | ✅ Dry-Run + Vorschau + Löschen | 8 |
| DuplicateEngine | `IDuplicateFinder` | ✅ nur anzeigen (§31) | 8 |
| ApkManager | `IAppManager` | ⚠ nur Erkennung | 8 / 11 |
| WhatsAppCleaner | `IMessengerCleaner` | ❌ Gerüst | 9 |
| Telegram/SignalCleaner | `IMessengerCleaner` | ❌ Gerüst | 10 |
| SecurityEngine | `ISecurityScanner` | ⚠ Schutzregeln aktiv, Scan später | 12 |
| LiveScreen | `ILiveScreenProvider` | ❌ Gerüst, UI „später verfügbar" | 13 |
| HealthEngine / Diagnostic | `IHealthProvider` | ⚠ Basisdiagnose | 14 |
| SyncEngine | `ISyncProvider` | ❌ Gerüst | nach MVP |
| UpdateManager | `IUpdateService` | ❌ Gerüst (offline-first: manuell) | nach MVP |
| Installer | — | ✅ Inno Setup | 16 |

Nicht freigeschaltete Module erscheinen in der Navigation **sichtbar deaktiviert** mit dem
lokalisierten Hinweis „Später verfügbar" (§43) — keine blinden Menüpunkte, keine Attrappen,
die so tun, als würden sie etwas tun.

---

## 4. Kernmechanismus: Evidenzbasierte Klassifikation (§4, §12)

Das Herzstück der Sicherheitsarchitektur. Keine Kategorie entsteht aus einem einzelnen Signal.

```csharp
public sealed record ClassificationEvidence(
    EvidenceKind Kind,     // Extension, Signature, MimeType, Path, FileName,
                           // Metadata, ContainerContent, Size, Duration
    string Detail,         // z.B. "Pfad enthält /WhatsApp/Media/WhatsApp Voice Notes"
    double Weight,         // -1.0 .. +1.0
    bool IsDecisive);      // echter Nachweis vs. bloßes Indiz

public sealed record ClassificationResult(
    MediaCategory Category,
    double Confidence,                         // 0..1
    IReadOnlyList<ClassificationEvidence> Evidence,
    MediaOrigin Origin,                        // Unknown, Camera, Download, WhatsApp, …
    bool IsAnimated,
    bool RequiresUserReview);                  // true bei Confidence < Schwelle
```

### Unverhandelbare Regeln (als Unit-Tests fixiert)
* **R1** — Eine Datei erhält **nie** die Kategorie `WhatsAppMedia`, `TelegramMedia`,
  `SignalMedia` oder `VoiceMessage`, wenn kein Beweis mit `IsDecisive = true` aus Pfad,
  Dateiname oder Containerstruktur vorliegt. Endung allein ist **niemals** `IsDecisive`.
* **R2** — Kein destruktiver Vorgang darf eine Datei mit `RequiresUserReview = true`
  **vorausgewählt** in eine Löschliste aufnehmen.
* **R3** — Widersprechen sich Signatur und Endung (`.mp3` mit PNG-Signatur), sinkt die Konfidenz
  und die Datei wird als `Anomalie` markiert, nie stillschweigend zugeordnet.
* **R4** — `MediaCategory.Other` ist zulässig; eine **falsche** Kategorie ist es nicht.
  Im Zweifel: `Other` + Review-Flag.

Kategorien (§11): `Image, Video, Animation, Music, Podcast, Audiobook, VoiceRecording,
VoiceMessage, WhatsAppMedia, TelegramMedia, SignalMedia, Download, Document, Apk, Archive,
Cache, Other`.

---

## 5. Transfer-Architektur (§16–§20)

```csharp
public interface ITransferBackend {
    TransferCapabilities Capabilities { get; }   // CanResume, CanHash, CanPreserveTimestamps …
    Task<TransferResult> TransferAsync(TransferItem item, IProgress<TransferProgress> p,
                                       CancellationToken ct);
}
```
Implementierungen: `WindowsFileSystemBackend` (Robocopy erlaubt) ·
`AndroidAdbBackend` · `AndroidMtpBackend` (Phase 4b).

**Guard (§17, aus Feasibility-Befund 4):** `WindowsFileSystemBackend` wirft
`InvalidOperationException`, wenn Quelle oder Ziel kein gültiger Win32-Pfad ist. Robocopy kann
dadurch konstruktiv nicht auf ein Android-Gerät losgelassen werden. Dazu ein expliziter
Negativtest.

**`TransferCapabilities` statt Versprechen:** Die UI zeigt Pause/Resume/Hash nur an, wenn das
Backend sie wirklich kann. Keine ausgegrauten Lügen, keine toten Buttons.

Queue-Zustände (§20): `Waiting → Running → (Paused) → Completed | Failed | Cancelled`.
Parallelität Standard 4 (§18), adaptiv begrenzt: bei ADB-Backend **1 Kanal pro Gerät** für
kleine Dateien (Roundtrip-dominiert), parallel nur über mehrere Geräte bzw. große Dateien.
Kein blindes Hochdrehen.

**Verifikation (§74):** Nach jedem Transfer: Zielgröße == Quellgröße (immer), optional Hash.
Erfolgsmeldung **erst** nach bestandener Verifikation — sonst `Failed`.

---

## 6. Destruktiver Pfad (§5) — als Typsystem, nicht als Konvention

```
IAnalysisStage → ISafetyStage → ITargetStage → IPreviewStage
   → IConfirmationStage → IExecutionStage → IVerificationStage → IAuditStage
```
`DeletionPlan` entsteht nur über diese Kette. `IExecutionStage.Execute` nimmt ausschließlich
ein `ConfirmedDeletionPlan` entgegen, und dieser Typ kann nur von `IConfirmationStage` erzeugt
werden (interner Konstruktor). **Ein Löschvorgang ohne Bestätigung ist damit nicht
kompilierbar** — nicht nur „nicht vorgesehen".

Schutzmechanismen:
* `ProtectedPathRegistry` (§30): Benutzerordner + eingebaute Standardschutzpfade
  (`/DCIM/Camera`, `/Music`, `/Documents`, Backup-Ziele). Geschützte Elemente erscheinen in der
  Vorschau sichtbar markiert und **nicht vorausgewählt**.
* `BackupBeforeDelete` (§28): Löschen wird erst freigegeben, wenn das Backup verifiziert ist.
* „Verschieben statt Löschen" (§29) ist in der UI die **Standardaktion**, Löschen die Ausnahme.

---

## 7. Lokalisierung (§2, §46)

```csharp
public interface ILocalizationService : INotifyPropertyChanged {
    CultureInfo CurrentCulture { get; }
    IReadOnlyList<CultureInfo> AvailableCultures { get; }
    string this[string key] { get; }
    string Format(string key, params object[] args);
    Task SetCultureAsync(CultureInfo culture);     // persistiert
    event EventHandler<CultureChangedEventArgs> CultureChanged;
}
```
* Ressourcen als **JSON** (`de.json`, `en.json`), nicht `.resx` — diffbar, zur Laufzeit
  nachladbar, einfach auf Vollständigkeit testbar.
* **Live-Wechsel ohne Neustart:** Views binden an einen Indexer-Proxy; bei Kulturwechsel wird
  `PropertyChanged(Binding.IndexerName)` ausgelöst → alle Bindings aktualisieren sich.
* **Test-Gate:** Ein Testlauf vergleicht die Schlüsselmengen von `de.json` und `en.json`.
  Fehlende oder überzählige Schlüssel = **FAIL**, nicht Warnung.
* Standard beim Erststart: **Deutsch**, persistiert in `%APPDATA%\AndroidSuite\settings.json`.
* Fallback-Kette: angeforderte Kultur → Deutsch → Schlüsselname (nie leerer Text, nie Absturz).

---

## 8. Nebenläufigkeit & Performance (§73)

* Alle IO- und Analysevorgänge `async`, kein `.Result`, kein `.Wait()` (Architekturtest).
* UI-Thread führt keine Dateioperation aus. Scans laufen in `Channel<T>`-Pipelines mit
  begrenzter Parallelität.
* Geräteindex in SQLite, UI paginiert; Listen virtualisiert.
* Inkrementeller Scan über `(Pfad, Größe, mtime)` — kein Vollscan bei jeder Änderung (§8).
* **STOP ALL (§54):** Ein globaler `CancellationTokenSource` ist Wurzel aller Operationen.
  Jede lange Operation muss ihn respektieren; ein Test prüft, dass registrierte Jobs innerhalb
  von 2 Sekunden in den Zustand `Cancelled` übergehen.

---

## 9. Logging (§53)

Serilog, getrennte Dateisenken: `connection`, `transfer`, `cleanup`, `security`, `error`,
`diagnostic`, `backup`, `restore`, `installer`. Rotation täglich, Aufbewahrung 14 Dateien,
Obergrenze pro Datei 10 MB. Ablage: `%APPDATA%\AndroidSuite\logs\`.
**Nie geloggt:** Passwörter, Tokens, Dateiinhalte, Nachrichtentexte, Kontakte. Dateipfade werden
geloggt (für Nachvollziehbarkeit nötig), auf Wunsch pseudonymisierbar.

---

## 10. Fehlerbehandlung (§75)

```csharp
public sealed record UserFacingError(
    string TitleKey, string WhatHappenedKey, string WhatItMeansKey, string WhatToDoKey,
    string? TechnicalDetail, ErrorSeverity Severity, string ErrorCode);
```
Benutzer sieht drei lokalisierte Sätze; technische Details stehen aufklappbar darunter und im
Log. Fehlercodes sind stabil (`ADB-UNAUTHORIZED`, `MTP-NO-WIN32PATH`,
`CLEANUP-PROTECTED-TARGET`, …) und über Sprachen hinweg identisch.
