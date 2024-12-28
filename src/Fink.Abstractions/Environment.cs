using System.Collections.Immutable;

namespace Fink.Abstractions;

public sealed record Environment(string DotNetPath, IImmutableDictionary<string, string> EnvironmentVariables);