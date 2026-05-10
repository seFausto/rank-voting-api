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

**Stack:** ASP.NET Core 8 Web API · SQLite + Dapper · FluentMigrator · Confluent Kafka

The project uses a classic layered architecture with a single feature domain (`Votes`):

```
VoteController  →  IVoteBusiness / VoteBusiness  →  IVoteRepository / VoteRepository  →  SQLite
                                                  ↑
                                    KafkaConsumerService (background hosted service)
```

- **Controller** (`Votes/VoteController.cs`): REST endpoints. Uses old-style ASP.NET Core startup (`Startup.cs` + `Program.cs` with `CreateHostBuilder`).
- **Business layer** (`Votes/VoteBusiness.cs`): Implements Borda count voting logic. Candidates are shuffled on retrieval to prevent UI order bias. Scores are cumulative — lower total = better rank.
- **Repository** (`Repository/VoteRepository.cs`): All SQL via Dapper. SQLite database file is `RankChoiceVoting.db` (auto-created).
- **Migrations** (`Repository/_Migrations/`): FluentMigrator migrations run automatically at startup in `ConfigureServices`. Timestamp-prefixed class names.
- **Kafka consumer** (`KafkaConsumer/KafkaConsumerService.cs`): Hosted background service consuming the `new-ranking` topic. Kafka connection configured via INI-style `kafkaClient.properties` file parsed in `Common/Common.cs`.

## Key Configuration

- `appsettings.json`: Kafka topic name (`KafkaOptions:Topic`).
- `kafkaClient.properties`: Confluent Cloud connection settings (bootstrap servers, SASL credentials). This file is INI-format, parsed via `Microsoft.Extensions.Configuration.Ini`.
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

The app targets Google Cloud (see `app.yaml`, `.gcloudignore`). The Dockerfile is a multi-stage Linux build using `mcr.microsoft.com/dotnet/sdk:8.0` → `mcr.microsoft.com/dotnet/aspnet:8.0`.
