using System.Collections.Immutable;

namespace Fink.Abstractions;

public abstract record BuildOptions(
    string WorkingDirectory,
    IImmutableList<string> TargetFrameworks,
    IImmutableList<string> TargetsToBuild,
    IImmutableList<string> Arguments);