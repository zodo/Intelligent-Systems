namespace MazeAstar
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Media;

    using Color = System.Windows.Media.Color;

    public class Direction
    {
        public Point Destionation { get; set; }
        public Point Restriction { get; set; }

        public Point Restriction2 { get; set; }

    }

    /// <summary>
    /// Клетка поля.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Тип клетки.
        /// </summary>
        public CellType Type { get; set; }

        /// <summary>
        /// Лабиринт.
        /// </summary>
        public Maze Maze { get; set; }

        /// <summary>
        /// Координаты.
        /// </summary>
        public Point Point => Maze[this];

        /// <summary>
        /// Соседи.
        /// </summary>
        public IReadOnlyCollection<Cell> Neighbours
        {
            get
            {
                var result = new List<Cell>();
                foreach (var direction in _directions)
                {
                    var point = Maze[this];
                    point.Offset(direction.X, direction.Y);
                    if (Maze.PointFitsMaze(point))
                    {
                        result.Add(Maze[point]);
                    }
                }
                foreach (var diagDir in _diagonals)
                {
                    var point = Maze[this];
                    point.Offset(diagDir.Destionation.X, diagDir.Destionation.Y);
                    if (Maze.PointFitsMaze(point))
                    {
                        var checkPoint = Maze[this];
                        checkPoint.Offset(diagDir.Restriction);
                        var checkPoint2 = Maze[this];
                        checkPoint2.Offset(diagDir.Restriction2);
                        if (Maze[checkPoint].Type != CellType.Wall && Maze[checkPoint2].Type != CellType.Wall)
                        {
                            result.Add(Maze[point]);
                        }
                    }
                }
                return result.Where(x => x.Type == CellType.Empty || x.Type == CellType.Finish || x.Type == CellType.Start).ToList();
            }
        }

        /// <summary>
        /// Возможные пути
        /// </summary>
        private readonly Point[] _directions = { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };


        private readonly Direction[] _diagonals =
        {
            new Direction {Destionation = new Point(1,1), Restriction = new Point(1,0), Restriction2 = new Point(0,1)},
            new Direction {Destionation = new Point(-1,-1), Restriction = new Point(-1,0), Restriction2 = new Point(0,1)},
            new Direction {Destionation = new Point(1,-1), Restriction = new Point(0,-1), Restriction2 = new Point(1,0)},
            new Direction {Destionation = new Point(-1, 1), Restriction = new Point(-1,0), Restriction2 = new Point(0,1)}   
        };
        

        public Cell(Maze maze, CellType type)
        {
            Maze = maze;
            Type = type;
        }


        /// <summary>
        /// Цвет.
        /// </summary>
        public Color Color
        {
            get
            {
                switch (Type)
                {
                    case CellType.Empty: return Colors.White;
                    case CellType.Wall: return Colors.DarkSlateGray;
                    case CellType.Start: return Colors.GreenYellow;
                    case CellType.Finish: return Colors.DodgerBlue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Расстояние от старта.
        /// </summary>
        public double DistFromStart { get; set; }

        /// <summary>
        /// Оценочное расстояние до финиша.
        /// </summary>
        public double DistToEnd { get; set; }

        /// <summary>
        /// Оценочное полное расстояние пути.
        /// </summary>
        public double FullDistance => DistFromStart + DistToEnd;

        /// <summary>
        /// Предыдущая клетка в пути.
        /// </summary>
        public Cell Previous { get; set; }

        
    }

    /// <summary>
    /// Тип клетки.
    /// </summary>
    public enum CellType
    {
        /// <summary>
        /// Пустая клетка.
        /// </summary>
        Empty,

        /// <summary>
        /// Стена.
        /// </summary>
        Wall,

        /// <summary>
        /// Старт.
        /// </summary>
        Start,

        /// <summary>
        /// Финиш.
        /// </summary>
        Finish
    }
}
