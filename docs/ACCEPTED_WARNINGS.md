# ACCEPTED_WARNINGS

§61 verlangt: `UNRESOLVED WARNINGS = 0`. Eine Warnung darf nur dann bestehen bleiben, wenn sie
hier mit Ort, Begründung und Bewertung erfasst ist. Projektweites Abschalten von Regeln ist
unzulässig — jede Unterdrückung erfolgt eng begrenzt.

| ID | Regel | Ort | Begründung | Bewertung |
|---|---|---|---|---|
| AW-001 | CA1707 (Unterstriche in Bezeichnern) | nur `tests/AndroidSuite.Localization.Tests` | Testmethoden heißen bewusst `Save_and_load_round_trip`, weil der Name im Testbericht als Satz lesbar sein soll. Betrifft keinen Produktivcode. | nicht blockierend |
| AW-002 | CA1031 (allgemeines `catch`) | `tools/AndroidSuite.SmokeRunner/Program.cs`, eine Stelle | Der Laufzeittest muss **jede** Ausnahme als Gate-Verstoß melden und mit Exitcode 1 enden; ein gezieltes Abfangen würde unerwartete Fehler verbergen. Kein Produktivcode. | nicht blockierend |

## Regeln für neue Einträge
1. Zuerst versuchen, die Ursache zu beheben — eine Unterdrückung ist die Ausnahme.
2. Unterdrückung immer lokal (`#pragma warning disable` mit Begründung oder `NoWarn` je Projekt).
3. Eintrag hier mit ID, Regel, Ort, Begründung, Bewertung.
4. Produktivcode-Unterdrückungen brauchen eine zusätzliche Sicherheitsbewertung.
