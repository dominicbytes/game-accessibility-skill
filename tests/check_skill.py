#!/usr/bin/env python3
"""Static checks for the game-accessibility skill. Standard library only.

Checks:
  1. SKILL.md frontmatter: name format, name == folder, description length/trigger style.
  2. SKILL.md length (progressive disclosure budget).
  3. Every relative markdown link inside the skill folder resolves.
  4. settings-registry.json is valid: unique ids, defaults within range/values, presets reference
     real keys with legal values; engine copies of the registry are identical.
  5. Option IDs used in references exist in option-catalog.md; signal IDs are S001-S157;
     backticked setting keys that look like registry keys exist in the registry.
  6. Tier consistency: Advanced labels in signal-routing.md and topic option tables match
     option-catalog.md (no silent scope cuts to A, no catalogue-A option listed as B/C).
  7. The skill folder's LICENSE matches the repository LICENSE.
  8. Policy guard: no sample code writes an assist flag into saves or reads assist state in
     achievement code (heuristic grep).
Exit code 1 on any failure.
"""
from __future__ import annotations

import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SKILL = os.path.join(ROOT, "skills", "game-accessibility")
FAILS: list[str] = []
WARNS: list[str] = []


def fail(msg: str) -> None:
    FAILS.append(msg)


def read(path: str) -> str:
    with open(path, encoding="utf-8") as fh:
        return fh.read()


def md_files(base: str):
    for dp, _, fns in os.walk(base):
        for fn in fns:
            if fn.endswith(".md"):
                yield os.path.join(dp, fn)


def check_frontmatter() -> None:
    text = read(os.path.join(SKILL, "SKILL.md"))
    m = re.match(r"^---\n(.*?)\n---\n", text, re.S)
    if not m:
        fail("SKILL.md: missing YAML frontmatter")
        return
    fm = m.group(1)
    name = re.search(r"^name:\s*(\S+)\s*$", fm, re.M)
    desc = re.search(r"^description:\s*(.+)$", fm, re.M)
    if not name or not re.fullmatch(r"[a-z0-9]+(-[a-z0-9]+)*", name.group(1)) or len(name.group(1)) > 64:
        fail("SKILL.md: invalid name")
    elif name.group(1) != os.path.basename(SKILL):
        fail("SKILL.md: name does not match folder")
    if not desc:
        fail("SKILL.md: missing description")
    else:
        d = desc.group(1).strip()
        if len(d) > 1024:
            fail(f"SKILL.md: description is {len(d)} chars (> 1024)")
        if not d.startswith("Use when"):
            fail("SKILL.md: description should start with 'Use when'")
        if "<" in d or ">" in d:
            fail("SKILL.md: description must not contain XML-like brackets")
    lines = text.count("\n") + 1
    if lines > 250:
        fail(f"SKILL.md: {lines} lines (> 250)")
    print(f"SKILL.md: {lines} lines, description {len(desc.group(1).strip()) if desc else 0} chars")


LINK_RE = re.compile(r"\[[^\]]*\]\(([^)#\s]+)(#[^)]*)?\)")


def check_links() -> None:
    count = 0
    for path in md_files(SKILL):
        for target, _ in LINK_RE.findall(read(path)):
            if re.match(r"^[a-z]+:", target):
                continue
            count += 1
            resolved = os.path.normpath(os.path.join(os.path.dirname(path), target))
            if not os.path.exists(resolved):
                fail(f"broken link in {os.path.relpath(path, ROOT)} -> {target}")
            elif not resolved.startswith(SKILL):
                fail(f"link leaves the skill folder: {os.path.relpath(path, ROOT)} -> {target}")
    print(f"links checked: {count}")


