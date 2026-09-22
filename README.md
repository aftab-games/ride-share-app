# RideShare API

A lightweight ride-sharing backend focused on real-time systems: SignalR, Redis, and geospatial matching. Built as a deliberate step up from a prior CRUD-only REST API project — scoped narrowly to avoid re-covering the same ground.

**Core loop:** rider requests a ride → nearest available driver is matched and notified in real time → driver accepts, starts, streams live location → ride completes. No polling anywhere in that flow.

## Highlights

- Real-time, bidirectional SignalR communication (not just request/response)
- Redis as both a SignalR backplane and a swappable storage backend behind an interface
- Haversine-distance nearest-driver matching
- Ride lifecycle enforced as an explicit, unit-tested state machine
- CI: build, test, dependency vulnerability scan, Docker image validation on every push

## Architecture

```
RideShare.Api --> RideShare.Infrastructure --> RideShare.Domain
RideShare.Tests --> Api, Domain
```

| Project | Responsibility |
|---|---|
| **Domain** | Entities, enums, `Ride.CanTransitionTo` state rules, `GeoUtils` (Haversine). No external dependencies. |
| **Infrastructure** | `RideShareDbContext` (EF Core + PostgreSQL), `IDriverLocationStore` with in-memory and Redis implementations. |
| **Api** | REST controllers, `RideHub` (SignalR), connection identity tracking, matching logic. |
| **Tests** | xUnit — state machine and distance calculation. |

No repository pattern (`DbContext` already is one), no CQRS/Application layer — kept intentionally lighter-weight than full Clean Architecture.

## Stack

.NET 10 - ASP.NET Core - EF Core - PostgreSQL 18 - SignalR - Redis 7 - StackExchange.Redis - xUnit - Docker - GitHub Actions

## Setup

```bash
docker compose up -d
dotnet user-secrets set "ConnectionStrings:RideShareDb" "Host=localhost;Port=5433;Database=rideshare;Username=rideshare;Password=rideshare_dev" --project RideShare.Api
dotnet ef database update --project RideShare.Infrastructure --startup-project RideShare.Api
dotnet run --project RideShare.Api
```
Manual test clients at `/test.html` (driver) and `/test-rider.html` (rider).

## REST API

| Method | Route | Description |
|---|---|---|
| POST | `/api/riders` | Create rider |
| GET | `/api/riders/{id}` | Get rider |
| POST | `/api/drivers` | Create driver |
| GET | `/api/drivers/{id}` | Get driver |
| POST | `/api/rides` | Request ride -- triggers matching + real-time push |
| GET | `/api/rides/{id}` | Get ride |

## SignalR Hub (`/hubs/ride`)

**Invoke:** `RegisterAsDriver` - `RegisterAsRider` - `SetAvailable` - `UpdateLocation` - `AcceptRide` - `StartRide` - `CompleteRide`
**Listen:** `RideRequested` - `RideAccepted` - `RideAcceptFailed` - `RideStarted` - `RideActionFailed` - `RideCompleted` - `DriverLocationUpdated`

## Known limitations

- Single connection per driver -- a second simultaneous device will misbehave on disconnect
- `AcceptRide`'s race guard is a status check, not a DB-level lock
- `UpdateLocation` queries Postgres per ping rather than caching the active-ride lookup
- Two separate Redis connections (location store + SignalR backplane) instead of a shared factory
- No payments, ratings, or multi-stop trips -- out of scope by design

## CI

GitHub Actions on every push to `main`: restore -> build -> test -> vulnerability scan -> Docker build validation.
