<!-- Statusbanner: muss bei jedem Release aktualisiert werden. -->

# 🚧 ANDROID MAINTENANCE & TRANSFER SUITE

> ## ⚠️ DIESES PROGRAMM IST IM AUFBAU — ES GIBT NOCH KEINE BENUTZBARE ANWENDUNG
>
> **Stand: Phase 0 von 18 · Version 0.1.0-alpha.1 · 4. Oktober 2026**
>
> Dieses Repository enthält zurzeit **Fundament und Dokumentation**, nicht das fertige Programm.
> Es gibt **keine EXE**, **keinen Installer**, **keine Oberfläche** und **keine Gerätefunktionen**.
> Wer hier etwas herunterlädt, kann damit **noch kein Android-Gerät verwalten**.
>
> **Bitte nicht als fertige Software verwenden oder weitergeben.**

![Status](https://img.shields.io/badge/Status-In%20Entwicklung-orange?style=for-the-badge)
![Phase](https://img.shields.io/badge/Phase-0%20von%2018-blue?style=for-the-badge)
![Release](https://img.shields.io/badge/Release-NICHT%20FREIGEGEBEN-red?style=for-the-badge)
![Installer](https://img.shields.io/badge/Installer-noch%20nicht%20vorhanden-lightgrey?style=for-the-badge)
![Tests](https://img.shields.io/badge/Tests-57%2F57%20bestanden-success?style=flat-square)
![Warnungen](https://img.shields.io/badge/Warnungen-0-success?style=flat-square)
![Plattform](https://img.shields.io/badge/Zielplattform-Windows%2010%2F11%20x64-informational?style=flat-square)
![Sprachen](https://img.shields.io/badge/Sprachen-DE%20%7C%20EN-informational?style=flat-square)

---

## Was das hier einmal werden soll

Eine Windows-10/11-x64-Desktopanwendung zur sicheren Verwaltung von Android-Smartphones und
-Tablets: Geräteerkennung, Dateiverwaltung, Übertragung, Backup/Restore, Speicheranalyse und
Cleanup Center. **Offline-first** — keine Cloud, keine Telemetrie, keine Übertragung privater
Daten. Oberfläche Deutsch (Standard) und Englisch, zur Laufzeit umschaltbar.

## Was heute tatsächlich fertig und geprüft ist

| Komponente | Status | Nachweis |
|---|---|---|
| Phase-0-Dokumentation (Architektur, Machbarkeit, Sicherheitsmodell, Testplan, Roadmap, Release-Checkliste) | ✅ fertig | [`docs/`](docs/) |
| Projektgerüst, Zero-Warning-Build, Testinfrastruktur | ✅ fertig | [`scripts/build-and-test.sh`](scripts/build-and-test.sh) |
| GitHub-Actions-CI | ⏳ noch einzurichten | [`docs/ci/`](docs/ci/) |
| **Komponente 1: Lokalisierung** (DE/EN, Live-Wechsel, dauerhafte Speicherung) | ✅ **freigegeben** | [Checkpoint 001](docs/CHECKPOINTS.md) · 57/57 Tests |
| Logging, Einstellungen, Notbremse STOP ALL | ⏳ als Nächstes | — |
| Geräteerkennung, ADB-Anbindung | ⏳ offen (Phase 1) | — |
| Dateimanager, Transfer, Backup, Restore | ⏳ offen (Phasen 3–5) | — |
| Speicheranalyse, Cleanup Center | ⏳ offen (Phasen 7–8) | — |
| WhatsApp-/Messenger-Module, APK, Live Screen | ⏳ offen (Phasen 9–13) | — |
| **Windows-Installer** | ⏳ offen (Phase 16) | — |

Der jeweils aktuelle Stand steht immer in **[`PROJECT_STATE.md`](PROJECT_STATE.md)**.

## Grundregeln des Projekts

1. **Niemals löschen aufgrund einer Dateiendung.** Kategorien entstehen aus mehreren Nachweisen
   (Dateisignatur, Pfad, Name, Metadaten) mit Konfidenzwert und Begründung.
2. **Musik ist kein Messenger-Audio.** Diese Trennung ist testgesichert und release-blockierend.
3. **Nichts Destruktives ohne Analyse, Vorschau und ausdrückliche Bestätigung.**
4. **Keine falschen Versprechen.** Was Android nicht erlaubt, wird erklärt — nicht umgangen.
   Kein Root, keine Exploits, kein Umgehen von Berechtigungen.
5. **Stabilität vor Funktionsmenge.** Jede Komponente durchläuft ein Gate
   (Build → Tests → Sicherheit → Regression → Laufzeit), bevor die nächste begonnen wird.

## Dokumentation

| Datei | Inhalt |
|---|---|
| [`PROJECT_STATE.md`](PROJECT_STATE.md) | aktueller Stand, Entscheidungen, nächster Schritt |
| [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) | Schichten, Komponenten, Evidenzklassifikation |
| [`docs/TECHNICAL_FEASIBILITY.md`](docs/TECHNICAL_FEASIBILITY.md) | geprüfte Machbarkeit, 6 korrigierte Annahmen |
| [`docs/SECURITY_MODEL.md`](docs/SECURITY_MODEL.md) | Bedrohungsmodell, Löschkette, Verbotsliste |
| [`docs/ROADMAP.md`](docs/ROADMAP.md) | Phasen 0–18 |
| [`docs/TEST_PLAN.md`](docs/TEST_PLAN.md) | Teststufen, Testmatrix, Grenzwerte |
| [`docs/RELEASE_CHECKLIST.md`](docs/RELEASE_CHECKLIST.md) | 16 Release-Gates — **1 von 16 erfüllt** |
| [`docs/CHECKPOINTS.md`](docs/CHECKPOINTS.md) | Gate-Protokoll je Komponente |
| [`docs/ACCEPTED_WARNINGS.md`](docs/ACCEPTED_WARNINGS.md) | bewertete, nicht blockierende Warnungen |
| [`KNOWN_LIMITATIONS.md`](KNOWN_LIMITATIONS.md) | ehrliche Liste dessen, was technisch **nicht** geht |

## Technik

* .NET 8 (LTS), C# 12, `Nullable` aktiv, Analyzer auf `latest-recommended`
* Oberfläche: Avalonia 11 (native Windows-x64-EXE, Dark Mode als Standard)
* Installer: Inno Setup — frei wählbarer Installationspfad, kein Admin-Zwang, kein Autostart
* Gerätezugriff: ADB; MTP zunächst nur Erkennung

## Selbst bauen und prüfen

```bash
bash scripts/setup-toolchain.sh                      # .NET SDK nach .cache/ (nicht ins Projekt)
bash scripts/build-and-test.sh                       # Build + 57 Tests + Laufzeittest
bash scripts/build-and-test.sh --windows-artifact    # zusätzlich win-x64-Artefakt
bash scripts/clean-workspace.sh                      # regenerierbare Artefakte entfernen
```

Unter Windows genügt ein installiertes .NET-8-SDK und `dotnet test -c Release`.
Das Zero-Warning-Gate ist scharf: `TreatWarningsAsErrors=true` — ein Build mit Warnung schlägt fehl.

## Rechtliches

* **Lizenz: noch nicht festgelegt.** Bis zur Festlegung gelten alle Rechte als vorbehalten.
* **LEGAL-001:** Die Android SDK Terms of Service untersagen die Weiterverteilung der
  Platform-Tools (`adb.exe`). Für eine öffentliche Verteilung ist statt der Bündelung ein
  Download-Assistent mit Hash-Prüfung vorgesehen. Siehe [`PROJECT_STATE.md`](PROJECT_STATE.md).
* Android ist eine Marke von Google LLC. Dieses Projekt steht in keiner Verbindung zu Google,
  Meta/WhatsApp, Telegram oder Signal.
