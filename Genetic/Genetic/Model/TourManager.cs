using System.Collections.Generic;

namespace Genetic
{
    public class TourManager
    {
        private static TourManager _instance;

        public static TourManager Instance => _instance ?? (_instance = new TourManager());

        private readonly List<City> _destinationCities = new List<City>();

        public void AddCity(City city)
        {
            _destinationCities.Add(city);
        }

        public City GetCity(int index)
        {
            return _destinationCities[index];
        }

        public int GetNumberOfCities()
        {
            return _destinationCities.Count;
        }

        public void Clear()
        {
            _destinationCities.Clear();
        }
    }
}
