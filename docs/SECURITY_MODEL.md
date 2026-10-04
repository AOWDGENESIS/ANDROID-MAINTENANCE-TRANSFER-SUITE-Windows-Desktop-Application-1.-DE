# SECURITY_MODEL — Android Maintenance & Transfer Suite

Stand: 2026-10-04 · Version 0.1.0

---

## 1. Schutzziele

| Ziel | Bedeutung in diesem Projekt |
|---|---|
| **Datenverlustfreiheit** | Oberstes Ziel. Ein fälschlich gelöschtes Foto ist der schlimmste denkbare Fehler — schwerwiegender als ein Absturz. |
| **Vertraulichkeit** | Private Inhalte verlassen niemals das lokale System (§3). |
| **Transparenz** | Der Benutzer sieht vor jeder Aktion genau, was passieren wird. |
| **Nachvollziehbarkeit** | Jede destruktive Aktion ist im Protokoll rekonstruierbar. |
| **Integrität** | Übertragene Daten werden verifiziert, bevor Erfolg gemeldet wird (§74). |

---

## 2. Bedrohungsmodell (Was kann dieses Programm kaputt machen?)

| ID | Bedrohung | Auswirkung | Gegenmaßnahme |
|---|---|---|---|
| T-01 | Fehlklassifikation löscht Musik als „WhatsApp-Audio" | **Kritisch** | Evidenzregel R1 + Pflichtnachweis + Release-blockierender Test |
| T-02 | Massenlöschung ohne Vorschau | **Kritisch** | `ConfirmedDeletionPlan` nur über Bestätigungsstufe erzeugbar (Typsystem) |
| T-03 | Löschung vor abgeschlossenem Backup | **Kritisch** | Backup-Verifikation ist Vorbedingung (§28) |
| T-04 | Transfer meldet Erfolg, Datei ist unvollständig | **Hoch** | Größen-/Hashverifikation, sonst `Failed` |
| T-05 | Robocopy auf MTP-Pfad zerstört Zielstruktur | **Hoch** | Win32-Pfad-Guard + Negativtest |
| T-06 | Pfad-Traversal über manipulierte Gerätedateinamen (`../../`) | **Hoch** | Pfadnormalisierung + Wurzelprüfung bei jedem Schreibvorgang |
| T-07 | Geschützter Ordner landet in Bereinigungsvorschlag | **Hoch** | `ProtectedPathRegistry` greift vor der Vorschau, nicht danach |
| T-08 | Abbruch mitten im Schreiben hinterlässt Torso-Datei | **Mittel** | Schreiben in `.part`, atomares Umbenennen nach Verifikation |
| T-09 | Log enthält private Inhalte | **Mittel** | Allowlist-Logging, keine Dateiinhalte |
| T-10 | Manipulierte `adb.exe` im PATH wird ausgeführt | **Hoch** | Fester Pfad, Hash-Prüfung, kein `PATH`-Lookup ohne Zustimmung |
| T-11 | Kommandoinjektion über Dateinamen in `adb shell` | **Hoch** | Argumentliste statt Kommandozeilen-String, `ProcessStartInfo.ArgumentList` |
| T-12 | Zielspeicher läuft während Transfer voll | **Mittel** | Vorabprüfung freier Speicher + laufende Überwachung + sauberes `Failed` |
| T-13 | Installer schreibt ungefragt in Systemverzeichnisse | **Mittel** | Per-User-Installation, freie Pfadwahl, kein Autostart |
| T-14 | Gerät wird während Transfer getrennt | **Mittel** | Erkennung, Job → `Failed`, Wiederaufnahme angeboten, kein Absturz |

---

## 3. Was dieses Programm ausdrücklich NICHT tut (§6)

Diese Liste ist normativ. Ein Feature-Wunsch, der hier hineinfällt, wird abgelehnt, nicht
„kreativ gelöst":

* Keine Root-Exploits, kein Rooting, keine Exploit-Nutzung.
* Kein Lock-Screen-Bypass, kein Umgehen von FRP oder Verschlüsselung.
* Kein Umgehen von Scoped Storage, SELinux oder Berechtigungsmodellen.
* Keine heimliche Installation/Deinstallation von Apps, keine stillen Systemänderungen.
* Kein Auslesen von `/data/data`, keine Entschlüsselung von `msgstore.db.crypt15`.
* Keine Aktivierung von USB-Debugging ohne den Benutzer.
* Keine Hintergrunddienste, kein Autostart, keine Telemetrie, kein Phone-Home.

