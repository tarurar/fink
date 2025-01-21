namespace Fink.Abstractions;

public record DependencyName(string Name)
{
    public static implicit operator string(DependencyName name)
    {
        return name == null ? string.Empty : name.Name;
    }

    public override string ToString() => Name;
}