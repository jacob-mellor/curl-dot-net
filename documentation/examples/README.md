# Examples

Sample projects that demonstrate CurlDotNet in multiple .NET languages.

## Contents

- `Example.cs` – quick-start console sample that copies a curl command, executes it, and prints the JSON response.
- `CSharp/Example.cs` – idiomatic C# project showcasing the fluent builder, middleware, and result helpers.
- `FSharp/Example.fs` – F# script demonstrating functional composition with CurlDotNet.
- `VB.NET/Example.vb` – VB.NET equivalent for teams modernizing legacy projects.

## Usage

Each sample is standalone. From the repo root:

```bash
dotnet run --project examples/CSharp/Example.csproj
```

The examples favor real-world APIs (GitHub, Stripe, DigitalOcean) and highlight how curl commands map directly into .NET.

## Contributing

If you add a new language or scenario:

1. Keep dependencies minimal (prefer the BCL).
2. Follow the same attribution header as the rest of the repo.
3. Document the scenario in `EXAMPLES.md` so developers can discover it from the README.

