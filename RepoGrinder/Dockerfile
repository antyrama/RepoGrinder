FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RepoGrinder/RepoGrinder.csproj", "RepoGrinder/"]
RUN dotnet restore "RepoGrinder/RepoGrinder.csproj"
COPY . .
WORKDIR "/src/RepoGrinder"
RUN dotnet build "RepoGrinder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RepoGrinder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RepoGrinder.dll"]