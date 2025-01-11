using System;
using System.Threading;

namespace taskThird.Utils
{
    /// <summary>
    /// Klasa zawierająca metodę do obliczania całki metodą trapezów.
    /// </summary>
    public static class MainCalculator
    {
        /// <summary>
        /// Oblicza wartość przybliżoną całki oznaczonej metodą trapezów.
        /// </summary>
        /// <param name="func">Funkcja matematyczna do całkowania.</param>
        /// <param name="a">Początek przedziału całkowania.</param>
        /// <param name="b">Koniec przedziału całkowania.</param>
        /// <param name="n">Liczba kroków aproksymacji.</param>
        /// <param name="token">Token do obsługi anulowania obliczeń.</param>
        /// <param name="reportProgress">Metoda wywoływana do raportowania postępu obliczeń.</param>
        /// <returns>Przybliżona wartość całki oznaczonej.</returns>
        public static double Calculate(Func<double, double> func, double a, double b, int n, CancellationToken token, Action<int> reportProgress)
        {
            double stepSize = (b - a) / n; // Rozmiar jednego kroku
            double sum = 0;

            // Początkowe wartości na końcach przedziału
            double firstValue = func(a);
            double lastValue = func(b);
            sum += 0.5 * (firstValue + lastValue);

            // Iteracyjne dodawanie wartości
            for (int i = 1; i < n; i++)
            {
                token.ThrowIfCancellationRequested(); // Sprawdzenie, czy obliczenia zostały anulowane

                double x = a + i * stepSize;
                double y = func(x);
                sum += y;

                // Raportowanie postępu co 10%
                if (i % (n / 10) == 0)
                {
                    int progress = (i * 100) / n;
                    reportProgress(progress);
                }
            }

            // Raportowanie zakończenia obliczeń
            reportProgress(100);

            return sum * stepSize; // Wynik całki
        }
    }
}
