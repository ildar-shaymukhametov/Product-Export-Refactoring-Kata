using System.Text;

namespace ProductExport
{
    public class TagNode
    {
        private readonly StringBuilder _attributes;
        private readonly List<TagNode> _children;
        private string _value;

        public string Name { get; }
        public TagNode Parent { get; set; }

        public TagNode(string name)
        {
            Name = name;
            _attributes = new StringBuilder();
            _children = new List<TagNode>();
        }

        internal void AddAttribute(string name, object value)
        {
            _attributes.Append(' ');
            _attributes.Append(name);
            _attributes.Append("=\"");
            _attributes.Append(value.ToString());
            _attributes.Append('"');
        }

        internal void AddValue(object value)
        {
            this._value = value.ToString();
        }

        public override string ToString()
        {
            return $"<{Name}{_attributes}>{RenderChildren()}{_value}</{Name}>";
        }

        public void Add(TagNode tagNode)
        {
            tagNode.Parent = this;
            _children.Add(tagNode);
        }

        private string RenderChildren()
        {
            return _children.Aggregate("", (acc, v) => acc += v);
        }
    }
}
