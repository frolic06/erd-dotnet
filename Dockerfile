FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/*.csproj ./src/
RUN dotnet restore ./src/erd-dotnet.csproj

COPY src/. ./src/
WORKDIR /app/src
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y graphviz
RUN apt-get install fonts-noto-color-emoji

COPY --from=build /app/src/out ./
ENTRYPOINT ["dotnet", "erd-dotnet.dll"]