#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["worker-netcore-crawl/worker-netcore-crawl.csproj", "worker-netcore-crawl/"]
RUN dotnet restore "worker-netcore-crawl/worker-netcore-crawl.csproj"
COPY . .
WORKDIR "/src/worker-netcore-crawl"
RUN dotnet build "worker-netcore-crawl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "worker-netcore-crawl.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "worker-netcore-crawl.dll"]
