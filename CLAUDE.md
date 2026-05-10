# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from `RankVotingApi/RankVotingApi/` unless otherwise noted.

```bash
# Build
dotnet build

# Run (from RankVotingApi/RankVotingApi/)
dotnet run

# Build Docker image (from RankVotingApi/)
docker build -t rank-voting-api .
```

There are no automated tests in this project.

## Architecture

**Stack:** ASP.NET Core 8 Web API · SQLite + Dapper · FluentMigrator

The project uses a classic layered architecture with a single feature domain (`Votes`):

```
VoteController  →  IVoteBusiness / VoteBusiness  →  IVoteRepository / VoteRepository  →  SQLite
```

- **Controller** (`Votes/VoteController.cs`): REST endpoints. Uses old-style ASP.NET Core startup (`Startup.cs` + `Program.cs` with `CreateHostBuilder`).
- **Business layer** (`Votes/VoteBusiness.cs`): Implements Borda count voting logic. Candidates are shuffled on retrieval to prevent UI order bias (`Common/Extensions.cs`). Scores are cumulative — lower total = better rank.
- **Repository** (`Repository/VoteRepository.cs`): All SQL via Dapper. SQLite database file is `RankChoiceVoting.db` (auto-created).
- **Migrations** (`Repository/_Migrations/`): FluentMigrator migrations run automatically at startup in `ConfigureServices`. Timestamp-prefixed class names.

## Key Configuration

- Port defaults to `8080`; override with `PORT` environment variable (used in Docker).
- CORS is open (`AllowEverything` policy) — all origins, methods, and headers allowed.
- Swagger UI is only enabled in the Development environment.

## API Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/vote` | Health check |
| POST | `/vote/new/{rankingName}` | Create new voting event |
| GET | `/vote/{voteId}/info` | Get voting event metadata |
| POST | `/vote/{voteId}/candidates/{didVote}` | List candidates (shuffled); checks if user already voted |
| POST | `/vote/{voteId}/submit/{userId}` | Submit ranked ballot |
| GET | `/vote/{voteId}/result` | Get Borda count results |

## Database Schema

Four tables managed by FluentMigrator migrations:
- `Ranking` — voting events (VoteId, Title, Description)
- `Candidates` — candidates per event with accumulated Borda score
- `UserVotes` — individual ballot submissions (one row per candidate per user)
- `Log` — legacy logging table (unused)

## Deployment

The app deploys to **Azure App Service** (Linux container) using the existing Dockerfile (multi-stage build: `mcr.microsoft.com/dotnet/sdk:8.0` → `mcr.microsoft.com/dotnet/aspnet:8.0`).

**One-time setup:**
```bash
# Push image to Azure Container Registry
az acr create --resource-group <rg> --name <acr-name> --sku Basic
az acr login --name <acr-name>
docker build -t <acr-name>.azurecr.io/rank-voting-api:latest ./RankVotingApi
docker push <acr-name>.azurecr.io/rank-voting-api:latest

# Create App Service
az appservice plan create --name <plan> --resource-group <rg> --is-linux --sku B1
az webapp create --resource-group <rg> --plan <plan> --name <app-name> \
  --deployment-container-image-name <acr-name>.azurecr.io/rank-voting-api:latest
```

**Required App Service application settings:**
- `WEBSITES_PORT=8080` — routes traffic to the container's listening port
