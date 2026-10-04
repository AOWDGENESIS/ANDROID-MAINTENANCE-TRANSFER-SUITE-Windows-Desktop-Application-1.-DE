#!/usr/bin/env bash
# §67 WORKSPACE-SCHUTZ: entfernt alle regenerierbaren Artefakte.
# Quellcode, Dokumentation und Projektstand bleiben unangetastet.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

before=$(du -sm "${ROOT}" 2>/dev/null | cut -f1)

find . -type d \( -name bin -o -name obj \) -not -path "./.cache/*" -prune -exec rm -rf {} + 2>/dev/null || true
rm -rf "${ROOT}/.cache/win-x64" "${ROOT}/TestResults" 2>/dev/null || true
find . -name "*.trx" -delete 2>/dev/null || true

after=$(du -sm "${ROOT}" 2>/dev/null | cut -f1)
echo "Workspace bereinigt: ${before} MB -> ${after} MB"
echo "Hinweis: .cache/ (SDK, NuGet) wird nicht gespeichert und ist ueber scripts/setup-toolchain.sh reproduzierbar."
