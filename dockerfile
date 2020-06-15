# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/Injhinuity.Client/*.csproj Injhinuity.Client/
COPY src/Injhinuity.Client.Core/*.csproj Injhinuity.Client.Core/
RUN dotnet restore Injhinuity.Client/Injhinuity.Client.csproj

# copy and build app and libraries
COPY src/Injhinuity.Client/ Injhinuity.Client/
COPY src/Injhinuity.Client.Core/ Injhinuity.Client.Core/
WORKDIR /source/Injhinuity.Client
RUN dotnet build -c release

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:5.0
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Injhinuity.Client.dll"]