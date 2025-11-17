# Multi-stage build for CurlDotNet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY src/CurlDotNet/CurlDotNet.csproj ./CurlDotNet/
RUN dotnet restore "CurlDotNet/CurlDotNet.csproj"

# Copy source code
COPY src/ .

# Build
WORKDIR /src/CurlDotNet
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD dotnet --info || exit 1

ENTRYPOINT ["dotnet", "CurlDotNet.dll"]
