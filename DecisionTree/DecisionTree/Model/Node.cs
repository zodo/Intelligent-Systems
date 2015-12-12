namespace DecisionTree.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class Node
    {
        public readonly List<Node> Childrens;

        private readonly double _infoGain;

        private readonly Attribute _attr;

        private Node(Attribute attr)
        {
            _attr = attr;
            _infoGain = attr.InfoGain;
            Childrens = new List<Node>();
        }

        public Node(Attribute attr, string value) : this(attr)
        {
            Value = value;
        }

        public string Value { get; }

        public string Name => _attr.Name;

        public override string ToString() => $"{_attr.Name}: {Value}";
        
    }
}
