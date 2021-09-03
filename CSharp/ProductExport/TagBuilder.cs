using ProductExport;

internal class TagBuilder
{
    private readonly TagNode root;
    private TagNode current;

    public TagBuilder(string rootTagName)
    {
        this.root = new TagNode(rootTagName);
        current = this.root;
    }

    internal string ToXml()
    {
        return root.ToString();
    }

    internal void AddChild(string tagName)
    {
        AddTo(current, tagName);
    }

    internal void AddSibling(string tagName)
    {
        AddTo(current.Parent, tagName);
    }

    private void AddTo(TagNode parent, string tagName)
    {
        current = new TagNode(tagName);
        parent.Add(current);
    }

    internal void AddToParent(string parent, string child)
    {
        var parentNode = FindParent(parent);
        if (parentNode == null)
        {
            throw new InvalidOperationException("parent not found");
        }
        AddTo(parentNode, child);
    }

    private TagNode FindParent(string tagName)
    {
        var parent = current;
        while (parent != null)
        {
            if (tagName == parent.Name)
            {
                return parent;
            }
            parent = parent.Parent;
        }
        return null;
    }

    internal void AddAttribute(string name, object value)
    {
        current.AddAttribute(name, value);
    }

    internal void AddValue(object value)
    {
        current.AddValue(value);
    }
}