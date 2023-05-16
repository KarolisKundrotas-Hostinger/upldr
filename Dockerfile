FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore --use-current-runtime /p:PublishReadyToRun=true

# copy everything else and build app
COPY . .
RUN dotnet publish --use-current-runtime --no-restore -o /app /p:PublishTrimmed=true /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./upldr"]