using System;
using System.Collections.Generic;
using System.Threading;
using taskThird.Models;

namespace taskThird.Processing
{
    /// <summary>
    /// Interfejs definiujący metodę przetwarzania obliczeń.
    /// </summary>
    public interface ICalculate
    {
        /// <summary>
        /// Proces obliczeniowy dla zadanej funkcji i przedziałów.
        /// </summary>
        /// <param name="func">Funkcja matematyczna do całkowania.</param>
        /// <param name="intervals">Lista przedziałów dla obliczeń.</param>
        /// <param name="steps">Liczba kroków aproksymacji trapezowej.</param>
        /// <param name="token">Token do obsługi anulowania obliczeń.</param>
        void Process(Func<double, double> func, List<Interval> intervals, int steps, CancellationToken token);
    }
}
