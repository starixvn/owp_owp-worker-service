FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS prepare-restore-files

ENV PATH="${PATH}:/root/.dotnet/tools"


# Install the tools
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.1

WORKDIR /src
COPY . .
RUN dotnet subset restore ./OWP.WorkerService/OWP.WorkerService.csproj --root-directory /src --output /src/restore_subset/

FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder

WORKDIR /src
COPY --from=prepare-restore-files /src/restore_subset .
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore ./OWP.WorkerService/OWP.WorkerService.csproj

COPY ./src/. ./src/
COPY ./OWP.WorkerService ./OWP.WorkerService

WORKDIR /src/OWP.WorkerService
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -c release -o /app --no-restore
	
FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .


ENV TZ="America/La_Paz"
ENV COMPlus_EnableDiagnostics=0

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

ENTRYPOINT ["dotnet", "OWP.WorkerService.dll"]
