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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap _bitmap;

        private Bounds _bounds;

        private readonly ImageOperations _imgOperations = new ImageOperations();

        private readonly Manager _manager = new Manager();

        private Point? _prevPos = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            _bitmap = BitmapFactory.New(220, 280);
            _bitmap.Clear(Colors.White);
            NumberImg.Source = _bitmap;
            _bounds = new Bounds();
            
        }

        
        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _bitmap = BitmapFactory.New(220, 280);
            _bitmap.Clear(Colors.White);
            NumberImg.Source = _bitmap;
            _bounds = new Bounds();
            _prevPos = null;
        }

        private void NumberImg_OnMouseMove(object sender, MouseEventArgs e)
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
                    DrawEllipses((int)pos.X, (int)_prevPos.Value.X,(int)pos.Y, (int)_prevPos.Value.Y);
                }
                _prevPos = pos;

            }
        }

        private void DrawEllipses(int x0, int x1, int y0, int y1)
        {
            if (x0 > x1)
            {
                var t = x0;
                x0 = x1;
                x1 = t;
            }
            if (y0 > y1)
            {
                var t = y0;
                y0 = y1;
                y1 = t;
            }
            int deltax = Math.Abs(x1 - x0);
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int deltaerr = deltay;
            int y = y0;
            for (int x = x0; x < x1; x++)
            {
                _bitmap.FillEllipseCentered(x, y, 4, 4, Colors.Black);

                error = error + deltaerr;
                if (2 * error >= deltax)
                {
                    y = y - 1;
                    error = error - deltax;
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
            LearnBtn.IsEnabled = false;
            Window.IsEnabled = false;
            RecognizeInProccessLbl.Visibility = Visibility.Visible;
            await _manager.Learn();
            Window.IsEnabled = true;
            RecognizeInProccessLbl.Visibility = Visibility.Hidden;
            Recognize.IsEnabled = true;
                   
        }

        private void Recognize_OnClick(object sender, RoutedEventArgs e)
        {
            var bmp = _imgOperations.CropResize(_bitmap, _bounds);
            var bytes = _imgOperations.GetImageBytes(bmp);
            ResultsTxt.Text = string.Join(Environment.NewLine, 
                _manager.Recognize(bytes)
                .Select((x, i) => new {x, i})
                .OrderByDescending(x => x.x)
                .Select(x => $"{x.i} - {x.x}"));
        }

        private void NumberImg_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _prevPos = null;
        }

        private void NumberImg_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _prevPos = e.GetPosition(NumberImg);
        }
    }
}
