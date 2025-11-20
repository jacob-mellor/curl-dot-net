# Lightweight Windows container for building .NET Framework targets
# This uses Microsoft's official .NET SDK image which is much smaller than full Windows
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build

# Install .NET 9 SDK
RUN powershell -Command \
    Invoke-WebRequest -Uri https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1; \
    ./dotnet-install.ps1 -Channel 9.0; \
    Remove-Item -Force dotnet-install.ps1

# Set working directory
WORKDIR /src

# Copy project files
COPY src/CurlDotNet/CurlDotNet.csproj src/CurlDotNet/
RUN dotnet restore src/CurlDotNet/CurlDotNet.csproj

# Copy source code
COPY . .

# Build the project with all Windows frameworks
RUN dotnet build src/CurlDotNet/CurlDotNet.csproj \
    --configuration Release \
    --no-restore \
    -p:TargetFrameworks="netstandard2.0;net472;net48;net8.0;net9.0"

# Create NuGet package
RUN dotnet pack src/CurlDotNet/CurlDotNet.csproj \
    --configuration Release \
    --no-build \
    --output /packages

# Final stage - just the packages
FROM scratch AS packages
COPY --from=build /packages/*.nupkg /