public class Calka
{
    double a = 0;
    double b = 2;
    
    // int ile = 100;
    int ile = 200;

    public void Start()
    {
        double sumaFinalna = CalkaProstokat();

        WriteLine($"Wartość całki metodą prostokątów (n={ile}): {sumaFinalna}");
    }
    private double Funkcja(double x)
    {
        return 0.5 * x;
    }

    private double CalkaProstokat()
    {
        double h = (b - a) / ile;
        double suma = 0;
        for (int i = 0; i < ile; i++)
        {
            double x = a + i * h;
            suma += h * Funkcja(x);
        }
        return suma;
    }
}