using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    public class Population
    {
        public List<Tour> Tours { get; private set; }

        public List<Tour> Parents { get; set; } 

        public Population(int populationSize, bool initialize)
        {
            InitializeTour(populationSize);

            if (initialize)
            {
                InitializePopulation(populationSize);
            }
        }

        private void InitializeTour(int populationSize)
        {
            Tours = new List<Tour>();
            Parents = new List<Tour>();
            for (var i = 0; i < populationSize; i += 1)
            {
                Tours.Add(new Tour());
            }
        }

        private void InitializePopulation(int populationSize)
        {
            for (var i = 0; i < populationSize; i += 1)
            {
                var newTour = new Tour();
                newTour.GenerateIndividual();
                Tours[i] = newTour;
            }
        }

        public Tour GetFittest()
        {
            return Tours.OrderByDescending(x => x.GetFitness()).FirstOrDefault();
        }

        public double GetAverageFitness()
        {
            return Tours.Average(x => x.GetFitness());
        }
    }
}
