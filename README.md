# Blindside Poker

Play-money Texas Hold'em tournaments for 2-10 players, joined by invite link.
Play chips only: nothing can be bought or cashed out.

**Status:** early development. The game engine is being built first.

## Stack

- Backend: ASP.NET Core (.NET 10), SignalR, EF Core + PostgreSQL
- Frontend: React + TypeScript (Vite), Tailwind
- Tests: xUnit

## Repository layout

```
backend/
  src/Blindside.Engine/    pure game logic (no ASP.NET, SignalR or EF Core)
  tests/Blindside.Tests/   xUnit tests
frontend/                  React app (not started yet)
```

## Getting started

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download) (version pinned in `backend/global.json`).

```bash
cd backend
dotnet build
dotnet test
```
