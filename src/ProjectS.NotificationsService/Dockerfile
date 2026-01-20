# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8081

# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ---- Copy csproj files (cache-friendly)
COPY ["ProjectS.NotificationsService/NotificationsService.API/NotificationsService.API.csproj", "ProjectS.NotificationsService/NotificationsService.API/"]
COPY ["ProjectS.NotificationsService/NotificationsService.Application/NotificationsService.Application.csproj", "ProjectS.NotificationsService/NotificationsService.Application/"]
COPY ["ProjectS.NotificationsService/NotificationsService.Domain/NotificationsService.Domain.csproj", "ProjectS.NotificationsService/NotificationsService.Domain/"]
COPY ["ProjectS.NotificationsService/NotificationsService.Infrastructure/NotificationsService.Infrastructure.csproj", "ProjectS.NotificationsService/NotificationsService.Infrastructure/"]

# 🔹 Shared contracts
COPY ["Shared/UsersService.Contracts/UsersService.Contracts.csproj", "Shared/UsersService.Contracts/"]

# ---- Restore
RUN dotnet restore "ProjectS.NotificationsService/NotificationsService.API/NotificationsService.API.csproj"

# ---- Copy everything
COPY . .

# ---- Build
WORKDIR "/src/ProjectS.NotificationsService/NotificationsService.API"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# =========================
# Publish
# =========================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationsService.API.dll"]
