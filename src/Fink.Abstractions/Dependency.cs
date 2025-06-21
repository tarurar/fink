namespace Fink.Abstractions;

public record Dependency(DependencyName Name, IDependencyVersioning Versioning, Dependency? ParentDependency = null)
{
    public DependencyPath Path => ParentDependency switch
    {
        null => DependencyPath.Create(PathSegment),
        _ => ParentDependency.Path.AddSegment(PathSegment)
    };

    internal DependencyPathSegment PathSegment => Versioning switch
    {
        null => new DependencyPathSegment(Name),
        _ => new DependencyPathSegment($"{Name} {Versioning.ToString()}")
    };
}
