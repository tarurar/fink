using System.Collections.Immutable;

namespace Fink.Abstractions;

public sealed record Project(string ProjectFilePath, IImmutableList<PackageDependency> Dependencies);