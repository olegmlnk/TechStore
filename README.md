# TechStore

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
