namespace Fink.Abstractions;

public record DependencyPathSegment(string Name)
{
    public string Name { get; init; } = string.IsNullOrWhiteSpace(Name)
        ? throw new ArgumentException("Value cannot be null or whitespace.", nameof(Name))
        : Name.Trim();

    public override string ToString() => Name;
}
