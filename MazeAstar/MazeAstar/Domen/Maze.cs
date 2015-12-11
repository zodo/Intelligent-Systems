namespace MazeAstar
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Algorithm;
    using Algorithm.Distance;

    using Domen;

    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    /// <summary>
    /// Лабиринт.
    /// </summary>
    public class Maze
    {
        /// <summary>
        /// Размер.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Поле с точками.
        /// </summary>
        private Dictionary<Point, Cell> _field = new Dictionary<Point, Cell>(); 

        /// <summary>
        /// История состояний.
        /// </summary>
        public History History { get; set; }

        /// <summary>
        /// Путь в решенном лабиринте.
        /// </summary>
        public Path Path { get; set; }

        public Maze(Size size)
        {
            Size = size;
            for (var x = 0; x < Size.Width; x++)
            {
                for (var y = 0; y < Size.Height; y++)
                {
                    _field.Add(new Point(x, y), new Cell(this, CellType.Empty));
                }
            }
        }

        public Maze(Maze maze, Size size):this(size)
        {
            if (maze == null)
            {
                return;
            }
            foreach (var pair  in maze._field.Where(cell => cell.Key.X < Size.Width && cell.Key.Y < Size.Height))
            {
                _field[pair.Key].Type = pair.Value.Type;
            }
            History = maze.History;
            Size = size;
            Path = maze.Path;
        }

        /// <summary>
        /// Получение точки по координатам.
        /// </summary>
        /// <param name="x">Координата x.</param>
        /// <param name="y">Координата y.</param>
        /// <returns>Клетка поля.</returns>
        public Cell this[int x, int y]
        {
            get
            {
                var point = new Point(x, y);
                return this[point];
            }
            set
            {
                var point = new Point(x, y);
                this[point] = value;
            }
        }

        /// <summary>
        /// Получение точки по координатам.
        /// </summary>
        /// <param name="point">Координаты точки.</param>
        /// <returns>Клетка поля.</returns>
        public Cell this[Point point]
        {
            get
            {
                if (!PointFitsMaze(point))
                {
                    return new Cell(this, CellType.Empty);
                }

                return _field[point];
            }
            set
            {
                if (PointFitsMaze(point))
                {
                    _field[point] = value;
                }
                
            }
        }

        /// <summary>
        /// Получить координаты клетки.
        /// </summary>
        /// <param name="cell">Клетка.</param>
        /// <returns>Координаты клетки.</returns>
        public Point this[Cell cell]
        {
            get
            {
                return _field.FirstOrDefault(x => x.Value == cell).Key;
            }
        }

        /// <summary>
        /// Подходит ли точка лабиринту по координатам.
        /// </summary>
        /// <param name="point">Точка.</param>
        public bool PointFitsMaze(Point point)
        {
            return point.X >= 0 && point.Y >= 0 && point.X < Size.Width && point.Y < Size.Height;
        }

        /// <summary>
        /// Найти путь в лабиринте.
        /// </summary>
        /// <param name="heuristicType">Тип эвристики.</param>
        public void FindPath(HeuristicType heuristicType)
        {
            ClearPath();

            IHeuristic heuristic = HeuristicFactory.Get(heuristicType);

            var finder = new PathFinder(this);



            var start = _field.Values.FirstOrDefault(x => x.Type == CellType.Start);
            var finish = _field.Values.FirstOrDefault(x => x.Type == CellType.Finish);

            if (start != null && finish != null)
            {
                var mes = new History();
                Path = finder.GetPath(start, finish, heuristic, ref mes);
                History = mes;
                if (Path == null)
                {
                    MessageBox.Show("Path haven`t found");
                }
            }
        }

        private void ClearPath()
        {
            Path = null;
        }

        public void ClearField()
        {
            _field = new Dictionary<Point, Cell>();
            for (var x = 0; x < Size.Width; x++)
            {
                for (var y = 0; y < Size.Height; y++)
                {
                    _field.Add(new Point(x, y), new Cell(this, CellType.Empty));
                }
            }
            Path = null;
        }
    }
}
