FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base

WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build

RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs

RUN node -v

RUN npm -v

WORKDIR /src
COPY ["src/server/server.csproj", "src/server/server.csproj"]
RUN dotnet restore "src/server/server.csproj"
COPY . .
WORKDIR /src/src/server
RUN dotnet build "server.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "server.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "server.dll"]
