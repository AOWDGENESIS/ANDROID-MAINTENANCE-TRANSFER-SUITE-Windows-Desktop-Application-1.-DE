#!/usr/bin/env bash
# Komponenten-Gate aus §60/§61 in einem Lauf:
#   BUILD -> UNIT TESTS -> RUNTIME TEST -> WINDOWS-ARTEFAKT -> AUFRAEUMEN
# Bricht beim ersten Fehler ab. Warnungen sind durch TreatWarningsAsErrors Fehler.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck source=/dev/null
source "${ROOT}/scripts/setup-toolchain.sh"

cd "${ROOT}"

echo
echo "=== 1/4 BUILD (Release, Zero-Warning-Gate) ==="
dotnet build -c Release

echo
echo "=== 2/4 UNIT- UND INTEGRATIONSTESTS ==="
dotnet test -c Release --no-build

echo
echo "=== 3/4 RUNTIME TEST (isolierte Testumgebung) ==="
dotnet "${ROOT}/tools/AndroidSuite.SmokeRunner/bin/Release/net8.0/AndroidSuite.SmokeRunner.dll"

if [[ "${1:-}" == "--windows-artifact" ]]; then
  echo
  echo "=== 4/4 WINDOWS-ARTEFAKT (win-x64) ==="
  OUT="${ROOT}/.cache/win-x64"
  rm -rf "${OUT}"
  dotnet publish tools/AndroidSuite.SmokeRunner -c Release -r win-x64 \
    --self-contained true -p:PublishSingleFile=true -o "${OUT}" >/dev/null
  sha256sum "${OUT}"/*.exe
else
  echo
  echo "=== 4/4 WINDOWS-ARTEFAKT uebersprungen (Aufruf mit --windows-artifact) ==="
fi

echo
echo "GATE-ERGEBNIS: PASS"
