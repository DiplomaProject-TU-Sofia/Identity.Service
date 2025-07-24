FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Identity.Service/Identity.Service.API.csproj", "Identity.Service/"]
COPY ["Identity.Server.Data/Identity.Service.Data.csproj", "Identity.Server.Data/"]
COPY . .

WORKDIR "/src/Identity.Service"

RUN dotnet build "Identity.Service.API.csproj" -c Release -o /app/build

FROM build AS publish

WORKDIR "/src/Identity.Service"
RUN dotnet publish "Identity.Service.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Identity.Service.API.dll"]
