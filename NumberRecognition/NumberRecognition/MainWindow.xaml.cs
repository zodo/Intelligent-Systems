using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumberRecognition
{
    using System.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageOperations _imgOperations = new ImageOperations();

        private readonly Manager _manager = new Manager();

        private Point? _prevPos;

        private WriteableBitmap _bitmap;

        private Bounds _bounds;

        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
            ProgressBorder.Visibility = Visibility.Hidden;
        }

        private void Clear()
        {
            _bitmap = BitmapFactory.New(220, 280);
            _bitmap.Clear(Colors.White);
            NumberImg.Source = _bitmap;
            _bounds = new Bounds();
            _prevPos = null;
        }
        private async Task RecognizeNumber()
        {
            var bmp = _imgOperations.CropResize(_bitmap, _bounds);
            var bytes = _imgOperations.BitmapToVector(bmp);
            var result = await _manager.Recognize(bytes);
            ResultsTxt.Text = string.Join(Environment.NewLine,
                result
                .Select((x, i) => new { x, i })
                .OrderByDescending(x => x.x)
                .Where(x => x.x > 0.01)
                .Select(x => $"{x.i} - {x.x.ToString("F")}"));
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            Clear();
        }
        
        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private async void Recognize_OnClick(object sender, RoutedEventArgs e)
        {
            await RecognizeNumber();
        }

        private void NumberImg_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _prevPos = null;
        }

        private void NumberImg_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                Clear();
                return;
            }
            _prevPos = e.GetPosition(NumberImg);
        }

        private async void NumberImg_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(NumberImg);
                _bitmap.FillEllipseCentered((int)pos.X, (int)pos.Y, 4, 4, Colors.Black);
                _bounds.Left = (int)(pos.X < _bounds.Left ? pos.X : _bounds.Left);
                _bounds.Right = (int)(pos.X > _bounds.Right ? pos.X : _bounds.Right);
                _bounds.Top = (int)(pos.Y < _bounds.Top ? pos.Y : _bounds.Top);
                _bounds.Bottom = (int)(pos.Y > _bounds.Bottom ? pos.Y : _bounds.Bottom);
                _bounds.WasModified = true;
                if (_prevPos != null)
                {
                    _imgOperations.DrawEllipses(_bitmap, (int)pos.X, (int)_prevPos.Value.X,(int)pos.Y, (int)_prevPos.Value.Y);
                }
                _prevPos = pos;

                if (RecognizeOnTheFly.IsEnabled && RecognizeOnTheFly.IsChecked.HasValue && RecognizeOnTheFly.IsChecked.Value)
                {
                    await RecognizeNumber();
                }
            }
        }
        
        private void SaveNumberBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_bounds.WasModified)
            {
                var bmp = _imgOperations.CropResize(_bitmap, _bounds);
                var num = Convert.ToInt32(((ComboBoxItem)NumbersCmb.SelectedValue).Content);
                _imgOperations.SaveImage(bmp, num);
                ClearBtn_OnClick(null, null);
            }
        }

        private async void LearnBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            ProgressBorder.Visibility = Visibility.Visible;
            var savedTime = DateTime.Now;
            _cts= new CancellationTokenSource();

            try
            {
                var epochs =
                    await _manager.Learn(new Progress<int>(x => ProgressBar.Value = x), _cts.Token).ConfigureAwait(true);
                ResultsTxt.Text =
                    $"Обучение окончено.\nКол-во эпох: {epochs}\nВремя: {(DateTime.Now - savedTime).TotalSeconds.ToString("F1")} с.";
            }
            catch (OperationCanceledException)
            {
                ResultsTxt.Text = "Обучение прервано";
            }
            ProgressBorder.Visibility = Visibility.Hidden;
            Recognize.IsEnabled = true;
            RecognizeOnTheFly.IsEnabled = true;

        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }
    }
}
