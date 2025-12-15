# Installation Guide

## 🎯 What You'll Learn

- How to install .NET SDK
- How to install CurlDotNet
- How to verify your installation
- Platform-specific setup instructions

## 📚 Prerequisites

- A computer running Windows, macOS, or Linux
- Internet connection
- 10-15 minutes

## 🚀 Quick Start

If you already have .NET installed, getting CurlDotNet is just one command:

```bash
dotnet add package CurlDotNet
```

That's it! Jump to [Verify Installation](#verify-installation) to confirm everything works.

## 📦 Step-by-Step Installation

### Step 1: Install .NET SDK

CurlDotNet requires .NET. Here's how to install it on your platform:

#### Windows

**Option 1: Official Installer (Recommended)**

1. Visit [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
2. Download the .NET SDK (not just the runtime)
3. Run the installer
4. Click through the wizard (defaults are fine)
5. Restart your terminal/command prompt

**Option 2: Windows Package Manager (winget)**

```powershell
winget install Microsoft.DotNet.SDK.8
```

**Option 3: Chocolatey**

```powershell
choco install dotnet-sdk
```

#### macOS

**Option 1: Official Installer (Recommended)**

1. Visit [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
2. Download the .NET SDK for macOS
3. Open the .pkg file
4. Follow the installation wizard
5. Restart your terminal

**Option 2: Homebrew**

```bash
brew install --cask dotnet-sdk
```

#### Linux (Ubuntu/Debian)

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

#### Linux (Fedora/RHEL/CentOS)

```bash
# Add Microsoft package repository
sudo dnf install dotnet-sdk-8.0
```

#### Linux (Arch)

```bash
sudo pacman -S dotnet-sdk
```

#### Linux (Other Distributions)

See the [official .NET installation guide](https://docs.microsoft.com/dotnet/core/install/linux) for your specific distribution.

### Step 2: Verify .NET Installation

Open a terminal/command prompt and run:

```bash
dotnet --version
```

You should see a version number like `8.0.100` or `10.0.0`. If you get an error, restart your terminal or check the [.NET installation troubleshooting guide](https://docs.microsoft.com/dotnet/core/install/).

### Step 3: Create a New Project

```bash
# Create a directory for your project
mkdir MyCurlApp
cd MyCurlApp

# Create a new console application
dotnet new console

# Verify the project was created
ls
```

You should see:
- `MyCurlApp.csproj` - Project file
- `Program.cs` - Your code file
- `obj/` directory - Build artifacts

### Step 4: Install CurlDotNet Package

Now install CurlDotNet using one of these methods:

#### Option 1: .NET CLI (Recommended)

```bash
dotnet add package CurlDotNet
```

#### Option 2: NuGet Package Manager (Visual Studio)

1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages"
3. Search for "CurlDotNet"
4. Click "Install"

#### Option 3: Package Manager Console (Visual Studio)

```powershell
Install-Package CurlDotNet
```

#### Option 4: Manual PackageReference

Edit your `.csproj` file and add:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="CurlDotNet" Version="1.0.0" />
  </ItemGroup>
</Project>
```

Then run:

```bash
dotnet restore
```

### Step 5: Verify CurlDotNet Installation

Create a simple test file to verify everything works:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing CurlDotNet installation...\n");

        try
        {
            var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/200");

            if (result.IsSuccess)
            {
                Console.WriteLine("✓ CurlDotNet is working correctly!");
                Console.WriteLine($"✓ Status Code: {result.StatusCode}");
                Console.WriteLine($"✓ .NET Version: {Environment.Version}");
            }
            else
            {
                Console.WriteLine($"✗ Unexpected status: {result.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
        }
    }
}
```

Run it:

```bash
dotnet run
```

You should see:

```
Testing CurlDotNet installation...

✓ CurlDotNet is working correctly!
✓ Status Code: 200
✓ .NET Version: 8.0.0
```

If you see this, congratulations! CurlDotNet is installed and working.

## 🔧 IDE and Editor Setup

### Visual Studio 2022 (Windows/Mac)

**Installation:**
1. Download from [visualstudio.microsoft.com](https://visualstudio.microsoft.com/)
2. During installation, select ".NET desktop development" workload
3. Install CurlDotNet via NuGet Package Manager

**Creating a project:**
1. File → New → Project
2. Select "Console App (.NET)"
3. Name your project
4. Right-click project → Manage NuGet Packages
5. Search "CurlDotNet" and install

### Visual Studio Code (Cross-Platform)

**Installation:**
1. Download from [code.visualstudio.com](https://code.visualstudio.com/)
2. Install the C# extension (by Microsoft)
3. Install the C# Dev Kit extension (optional but recommended)

**Creating a project:**
```bash
dotnet new console -n MyCurlApp
cd MyCurlApp
dotnet add package CurlDotNet
code .
```

**Running in VS Code:**
- Press `F5` to debug
- Or use the terminal: `dotnet run`

### JetBrains Rider (Cross-Platform)

**Installation:**
1. Download from [jetbrains.com/rider](https://www.jetbrains.com/rider/)
2. Install and open Rider
3. Create new solution or open existing

**Adding CurlDotNet:**
1. Right-click project → Manage NuGet Packages
2. Search "CurlDotNet"
3. Click Install

### Command Line / Terminal Only

You don't need an IDE! You can use any text editor:

```bash
# Create project
dotnet new console
dotnet add package CurlDotNet

# Edit with your favorite editor
nano Program.cs
# or
vim Program.cs
# or
code Program.cs

# Build and run
dotnet run
```

## 🎯 Platform-Specific Notes

### Windows

**Supported Versions:**
- Windows 10 (1607+)
- Windows 11
- Windows Server 2012 R2+

**Requirements:**
- .NET SDK 8.0 or later (or .NET Standard 2.0 compatible runtime)
- PowerShell 5.1+ or PowerShell Core 7+
- Windows Terminal recommended for best experience

**Common Issues:**
- If `dotnet` command not found, restart your terminal
- If SSL errors occur, ensure Windows is updated
- For Windows 7/8, use .NET Framework 4.7.2+

### macOS

**Supported Versions:**
- macOS 10.15 (Catalina) or later
- macOS 11 (Big Sur)
- macOS 12 (Monterey)
- macOS 13 (Ventura)
- macOS 14 (Sonoma)

**Requirements:**
- .NET SDK 8.0 or later
- Xcode Command Line Tools (install with: `xcode-select --install`)

**Common Issues:**
- If you get permission errors, check your Gatekeeper settings
- For M1/M2 Macs, use ARM64 version of .NET
- SSL certificates: use `security find-certificate -a` to verify system certs

### Linux

**Supported Distributions:**
- Ubuntu 20.04, 22.04, 24.04
- Debian 10, 11, 12
- Fedora 37+
- RHEL/CentOS 7, 8, 9
- Arch Linux (rolling)
- Alpine Linux 3.16+
- Many others via .NET Standard 2.0

**Requirements:**
- .NET SDK 8.0 or later
- OpenSSL 1.1 or 3.0
- ca-certificates package

**Common Issues:**
- If SSL errors occur: `sudo apt-get install ca-certificates` (Ubuntu/Debian)
- For permission issues: ensure user is in required groups
- Some distros need: `sudo apt-get install libicu-dev`

### Docker / Containers

**Dockerfile example:**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MyCurlApp.dll"]
```

**Building and running:**

```bash
docker build -t mycurlapp .
docker run mycurlapp
```

### Azure Functions / App Service

**Installation via portal:**
1. Create Azure Function or App Service
2. Select .NET 8.0 runtime
3. Add CurlDotNet to your project dependencies

**Local development:**

```bash
# Install Azure Functions Core Tools
npm install -g azure-functions-core-tools@4

# Create function app
func init MyFunctionApp --dotnet
cd MyFunctionApp
dotnet add package CurlDotNet

# Create function
func new --name MyHttpTrigger --template "HTTP trigger"

# Run locally
func start
```

## 📋 Version Compatibility

### .NET Target Frameworks

CurlDotNet supports these .NET versions:

| Target Framework | Support Status | Recommended |
|------------------|----------------|-------------|
| .NET 10.0 | ✅ Fully Supported | Yes |
| .NET 8.0 (LTS) | ✅ Fully Supported | **Best Choice** |
| .NET 6.0 (LTS) | ✅ Fully Supported | Yes |
| .NET 5.0 | ✅ Supported | No (EOL) |
| .NET Core 3.1 | ✅ Supported | No (EOL) |
| .NET Core 2.0+ | ✅ Via .NET Standard 2.0 | Legacy only |
| .NET Framework 4.7.2+ | ✅ Via .NET Standard 2.0 | Windows only |
| .NET Standard 2.0 | ✅ Fully Supported | Maximum compatibility |

**Recommendation:** Use .NET 8.0 (current LTS) for new projects.

### CurlDotNet Versions

```bash
# Install latest version (recommended)
dotnet add package CurlDotNet

# Install specific version
dotnet add package CurlDotNet --version 1.0.0

# Update to latest
dotnet add package CurlDotNet
```

**Version History:**
- `1.0.0` - Initial stable release
- `1.0.1` - Bug fixes and improvements (upcoming)

## 🔍 Troubleshooting Installation

### Problem: "dotnet: command not found"

**On Windows:**
1. Restart your command prompt/terminal
2. Check if .NET is in PATH: `echo %PATH%`
3. Manually add to PATH if needed: `C:\Program Files\dotnet`

**On macOS/Linux:**
1. Restart your terminal
2. Check PATH: `echo $PATH`
3. Add to PATH: `export PATH=$PATH:/usr/local/share/dotnet`
4. Add to shell profile: `echo 'export PATH=$PATH:/usr/local/share/dotnet' >> ~/.bashrc`

For more details, see the [troubleshooting guide](../troubleshooting/README.html).

### Problem: "Package 'CurlDotNet' not found"

**Solution:**
```bash
# Update NuGet sources
dotnet nuget list source

# Add NuGet.org if missing
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Clear cache and retry
dotnet nuget locals all --clear
dotnet restore
dotnet add package CurlDotNet
```

For more details, see the [troubleshooting guide](../troubleshooting/README.html).

### Problem: "The target framework 'netX.X' is not supported"

**Solution:**
```bash
# Check your .NET version
dotnet --version

# Update your project's target framework in .csproj
# Change from:
<TargetFramework>net5.0</TargetFramework>
# To:
<TargetFramework>net8.0</TargetFramework>

# Or for maximum compatibility:
<TargetFramework>netstandard2.0</TargetFramework>
```

### Problem: "SDK not found" or "A compatible SDK version was not found"

**Solution:**
1. Install .NET SDK (not just runtime)
2. Check installed SDKs: `dotnet --list-sdks`
3. If empty, reinstall .NET SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/)

For more details, see the [troubleshooting guide](../troubleshooting/README.html).

### Problem: SSL/TLS Errors on First Run

**On Windows:**
```bash
# Update certificates
certutil -generateSSTFromWU roots.sst
```

**On macOS:**
```bash
# Update certificates
security find-certificate -a -p /System/Library/Keychains/SystemRootCertificates.keychain > /dev/null
```

**On Linux:**
```bash
# Update CA certificates
sudo apt-get update
sudo apt-get install --reinstall ca-certificates
```

For more details, see the [troubleshooting guide](../troubleshooting/README.html).

### Problem: Permission Errors

**On Windows:**
Run terminal as Administrator

**On macOS/Linux:**
```bash
# Don't use sudo for dotnet commands
# Instead, fix permissions:
sudo chown -R $USER ~/.dotnet
sudo chown -R $USER ~/.nuget
```

## 🎓 Next Steps

Now that CurlDotNet is installed:

1. **Try it out** → [Your First Request](../tutorials/04-your-first-request.html)
2. **Learn the basics** → [Tutorials](../tutorials/README.html)
3. **Explore recipes** → [Cookbook](../cookbook/README.html)
4. **Read API docs** → [API Guide](../api-guide/README.html)

## 📚 Additional Resources

### Official Documentation

- [CurlDotNet Documentation](https://jacob-mellor.github.io/curl-dot-net/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [NuGet Package Page](https://www.nuget.org/packages/CurlDotNet/)

### Getting Help

- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - Bug reports
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) - Questions
- [Stack Overflow](https://stackoverflow.com/questions/tagged/curldotnet) - Community help

### Learning Resources

- [Tutorial Series](../tutorials/README.html) - Step-by-step learning
- [Cookbook Recipes](../cookbook/README.html) - Ready-to-use samples
- [API Reference](../api-guide/README.html) - Complete API reference

## 🎯 Quick Reference

```bash
# Install .NET SDK
# Visit: https://dotnet.microsoft.com/download

# Verify installation
dotnet --version

# Create new project
dotnet new console -n MyCurlApp
cd MyCurlApp

# Add CurlDotNet
dotnet add package CurlDotNet

# Run your app
dotnet run

# Build for production
dotnet publish -c Release
```

## ✅ Installation Checklist

Before continuing, make sure:

- [ ] .NET SDK is installed (`dotnet --version` works)
- [ ] CurlDotNet package is added to your project
- [ ] Test program runs successfully
- [ ] Your IDE/editor is set up (optional)
- [ ] You can make a simple HTTP request

If all boxes are checked, you're ready to start building with CurlDotNet!

---

**Ready to make your first request?** → [Your First Request](../tutorials/04-your-first-request.html)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/common-issues.html) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
