using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Genetic.Test
{
    using System.Collections.Generic;

    [TestClass]
    public class GeneticTest
    {
        [TestMethod]
        public void CityDistance_ReturnsCorrect()
        {
            // Arrange
            var city1 = new City(0, 0);
            var city2 = new City(0, 42);

            // Act
            var distance = city1.CalculateDistance(city2);

            // Assert
            Assert.AreEqual(distance, 42);
        }

        [TestMethod]
        public void TourManager_SaveCity_CitySaved()
        {
            // Arrange
            var tourManager = new TourManager();
            var city = new City(42, 42);
            
            // Act
            tourManager.AddCity(city);
            var retrivedCity = tourManager.GetCity(0);

            // Assert
            Assert.AreEqual(city, retrivedCity);
        }

        [TestMethod]
        public void Tour_GetDistance_ReturnDistance()
        {
            // Arrange
            var cities = new List<City> {new City(0,10), new City(10,10), new City(10,20)};
            var tour = new Tour(cities);

            // Act
            var distance = tour.GetDistance();

            // Assert
            Assert.AreEqual(distance, 34);
        }

        [TestMethod]
        public void Tour_GetFitness_ReturnFitness()
        {
            // Arrange
            var cities = new List<City> { new City(0, 10), new City(10, 10), new City(10, 20) };
            var tour = new Tour(cities);

            // Act
            var fitness = tour.GetFitness();

            // Assert
            Assert.IsTrue(Math.Abs(fitness - 0.0294117) < 0.0001);
        }
    }
}