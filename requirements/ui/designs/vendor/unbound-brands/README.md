unbound-default: Unbound's default ("unbranded sponsor") theme. Balanced Charcoal and Bronze.
- Colour, font and radius values are copied from kemiller2002/echelon-design-system theme/echelon.brand.json @ 2c0e82e (Echelon theme 1.1.0; values verified in brand/AUDIT.md against Forma 0.14 sources). Only id, identity and terms were changed.
- unbound-default.css was compiled with Forma's own BrandCompiler (Forma @ d7c499e, tools/BrandCompiler). The same compiler reproduces vendor/forma-1.0.2/brands/example-harbor.css byte for byte, which checks the build.
- The Echelon application layer (echelon-app.css) is NOT used: it is scoped to data-forma-brand="echelon" and would not apply to sponsor brands (finding UXD-SYS, report 18).
Prototype asset; not a released token package.
