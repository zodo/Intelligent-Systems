namespace MazeAstar.Domen
{
    using System.Drawing;

    /// <summary>
    /// Состояние на одной из итераций.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Размер поля.
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Поле.
        /// </summary>
        public HistoryCellType[,] CellTypes;

        /// <summary>
        /// Номер итерации.
        /// </summary>
        public int IterationNumber { get; set; }


        public State(Size size)
        {
            Size = size;
            CellTypes = new HistoryCellType[size.Width, size.Height];
        }
    }





}
