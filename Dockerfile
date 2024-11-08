FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ["./src/Fanex.NAP.SPU.Main/Fanex.NAP.SPU.Main.csproj", "Fanex.NAP.SPU.Main/"]

WORKDIR "/src/Fanex.NAP.SPU.Main"

RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./src/Fanex.NAP.SPU.Main .

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.NAP.SPU.Main.dll"]