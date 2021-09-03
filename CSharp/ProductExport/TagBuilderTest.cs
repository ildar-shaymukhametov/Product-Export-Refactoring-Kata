using Xunit;

public class TagBuilderTest
{
    [Fact]
    public void Build_one_node()
    {
        const string expectedXml = "<flavors></flavors>";
        var actualXml = new TagBuilder("flavors").ToXml();
        Assert.Equal(expectedXml, actualXml);
    }
}