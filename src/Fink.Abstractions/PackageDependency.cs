namespace Fink.Abstractions;

public abstract record PackageDependency(PackageIdentity Id, PackageMajorVersion MajorVersion);