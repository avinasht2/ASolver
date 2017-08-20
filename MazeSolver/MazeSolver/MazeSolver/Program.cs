using MazeSolver.BreadthFirstSolver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MazeSolver
{
    class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid number of arguments");
                PrintUsage();
                return;
            }

            string inputImagePath = args[0];
            string outputImagePath = args[1];

            try
            {
                if (!File.Exists(inputImagePath))
                    throw new FileNotFoundException(string.Format("{0} not found", inputImagePath));

                Bitmap image = null;

                try
                {
                    image = new Bitmap(inputImagePath);
                }
                catch (ArgumentException)//Image is not a valid bitmap(.jpg,.png etc..). Framework exception message should be appropriate
                {
                    throw;
                }

                Console.WriteLine();
                Console.WriteLine("Read the image from the location {0}", inputImagePath);

                // Use the lock bits to process the bitmap image faster.
                int bytesPerPixel = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                int byteCount = bitmapData.Stride * image.Height;
                byte[] pixelBytes = new byte[byteCount];
                IntPtr ptrFirstPixel = bitmapData.Scan0;
                Marshal.Copy(ptrFirstPixel, pixelBytes, 0, pixelBytes.Length);
                image.UnlockBits(bitmapData);

                IWalledMaze mazeProblem = CreateWalledMaze(bitmapData, pixelBytes, bytesPerPixel);//configures the maze.
                IMazeSolver breadthFirstMazeSolver = new BreadthFirstMazeSolver();//intializes the solver.

                Console.WriteLine();
                Console.WriteLine("****Initializing the process to solve the maze provided****");

                mazeProblem.Solve(breadthFirstMazeSolver, (solvedPath, solutionFound) => //callback defines action to perform after the maze is solved. Here the action is to save the bitmap in "outputImagepasth".
                {
                    Console.WriteLine();
                    Console.WriteLine("*********Process Completed*********");
                    if (!solutionFound)
                    {
                        Console.WriteLine("No solution for the maze provided");
                        Console.WriteLine();
                        return;
                    }
                    else
                    {
                        Bitmap bitMap = new Bitmap(inputImagePath);
                        bitmapData = bitMap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, bitMap.PixelFormat);
                        ptrFirstPixel = bitmapData.Scan0;
                        foreach (var node in solvedPath)
                        {
                            solutionFound = true;
                            SetPixelColor(pixelBytes, bytesPerPixel, ((node.RowPosition * bitmapData.Stride) + node.ColumnPosition * bytesPerPixel), Color.Green);
                        }

                        // copy modified bytes back
                        Marshal.Copy(pixelBytes, 0, ptrFirstPixel, pixelBytes.Length);
                        bitMap.UnlockBits(bitmapData);

                        bitMap.Save(outputImagePath);
                        Console.WriteLine("Solved maze can be found at '{0}'", outputImagePath);
                    }

                    Console.WriteLine();
                });
            }
            catch (Exception e)//generic error handling. Specific errors are handled inside the main code.
            {
                Console.WriteLine();
                Console.WriteLine("!!!ERROR");
                Console.WriteLine("{0}", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Scans the image and stores each pixel information as <see cref="IWalledMazeNode"/>.
        /// </summary>
        public static IWalledMaze CreateWalledMaze(BitmapData bitmapData, byte[] pixelBytes, int bytesPerPixel)
        {
            var wallColor = Color.Black;
            var openColor = Color.White;
            var startColor = Color.Red;
            var finishColor = Color.Blue;
            int heightInPixels = bitmapData.Height;
            int widthInPixels = bitmapData.Width;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            var mazeMap = new WalledMazeNode[heightInPixels, widthInPixels];
            WalledMaze walledMaze = new WalledMaze(mazeMap);

            for (var x = 0; x < heightInPixels; x++)
            {
                int currentLine = x * bitmapData.Stride;
                for (var y = 0; y < widthInBytes; y = y + bytesPerPixel)
                {
                    var pixelColor = GetPixelColor(pixelBytes, bytesPerPixel, currentLine + y);
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
                    else
                    {
                        //Invalid color.
                        //Eventhough anomalous colors could be defined as OPEN nodes, such implementations may confuse the solver when BLACK walls are defined with a hue. 
                        //I restricted such usage by throwing exceptions for unknown colors. Better safe than sorry. :)
                        throw new Exception(string.Format("I found a color {0} that this maze was not configured to handle. Please use colors that you originally specified.", pixelColor.Name));
                    }
                }
            }

            return walledMaze;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelBytes"></param>
        /// <param name="bytesPerPixel"></param>
        /// <param name="index"></param>
        /// <returns>color</returns>
        public static Color GetPixelColor(byte[] pixelBytes, int bytesPerPixel, int index)
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

        /// <summary>
        /// Prints the usage of the tool.
        /// </summary>
        public static void PrintUsage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("=====");
            Console.WriteLine("maze source.[bmp,png,jpg] destination.[bmp,png,jpg]");
            Console.WriteLine();
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void SetPixelColor(byte[] pixelBytes, int bytesPerPixel, int index, Color color)
        {
            if (bytesPerPixel == 4) // For 4 bpp set Red, Green, Blue and Alpha
            {
                pixelBytes[index] = color.B;
                pixelBytes[index + 1] = color.G;
                pixelBytes[index + 2] = color.R;
                pixelBytes[index + 3] = color.A;
            }
            if (bytesPerPixel == 3) // For 3 bpp set Red, Green and Blue
            {
                pixelBytes[index] = color.B;
                pixelBytes[index + 1] = color.G;
                pixelBytes[index + 2] = color.R;
            }
        }
    }
}
