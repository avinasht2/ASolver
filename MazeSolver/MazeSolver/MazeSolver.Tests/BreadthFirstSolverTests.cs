using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MazeSolver.BreadthFirstSolver;

namespace MazeSolver.Tests
{
    [TestClass]
    public class BreadthFirstSolverTests
    {
        private WalledMazeNode[,] mazeMap;
        private IWalledMaze walledMaze;
        private BreadthFirstMazeSolver breadthFirstMazeSolver;

        [TestInitialize]
        public void InitializeBreadthFirstSolver()
        {
            this.breadthFirstMazeSolver = new BreadthFirstMazeSolver();
        }

        /// <summary>
        /// /Initializes the maze
        /// </summary>
        /// <returns></returns>
        private void InitializeWalledMaze()
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format("MazeSolver.Tests.maze1.png");
            Stream imgStream =
                      Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            var image = new Bitmap(imgStream);

            // Use the lock bits to process the bitmap image faster.
            int bytesPerPixel = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            int byteCount = bitmapData.Stride * image.Height;
            byte[] pixelBytes = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixelBytes, 0, pixelBytes.Length);
            image.UnlockBits(bitmapData);

            var wallColor = Color.Black;
            var openColor = Color.White;
            var startColor = Color.Red;
            var finishColor = Color.Blue;
            int heightInPixels = bitmapData.Height;
            int widthInPixels = bitmapData.Width;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            this.mazeMap = new WalledMazeNode[heightInPixels, widthInPixels];
            WalledMaze walledMaze = new WalledMaze(mazeMap);

            for (var x = 0; x < heightInPixels; x++)
            {
                int currentLine = x * bitmapData.Stride;
                for (var y = 0; y < widthInBytes; y = y + bytesPerPixel)
                {
                    var pixelColor = this.GetPixelColor(pixelBytes, bytesPerPixel, currentLine + y);
                    if (pixelColor.ToArgb() == wallColor.ToArgb())//Black Color is a wall. Mark them as blocked
                    {
                        mazeMap[x, y / bytesPerPixel] = new WalledMazeNode(x, y / bytesPerPixel) { State = WalledMazeNodeState.Blocked };
                        continue;
                    }
                    else if (pixelColor.ToArgb() == startColor.ToArgb())
                    {
                        mazeMap[x, y / bytesPerPixel] = new WalledMazeNode(x, y / bytesPerPixel) { State = WalledMazeNodeState.Start };
                        if (walledMaze.StartNode == null) // Pick the first one as start node.
                            walledMaze.StartNode = mazeMap[x, y / bytesPerPixel];
                    }
                    else if (pixelColor.ToArgb() == finishColor.ToArgb())
                    {
                        mazeMap[x, y / bytesPerPixel] = new WalledMazeNode(x, y / bytesPerPixel) { State = WalledMazeNodeState.End };
                        if (walledMaze.EndNodes == null)
                            walledMaze.EndNodes = new HashSet<IWalledMazeNode>();
                        walledMaze.EndNodes.Add(mazeMap[x, y / bytesPerPixel]);
                    }
                    else if (pixelColor.ToArgb() == openColor.ToArgb())
                    {
                        mazeMap[x, y / bytesPerPixel] = new WalledMazeNode(x, y / bytesPerPixel) { State = WalledMazeNodeState.Open };
                    }
                }
            }

            this.walledMaze = walledMaze;
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void BreadthFirstSolver_Solve_Throws_If_Null_Is_Passed_ForWalledMaze()
        {
            this.breadthFirstMazeSolver.Solve(null, null);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void BreadthFirstSolver_Solve_Throws_If_Null_Is_Passed_ForSolver()
        {
            this.breadthFirstMazeSolver.Solve(this.walledMaze, null);
        }

        [TestMethod]
        public void BreadthFirstSolver_Solve_Verify_For_NullStartNode()
        {
            var nullStartMazeMap = new WalledMazeNode[,] { { new WalledMazeNode(0, 0) { State = WalledMazeNodeState.Blocked }, new WalledMazeNode(0, 1) { State = WalledMazeNodeState.Open } } };

            var nullStartWalledmaze = new WalledMaze(nullStartMazeMap);
            this.breadthFirstMazeSolver.Solve(nullStartWalledmaze,
                new Action<IEnumerable<IWalledMazeNode>>((solvedPath)=>
                {
                    Assert.IsNull(solvedPath);
                    return;
                }
                ));
        }

        [TestMethod]
        public void BreadthFirstSolver_Solve_Check_For_SolvedPath()
        {
            this.InitializeWalledMaze();
            this.breadthFirstMazeSolver.Solve(this.walledMaze,
                new Action<IEnumerable<IWalledMazeNode>>((solvedPath) =>
                {
                    // No solution
                    if (solvedPath == null)
                    {
                        Assert.IsNull(solvedPath);
                        return;
                    }
                    else
                    {
                        var solutionWalledMazeNodes = solvedPath as IEnumerable<IWalledMazeNode>;
                        Assert.IsFalse(solutionWalledMazeNodes.Any((mn) => mn.State == WalledMazeNodeState.Blocked));
                        Assert.IsTrue(solutionWalledMazeNodes.Any((mn) => mn.State == WalledMazeNodeState.Start));
                        Assert.IsTrue(solutionWalledMazeNodes.Any((mn) => mn.State == WalledMazeNodeState.End));
                        return;
                    }                    
                }
                ));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelBytes"></param>
        /// <param name="bytesPerPixel"></param>
        /// <param name="index"></param>
        /// <returns>color</returns>
        private Color GetPixelColor(byte[] pixelBytes, int bytesPerPixel, int index)
        {
            Color pixelColor = Color.Empty;

            if (bytesPerPixel == 4) // For 4 bpp get Red, Green, Blue and Alpha
            {
                byte b = pixelBytes[index];
                byte g = pixelBytes[index + 1];
                byte r = pixelBytes[index + 2];
                byte a = pixelBytes[index + 3]; // a
                pixelColor = Color.FromArgb(a, r, g, b);
            }
            if (bytesPerPixel == 3) // For 3 bpp get Red, Green and Blue
            {
                byte b = pixelBytes[index];
                byte g = pixelBytes[index + 1];
                byte r = pixelBytes[index + 2];
                pixelColor = Color.FromArgb(r, g, b);
            }

            return pixelColor;
        }
    }
}
