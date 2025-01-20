using FsCheck;
using FsCheck.Fluent;

namespace Fink.Abstractions.Tests;

internal static class Gen
{
    public static Arbitrary<string> PathString =>
        ArbMap.Default.GeneratorFor<string>()
            .Where(s => !s.ContainsInvalidPathChars())
            .ToArbitrary();
}
