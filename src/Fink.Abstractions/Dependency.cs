namespace Fink.Abstractions;

public record Dependency(DependencyName Name, DependencyVersion? Version = null, Dependency? ParentDependency = null)
{
    public DependencyPath Path => ParentDependency switch
    {
        null => DependencyPath.Create(PathSegment),
        _ => ParentDependency.Path.AddSegment(PathSegment)
    };

    internal DependencyPathSegment PathSegment => Version switch
    {
        null => new DependencyPathSegment(Name),
        _ => new DependencyPathSegment($"{Name} {Version}")
    };
}