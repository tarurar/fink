using System.Collections.Immutable;
using System.Globalization;
using System.Resources;

using Fink.Integrations.Buildalyzer;

namespace Fink;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        ResourceManager rm = new("Fink.Resources", typeof(Program).Assembly);

        if (args.Length == 0)
        {
            Console.WriteLine(rm.GetString("UsageMessage", CultureInfo.InvariantCulture));
            return;
        }

        if (!args[0].EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine(rm.GetString("InvalidFileExtension", CultureInfo.InvariantCulture));
            return;
        }

        IEnumerable<DotNetProjectBuildResult> results = DotNetProjectBuilder.Build(
            args[0],
            new Abstractions.Environment(
                string.Empty,
                ImmutableDictionary<string, string>.Empty),
            new BuildalyzerBuildOptions(
                string.Empty,
                [args[1]],
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty));
        // ImmutableList.Create(
        //     "/p:BaseOutputPath=/Users/atarutin/RiderProjects/bookkeeper/src/BookKeeper/xxx_bin/",
        //     "/p:BaseIntermediateOutputPath=/Users/atarutin/RiderProjects/bookkeeper/src/BookKeeper/xxx_obj/")));

        DotNetProjectBuildResult result = results.First();

        if (result is DotNetProjectBuildError buildError)
        {
            Console.WriteLine(rm.GetString("BuildFailed", CultureInfo.InvariantCulture));
            Console.WriteLine(buildError.BuildLog);
            return;
        }

        Console.WriteLine(rm.GetString("BuildSucceeded", CultureInfo.InvariantCulture));
        Console.WriteLine($"Lock file path: {result.LockFilePath}");
    }
}