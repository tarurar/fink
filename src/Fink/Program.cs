namespace Fink;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine($"Hello, {args[0]}!");
            return;
        }
    }
}
