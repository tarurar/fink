using System.Collections.Immutable;

namespace Fink.Abstractions;

public record DependencyPath(IImmutableList<DependencyPathSegment> Segments)
{
    public DependencyPath AddSegment(DependencyPathSegment segment) =>
        new(Segments.Add(segment));

    public static DependencyPath Create(DependencyPathSegment root) =>
        new(ImmutableList.Create(root));

    public override string ToString() => string.Join("->", Segments);
}
