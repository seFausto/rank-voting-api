# Ranked Choice Voting API

REST API for ranked choice voting. Currently implements the **Borda count** method — candidates accumulate points based on the position each voter places them, and the candidate with the lowest score wins.

## Tech stack

- ASP.NET Core (.NET 8) — SQLite via Dapper — FluentMigrator

## Running locally

```bash
dotnet run --project RankVotingApi/RankVotingApi/RankVotingApi.csproj
```

Defaults to `http://0.0.0.0:8080`. Override with the `PORT` environment variable.

Swagger UI is available at `/swagger` in development.

The SQLite database (`RankChoiceVoting.db`) is created and migrated automatically on startup.

## API

### Rankings

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/rankings/{name}` | Create a new ranking. Body: JSON array of candidate names. Returns the ranking ID. |
| `GET` | `/rankings/{id}` | Get ranking title. |
| `GET` | `/rankings/{id}/candidates` | Get candidates in randomized order. |
| `GET` | `/rankings/{id}/results` | Get candidates ordered by current Borda score (ascending). |
| `GET` | `/rankings/{id}/ballots/count` | Get number of ballots submitted. |

### Ballots

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/rankings/{id}/ballots/{userId}` | Submit a ballot. Body: JSON array of candidate names in preferred order (first = most preferred). |
| `GET` | `/rankings/{id}/ballots/{userId}` | Get a previously submitted ballot. |

### Other

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check. |

## How voting works

When a ballot is submitted, each candidate receives points equal to their position in the voter's ranking (0 = first choice, 1 = second choice, etc.). These points accumulate in the database across all voters. Results are returned sorted ascending by total score — lower is better.
