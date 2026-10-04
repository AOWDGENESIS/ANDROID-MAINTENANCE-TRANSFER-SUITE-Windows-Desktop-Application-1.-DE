# CHECKPOINTS

Protokoll der Komponenten-Gates nach §60/§70. Eine Komponente gilt erst als abgeschlossen,
wenn **alle** Zeilen PASS sind. Nicht ausführbare Prüfungen erhalten `PENDING-WINDOWS`,
niemals `PASS`.

---

## Checkpoint 001 — Localization
Datum: 2026-10-04 · Phase 0

```
COMPONENT:  Localization (ILocalizationService, Kataloge DE/EN, dauerhafte Sprachwahl)
BUILD:      PASS   (dotnet build -c Release: 0 Fehler, 0 Warnungen)
TESTS:      PASS   (57/57; Unit, Integration, Nebenläufigkeit, Performance)
WARNINGS:   0      (akzeptiert: 2, dokumentiert in docs/ACCEPTED_WARNINGS.md)
SECURITY:   PASS   (keine Dateipfad-Eingaben von außen, keine Prozessaufrufe,
                    keine Geheimnisse, beschädigte Eingaben führen nie zum Abbruch)
REGRESSION: PASS   (vollständiger Lauf nach jeder Korrektur)
RUNTIME:    PASS   (12/12 Prüfpunkte im isolierten Testlauf)
ARTEFAKT:   PASS   (win-x64 PE32+ erzeugt; identischer SHA-256 bei Wiederholung
                    → reproduzierbarer Build nach §77)
RESULT:     APPROVED
NEXT:       Querschnittsdienste (Logging, Settings, UserFacingError, STOP ALL)
```

### Abgedeckte Anforderungen
§2 (Lokalisierungssystem, Deutsch als Standard, dauerhafte Speicherung) ·
§46 (Live-Wechsel ohne Neustart, keine hartcodierten Texte) ·
§61 (Zero-Warning-Gate) · §63 (Testmatrix: Normal, Fehler, Abbruch, leerer Datensatz,
große Last, fehlende Datei, Ziel nicht verfügbar, fehlende Berechtigung) ·
§67 (Workspace-Schutz) · §75 (Fehlertexte dreiteilig vorbereitet)

### Gefundene und behobene Fehler
| ID | Fund | Ursache | Behebung |
|---|---|---|---|
| BUG-001 | Speichern der Sprache schlug bei beschädigter `settings.json` dauerhaft fehl | `JsonException` im Schreibpfad wurde als Schreibfehler behandelt | Wiederherstellung mit Sicherungskopie `settings.json.corrupt-<Zeit>.bak`, Regressionstest ergänzt |
| CA1001 | `SemaphoreSlim` ohne `IDisposable` | Analyzer-Befund im Build | `IDisposable` implementiert |
| CA1416 / CA1861 | Plattform- und Allokationsbefunde im Testprojekt | — | Plattformabfrage ergänzt, Array als `static readonly` |

Keine Warnung wurde unterdrückt, kein Test entfernt, kein Timeout erhöht (§72).

### Offen (bewusst nicht in dieser Komponente)
Bindung an die Avalonia-Oberfläche (Phase 15), Setup-Sprachen im Installer (Phase 16),
Prüfung auf hartcodierte Texte in ViewModels (Architekturtest, sobald ViewModels existieren).
