using System.Collections.Generic;
using System.Linq;
using System;

public class KNN
{
    // Tahmin fonksiyonu
    public static string Predict(string[,] dataset, string[] input, int k)
    {
        // Veri setinden sýnýflarý ayýr
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

        // Mesafeye göre sýrala
        distances = distances.OrderBy(d => d.Item1).ToList();

        // Ýlk k komþuyu al
        var kNearestNeighbors = distances.Take(k).ToList();

        // En sýk görülen sýnýfý belirle
        var prediction = kNearestNeighbors
            .GroupBy(d => d.Item2)
            .OrderByDescending(g => g.Count())
            .First().Key;

        return prediction;
    }

    // Öklid Mesafesi Hesaplama Fonksiyonu
    public static double EuclideanDistance(double[] point1, double[] point2)
    {
        if (point1.Length != point2.Length)
            throw new ArgumentException("Veri noktalarýnýn uzunluklarý eþit olmalýdýr.");

        double sum = 0;
        for (int i = 0; i < point1.Length; i++)
        {
            sum += Math.Pow(point1[i] - point2[i], 2);
        }
        return Math.Sqrt(sum);
    }

    // String deðerleri sayýsala çevirme (örnek bir yöntem)
    public static double ConvertToNumeric(string value)
    {
        // Sayýsal deðerler için bu fonksiyonu deðiþtirin
        return double.TryParse(value, out double result) ? result : 0.0; // Eðer sayýya dönüþtürülemezse 0 döner
    }

    // Veri setini eðitim ve test setlerine ayýran fonksiyon
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