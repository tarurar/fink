using FsCheck;
using FsCheck.Fluent;

namespace Fink.Abstractions.Tests;

public class DependencyPathSegmentTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Cannot_Create_With_Empty_Name(string name)
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new DependencyPathSegment(name));
    }

    [Theory]
    [InlineData("name")]
    [InlineData(" name ")]
    [InlineData(" name")]
    [InlineData("name ")]
    public void Trims_Name(string name)
    {
        DependencyPathSegment segment = new(name);

        Assert.Equal("name", segment.Name);
    }

    [Fact]
    public void ToString_Returns_Name()
    {
        Prop.ForAll<NonEmptyString>(name =>
        {
            DependencyPathSegment segment = new(name.Get);

            return segment.ToString() == segment.Name;
        }).QuickCheckThrowOnFailure();
    }
}