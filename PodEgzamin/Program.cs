using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

internal partial class Program
{
    private static void Main(string[] args)
    {
        ConcurrentBag<City> cityList = new();

        GenerateTemperatures(cityList);

        foreach (var city in cityList)
        {
            Console.WriteLine(city.cityName);
            foreach (var temperature in city.Temperatures)
            {
                Console.WriteLine("    " + temperature);
            }
            Console.WriteLine();
        }
    }

    static void GenerateTemperatures(ConcurrentBag<City> cityList) 
    {
        Parallel.For(0, 99, i =>
        {
            City city = new($"Miasto{i + 1}");
            ConcurrentBag<double> temperatures = new();
            Random localRandom = new Random();

            Parallel.For(0, 366, j =>
            {
                double temperature = Math.Round(localRandom.NextDouble() + localRandom.Next(40) - 20, 2);
                temperatures.Add(temperature);
            });

            city.Temperatures = temperatures;
            cityList.Add(city);
        });
    }
}

public class City
{
    public string cityName { get; set; }
    public ConcurrentBag<double> Temperatures { get; set; } = new();

    public City(string name)
    {
        cityName = name;
    }
}
