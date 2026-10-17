#!/usr/bin/env bash
# Compiles the Unity sample's pure C# Core against the netstandard2.1 reference assembly, then builds and runs
# the behaviour checks (CoreChecks.cs) on an installed .NET runtime. No Unity Editor or .NET SDK required.
#
# Needs: a Roslyn csc.exe (Visual Studio or Build Tools; found via vswhere, or set CSC=/path/to/csc.exe),
#        `dotnet` with any Microsoft.NETCore.App runtime >= 6, curl, and unzip or python.
# Windows/Git Bash is the tested environment. Exit code is non-zero on any failure.
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$HERE/../.." && pwd)"
CORE_DIR="$ROOT/skills/game-accessibility/engines/unity/Runtime/Core"
REGISTRY="$ROOT/skills/game-accessibility/assets/settings-registry.json"
CACHE="$HERE/.cache"
BUILD="$HERE/.build"
NS_VERSION="2.1.0"
NS_DLL="$CACHE/netstandard.library.ref.$NS_VERSION/ref/netstandard2.1/netstandard.dll"

die() { echo "ERROR: $*" >&2; exit 1; }

# Native Windows path for csc.exe/dotnet arguments when running under Git Bash/MSYS.
winpath() { if command -v cygpath >/dev/null 2>&1; then cygpath -w "$1"; else printf '%s' "$1"; fi; }

# ---------------------------------------------------------------- find csc.exe
find_csc() {
  if [[ -n "${CSC:-}" ]]; then
    [[ -f "$CSC" ]] || die "CSC is set to '$CSC' but that file does not exist."
    printf '%s' "$CSC"; return
  fi
  local vswhere="/c/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe"
  if [[ -f "$vswhere" ]]; then
    local found
    found="$("$vswhere" -latest -products '*' -requires Microsoft.Component.MSBuild \
      -find 'MSBuild\**\Bin\Roslyn\csc.exe' 2>/dev/null | tr -d '\r' | head -n 1 || true)"
    if [[ -n "$found" ]]; then
      if command -v cygpath >/dev/null 2>&1; then found="$(cygpath -u "$found")"; fi
      [[ -f "$found" ]] && { printf '%s' "$found"; return; }
    fi
  fi
  if command -v csc >/dev/null 2>&1; then command -v csc; return; fi
  die "No Roslyn C# compiler found. Install Visual Studio or the VS Build Tools (MSBuild component), or set CSC=/path/to/csc.exe."
}
CSC_EXE="$(find_csc)"
echo "csc: $CSC_EXE"

# ---------------------------------------------------------------- find a .NET runtime
command -v dotnet >/dev/null 2>&1 || die "'dotnet' not found. Install any .NET runtime >= 6 (the SDK is not needed)."
RUNTIME_LINE="$(dotnet --list-runtimes | tr -d '\r' | grep '^Microsoft.NETCore.App ' | sort -t' ' -k2 -V | tail -n 1 || true)"
[[ -n "$RUNTIME_LINE" ]] || die "No Microsoft.NETCore.App runtime found (dotnet --list-runtimes)."
RT_VERSION="$(echo "$RUNTIME_LINE" | awk '{print $2}')"
RT_BASE="$(echo "$RUNTIME_LINE" | sed -E 's/^[^[]*\[(.*)\]$/\1/')"
RT_MAJOR="${RT_VERSION%%.*}"
(( RT_MAJOR >= 6 )) || die "Need Microsoft.NETCore.App >= 6, found $RT_VERSION."
if command -v cygpath >/dev/null 2>&1; then RT_DIR="$(cygpath -u "$RT_BASE")/$RT_VERSION"; else RT_DIR="$RT_BASE/$RT_VERSION"; fi
[[ -d "$RT_DIR" ]] || die "Runtime directory not found: $RT_DIR"
echo "runtime: Microsoft.NETCore.App $RT_VERSION"

# ---------------------------------------------------------------- netstandard2.1 reference (cached)
if [[ ! -f "$NS_DLL" ]]; then
  echo "Downloading NETStandard.Library.Ref $NS_VERSION (compile-time reference only) ..."
  mkdir -p "$CACHE"
  PKG="$CACHE/netstandard.library.ref.$NS_VERSION.nupkg"
  curl -fsSL -o "$PKG" "https://www.nuget.org/api/v2/package/NETStandard.Library.Ref/$NS_VERSION" \
    || die "Download failed. Fetch the package manually and extract ref/netstandard2.1/netstandard.dll to $NS_DLL"
  OUT="$CACHE/netstandard.library.ref.$NS_VERSION"
  mkdir -p "$OUT"
  if command -v unzip >/dev/null 2>&1; then
    unzip -q -o "$PKG" 'ref/netstandard2.1/netstandard.dll' -d "$OUT"
  else
    python -I -c "import sys, zipfile; zipfile.ZipFile(sys.argv[1]).extract('ref/netstandard2.1/netstandard.dll', sys.argv[2])" \
      "$(winpath "$PKG")" "$(winpath "$OUT")"
  fi
  [[ -f "$NS_DLL" ]] || die "Extraction failed: $NS_DLL missing."
fi

# ---------------------------------------------------------------- compile Core (netstandard2.1)
rm -rf "$BUILD"
mkdir -p "$BUILD"
CORE_RSP="$BUILD/core.rsp"
{
  echo "-nologo -nostdlib -langversion:8 -target:library -warn:4 -warnaserror+"
  echo "-out:\"$(winpath "$BUILD/GameAccessibility.Core.dll")\""
  echo "-r:\"$(winpath "$NS_DLL")\""
  for f in "$CORE_DIR"/*.cs; do echo "\"$(winpath "$f")\""; done
} > "$CORE_RSP"
echo "Compiling Core against netstandard2.1 ..."
"$CSC_EXE" -noconfig "@$(winpath "$CORE_RSP")" || die "Core failed to compile against netstandard2.1."

# ---------------------------------------------------------------- compile and run the checks
CHECKS_RSP="$BUILD/checks.rsp"
{
  echo "-nologo -nostdlib -langversion:8 -target:exe -warn:4"
  echo "-out:\"$(winpath "$BUILD/CoreChecks.dll")\""
  echo "-r:\"$(winpath "$BUILD/GameAccessibility.Core.dll")\""
  # Managed assemblies of the runtime (skip native binaries that live in the same folder).
  for f in "$RT_DIR"/*.dll; do
    b="$(basename "$f")"
    case "$b" in
      Microsoft.DiaSymReader.Native*|clr*|coreclr*|host*|mscordaccore*|mscordbi*|mscorrc*|msquic*|System.IO.Compression.Native*|Microsoft.VisualBasic.dll) continue ;;
    esac
    echo "-r:\"$(winpath "$f")\""
  done
  echo "\"$(winpath "$HERE/CoreChecks.cs")\""
} > "$CHECKS_RSP"
echo "Compiling CoreChecks against Microsoft.NETCore.App $RT_VERSION ..."
"$CSC_EXE" -noconfig "@$(winpath "$CHECKS_RSP")" || die "CoreChecks failed to compile."

cat > "$BUILD/CoreChecks.runtimeconfig.json" <<EOF
{"runtimeOptions":{"tfm":"net$RT_MAJOR.0","framework":{"name":"Microsoft.NETCore.App","version":"$RT_MAJOR.0.0"}}}
EOF

echo "Running behaviour checks ..."
dotnet "$(winpath "$BUILD/CoreChecks.dll")" "$(winpath "$REGISTRY")"
