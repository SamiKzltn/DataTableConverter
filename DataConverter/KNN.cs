using System.Collections.Generic;
using System.Linq;
using System;

public class KNN
{
    // Tahmin fonksiyonu
    public static string Predict(string[,] dataset, string[] input, int k)
    {
        // Veri setinden s�n�flar� ay�r
        int rowCount = dataset.GetLength(0);
        int colCount = dataset.GetLength(1);
        var distances = new List<Tuple<double, string>>();

        for (int i = 0; i < rowCount; i++)
        {
            double[] features = new double[colCount - 1];
            for (int j = 0; j < colCount - 1; j++)
            {
                features[j] = ConvertToNumeric(dataset[i, j]);
            }
            string label = dataset[i, colCount - 1];

            double[] inputNumeric = input.Select(ConvertToNumeric).ToArray();
            double distance = EuclideanDistance(features, inputNumeric);
            distances.Add(new Tuple<double, string>(distance, label));
        }

        // Mesafeye g�re s�rala
        distances = distances.OrderBy(d => d.Item1).ToList();

        // �lk k kom�uyu al
        var kNearestNeighbors = distances.Take(k).ToList();

        // En s�k g�r�len s�n�f� belirle
        var prediction = kNearestNeighbors
            .GroupBy(d => d.Item2)
            .OrderByDescending(g => g.Count())
            .First().Key;

        return prediction;
    }

    // �klid Mesafesi Hesaplama Fonksiyonu
    public static double EuclideanDistance(double[] point1, double[] point2)
    {
        if (point1.Length != point2.Length)
            throw new ArgumentException("Veri noktalar�n�n uzunluklar� e�it olmal�d�r.");

        double sum = 0;
        for (int i = 0; i < point1.Length; i++)
        {
            sum += Math.Pow(point1[i] - point2[i], 2);
        }
        return Math.Sqrt(sum);
    }

    // String de�erleri say�sala �evirme (�rnek bir y�ntem)
    public static double ConvertToNumeric(string value)
    {
        // Say�sal de�erler i�in bu fonksiyonu de�i�tirin
        return double.TryParse(value, out double result) ? result : 0.0; // E�er say�ya d�n��t�r�lemezse 0 d�ner
    }

    // Veri setini e�itim ve test setlerine ay�ran fonksiyon
    public static (string[,], string[,]) SplitDataset(string[,] dataset, double trainRatio)
    {
        int rowCount = dataset.GetLength(0);
        int colCount = dataset.GetLength(1);
        int trainSize = (int)(rowCount * trainRatio);

        string[,] trainSet = new string[trainSize, colCount];
        string[,] testSet = new string[rowCount - trainSize, colCount];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                if (i < trainSize)
                {
                    trainSet[i, j] = dataset[i, j];
                }
                else
                {
                    testSet[i - trainSize, j] = dataset[i, j];
                }
            }
        }

        return (trainSet, testSet);
    }
}