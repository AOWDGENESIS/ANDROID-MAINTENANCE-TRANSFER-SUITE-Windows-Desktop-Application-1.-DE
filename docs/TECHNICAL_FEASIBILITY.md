# TECHNICAL_FEASIBILITY — Android Maintenance & Transfer Suite

Stand: 2026-10-04 · Version 0.1.0 · Status: Phase 0 (verbindlich für MVP-Planung)

Dieses Dokument prüft die technischen Annahmen des Master-Prompts **vor** der Implementierung
(§79). Jede Annahme wird mit MACHBAR / EINGESCHRÄNKT / NICHT MACHBAR bewertet. Eine als falsch
erkannte Annahme führt laut §82 zu Stopp und Architekturanpassung — die betroffenen Punkte sind
unten explizit als **ANNAHME-KORREKTUR** markiert.

---

## 0. Entwicklungs- und Buildumgebung (geprüft, nicht angenommen)

| Punkt | Messwert |
|---|---|
| Workspace-OS | Debian GNU/Linux 13 x86_64, Kernel 6.1 |
| Freier Speicher | 20 GB (§67 Workspace-Schutz: Budget-Grenze 2 GB, siehe unten) |
| Netzzugang | vorhanden (nuget.org 200, dot.net 200, github.com 200) |
| .NET SDK | 8.0 LTS, installiert nach `/home/user/.dotnet` |
| Zielartefakt | `dotnet publish -r win-x64` → native Windows-x64-EXE |

