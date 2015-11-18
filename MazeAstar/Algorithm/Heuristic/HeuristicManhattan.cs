namespace MazeAstar.Algorithm.Distance
{
    using System;

    class HeuristicManhattan : IHeuristic
    {
        public double Distance(Cell c1, Cell c2)
        {
            return Math.Abs(c1.Point.X - c2.Point.X) + Math.Abs(c1.Point.Y - c2.Point.Y);
        }
    }
}
