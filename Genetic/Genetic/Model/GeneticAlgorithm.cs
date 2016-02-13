using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    public class GeneticAlgorithm
    {
        public double MutationRate { get; }

        public int TournamentSize { get; }

        public bool Elitism { get; }

        private static Random rand = new Random();

        public GeneticAlgorithm(double mutationRate = 0.015, int tournamentSize = 5, bool elitism = true)
        {
            MutationRate = mutationRate;
            TournamentSize = tournamentSize;
            this.Elitism = elitism;
        }

        public async Task<Population> EvolvePopulationAsync(Population population)
        {
            return await Task.Run(() => EvolvePopulation(population));
        } 

        public  Population EvolvePopulation(Population population)
        {
            var newPopulation = new Population(population.Tours.Count, false);
            var elittismOffset = 0;

            if (Elitism)
            {
                newPopulation.Tours[0] = population.GetFittest();
                elittismOffset = 1;
            }

            for (var i = elittismOffset; i < newPopulation.Tours.Count; i ++)
            {
                var firstParent = TournamentSelection(population);
                var secondParent = TournamentSelection(population);

                AddToParents(newPopulation, firstParent);
                AddToParents(newPopulation, secondParent);

                var child = Crossover(firstParent, secondParent);
                newPopulation.Tours[i] = child;
            }

            for (var i = elittismOffset; i < newPopulation.Tours.Count; i ++)
            {
                Mutate(newPopulation.Tours[i]);
            }

            return newPopulation;
        }

        private void Mutate(Tour tour)
        {
            for (int firstPosition = 0, tourSize = tour.GetTourSize(); firstPosition < tourSize; firstPosition ++)
            {
                if (rand.NextDouble() < MutationRate)
                {
                    var secondPosition = rand.Next(tourSize);

                    var firstCity = tour.GetCity(firstPosition);
                    var secondCity = tour.GetCity(secondPosition);

                    tour.SetCity(secondPosition, firstCity);
                    tour.SetCity(firstPosition, secondCity);
                }
            }
        }

        private Tour Crossover(Tour firstParent, Tour secondParent)
        {
            var child = new Tour();
            var startPosition = rand.Next(firstParent.GetTourSize());
            var endPosition = rand.Next(firstParent.GetTourSize());

            for (int i = 0, tourSize = child.GetTourSize(); i < tourSize; i ++)
            {
                if (startPosition < endPosition && i > startPosition && i < endPosition)
                {
                    child.SetCity(i, firstParent.GetCity(i));
                }
                else if (startPosition > endPosition)
                {
                    if (!(i < startPosition && i > endPosition))
                    {
                        child.SetCity(i, firstParent.GetCity(i));
                    }
                }
            }

            for (int i = 0, tourSize = secondParent.GetTourSize(); i < tourSize; i ++)
            {
                if (!child.ContainsCity(secondParent.GetCity(i)))
                {
                    for (int j = 0, childTourSize = child.GetTourSize(); j < childTourSize; j ++)
                    {
                        if (child.GetCity(j) == null)
                        {
                            child.SetCity(j, secondParent.GetCity(i));
                            break;
                        }
                    }
                }
            }

            return child;
        }

        private Tour TournamentSelection(Population population)
        {
            return population.Tours.OrderBy(x => rand.Next())
                .Take(TournamentSize)
                .OrderByDescending(x => x.GetFitness())
                .FirstOrDefault();
        }

        private void AddToParents(Population population, Tour parent)
        {
            if (!population.Parents.Contains(parent))
            {
                population.Parents.Add(parent);
            }
        }
    }
}
