using Buildalyzer.Environment;

using Environment = Fink.Abstractions.Environment;

namespace Fink.Integrations.Buildalyzer;

internal static class EnvironmentExtensions
{
    public static EnvironmentOptions Map(this Environment env, BuildalyzerBuildOptions buildOptions) =>
        new EnvironmentOptions { DotnetExePath = env.DotNetPath, WorkingDirectory = buildOptions.WorkingDirectory, }
            .AddTargetsFromScratch(buildOptions.TargetsToBuild)
            .AddArgumentsFromScratch(buildOptions.Arguments)
            .AddEnvVarsFromScratch(env.EnvironmentVariables);

    public static EnvironmentOptions AddTargetsFromScratch(this EnvironmentOptions options, IEnumerable<string> targets)
    {
        options.TargetsToBuild.Clear();
        options.TargetsToBuild.AddRange(targets);

        return options;
    }

    public static EnvironmentOptions AddArgumentsFromScratch(
    this EnvironmentOptions options,
    IEnumerable<string> arguments)
    {
        options.Arguments.Clear();
        foreach (string arg in arguments)
        {
            options.Arguments.Add(arg);
        }

        return options;
    }

    public static EnvironmentOptions AddEnvVarsFromScratch(
    this EnvironmentOptions options,
    IEnumerable<KeyValuePair<string, string>>
        environmentVariables)
    {
        options.EnvironmentVariables.Clear();
        foreach (KeyValuePair<string, string> kvp in environmentVariables)
        {
            options.EnvironmentVariables.Add(kvp.Key, kvp.Value);
        }

        return options;
    }
}