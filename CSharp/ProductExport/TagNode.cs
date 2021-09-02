using System.Text;

namespace ProductExport
{
    public class TagNode
    {
        private readonly string _name;
        private readonly StringBuilder _attributes;
        private string _value;

        public TagNode(string name)
        {
            _name = name;
            _attributes = new StringBuilder();
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
            return $"<{_name}{_attributes}>{_value}</{_name}>";
        }
    }
}
