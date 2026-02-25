# AGENTS.md - Coding Agent Guidelines for Fink

## Project Overview

Fink is a .NET 9.0 dependency analysis tool that detects NuGet package version conflicts. Solution: `src/Fink.sln`

## Build/Lint/Test Commands

```bash
# Build the solution
dotnet build ./src/Fink.sln

# Run all tests
dotnet test ./src/Fink.sln

# Run a single test by name pattern
dotnet test ./src/Fink.sln --filter "FullyQualifiedName~TestName"

# Run a single test by class
dotnet test ./src/Fink.sln --filter "FullyQualifiedName~ClassName"

# Run tests without rebuilding (after already built)
dotnet test ./src/Fink.sln --no-build

# Run security vulnerability scan
dotnet list ./src/Fink.sln package --vulnerable --include-transitive

# Check for deprecated packages
dotnet list ./src/Fink.sln package --deprecated --include-transitive
```

## Project Structure

```
src/
├── Fink/                          # Main console application
├── Fink.Abstractions/             # Core domain models and interfaces
├── Fink.Abstractions.Tests/       # Unit tests (xUnit + FsCheck)
├── Fink.Build/                    # MSBuild custom tasks
├── Fink.Integrations.Buildalyzer/ # Buildalyzer integration
├── Fink.Integrations.NuGet/       # NuGet APIs integration
├── Directory.Build.props          # Centralized build configuration
└── Directory.Packages.props       # Centralized package versions
```

## Code Style Guidelines

### Formatting

- Use 4 spaces for indentation (no tabs)
- Max line length: 100 characters
- File-scoped namespaces: `namespace Fink.Abstractions;`
- Braces on new lines (Allman style)
- Space after keywords: `if`, `for`, `foreach`, `while`, `using`
- Space around binary operators

### Imports

- Place `using` directives outside namespace
- Sort `System.*` directives first
- Separate import directive groups
- Implicit usings enabled

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes, records | PascalCase | `DependencyAnalyzer` |
| Interfaces | IPascalCase | `IDependencyVersioning` |
| Type parameters | TPascalCase | `TResult` |
| Methods, Properties | PascalCase | `FindVersionConflicts` |
| Private instance fields | _camelCase | `_dependencies` |
| Private static fields | s_camelCase | `s_cache` |
| Constants | PascalCase | `DefaultTimeout` |
| Local variables, Parameters | camelCase | `lockFilePath` |

### Types

- Use `record` types for immutable data: `public record Dependency(...)`
- Seal concrete classes that aren't designed for inheritance
- Use nullable reference types: `Nullable` enabled

### Code Patterns

#### Result Pattern (Functional Error Handling)

```csharp
public abstract record Result;
public interface ISuccessResult;
public interface IErrorResult;

public sealed record SuccessResult : Result, ISuccessResult;
public sealed record ErrorResult(string Message) : Result, IErrorResult;

Result result = operation();
return result switch
{
    ISuccessResult => HandleSuccess(),
    IErrorResult error => HandleError(error),
    _ => throw new InvalidOperationException()
};
```

#### Functional Chaining

```csharp
return args.Validate()
    .Bind(() => Build(projectPath))
    .Bind<BuildSuccess>(s => Analyze(s.OutputPath))
    .Tap(result => Log(result))
    .Map(ToExitCode);
```

#### Pattern Matching

```csharp
return result switch
{
    { Count: > 0 } conflicts => new ConflictsDetectedError(conflicts),
    _ => new Success()
};
```

### Error Handling

- Use the Result pattern instead of throwing exceptions for expected failures
- Use `ArgumentNullException.ThrowIfNull()` for parameter validation
- Throw `InvalidOperationException` for invalid states
- Catch and convert exceptions to error results at boundaries

### Comments

- Do NOT add comments to code unless explicitly requested
- Code should be self-documenting through clear naming

## Testing Guidelines

### Test Framework

- xUnit for unit tests
- FsCheck for property-based testing
- Test project naming: `*.Tests`

### Test Naming

- Test method names use underscores (CA1707 disabled for tests)
- Pattern: `When_Condition_Then_ExpectedBehavior`

### Test Structure

```csharp
[Fact]
public void When_Condition_Then_ExpectedBehavior()
{
    var input = new Dependency(new("name"), new DependencyVersion("1.0"));
    
    var result = input.Path;
    
    Assert.Single(result.Segments);
}
```

### Running Tests

- Always verify tests fail before marking complete
- Do NOT use `--no-build` when testing changed code

## Static Analysis

- All warnings treated as errors: `TreatWarningsAsErrors=true`
- Code analysis enabled: `AnalysisLevel=latest`, `AnalysisMode=AllEnabledByDefault`

### Disabled Warnings

- `IDE0210`: Top-level statements warning
- `CS1591`: Missing XML documentation
- `IDE0008`: Use var for built-in types
- `CA1707`: Underscores in test method names (tests only)
- `IDE0055`: Fix formatting
- `IDE0046`: Convert to conditional expression

## Key Conventions

1. Prefer immutability - use `record` types with `init` or positional parameters
2. Prefer expression-bodied members for single-line properties/methods
3. Prefer pattern matching over `is`/`as` casts
4. Prefer switch expressions over switch statements
5. Use LINQ and lambda expressions for collection operations
6. Every method should have a single responsibility
7. Every method should return a value (avoid void when possible)
8. Avoid assignments; prefer returning values directly
