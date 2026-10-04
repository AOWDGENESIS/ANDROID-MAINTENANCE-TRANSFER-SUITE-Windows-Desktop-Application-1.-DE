# TEST_PLAN — Android Maintenance & Transfer Suite

Stand: 2026-10-04 · Version 0.1.0

---

## 1. Teststufen

| Stufe | Werkzeug | Läuft wo | Gate |
|---|---|---|---|
| Unit | xUnit | Buildumgebung (Linux) + Windows | §60 |
| Architektur | xUnit + Reflection | Buildumgebung | §60 |
| Lokalisierung | xUnit (Schlüsselabgleich) | Buildumgebung | §64 |
| Integration (Fake-Gerät) | xUnit + `FakeAdbEngine` | Buildumgebung | §60 |
| Integration (echtes Gerät) | manuelles Protokoll | Windows + Testgerät | §64 |
| UI/Runtime | Avalonia.Headless + Start-Rauchtest | Buildumgebung | §60 |
| Installer | Inno Setup + Windows-VM | Windows | §64 |
| Regression | vollständiger Testlauf | beide | §61 |

**Wichtig:** Tests, die hier nicht ausführbar sind (echte Hardware, Installer), erhalten den
Status `PENDING-WINDOWS` — **niemals** `PASS`. Ein nicht gelaufener Test ist kein bestandener
Test (§72).

---

## 2. Testumgebung (§62)

* Niemals Tests gegen echte persönliche Daten.
* Testdaten werden **generiert**: `tests/TestData/FixtureBuilder` erzeugt Dateien mit echten
  Magic-Bytes (gültige Mini-JPEG/PNG/GIF/MP3/MP4), definierte Größen, definierte Zeitstempel.
* Zerstörerische Tests laufen ausschließlich in temporären Verzeichnissen, die nach dem Test
  gelöscht werden (§67 Workspace-Schutz).
* Gerätesimulation: `FakeAdbEngine` liefert reproduzierbare Ausgaben für `devices`, `getprop`,
  `ls`, `df`, `dumpsys battery` — inklusive Fehlerfällen (`unauthorized`, `offline`,
  Trennung mitten im Transfer).

---

## 3. Testmatrix (§63) — Pflicht je Funktionsgruppe

| Szenario | Transfer | Backup | Restore | Cleanup | Scan | Lokalisierung |
|---|---|---|---|---|---|---|
| NORMAL | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ |
| FEHLER | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ |
| ABBRUCH | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| WIEDERHOLUNG | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| LEERER DATENSATZ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ |
| GROSSE DATENMENGE | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| SEHR VIELE DATEIEN | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| USB-TRENNUNG | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| GERÄTENEUSTART | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| SPEICHER VOLL | ✔ | ✔ | ✔ | — | — | — |
| BERECHTIGUNG FEHLT | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| DATEI NICHT VORHANDEN | ✔ | ✔ | ✔ | ✔ | ✔ | — |
| ZIEL NICHT VERFÜGBAR | ✔ | ✔ | ✔ | ✔ | — | — |

---

## 4. Release-blockierende Sicherheitstests

Übernommen aus SECURITY_MODEL.md §8. Schlägt einer fehl, ist kein Release möglich —
unabhängig davon, wie vollständig die übrigen Funktionen sind.

---

## 5. Lokalisierungstests (§46)

| Test | Erwartung |
|---|---|
| `AllKeysExistInAllCultures` | Schlüsselmengen DE/EN identisch, sonst FAIL |
| `NoEmptyTranslations` | kein leerer oder nur aus Leerzeichen bestehender Wert |
| `DefaultCultureIsGerman` | Erststart ohne gespeicherte Einstellung → `de` |
| `CultureChangeRaisesIndexerNotification` | Live-Wechsel aktualisiert Bindings |
| `CulturePersistsAcrossRestart` | Einstellung überlebt Neustart |
| `UnknownKeyReturnsKeyNotCrash` | fehlender Schlüssel → Schlüsselname, keine Ausnahme |
| `FormatHandlesWrongArgumentCount` | fehlerhafte Platzhalter → kein Absturz |
| `NoHardcodedUserStringsInViewModels` | Architekturtest gegen literale UI-Texte |
| `PlaceholdersMatchBetweenCultures` | `{0}`/`{1}` in DE und EN identisch |

---

## 6. Performancetests (§73)

| Test | Grenzwert |
|---|---|
| Scan 100 000 Einträge (simuliert) | < 30 s, Speicher < 500 MB |
| Scan 500 000 Einträge (simuliert) | < 180 s, Speicher < 1 GB, kein UI-Block |
| Duplikat-Kaskade 100 000 Dateien | < 60 s bis Kandidatenliste |
| UI-Reaktion während Scan | Eingabeverzögerung < 100 ms |
| Sprachwechsel | < 200 ms, kein Flackern, kein Neustart |

---

## 7. Zero-Warning-Gate (§61)

Build läuft mit `TreatWarningsAsErrors=true`, `EnableNETAnalyzers=true`,
`AnalysisLevel=latest-recommended`, `Nullable=enable`.
Eine bewusst akzeptierte Warnung erfordert:
1. Eintrag in `docs/ACCEPTED_WARNINGS.md` mit ID, Ort, Begründung, Bewertung,
2. lokale Unterdrückung mit Begründungstext — niemals projektweites Abschalten.

---

## 8. Berichtsformat je Komponente (§70)

```
COMPONENT: <Name>
BUILD:     PASS | FAIL
TESTS:     <bestanden>/<gesamt>
WARNINGS:  <Anzahl> (akzeptiert: <Anzahl>, dokumentiert in ACCEPTED_WARNINGS.md)
SECURITY:  PASS | FAIL | N/A
REGRESSION:PASS | FAIL
RESULT:    APPROVED | REJECTED
NEXT:      <nächste Komponente>
```
