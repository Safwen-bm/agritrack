FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY AgriTrack.Api/AgriTrack.Api.csproj AgriTrack.Api/
RUN dotnet restore AgriTrack.Api/AgriTrack.Api.csproj
COPY AgriTrack.Api/ AgriTrack.Api/
RUN dotnet publish AgriTrack.Api/AgriTrack.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# Render expects the app on port 10000 by default
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "AgriTrack.Api.dll"]