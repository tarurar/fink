namespace Fink.Abstractions;

public record FilePath(string Value)
{
    public string Value { get; init; } = Value switch
    {
        _ when Value.IsEmpty() => string.Empty,
        _ when Value.ContainsInvalidPathChars() => throw new ArgumentException("Value contains invalid characters."),
        _ => Value.Trim()
    };

    public bool IsEmpty => Value.IsEmpty();

    public override string ToString() => Value;

    public static FilePath FromString(string filePath) => new(filePath);

    public static FilePath Empty => new(string.Empty);

    public static implicit operator string(FilePath filePath)
    {
        return filePath == null ? string.Empty : filePath.Value;
    }

    public static implicit operator FilePath(string filePath)
    {
        return FromString(filePath);
    }
}
