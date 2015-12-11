namespace MazeAstar.Algorithm
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Distance;

    using Domen;

    /// <summary>
    /// Калькулятор оптимального пути.
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// Лабиринт.
        /// </summary>
        private readonly Maze _maze;

        public PathFinder(Maze maze)
        {
            _maze = maze;
        }

        /// <summary>
        /// Получить путь.
        /// </summary>
        /// <param name="start">Стартовая клетка.</param>
        /// <param name="end">Конечная клетка.</param>
        /// <param name="heuristic">Используемая эвристика.</param>
        /// <param name="history">Сообщения.</param>
        /// <returns></returns>
        public Path GetPath(Cell start, Cell end, IHeuristic heuristic, ref History history)
        {
            var closed = new List<Cell>();
            var opened = new List<Cell> { start };
            var iterations = 1;
            start.DistToEnd = heuristic.Distance(start, end);

            while (opened.Any())
            {
                var optimal = opened.OrderBy(c => c.FullDistance).First();
                if (opened.Contains(end))
                {
                    optimal = end;
                }
                Debug.WriteLine($"ITER: {iterations}, optdist: {optimal.FullDistance}");

                if (optimal == end)
                {
                    history.PathLength = end.FullDistance;
                    return new Path(end);
                }
                opened.Remove(optimal);
                closed.Add(optimal);
                foreach (var neighbour in optimal.Neighbours.Where(neighbour => !closed.Contains(neighbour)))
                {
                    if (!opened.Contains(neighbour))
                    {
                        opened.Add(neighbour);
                        neighbour.Previous = optimal;
                        neighbour.DistFromStart = optimal.DistFromStart + heuristic.Distance(optimal, neighbour);
                        neighbour.DistToEnd = heuristic.Distance(neighbour, end);
                    }
                    else if (optimal.DistFromStart + heuristic.Distance(optimal, neighbour) < neighbour.DistFromStart)
                        {
                            neighbour.Previous = optimal;
                            neighbour.DistFromStart = optimal.DistFromStart + heuristic.Distance(optimal, neighbour);
                            neighbour.DistToEnd = heuristic.Distance(neighbour, end);
                        }
                    
                }

                var size = _maze.Size;
                var state = new State(size) { IterationNumber = ++iterations };
                for (var x = 0; x < size.Width; x++)
                {
                    for (var y = 0; y < size.Height; y++)
                    {
                        var cell = _maze[x, y];
                        if (opened.Contains(cell))
                        {
                            state.CellTypes[x, y] |= HistoryCellType.OpenedContains;
                        }
                        if (closed.Contains(cell))
                        {
                            state.CellTypes[x, y] |= HistoryCellType.ClosedContains;
                        }
                        if (cell == optimal)
                        {
                            state.CellTypes[x, y] |= HistoryCellType.IsOptimal;
                        }
                    }
                }
                history.Add(state);
            }
            return null;
        }



    }
}
