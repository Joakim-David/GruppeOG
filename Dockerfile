# Use .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy all project files
COPY src/Chirp.Core/*.csproj ./src/Chirp.Core/
COPY src/Chirp.Infrastructure/Chirp.Services/*.csproj ./src/Chirp.Infrastructure/Chirp.Services/
COPY src/Chirp.Infrastructure/Chirp.Repositories/*.csproj ./src/Chirp.Infrastructure/Chirp.Repositories/
COPY src/Chirp.Web/*.csproj ./src/Chirp.Web/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/src/Chirp.Web
RUN dotnet publish -c Release -o /app/publish

# Use runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 7273
EXPOSE 7273

# Set environment to listen on all interfaces
ENV ASPNETCORE_URLS=http://+:7273

ENTRYPOINT ["dotnet", "Chirp.Web.dll"]
