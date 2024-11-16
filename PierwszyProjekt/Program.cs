using System.Numerics;

Fibonacci fibonacci = new Fibonacci();

Calka calka = new Calka();


int ile = 100;

Console.Write("\nWybierz co chcesz zrobic:");
Console.WriteLine("  1 - Fibonacci");
Console.WriteLine("  2 - Calki");
Console.WriteLine("  3 - Wnioski z zadań");
int wyborOpcji = int.Parse(Console.ReadLine()!);
switch (wyborOpcji)
{
    case 1:
        {
            FibonacciWybor();
            break;
        }
    case 2:
        {
            Calkowanie();
            break;
        }
    case 3:
        {
            Wnioski.WszystkieWnioski();
            break;
        }
}

void FibonacciWybor()
{ 
    Console.Write("Podaj liczbę ciągu od której ma zacząć się ciąg fibonacciego ");
    BigInteger startingIndex = BigInteger.Parse(Console.ReadLine()!);

    Console.Write("Podaj liczbę ciągu na której ma zostać zakończony ciąg fibonacciego ");
    BigInteger endingIndex = BigInteger.Parse(Console.ReadLine()!);
    fibonacci.GetFibonacciSequence(startAt: startingIndex, endAt: endingIndex);
}

void Calkowanie()
{ 
        Console.Write("Wybierz metodę obliczeń (L - lewa, P - prawa, S - środek): ");
    char wyborMetody = Console.ReadKey().KeyChar;
    Calka.MetodaObliczen metoda = wyborMetody switch
    {
        'l' => Calka.MetodaObliczen.Lewa,
        'L' => Calka.MetodaObliczen.Lewa,
        'p' => Calka.MetodaObliczen.Prawa,
        'P' => Calka.MetodaObliczen.Prawa,
        's' => Calka.MetodaObliczen.Srodek,
        'S' => Calka.MetodaObliczen.Srodek,
        _ => throw new ArgumentException("Niepoprawny wybór metody")
    };

    Console.Write("\nWybierz element aproksymacji (P - prostokąt, T - trapez): ");
    char wyborElementu = Console.ReadKey().KeyChar;
    Calka.ElementAproksymacji element = wyborElementu switch
    {
        'p' => Calka.ElementAproksymacji.Prostokąt,
        'P' => Calka.ElementAproksymacji.Prostokąt,
        't' => Calka.ElementAproksymacji.Trapez,
        'T' => Calka.ElementAproksymacji.Trapez,
        _ => throw new ArgumentException("Niepoprawny wybór elementu")
    };


    Console.Write("\nWybierz ilosc elementow aproksymacji (1-1000): ");
    int ileElementow = int.Parse(Console.ReadLine()!);

    double sumaFinalna = calka.CalkaProstokatTrapez(metoda, element, ileElementow);

    Console.WriteLine($"\nWartość całki metodą prostokątów (n={ile}): {sumaFinalna}");
}