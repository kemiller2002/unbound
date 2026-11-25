# Public-data replay (F#)

Replays sponsors' public Form 990 grant data through Unbound's reference screening policy. It measures the
eligibility-exception rate, how often eligibility determinations can be reused, revocation events, and sanctions
alert and recall rates. It also runs a safety test with injected bad cases. Interpretation is in
[report 14](../14-PUBLIC-DATA-REPLAY.md); generated tables are in [report 14a](../14a-PUBLIC-DATA-REPLAY-TABLES.md).

This is an analysis tool, not product code. It is written in F# to keep the technology footprint to one language.
Its pure core covers parsing, screening, name matching, statistics and rendering; file and network effects live only
in `Program.fs`.

## Run

Requires the .NET 10 SDK (`global.json`).

```bash
cd requirements/convergence/2026-11-24-master/replay
DATA=/path/to/scratch/replay-data                     # ~700 MB of public data; not committed
dotnet test tests/Unbound.Replay.Tests                # 20 fixture tests, no data needed
dotnet run --project src/Unbound.Replay -c Release -- fetch "$DATA" inputs/returns.csv
dotnet run --project src/Unbound.Replay -c Release -- run "$DATA" inputs/returns.csv \
  out/replay-results.json ../14a-PUBLIC-DATA-REPLAY-TABLES.md   # ~10 min on 4 cores
```

The output is deterministic: two runs over the same inputs are byte-identical. `out/replay-results.json` records
the SHA-256 of every input, so a rerun on newer IRS or OFAC files can be told apart from a code change.

## Inputs

| File | Source |
|---|---|
| `inputs/returns.csv` | Sponsor EINs, tax years and e-file object IDs (from the IRS `index_YYYY.csv` files) |
| Form 990 XML | `gt990datalake-rawdata.s3.amazonaws.com/EfileData/XmlFiles/{object_id}_public.xml` |
| Pub 78 | `apps.irs.gov/pub/epostcard/data-download-pub78.zip` |
| Automatic revocations | `apps.irs.gov/pub/epostcard/data-download-revocation.zip` |
| EO BMF | `www.irs.gov/pub/irs-soi/eo1.csv` … `eo4.csv` |
| OFAC SDN and aliases | `www.treasury.gov/ofac/downloads/sdn.csv`, `alt.csv` |

The screening policy in `Screening.fs` is a design artifact for measurement. It is not legal advice, and counsel
confirms the class rules (OD-P-LEG).
