# -----------------------------
# Runtime image
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -----------------------------
# Build image
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first (for Docker cache)
COPY ["ProjectS.UtilitiesService/UtilitiesService.API/UtilitiesService.API.csproj", "ProjectS.UtilitiesService/UtilitiesService.API/"]
COPY ["ProjectS.UtilitiesService/UtilitiesService.Application/UtilitiesService.Application.csproj", "ProjectS.UtilitiesService/UtilitiesService.Application/"]
COPY ["ProjectS.UtilitiesService/UtilitiesService.Domain/UtilitiesService.Domain.csproj", "ProjectS.UtilitiesService/UtilitiesService.Domain/"]
COPY ["ProjectS.UtilitiesService/UtilitiesService.Infrastructure/UtilitiesService.Infrastructure.csproj", "ProjectS.UtilitiesService/UtilitiesService.Infrastructure/"]

# Restore
RUN dotnet restore "ProjectS.UtilitiesService/UtilitiesService.API/UtilitiesService.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/ProjectS.UtilitiesService/UtilitiesService.API"
RUN dotnet build "UtilitiesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------------
# Publish
# -----------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/ProjectS.UtilitiesService/UtilitiesService.API"
RUN dotnet publish "UtilitiesService.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# -----------------------------
# Final image
# -----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment variables (PostgreSQL)
ENV USERS_MICROSERVICE_NAME=users.api
ENV USERS_MICROSERVICE_PORT=8080
ENV MONGODB_CONNECTION_STRING=mongodb://mongo:27017
ENV MONGODB_DATABASE=Utilities
ENV RABBITMQ_HOST=rabbitmq
ENV RABBITMQ_USERNAME=user
ENV RABBITMQ_PASSWORD=password
ENV RABBITMQ_PORT=5672
ENV RABBITMQ_USERS_EXCHANGE=user.exchange

ENTRYPOINT ["dotnet", "UtilitiesService.API.dll"]
