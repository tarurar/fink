using System.Collections.Immutable;

using Fink.Abstractions;

namespace Fink.Integrations.Buildalyzer;

public sealed record BuildalyzerBuildOptions(
    string WorkingDirectory,
    IImmutableList<string> TargetFrameworks,
    IImmutableList<string> TargetsToBuild,
    IImmutableList<string> Arguments)
    : BuildOptions(WorkingDirectory, TargetFrameworks, TargetsToBuild, Arguments)
{
    public static BuildalyzerBuildOptions Default => new(
        WorkingDirectory: string.Empty,
        TargetFrameworks: ImmutableList<string>.Empty,
        TargetsToBuild: ["Clean", "Build"],
        Arguments: ImmutableList<string>.Empty);
}
