using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;

namespace ImageProcessor
{
    class Program
    {
        private const string RenamedDirectory = "Renamed";
        private const string MarkedDirectory = "Marked";

        private static readonly String[] imageExtensions = { ".JPG", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".svg" };

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter directory to process:");
                Console.WriteLine("Enter 1 to add date to files;");
                Console.WriteLine("Enter 2 to move and mark files;");

                var sourceDirectoryPath = @"C:\Temp";

                Console.WriteLine("Select an action:");
                int.TryParse(Console.ReadLine(), out var selectedAction);

                switch (selectedAction)
                {
                    case 1:
                        AddDate(sourceDirectoryPath, RenamedDirectory);
                        break;
                    case 2:
                        MarkFiles(sourceDirectoryPath, MarkedDirectory);
                        break;
                    default:
                        Console.WriteLine("Select a valid action");
                        break;
                }

                Console.WriteLine("Continue?");

                var isRepeatRequired = Console.ReadLine();

                if (string.Equals(isRepeatRequired, "n"))
                {
                    break;
                }
            }

            Console.ReadLine();
        }

        private static void AddDate(string sourceDirectoryPath, string operationName)
        {
            var targetDirectoryPath = CreateTargetFolder(sourceDirectoryPath, operationName);
            var imageFilePaths = GetImageFiles(sourceDirectoryPath);

            foreach (var imageFilePath in imageFilePaths)
            {
                AddDateToFileName(imageFilePath, targetDirectoryPath);
            }
        }

        private static IEnumerable<string> GetImageFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath).Where(itemPath => imageExtensions.Contains(Path.GetExtension(itemPath)));
        }

        private static void DrawDate(string sourceFilePath, string targetDirectoryName)
        {
            using (var fileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var image = Image.FromStream(fileStream, false, false))
                {
                    var graphics = Graphics.FromImage(image);
                    var drawString = GetLastModifiedDate(sourceFilePath).ToString("MM-dd-yyyy HH mm ss fffffff");
                    var drawFont = new Font("Arial", 16);
                    var drawBrush = new SolidBrush(Color.Black);

                    // Create rectangle for drawing.
                    var width = 350.0F;
                    var height = 50.0F;
                    var x = image.Width - width - 10.0F;
                    var y = 75.0F;
                    var drawRect = new RectangleF(x, y, width, height);

                    // Draw string to screen.
                    graphics.DrawString(drawString, drawFont, drawBrush, drawRect);

                    var newPath = Path.Combine(targetDirectoryName, Path.GetFileName(sourceFilePath));

                    image.Save(newPath, image.RawFormat);
                }
            }
        }

        private static void MarkFiles(string sourceDirectoryPath, string operationName)
        {
            var targetDirectoryPath = CreateTargetFolder(sourceDirectoryPath, operationName);
            var imageFilePaths = GetImageFiles(sourceDirectoryPath);

            foreach (var imageFilePath in imageFilePaths)
            {
                DrawDate(imageFilePath, targetDirectoryPath);
            }
        }

        private static void AddDateToFileName(string filePath, string targetDirectoryPath)
        {
            var lastModifiedDate = GetLastModifiedDate(filePath).ToString("MM-dd-yyyy HH mm ss fffffff");

            File.Copy(filePath, Path.Combine(targetDirectoryPath, Path.GetFileNameWithoutExtension(filePath) + " " + lastModifiedDate + Path.GetExtension(filePath)), true);
        }

        private static string CreateTargetFolder(string sourceDirectoryPath, string operationName)
        {
            var directoryInfo = new DirectoryInfo(sourceDirectoryPath);
            var targetDirectoryPath = Path.Combine($"{directoryInfo.FullName}", $"{directoryInfo.Name}_{ operationName}");

            Directory.CreateDirectory(targetDirectoryPath);

            return targetDirectoryPath;
        }

        private static DateTime GetLastModifiedDate(string path)
        {
            return File.GetLastWriteTime(path);
        }
    }
}
