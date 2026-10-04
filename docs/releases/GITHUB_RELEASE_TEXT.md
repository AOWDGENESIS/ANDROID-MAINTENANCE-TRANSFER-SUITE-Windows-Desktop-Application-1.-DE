# Vorlage für das GitHub-Release

So anlegen, dass sofort erkennbar ist, dass das Programm noch im Aufbau ist.

## Einstellungen im GitHub-Formular

| Feld | Wert |
|---|---|
| Tag | `v0.1.0-alpha.1` |
| Target | `main` |
| Release title | `🚧 v0.1.0-alpha.1 — Phase 0: Fundament (NOCH KEIN LAUFFÄHIGES PROGRAMM)` |
| **Set as a pre-release** | ✅ **anhaken — wichtig!** GitHub zeigt dann den gelben Hinweis „Pre-release" und markiert das Release **nicht** als „Latest". |
| Set as the latest release | ❌ **nicht** anhaken |
| Anhänge | `ANDROID-SUITE-v0.1.0-alpha.1-quellen.zip` und `SHA256SUMS.txt` |

---

## Text zum Einfügen in das Beschreibungsfeld

```markdown
> # ⚠️ KEIN FERTIGES PROGRAMM — DIESES PROJEKT IST IM AUFBAU
>
> Dieses Release enthält **keine EXE, keinen Installer, keine Oberfläche und keine
> Android-Funktionen**. Es dokumentiert den geprüften Stand des Fundaments.
> **Sie können damit noch kein Gerät verwalten.** Bitte nicht als fertige Software verwenden
> oder weitergeben.
>
> **Phase 0 von 18 · 1 von 16 Release-Gates erfüllt**

> # ⚠️ NOT A FINISHED PROGRAM — THIS PROJECT IS UNDER CONSTRUCTION
>
> This release contains **no EXE, no installer, no user interface and no Android functionality**.
> It documents the verified state of the foundation. **You cannot manage a device with it yet.**
>
> **Phase 0 of 18 · 1 of 16 release gates met**

---

## Was geprüft und freigegeben ist

| Prüfung | Ergebnis |
|---|---|
| Build (Release) | ✅ 0 Fehler, 0 Warnungen |
| Unit- und Integrationstests | ✅ 57 / 57 |
| Laufzeittest (isolierte Umgebung) | ✅ 12 / 12 |
| Windows-x64-Artefakt | ✅ erzeugt, SHA-256 reproduzierbar |
| Sicherheitsprüfung Komponente 1 | ✅ bestanden |
| Installer / echte Hardware / MTP | ⏳ PENDING-WINDOWS — **nicht** als bestanden gewertet |

**Freigegebene Komponente:** Lokalisierung — 178 Textschlüssel DE/EN, Deutsch als Standard,
Sprachwechsel zur Laufzeit ohne Neustart, dauerhafte Speicherung, abgesichert gegen beschädigte
Einstellungsdateien.

**Enthaltene Dokumentation:** Architektur · technische Machbarkeit (6 korrigierte Annahmen) ·
Sicherheitsmodell (14 Bedrohungen) · Roadmap (Phasen 0–18) · Testplan · Release-Checkliste ·
bekannte Grenzen.

## Was ausdrücklich NICHT geht

- Kein garantiertes „sicheres Überschreiben" auf Flash-Speicher — das ist technisch unmöglich
  und wird deshalb nicht versprochen.
- Kein Zugriff auf `/Android/data/` ab Android 11 und kein Zugriff auf App-Datenbanken.
- Kein Root, keine Exploits, kein Umgehen von Berechtigungen oder Sperrbildschirmen.
- Kein byte-genaues Fortsetzen innerhalb einer Datei bei ADB-Übertragungen.

Vollständige Liste: [`KNOWN_LIMITATIONS.md`](KNOWN_LIMITATIONS.md)

## Nächster Schritt

Protokollierung, Einstellungen, Fehlerobjekte und die globale Notbremse **STOP ALL** —
danach Phase 1: Geräteerkennung und ADB-Anbindung.

## Rechtliches

- **Lizenz noch nicht festgelegt**, bis dahin alle Rechte vorbehalten.
- **LEGAL-001:** Die Android SDK Terms of Service untersagen die Weiterverteilung der
  Platform-Tools; für eine öffentliche Verteilung ist ein Download-Assistent vorgesehen.
- Android ist eine Marke von Google LLC. Keine Verbindung zu Google, Meta/WhatsApp,
  Telegram oder Signal.

---

Selbst prüfen:
`bash scripts/setup-toolchain.sh && bash scripts/build-and-test.sh`
```

---

## Alternative: Release über die Kommandozeile

```bash
gh release create v0.1.0-alpha.1 \
  --repo AOWDGENESIS/ANDROID-MAINTENANCE-TRANSFER-SUITE-Windows-Desktop-Application-1.-DE \
  --title "🚧 v0.1.0-alpha.1 — Phase 0: Fundament (NOCH KEIN LAUFFÄHIGES PROGRAMM)" \
  --notes-file docs/releases/RELEASE_NOTES_v0.1.0-alpha.1.md \
  --prerelease \
  release/ANDROID-SUITE-v0.1.0-alpha.1-quellen.zip \
  release/SHA256SUMS.txt
```

Der Schalter `--prerelease` ist das entscheidende Merkmal: GitHub zeigt dann die gelbe
Kennzeichnung „Pre-release" und stuft das Release nicht als „Latest" ein.
