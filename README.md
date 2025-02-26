# 🌟 Fink - .NET Dependency Analysis Tool

![CI](https://github.com/tarurar/fink/actions/workflows/ci.yml/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Welcome to **Fink** - a powerful .NET tool designed to analyze and identify dependency version conflicts in your projects! 🚀

## 📚 Summary of Project

Fink is a specialized tool that helps you manage and troubleshoot NuGet package dependencies in .NET projects. It focuses on:

- 🔍 Detecting multiple versions of the same package in your dependency tree
- 📈 Visualizing dependency paths to understand how conflicts occur
- 🛠️ Building projects with customizable options to analyze dependencies accurately

The core architecture is organized into several components:

- **Fink.Abstractions**: Core domain models and interfaces
- **Fink.Integrations.Buildalyzer**: Integration with Buildalyzer for .NET project building
- **Fink.Integrations.NuGet**: Integration with NuGet APIs for dependency analysis

Fink helps you solve the "dependency hell" problem by providing clear insights into your project's package ecosystem.

## 🚀 How to Use

### Prerequisites

- .NET 9.0 SDK or later
- A .NET project with NuGet dependencies

### Installation

Clone the repository and build the solution:

```bash
git clone https://github.com/tarurar/fink.git
cd fink
dotnet build
```

### Basic Usage

Run Fink against your project file:

```bash
dotnet run --project src/Fink/Fink.csproj <path-to-your-project.csproj> <target-framework>
```

For example:

```bash
dotnet run --project src/Fink/Fink.csproj ~/Projects/MyApp/MyApp.csproj net9.0
```

### Understanding the Output

Fink will:
1. Build your project to generate an up-to-date `project.assets.json` file
2. Analyze the dependencies
3. Report any packages that have multiple versions in use
4. Show the dependency paths that lead to each version

Example output:
```
Build succeeded
Lock file path: /Users/username/Projects/MyApp/obj/project.assets.json
Number of dependencies: 127
Number of distinct dependencies: 118
Number of dependencies with multiple versions: 3
Package Newtonsoft.Json has 2 versions:
  13.0.1 (Path: net9.0->Newtonsoft.Json 13.0.1)
  12.0.3 (Path: net9.0->SomePackage 2.0.0->Newtonsoft.Json 12.0.3)
...
```

## 🔧 Tech Info

### Project Architecture

Fink follows a modular, domain-driven design:

- **Fink.Abstractions**: Core domain models like `Dependency`, `DependencyPath`, and `Environment`
- **Fink.Integrations.Buildalyzer**: Project building capabilities using Buildalyzer
- **Fink.Integrations.NuGet**: NuGet lock file parsing and dependency resolution
- **Fink.Abstractions.Tests**: Comprehensive test coverage for the domain models

### Key Technologies

- **Language**: C# 12 with .NET 9.0
- **Build System**: MSBuild with centralized package version management
- **Project Analysis**: Buildalyzer for design-time builds
- **Dependency Analysis**: NuGet.ProjectModel for parsing lock files
- **Testing**: xUnit with FsCheck for property-based testing
- **CI/CD**: GitHub Actions for continuous integration

### Development

1. Fork the repository
2. Create a feature branch
3. Implement your changes with tests
4. Run the tests with `dotnet test`
5. Submit a pull request

The project uses strict code analysis and treats warnings as errors to maintain high code quality.

### Requirements

- .NET 9.0 SDK (specified in global.json)
- Compatible with Windows, macOS, and Linux

Fink is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Made with ❤️ by [tarurar](https://github.com/tarurar)
