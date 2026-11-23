# Vendored Forma 1.0.2 (design review only)

- Source repository: kemiller2002/forma
- Commit: d7c499e ("Add reusable contextual AI assistance UX requirements"), full SHA d7c499e3d2d46fb564a358f2b7a11deeeb66b38e
- Package version: 1.0.2 (`package.json`; `dist/runtime.js` exports `FORMA_VERSION = "1.0.2"`)
- Built with the repository's own build (`dist/`), not hand-edited.
- Purpose: vendored **for design review only** so the Unbound screen prototypes in
  `../../screens/` render offline. This is not a production dependency, and these files
  must not be edited here; changes belong upstream.
- License: PolyForm Noncommercial 1.0.0, see `LICENSE` and `NOTICE` (copied unchanged).

## Files (byte-identical copies of `dist/`, SHA-256)

| Path | Source | SHA-256 |
| --- | --- | --- |
| `all.css` | `dist/all.css` | `c38f80d8dedb6b2f6a896f0200ae34a60d22c4ff28dd73422cf40d2d16c87950` |
| `brands/example-harbor.css` | `dist/brands/example-harbor.css` | `67944695734059fa555f540b540b5aa83cf3cfdc00f9893496fb599df08381e0` |
| `LICENSE` | `LICENSE` | `ffcca38841adb694b6f380647e15f17c446a4d1656fed51a1e2041d064c94cc8` |
| `NOTICE` | `NOTICE` | `4c846a85dcfd3c39e894ab832db2e106805f94cef6870ab2d36026e7d49d86e2` |

`runtime.js` is not vendored: no pattern used by the screens requires it (it only registers
the Forma version in a global runtime registry). Dialogs and flyouts open with native
`commandfor`/`command` invokers.

`brands/example-harbor.css` is Forma's compiled example Brand Manifest; the screens use it
as the stand-in for the fictional sponsor brand "Harbor Giving" (`data-forma-brand="example-harbor"`).
icons-registry.json: kemiller2002/forma icons/registry.json @ d7c499e (80 icons, 24px grid, 1.8 stroke). Rendered with Forma's .forma-icon markup.
