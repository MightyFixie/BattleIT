# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0/publish/ App/
COPY Input/data.csv App/data.csv
WORKDIR /App
ENTRYPOINT ["dotnet", "SC.DevChallenge.Api.dll"]
