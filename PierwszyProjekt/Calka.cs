public class Calka
{
    double a = 0;
    double b = 2;
    int ile = 150;
    bool lewaStrona = true;

    public void Start()
    {
        double suma = CalkaProstokat();

        WriteLine("Wartość całki metodą prostokątów (n={0}, {1}): {2}",
        arg0: ile,
        arg1: lewaStrona ? "lewy bok" : "prawy bok",
        arg2: suma);
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
            if (lewaStrona)
            {
                suma += Funkcja(x) * h;
            }
            else
            {
                suma += Funkcja(x + h) * h;
            }
        }
        return suma;
    }
}