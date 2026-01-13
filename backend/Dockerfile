FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# COPY . ./ often break restore, therefore we copy exactly only solutions we need:
COPY RealEstate.slnx ./
COPY src/RealEstate.Api ./src//RealEstate.Api

# Copy csproj files (more control over what is going on in container)
COPY src/RealEstate.Api/RealEstate.Api.csproj ./src/RealEstate.Api/
COPY src/RealEstate.Application/RealEstate.Application.csproj ./src/RealEstate.Application/
COPY src/RealEstate.Domain/RealEstate.Domain.csproj ./src/RealEstate.Domain/
COPY src/RealEstate.Infrastructure/RealEstate.Infrastructure.csproj ./src/RealEstate.Infrastructure/

RUN dotnet restore ./src/RealEstate.Api/RealEstate.Api.csproj

# Better safe than sorry, so...
COPY . .

# Publish API
RUN dotnet publish ./src/RealEstate.Api/RealEstate.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

CMD ["dotnet", "RealEstate.Api.dll"]
