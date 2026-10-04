# KNOWN_LIMITATIONS — bekannte und bewusst akzeptierte Grenzen

Stand: 2026-10-04 · Version 0.1.0-alpha.1

Diese Liste existiert, weil ein Werkzeug, das mit fremden Daten arbeitet, über seine Grenzen
nicht schweigen darf. Alle Punkte sind technisch geprüft, nicht vermutet.

---

## A — Der aktuelle Entwicklungsstand

| # | Einschränkung |
|---|---|
| A1 | **Es gibt keine ausführbare Anwendung.** Vorhanden sind Dokumentation, Projektgerüst und eine freigegebene Komponente (Lokalisierung). |
| A2 | **Es gibt keinen Installer.** Phase 16. |
| A3 | **Es gibt keine Benutzeroberfläche.** Phase 15. |
| A4 | **Es findet noch keine Kommunikation mit Android-Geräten statt.** Phase 1. |
| A5 | Der mitgelieferte `AndroidSuite.SmokeRunner` ist ein **Prüfwerkzeug für den Testlauf**, kein Produktbestandteil. |

---

## B — Dauerhafte technische Grenzen (ändern sich auch mit der fertigen Version nicht)

| # | Einschränkung | Ursache |
|---|---|---|
| B1 | **Kein garantiertes „sicheres Überschreiben" auf Android-Speicher.** Gelöschte Dateien sind durch diese Anwendung nicht wiederherstellbar; forensische Wiederherstellung lässt sich nicht ausschließen. | Wear-Leveling und FTL in Flash-Speichern verhindern den gezielten Zugriff auf physische Blöcke. Mehrfaches Überschreiben erzeugt nur Schreiblast und falsche Sicherheit. |
| B2 | **Kein Zugriff auf `/sdcard/Android/data/…`** | Seit Android 11 sperrt das Betriebssystem diesen Bereich auch für die ADB-Shell. Wird gemeldet, nicht umgangen. |
| B3 | **Kein Zugriff auf `/data/data/…`, keine App-Datenbanken, keine Entschlüsselung von WhatsApp-Backups** | Erfordert Root. Root-Exploits sind im Projekt ausdrücklich verboten. |
| B4 | **Kein byte-genaues Fortsetzen innerhalb einer Datei bei ADB-Übertragungen.** Fortsetzen wirkt auf Dateiebene: abgeschlossene Dateien werden übersprungen, eine angefangene Datei wird neu übertragen. | Das ADB-Protokoll kennt keinen Teilbereich-Transfer. |
| B5 | **Fortschrittsanzeige bei ADB auf Dateiebene**, nicht byte-genau | `adb pull` liefert keinen verlässlichen maschinenlesbaren Byte-Fortschritt. Ein erfundener Prozentwert wäre eine Lüge. |
| B6 | **Robocopy wird niemals für Android-Geräte verwendet** | MTP-Geräte besitzen keinen gültigen Win32-Pfad. Konstruktiv ausgeschlossen und durch einen Test abgesichert. |
| B7 | **USB-Debugging muss der Benutzer selbst aktivieren und auf dem Gerät bestätigen** | Android-Sicherheitsmechanismus. Wird erklärt, nicht umgangen. |
| B8 | **Kein Lock-Screen-Bypass, kein FRP-Bypass, keine Umgehung von Berechtigungen** | Ausdrücklich verboten (siehe `docs/SECURITY_MODEL.md`). |
| B9 | **MTP-Übertragung ist im MVP nicht enthalten** — nur die Erkennung „MTP verfügbar". | MTP ist bei vielen Objekten instabil, ohne zuverlässige Zeitstempel und ohne Teil-Lesen. Der produktive Pfad ist ADB. |
| B10 | **Keine zuverlässigen Gesundheitswerte für SD-Karten** | SMART-Daten stehen über MTP/ADB nicht zur Verfügung. Erfundene Werte wird es nicht geben. |

---

## C — Was noch nicht geprüft werden konnte

| # | Punkt | Status |
|---|---|---|
| C1 | Installation, benutzerdefinierter Pfad, Upgrade, Deinstallation | `PENDING-WINDOWS` — Inno Setup ist ein Windows-Werkzeug |
| C2 | Verhalten an echter Android-Hardware verschiedener Hersteller | `PENDING-HARDWARE` |
| C3 | WPD/MTP-Implementierung | `PENDING-WINDOWS` |

Ein nicht ausgeführter Test wird in diesem Projekt **niemals** als bestanden ausgewiesen.
