# Release Notes — v0.1.0-alpha.1

**Veröffentlicht: 2026-10-04 · Typ: Vorabversion (Pre-Release) · Phase 0 von 18**

---

## 🚧 DEUTSCH

### Das Wichtigste zuerst

**Dies ist kein benutzbares Programm.** Diese Vorabversion enthält **keine EXE**, **keinen
Installer**, **keine Oberfläche** und **keine Android-Funktionen**. Sie dokumentiert den
geprüften Stand des Fundaments. Wer sie herunterlädt, kann damit **kein Gerät verwalten**.

Zweck dieser Veröffentlichung: der Entwicklungsstand soll von Anfang an nachvollziehbar und
überprüfbar sein — nicht erst, wenn etwas „fertig aussieht".

### Was in dieser Version enthalten ist

**Phase-0-Dokumentation**
* `ARCHITECTURE.md` — Schichtenmodell, Komponentenübersicht, evidenzbasierte Klassifikation
* `TECHNICAL_FEASIBILITY.md` — geprüfte Machbarkeit mit **6 korrigierten Annahmen**
* `SECURITY_MODEL.md` — Bedrohungsmodell (14 Bedrohungen), erzwungene Löschkette, Verbotsliste
* `ROADMAP.md` — Phasen 0 bis 18 mit MVP-Grenze
* `TEST_PLAN.md` — Teststufen, Testmatrix, Grenzwerte
* `RELEASE_CHECKLIST.md` — 16 Release-Gates, davon **1 erfüllt**
* `KNOWN_LIMITATIONS.md` — was technisch nicht geht und warum

**Komponente 1: Lokalisierung — freigegeben**
* 178 Textschlüssel in Deutsch und Englisch
* Deutsch ist Standard beim ersten Start
* Sprachwechsel zur Laufzeit **ohne Neustart**
* Sprachwahl wird dauerhaft gespeichert und übersteht einen Neustart
* Beschädigte Einstellungsdateien werden gesichert und ersetzt, nicht stillschweigend verworfen
* Fehlende Schlüssel oder falsche Platzhalter führen nie zum Absturz
* Ein Test stellt sicher, dass **jeder** Schlüssel in beiden Sprachen existiert

**Projektgerüst**
* .NET 8, C# 12, `Nullable` aktiv, Analyzer `latest-recommended`
* **Zero-Warning-Gate**: `TreatWarningsAsErrors=true`
* Reproduzierbarer Build; das Windows-Artefakt hatte bei Wiederholung denselben SHA-256
* Build-, Test- und Aufräumskripte

### Prüfergebnis

| Prüfung | Ergebnis |
|---|---|
| Build (Release) | ✅ 0 Fehler, 0 Warnungen |
| Unit- und Integrationstests | ✅ 57 von 57 |
| Laufzeittest (isolierte Umgebung) | ✅ 12 von 12 |
| Windows-x64-Artefakt | ✅ erzeugt, Hash reproduzierbar |
| Sicherheitsprüfung Komponente 1 | ✅ bestanden |
| Installer-, Hardware- und MTP-Tests | ⏳ `PENDING-WINDOWS` — **nicht** als bestanden gewertet |

### Behobene Fehler

* **BUG-001** — Eine beschädigte `settings.json` blockierte das Speichern der Sprachwahl
  dauerhaft. Ursache war eine `JsonException` im Schreibpfad. Die beschädigte Datei wird nun als
  Sicherungskopie abgelegt und ersetzt. Regressionstest ergänzt.

### Nächster Schritt

Komponente 2: Protokollierung (neun getrennte Log-Dateien), Einstellungen, benutzerfreundliche
Fehlerobjekte und die globale Notbremse **STOP ALL**. Danach Phase 1: Geräteerkennung und
ADB-Anbindung.

### Offene rechtliche Punkte

* **LEGAL-001** — Die Android SDK Terms of Service untersagen die Weiterverteilung der
  Platform-Tools. Für eine öffentliche Verteilung ist ein Download-Assistent mit Hash-Prüfung
  vorgesehen statt einer Bündelung.
* **Lizenz des Projekts noch nicht festgelegt.** Bis dahin gelten alle Rechte als vorbehalten.

---

## 🚧 ENGLISH

### First things first

**This is not a usable program.** This pre-release contains **no EXE**, **no installer**,
**no user interface** and **no Android functionality**. It documents the verified state of the
foundation. Downloading it will **not** let you manage a device.

The purpose of publishing it is to make the development state traceable and verifiable from the
start — not only once something "looks finished".

### What is included

**Phase 0 documentation** — architecture, technical feasibility (with 6 corrected assumptions),
security model (14 threats, enforced deletion chain), roadmap (phases 0–18), test plan,
release checklist (16 gates, 1 met), known limitations.

**Component 1: Localization — approved**
* 178 text keys in German and English
* German is the default on first start
* Language switching at runtime **without restart**
* The choice is stored permanently and survives a restart
* Corrupt settings files are backed up and replaced, never silently discarded
* Missing keys or malformed placeholders never cause a crash
* A test guarantees that **every** key exists in both languages

**Project skeleton** — .NET 8, C# 12, nullable enabled, zero-warning gate
(`TreatWarningsAsErrors=true`), reproducible build, build/test/clean scripts.

### Verification results

| Check | Result |
|---|---|
| Build (Release) | ✅ 0 errors, 0 warnings |
| Unit and integration tests | ✅ 57 of 57 |
| Runtime test (isolated) | ✅ 12 of 12 |
| Windows x64 artefact | ✅ produced, hash reproducible |
| Security review of component 1 | ✅ passed |
| Installer, hardware and MTP tests | ⏳ `PENDING-WINDOWS` — **not** counted as passed |

### Fixed

* **BUG-001** — a corrupt `settings.json` permanently blocked saving the language choice.
  The corrupt file is now backed up and replaced. Regression test added.

### Next

Component 2: logging (nine separate log files), settings, user-facing error objects and the
global **STOP ALL** emergency brake. Then phase 1: device discovery and ADB.

### Open legal items

* **LEGAL-001** — the Android SDK Terms of Service prohibit redistributing the platform tools.
  A download assistant with hash verification is planned instead of bundling.
* **No licence has been chosen yet.** Until then, all rights reserved.
