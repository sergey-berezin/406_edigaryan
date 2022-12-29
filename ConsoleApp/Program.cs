using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Nuget;

internal class Program
{
    private const string imagesFolderName = "images";

    private static async Task Main(string[] args)
    {
        try
        {
            using var arcFaceComponent = new Component();

            var canceleationToken = new CancellationToken();
            var imageFolderPath = Path.GetFullPath($"{Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\")}{imagesFolderName}");
            string[] allfiles = Directory.GetFiles(imageFolderPath, "*.png");
            var images = new List<Image<Rgb24>>();
            foreach (string filename in allfiles)
            {
                images.Add(Image.Load<Rgb24>(filename));
            }

            var distanceMatrix = await arcFaceComponent.GetDistanceMatrix(images.ToArray(), canceleationToken);
            var similarityMatrix = await arcFaceComponent.GetSimilarityMatrix(images.ToArray(), canceleationToken);

            PrintMatrix(distanceMatrix, "Distance Matrix");
            PrintMatrix(similarityMatrix, "Similarity Matrix");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            if(ex.InnerException != null)
            {
                Console.WriteLine(ex.InnerException);
            }
        }
    }

    private static void PrintMatrix(float[,] matrix, string info)
    {
        Console.WriteLine(info);

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write(matrix[i, j]);
                Console.Write(' ');
            }

            Console.WriteLine(Environment.NewLine);
        }
    }
}