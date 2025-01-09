using System;
using System.Threading;

namespace taskThird.Utils
{
    public static class IntegralCalculator
    {
        public static double Calculate(Func<double, double> func, double a, double b, int n, CancellationToken token, Action<int> reportProgress)
        {
            double stepSize = (b - a) / n;
            double sum = 0;

            double firstValue = func(a);
            double lastValue = func(b);
            sum += 0.5 * (firstValue + lastValue);

            // Iteracyjne dodawanie wartości
            for (int i = 1; i < n; i++)
            {
                token.ThrowIfCancellationRequested();

                double x = a + i * stepSize;
                double y = func(x);
                sum += y;

                if (i % (n / 10) == 0)
                {
                    int progress = (i * 100) / n;
                    reportProgress(progress);
                }
            }

            reportProgress(100);

            return sum * stepSize;
        }
    }
}
