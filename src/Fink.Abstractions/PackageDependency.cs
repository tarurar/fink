namespace Fink.Abstractions;

public abstract record PackageDependency(PackageIdentity Id, PackageDependencyType Type, PackageMajorVersion MajorVersion);