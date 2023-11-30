#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ha.kostal.service/ha.kostal.service.csproj", "ha.kostal.service/"]
RUN dotnet restore "ha.kostal.service/ha.kostal.service.csproj"
COPY . .
WORKDIR "/src/ha.kostal.service"
RUN dotnet build "ha.kostal.service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ha.kostal.service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ha.kostal.service.dll"]