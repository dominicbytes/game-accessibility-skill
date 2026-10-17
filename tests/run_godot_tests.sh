#!/usr/bin/env bash
# Runs the Godot/Redot sample tests headless and muted.
# Usage: REDOT_BIN=/path/to/redot tests/run_godot_tests.sh   (GODOT_BIN is used when REDOT_BIN is unset)
set -u
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BIN="${REDOT_BIN:-${GODOT_BIN:-}}"
if [ -z "$BIN" ]; then
  echo "BLOCKED: set REDOT_BIN or GODOT_BIN to a Redot/Godot 4.3+ editor binary." >&2
  exit 2
fi
PROJECT="$ROOT/tests/godot"
SRC="$ROOT/skills/game-accessibility/engines/godot/addons"

rm -rf "$PROJECT/addons"
mkdir -p "$PROJECT/addons"
cp -R "$SRC/." "$PROJECT/addons/"

"$BIN" --version

# Register class_name scripts (the global class cache lives in .godot/, which is not committed).
# --import quits by itself once the scan is done; --quit-after is not used here because it can stop
# the editor before the threaded scan registers the classes. The wall-clock timeout bounds the run.
# Output is discarded: Redot 26.3's editor reports RID leaks at exit and sometimes crashes during
# shutdown after the scan finished. The class cache is checked instead, with up to 3 attempts.
CACHE="$PROJECT/.godot/global_script_class_cache.cfg"
for attempt in 1 2 3; do
  timeout 180 "$BIN" --headless --audio-driver Dummy --path "$PROJECT" --import >/dev/null 2>&1
  IMPORT_STATUS=$?
  if [ -f "$CACHE" ] && grep -q "A11ySettingsMenu" "$CACHE"; then
    [ "$IMPORT_STATUS" -ne 0 ] && echo "note: import exited with $IMPORT_STATUS after registering classes (attempt $attempt)" >&2
    break
  fi
  echo "note: class cache missing after import attempt $attempt (exit $IMPORT_STATUS)" >&2
done
if ! grep -q "A11ySettingsMenu" "$CACHE" 2>/dev/null; then
  echo "BLOCKED: could not register the addon classes with --import." >&2
  exit 2
fi

LOG="$(mktemp)"
timeout 180 "$BIN" --headless --audio-driver Dummy --path "$PROJECT" --script res://run_tests.gd --quit-after 600 2>&1 | tee "$LOG"
STATUS=${PIPESTATUS[0]}

# Engine or script errors fail the run even when every assertion passed.
if grep -Eq "SCRIPT ERROR|Parse Error|^ERROR:|^WARNING:" "$LOG"; then
  echo "FAIL: engine errors or warnings were printed (see above)." >&2
  [ "$STATUS" -eq 0 ] && STATUS=1
fi
rm -f "$LOG"
exit "$STATUS"
