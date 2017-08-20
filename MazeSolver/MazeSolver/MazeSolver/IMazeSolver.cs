using System;
using System.Collections.Generic;

namespace MazeSolver
{
    /// <summary>
    /// Interface for maze solver.
    /// </summary>
    public interface IMazeSolver
    {
       void Solve(IWalledMaze maze, Action<IEnumerable<IWalledMazeNode>> solvedResultCallback);
    }
}
