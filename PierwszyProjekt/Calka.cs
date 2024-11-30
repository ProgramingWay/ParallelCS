public class Calka
{

    public enum MetodaObliczen { Lewa, Prawa, Srodek }
    public enum ElementAproksymacji { Prostokąt, Trapez }

    double a = 0;
    double b = 2;
    
    int ile;

    private double Funkcja(double x)
    {
        return 0.5 * x;
    }   

    public double CalkaProstokatTrapez(MetodaObliczen metoda, ElementAproksymacji element, int ile)
    {
        double h = (b - a) / ile;
        double suma = 0;
        for (int i = 0; i < ile; i++)
        {
            double x1 = a + i * h;
            double x2 = a + (i + 1) * h;
            double fx1 = Funkcja(x1);
            double fx2 = Funkcja(x2);

            switch (element)
            {
                case ElementAproksymacji.Prostokąt:
                    switch (metoda)
                    {
                        case MetodaObliczen.Lewa:
                            suma += h * fx1;
                            break;
                        case MetodaObliczen.Prawa:
                            suma += h * fx2;
                            break;
                        case MetodaObliczen.Srodek:
                            suma += h * Funkcja((x1 + x2) / 2);
                            break;
                    }
                    break;
                case ElementAproksymacji.Trapez:
                    suma += h * (fx1 + fx2) / 2;
                    break;
            }
        }
        return suma;
    }
    
}