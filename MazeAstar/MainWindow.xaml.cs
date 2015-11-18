namespace MazeAstar
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using Algorithm.Distance;

    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private WriteableBitmap _bitmap;

        private int _imageWidth => (int)ImageBorder.ActualWidth;
        private int _imageHeight => (int)ImageBorder.ActualHeight;

        private bool _contentRendered;

        private MazeDrawer _mazeDrawer;

        private HeuristicType _heuristicType = HeuristicType.Euclidean;

        public MainWindow()
        {
            _mazeDrawer = MazeDrawer.GetInstance;
            InitializeComponent();

            SettingsFlyout.ClosingFinished += (sender, args) => OpenSettingsButton.Visibility = Visibility.Visible;

           
        }

        private void ButtonSettins_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = true;
            OpenSettingsButton.Visibility = Visibility.Hidden;
        }

        private void MazeSizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_contentLoaded)
            {
                _mazeDrawer.CellSize = (int)CellSizeSlider.Value;
                _mazeDrawer.ReinitMaze();
                _mazeDrawer.Redraw();
            }
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            FindPath();
        }

        private void FindPath()
        {
            _mazeDrawer.Maze.FindPath(_heuristicType);
            _mazeDrawer.Redraw();
            if (_mazeDrawer.Maze.History != null)
            {
                HistorySlider.Maximum = _mazeDrawer.Maze.History.IterationAmount;
                HistorySlider.IsEnabled = true;
            }
            
        }

        private void ResultImage_OnMouseDown(object sender, MouseEventArgs e)
        {

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _mazeDrawer.HandleClick(e.GetPosition(ImageBorder), CellType.Start);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    _mazeDrawer.HandleClick(e.GetPosition(ImageBorder), CellType.Finish);
                }
                _mazeDrawer.Redraw();
            }
            else
            {
                ResultImage_OnMouseMove(sender, e);
            }
        }

        private void ResultImage_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            _bitmap = BitmapFactory.New(_imageWidth, _imageHeight);
            ResultImage.Source = _bitmap;
            _mazeDrawer = MazeDrawer.GetInstance;
            _mazeDrawer.Bitmap = _bitmap;
            _mazeDrawer.CellSize = (int)CellSizeSlider.Value;
            _mazeDrawer.ReinitMaze();
            _mazeDrawer.Redraw();
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _bitmap = BitmapFactory.New(_imageWidth, _imageHeight);
            ResultImage.Source = _bitmap;
            _mazeDrawer.Bitmap = _bitmap;
            _mazeDrawer.ReinitMaze();
            _mazeDrawer.Redraw();

        }

        private void ResultImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mazeDrawer.HandleClick(e.GetPosition(ImageBorder), CellType.Wall);
                _mazeDrawer.Redraw();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _mazeDrawer.HandleClick(e.GetPosition(ImageBorder), CellType.Empty);
                _mazeDrawer.Redraw();
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                FindPath();
            }
        }

        private void EuclideanButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ManhattanBtn != null && ChebyshevBtn != null)
            {
                ManhattanBtn.IsChecked = false;
                ChebyshevBtn.IsChecked = false;
                _heuristicType = HeuristicType.Euclidean;
            }
        }

        private void ManhattanBtn_OnChecked(object sender, RoutedEventArgs e)
        {
            if (EuclideanBtn != null && ChebyshevBtn != null)
            {
                EuclideanBtn.IsChecked = false;
                ChebyshevBtn.IsChecked = false;
                _heuristicType = HeuristicType.Manhattan;
            }
        }

        private void Chebyshev_OnChecked(object sender, RoutedEventArgs e)
        {
            if (EuclideanBtn != null && ManhattanBtn != null)
            {
                EuclideanBtn.IsChecked = false;
                ManhattanBtn.IsChecked = false;
                _heuristicType = HeuristicType.Chebyshev;
            }
        }

        private void HistorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mazeDrawer.DrawHistory((int)HistorySlider.Value);
        }
    }
}