**ANNAHME-KORREKTUR 1:** Eine WPF- oder WinUI-3-Anwendung lässt sich auf dieser Buildumgebung
weder kompilieren noch starten. Das §60-Gate verlangt aber einen **nachweisbaren** RUNTIME TEST.
Ein Stack, dessen Tests nur behauptet werden könnten, verletzt §72 („Keine schönen Fehler").
→ Entscheidung: **Avalonia UI 11** (durch den Benutzer bestätigt). Avalonia ist XAML-basiert,
rendert mit eigenem Compositor, läuft auf Windows 10/11 x64 nativ und ist hier baubar,
startbar und headless testbar.

### Workspace-Budget (§67)
| Kategorie | Grenze | Durchsetzung |
|---|---|---|
| Quellcode + Doku | 50 MB | Review |
| `bin/`, `obj/`, `.dotnet` | nicht im Snapshot | von der Plattform ausgeschlossen |
| Testdaten (generiert) | 200 MB, nach Testlauf gelöscht | `tests/fixtures` wird generiert, nicht eingecheckt |
| Logs im Workspace | 10 MB, Rotation | `scripts/clean-workspace.sh` |

---

## 1. Android-Konnektivität: ADB vs. MTP

### 1.1 ADB (Android Debug Bridge) — MACHBAR, mit Nutzerhürde
* Offizieller Weg: `adb` aus den Google **SDK Platform-Tools**.
* Voraussetzung: USB-Debugging in den Entwickleroptionen aktiv **und** RSA-Fingerprint vom Gerät
  bestätigt. Beides kann die Anwendung **nicht** umgehen (§6) — nur erklären und anleiten.
* Relevante Zustände, die unterschieden werden MÜSSEN:
  `device` · `unauthorized` · `offline` · `no permissions` · `recovery` · `sideload` · kein Gerät.
  Jeder Zustand bekommt eine eigene benutzerfreundliche Meldung (§75), keine Sammelfehlermeldung.
* Dateizugriff: `adb shell ls/stat`, `adb pull`, `adb push`, `adb shell content`, `adb shell df`,
  `adb shell dumpsys battery`, `adb shell getprop`.
* **Geschwindigkeit:** `adb pull` erreicht über USB 3 real ca. 25–40 MB/s bei großen Dateien,
  bricht aber bei sehr vielen kleinen Dateien stark ein (Prozess-Roundtrip pro Datei).
  → Architekturkonsequenz: Batch-Pull ganzer Verzeichnisse statt Datei-für-Datei-Aufrufe.

**ANNAHME-KORREKTUR 2:** `adb pull` liefert **keinen** maschinenlesbaren Fortschritt pro Byte in
allen Versionen zuverlässig. §19 (Fortschritt, Geschwindigkeit, ETA) wird deshalb auf
**Datei-Ebene** umgesetzt (n von m Dateien, Bytes abgeschlossener Dateien + Größe der laufenden
Datei), nicht auf vorgetäuschter Byte-Granularität. Kein erfundener Prozentwert.

**ANNAHME-KORREKTUR 3:** „Pause/Fortsetzen" existiert bei `adb pull` nicht als Protokollfunktion.
→ Umsetzung als **Queue-Level-Pause**: laufende Datei wird zu Ende übertragen oder sauber
abgebrochen und beim Fortsetzen **neu begonnen**; abgeschlossene Dateien werden übersprungen.
Das wird dem Benutzer so angezeigt. Kein „Resume mitten in der Datei"-Versprechen.

### 1.2 MTP (Media Transfer Protocol) — EINGESCHRÄNKT
* MTP ist kein Dateisystem. Windows bildet Geräte unter dem **Shell-Namespace**
  („Dieser PC\\Gerät\\Interner Speicher") ab, **ohne** Laufwerksbuchstaben und **ohne** gültigen
  Win32-Pfad.
* **ANNAHME-KORREKTUR 4 (zentral, §17):** Weil es keinen Win32-Pfad gibt, kann **Robocopy
  grundsätzlich nicht** auf MTP-Geräte zugreifen. Der Master-Prompt vermutet das bereits —
  es ist hiermit bestätigt und harte Architekturregel: `WindowsFileSystemBackend` (Robocopy
  erlaubt) ist strikt getrennt von `AndroidMtpBackend` (Robocopy verboten, Guard-Klausel + Test).
* Zugriff erfolgt über **WPD (Windows Portable Devices), COM**. Das ist Windows-only, nicht in
  dieser Umgebung testbar und erfordert einen Interop-Layer.
* MTP-Schwächen (real, dokumentiert): keine zuverlässigen Zeitstempel beim Schreiben, kein
  Teil-Lesen/Resume, instabil bei >50.000 Objekten pro Ordner, Enumerierung sehr langsam.
* → **MVP-Entscheidung:** MTP wird als Interface (`IMtpEngine`) und als *Erkennung* („MTP
  verfügbar: ja/nein") implementiert; der produktive Transferpfad im MVP ist **ADB**.
  Vollständige WPD-Implementierung = Phase 4b, erst nach Verifikation auf echter Hardware.
  Begründung: §80 Stabilität vor Funktionsmenge.

### 1.3 Geräteerkennung ohne ADB-Autorisierung — MACHBAR
USB-Deskriptor-Ebene (VID/PID) über WinUSB/SetupAPI liefert Hersteller und Verbindungsstatus auch
ohne ADB-Freigabe. Damit ist „Gerät angeschlossen, aber nicht autorisiert" korrekt darstellbar.

---

## 2. Android-Speicherzugriff: Scoped Storage (Android 11–16)

| Pfad | Zugriff über ADB (nicht-root) | Konsequenz |
|---|---|---|
| `/sdcard/DCIM`, `Pictures`, `Movies`, `Music`, `Download`, `Documents` | **lesbar/schreibbar** | Kernarbeitsbereich des MVP |
| `/sdcard/Android/media/<pkg>/…` | **lesbar** | WhatsApp-Medien ab Android 11 liegen hier |
| `/sdcard/Android/data/<pkg>/…` | **ab Android 11 gesperrt** (auch für `adb shell` in der Shell-Domain) | Nur eingeschränkt, muss ehrlich gemeldet werden |
| `/data/data/<pkg>/…` | **gesperrt** (nur Root) | Außerhalb des Projekts (§6) |

**ANNAHME-KORREKTUR 5 (WhatsApp, §36):** Der klassische Pfad `/sdcard/WhatsApp/` existiert nur
auf Altgeräten bzw. nach Migration. Ab Android 11 ist der korrekte Ort
`/sdcard/Android/media/com.whatsapp/WhatsApp/Media/`. Business-Variante:
`com.whatsapp.w4b`. Die Anwendung darf **keinen** dieser Pfade annehmen, sondern muss beide
prüfen und bei Nichtzugänglichkeit dies ausweisen statt zu raten.
Datenbanken (`msgstore.db`) sind verschlüsselt (`.crypt14/15`) und im MVP **tabu** (§58).

**Folge für §27 „Sicheres Löschen":** Auf Flash-Speicher mit FTL/Wear-Leveling ist ein
garantiertes physisches Überschreiben **nicht möglich** und wird daher **nicht behauptet**.
Die UI nennt die Funktion „Datei löschen (nicht wiederherstellbar durch die App)" und erklärt den
Unterschied in einem Hinweistext. Bewertung: NICHT MACHBAR → ehrlich kommuniziert.

---

## 3. Dateityp- und Medienerkennung

### 3.1 Signaturerkennung — MACHBAR (eigene Implementierung, keine libmagic-Abhängigkeit)
Magic-Bytes für alle in §9 geforderten Formate sind stabil und kurz:

| Format | Signatur |
|---|---|
| JPEG | `FF D8 FF` |
| PNG / APNG | `89 50 4E 47 0D 0A 1A 0A` (+ `acTL`-Chunk ⇒ **animiert**) |
| GIF | `GIF87a` / `GIF89a` (+ >1 Image Descriptor ⇒ **animiert**) |
| WEBP | `RIFF`…`WEBP` (+ `ANIM`-Chunk ⇒ **animiert**) |
| HEIC/HEIF | ISO-BMFF `ftyp` mit Brand `heic`/`heix`/`mif1` |
| MP4/M4A/M4B/MOV/3GP | ISO-BMFF `ftyp`, Unterscheidung über Brand + Track-Typ |
| MKV/WEBM | `1A 45 DF A3` (EBML) |
| MP3 | `ID3` oder Frame-Sync `FF Ex/Fx` |
| FLAC | `fLaC` · OGG/OPUS | `OggS` · WAV/AVI | `RIFF` · AIFF | `FORM` |
| PDF | `%PDF-` · ZIP/APK/DOCX/XLSX/PPTX | `PK\x03\x04` |
| RAR | `Rar!\x1A\x07` · 7Z | `7z\xBC\xAF\x27\x1C` |

**ANNAHME-KORREKTUR 6 (§10 GIF):** „GIF" ist kein ausreichendes Kriterium für die Kategorie
ANIMATIONEN — ein GIF kann statisch sein, und ein animiertes WEBP/APNG ist kein GIF. Die
Kategorie wird deshalb über das geprüfte Merkmal **„enthält mehr als einen Frame"** gebildet,
nicht über die Endung. Statische GIFs landen in BILDER, nicht in SONSTIGES.

### 3.2 Container-Mehrdeutigkeit — EINGESCHRÄNKT, Architekturkonsequenz
`PK\x03\x04` ist gleichzeitig ZIP, APK, APKS, XAPK, DOCX, XLSX, PPTX, EPUB, JAR.
`ftyp` ist gleichzeitig MP4-Video, M4A-Musik, M4B-Hörbuch und 3GP-Sprachaufnahme.
→ Ein einzelnes Signal reicht prinzipiell nicht aus. Das rechtfertigt die
**Evidenz-Architektur** (siehe ARCHITECTURE.md §4): Klassifikation = gewichtete Evidenz aus
Signatur + Containerinhalt + MIME + Pfad + Name + Metadaten, mit **Konfidenzwert** und
**Begründungsliste**. Genau das verlangt §4 des Master-Prompts.

### 3.3 Musik vs. Messenger-Audio (§12, sicherheitskritisch) — MACHBAR
Unterscheidbare, belastbare Signale:

| Signal | Musik | WhatsApp-Sprachnachricht |
|---|---|---|
| Pfad | `/Music`, `/Audiobooks`, App-Ordner von Podcast-Apps | `…/WhatsApp/Media/WhatsApp Voice Notes/` |
| Dateiname | frei | `PTT-YYYYMMDD-WAnnnn.opus` |
| Codec | MP3/FLAC/AAC | OPUS in OGG |
| ID3/Tags | Artist/Album meist gesetzt | praktisch nie |
| Dauer | meist > 60 s | meist < 60 s |

**Harte Sicherheitsregel (wird als Unit-Test fixiert):** Eine Datei wird nur dann als
Messenger-Audio klassifiziert, wenn ein **pfad- oder namensbasierter Messenger-Nachweis**
vorliegt. Dateiendung allein erzeugt **niemals** diese Kategorie. Test
`MusicIsNeverClassifiedAsWhatsAppAudio` ist Release-blockierend (§64).

### 3.4 Audio-Metadaten (§14) — MACHBAR
ID3v1/ID3v2.3/2.4, Vorbis Comments, MP4-`ilst`-Atome sind offen dokumentiert und ohne
Fremdbibliothek lesbar. Kandidat als Abhängigkeit: `TagLibSharp` (LGPL-2.1) — Entscheidung im
Dependency-Register (siehe unten). Grundsatz: **keine Metadaten erfinden**; fehlende Felder =
`null`, UI zeigt „unbekannt" (lokalisiert), nicht „-" oder erratene Werte.

---

## 4. Duplikaterkennung & Performance bei 100k–500k Dateien (§31, §73)

Gemessene Rahmenbedingung: Ein voller SHA-256 über 100 GB dauert bei 400 MB/s ≈ 4 Minuten lokal
— über ADB wäre es ein kompletter Download des Geräts und damit **nicht vertretbar**.

→ **Kaskade** (nur die jeweils nächste Stufe für Kandidaten der vorigen):
1. Dateigröße gruppieren (O(n), nahezu kostenlos)
2. Name + Zeitstempel (Heuristik, nur Hinweis)
3. Teil-Hash: erste 64 KB + letzte 64 KB + Größe (xxHash64)
4. Voller Hash **nur** auf ausdrückliche Benutzeranforderung je Kandidatengruppe

Datenhaltung: **SQLite** als Geräteindex (`DeviceMap`) mit Indizes auf Größe, Hash, Kategorie,
Änderungsdatum. UI liest ausschließlich paginiert und über `ObservableCollection` mit
Virtualisierung. Kein In-Memory-Volldatensatz, kein Blockieren des UI-Threads.
Inkrementelle Scans über `(Pfad, Größe, mtime)`-Vergleich (§8).

---

## 5. Installer (§49/§50) — MACHBAR

**Inno Setup 6** (durch den Benutzer bestätigt).
* Freie Pfadwahl: `DisableDirPage=no`, Vorschlag `{autopf}\AndroidSuite`, aber jedes Ziel
  (`D:\Programme\…`, `F:\Apps\…`) erlaubt → erfüllt §49.
* Per-User-Installation (`PrivilegesRequiredOverridesAllowed=dialog`) vermeidet Admin-Zwang.
* Keine Autostart-Einträge, keine Desktop-Verknüpfung als Vorgabe (opt-in), kein Dienst (§49).
* Uninstaller entfernt Programmdateien; Benutzerdaten (`%APPDATA%`) nur nach Rückfrage.
* Setup-Sprachen DE (Standard) + EN.
* **Einschränkung:** `ISCC.exe` ist ein Windows-Tool. In dieser Umgebung wird das `.iss`-Skript
  erstellt und syntaktisch geprüft; der **Installer-Build und die Tests nach §50 erfolgen auf
  Windows** und werden erst dort als PASS gewertet. Bis dahin: Status `PENDING`, nicht `PASS`.

---

## 6. Abhängigkeits-Register (§51) — Vorentscheidung

| Name | Version | Quelle | Lizenz | Zweck | Status |
|---|---|---|---|---|---|
| .NET Runtime | 8.0 LTS | microsoft.com | MIT | Laufzeit (self-contained publiziert) | FREIGEGEBEN |
| Avalonia UI | 11.x | nuget.org | MIT | Desktop-UI | FREIGEGEBEN |
| CommunityToolkit.Mvvm | 8.x | nuget.org | MIT | MVVM, Source-Generatoren | FREIGEGEBEN |
| Microsoft.Data.Sqlite | 8.x | nuget.org | MIT | Geräteindex | FREIGEGEBEN |
| xUnit + coverlet | aktuell | nuget.org | Apache-2.0/MIT | Tests | FREIGEGEBEN |
| Serilog | 4.x | nuget.org | Apache-2.0 | Logging mit Dateirotation (§53) | FREIGEGEBEN |
| TagLibSharp | 2.3 | nuget.org | LGPL-2.1 | Audio-Metadaten | **PRÜFUNG** (LGPL: dynamische Bindung ok, aber bei self-contained Publish bewerten) |
| Inno Setup | 6.x | jrsoftware.org | eigene, redistributionsfreundlich | Installer | FREIGEGEBEN |
| Android SDK Platform-Tools | aktuell | dl.google.com | Android SDK ToS | ADB | **⚠ RECHTSPRÜFUNG, siehe unten** |

### ⚠ RECHTLICHER BEFUND — ADB-Bündelung
Der Benutzer hat „platform-tools mitliefern" gewählt. Pflichtgemäßer Hinweis: Die **Android SDK
Terms of Service** untersagen in Abschnitt 3.4 ausdrücklich die Weiterverteilung des SDK bzw.
seiner Bestandteile. Ein kommerziell oder öffentlich verteilter Installer mit eingebettetem
`adb.exe` ist damit **rechtlich angreifbar**.

Umsetzung, die beides erfüllt (Komfort + Rechtssicherheit), als Architekturvorgabe:
1. `AdbProvisioningService` sucht ADB in dieser Reihenfolge: konfigurierter Pfad → mitgelieferter
   Ordner → `PATH` → bekannte SDK-Installationsorte.
2. Für den **privaten Eigengebrauch** kann der Build-Schritt `tools/fetch-platform-tools.ps1`
   die Platform-Tools lokal in den Installer einbetten (Version, URL, SHA-256 protokolliert).
3. Für eine **öffentliche Verteilung** wird dieser Schritt deaktiviert; der Erst-Start-Assistent
   lädt die Tools dann von `dl.google.com` nach Zustimmung des Benutzers herunter und verifiziert
   den Hash.
Damit ist die Entscheidung umgesetzt, ohne eine Lizenzverletzung als „erledigt" zu verbuchen.
Eintrag in `docs/OPEN_ISSUES` → **LEGAL-001**, Entscheidung vor Release erforderlich.

---

## 7. Live Screen (§52) — MACHBAR, aber Phase 13
`scrcpy` (Apache-2.0) ist der einzige realistische Weg und ist frei weiterverteilbar. Es nutzt
ausschließlich offizielle Mechanismen (ADB + `app_process`), umgeht nichts. Integration als
externer Prozess in einem eingebetteten Fensterbereich. **Nicht im MVP** (§58).

---

## 8. Zusammenfassung der Machbarkeit

| Anforderung | Bewertung |
|---|---|
| Geräteerkennung USB/ADB | MACHBAR |
| MTP-Transfer | EINGESCHRÄNKT → Erkennung im MVP, WPD-Transfer Phase 4b |
| Robocopy für Android | **NICHT MACHBAR** → architektonisch ausgeschlossen |
| Device Map inkrementell | MACHBAR (SQLite) |
| Dateityp-/Animationserkennung | MACHBAR (Signatur + Frame-Prüfung) |
| Musik ≠ Messenger-Audio | MACHBAR (Evidenz + Pflichtnachweis), testgesichert |
| Metadaten | MACHBAR |
| Byte-genaues Resume über ADB | **NICHT MACHBAR** → Datei-Level-Resume, ehrlich kommuniziert |
| Garantiertes sicheres Überschreiben auf Flash | **NICHT MACHBAR** → wird nicht behauptet |
| `/Android/data`-Zugriff ab Android 11 | **NICHT MACHBAR** ohne Root → wird gemeldet, nicht erzwungen |
| WhatsApp-Medien ab Android 11 | MACHBAR über `/Android/media/` |
| Duplikate 500k Dateien | MACHBAR (Kaskade + SQLite) |
| Installer mit freier Pfadwahl | MACHBAR (Inno Setup), Test auf Windows ausstehend |
| Live DE/EN-Wechsel ohne Neustart | MACHBAR (Avalonia-Bindings + INotifyPropertyChanged) |
