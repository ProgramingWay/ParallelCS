using System;
using System.Linq;
using System.Threading.Tasks;
using ScottPlot;
using ScottPlot.TickGenerators;

public interface IIntegralCalculator
{
    Task<double> CalculateAsync(Func<double, double> f, double a, double b, int n);
}

public interface IPlotGenerator
{
    Task<(DateTime[] x, double[] y)> GeneratePlotDataAsync(Func<double, double> f, double a, double b, int points);
}

public interface IPlotSaver
{
    Task SavePlotAsync(DateTime[] x, double[] y, string fileName);
}

public class RectangleIntegralCalculator : IIntegralCalculator
{
    public async Task<double> CalculateAsync(Func<double, double> f, double a, double b, int n)
    {
        return await Task.Run(() =>
        {
            double h = (b - a) / n;
            double sum = 0.0;
            object lockObj = new();

            Parallel.For(0, n, () => 0.0, (i, state, localSum) =>
            {
                Thread.Sleep(10);
                double x = a + i * h;
                return localSum + f(x);
            },
            localSum => { lock (lockObj) { sum += localSum; } });

            return h * sum;
        });
    }
}

public class DateTimePlotGenerator : IPlotGenerator
{
    private readonly DateTime _baseDate = new(2023, 1, 1);

    public async Task<(DateTime[] x, double[] y)> GeneratePlotDataAsync(Func<double, double> f, double a, double b, int points)
    {
        return await Task.Run(() =>
        {
            DateTime[] x = new DateTime[points];
            double[] y = new double[points];
            double step = (b - a) / (points - 1);

            Parallel.For(0, points, i =>
            {
                double days = a + i * step;
                x[i] = _baseDate.AddDays(days);
                y[i] = f(days);
            });

            return (x, y);
        });
    }
}

public class PlotFileSaver : IPlotSaver
{
    public async Task SavePlotAsync(DateTime[] x, double[] y, string fileName)
    {
        await Task.Run(() =>
        {
            var plotBuilder = new MondayPlotBuilder();
            Plot plot = plotBuilder.BuildPlot(x, y);
            plot.SavePng(fileName, 800, 600);
        });
    }
}

public class MondayPlotBuilder
{
    public Plot BuildPlot(DateTime[] x, double[] y)
    {
        Plot plot = new();
        plot.Title("Wykres funkcji z całkowania");
        plot.XLabel("Data");
        plot.YLabel("Wartość");

        double[] xAsOADate = x.Select(d => d.ToOADate()).ToArray();
        var scatter = plot.Add.Scatter(xAsOADate, y);
        scatter.LineWidth = 2;
        scatter.Color = Colors.Blue;

        ConfigureTicks(plot, x);
        return plot;
    }

    private static void ConfigureTicks(Plot plot, DateTime[] x)
    {
        var tickGen = new NumericManual();

        foreach (DateTime dt in x.Where(d => d.DayOfWeek == DayOfWeek.Monday))
        {
            tickGen.AddMajor(dt.ToOADate(), dt.ToString("dd MMM"));
        }

        plot.Axes.Bottom.TickGenerator = tickGen;
        plot.Axes.Bottom.Label.Text = "Daty poniedziałków";
    }
}

public interface IUserInputHandler
{
    Task<string> GetValidFileNameAsync();
}

public class ConsoleInputHandler : IUserInputHandler
{
    public async Task<string> GetValidFileNameAsync()
    {
        return await Task.Run(() =>
        {
            while (true)
            {
                Console.Write("Podaj nazwę pliku (np. wykres.png): ");
                string fileName = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Nazwa pliku nie może być pusta!");
                    continue;
                }

                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    fileName += ".png";

                return fileName;
            }
        });
    }
}

public class IntegrationApp
{
    private readonly IIntegralCalculator _integralCalculator;
    private readonly IPlotGenerator _plotGenerator;
    private readonly IPlotSaver _plotSaver;
    private readonly IUserInputHandler _userInputHandler;

    public IntegrationApp(
        IIntegralCalculator integralCalculator,
        IPlotGenerator plotGenerator,
        IPlotSaver plotSaver,
        IUserInputHandler userInputHandler)
    {
        _integralCalculator = integralCalculator;
        _plotGenerator = plotGenerator;
        _plotSaver = plotSaver;
        _userInputHandler = userInputHandler;
    }

    public async Task RunAsync()
    {
        Func<double, double> function = x => Math.Sqrt(2 * x) + 3;
        double a = 0;
        double b = 100;
        int intervals = 1_000_000;
        int plotPoints = 1000;

        var plotDataTask = _plotGenerator.GeneratePlotDataAsync(function, a, b, plotPoints);
        var integralTask = _integralCalculator.CalculateAsync(function, a, b, intervals);
        var fileNameTask = _userInputHandler.GetValidFileNameAsync();

        await Task.WhenAll(plotDataTask, fileNameTask);

        await _plotSaver.SavePlotAsync(plotDataTask.Result.x, plotDataTask.Result.y, fileNameTask.Result);
        
        double result = await integralTask;
        Console.WriteLine($"Wartość całki: {result:F4}");
    }
}

public class Program
{
    public static async Task Main()
    {
        var app = new IntegrationApp(
            new RectangleIntegralCalculator(),
            new DateTimePlotGenerator(),
            new PlotFileSaver(),
            new ConsoleInputHandler());

        await app.RunAsync();
    }
}