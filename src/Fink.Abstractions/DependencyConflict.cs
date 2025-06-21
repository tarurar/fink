namespace Fink.Abstractions;

public record DependencyConflict(
    DependencyName Name,
    IReadOnlyCollection<IDependencyVersioning> Versions,
    IReadOnlyCollection<Dependency> ConflictedDependencies);