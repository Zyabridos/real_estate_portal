FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RealEstate.slnx ./
COPY ./Directory.Packages.props ./Directory.Packages.props

COPY src/Api/Api.csproj ./src/Api/
COPY src/Application/Application.csproj ./src/Application/
COPY src/Application/Domain/RealEstate.Domain.csproj ./src/Application/Domain/
COPY src/Application/Infrastructure/RealEstate.Infrastructure.csproj ./src/Application/Infrastructure/

RUN dotnet restore ./src/Api/Api.csproj

COPY . .

RUN dotnet publish ./src/Api/Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl

# Run as non-root (`app`) to follow container security best practices. 
# Default is root, so "no sudo today".
USER app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=8 \
  CMD curl -fsS http://localhost:5000/api/health/liveness || exit 1

CMD ["dotnet", "Api.dll"]
