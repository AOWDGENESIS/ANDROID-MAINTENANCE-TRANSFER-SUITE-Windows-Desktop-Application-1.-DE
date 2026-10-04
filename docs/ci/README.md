# GitHub-Actions-Workflow nachtragen

Die Datei `github-actions-ci.yml` gehoert nach `.github/workflows/ci.yml`.

Sie konnte beim automatischen Hochladen nicht mituebertragen werden: Ein Personal Access
Token darf Workflow-Dateien nur mit dem zusaetzlichen Scope `workflow` anlegen oder aendern.
Das ist eine Schutzmassnahme von GitHub und wird bewusst nicht umgangen.

## Variante 1 - ueber die Weboberflaeche (einfachste)

1. Im Repository: **Add file** -> **Create new file**
2. Als Dateinamen eintippen: `.github/workflows/ci.yml`
3. Den Inhalt von `docs/ci/github-actions-ci.yml` einfuegen
4. **Commit changes**

## Variante 2 - mit einem Token, das den Scope `workflow` besitzt

```bash
mkdir -p .github/workflows
cp docs/ci/github-actions-ci.yml .github/workflows/ci.yml
git add .github/workflows/ci.yml
git commit -m "CI: Build und Tests fuer Ubuntu und Windows"
git push
```

Danach baut und testet GitHub jeden Commit auf Ubuntu und Windows und das Abzeichen im
README zeigt den tatsaechlichen Zustand an.
