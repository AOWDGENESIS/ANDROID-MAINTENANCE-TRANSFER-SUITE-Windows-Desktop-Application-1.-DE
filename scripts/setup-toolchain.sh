#!/usr/bin/env bash
# Stellt die Buildumgebung her (§67: der Workspace bleibt frei von Toolchain-Ballast).
# SDK und NuGet-Zwischenspeicher liegen bewusst unter .cache/ und werden nicht
# mit dem Projekt gespeichert - sie sind jederzeit reproduzierbar herstellbar.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
export DOTNET_ROOT="${ROOT}/.cache/dotnet"
export NUGET_PACKAGES="${ROOT}/.cache/nuget"
export DOTNET_CLI_HOME="${ROOT}/.cache/dotnet-home"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export PATH="${DOTNET_ROOT}:${PATH}"

DOTNET_CHANNEL="8.0"

if [[ ! -x "${DOTNET_ROOT}/dotnet" ]]; then
  echo "[setup] .NET SDK ${DOTNET_CHANNEL} wird nach ${DOTNET_ROOT} installiert ..."
  mkdir -p "${DOTNET_ROOT}"
  curl -sSL https://dot.net/v1/dotnet-install.sh -o "${ROOT}/.cache/dotnet-install.sh"
  chmod +x "${ROOT}/.cache/dotnet-install.sh"
  "${ROOT}/.cache/dotnet-install.sh" --channel "${DOTNET_CHANNEL}" --install-dir "${DOTNET_ROOT}" --no-path
else
  echo "[setup] .NET SDK bereits vorhanden: $(dotnet --version)"
fi

mkdir -p "${NUGET_PACKAGES}"
echo "[setup] bereit. DOTNET_ROOT=${DOTNET_ROOT}"
