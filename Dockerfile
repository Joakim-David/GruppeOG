ARG DOTNET_VERSION=9.0
ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY . .

RUN dotnet restore "./Chirp.Web/Chirp.Web.csproj"
RUN dotnet build "./Chirp.Web/Chirp.Web.csproj" 
RUN dotnet run "./Chirp.Web/Chirp.Web.csproj"

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /src
