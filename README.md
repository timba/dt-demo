
## Prerequisites

- .NET Core SDK 2.2
- node 10+
- npm 6+
- Chrome for tests

## Restore dependencies

`dotnet restore`

## Build

`dotnet build`

## Run 

`dotnet run -p src/server/server.csproj`

Open browser

## Tests

### Server test

`dotnet test`

### UI Tests

Ensure that Chrome is installed and available in path. On Linux run:

`export CHROME_BIN=chromium-browser`

Run:

`ng test` from `src/ui` folder

### End to End tests

`ng e2e` from `src/ui` folder
