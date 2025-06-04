# ---- Build & Test Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all project files
COPY *.sln ./
COPY StockPriceMonitoringAndAlerts/*.csproj StockPriceMonitoringAndAlerts/
COPY StockPriceMonitoring.Tests/*.csproj StockPriceMonitoring.Tests/
COPY StockPriceMonitoring.IntegrationTests/*.csproj StockPriceMonitoring.IntegrationTests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Optional test toggle
ARG RUN_TESTS=true
RUN if [ "$RUN_TESTS" = "true" ]; then \
    dotnet test StockPriceMonitoring.Tests/ --no-restore --verbosity normal && \
    dotnet test StockPriceMonitoring.IntegrationTests/ --no-restore --verbosity normal ; \
    fi

# Publish the main app
WORKDIR /src/StockPriceMonitoringAndAlerts
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "StockPriceMonitoringAndAlerts.dll"]
