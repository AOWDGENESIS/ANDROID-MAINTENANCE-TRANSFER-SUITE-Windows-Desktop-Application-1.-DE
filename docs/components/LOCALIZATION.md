# Komponente: Localization

Status: **APPROVED** (Checkpoint 001) · Projekte: `AndroidSuite.Core`, `AndroidSuite.Localization`

Erfüllt §2 (zentrales Lokalisierungssystem, Deutsch als Standard, dauerhafte Speicherung) und
§46 (Live-Wechsel ohne Neustart, keine hartcodierten UI-Texte).

---

## Bestandteile

| Typ | Projekt | Aufgabe |
|---|---|---|
| `ILocalizationService` | Core | Vertrag für alle Oberflächentexte |
| `ITranslationCatalog` | Core | Quelle der Übersetzungen (austauschbar) |
| `ILanguagePreferenceStore` | Core | dauerhafte Ablage der Sprachwahl |
| `CultureChangedEventArgs` | Core | Meldung über den Wechsel |
| `JsonLocalizationService` | Localization | Implementierung inkl. Rückfallkette |
| `EmbeddedJsonTranslationCatalog` | Localization | lädt eingebettete `de.json`/`en.json` |
| `JsonFileLanguagePreferenceStore` | Localization | `%APPDATA%\AndroidSuite\settings.json` |
| `TranslationCatalogValidator` | Localization | Vollständigkeits- und Platzhalterprüfung |
| `TranslationKeys` | Localization | 178 Konstanten statt Zeichenketten im Code |

---

## Verwendung

```csharp
var catalog = EmbeddedJsonTranslationCatalog.Load();
using var store = new JsonFileLanguagePreferenceStore(JsonFileLanguagePreferenceStore.GetDefaultPath());
ILocalizationService loc = await JsonLocalizationService.CreateAsync(catalog, store);

var title = loc[TranslationKeys.DeviceConnected];                       // "Gerät verbunden"
var progress = loc.Format(TranslationKeys.TransferProgressFiles, 12, 340); // "12 von 340 Dateien"

await loc.SetCultureAsync(CultureInfo.GetCultureInfo("en"));            // Live-Wechsel
```

In XAML erfolgt die Bindung später über den Indexer, damit `PropertyChanged("Item[]")` beim
Sprachwechsel alle sichtbaren Texte neu auflöst — ohne Neustart und ohne Neuaufbau der Fenster.

---

## Zugesicherte Eigenschaften

| Zusicherung | Umsetzung | Test |
|---|---|---|
| Erststart ist Deutsch | Rückfallsprache `de`, auch bei leerer Einstellung | `First_start_uses_german` |
| Sprache überlebt Neustart | JSON-Datei im Benutzerprofil | `Persisted_language_is_restored_on_next_start` |
| Wechsel ohne Neustart | `PropertyChanged("Item[]")` + `CultureChanged` | `Culture_change_raises_indexer_notification…` |
| Nie ein Absturz wegen Text | fehlender Schlüssel → Schlüsselname; falsche Platzhalter → Vorlage | `Unknown_key_returns_the_key…`, `Format_with_wrong_argument_count…` |
| Beschädigte Einstellung blockiert nichts | Sicherungskopie, dann Neuanlage | `Corrupt_file_is_backed_up_before_being_replaced` |
| Fremde Einstellungen bleiben erhalten | nur `language` wird geschrieben | `Save_preserves_unrelated_settings…` |
| Kein halb geschriebener Zustand | `.tmp` + atomares Ersetzen | `Save_leaves_no_temporary_file_behind` |
| Threadsicher | unveränderliche Kataloge, `lock` nur um das Kulturfeld | `Concurrent_reads_during_a_language_switch…` |
| Keine falschen Sprachen | unbekannte Kultur wird abgelehnt, nicht stillschweigend ersetzt | `Unknown_culture_is_rejected…` |
| `de-AT`/`en-GB` funktionieren | Rückfall auf den neutralen Katalog | `Regional_culture_resolves…` |

---

## Katalogpflege

* Quelle: `src/AndroidSuite.Localization/Resources/de.json` und `en.json` — flache
  JSON-Objekte, Schlüssel nach dem Muster `bereich.name`, identische Reihenfolge.
* Jeder neue Text braucht: Eintrag in **beiden** Dateien **und** eine Konstante in
  `TranslationKeys`. Fehlt eines davon, schlägt der Testlauf fehl — Übersetzungslücken können
  nicht unbemerkt ins Release gelangen.
* Platzhalter (`{0}`, `{1}`) müssen in beiden Sprachen übereinstimmen; das wird geprüft.
* Deutsch ist die Referenzsprache für die Vollständigkeitsprüfung.

---

## Bewusste Grenzen

* Noch keine XAML-Markup-Erweiterung — folgt mit der Oberfläche (Phase 15).
* Keine Rechts-nach-links-Sprachen, keine Pluralformen. Für DE/EN nicht erforderlich; eine
  spätere Erweiterung bliebe hinter demselben Interface verborgen.
* Der Dienst löst `PropertyChanged` im aufrufenden Kontext aus. Die Oberflächenschicht ruft
  `SetCultureAsync` vom UI-Thread auf; dadurch läuft auch die Benachrichtigung dort.
