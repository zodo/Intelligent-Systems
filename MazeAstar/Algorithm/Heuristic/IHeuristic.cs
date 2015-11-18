namespace MazeAstar.Algorithm.Distance
{
    /// <summary>
    /// Интерфейс эвристики.
    /// </summary>
    public interface IHeuristic
    {
        /// <summary>
        /// Расчитать расстояние между точками.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>Расстояние.</returns>
        double Distance(Cell c1, Cell c2);
    }
}