def load_registry() -> dict:
    path = os.path.join(SKILL, "assets", "settings-registry.json")
    reg = json.loads(read(path))
    ids = [s["id"] for s in reg["settings"]]
    if len(ids) != len(set(ids)):
        fail("registry: duplicate ids")
    by_id = {s["id"]: s for s in reg["settings"]}

    def legal(s: dict, v) -> bool:
        t = s["type"]
        if t == "bool":
            return isinstance(v, bool)
        if t in ("float", "int"):
            if isinstance(v, bool) or not isinstance(v, (int, float)):
                return False
            return s.get("min", v) <= v <= s.get("max", v)
        if t == "enum":
            return v in s["values"]
        return isinstance(v, str)

    for s in reg["settings"]:
        for field in ("id", "category", "type", "default", "label", "description", "catalog", "signals", "scope", "apply"):
            if field not in s:
                fail(f"registry {s.get('id')}: missing {field}")
        if not legal(s, s["default"]):
            fail(f"registry {s['id']}: default {s['default']!r} not legal")
    for p in reg["presets"]:
        for k, v in p["changes"].items():
            if k not in by_id:
                fail(f"preset {p['id']}: unknown key {k}")
            elif not legal(by_id[k], v):
                fail(f"preset {p['id']}: illegal value {v!r} for {k}")
    canon = json.loads(read(path))
    for dp, _, fns in os.walk(os.path.join(SKILL, "engines")):
        for fn in fns:
            if fn in ("settings_registry.json", "settings-registry.json"):
                other = json.loads(read(os.path.join(dp, fn)))
                if other != canon:
                    fail(f"engine registry copy differs: {os.path.relpath(os.path.join(dp, fn), ROOT)}")
    print(f"registry: {len(ids)} settings, {len(reg['presets'])} presets")
    return by_id


def check_ids(registry: dict) -> None:
    catalog = read(os.path.join(SKILL, "references", "option-catalog.md"))
    known = set(re.findall(r"^\|\s*([VHMCNPOFDKLR]\d{2})\s*\|", catalog, re.M))
    if len(known) < 250:
        fail(f"option-catalog.md: only {len(known)} option rows found")
    id_re = re.compile(r"(?<![#A-Za-z0-9_\-])([VHMCNPOFDKLR]\d{2})(?![\dA-Fa-f])")
    sig_re = re.compile(r"\bS(\d{3})\b")
    key_re = re.compile(r"`((?:text|subtitles|audio|visual|motion|guidance|hud|effects|notifications|content|input|assist|narration|memory|system)\.[a-z_]+)`")
    unknown_ids, unknown_keys = set(), set()
    for path in md_files(os.path.join(SKILL, "references")) :
        text = read(path)
        rel = os.path.relpath(path, ROOT)
        for oid in id_re.findall(text):
            if oid not in known:
                unknown_ids.add((rel, oid))
        for n in sig_re.findall(text):
            if not 1 <= int(n) <= 157:
                fail(f"{rel}: signal S{n} out of range")
        for key in key_re.findall(text):
            if key not in registry:
                unknown_keys.add((rel, key))
    for rel, oid in sorted(unknown_ids):
        WARNS.append(f"{rel}: option id {oid} not in catalog (check it is not an XAG/GAG reference)")
    for rel, key in sorted(unknown_keys):
        WARNS.append(f"{rel}: setting key {key} not in registry (fine if marked game-specific)")
    print(f"catalog option ids: {len(known)}")


TIER_ID = r"[VHMCNPOFDKLR]\d{2}"


def _expand_ids(segment: str) -> list[str]:
    out = []
    for m in re.finditer(rf"({TIER_ID})\s*[–-]\s*({TIER_ID})|({TIER_ID})", segment):
        if m.group(3):
            out.append(m.group(3))
        elif m.group(1)[0] == m.group(2)[0]:
            for n in range(int(m.group(1)[1:]), int(m.group(2)[1:]) + 1):
                out.append(f"{m.group(1)[0]}{n:02d}")
    return out


