FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:6.0 AS builder
# Install the tools
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-dump
RUN dotnet tool install --tool-path /tools dotnet-counters

WORKDIR /src

COPY ./src/OWP.Core/. ./src/OWP.Core
COPY ./src/OWP.InforsiteIdSync/. ./src/OWP.InforsiteIdSync
COPY ./src/OWP.Infosite/. ./src/OWP.Infosite
COPY ./src/OWP.LinkLeagueAndTeam/. ./src/OWP.LinkLeagueAndTeam
COPY ./src/OWP.MatchReadyNotification/. ./src/OWP.MatchReadyNotification
COPY ./src/OWP.OtherSport/. ./src/OWP.OtherSport
COPY ./src/OWP.SumReportData/. ./src/OWP.SumReportData
COPY ./src/OWP.SyncLeagueSettings/. ./src/OWP.SyncLeagueSettings
COPY ./OWP.WorkerService/OWP.WorkerService.csproj ./OWP.WorkerService/OWP.WorkerService.csproj

WORKDIR /src/OWP.WorkerService
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v diag

COPY ./OWP.WorkerService .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app 

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:6.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

WORKDIR /app
COPY --from=builder /app .
COPY --from=builder /tools /tools

ENV TZ="America/La_Paz" 

ENTRYPOINT ["dotnet", "OWP.WorkerService.dll"]