using System;
using System.Threading;
using System.Threading.Tasks;

//* Klasa odpowiedzialna za obliczenia i raportowanie postępu.
public class CalculationProgress
{
    private readonly IProgress<(int IntervalId, int Progress)> _progress;
    private readonly CancellationToken _cancellationToken;

    public CalculationProgress(IProgress<(int IntervalId, int Progress)> progress, CancellationToken cancellationToken)
    {
        _progress = progress;
        _cancellationToken = cancellationToken;
    }

    public async Task<double> CalculateWithProgress(int intervalId, Func<double, double> function, double start, double end, int steps)
    {
        return await Task.Run(() =>
        {
            double result = 0;
            for (int i = 0; i <= steps; i++)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    MessageQueue.Enqueue($"Przerwano obliczenia dla przedziału {intervalId}.");
                    return double.NaN;
                }

                double x1 = start + i * (end - start) / steps;
                double x2 = x1 + (end - start) / steps;
                result += (function(x1) + function(x2)) * (end - start) / (2 * steps);

                if (i % (steps / 10) == 0)
                {
                    int progressPercent = i * 100 / steps;
                    _progress?.Report((intervalId, progressPercent));
                }
            }
            return result;
        });
    }
}