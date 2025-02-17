using System.Collections.Concurrent;
using System.ComponentModel;

class Program
{
    const int CITIES_COUNT = 99;
    const int DAYS_IN_YEAR = 365;
    static ConcurrentDictionary<string, List<double>> weatherData = new();
    static double globalMin = 0;
    static double globalMax = 0;
    static readonly object lockObject = new();
    static bool isAnalysisCompleted = false;

    static void Main()
    {
        GenerateWeatherData();
        Console.WriteLine("Dane pogodowe zostały wygenerowane. Rozpoczęto analizę...");

        BackgroundWorker bgWorker = new();
        bgWorker.DoWork += DisplayWaitingMessage;
        bgWorker.RunWorkerAsync();

        AnalyzeWeatherData();
        isAnalysisCompleted = true;

        Console.WriteLine("\nAnaliza zakończona.");
        Console.WriteLine($"Najniższa temperatura w zbiorze: {globalMin}C");
        Console.WriteLine($"Najwyższa temperatura w zbiorze: {globalMax}C");
    }

    static void GenerateWeatherData()
    {
        Parallel.For(0, CITIES_COUNT, i =>
        {
            string cityName = $"Miasto{i + 1}";
            var temperatures = new List<double>();
            Random random = new Random();

            for (int j = 0; j < DAYS_IN_YEAR; j++)
            {
                temperatures.Add(Math.Round(random.NextDouble() * 40 - 10, 1));
            }

            weatherData[cityName] = temperatures;
        });
    }

    static void AnalyzeWeatherData()
    {
        Parallel.ForEach(weatherData, cityData =>
        {
            var temperatures = cityData.Value;
            temperatures.Sort();
            double median = temperatures[DAYS_IN_YEAR / 2];
            double minTemp = temperatures.First();
            double maxTemp = temperatures.Last();

            lock (lockObject)
            {
                if (minTemp < globalMin) globalMin = minTemp;
                if (maxTemp > globalMax) globalMax = maxTemp;
            }
            Console.WriteLine($"{cityData.Key}: Mediana {median}C");
        });
    }

    static void DisplayWaitingMessage(object sender, DoWorkEventArgs e)
    {
        while (!isAnalysisCompleted)
        {
            Console.WriteLine("Analiza danSych w toku, proszę czekać...");
            Thread.Sleep(1000);
        }
    }
}
