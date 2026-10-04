# Anleitung: Stand veröffentlichen und als „im Aufbau" kennzeichnen

Alles ist vorbereitet und geprüft. Ich kann nicht selbst in Ihr Repository schreiben —
im Arbeitsbereich gibt es keine GitHub-Zugangsdaten, und ich frage Sie bewusst nicht nach
einem Token. Sie brauchen für die Veröffentlichung einen der beiden folgenden Wege.

Repository:
`AOWDGENESIS/ANDROID-MAINTENANCE-TRANSFER-SUITE-Windows-Desktop-Application-1.-DE`

---

## Weg A — über die GitHub-Webseite (ohne Git-Kenntnisse)

### Schritt 1: Dateien hochladen

1. Repository öffnen → Schaltfläche **„Add file"** → **„Upload files"**.
2. Aus diesem Arbeitsbereich folgende Ordner und Dateien hineinziehen:

   ```
   .github/        docs/           scripts/        src/
   tests/          tools/          .gitignore
   README.md       PROJECT_STATE.md   KNOWN_LIMITATIONS.md
   AndroidSuite.sln   Directory.Build.props
   ```

   > Die Ordner `.cache/`, `bin/` und `obj/` **nicht** hochladen — das sind regenerierbare
   > Build-Artefakte. Sie stehen bereits in `.gitignore`.

3. Commit-Beschreibung einsetzen:

   ```
   Phase 0: Fundament, Dokumentation und freigegebene Komponente Lokalisierung

   Stand: Phase 0 von 18 - noch KEIN lauffaehiges Programm.
   57/57 Tests bestanden, 0 Warnungen, Laufzeittest bestanden.
   ```

4. **„Commit changes"**.

> Die vorhandene `README.md` wird dabei durch die neue mit dem Warnbanner ersetzt — das ist
> beabsichtigt. Die Datei `ANDROID_MAINTENANCE_TRANSFER_SUITE_MASTER_PROMPT.md` bleibt erhalten.

### Schritt 2: Release anlegen und als Vorabversion kennzeichnen

1. Rechts in der Seitenleiste **„Releases"** → **„Create a new release"**.
2. **Choose a tag** → `v0.1.0-alpha.1` eintippen → **„Create new tag: v0.1.0-alpha.1 on publish"**.
3. **Release title**:
   ```
   🚧 v0.1.0-alpha.1 — Phase 0: Fundament (NOCH KEIN LAUFFÄHIGES PROGRAMM)
   ```
4. **Beschreibung**: den Textblock aus [`GITHUB_RELEASE_TEXT.md`](GITHUB_RELEASE_TEXT.md)
   einfügen (der Teil innerhalb des Codeblocks).
5. **Anhänge** hinzufügen (aus dem Ordner `release/` dieses Arbeitsbereichs):
   * `ANDROID-SUITE-v0.1.0-alpha.1-quellen.zip`
   * `SHA256SUMS.txt`
6. ☑️ **„Set as a pre-release" anhaken** — **das ist der entscheidende Schritt.**
   GitHub zeigt dann neben dem Release dauerhaft die gelbe Markierung **„Pre-release"**.
7. ☐ **„Set as the latest release" NICHT anhaken**.
8. **„Publish release"**.

---

## Weg B — über die Kommandozeile

```bash
bash scripts/publish-to-github.sh --tag
```

Das Skript prüft **zuerst** das Gate (Build, 57 Tests, Laufzeittest) und bricht ab, wenn etwas
fehlschlägt — es wird also niemals ein ungeprüfter Stand veröffentlicht. Danach räumt es auf,
erzeugt den Commit, pusht und setzt den Tag `v0.1.0-alpha.1`.

Anschließend das Release anlegen:

```bash
gh release create v0.1.0-alpha.1 \
  --title "🚧 v0.1.0-alpha.1 — Phase 0: Fundament (NOCH KEIN LAUFFÄHIGES PROGRAMM)" \
  --notes-file docs/releases/RELEASE_NOTES_v0.1.0-alpha.1.md \
  --prerelease \
  release/ANDROID-SUITE-v0.1.0-alpha.1-quellen.zip \
  release/SHA256SUMS.txt
```

---

## Woran man den Aufbau-Status danach sofort erkennt

| Ort | Kennzeichnung |
|---|---|
| Release-Liste | gelbes Abzeichen **„Pre-release"**, nicht als „Latest" markiert |
| Release-Titel | 🚧 und **„NOCH KEIN LAUFFÄHIGES PROGRAMM"** |
| Release-Text | Warnkasten ganz oben, zweisprachig DE/EN |
| Versionsnummer | `v0.1.0-alpha.1` — `0.x` und `-alpha` signalisieren Vorabstand |
| README oben | 🚧-Überschrift plus Warnkasten in Zitatform |
| README-Abzeichen | „Status: In Entwicklung", „Phase 0 von 18", „Release: NICHT FREIGEGEBEN", „Installer: noch nicht vorhanden" |
| Statustabelle | zeigt pro Komponente ✅ fertig oder ⏳ offen |
| Anhänge | heißen ausdrücklich **„quellen"**, keine EXE, kein Setup |
| CI-Abzeichen | belegt öffentlich, dass der Stand baut und die Tests bestehen |
| `KNOWN_LIMITATIONS.md` | Abschnitt A listet auf, was es noch nicht gibt |

---

## Was bewusst **nicht** im Release liegt

Es wird **keine EXE und kein Installer** angehängt. Das wäre irreführend: Das einzige derzeit
erzeugbare Windows-Programm ist der `SmokeRunner` — ein Prüfwerkzeug für den Testlauf, keine
Anwendung. Eine EXE im Release würde den Eindruck erwecken, man könne damit arbeiten.

Sobald Phase 16 erreicht ist, kommen Setup-EXE, SHA-256 und Installationsanleitung hinzu.

---

## Prüfsumme

```
f47a478974d05357d81674b86ccb8b683b566bc707a01ae4912dfe3d8d149e5c  ANDROID-SUITE-v0.1.0-alpha.1-quellen.zip
```
