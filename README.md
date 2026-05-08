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

У плейсхолдері стоїть `phc_PLACEHOLDER_REPLACE_WITH_REAL_KEY`. Для деплою на Vercel реальний ключ слід задати у **Project Settings → Environment Variables** і пересобрати клієнт. Хост за замовчуванням — `https://eu.i.posthog.com`.

### SSR-безпека

`AnalyticsService` усі публічні методи (`init`, `capture`, `identify`, `setPersonProperties`, `isFeatureEnabled`, `onFeatureFlagsLoaded`, `reset`) обгортає `isPlatformBrowser(this.platformId)` — на сервері виклики стають no-op, тому prerender не торкається `window`/`document`.

### Дашборд

PostHog dashboard: _TBD_ (буде заповнено після створення проєкту в PostHog UI).
