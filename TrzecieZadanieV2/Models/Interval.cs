namespace taskThird.Models
{
    public class Interval
    {
        public double Start { get; }
        public double End { get; }

        public Interval(double start, double end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"[{Start}, {End}]";
        }
    }
}
