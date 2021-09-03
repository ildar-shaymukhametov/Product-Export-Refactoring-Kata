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

    [Fact]
    public void Build_one_child()
    {
        const string expectedXml = "<flavors><flavor></flavor></flavors>";
        var builder = new TagBuilder("flavors");
        builder.AddChild("flavor");
        var actualXml = builder.ToXml();
        Assert.Equal(expectedXml, actualXml);
    }

    [Fact]
    public void Build_children_of_children()
    {
        const string expectedXml = "<flavors><flavor><foo><bar></bar></foo></flavor></flavors>";
        var builder = new TagBuilder("flavors");
        builder.AddChild("flavor");
        builder.AddChild("foo");
        builder.AddChild("bar");
        var actualXml = builder.ToXml();
        Assert.Equal(expectedXml, actualXml);
    }

    [Fact]
    public void Build_sibling()
    {
        const string expectedXml = "<flavors><flavor1></flavor1><flavor2></flavor2></flavors>";
        var builder = new TagBuilder("flavors");
        builder.AddChild("flavor1");
        builder.AddSibling("flavor2");
        var actualXml = builder.ToXml();
        Assert.Equal(expectedXml, actualXml);
    }

    [Fact]
    public void Repeating_children_and_grandchildren()
    {
        const string expectedXml = "<flavors><flavor><foo><bar></bar></foo></flavor><flavor><foo><bar></bar></foo></flavor></flavors>";
        var builder = new TagBuilder("flavors");
        for (int i = 0; i < 2; i++)
        {
            builder.AddToParent("flavors", "flavor");
            builder.AddChild("foo");
            builder.AddChild("bar");
        }
        var actualXml = builder.ToXml();
        Assert.Equal(expectedXml, actualXml);
    }

    [Fact]
    public void Parent_not_found()
    {
        var builder = new TagBuilder("flavors");
        Assert.Throws<InvalidOperationException>(() => builder.AddToParent("favors", "flavor"));
    }

    [Fact]
    public void Attributes_and_values()
    {
        const string expectedXml = "<foo name=\"fox\"><bar><baz type=\"hardware\">value</baz><baz type=\"software\">value2</baz></bar></foo>";
        var builder = new TagBuilder("foo");
        builder.AddAttribute("name", "fox");
        builder.AddChild("bar");
        builder.AddToParent("bar", "baz");
        builder.AddAttribute("type", "hardware");
        builder.AddValue("value");
        builder.AddToParent("bar", "baz");
        builder.AddAttribute("type", "software");
        builder.AddValue("value2");
        var actualXml = builder.ToXml();
        Assert.Equal(expectedXml, actualXml);
    }
}