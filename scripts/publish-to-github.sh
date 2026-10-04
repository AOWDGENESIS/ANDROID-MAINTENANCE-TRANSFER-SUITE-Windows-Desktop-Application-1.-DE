#!/usr/bin/env bash
# Veroeffentlicht den geprueften Stand im GitHub-Repository.
#
# WICHTIG: Dieses Skript fragt NIEMALS nach einem Token und speichert keine Zugangsdaten.
# Die Anmeldung erfolgt ueber Ihre vorhandene Git-Konfiguration (SSH-Schluessel,
# Windows Credential Manager, gh auth login oder Git Credential Helper).
#
# Aufruf:
#   bash scripts/publish-to-github.sh            # Stand pushen
#   bash scripts/publish-to-github.sh --tag      # zusaetzlich Vorabversions-Tag setzen
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

REMOTE_URL="https://github.com/AOWDGENESIS/ANDROID-MAINTENANCE-TRANSFER-SUITE-Windows-Desktop-Application-1.-DE.git"
TAG="v0.1.0-alpha.1"
BRANCH="main"

# --- Sicherheitsnetz: nur geprueften Stand veroeffentlichen (§78) -------------
echo "[1/5] Gate-Pruefung vor der Veroeffentlichung ..."
if ! bash "${ROOT}/scripts/build-and-test.sh" >/tmp/gate.log 2>&1; then
  echo "ABBRUCH: Das Gate ist nicht bestanden. Es wird nichts veroeffentlicht."
  tail -30 /tmp/gate.log
  exit 1
fi
echo "       Gate bestanden (Build, Tests, Laufzeittest)."

echo "[2/5] Arbeitsverzeichnis aufraeumen ..."
bash "${ROOT}/scripts/clean-workspace.sh" >/dev/null

echo "[3/5] Git-Repository vorbereiten ..."
if [[ ! -d "${ROOT}/.git" ]]; then
  git init -q -b "${BRANCH}"
fi
if ! git remote get-url origin >/dev/null 2>&1; then
  git remote add origin "${REMOTE_URL}"
fi

git add -A
if git diff --cached --quiet; then
  echo "       Keine Aenderungen zu uebernehmen."
else
  git commit -q -m "Phase 0: Fundament, Dokumentation und freigegebene Komponente Lokalisierung

- Phase-0-Dokumente: Architektur, Machbarkeit, Sicherheitsmodell, Roadmap, Testplan,
  Release-Checkliste, bekannte Grenzen
- Komponente 1 (Lokalisierung) freigegeben: 178 Schluessel DE/EN, Live-Sprachwechsel,
  dauerhafte Speicherung, 57/57 Tests, 0 Warnungen
- Zero-Warning-Gate, reproduzierbarer Build, Build-/Test-/Aufraeumskripte
- BUG-001 behoben: beschaedigte settings.json blockierte das Speichern der Sprachwahl

Stand: Phase 0 von 18 - noch KEIN lauffaehiges Programm."
  echo "       Commit erstellt."
fi

echo "[4/5] Nach GitHub uebertragen ..."
echo "       Falls jetzt nach Zugangsdaten gefragt wird: Benutzername = GitHub-Name,"
echo "       Passwort = Personal Access Token (classic) mit dem Recht 'repo'."
git push -u origin "${BRANCH}"

if [[ "${1:-}" == "--tag" ]]; then
  echo "[5/5] Vorabversions-Tag ${TAG} setzen ..."
  git tag -a "${TAG}" -m "Phase 0 - Fundament. KEIN lauffaehiges Programm, keine EXE, kein Installer."
  git push origin "${TAG}"
  echo
  echo "Tag gesetzt. Release jetzt anlegen unter:"
  echo "  https://github.com/AOWDGENESIS/ANDROID-MAINTENANCE-TRANSFER-SUITE-Windows-Desktop-Application-1.-DE/releases/new?tag=${TAG}&prerelease=1"
  echo "  Text: docs/releases/GITHUB_RELEASE_TEXT.md"
  echo "  WICHTIG: 'Set as a pre-release' anhaken!"
else
  echo "[5/5] Tag uebersprungen (Aufruf mit --tag, um ${TAG} zu setzen)."
fi

echo
echo "FERTIG."
