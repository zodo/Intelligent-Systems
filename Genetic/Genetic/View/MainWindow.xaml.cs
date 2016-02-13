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

namespace Genetic
{
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateCity(int x, int y)
        {
            var ellipse = new Ellipse { Width = 10, Height = 10, Fill = new SolidColorBrush(Colors.Green) };
            PointsCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, x - 5);
            Canvas.SetTop(ellipse, y - 5);
            TourManager.Instance.AddCity(new City(x, y));
        }

        private void ClearCities()
        {
            TourManager.Instance.Clear();
            PointsCanvas.Children.Clear();
            LogsTxt.Clear();
        }

        private void PointsGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(PointsGrid);
            CreateCity((int)pos.X, (int)pos.Y);
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
        }

        private void GenerateTenMoreBtnClick(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            int width = (int)PointsGrid.ActualWidth;
            int height = (int)PointsGrid.ActualHeight;
            foreach (var coords in Enumerable.Range(0, 10).Select(x => new { X = random.Next(width), Y = random.Next(height) }))
            {
                CreateCity(coords.X, coords.Y);
            }
        }

        private void ClearBtnClick(object sender, RoutedEventArgs e)
        {
            ClearCities();
        }

        private async void CalculateBtnClick(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ClearLines();
            LogsTxt.Clear();

            var geneticAlgo = new GeneticAlgorithm(double.Parse(MutationRateTxt.Text, NumberStyles.Number, CultureInfo.InvariantCulture)/100,
               int.Parse(TournamentSizeTxt.Text), ElitismChck.IsChecked.GetValueOrDefault());

            var population = new Population(int.Parse(PopulationSizeTxt.Text), true);
           
           var genAmount = int.Parse(GenerationsTxt.Text);
            for (var i = 0; i < genAmount; i += 1)
            {
                population = await geneticAlgo.EvolvePopulationAsync(population);
                AddToLogs($"{i} - среднее {population.GetFittest().GetFitness()}");
                //AddToLogs($"Родители этого поколения:");
                //foreach (var parent in population.Parents.Select(x => x.GetFitness()).Distinct())
                //{
                //    AddToLogs($"\t{parent}");
                //}
                ClearLines();
                var cities = population.GetFittest().Cities;
                for (int j = 0; j < cities.Count - 1; j++)
                {
                    CreateWay(cities[j].X, cities[j].Y, cities[j + 1].X, cities[j + 1].Y);
                }
            }

            AddToLogs($"Полный путь: {population.GetFittest().GetDistance()}");
            Mouse.OverrideCursor = null;
        }



        private void AddToLogs(string text)
        {
            LogsTxt.Text = text + Environment.NewLine + LogsTxt.Text;
        }

        private void ClearLines()
        {
            var lines = PointsCanvas.Children.OfType<Line>().ToList();
            foreach (var line in lines)
            {
                PointsCanvas.Children.Remove(line);
            }
        }

        private void CreateWay(int x, int y, int x2, int y2)
        {
            PointsCanvas.Children.Add(
                new Line { X1 = x, X2 = x2, Y1 = y, Y2 = y2, StrokeThickness = 3, Stroke = Brushes.Green });
        }

    }
}
