# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HolaMundoScheduler.csproj", "./"]
RUN dotnet restore "./HolaMundoScheduler.csproj"

COPY . .
RUN dotnet publish "./HolaMundoScheduler.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system --gid 1001 appgroup \
 && adduser --system --uid 1001 --ingroup appgroup appuser

COPY --from=build /app/publish .

USER appuser

ENTRYPOINT ["dotnet", "HolaMundoScheduler.dll"]
