using ProductExport;

internal class TagBuilder
{
    private readonly TagNode root;

    public TagBuilder(string rootTagName)
    {
        this.root = new TagNode(rootTagName);
    }

    internal string ToXml()
    {
        return root.ToString();
    }
}