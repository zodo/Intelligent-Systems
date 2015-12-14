namespace DecisionTree.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;

    /// <summary>
    /// Вершина дерева.
    /// </summary>
    public class Node : ObservableObject
    {
        private ObservableCollection<Node> _children = new ObservableCollection<Node>();

        private bool _isSelected;

        public Operation? Operation { get; set; }

        public Brush BackgroundColor => IsSelected ? Brushes.PaleGreen : Brushes.White;

        public Attribute Attribute { get; }

        public string Value { get; }

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
                
                return $"{Attribute?.Name ?? ""} {OperationExtension.ToString(Operation)} {Value??""}";
            }
            set
            {
                RaisePropertyChangedEvent(nameof(Text));
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChangedEvent(nameof(IsSelected));
                RaisePropertyChangedEvent(nameof(BackgroundColor));
            }
        }

        public Node(Attribute attribute, string value)
        {
            Attribute = attribute;
            Value = value;
        }

        public void ClearSelection()
        {
            IsSelected = false;
            Children.ToList().ForEach(x => x.ClearSelection());
        }
    }

    /// <summary>
    /// Условие перехода.
    /// </summary>
    public enum Operation
    {
        Less, MoreEq, Eq
    }

    public static class OperationExtension
    {
        public static string ToString(this Operation? op)
        {
            switch (op)
            {
                case Operation.Less:
                    return "<";
                case Operation.MoreEq:
                    return ">=";
                case Operation.Eq:
                    return "=";
                case null:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }
    }
}
