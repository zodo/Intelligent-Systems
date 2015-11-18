namespace MazeAstar.Domen
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// История.
    /// </summary>
    public class History : List<State>
    {
        /// <summary>
        /// Длина пути.
        /// </summary>
        public double PathLength { get; set; }


        /// <summary>
        /// Количество итераций.
        /// </summary>
        public double IterationAmount => this.Select(x => x.IterationNumber).Max() - 1;
    }
}
