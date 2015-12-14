namespace DecisionTree.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ViewModel;

    using Attribute = ViewModel.Attribute;

    public class QueryRunner
    {
        private readonly Node _startNode;

        private readonly List<Attribute> _attributes;

        public QueryRunner(Node startNode, List<Attribute> attributes)
        {
            _startNode = startNode;
            _attributes = attributes;
        }

        public void RunQuery()
        {
            _startNode.ClearSelection();
            MarkNodes(_startNode);
        }

        private void MarkNodes(Node node)
        {
            if (node.Attribute.IsGoal)
            {
                node.IsSelected = true;
                return;
            }
            if (node.Value == string.Empty)
            {
                var nextNode = node.Children.SingleOrDefault(
                    delegate(Node n)
                        {
                            var value = _attributes.SingleOrDefault(x => x.Name == n.Attribute.Name).Value;
                            switch (n.Operation)
                            {
                                case Operation.Less:
                                    return double.Parse(n.Value) >= double.Parse(value);
                                case Operation.MoreEq:
                                    return double.Parse(n.Value) < double.Parse(value);
                                case Operation.Eq:
                                    return n.Value == value;
                                case null:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            return false;
                        });
                nextNode.IsSelected = true;
                MarkNodes(nextNode);
                return;
            }
            node.Children.ToList().ForEach(MarkNodes);
        }
    }
}
