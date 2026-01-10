FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# COPY . ./ often break restore, therefore we copy exactly only solutions we need:
COPY RealEstate.slnx ./
COPY RealEstate.Api ./RealEstate.Api

# important to point to correct csproj, otherwise docker MIGHT build something weird
RUN dotnet restore RealEstate.Api/RealEstate.Api.csproj
RUN dotnet publish RealEstate.Api/RealEstate.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

CMD ["dotnet", "RealEstate.Api.dll"]