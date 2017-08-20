using System;
using System.Collections.Generic;

namespace MazeSolver
{
    /// <summary>
    /// Interface for a walled maze.
    /// </summary>
    public interface IWalledMaze
    {
        /// <summary>
        /// Gets the end points of the maze
        /// </summary>
        ICollection<IWalledMazeNode> EndNodes { get; }

        /// <summary>
        /// Gets the height of the maze
        /// </summary>
        int Height { get; }        

        /// <summary>
        /// Gets the start point of the maze
        /// </summary>
        IWalledMazeNode StartNode { get; }

        /// <summary>
        /// Gets the width of the maze
        /// </summary>
        int Width { get; }              

        /// <summary>
        /// Gets all the adjacent nodes for a given node. Used by the solver to find neighboring nodes during scan.
        /// </summary>
        /// <param name="currentNode">currentNode</param>
        /// <returns></returns>
        IEnumerable<IWalledMazeNode> GetAdjacentNodes(IWalledMazeNode currentNode); 

        /// <summary>
        /// Gets a specific node given the position.
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="column">column</param>
        /// <returns></returns>
        IWalledMazeNode GetNode(int row, int column); 

        /// <summary>
        /// Gets all nodes for the maze.
        /// </summary>
        /// <returns></returns>
        IEnumerator<IWalledMazeNode> GetNodes();

        /// <summary>
        /// Used by a <see cref="IMazeSolver"/> to determine if it has arrived at a destination.
        /// </summary>
        /// <param name="curNode"></param>
        /// <returns></returns>
        bool IsEndNode(IWalledMazeNode currentNode);

        /// <summary>
        /// Used to solve the maze.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="solvedResultCallback"></param>
        void Solve(IMazeSolver solver, Action<IEnumerable<IWalledMazeNode>, bool> solvedResultCallback);
    }
}
