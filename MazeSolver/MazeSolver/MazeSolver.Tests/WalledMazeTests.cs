using MazeSolver;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MazeSolver.BreadthFirstSolver;
using System.Linq;

namespace MazeSolver.Tests
{
    [TestClass]
    public class WalledMazeTests
    {
        private WalledMazeNode[,] mazeMap;
        private IWalledMaze walledMaze;

        /// <summary>
        /// /Initializes the maze
        /// </summary>
        /// <returns></returns>
        [TestInitialize]
        public void InitializeWalledMaze()
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
        public void WallMaze_Solve_Throws_If_Null_Is_Passed_ForMazeMap()
        {
            var walledMaze = new WalledMaze(null);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void WallMaze_Solve_Throws_If_Null_Is_Passed_ForSolver()
        {
            this.walledMaze.Solve(null, null);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void WallMaze_Solve_Throws_If_Null_Is_Passed_ForCallback()
        {
            this.walledMaze.Solve(new BreadthFirstMazeSolver(), null);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void WallMaze_GetNode_Throws_If_Argument_IsOutofRange()
        {
            this.walledMaze.GetNode(-1, -1);
        }


        [TestMethod]
        public void WallMaze_GetNode_GetsFirstElement_WhenAskedForIt()
        {
           Assert.IsTrue(this.mazeMap[0,0] == this.walledMaze.GetNode(0,0));
        }

        [TestMethod]
        public void WallMaze_IsEnd_CheckForPass()
        {
            Assert.IsTrue(this.walledMaze.IsEndNode(new WalledMazeNode(0,0) {State =  WalledMazeNodeState.End }) == true);
        }


        [TestMethod]
        public void WallMaze_IsEnd_CheckForFail()
        {
            Assert.IsTrue(this.walledMaze.IsEndNode(new WalledMazeNode(0, 0) { State = WalledMazeNodeState.Open }) == false);
        }

        [TestMethod]
        public void WallMaze_GetAdjacentNodes_CheckForAdjacentNodes()
        {
            var adjacentNodes = this.walledMaze.GetAdjacentNodes(new WalledMazeNode(1, 1));
            var isAdjacent = adjacentNodes.Any((i) => ((i.RowPosition != 0 && i.RowPosition != 2)) || (i.ColumnPosition != 0 && i.ColumnPosition != 2));
            Assert.IsTrue(isAdjacent == true);
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
