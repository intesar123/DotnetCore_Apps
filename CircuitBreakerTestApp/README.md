# CircuitBreakerTestApp

An ASP.NET Core Web API sample demonstrating resilience for outbound HTTP requests and a database-style operation. It targets .NET 10.

## Prerequisites

- .NET 10 SDK

## Install the resilience package

The project uses `Microsoft.Extensions.Http.Resilience` (version `10.11.0`). To add it to the project with the .NET CLI:

```powershell
dotnet add package Microsoft.Extensions.Http.Resilience --version 10.11.0
```

The package is already referenced in `CircuitBreakerTestApp.csproj`; this command is for adding the dependency when setting up the project from scratch.

## Run the API

From the `CircuitBreakerTestApp` directory:

```powershell
dotnet restore
dotnet run
```

The HTTP launch profile listens at `http://localhost:5041`. To use HTTPS, run:

```powershell
dotnet run --launch-profile https
```

## Endpoints

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/order/{id}` | Retrieves an order from the configured upstream Orders API. |
| `GET` | `/api/inventory/{id}` | Retrieves an inventory item name through the database resilience pipeline. |
| `GET` | `/weatherforecast` | Returns sample weather forecasts. |

In the Development environment, open the Scalar UI at `/scalar/v1`. The OpenAPI document is available at `/openapi/v1.json`. The `http` and `https` launch profiles open the Scalar UI automatically.

## Resilience behavior

- The Orders HTTP client has a total timeout, per-attempt timeout, retry strategy, and circuit breaker.
- The database-named pipeline uses a circuit breaker around the sample inventory repository operation. The repository currently simulates work; replace it with a real database call as needed.
- Resilience settings are configurable under the `Resilience` section in `appsettings.json`.
- The upstream Orders API base URL is configured under `OrdersApi:BaseUrl`. The default (`https://orders.internal`) is a placeholder and must be replaced with a reachable service to retrieve orders.
