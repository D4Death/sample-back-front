#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["sample-api/sample-api.csproj", "sample-api/"]
COPY ["sample-ultilities/sample-ultilities.csproj", "sample-ultilities/"]
COPY ["sample-model/sample-model.csproj", "sample-model/"]
RUN dotnet restore "sample-api/sample-api.csproj"
COPY . .
WORKDIR "/src/sample-api"
RUN dotnet build "sample-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "sample-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 8090
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sample-api.dll"]
