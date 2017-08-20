using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{ 
    /// <summary>
    /// Implementaion of <see cref="IWalledMazeNode"/>.
    /// 
    /// </summary>
    public sealed class WalledMazeNode : IWalledMazeNode
    {
        int columnPosition = 0;

        int rowPosition = 0;

        WalledMazeNodeState state = WalledMazeNodeState.Open;        

        /// <summary>
        /// Gets the column position of the node
        /// </summary>
        public int ColumnPosition
        {
            get { return columnPosition; }
        }

        /// <summary>
        /// Gets the row position of the node
        /// </summary>
        public int RowPosition
        {
            get { return rowPosition; }
        }

        /// <summary>
        /// Gets the state of the node
        /// </summary>
        public WalledMazeNodeState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public WalledMazeNode(int row, int column)
        {
            rowPosition = row;
            columnPosition = column;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            WalledMazeNode mazeNode = obj as WalledMazeNode;
            if (mazeNode == null)
                return false;

            if (object.ReferenceEquals(mazeNode, this))
                return true;

            return (mazeNode.ColumnPosition == this.ColumnPosition && mazeNode.RowPosition == this.RowPosition && mazeNode.State == this.State);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() + ColumnPosition + RowPosition + (int)State;
        }
    }
}
