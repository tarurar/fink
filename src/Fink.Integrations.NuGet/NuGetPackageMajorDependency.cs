using Fink.Abstractions;

namespace Fink.Integrations.NuGet;

internal sealed record NuGetPackageMajorDependency(PackageIdentity Id, PackageMajorVersion MajorVersion) : PackageDependency(Id, MajorVersion);