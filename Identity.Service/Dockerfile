FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Identity.Service/Identity.Service.sln", "Identity.Service/"]
COPY ["Identity.Service/Identity.Service.API.csproj", "Identity.Service/"]
COPY ["Identity.Server.Data/Identity.Service.Data.csproj", "Identity.Server.Data/"]

RUN dotnet restore "Identity.Service/Identity.Service.sln"

COPY . .

WORKDIR "/src/Identity.Service"

RUN dotnet build "Identity.Service.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Service.sln" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Identity.Service.API.dll"]
