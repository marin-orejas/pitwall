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

## Database

The backend uses PostgreSQL 18 through EF Core with the Npgsql provider. The local setup on Fedora, done once:

```
sudo dnf install postgresql-server pgadmin4 pgadmin4-qt
sudo postgresql-setup --initdb
sudo sed -i -E 's/^(host\s.*\s)ident$/\1scram-sha-256/' /var/lib/pgsql/data/pg_hba.conf
sudo systemctl enable --now postgresql
sudo -u postgres createuser --pwprompt pitwall
sudo -u postgres createdb --owner=pitwall pitwall
```

The `sed` line switches local TCP connections from `ident` to password authentication. The `pitwall` role is not a superuser; it owns the `pitwall` database. pgAdmin 4 connects to `localhost:5432` with the same role.

The connection string is kept in user-secrets, never in the repository. From `backend/`:

```
read -rsp 'Password: ' PITWALL_DB_PASSWORD; echo
dotnet user-secrets set --project PitWall.Api "ConnectionStrings:PitWall" "Host=localhost;Port=5432;Database=pitwall;Username=pitwall;Password=$PITWALL_DB_PASSWORD"
unset PITWALL_DB_PASSWORD
```

Without `ConnectionStrings:PitWall`, the first database access fails with an error that names the missing key.

### Migrations

`dotnet-ef` is a local tool (`backend/.config/dotnet-tools.json`). From `backend/`:

```
dotnet tool restore
dotnet ef database update --project PitWall.Data --startup-project PitWall.Api
dotnet ef migrations list --project PitWall.Data --startup-project PitWall.Api
```

After a change to the model, add a migration with a PascalCase name, then apply it with `database update`:

```
dotnet ef migrations add AddUsers --project PitWall.Data --startup-project PitWall.Api
```
