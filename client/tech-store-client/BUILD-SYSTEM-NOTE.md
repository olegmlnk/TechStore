# Build System Note

This frontend stays on the current Angular CLI build system and is not migrated to a standalone Vite project.

Why this is still valid:

- Angular 21 uses the modern `@angular/build` pipeline.
- The Angular dev server is Vite-based under the hood.
- The production build also uses the newer Angular build stack instead of the legacy webpack-only flow.

Why the project uses `src/environments/*` instead of `import.meta.env`:

- In Angular CLI applications, the standard and officially supported way to switch runtime configuration between development and production is environment file replacement.
- Because of that, the UI status label is read from:
  - `src/environments/environment.ts`
  - `src/environments/environment.production.ts`

How this maps to the lab requirement:

- `npm run dev` shows `Development`
- `npm run build` + `npm run preview` shows `Production Mode`

Local `.env` and `.env.production` files were created for the lab checklist, but they are intentionally ignored by Git and are not the runtime source of truth for this Angular application.
