namespace MazeAstar.Algorithm.Distance
{
    /// <summary>
    /// Тип эвристики.
    /// </summary>
    public enum HeuristicType
    {
        /// <summary>
        /// Расстояние чебышева.
        /// </summary>
        Chebyshev,

        /// <summary>
        /// Эвклидово расстояния.
        /// </summary>
        Euclidean,

        /// <summary>
        /// Манхэттенское расстояние.
        /// </summary>
        Manhattan
    }
}
