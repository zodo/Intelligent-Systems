namespace MazeAstar.Algorithm.Distance
{
    using System;

    /// <summary>
    /// Фабрика эвристик.
    /// </summary>
    public static class HeuristicFactory
    {
        /// <summary>
        /// Получить эвристику.
        /// </summary>
        /// <param name="type">Тип эвристики.</param>
        /// <returns>Эвристика.</returns>
        public static IHeuristic Get(HeuristicType type)
        {
            switch (type)
            {
                case HeuristicType.Chebyshev: return new HeuristicChebyshev();
                case HeuristicType.Euclidean: return new HeuristicEuclidean();
                case HeuristicType.Manhattan: return new HeuristicManhattan();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
