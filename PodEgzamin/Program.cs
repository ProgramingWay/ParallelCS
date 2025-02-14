using System.Collections;


ArrayList cityList = new();


Random r = new();


Parallel.For(0, 100, i =>
{
    City city = new($"Miasto{i}");
    ArrayList temperatures = new(366);

    cityList.Add(city);

    Parallel.For(0, 366, j => {

        double temperature = Math.Round(r.NextDouble() + r.Next(40) - 20, 2);
        temperatures.Add(temperature);
    });
    city.Temperatures = temperatures;
});

foreach (var city in cityList)
{
    Console.WriteLine(((City)city).cityName);

    foreach (var temperatura in ((City)city).Temperatures)
    {
        Console.WriteLine("    " + temperatura);
    }

    Console.WriteLine();
}


public class City 
{
    public string cityName {  get; set; }
    public ArrayList Temperatures {  get; set; }

    public City(string name) 
    {
        cityName = name;
    }
}