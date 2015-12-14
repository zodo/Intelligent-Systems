namespace DecisionTree.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Атрибут.
    /// </summary>
    public class Attribute : ObservableObject
    {
        private string _name;

        private string _value;

        private ObservableCollection<string> _possibleValues;

        /// <summary>
        /// Дискретен?
        /// </summary>
        public bool IsDiscrete => PossibleValues != null;

        /// <summary>
        /// Видимость текстбокса.
        /// </summary>
        public Visibility TextBoxVisibility => !IsDiscrete ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Видимость скомбобокса.
        /// </summary>
        public Visibility ComboBoxVisibility => IsDiscrete ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Целевой атрибут?
        /// </summary>
        public bool IsGoal { get; private set; }

        /// <summary>
        /// Значения атрибута.
        /// </summary>
        public List<string> Values { get; private set; } = new List<string>();

        public double InfoGain { get; set; }

        public Attribute(string name, string attr)
        {
            _name = name;
            IsGoal = attr == "g";
            if (attr == "d" || attr == "g")
            {
                PossibleValues = new ObservableCollection<string>();
            }
        }

        /// <summary>
        /// Имя.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent(nameof(Name));
            }
        }

        /// <summary>
        /// Введенное пользователем значение.
        /// </summary>
        public string Value 
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChangedEvent(nameof(Value));
            }
        }

        /// <summary>
        /// Возможные значения дискретного атрибута.
        /// </summary>
        public ObservableCollection<string> PossibleValues
        {
            get { return _possibleValues; }
            set
            {
                _possibleValues = value;
                Value = _possibleValues?.FirstOrDefault();
                RaisePropertyChangedEvent(nameof(PossibleValues));
            }
        }

        /// <summary>
        /// Отфильтровать набор значений атрибута.
        /// </summary>
        /// <param name="valuesIndexes">Индексы значений.</param>
        /// <returns>Новый атрибут, с отфильтрованным набором значений.</returns>
        public Attribute FilterValues(IEnumerable<int> valuesIndexes)
        {
            var attr = new Attribute(Name, "")
            {
                InfoGain = InfoGain,
                IsGoal = IsGoal,
                Value = Value,
                Values = Values.Select(y => y).ToList(),
                PossibleValues = PossibleValues
            };
            attr.Values = Values.Where((x, i) => valuesIndexes.Contains(i)).ToList();
            return attr;
        }
    }
}
