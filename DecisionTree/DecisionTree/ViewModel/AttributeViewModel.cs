namespace DecisionTree.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    public class AttributeViewModel : ObservableObject
    {
        private string _name;

        private string _value;

        private ObservableCollection<string> _possibleValues;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChangedEvent(nameof(Name));
            }
        }

        public string Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChangedEvent(nameof(Value));
            }
        }

        public ObservableCollection<string> PossibleValues
        {
            get
            {
                return _possibleValues;
            }
            set
            {
                _possibleValues = value;
                Value = _possibleValues?.FirstOrDefault();
                RaisePropertyChangedEvent(nameof(PossibleValues));
            }
        }

        private bool IsDiscrete => PossibleValues != null;

        public Visibility TextBoxVisibility => !IsDiscrete ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ComboBoxVisibility => IsDiscrete ? Visibility.Visible : Visibility.Collapsed;
    }
}
