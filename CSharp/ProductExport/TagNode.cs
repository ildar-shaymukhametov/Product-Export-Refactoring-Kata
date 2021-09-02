using System.Text;

namespace ProductExport
{
    public class TagNode
    {
        private readonly string _name;
        private readonly StringBuilder _attributes;
        private readonly List<TagNode> _children;
        private string _value;

        public TagNode(string name)
        {
            _name = name;
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
            return $"<{_name}{_attributes}>{RenderChildren()}{_value}</{_name}>";
        }

        public void Add(TagNode tagNode)
        {
            _children.Add(tagNode);
        }

        private string RenderChildren()
        {
            return _children.Aggregate("", (acc, v) => acc += v);
        }
    }
}
