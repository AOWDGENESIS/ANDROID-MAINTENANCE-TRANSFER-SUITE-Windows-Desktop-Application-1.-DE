# RELEASE_CHECKLIST — Android Maintenance & Transfer Suite

Stand: 2026-10-04 · Zielversion 1.0.0 · aktueller Stand 0.1.0-dev

Ein Release erfolgt **niemals**, weil das Programm startet (§78). Erst wenn jede Zeile unten
auf PASS steht, gilt: **RELEASE READY**.

| # | Gate | Nachweis | Status |
|---|---|---|---|
| 1 | BUILD PASS | `dotnet build -c Release`, 0 Fehler, 0 unbehandelte Warnungen | ⬜ |
| 2 | UNIT TEST PASS | alle xUnit-Projekte grün, Abdeckung Core ≥ 80 % | ⬜ |
| 3 | INTEGRATION TEST PASS | Fake-Gerät + mindestens ein echtes Android-Gerät | ⬜ |
| 4 | SECURITY TEST PASS | alle 10 blockierenden Tests aus SECURITY_MODEL.md §8 | ⬜ |
| 5 | REGRESSION TEST PASS | Vollmatrix §63 | ⬜ |
| 6 | INSTALLER TEST PASS | Installation, **benutzerdefinierter Pfad** (z. B. `D:\Programme\AndroidSuite`), Upgrade, Rollback | ⬜ |
| 7 | UNINSTALL TEST PASS | rückstandsfreie Entfernung, Benutzerdaten nur nach Rückfrage | ⬜ |
| 8 | TRANSFER TEST PASS | Android→PC, PC→Android, groß/klein/viele, Trennung, Wiederaufnahme | ⬜ |
| 9 | BACKUP TEST PASS | Manifest korrekt, Hash stimmt, Ziel frei wählbar (nicht nur `C:`) | ⬜ |
| 10 | RESTORE TEST PASS | Datei/Ordner/Kategorie/Vollbackup, alle 3 Konfliktstrategien | ⬜ |
| 11 | CLEANUP TEST PASS | Dry-Run, Vorschau, geschützte Ordner, Vorher/Nachher-Report | ⬜ |
| 12 | LOCALIZATION TEST PASS | DE/EN vollständig, Live-Wechsel, Persistenz, keine Hardcodes | ⬜ |
| 13 | UI TEST PASS | Dark Mode, Kontraste, Tastatur, Responsivität, keine toten Buttons | ⬜ |
| 14 | DOCUMENTATION PASS | alle Dokumente aktuell, Benutzerhandbuch DE/EN, bekannte Einschränkungen | ⬜ |
| 15 | DEPENDENCY/LEGAL PASS | Register vollständig, **LEGAL-001 (ADB-Bündelung) entschieden** | ⬜ |
| 16 | RELEASE ARTEFAKTE | Setup-EXE, Version, Buildnummer, SHA-256, Release Notes | ⬜ |

## Release-Artefakte (§77)
```
AndroidSuite-Setup-<Version>.exe      + SHA-256
AndroidSuite-<Version>-portable.zip   + SHA-256   (optional)
RELEASE_NOTES_<Version>.md            (DE + EN)
KNOWN_LIMITATIONS.md
TEST_REPORT_<Version>.md
DEPENDENCIES.md
```

## Bekannte Einschränkungen, die im Release dokumentiert sein MÜSSEN
1. Kein byte-genaues Fortsetzen innerhalb einer Datei über ADB — nur auf Dateiebene.
2. `/Android/data/` ist ab Android 11 ohne Root nicht zugänglich.
3. Kein garantiertes physisches Überschreiben auf Flash-Speicher.
4. MTP-Transfer im MVP nicht enthalten (nur Erkennung).
5. USB-Debugging muss vom Benutzer selbst aktiviert und bestätigt werden.
6. WhatsApp-Datenbanken werden nicht gelesen oder entschlüsselt.
