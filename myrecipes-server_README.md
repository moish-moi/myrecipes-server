# MyRecipes – Server (ASP.NET Core Web API)

Backend API for **MyRecipes**, a personal recipe manager with **JWT authentication** and **SQLite** persistence.

## Tech Stack
- ASP.NET Core Web API (C#)
- Entity Framework Core
- SQLite
- JWT Bearer Auth

## Features
- **Auth**: Register + Login (JWT)
- **Recipes** (authorized):
  - Create / Read / Update / Delete recipes
  - Get a single recipe by id
  - **Search recipes by tag** (`GET /api/recipes/search/{tag}`)
- **User isolation**: recipes are scoped to the logged-in user (via JWT claims)
- Recipe fields include: `title`, `ingredients`, `instructions`, `tags`, `preparationTime`, optional `imageUrl`, and timestamps (`createdAt`, `updatedAt`).

## Run Locally
### Prerequisites
- .NET SDK (project targets .NET 8 in most setups)
- (Optional) `dotnet-ef` for migrations

### Start the API
```bash
cd MyRecipes.Api
dotnet restore
dotnet run
```

API base:
- `http://localhost:5058/api`

## Database (SQLite)
The project uses SQLite. If you need to apply migrations:
```bash
dotnet ef database update
```

## Auth (JWT)
Client sends:
`Authorization: Bearer <token>`

JWT settings are in `appsettings.json` under `Jwt`.
