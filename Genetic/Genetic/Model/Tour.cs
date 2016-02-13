using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    public class Tour
    {
        public List<City> Cities { get; }

        private double _fitness;
        private int _distance;

        public Tour()
        {
            Cities = new List<City>();
            _fitness = 0;
            _distance = 0;

            InitializeTour();
        }

        public Tour(List<City> tour)
        {
            Cities = tour;
        }

        private void InitializeTour()
        {
            for (var i = 0; i < TourManager.Instance.GetNumberOfCities(); i += 1)
            {
                Cities.Add(null);
            }
        }

        public void GenerateIndividual()
        {
            for (var i = 0; i < TourManager.Instance.GetNumberOfCities(); i += 1)
            {
                SetCity(i, TourManager.Instance.GetCity(i));
            }
        }

        public City GetCity(int tourPosition)
        {
            return Cities[tourPosition];
        }

        public void SetCity(int cityIndex, City city)
        {
            Cities[cityIndex] = city;

            _fitness = 0;
            _distance = 0;
        }

        public double GetFitness()
        {
            if (_fitness == 0)
            {
                _fitness = 1 / (double)GetDistance();
            }

            return _fitness;
        }

        public int GetTourSize()
        {
            return Cities.Count;
        }

        public int  GetDistance()
        {
            if (_distance == 0)
            {
                var tourDistance = 0.0;

                for (var i = 0; i < Cities.Count; i += 1)
                {
                    var fromCity = Cities[i];
                    City destinationCity;

                    if ((i + 1) < Cities.Count)
                    {
                        destinationCity = Cities[i + 1];
                    }
                    else
                    {
                        destinationCity = Cities[0];
                    }

                    tourDistance += fromCity.CalculateDistance(destinationCity);
                }
                _distance = (int)tourDistance;
            }

            return _distance;
        }

        public bool ContainsCity(City city)
        {
            return Cities.Contains(city);
        }

        public override string ToString()
        {
            var geneString = "|";

            for (var i = 0; i < Cities.Count; i += 1)
            {
                geneString += Cities[i] + "|";
            }

            return geneString;
        }
    }
}
