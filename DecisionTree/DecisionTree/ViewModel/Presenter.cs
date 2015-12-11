namespace DecisionTree.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    public class Presenter : ObservableObject
    {
        public ObservableCollection<AttributeViewModel> Attributes
        {
            get
            {
                return _attributes;
            }
            set
            {
                _attributes = value;
                RaisePropertyChangedEvent(nameof(Attributes));
            }
        }

        public ICommand AddNewControl => new DelegateCommand(AddNewCtrl);

        public ICommand ShowAdded => new DelegateCommand(ShowAttr);

        private ObservableCollection<AttributeViewModel> _attributes;

        public Presenter()
        {
            _attributes = new ObservableCollection<AttributeViewModel>();
        }


        private void AddNewCtrl()
        {
            _attributes.Add(new AttributeViewModel {Name = Guid.NewGuid().ToString(), PossibleValues = new ObservableCollection<string> {"hi", "there", "johny"} });
        }

        private void ShowAttr()
        {
            Debug.WriteLine(string.Join("\n", _attributes.Select(x => $"{x.Name}, {x.Value}")));
        }
    }
}
