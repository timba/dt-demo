
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

Open browser on URL https://localhost:5001/. Ignore SSL certificate error.

## Docker

The service could be run inside Docker container. You can either build the image locally or pull the image from Docker Hub.

Docker commands are run from solution root. If your Docker installation requires elevated permissions, consider using `sudo` or running under Administrator privileges.

### Build image

Image build takes some time and significant disk space (uo to 2 GB) for intermeiate SDK layers. To build the image locally, run this command:

`docker build . -t timbabyuk/dt-demo`

### Pull image from Docker Hub

Instead of building image locally, you canjust pull it from Docker Hub. Demo service image is stored in `timbabyuk/dt-demo` repository. To pull the image, execute:

`docker pull timbabyuk/dt-demo`

### Run image

Once image is built or pulled, execute this command to run container from the image:

`docker run -p 8080:80 timbabyuk/dt-demo`

Now demo application is available on localhost, port 8080, URL http://localhost:8080

If 8080 port occupied, you can specify another in `docker run` command.

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
