FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/SmartInventory.Domain/SmartInventory.Domain.csproj src/SmartInventory.Domain/
COPY src/SmartInventory.Application/SmartInventory.Application.csproj src/SmartInventory.Application/
COPY src/SmartInventory.Infrastructure/SmartInventory.Infrastructure.csproj src/SmartInventory.Infrastructure/
COPY src/SmartInventory.Api/SmartInventory.Api.csproj src/SmartInventory.Api/
RUN dotnet restore src/SmartInventory.Api/SmartInventory.Api.csproj

COPY src/ src/
RUN dotnet publish src/SmartInventory.Api/SmartInventory.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
ENV PORT=8080 \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "SmartInventory.Api.dll"]
