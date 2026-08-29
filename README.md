# jerseyMarket

A full-stack product catalog app: an ASP.NET Core Web API backend with JWT authentication,
and an Angular client.

## Project structure

```
jerseyMarket/                 ASP.NET Core Web API (.NET 10)
  Controllers/                 AuthController, ProductsController
  Services/                    AuthService, ProductService (business logic)
  Models/                      EF Core entities (User, Product, Category)
  Dtos/                        request/response DTOs
  Enums/                       result enums used for service -> controller signaling
  Migrations/                  EF Core migrations
  Middleware/                  global exception handling

client/                       Angular client (v21)
  src/app/auth/                 login, register, logout, auth service, guard, interceptors
  src/app/products/             product list, product create/edit form

jerseyMarket.postman_collection.json   Postman collection for the API
```

## Backend

**Stack:** ASP.NET Core, Entity Framework Core, SQL Server, JWT Bearer auth.

### Setup

1. Configure a SQL Server connection string and a JWT signing key via user secrets
   (or `appsettings.Development.json`):

   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
   dotnet user-secrets set "AppSettings:Token" "<a random string, at least 64 bytes>"
   ```

2. Apply migrations:

   ```
   cd jerseyMarket
   dotnet ef database update
   ```

3. Run the API:

   ```
   dotnet run
   ```

### Auth

- Register/login return a short-lived access token (30 min) and a refresh token.
- The access token is a JWT (`HmacSha512`), validated on `Issuer`/`Audience`/`Lifetime`.
- `POST /api/Auth/regenerate-tokens` exchanges a valid refresh token for a new token pair.
- `POST /api/Auth/logout` (requires a valid access token) revokes the stored refresh token.
- Login is rate-limited (5 attempts/minute per IP).

### API endpoints

| Method | Route                          | Auth | Description                        |
| ------ | ------------------------------- | ---- | ----------------------------------- |
| POST   | `/api/Auth/register`            | –    | Create a user                       |
| POST   | `/api/Auth/login`                | –    | Log in, get access + refresh token  |
| POST   | `/api/Auth/regenerate-tokens`   | –    | Refresh an expired access token     |
| POST   | `/api/Auth/logout`              | JWT  | Revoke the refresh token            |
| GET    | `/api/Products`                 | –    | List products (filter by name/category) |
| GET    | `/api/Products/{id}`            | –    | Get a single product                |
| POST   | `/api/Products`                 | JWT  | Create a product                    |
| PUT    | `/api/Products/{id}`            | JWT  | Update a product                    |

A ready-to-import Postman collection covering all of the above is at
`jerseyMarket.postman_collection.json`.

## Frontend

**Stack:** Angular 21, standalone components, signals.

```
cd client
npm install
npm start        # ng serve, dev proxy forwards /api to the backend
```

- Access token is kept in memory only; refresh token + user id persist in `localStorage`
  to silently restore a session on reload.
- An HTTP interceptor attaches the bearer token to outgoing requests; a second interceptor
  retries once on a `401` after refreshing the access token.
- Routes requiring auth (`/products/new`, `/products/:id/edit`, `/logout`) are protected by
  an `authGuard`.

## Running both together

Start the backend (`dotnet run` in `jerseyMarket/`) and the frontend (`npm start` in
`client/`); the Angular dev server proxies API calls through to the backend.
