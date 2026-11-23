#!/usr/bin/env node
// Builds the v2 prototypes: one HTML page per screen state, plus an index.
// Usage: node tools/build.mjs   (from designs/v2)
import { writeFile, mkdir, readFile } from "node:fs/promises";
import { fileURLToPath, pathToFileURL } from "node:url";
import path from "node:path";
import { appPage } from "./lib/ui.mjs";

const here = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(here, "..");
const SCREENS = (process.env.SCREENS ?? "S-001").split(",");

const load = async (id) => (await import(pathToFileURL(path.join(here, "screens", `${id}.mjs`)).href)).default;

const pagesOf = (screen) =>
  screen.states.map((s) => ({
    file: path.join("screens", screen.slug, `${s.id}.html`),
    html: s.html ?? appPage({ ...s.page, docTitle: `${screen.title}: ${s.title}` }),
    meta: { screen: screen.id, requirement: screen.requirement, state: s.id, title: s.title, persona: s.persona, scenarios: s.scenarios, kind: s.kind ?? "app" },
  }));

const index = (pages) => `<!doctype html><html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Unbound v2 design prototypes</title><link rel="stylesheet" href="../vendor/forma-1.0.2/all.css"></head>
<body class="forma-site"><main class="forma-site__main" id="main"><div class="forma-container forma-prose"><h1>Unbound v2 design prototypes</h1><p>Static design prototypes for report 18. Synthetic data. One page per state.</p>
<ul>${pages.map((p) => `<li><a href="${p.file}">${p.meta.screen} ${p.meta.title}</a> (${p.meta.scenarios.join(", ")})</li>`).join("")}</ul></div></main></body></html>`;

const screens = await Promise.all(SCREENS.map(load));
const pages = screens.flatMap(pagesOf);
await Promise.all(pages.map(async (p) => { await mkdir(path.dirname(path.join(root, p.file)), { recursive: true }); await writeFile(path.join(root, p.file), p.html); }));
// Merge with pages of screens not rebuilt this run, so partial builds do not drop other screens.
const prior = await readFile(path.join(root, "pages.json"), "utf8").then(JSON.parse).catch(() => []);
const metas = [...prior.filter((m) => !SCREENS.includes(m.screen)), ...pages.map((p) => ({ file: p.file, ...p.meta }))].sort((a, b) => a.file.localeCompare(b.file));
await writeFile(path.join(root, "index.html"), index(metas.map((m) => ({ file: m.file, meta: m }))));
await writeFile(path.join(root, "pages.json"), JSON.stringify(metas, null, 2) + "\n");
console.log(`built ${pages.length} pages`);
