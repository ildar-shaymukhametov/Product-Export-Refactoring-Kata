using ProductExport;
using Xunit;

public class TagNodeTest
{
    const string Price = "149.99";

    [Fact]
    public void Tag_with_one_value_and_attribute()
    {
        var tag = new TagNode("price");
        tag.AddAttribute("currency", "USD");
        tag.AddValue(Price);
        var expected = $"<price currency=\"USD\">{Price}</price>";
        Assert.Equal(expected, tag.ToString());
    }

    [Fact]
    public void Tag_with_one_child()
    {
        var tag = new TagNode("product");
        tag.Add(new TagNode("price"));
        var expected = $"<product><price></price></product>";
        Assert.Equal(expected, tag.ToString());
    }

    [Fact]
    public void Tag_with_children_and_grandchildren()
    {
        var ordersTag = new TagNode("orders");
        var orderTag = new TagNode("order");
        var productTag = new TagNode("product");
        ordersTag.Add(orderTag);
        orderTag.Add(productTag);
        var expected = $"<orders><order><product></product></order></orders>";
        Assert.Equal(expected, ordersTag.ToString());
    }

    [Fact]
    public void Parent_initially_null()
    {
        var root = new TagNode("root");
        Assert.Null(root.Parent);
    }

    [Fact]
    public void Child_has_parent()
    {
        var root = new TagNode("root");
        var child = new TagNode("child");
        root.Add(child);
        Assert.Equal(root, child.Parent);
        Assert.Equal("root", child.Parent.Name);
    }
}