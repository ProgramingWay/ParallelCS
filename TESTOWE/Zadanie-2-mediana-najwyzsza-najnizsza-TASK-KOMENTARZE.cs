using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CityTemperatureData
{
    public string CityName { get; set; }
    public double[] Temperatures { get; set; }

    public CityTemperatureData(string cityName, double[] temperatures)
    {
        CityName = cityName;
        Temperatures = temperatures;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        List<CityTemperatureData> cityTemperatureData = new List<CityTemperatureData>();
        string[] cities = Enumerable.Range(1, 99).Select(i => $"Miasto{i}").ToArray();

        foreach (var city in cities)
        {
            double[] temperatures = GenerateTemperatureData(365);
            cityTemperatureData.Add(new CityTemperatureData(city, temperatures));
        }

        List<Task> tasks = new List<Task>();

        double globalMin = double.MaxValue;
        double globalMax = double.MinValue;

        List<(string city, double median)> cityMedians = new List<(string, double)>();

        int processedCities = 0;

        foreach (var cityData in cityTemperatureData)
        {
            var task = Task.Run(() =>
            {
                double median = GetMedian(cityData.Temperatures);

                lock (cityTemperatureData)
                {
                    cityMedians.Add((cityData.CityName, median));

                    double cityMin = cityData.Temperatures.Min();
                    double cityMax = cityData.Temperatures.Max();

                    if (cityMin < globalMin)
                        globalMin = cityMin;

                    if (cityMax > globalMax)
                        globalMax = cityMax;
                }
            });

            tasks.Add(task);

            processedCities++;
            if (processedCities % 20 == 0)
            {
                Console.WriteLine($"Przetworzono {processedCities} miast, proszę czekać...");
            }
        }

        while (tasks.Any(t => !t.IsCompleted))
        {
            await Task.Delay(1000);
        }

        await Task.WhenAll(tasks);

        foreach (var cityMedian in cityMedians)
        {
            Console.WriteLine($"{cityMedian.city}: Mediana temperatur = {cityMedian.median}");
        }

        Console.WriteLine($"Globalna minimalna temperatura: {globalMin}");
        Console.WriteLine($"Globalna maksymalna temperatura: {globalMax}");
    }

    static double[] GenerateTemperatureData(int days)
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());
        double[] temperatures = new double[days];

        for (int i = 0; i < days; i++)
        {
            temperatures[i] = Math.Round(random.NextDouble() * (30.0 - 10.0) + 10.0, 1);
        }

        return temperatures;
    }

    static double GetMedian(double[] temperatures)
    {
        var sortedTemperatures = temperatures.OrderBy(t => t).ToArray();
        int count = sortedTemperatures.Length;

        if (count % 2 == 0)
        {
            return (sortedTemperatures[count / 2 - 1] + sortedTemperatures[count / 2]) / 2.0;
        }
        else
        {
            return sortedTemperatures[count / 2];
        }
    }
}
