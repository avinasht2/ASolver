using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver.BreadthFirstSolver
{
    /// <summary>
    /// BFS node representation.
    /// </summary>
    public class BreadthFirstSolverNode
    {
        /// <summary>
        /// column position of the node.
        /// </summary>
        public int ColumnPosition { get; set; }

        /// <summary>
        /// Distance from the source node.
        /// </summary>
        public int Distance
        {
            get;
            set;
        }

        /// <summary>
        /// Pointer to the previous node.
        /// </summary>
        public BreadthFirstSolverNode Predecessor
        {
            get;
            set;
        }

        /// <summary>
        /// row position of the node.
        /// </summary>
        public int RowPosition
        {
            get;
            set;
        }

        /// <summary>
        /// State of the BreadthFirstSolvernodestate.
        /// </summary>
        public BreadthFirstSolverNodeState State
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public BreadthFirstSolverNode(int row, int column)
        {
            RowPosition = row;
            ColumnPosition = column;
        }
    }
}
