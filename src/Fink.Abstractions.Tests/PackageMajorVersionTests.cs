using FsCheck;
using FsCheck.Fluent;

namespace Fink.Abstractions.Tests;

public class PackageMajorVersionTests
{
    [Fact]
    public void Create_With_Non_Negative_Version_Number()
    {
        Prop.ForAll<NonNegativeInt>(i => PackageMajorVersion.Create(i.Get).MajorVersion == i.Get)
            .QuickCheckThrowOnFailure();
    }

    [Fact]
    public void Create_With_Negative_Version_Number_Raises_Exception()
    {
        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() => PackageMajorVersion.Create(-1));
    }
}