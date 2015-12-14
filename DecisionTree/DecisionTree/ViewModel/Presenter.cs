namespace DecisionTree.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Controller;

    using Microsoft.Win32;

    public class Presenter : ObservableObject
    {
        public ObservableCollection<Attribute> Attributes
        {
            get
            {
                return new ObservableCollection<Attribute>(_attributes.Where(x => !x.IsGoal));
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

        private ObservableCollection<Attribute> _attributes;

        private ObservableCollection<Node>  _node = new ObservableCollection<Node>();


        public ObservableCollection<Node> Node
        {
            get
            {
                return _node;
            }
            set
            {
                _node = value;
                RaisePropertyChangedEvent(nameof(Node));
            }
        }

        

        public Presenter()
        {
            _attributes = new ObservableCollection<Attribute>();
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

            Attributes = new ObservableCollection<Attribute>(filereader.ReadAttributes());

            var treeSolver = new DecisionTreeSolver(_attributes.ToList());
            Node.Clear();
            Node.Add(treeSolver.MountTree());
        }

        private void CalculateFunc()
        {
            var queryRunner = new QueryRunner(Node.First(), Attributes.ToList());
            queryRunner.RunQuery();
        }
    }
}
