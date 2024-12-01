/// <summary>
/// Klasa odpowiedzialna za obliczenia całki metodą trapezów.
/// </summary>
public static class IntegralCalculator
{
    public static double Calculate(Func<double, double> function, double start, double end, int steps)
    {
        double stepSize = (end - start) / steps;
        double integral = 0;

        for (int i = 0; i < steps; i++)
        {
            double x1 = start + i * stepSize;
            double x2 = x1 + stepSize;
            integral += (function(x1) + function(x2)) * stepSize / 2;
        }

        return integral;
    }
}
