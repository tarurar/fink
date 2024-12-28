using System.Collections.Immutable;

namespace Fink.Abstractions;

public sealed record Solution(string SolutionFilePath, IImmutableList<Project> Projects);