**Verhalten bei Blockade durch Android (§6, Pflichtablauf):**
1. Aktion abbrechen — niemals erzwingen, niemals wiederholt „probieren".
2. Benutzer verständlich informieren (was, warum, was nun).
3. Technische Ursache in `security.log` protokollieren.
4. Wenn vorhanden, offiziellen Android-Weg nennen
   (z. B. „Einstellungen → Apps → Speicher → Cache leeren" statt erzwungener Cache-Löschung).

---

## 4. Die Löschkette (§5) — technisch erzwungen

```
ANALYSIEREN → SICHERHEITSPRÜFUNG → ZIELPRÜFUNG → VORSCHAU
→ BENUTZERBESTÄTIGUNG → AUSFÜHREN → VERIFIZIEREN → PROTOKOLLIEREN
```

Durchsetzung nicht per Disziplin, sondern per Compiler:
* `DeletionCandidate` → nur `AnalysisStage` erzeugt sie.
* `DeletionPlan` → enthält Vorschau, Summen, Warnungen, geschützte Elemente.
* `ConfirmedDeletionPlan` → **internal ctor**, ausschließlich durch `ConfirmationStage`
  erzeugbar, trägt Zeitstempel, Benutzerentscheidung und Hash des Plans.
* `IExecutionStage.ExecuteAsync(ConfirmedDeletionPlan)` — andere Überladung existiert nicht.
* Weicht der ausgeführte Plan vom bestätigten ab (Hash-Vergleich), wird abgebrochen:
  Schutz davor, dass sich der Bestand zwischen Bestätigung und Ausführung ändert (TOCTOU).

**Sicherheitsprüfung vor jeder Löschung:**
| Prüfung | Verhalten bei Verstoß |
|---|---|
| Ziel in geschütztem Pfad? | Element wird markiert, nicht vorausgewählt, Warnung |
| `RequiresUserReview == true`? | nicht vorausgewählt (R2) |
| Einziges Vorkommen einer Datei (kein Duplikat)? | Hinweis bei Duplikat-Bereinigung |
| Backup gefordert und nicht verifiziert? | Löschung gesperrt |
| Ziel ist Verzeichnis mit unerwarteter Kindanzahl? | Warnung, erneute Bestätigung |
| Mehr als 1 000 Elemente oder mehr als 5 GB? | Zweite, explizite Bestätigung mit Summenanzeige |

---

## 5. Umgang mit „sicherem Löschen" (§27) — Ehrlichkeitsgebot

Auf Flash-Speichern (eMMC/UFS/SD) verhindern Wear-Leveling und FTL ein garantiertes
Überschreiben eines physischen Blocks. Mehrfaches Überschreiben erzeugt nur Schreiblast und
eine **falsche Sicherheit**.

Daher:
* Die Anwendung bietet **kein** „Secure Erase" und **keine** Mehrfach-Überschreibung auf
  Android-Speicher an.
* Die Funktion heißt „Dateien löschen" mit dem lokalisierten Zusatz: Wiederherstellung durch
  diese Anwendung ist nicht möglich, forensische Wiederherstellung kann nicht ausgeschlossen
  werden. Für echte Datenvernichtung: Geräteverschlüsselung + Werksreset.
* Auf **Windows-Zielen** ist Überschreiben technisch sinnvoller, wird aber ebenfalls nur mit
  korrekter Einschränkung (SSD/TRIM) beschrieben.

---

## 6. Umgang mit Prozessen und externen Werkzeugen

* ADB wird **nie** über eine zusammengesetzte Kommandozeile aufgerufen, sondern über
  `ProcessStartInfo.ArgumentList` → keine Shell-Interpretation, kein Injection-Vektor (T-11).
* ADB-Binärpfad: fest konfiguriert; vor dem ersten Start SHA-256 gegen die im Dependency-Register
  hinterlegte Prüfsumme (T-10). Abweichung = Warnung + Benutzerentscheidung, kein stiller Start.
* Keine `shell: true`-Ausführung, kein `cmd /c`, kein PowerShell-Aufruf mit String-Interpolation.
* Timeouts für jeden externen Prozessaufruf; hängende Prozesse werden beendet und protokolliert.

---

## 7. Datenschutz (DSGVO-Perspektive)

* Verarbeitung findet ausschließlich lokal statt; kein Auftragsverarbeiter, keine Übermittlung.
* Logs enthalten Pfade und Dateinamen — diese können personenbezogen sein. Deshalb:
  lokale Ablage, Rotation, Löschfunktion in den Einstellungen, keine automatische Versendung.
* Kein Crash-Reporting ins Netz. Absturzberichte werden lokal abgelegt und können vom Benutzer
  manuell weitergegeben werden.
* Vorschaubilder/Thumbnails werden im lokalen Cache gehalten und beim Trennen des Geräts bzw.
  auf Wunsch gelöscht.

---

## 8. Sicherheitstests, die Release-blockierend sind (§64)

| Test | Erwartung |
|---|---|
| `MusicIsNeverClassifiedAsWhatsAppAudio` | MP3 in `/Music` mit ID3-Tags → niemals Messenger-Kategorie |
| `ExtensionAloneNeverProducesDecisiveEvidence` | kein `IsDecisive` allein aus Endung |
| `DeletionWithoutConfirmationDoesNotCompile` | Architekturtest: keine öffentliche Ausführungs-API ohne bestätigten Plan |
| `ProtectedPathIsNeverPreselected` | geschützte Elemente nie vorausgewählt |
| `RobocopyRejectsNonWin32Path` | Ausnahme statt Ausführung |
| `AdbArgumentsAreNeverShellInterpreted` | Dateiname mit `; rm -rf` bleibt wirkungslos |
| `PathTraversalIsRejected` | `../` im Gerätepfad wird abgewiesen |
| `FailedVerificationNeverReportsSuccess` | manipulierte Zielgröße → `Failed` |
| `StopAllCancelsWithinTwoSeconds` | globale Notbremse wirkt |
| `NoSecretsInLogs` | Log-Senken enthalten keine Inhalte, nur Metadaten |
