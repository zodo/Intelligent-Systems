using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    public class City
    {
        private static Random rand = new Random(0);

        public int X{get;}

        public int Y{get;}

        public City(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double CalculateDistance(City city)
        {
            var xDistance = Math.Abs(X - city.X);
            var yDistance = Math.Abs(Y - city.Y);
            var distance = Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance));

            return distance;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
