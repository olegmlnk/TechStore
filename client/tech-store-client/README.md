# TechStoreClient

Frontend application for TechStore built on Angular CLI 21.

## Main scripts

Use the npm scripts below from `client/tech-store-client`:

```bash
npm run dev
npm run build
npm run preview
npm run lint
```

- `npm run dev` starts the Angular development server on `http://localhost:4200/`.
- `npm run build` creates a production bundle in `dist/tech-store-client/`.
- `npm run preview` starts the built SSR artifact locally.
- `npm run lint` runs ESLint for TypeScript and Angular templates.

## Testing

Unit and end-to-end tests are available through npm scripts:

```bash
npm run test:unit
npm run test:unit:coverage
npm run test:e2e
```

## Environment status

The application shows the current build mode in the footer:

- development mode displays `Development`
- production build plus `npm run preview` displays `Production Mode`

This value is provided by Angular environment files:

- `src/environments/environment.ts`
- `src/environments/environment.production.ts`

## Build artifact snapshot

The current production build generates hashed browser assets such as:

- `main-A4DALZSG.js`
- `styles-EYE3NDYK.css`
- `chunk-C6MVCWWG.js`

Size comparison from the latest build:

- `src/`: `114767` bytes
- `dist/`: `2577995` bytes

The larger `dist/` size is expected because it contains compiled browser bundles, SSR server bundles, and prerendered output.

## Notes for the lab

- Local `.env` and `.env.production` files are kept outside Git and ignored by the repository.
- The runtime environment status for the UI is intentionally implemented through Angular environments, not `import.meta.env`.
- A short explanation for this decision is documented in [BUILD-SYSTEM-NOTE.md](./BUILD-SYSTEM-NOTE.md).

## CI/CD and Vercel

The repository includes a GitHub Actions workflow at `/.github/workflows/ci-cd.yml`.

- On every push or pull request to `main` and `develop`, the pipeline installs dependencies, runs ESLint, runs unit tests, builds the Angular app, and uploads the build artifact.
- On pushes to `develop`, the workflow deploys a preview build to Vercel.
- On pushes to `main`, the workflow deploys a production build to Vercel.

Before enabling deployment, add these repository secrets in GitHub:

- `VERCEL_TOKEN`
- `VERCEL_ORG_ID`
- `VERCEL_PROJECT_ID`

In the Vercel project settings, set the Root Directory to `client/tech-store-client`.
