# -----------------------------
# Runtime image
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

# -----------------------------
# Build image
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj first (for Docker cache)
COPY ProjectS.Gateway.csproj ./

# Restore
RUN dotnet restore ProjectS.Gateway.csproj

# Copy everything else
COPY . .

# Build
RUN dotnet build ProjectS.Gateway.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# -----------------------------
# Publish
# -----------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ProjectS.Gateway.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# -----------------------------
# Final image
# -----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV USERS_API_URL=http://localhost:8080

ENTRYPOINT ["dotnet", "ProjectS.Gateway.dll"]
