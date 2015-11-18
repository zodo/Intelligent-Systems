namespace MazeAstar.Algorithm.Distance
{
    using System;

    class HeuristicEuclidean : IHeuristic
    {
        public double Distance(Cell c1, Cell c2)
        {
            return Math.Sqrt(Math.Pow(c1.Point.X - c2.Point.X, 2) + Math.Pow(c1.Point.Y - c2.Point.Y, 2));
        }
    }
}
