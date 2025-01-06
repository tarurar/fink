using System.Collections.Immutable;

namespace Fink.Abstractions;

public sealed record Environment(string DotNetPath, IImmutableDictionary<string, string> EnvironmentVariables)
{
    public string DotNetPath { get; init; } =
        string.IsNullOrWhiteSpace(DotNetPath) ? "dotnet" : DotNetPath.Trim();
}