# Fink.Integrations.Buildalyzer

This integration project is for building .NET projects. Specifically, this integration with `Buildalyzer` is used to perform design-time builds and obtain the `project.assets.json` file.

## Features

- Default targets to build: `Clean`, `Build`
- Design-time build support
- Customizable path to the `dotnet` executable
- Environment variables
- Additional MSBuild command-line arguments
- Alternate working directory support

For more details, refer to the [Buildalyzer documentation](https://github.com/daveaglick/Buildalyzer).
