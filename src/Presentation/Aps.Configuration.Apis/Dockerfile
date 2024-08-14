# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project files
COPY src/Presentation/Aps.Configuration.Apis/Aps.Configuration.Apis.csproj src/Presentation/Aps.Configuration.Apis/
COPY tests/Presentation.Tests/Aps.Configuration.Apis.Tests/Aps.Configuration.Apis.Tests.csproj tests/Presentation.Tests/Aps.Configuration.Apis.Tests/

# Restore dependencies for the main project and test projects
RUN dotnet restore src/Presentation/Aps.Configuration.Apis/Aps.Configuration.Apis.csproj

# Copy the rest of the source code
COPY src/ ./src
COPY tests/ ./tests

# Build the application
RUN dotnet build src/Presentation/Aps.Configuration.Apis/Aps.Configuration.Apis.csproj -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish src/Presentation/Aps.Configuration.Apis/Aps.Configuration.Apis.csproj -c Release -o /app/publish /p:UseAppHost=false

# Use the .NET 7 ASP.NET runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aps.Configuration.Apis.dll"]