# Vendored Folio 0.4.2 (design review only)

- Source repository: kemiller2002/folio
- Commit: 56a18e1 ("Add AI-discoverable component and template assistance requirements")
- Package version: 0.4.2 (`src/runtime-version.js`)
- Purpose: vendored **for design review only** so the Unbound print-document prototypes in
  `../../documents/` can be rendered offline. This is not a production dependency, and these
  files must not be edited here; changes belong upstream.
- License: PolyForm Noncommercial 1.0.0, see `LICENSE` and `NOTICE` (copied unchanged).

## Files (byte-identical copies, SHA-256)

| Path | SHA-256 |
| --- | --- |
| `src/components/register.js` | `0e6aafdad5c0f6f96803cec773b5b2dbc858afaf2115c482a5b80be2e55df6c9` |
| `src/runtime.js` (imported by register.js) | `9f7791e55f4108fbcc28060333a282576aa538991422c66a82c08d004e7fbf5f` |
| `src/runtime-registry.js` (imported by runtime.js) | `33ac7841876e5b212cfbb6454f101dde1c5775596efc95344abcb99b81321eca` |
| `src/runtime-version.js` (imported by runtime.js) | `1ab0b3b9b698cefd1d2b581e752e8387bcc5d36a6a0d766c29c9a4b637882158` |
| `src/styles/print.css` | `28d72188b216049175c4690aa80cfda7a9827e261fef4b0bd75dd4a1cd4f1b19` |
| `LICENSE` | `ffcca38841adb694b6f380647e15f17c446a4d1656fed51a1e2041d064c94cc8` |
| `NOTICE` | `4c846a85dcfd3c39e894ab832db2e106805f94cef6870ab2d36026e7d49d86e2` |

The directory layout mirrors upstream `src/` because `register.js` imports `../runtime.js`.
`register.js` is an ES module, so documents must be served over HTTP (the verify script
starts a local static server); `file://` module loading is blocked by Chromium.
