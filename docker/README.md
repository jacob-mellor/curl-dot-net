# Docker Support (Optional)

CurlDotNet does **NOT** require Docker. These files are provided as optional convenience for containerized deployment scenarios.

## Files

- `Dockerfile` - Builds CurlDotNet in a container
- `Dockerfile.docs` - Builds documentation in a container
- `docker-compose.yml` - Orchestration for multi-container setup

## Note

CurlDotNet is a pure .NET library and works directly on any system with .NET installed. Docker is completely optional and only useful if you specifically want to run CurlDotNet in containers.

For normal usage, just install the NuGet package:
```bash
dotnet add package CurlDotNet
```