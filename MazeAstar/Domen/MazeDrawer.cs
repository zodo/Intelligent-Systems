namespace MazeAstar
{
    using System.Drawing;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Domen;

    /// <summary>
    /// Рисователь лабиринта.
    /// </summary>
    public class MazeDrawer
    {
        private static MazeDrawer _instance;

        public static MazeDrawer GetInstance => _instance ?? new MazeDrawer();

        /// <summary>
        /// Лабиринт.
        /// </summary>
        public Maze Maze { get; set; }

        /// <summary>
        /// Холст.
        /// </summary>
        public WriteableBitmap Bitmap { get; set; }

        /// <summary>
        /// Ширина в пикселях.
        /// </summary>
        public int Width => (int)(Bitmap?.Width ?? 0);

        /// <summary>
        /// Высота в пикселях.
        /// </summary>
        public int Height => (int)(Bitmap?.Height ?? 0);

        /// <summary>
        /// Размер клетки.
        /// </summary>
        public int CellSize
        {
            get
            {
                return _cellSize;
            }
            set
            {
                _cellSize = value == 0?1:value;
            }
        }

        private int _cellSize;

        /// <summary>
        /// Перерисовать холст.
        /// </summary>
        public void Redraw()
        {
            if (Bitmap == null)
            {
                return;
            }
            Bitmap.Clear();

            var widthInCells = Width / CellSize;
            var heightInCells = Height / CellSize;

            
            for (int x = 0; x < widthInCells; x++)
            {
                for (int y = 0; y < heightInCells; y++)
                {
                    var cell = Maze[x, y];
                    Bitmap.FillRectangle(
                            x * CellSize,
                            y * CellSize,
                            (x + 1) * CellSize,
                            (y + 1) * CellSize,
                            cell.Color);
                    
                }
            }

            for (int x = 1; x < widthInCells; x++)
            {
                Bitmap.DrawLineAa(x * CellSize, 0, x * CellSize, heightInCells * CellSize, Colors.LightGray);
            }
            for (int y = 1; y < heightInCells; y++)
            {
                Bitmap.DrawLineAa(0, y * CellSize, widthInCells * CellSize, y * CellSize, Colors.LightGray);
            }

            var path = Maze.Path?.ToList();
            if (path != null)
            {
                var pointarr = path.SelectMany(x => new int[] { (int)(x.X * CellSize + 0.5 * CellSize), (int)(x.Y * CellSize + 0.5 * CellSize) }).ToArray();
                for (double tension = 0.5; tension < 1; tension+=0.05)
                {
                    Bitmap.DrawCurve(pointarr, (float)tension, Colors.Red);
                }
            }
            if (Maze.History == null)
            {
                return;
            }
        }

        public void DrawHistory(int iterNumber)
        {
            if (Maze.History == null)
            {
                return;
            }
            Redraw();
            var cellTypes = Maze.History[iterNumber - 1].CellTypes;
            var size = Maze.History[iterNumber - 1].Size;
            for (var x = 0; x < size.Width; x++)
            {
                for (var y = 0; y < size.Height; y++)
                {
                    if (cellTypes[x, y].HasFlag(HistoryCellType.ClosedContains))
                    {
                        Bitmap.FillEllipseCentered(
                            (int)(x * CellSize + CellSize * 0.5),
                            (int)(y * CellSize + CellSize * 0.5),
                            (int)(CellSize * 0.1),
                            (int)(CellSize * 0.1), Colors.Black);
                    }
                    if (cellTypes[x, y].HasFlag(HistoryCellType.OpenedContains))
                    {
                        Bitmap.DrawEllipseCentered(
                            (int)(x * CellSize + CellSize * 0.5),
                            (int)(y * CellSize + CellSize * 0.5),
                            (int)(CellSize * 0.1),
                            (int)(CellSize * 0.1), Colors.Black);
                    }
                    if (cellTypes[x, y].HasFlag(HistoryCellType.IsOptimal))
                    {
                        Bitmap.DrawLineAa(
                            (int)(x * CellSize + CellSize * 0.3),
                            (int)(y * CellSize + CellSize * 0.3),
                            (int)(x * CellSize + CellSize * 0.7),
                            (int)(y * CellSize + CellSize * 0.7), 
                            Colors.Black);
                        Bitmap.DrawLineAa(
                            (int)(x * CellSize + CellSize * 0.3),
                            (int)(y * CellSize + CellSize * 0.7),
                            (int)(x * CellSize + CellSize * 0.7),
                            (int)(y * CellSize + CellSize * 0.3),
                            Colors.Black);
                    }
                }
            }
        }

        /// <summary>
                /// Переинициализировать лабиринт.
                /// </summary>
            public
            void ReinitMaze()
        {
            var cellsW = Width / CellSize;
            var cellsH = Height / CellSize;

            Maze = new Maze(Maze, new Size(cellsW, cellsH));
        }

        /// <summary>
        /// Обработать клик.
        /// </summary>
        /// <param name="point">Местоположение клика.</param>
        /// <param name="type">Тип клетки.</param>
        public void HandleClick(System.Windows.Point point,  CellType type)
        {
            var cell = GetByScreenCoords((int)point.X, (int)point.Y);

            cell.Type = type;
        }

        /// <summary>
        /// Получить клетку по экранным координатам.
        /// </summary>
        private Cell GetByScreenCoords(int x, int y) => Maze[x / CellSize, y / CellSize];
    }

   
}
