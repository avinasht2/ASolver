using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MazeSolver.BreadthFirstSolver
{
    /// <summary>
    /// Solver for a walled maze puzzle using Breadth First Algorithm.
    /// </summary>
    public class BreadthFirstMazeSolver : IMazeSolver
    {
        /// <summary>
        /// node collection
        /// </summary>
        BreadthFirstSolverNode[,] breadthFirstSolverNodes = null;

        /// <summary>
        /// Implementatin of the <see cref="IMazeSolver"/>'s Solve() method. 
        /// Uses the Breadth Fist Search Algorithm that tracks predecessors and distance from source.
        /// </summary>
        public void Solve(IWalledMaze maze, Action<IEnumerable<IWalledMazeNode>> solvedResultCallback)
        {
            if (maze == null)
            {
                throw new ArgumentNullException("WalledMaze");
            }

            if (solvedResultCallback == null)
            {
                throw new ArgumentNullException("solvedResultCallback");
            }

            this.InitializeSolver(maze);

            Console.WriteLine();
            Console.WriteLine("In Progress...");
            Console.WriteLine();

            var explorerQueue = new Queue<BreadthFirstSolverNode>(); //Queue that explores and maintains the routes

            if (maze.StartNode == null)
            {
                // No start node available to find the solution.
                solvedResultCallback(null);
                return;
            }

            BreadthFirstSolverNode startNode = GetBreadthFirstSolverNode(maze.StartNode); //Start of the maze as defined by IWalledMaze.
            startNode.Distance = 0;
            startNode.Predecessor = null;
            startNode.State = BreadthFirstSolverNodeState.Queued;
            explorerQueue.Enqueue(startNode);

            //var s = new Stopwatch();
            //s.Start();
            while (explorerQueue.Count > 0)
            {
                BreadthFirstSolverNode currentBreadthFirstSolverNodeNode = explorerQueue.Dequeue();
                IWalledMazeNode currentMazeNode = GetMazeNode(maze, currentBreadthFirstSolverNodeNode);

                if (maze.IsEndNode(currentMazeNode)) //Uses the goal defined by the IWalledMaze as terminating point.
                {
                    //s.Stop();
                    IEnumerable<IWalledMazeNode> solvedPath = TraceSolvedPath(maze, currentBreadthFirstSolverNodeNode);
                    solvedResultCallback(solvedPath); //Calls the callback Action with solved path and returns.
                    return;
                }

                foreach (IWalledMazeNode mazeNode in maze.GetAdjacentNodes(currentMazeNode))
                {
                    //Just use the positions from the adjNode and use the internal representation to do comparision.
                    BreadthFirstSolverNode adjacentBreadthFirstSolverNodeNode = GetBreadthFirstSolverNode(mazeNode);
                    if (adjacentBreadthFirstSolverNodeNode.State == BreadthFirstSolverNodeState.NotVisited)
                    {
                        var isValidNode = true;
                        foreach (var adjacentNode in maze.GetAdjacentNodes(mazeNode))
                        {
                            // Node is not valid if its neighboring nodes are not blocked
                            // this is to eliminate the nodes that just has 1 pixel opening.
                            if (adjacentNode.State == WalledMazeNodeState.Blocked)
                            {
                                isValidNode = false;
                                break;
                            }
                        }
                        if (isValidNode)
                        {
                            adjacentBreadthFirstSolverNodeNode.State = BreadthFirstSolverNodeState.Queued;
                            adjacentBreadthFirstSolverNodeNode.Predecessor = currentBreadthFirstSolverNodeNode;
                            adjacentBreadthFirstSolverNodeNode.Distance = currentBreadthFirstSolverNodeNode.Distance + 1;
                            explorerQueue.Enqueue(adjacentBreadthFirstSolverNodeNode);
                        }
                    }
                }
                currentBreadthFirstSolverNodeNode.State = BreadthFirstSolverNodeState.Visited;
            }

            solvedResultCallback(null); //unable to reach the end of the maze then no solution found.
        }

        /// <summary>
        /// Conversion function. Converts a maze node to a internal BreadthFirstSolverNode node.
        /// </summary>
        private BreadthFirstSolverNode GetBreadthFirstSolverNode(IWalledMazeNode mazeNode)
        {
            return breadthFirstSolverNodes[mazeNode.RowPosition, mazeNode.ColumnPosition]; //Both Breadth First Solver Node and Maze node have positional relationship.
        }

        /// <summary>
        /// Conversion function. Converts a BreadthFirstSolverNode to a maze node.
        /// </summary>
        private IWalledMazeNode GetMazeNode(IWalledMaze maze, BreadthFirstSolverNode breadthFirstSolverNode)
        {
            return maze.GetNode(breadthFirstSolverNode.RowPosition, breadthFirstSolverNode.ColumnPosition);//Both BFS and Maze node have positional relationship.
        }

        /// <summary>
        /// Initialize the bfs nodes <see cref="BreadthFirstSolverNode"/> for the corresponding maze nodes.
        /// </summary>
        private void InitializeSolver(IWalledMaze maze)
        {
            breadthFirstSolverNodes = new BreadthFirstSolverNode[maze.Height, maze.Width];
            IEnumerator<IWalledMazeNode> mazeNodes = maze.GetNodes();
            while (mazeNodes.MoveNext())
            {
                IWalledMazeNode mazeNode = mazeNodes.Current;
                if (mazeNode.State == WalledMazeNodeState.Blocked)//Blocked cells are walls. No need to visit these nodes again.
                    breadthFirstSolverNodes[mazeNode.RowPosition, mazeNode.ColumnPosition] = new BreadthFirstSolverNode(mazeNode.RowPosition, mazeNode.ColumnPosition) { State = BreadthFirstSolverNodeState.Visited, Distance = int.MaxValue };
                else //Other cells are open. These nodes are represented as "Not-Visited" nodes.
                    breadthFirstSolverNodes[mazeNode.RowPosition, mazeNode.ColumnPosition] = new BreadthFirstSolverNode(mazeNode.RowPosition, mazeNode.ColumnPosition) { State = BreadthFirstSolverNodeState.NotVisited, Distance = int.MaxValue };
            }
        }

        /// <summary>
        /// Traces the solved path using the end node and builds the internal BFS tree.
        /// </summary>
        private IEnumerable<IWalledMazeNode> TraceSolvedPath(IWalledMaze maze, BreadthFirstSolverNode endBreadthFirstSolverNode)
        {
            var currentNode = endBreadthFirstSolverNode;
            ICollection<IWalledMazeNode> pathTrace = new List<IWalledMazeNode>();
            while (currentNode != null)
            {
                pathTrace.Add(GetMazeNode(maze, currentNode));
                currentNode = currentNode.Predecessor;
            }
            return pathTrace;
        }
    }
}