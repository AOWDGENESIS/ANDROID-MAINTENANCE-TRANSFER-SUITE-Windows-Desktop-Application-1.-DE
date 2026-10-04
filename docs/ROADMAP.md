# ROADMAP — Android Maintenance & Transfer Suite

Stand: 2026-10-04 · Version 0.1.0

Jede Phase endet mit dem Gate aus §60/§61. Ohne `RESULT: APPROVED` wird die nächste Phase
nicht begonnen. Phasen sind absichtlich klein geschnitten (§80).

| Phase | Inhalt | Ergebnis | Status |
|---|---|---|---|
| **0** | Analyse, Architektur, Sicherheitsmodell, Testplan, Projektgerüst, **Localization** | Dokumente + baubare Solution + erste geprüfte Komponente | **IN ARBEIT** |
| 0b | Logging, Settings, Fehlerobjekte, globale Notbremse (STOP ALL) | Querschnittsdienste | offen |
| 1 | DeviceDiscovery, AdbEngine, Gerätezustände, MTP-Erkennung | Gerät wird erkannt und ehrlich beschrieben | offen |
| 2 | DeviceMap (SQLite), inkrementeller Scan, StorageProvider | Gerätekarte, 500k-Dateien-Test | offen |
| 3 | Dateimanager (PC ↔ Android), Mehrfachauswahl, Eigenschaften | Navigierbare zwei Bereiche | offen |
| 4 | TransferEngine, Queue, Pause/Abbruch/Wiederaufnahme, Verifikation | Robuster Transfer | offen |
| 4b | MTP/WPD-Transfer (nur nach Hardware-Verifikation) | optionaler zweiter Pfad | bedingt |
| 5 | BackupEngine, RestoreEngine, Manifest, Konfliktstrategien | Backup/Restore | offen |
| 6 | MediaAnalyzer, MetadataEngine, Animationserkennung, Audio-Trennung | Evidenzklassifikation | offen |
| 7 | StorageAnalyzer, Dashboard-Diagramme, SD-Karte getrennt | „Was verbraucht meinen Speicher?" | offen |
| 8 | Cleanup Center, Dry-Run, Löschvorschau, geschützte Ordner, Duplikate, große/alte Dateien, Vorher/Nachher-Report | Kernnutzen | offen |
| 9 | WhatsApp Deep Cleaner (nur zugängliche Medienpfade) | Messenger-Modul 1 | offen |
| 10 | Telegram-/Signal-Cleaner | Messenger-Module 2/3 | offen |
| 11 | APK-Manager (Metadaten, Backup, Installation mit Bestätigung) | App-Verwaltung | offen |
| 12 | SecurityEngine (Berechtigungsanalyse, Anomalien) | Sicherheitsübersicht | offen |
| 13 | Live Screen (scrcpy-Integration) | Livebild | offen |
| 14 | HealthEngine, Diagnose, Wartungsprotokoll | Gerätegesundheit | offen |
| 15 | UI-Feinschliff, Barrierefreiheit, Kontraste, Tastaturbedienung | Oberfläche fertig | offen |
| 16 | Inno-Setup-Installer, Pfadwahl, Upgrade/Deinstallation | Setup-EXE | offen |
| 17 | Vollregression über alle Phasen, Testmatrix §63 | Regressionsnachweis | offen |
| 18 | Release: Version, Hash, Release Notes, bekannte Einschränkungen | RELEASE READY | offen |

## MVP-Grenze
MVP = Phasen 0 bis 8 plus 15 bis 18. Die Phasen 9 bis 14 sind **vorbereitet** (Interfaces,
deaktivierte Navigationseinträge), aber nicht implementiert (§57/§58).

## Ausdrücklich zurückgestellt (§69 — nicht ungefragt implementieren)
Cloud-Sync · WhatsApp-Datenbankentschlüsselung · Malware-/YARA-Scan · Fernsteuerung ·
automatische Cache-/Duplikatlöschung · iOS-Unterstützung · Kontakte/SMS/Anrufliste ·
Auto-Update über Internet.
