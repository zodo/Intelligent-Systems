namespace DecisionTree.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Controller;

    using Microsoft.Win32;

    public class Presenter : ObservableObject
    {
        public ObservableCollection<AttributeViewModel> Attributes
        {
            get
            {
                return new ObservableCollection<AttributeViewModel>(_attributes.Where(x => !x.IsGoal));
            }
            set
            {
                _attributes = value;
                RaisePropertyChangedEvent(nameof(Attributes));
                RaisePropertyChangedEvent(nameof(CalculateBtnVisibility));
            }
        }

        public Visibility CalculateBtnVisibility => Attributes != null && Attributes.Any() ? Visibility.Visible : Visibility.Collapsed;

        public ICommand OpenFile => new DelegateCommand(OpenFileFunc);

        public ICommand Calculate => new DelegateCommand(CalculateFunc);

        private ObservableCollection<AttributeViewModel> _attributes;

        private TreeViewModel _treeViewModel;

        public TreeViewModel TreeViewModel
        {
            get
            {
                return _treeViewModel;
            }
            set
            {
                _treeViewModel = value;
                RaisePropertyChangedEvent(nameof(TreeViewModel));
            }
        }

        public Presenter()
        {
            _attributes = new ObservableCollection<AttributeViewModel>();
        }


        private void OpenFileFunc()
        {
            var dialog = new OpenFileDialog { DefaultExt = ".tsv" };

            var result = dialog.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            var filereader = new FileReader(dialog.FileName);

            Attributes = new ObservableCollection<AttributeViewModel>(filereader.ReadAttributes());
        }

        private void CalculateFunc()
        {
            Debug.WriteLine(string.Join("\n", _attributes.Select(x => $"{x.Name}, {x.Value}")));
        }
    }
}
