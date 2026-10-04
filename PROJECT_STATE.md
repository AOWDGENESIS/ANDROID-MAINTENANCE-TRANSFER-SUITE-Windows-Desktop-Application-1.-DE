# PROJECT_STATE

Letzte Aktualisierung: 2026-10-04 · Version 0.1.0-dev

## GOAL
Eine stabile, sichere, installierbare Windows-10/11-x64-Anwendung zur Verwaltung von
Android-Geräten: Erkennung, Dateiverwaltung, Übertragung, Backup/Restore, Speicheranalyse und
Cleanup Center — offline-first, deutschsprachig mit Live-Umschaltung auf Englisch, mit frei
wählbarem Installationspfad und strengem Release-Gate.

## CURRENT_PHASE
Phase 0 — Analyse, Architektur, Projektgerüst, erste Komponente.

## COMPLETED
* Umgebungsanalyse: Debian-Buildhost, .NET 8.0.425, Netzzugang, **WPF/WinUI hier nicht baubar**.
* Technische Machbarkeitsprüfung mit 6 dokumentierten Annahme-Korrekturen.
* `docs/TECHNICAL_FEASIBILITY.md`, `ARCHITECTURE.md`, `SECURITY_MODEL.md`, `ROADMAP.md`,
  `TEST_PLAN.md`, `RELEASE_CHECKLIST.md`.
* Solution-Gerüst: `AndroidSuite.Core`, `AndroidSuite.Localization`, Testprojekt, Smoke-Runner.
* Zero-Warning-Gate aktiv (`TreatWarningsAsErrors`, Analyzer `latest-recommended`, Nullable).
* **Komponente 1 — Localization: APPROVED** (siehe `docs/CHECKPOINTS.md`).
* Workspace-Schutz: Toolchain nach `.cache/` ausgelagert, `scripts/clean-workspace.sh`.

## IN_PROGRESS
Nichts. Gate-Stopp nach Komponente 1 gemäß §60.

## DECISIONS
| ID | Entscheidung | Begründung |
|---|---|---|
| D-01 | **Avalonia 11 + .NET 8** statt WPF/WinUI | nur so sind BUILD/RUNTIME-Tests auf diesem Host nachweisbar (§60, §72); erzeugt trotzdem native Win-x64-EXE |
| D-02 | **Inno Setup** als Installer | freie Pfadwahl, Per-User-Installation ohne Admin, DE/EN-Setup |
| D-03 | ADB mitliefern, Pfad überschreibbar | Offline-First; **offener Rechtspunkt LEGAL-001** |
| D-04 | Robocopy **nur** für Win32-Pfade, nie für Android | MTP hat keinen Win32-Pfad (Feasibility-Befund 4) |
| D-05 | MTP im MVP nur Erkennung, Transfer über ADB | Stabilität vor Funktionsmenge (§80) |
| D-06 | Resume nur auf **Datei**-Ebene | ADB kann kein Byte-Resume — keine falschen Versprechen (§19) |
| D-07 | Kein „Secure Erase" auf Flash | technisch nicht garantierbar (§27) |
| D-08 | Lokalisierung als **JSON**, nicht RESX | diffbar, zur Laufzeit prüfbar, einfache Vollständigkeitstests |
| D-09 | Löschkette über Typsystem erzwungen | `ConfirmedDeletionPlan` nur durch Bestätigungsstufe erzeugbar (§5) |
| D-10 | Toolchain unter `.cache/`, nicht im Projekt | §67 Workspace-Schutz |

## BLOCKERS
Keine technischen Blocker.

## OPEN_ISSUES
| ID | Thema | Fällig |
|---|---|---|
| LEGAL-001 | Android SDK ToS §3.4 untersagt Weiterverteilung der Platform-Tools. Bündelung ist für den privaten Eigengebrauch vorgesehen; für eine öffentliche Verteilung muss auf den Download-Assistenten umgestellt werden. | vor Release (Checkliste Nr. 15) |
| PENDING-WIN-001 | Installer-, Hardware- und WPD-Tests sind auf diesem Host nicht lauffähig → Status `PENDING-WINDOWS`, niemals `PASS`. | Phase 16/17 |

## KNOWN_BUGS
Keine offenen. Behoben: **BUG-001** — eine beschädigte `settings.json` blockierte dauerhaft das
Speichern der Sprachwahl. Ursache: `JsonException` im Schreibpfad. Behebung: Wiederherstellung mit
Sicherungskopie der beschädigten Datei. Regressionstest vorhanden.

## TEST_STATUS
| Stufe | Ergebnis |
|---|---|
| BUILD (Release) | PASS — 0 Fehler, 0 Warnungen |
| UNIT/INTEGRATION | PASS — 57/57 |
| RUNTIME (isoliert) | PASS — 12/12 Prüfpunkte |
| WINDOWS-ARTEFAKT | PASS — PE32+ x64 erzeugt, Hash reproduzierbar identisch |
| SECURITY (Komponente 1) | PASS — keine Pfad-/Prozess-/Geheimnisoberfläche |
| REGRESSION | PASS — vollständiger Lauf nach jeder Änderung |
| Installer / echtes Gerät | PENDING-WINDOWS |

Reproduktion: `bash scripts/build-and-test.sh --windows-artifact`

## NEXT_STEP
**Komponente 2 — Querschnittsdienste (Phase 0b):** `IAppLogger` mit getrennten Log-Senken (§53),
`ISettingsStore`, `UserFacingError` (§75) und die globale Notbremse `STOP ALL` (§54).
Danach Gate, dann Phase 1 (DeviceDiscovery + AdbEngine).

## RELEASE_STATUS
NOT RELEASABLE — 1 von 16 Release-Gates erfüllt (siehe `docs/RELEASE_CHECKLIST.md`).

**Vorabveröffentlichung v0.1.0-alpha.1 vorbereitet** (kein Produktrelease):
* Ziel: `AOWDGENESIS/ANDROID-MAINTENANCE-TRANSFER-SUITE-Windows-Desktop-Application-1.-DE`
* Kennzeichnung als „im Aufbau": Pre-Release-Flag, `-alpha`-Version, 🚧-Titel, Warnkasten in
  README und Release-Text, Statusabzeichen, `KNOWN_LIMITATIONS.md`
* Anhänge: `release/ANDROID-SUITE-v0.1.0-alpha.1-quellen.zip` + `SHA256SUMS.txt`
* **Bewusst ohne EXE/Installer** — es existiert noch keine Anwendung
* Anleitung: `docs/releases/ANLEITUNG_VEROEFFENTLICHUNG.md`
* Übertragung muss der Benutzer auslösen (keine GitHub-Zugangsdaten im Arbeitsbereich)
