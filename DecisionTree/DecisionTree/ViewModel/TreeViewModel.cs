namespace DecisionTree.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class TreeViewModel : ObservableObject
    {
        private ObservableCollection<TreeViewModel> _children;

        private string _text;

        public ObservableCollection<TreeViewModel> Children    
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
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChangedEvent(nameof(Text));
            }
        }
    }
}
