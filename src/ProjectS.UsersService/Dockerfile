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
COPY ["UsersService.API/UsersService.API.csproj", "UsersService.API/"]
COPY ["UsersService.Application/UsersService.Application.csproj", "UsersService.Application/"]
COPY ["UsersService.Domain/UsersService.Domain.csproj", "UsersService.Domain/"]
COPY ["UsersService.Infrastructure/UsersService.Infrastructure.csproj", "UsersService.Infrastructure/"]

# Restore
RUN dotnet restore "UsersService.API/UsersService.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/UsersService.API"
RUN dotnet build "UsersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------------
# Publish
# -----------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UsersService.API.csproj" \
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
ENV POSTGRES_HOST=localhost
ENV POSTGRES_PASSWORD=admin
ENV POSTGRES_DATABASE=UsersService
ENV POSTGRES_USER=postgres
ENV POSTGRES_PORT=5432

ENTRYPOINT ["dotnet", "UsersService.API.dll"]
