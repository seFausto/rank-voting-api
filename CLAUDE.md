# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Build
dotnet build RankVotingApi/RankVotingApi.sln

# Run (default port 8080, override with PORT env var)
dotnet run --project RankVotingApi/RankVotingApi/RankVotingApi.csproj

# Swagger UI available at /swagger in development
```

No test project exists yet.

## Architecture

Three-layer structure: **Controller → Business → Repository**, each layer backed by an interface.

- `Votes/RankingsController.cs` — HTTP endpoints, delegates all logic to `IVoteBusiness`
- `Votes/VoteBusiness.cs` — business logic (e.g. shuffles candidates before returning them)
- `Repository/VoteRepository.cs` — Dapper queries against SQLite (`RankChoiceVoting.db`)
- `Common/Extensions.cs` — Fisher-Yates shuffle extension on `List<T>`

Migrations run automatically on startup via FluentMigrator. Migration files live in `Repository/_Migrations/` and follow the naming convention `Migration_YYYYMMDDHHMMSS_Description.cs`.

## Database Schema

| Table | Key Columns | Notes |
|-------|-------------|-------|
| `Ranking` | `VoteId`, `Title`, `Description` | One row per ballot/election |
| `Candidates` | `VoteId`, `Candidate`, `Rank` | `Rank` accumulates Borda scores as votes are cast |
| `UserVotes` | `VoteId`, `UserId`, `Rank`, `Candidate` | Per-user ballot; `Rank` = 0-based position chosen |

## Voting Algorithm

Borda count: when a user submits a ballot, each candidate's `Rank` in the `Candidates` table is incremented by the position the user placed them (0 = first choice). Lower cumulative rank = more preferred. Results are returned ordered `ASC` by `Rank`.

## Coding Conventions

- Multi-parameter method signatures: put each parameter on its own line, indented (one parameter per line)
- Use primary constructor syntax for controllers (see `RankingsController`)
- Use expression-bodied members for simple async pass-throughs (see `VoteBusiness`)
- Wrap repository calls that mutate state in a SQLite transaction
- SQL strings are `const string` locals named descriptively (`insertUserVote`, `updateScore`, etc.)
- `VoteId` is an 8-character GUID prefix generated in `VoteBusiness.SubmitNewRanking`
- Interfaces live alongside their implementations in the same folder
