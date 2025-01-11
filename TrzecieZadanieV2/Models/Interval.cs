namespace taskThird.Models
{
    /// <summary>
    /// Model reprezentujący przedział wartości liczbowych (od start do end).
    /// </summary>
    public class Interval
    {
        /// <summary>
        /// Punkt początkowy przedziału.
        /// </summary>
        public double Start { get; }

        /// <summary>
        /// Punkt końcowy przedziału.
        /// </summary>
        public double End { get; }

        /// <summary>
        /// Inicjalizuje nowy obiekt przedziału.
        /// </summary>
        /// <param name="start">Punkt początkowy przedziału.</param>
        /// <param name="end">Punkt końcowy przedziału.</param>
        public Interval(double start, double end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Reprezentacja przedziału jako string.
        /// </summary>
        /// <returns>Przedział w formacie [start, end].</returns>
        public override string ToString()
        {
            return $"[{Start}, {End}]";
        }
    }
}
