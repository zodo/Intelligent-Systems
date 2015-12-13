namespace DecisionTree.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    public class Attribute : ObservableObject, ICloneable
    {
        private string _name;

        private string _value;

        private ObservableCollection<string> _possibleValues;

        public Attribute(string name, string attr)
        {
            _name = name;
            IsGoal = attr == "g";
            if (attr == "d" || attr == "g")
            {
                PossibleValues = new ObservableCollection<string>();
            }
        }

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

        public bool IsDiscrete => PossibleValues != null;

        public Visibility TextBoxVisibility => !IsDiscrete ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ComboBoxVisibility => IsDiscrete ? Visibility.Visible : Visibility.Collapsed;

        public bool IsGoal { get; set; }

        public List<string> Values { get; set; } = new List<string>();

        public double InfoGain { get; set; }

        public int IntValue => Convert.ToInt32(Value);

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        public Attribute Clone()
        {
            return new Attribute(Name, "") { InfoGain = InfoGain, IsGoal = IsGoal, Value = Value, Values = Values.Select(y => y).ToList(), PossibleValues = PossibleValues };
        }

        public Attribute FilterValues(int[] valuesIndexes)
        {
            var attr = Clone();
            attr.Values = Values.Where((x, i) => valuesIndexes.Contains(i)).ToList();
            return attr;
        }

    }
}
