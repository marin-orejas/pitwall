# PitWall

An Android app for a Formula 1 prediction game with friends. Users gather in private leagues and, before each race weekend, predict pole position, the top three and the fastest lap. After the race, the backend fetches the results and scores the predictions.

A course project at FOI.

## Backend

ASP.NET Core Web API on .NET 10, in the `backend/` folder:

- `PitWall.Api`: endpoints, DI, OpenAPI
- `PitWall.Business`: business rules
- `PitWall.Data`: the database and external data sources
- `PitWall.Tests`: xUnit tests

It needs the .NET 10 SDK (on Fedora: `sudo dnf install dotnet-sdk-10.0`).

Run the commands from the `backend/` folder, because its `global.json` selects the SDK and the test runner:

```
cd backend
dotnet build PitWall.sln
dotnet test --solution PitWall.sln
dotnet format PitWall.sln --verify-no-changes
dotnet run --project PitWall.Api
```

The API listens on `http://localhost:5000`. In the development environment, Swagger UI is at `http://localhost:5000/swagger` and the OpenAPI document at `http://localhost:5000/openapi/v1.json`.
