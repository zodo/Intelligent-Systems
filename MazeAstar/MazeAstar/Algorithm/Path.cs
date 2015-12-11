namespace MazeAstar.Algorithm
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Найденный путь.
    /// </summary>
    public class Path : IEnumerable<Point>
    {
        /// <summary>
        /// Список точек пути.
        /// </summary>
        private readonly List<Point> _points = new List<Point>();

        /// <summary>
        /// Конструктор <see cref="Path"/>
        /// </summary>
        /// <param name="end">Конечная точка пути.</param>
        public Path(Cell end)
        {
            var current = end;
            while (current.Type != CellType.Start)
            {
                _points.Add(current.Point);
                current = current.Previous;
                if (_points.Contains(current.Point))
                {
                    break;
                }
            }
            _points.Add(current.Point);
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _points.GetEnumerator();
        }
    }
}