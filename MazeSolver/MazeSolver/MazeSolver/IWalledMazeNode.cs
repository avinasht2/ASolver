using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    /// <summary>
    /// Interface for walled maze node.
    /// </summary>
    public interface IWalledMazeNode
    {
        /// <summary>
        /// Gets the column position
        /// </summary>
        int ColumnPosition { get; }

        /// <summary>
        /// Gets the row position
        /// </summary>
        int RowPosition { get; }

        /// <summary>
        /// Gets or sets the state information <see cref="WalledMazeNodeState"/>.
        /// </summary>
        WalledMazeNodeState State { get; set; } //
    }
}
