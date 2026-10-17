#!/usr/bin/env python3
"""WCAG contrast ratio checker for game UI colours. Standard library only.

Usage:
    python contrast_check.py "#FFFFFF" "#1A1A1A"            # one pair
    python contrast_check.py "#FFFFFF" "#1A1A1A" --large      # large-text threshold (3:1)
    python contrast_check.py --pairs theme_pairs.txt          # lines: <fg> <bg> [label]; '//' comments

Colours: #RGB, #RRGGBB or #RRGGBBAA. A translucent foreground is composited over the background.
A translucent background must be resolved against an opaque scene colour with --over (check the
brightest and the darkest scene the panel can appear over); otherwise the check refuses to run.

Thresholds (WCAG 2.x and Xbox Accessibility Guideline 102): 4.5:1 body text and essential icons,
3:1 large text and inactive elements, 7:1 for a high-contrast mode. The ratio is computed from
linearised sRGB relative luminance. It is never rounded up.
"""
from __future__ import annotations

import argparse
import sys


def parse(hex_colour: str) -> tuple[float, float, float, float]:
    s = hex_colour.strip().lstrip("#")
    if len(s) in (3, 4):
        s = "".join(ch * 2 for ch in s)
    if len(s) not in (6, 8):
        raise ValueError(f"bad colour: {hex_colour}")
    r, g, b = (int(s[i:i + 2], 16) / 255 for i in (0, 2, 4))
    a = int(s[6:8], 16) / 255 if len(s) == 8 else 1.0
    return r, g, b, a


def over(fg: tuple[float, float, float, float], bg: tuple[float, float, float, float]):
    a = fg[3]
    return tuple(fg[i] * a + bg[i] * (1 - a) for i in range(3)) + (1.0,)


def luminance(c) -> float:
    def lin(v: float) -> float:
        return v / 12.92 if v <= 0.04045 else ((v + 0.055) / 1.055) ** 2.4
    r, g, b = (lin(v) for v in c[:3])
    return 0.2126 * r + 0.7152 * g + 0.0722 * b


def ratio(fg_hex: str, bg_hex: str, under_hex: str | None = None) -> float:
    bg = parse(bg_hex)
    if under_hex:
        under = parse(under_hex)
        if under[3] < 1.0:
            raise ValueError(f"--over colour must be opaque: {under_hex}")
        bg = over(bg, under)
    elif bg[3] < 1.0:
        raise ValueError(f"background {bg_hex} is translucent; pass --over <opaque scene colour>")
    fg = over(parse(fg_hex), bg)
    l1, l2 = sorted((luminance(fg), luminance(bg)), reverse=True)
    return (l1 + 0.05) / (l2 + 0.05)


def verdict(r: float, large: bool) -> str:
    need = 3.0 if large else 4.5
    parts = [f"{r:.2f}:1", "PASS" if r >= need else "FAIL", f"(needs {need}:1"]
    parts[-1] += ", high-contrast mode needs 7:1)" if r < 7.0 else ")"
    return " ".join(parts)


def main(argv: list[str]) -> int:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("fg", nargs="?")
    ap.add_argument("bg", nargs="?")
    ap.add_argument("--large", action="store_true", help="use the large-text threshold (3:1)")
    ap.add_argument("--over", help="scene colour behind a translucent background colour")
    ap.add_argument("--pairs", help="file with '<fg> <bg> [label]' per line")
    args = ap.parse_args(argv)
    failures = 0
    rows: list[tuple[str, str, str]] = []
    if args.pairs:
        with open(args.pairs, encoding="utf-8") as fh:
            for line in fh:
                line = line.strip()
                if not line or line.startswith("//"):
                    continue
                bits = line.split(maxsplit=2)
                if len(bits) < 2:
                    print(f"error: expected '<fg> <bg> [label]': {line}", file=sys.stderr)
                    return 2
                rows.append((bits[0], bits[1], bits[2] if len(bits) > 2 else ""))
        if not rows:
            print("error: no colour pairs found in --pairs file", file=sys.stderr)
            return 2
    elif args.fg and args.bg:
        rows.append((args.fg, args.bg, ""))
    else:
        ap.print_help()
        return 2
    for fg, bg, label in rows:
        try:
            r = ratio(fg, bg, args.over)
        except ValueError as exc:
            print(f"error: {exc}", file=sys.stderr)
            return 2
        line = verdict(r, args.large)
        failures += "FAIL" in line
        print(f"{fg} on {bg}{' over ' + args.over if args.over else ''}: {line} {label}".rstrip())
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
