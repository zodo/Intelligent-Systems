namespace DecisionTree.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Node : ObservableObject
    {
        private ObservableCollection<Node> _children = new ObservableCollection<Node>();

        private string _text;

        public ObservableCollection<Node> Children    
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
                RaisePropertyChangedEvent(nameof(Children));
            }
        }

        public string Text
        {
            get
            {
                return Operation != null? Operation + " " + _text : "" + _text;
            }
            set
            {
                _text = value;
                RaisePropertyChangedEvent(nameof(Text));
            }
        }


        public Operation? Operation { get; set; }

        public Node(Attribute attr, string value)
        {
            Text = $"{attr.Name}: {value}";
        }
    }

    public enum Operation
    {
        Less, MoreEq, Eq
    }
}
