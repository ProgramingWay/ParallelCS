using System;
using System.Collections.Concurrent;
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
    static void Main(string[] args)
    {
        ConcurrentDictionary<string, CityTemperatureData> cityTemperatureData = new ConcurrentDictionary<string, CityTemperatureData>();
        string[] cities = Enumerable.Range(1, 99).Select(i => $"Miasto{i}").ToArray();

        Parallel.ForEach(cities, city =>
        {
            double[] temperatures = GenerateTemperatureData(365);
            cityTemperatureData[city] = new CityTemperatureData(city, temperatures);
        });

        foreach (var cityData in cityTemperatureData)
        {
            Console.WriteLine($"{cityData.Key}: {string.Join(", ", cityData.Value.Temperatures.Take(5))}...");
        }
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
}
