namespace Fink.Abstractions;

public static class StringExtensions
{
    public static bool IsEmpty(this string src) => string.IsNullOrWhiteSpace(src);

    public static bool ContainsInvalidPathChars(this string src)
    {
        if (src == null)
        {
            return false;
        }

        foreach (char c in Path.GetInvalidPathChars())
        {
            if (src.Contains(c, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
