using System.Globalization;
using System.Resources;

using Buildalyzer;

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

        AnalyzerManager manager = new();
        IProjectAnalyzer project = manager.GetProject(args[0]);
        Console.WriteLine(
                    rm.GetString("BuildingProjectFmt", CultureInfo.InvariantCulture) ?? throw new InvalidOperationException(),
                    project.ProjectFile.Path);
        foreach (IAnalyzerResult buildResult in project.Build().Where(r => r.TargetFramework != null))
        {
            Console.WriteLine(
                rm.GetString("BuildingResultsFmt", CultureInfo.InvariantCulture) ?? throw new InvalidOperationException(),
                buildResult.TargetFramework);

            string messageResourceName = buildResult.Succeeded switch
            {
                true => "BuildSucceeded",
                false => "BuildFailed"
            };
            Console.WriteLine(rm.GetString(messageResourceName, CultureInfo.InvariantCulture));

            if (buildResult.Succeeded)
            {
                Console.WriteLine(
                rm.GetString("ProjectAssetsFileFmt", CultureInfo.InvariantCulture) ?? throw new InvalidOperationException(),
                buildResult.GetProperty("ProjectAssetsFile") ?? "N/A");
            }
        }
        Console.WriteLine(project.ProjectFile.Path);
    }
}