#!/usr/bin/env python3
"""Summarise pgbench per-transaction logs into a results table (pure functions; I/O in main).

pgbench -l line: client_id transaction_no latency_us script_no epoch epoch_us [schedule_lag_us]
Script numbers: 0 = grant approval, 1 = payment release, 2 = hold publication.
Usage: python3 -I -B analyse.py <results-dir>  -> prints Markdown and writes summary.csv
"""
from __future__ import annotations

import csv
import re
import sys
from collections import defaultdict
from pathlib import Path
from typing import Dict, Iterable, List, Tuple

SCRIPTS = {0: "grant approval", 1: "payment release", 2: "hold publication"}


def pct(sorted_vals: List[float], p: float) -> float:
    return sorted_vals[min(len(sorted_vals) - 1, int(round(p * (len(sorted_vals) - 1))))] if sorted_vals else float("nan")


def parse_log(lines: Iterable[str]) -> Iterable[Tuple[int, float, float, float]]:
    """(script, latency_ms, epoch_seconds, schedule_lag_ms)"""
    for line in lines:
        f = line.split()
        if len(f) >= 6:
            yield int(f[3]), int(f[2]) / 1000.0, int(f[4]) + int(f[5]) / 1e6, (int(f[6]) / 1000.0 if len(f) > 6 else 0.0)


def summarise(rows: List[Tuple[int, float, float, float]]) -> Dict[int, dict]:
    if not rows:
        return {}
    span = max(r[2] for r in rows) - min(r[2] for r in rows) or 1.0
    by: Dict[int, List[float]] = defaultdict(list)
    lag: Dict[int, List[float]] = defaultdict(list)
    for s, lat, _, lg in rows:
        by[s].append(lat)
        lag[s].append(lg)
    def one(s: int) -> dict:
        v, l = sorted(by[s]), sorted(lag[s])
        return {"n": len(v), "tps": len(v) / span, "p50": pct(v, .5), "p95": pct(v, .95), "p99": pct(v, .99), "max": v[-1],
                "lag_p99": pct(l, .99)}
    return {s: one(s) for s in sorted(by)}


def failures(text: str) -> str:
    m = re.search(r"number of failed transactions: (\d+) \(([\d.]+)%\)", text)
    return f"{m.group(1)} ({m.group(2)}%)" if m else "n/a"


def main(argv) -> int:
    out = Path(argv[1])
    lag = {r["scenario"]: r for r in csv.DictReader((out / "outbox-lag.csv").open())}
    rows_out = []
    for txt in sorted(out.glob("S*.txt")):
        if txt.name.endswith(".dispatch.txt"):
            continue
        name = txt.stem
        rows = [r for p in out.glob(f"{name}.*") if p.suffix not in (".txt",) for r in parse_log(p.read_text().splitlines())]
        summ = summarise(rows)
        total_tps = sum(x["tps"] for x in summ.values())
        for s, x in summ.items():
            rows_out.append({"scenario": name, "script": SCRIPTS.get(s, s), "tx": x["n"], "tps": round(x["tps"], 1),
                             "total_tps": round(total_tps, 1), "p50_ms": round(x["p50"], 1), "p95_ms": round(x["p95"], 1),
                             "p99_ms": round(x["p99"], 1), "max_ms": round(x["max"], 1), "sched_lag_p99_ms": round(x["lag_p99"], 1),
                             "failed": failures(txt.read_text()),
                             "outbox_lag_p99_ms": lag.get(name, {}).get("lag_p99_ms", ""),
                             "outbox_undelivered": lag.get(name, {}).get("undelivered_after_drain", "")})
    hdr = list(rows_out[0].keys())
    with (out / "summary.csv").open("w", newline="") as fh:
        w = csv.DictWriter(fh, fieldnames=hdr); w.writeheader(); w.writerows(rows_out)
    print("| " + " | ".join(hdr) + " |\n|" + "---|" * len(hdr))
    print("\n".join("| " + " | ".join(str(r[h]) for h in hdr) + " |" for r in rows_out))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