def check_tiers() -> None:
    """Advanced (A) is a scope-decision tier. The router may label an option A only when the
    catalogue does, and must not list a catalogue-Advanced option as B or C. Topic option tables
    must repeat the catalogue tier exactly."""
    refs = os.path.join(SKILL, "references")
    catalog = read(os.path.join(refs, "option-catalog.md"))
    tiers = {m.group(1): m.group(2).strip()[:1]
             for m in re.finditer(rf"^\|\s*({TIER_ID})\s*\|[^|]*\|\s*([^|]*?)\s*\|", catalog, re.M)}
    checked = 0
    router = read(os.path.join(refs, "signal-routing.md"))
    for line in router.splitlines():
        m = re.match(r"^\|\s*(S\d{3})[^|]*\|[^|]*\|([^|]*)\|", line)
        if not m:
            continue
        sig, opts = m.group(1), re.sub(r"\([^)]*\)", "", m.group(2))
        for part in opts.split(";"):
            pm = re.match(r"\s*([BCA]):(.*)", part)
            if not pm:
                continue
            tier, ids = pm.group(1), pm.group(2).split(". ")[0]
            for oid in _expand_ids(ids):
                cat_tier = tiers.get(oid)
                if cat_tier is None:
                    continue
                checked += 1
                if (tier == "A") != (cat_tier == "A"):
                    fail(f"signal-routing.md {sig}: {oid} listed as {tier}, catalogue tier is {cat_tier}")
    topics = os.path.join(refs, "topics")
    for fn in sorted(os.listdir(topics)):
        if not fn.endswith(".md"):
            continue
        tier_col = None
        for line in read(os.path.join(topics, fn)).splitlines():
            cols = [c.strip() for c in line.strip().strip("|").split("|")]
            if line.startswith("| ID |"):
                tier_col = cols.index("Tier") if "Tier" in cols else None
                continue
            if not line.startswith("|"):
                tier_col = None
                continue
            if tier_col is None or len(cols) <= tier_col:
                continue
            for oid in re.findall(TIER_ID, cols[0]):
                cat_tier = tiers.get(oid)
                if cat_tier is None:
                    continue
                checked += 1
                if cols[tier_col][:1] != cat_tier:
                    fail(f"topics/{fn}: {oid} tier {cols[tier_col]!r}, catalogue tier is {cat_tier}")
    print(f"tier labels checked: {checked}")


def check_license() -> None:
    """The installable folder must carry the same MIT notice as the repository."""
    inner = os.path.join(SKILL, "LICENSE")
    if not os.path.exists(inner):
        fail("skills/game-accessibility/LICENSE is missing")
    elif read(inner) != read(os.path.join(ROOT, "LICENSE")):
        fail("skills/game-accessibility/LICENSE differs from the repository LICENSE")


def check_policy_guard() -> None:
    bad = re.compile(r"(achiev|trophy|unlock).{0,80}(assist|a11y_settings|difficulty)|save.{0,40}(assisted|used_assist|ever_assist)", re.I)
    for dp, _, fns in os.walk(os.path.join(SKILL, "engines")):
        for fn in fns:
            if fn.endswith((".gd", ".cs", ".cpp", ".h", ".js")):
                text = read(os.path.join(dp, fn))
                for line in text.splitlines():
                    if bad.search(line) and not line.strip().startswith(("#", "//", "*", "///")):
                        fail(f"policy guard: {os.path.relpath(os.path.join(dp, fn), ROOT)}: {line.strip()[:100]}")


def main() -> int:
    check_frontmatter()
    check_links()
    reg = load_registry()
    check_ids(reg)
    check_tiers()
    check_license()
    check_policy_guard()
    for w in WARNS:
        print("WARN", w)
    for f in FAILS:
        print("FAIL", f)
    print(f"{len(FAILS)} failure(s), {len(WARNS)} warning(s)")
    return 1 if FAILS else 0


if __name__ == "__main__":
    sys.exit(main())
