#!/usr/bin/env node
// Generates the static screen prototypes in ../screens from the per-screen modules in ./screens.
// Pure: each screen module is data + pure template functions; this script only renders and writes.
// Usage: node tools/build-screens.mjs
import { writeFile, mkdir, readFile } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import path from "node:path";
import { page } from "./lib/shell.mjs";
import { stateSection } from "./lib/markup.mjs";

const here = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.resolve(here, "../screens");

const SCREEN_IDS = ["S-001", "S-002", "S-003", "S-004", "S-005", "S-006", "S-007", "S-008", "S-009", "S-010", "S-011", "S-012", "S-013", "W-001"];

const prefixFor = (screenId, i) => `${screenId.toLowerCase().replace("-", "")}-${i + 1}`;

/** Expand state definitions into rendered, uniquely prefixed state sections. */
const renderStates = (screen) =>
  screen.states.map((def, i, all) => {
    const id = prefixFor(screen.id, i);
    return {
      id,
      title: def.title,
      html: stateSection({ id, n: i + 1, total: all.length, persona: def.persona, title: def.title, demonstrates: def.demonstrates, body: def.body(id) }),
    };
  });

const toManifestEntry = (screen) => ({
  page: screen.file,
  requirement_ids: screen.reqIds,
  states: screen.states.map((s, i) => ({ id: prefixFor(screen.id, i), title: s.title, persona: s.persona, demonstrates: s.demonstrates })),
  requirement_coverage: screen.coverage,
  gaps: screen.gaps,
});

const loadScreen = async (id) => (await import(`./screens/${id}.mjs`)).default;

const main = async () => {
  const requested = process.argv.slice(2);
  const ids = requested.length ? SCREEN_IDS.filter((id) => requested.includes(id)) : SCREEN_IDS;
  const screens = await Promise.all(ids.map(loadScreen));
  await mkdir(outDir, { recursive: true });
  await Promise.all(
    screens.map((s) => writeFile(path.join(outDir, s.file), s.render ? s.render(renderStates(s)) : page({ ...s, states: renderStates(s) })))
  );
  const manifestPath = path.join(outDir, "page-manifest.json");
  const previous = requested.length ? JSON.parse(await readFile(manifestPath, "utf8").catch(() => "[]")) : [];
  const fresh = screens.map(toManifestEntry);
  const merged = [...previous.filter((p) => !fresh.some((f) => f.page === p.page)), ...fresh].sort((a, b) => a.page.localeCompare(b.page));
  await writeFile(manifestPath, JSON.stringify(merged, null, 2) + "\n");
  console.log(`wrote ${screens.length} page(s) to ${outDir}`);
};

main().catch((e) => {
  console.error(e);
  process.exit(1);
});
