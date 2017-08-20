using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{

    /// <summary>
    /// Implementaion of <see cref="IWalledMaze"/>.
    /// Implements all necessary information for maze model
    /// </summary>
    public class WalledMaze : IWalledMaze
    {
        
        private ICollection<IWalledMazeNode> endNodes = null;

        private int height = 0;

        private WalledMazeNode[,] mazeMap = null;
        
        private IWalledMazeNode startNode = null;

        private int width = 0;

        /// <summary>
        /// Gets the end points for the maze
        /// </summary>
        public ICollection<IWalledMazeNode> EndNodes
        {
            get { return endNodes; }
            set { this.endNodes = value; }
        }

        /// <summary>
        /// Gets the total width of the maze
        /// </summary>
        public int Height
        {
            get { return height; }
        }
        
        /// <summary>
        /// Gets the starting points for the maze
        /// </summary>
        public IWalledMazeNode StartNode
        {
            get { return startNode; }
            set { this.startNode = value; }
        }

        /// <summary>
        /// Gets the total width of the maze
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Initializes the walled maze with a collection of nodes.
        /// </summary>
        /// <param name="mazeMap"></param>
        public WalledMaze(WalledMazeNode[,] mazeMap)
        {
            if (mazeMap == null)
            {
                throw new ArgumentNullException();
            }

            this.mazeMap = mazeMap;
            this.height = this.mazeMap.GetLength(0);
            this.width = this.mazeMap.GetLength(1);
        }        

        /// <summary>
        /// Gets adjacents for a node. Any node can have at most 8 adjacents.
        /// </summary>
        public IEnumerable<IWalledMazeNode> GetAdjacentNodes(IWalledMazeNode node)
        {
            int rowPosition = node.RowPosition;
            int columnPosition = node.ColumnPosition;

            for (int i = rowPosition - 1; i <= rowPosition + 1; i++)
            {
                for (int j = columnPosition - 1; j <= columnPosition + 1; j++)
                {
                    if (i < 0 || i >= height || j < 0 || j >= width || (i == rowPosition && j == columnPosition))//eliminates out of bounds from being sent as adjacents.
                        continue;
                    yield return mazeMap[i, j];
                }
            }
        }

        /// <summary>
        /// Gets a node.
        /// </summary>
        public IWalledMazeNode GetNode(int row, int col)
        {
            GuardPosition(row, col, "row\\col");
            return mazeMap[row, col];
        }

        /// <summary>
        /// Gets all maze nodes.
        /// </summary>
        public IEnumerator<IWalledMazeNode> GetNodes()
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    yield return mazeMap[i, j];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="exceptionParamName"></param>
        public void GuardPosition(IWalledMazeNode node, string exceptionParamName)
        {
            int rowPosition = node.RowPosition;
            int colPosition = node.ColumnPosition;

            GuardPosition(rowPosition, colPosition, exceptionParamName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowPosition"></param>
        /// <param name="colPosition"></param>
        /// <param name="exceptionParamName"></param>
        private void GuardPosition(int rowPosition, int colPosition, string exceptionParamName)
        {
            if (rowPosition < 0 || rowPosition >= this.height || colPosition < 0 || colPosition >= this.width)
                throw new ArgumentOutOfRangeException("The supplied node is out of bounds", exceptionParamName);
        }

        /// <summary>
        /// Used by a <see cref="IMazeSolver"/> to determine if it has arrived at a destination.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsEndNode(IWalledMazeNode node)
        {
            if (node == null)
            {
                return false;
            }

            return node.State == WalledMazeNodeState.End;
        }

        /// <summary>
        /// Function that solves the maze using the solver.
        /// </summary>
        public void Solve(IMazeSolver solver, Action<IEnumerable<IWalledMazeNode>, bool> solvedResultCallback)
        {
            if (solver == null)
                throw new ArgumentNullException("Solver cannot be null", "solver");
            if (solvedResultCallback == null)
                throw new ArgumentNullException("Please provide a callback action", "solvedResultCallback");
            //calls solver's solve method.
            solver.Solve(this, (solvedPath) =>
            {
                if (solvedPath == null)
                    solvedResultCallback(new List<IWalledMazeNode>(0), false);//return a empty path if the solver could not solve the maze.
                else
                    solvedResultCallback(solvedPath, true);
            });
        }
    }
}
