# TechStore

[![CI/CD Pipeline](https://img.shields.io/github/actions/workflow/status/olegmlnk/TechStore/ci-cd.yml?branch=main&label=CI%2FCD%20Pipeline)](https://github.com/olegmlnk/TechStore/actions/workflows/ci-cd.yml)

**Live demo:** [tech-store-theta-three.vercel.app](https://tech-store-theta-three.vercel.app/)

TechStore - це MVP онлайн-магазину техніки з поділом на `client` (Angular) і `server` (ASP.NET Core + EF Core).

## Ідея MVP

Мінімально життєздатна версія продукту покриває базовий сценарій покупки:
- каталог товарів за категоріями;
- додавання товарів у кошик і wishlist;
- оформлення замовлення та відстеження статусу (`Pending`, `Paid`, `Shipped`, ...).

Поточний стан репозиторію: закладена доменна модель і каркас застосунку, фронтенд поки стартовий шаблон Angular.

## Структура проєкту

- `client/tech-store-client` - Angular-клієнт.
- `server/TechStore/TechStore.Core` - ASP.NET Core Web API.
- `server/TechStore/TechStore.Infrastructure` - інфраструктурний шар (EF Core `AppDbContext`).

## Запуск локально

### 1. Передумови

- .NET SDK `10.0` (проєкт таргетить `net10.0`).
- Node.js `20+` і npm.
- Доступний SQL Server (наприклад, LocalDB або SQL Server Express).

### 2. Налаштувати backend-конфіг

Відкрий `server/TechStore/TechStore.Core/appsettings.Development.json` і додай:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TechStoreDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "https://localhost:4200"
    ]
  }
}
```

`DefaultConnection` є обов'язковим: без нього API не стартує.

### 3. Запустити backend

```bash
cd server/TechStore/TechStore.Core
dotnet restore
dotnet run --launch-profile https
```

Після старту API доступний за адресами:
- `https://localhost:7290`
- `http://localhost:5059`

Swagger UI: `https://localhost:7290/swagger`.

### 4. Запустити frontend

В іншому терміналі:

```bash
cd client/tech-store-client
npm install
npm start
```

Frontend буде доступний на `http://localhost:4200`.

> Якщо в PowerShell блокується `npm` через Execution Policy, використовуй `npm.cmd install` і `npm.cmd start`.

## Analytics (PostHog)

Клієнт інтегрований із PostHog для збору продуктових подій, побудови воронки конверсії, запису сесій та A/B-тестів через Feature Flags.

### Які події збираються

| Подія | Де викликається | Властивості |
| --- | --- | --- |
| `$pageview` | `AppComponent` на `Router.NavigationEnd` | `path` |
| `product_viewed` | `ProductPage.ngOnInit` після завантаження товару | `product_id`, `product_name`, `category`, `price`, `in_stock` |
| `added_to_cart` | `ProductPage.addToCart` | `product_id`, `product_name`, `price`, `quantity`, `cart_total_items` |
| `removed_from_cart` | `CartSidebarComponent.removeItem` | `product_id`, `product_name` |
| `checkout_started` | `CartSidebarComponent.proceedToCheckout` | `cart_total_value`, `items_count`, `cta_variant` |
| `purchase_completed` | `CheckoutPage.submit` | `order_id`, `total_value`, `items_count`, `payment_method` |

Користувача ідентифікуємо через `analytics.identify(userId, { email, name, role })` після успішного логіну, і скидаємо через `analytics.reset()` на логауті.

### A/B-тест `new-checkout-cta`

У сайдбарі кошика рендериться один із двох варіантів CTA-кнопки залежно від PostHog Feature Flag `new-checkout-cta`:

- `cta-button-old` — стандартна нейтральна кнопка;
- `cta-button-new` — яскравий рожево-фіолетовий градієнт зі збільшеним padding та `position: sticky` на мобільних.

Обидва варіанти емітять подію `checkout_started` з властивістю `cta_variant: 'old' | 'new'`, що дозволяє в PostHog порівняти конверсію між варіантами.

### Налаштування ключа

PostHog ключ та хост приходять із Angular environment files:

- `client/tech-store-client/src/environments/environment.ts` — для `development`;
- `client/tech-store-client/src/environments/environment.production.ts` — для `production` (підставляється в `angular.json` `fileReplacements`).

Реальні `phc_*` (project) ключі вже зашиті в обидва environment-файли — вони безпечні для коміту, бо це write-only ключі для browser SDK. Хост — `https://us.i.posthog.com` (US-регіон).

### SSR-безпека

`AnalyticsService` усі публічні методи (`init`, `capture`, `identify`, `setPersonProperties`, `isFeatureEnabled`, `onFeatureFlagsLoaded`, `reset`) обгортає `isPlatformBrowser(this.platformId)` — на сервері виклики стають no-op, тому prerender не торкається `window`/`document`.

### Дашборд

PostHog проєкт: [us.posthog.com/project/414958](https://us.posthog.com/project/414958)
Funnel dashboard: [us.posthog.com/project/414958/dashboard/1559320](https://us.posthog.com/project/414958/dashboard/1559320)

## Sentry (Lab 6)

### Setup

1. Create project on https://sentry.io (platform: Angular).
2. Copy DSN to `SENTRY_DSN` env var (locally to `.env`, in Vercel to project Environment Variables, in GitHub to repository Secrets).
3. Generate auth token for source maps: Settings → Account → Auth Tokens → Create Token with `project:releases` scope. Add as `SENTRY_AUTH_TOKEN` to GitHub Secrets.
4. Replace `YOUR_ORG` with your Sentry org slug in two places: `client/tech-store-client/package.json` (`sentry:sourcemaps` script) and `.github/workflows/ci-cd.yml` (Upload source maps step).

### Tracked events
- All unhandled exceptions in Angular components (via `Sentry.createErrorHandler` registered as Angular `ErrorHandler` in `app.config.ts`)
- HTTP errors and routing performance via `browserTracingIntegration` (`TraceService`)
- User session replays on errors (`replayIntegration`, inputs masked)

### Manual breadcrumbs
- Cart operations (`added_to_cart` in `pages/product`)
- Checkout flow (`Checkout submit initiated` in `pages/checkout`)
- Test widget actions (`ErrorTestComponent`)

### User context
`ErrorTrackingService.setUser({ id, email, username })` is called after a successful login (next to PostHog `identify`), and `clearUser()` on logout (next to PostHog `reset`).

### SSR safety & DSN gating
`ErrorTrackingService` guards every public method with `isPlatformBrowser`, so calls are no-ops during prerender. When `environment.sentryDsn` is empty the SDK is not initialized (a warning is logged), so local dev without a DSN keeps working.

### Source maps
Production builds emit hidden source maps (`angular.json` → `sourceMap: { scripts: true, hidden: true }`). On pushes to `main`, CI runs `sentry-cli sourcemaps inject` + `upload` so Sentry shows original (non-minified) stack traces.

### Verification
A temporary `ErrorTestComponent` renders two floating buttons (bottom-right):
- **🔥 Break the world** — throws an unhandled `Error` routed to Sentry via the global `ErrorHandler`.
- **⚠️ Send warning to Sentry** — sends a captured message at `warning` level.

Remove `<app-error-test />` from `app.html` (and the import in `app.ts`) after verification screenshots are taken.

### Alert Rule (configure via Sentry UI)
- Navigate to Alerts → Create Alert Rule
- Condition: "When count of events is more than 5 in 1 minute"
- Action: Send email notification
- Save as "High error rate"

### Dashboard
Production dashboard: [add link after deployment]
