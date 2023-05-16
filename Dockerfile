FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore --use-current-runtime /p:PublishReadyToRun=true

# copy everything else and build app
COPY . .
RUN dotnet publish --use-current-runtime --no-restore -o /app /p:PublishTrimmed=true /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine
LABEL org.opencontainers.image.source=https://github.com/KarolisKundrotas-Hostinger/upldr
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_UploadPath=/app/uploads
ENV ASPNETCORE_Key=changeme

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./upldr"]