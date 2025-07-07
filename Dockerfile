ARG CI_REGISTRY_IMAGE
FROM $CI_REGISTRY_IMAGE:builder AS deps
RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages
COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ./src/OWP/. ./src/OWP
COPY ./src/OWP.InforsiteIdSync/. ./src/OWP.InforsiteIdSync
COPY ./src/OWP.Infosite/. ./src/OWP.Infosite
COPY ./src/OWP.LinkLeagueAndTeam/. ./src/OWP.LinkLeagueAndTeam
COPY ./src/OWP.MatchReadyNotification/. ./src/OWP.MatchReadyNotification
COPY ./src/OWP.OtherSport/. ./src/OWP.OtherSport
COPY ./src/OWP.SumReport/. ./src/OWP.SumReport
COPY ./src/OWP.SyncLeagueSettings/. ./src/OWP.SyncLeagueSettings
COPY ./src/OWP.ArchiveData/. ./src/OWP.ArchiveData
COPY ./src/OWP.Provider.Share/. ./src/OWP.Provider.Share
COPY ./src/OWP.Adminsite/. ./src/OWP.Adminsite
COPY ./src/OWP.Chat/. ./src/OWP.Chat
COPY ./OWP.WorkerService/OWP.WorkerService.csproj ./OWP.WorkerService/OWP.WorkerService.csproj

WORKDIR /src/OWP.WorkerService
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n -r linux-x64

COPY ./OWP.WorkerService .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="America/La_Paz" 
ENV COMPlus_EnableDiagnostics=0

# Don’t run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#
ENTRYPOINT ["dotnet", "OWP.WorkerService.dll"]