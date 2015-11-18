namespace MazeAstar.Domen
{
    using System;

    /// <summary>
    /// Тип поля в истории.
    /// </summary>
    [Flags]
    public enum HistoryCellType
    {
        /// <summary>
        /// Содержится в открытом наборе.
        /// </summary>
        OpenedContains = 1,


        /// <summary>
        /// Содержится в закрытом наборе.
        /// </summary>
        ClosedContains = 2,

        IsOptimal = 4
    }
}
