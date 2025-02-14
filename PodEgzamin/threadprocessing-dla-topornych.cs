using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

// LOGIKA KODU: Bierzemy 2 wartosci, boundA oraz boundB (symbolizuja one wartosci x1 oraz x2 na osi X)
// bierzemy resolution (na ile chcemy podzielic ten przedzial, wartosc 0.5 to dzielenie na pol, 0.1 na dzisiec czesci)
// obliczamy Y podstawiajac do rownania np "2*x" x1 oraz x2. Jesli x1 = 0 (poczatek boundA) a x2 = 0.1 (boundA + rozdzielczosc)...
// ...to y(x1) = 2 * 0 = 0,     y(x2) = 2 * 0.1 = 0.2
// obliczamy teraz pole trapezu ze wzoru ((a + b) * h) / 2.
// a = y(x1),  b = y(x2),   h = resolution
// obliczylismy pole dla malego wycinka funkcji, teraz trzeba to zrobic miliony razy zeby precyzyjnie obliczyc...
// ...pole pod wykresem. Bo calka to po prostu pole pod wykresem funkcji
// Potem lecimy w ten sposob ze x1 = x2, do x2 dodajemy resolution i przesunelismy sie do kolenego trapeza
// Nizej objasnienie co i jak





// Tworzymy interfejs, do ktorego bedziemy mogli przypisac jakas funkcje taka jak TPLProcessing albo ThreadProcessing
// (dziala tylko ThreadProcessing)
interface IProcessingMethod
{
    // wrzucamy tutaj integral i Action<int>. Action to jest po prostu takie miejsce, ktore bedzie nam wyrzucac int
    // z zamknietego pudelka (petla for) do nas
    public void Calculate(Integral integral, Action<int> repProgress);
}




class TPLProcessing : IProcessingMethod // <- rozszerzamy klase o interfejs
{

    // Taki samy void jak w interfejsie w celu poprawnego dzialania (mozemy sobie potem wybrac czy uzywamy TPL czy TP)
    // Tyle ze nie bedziemy uzywac TPL bo nie dziala w tym kodzie na razie
    public void Calculate(Integral integral, Action<int> repProgress)
    {
        integral.Calculate(progress => {
            Console.WriteLine($"Progress: {progress}");
        });
    }
}

// Klasa TP ktora dzieli zadania na watki i przyspiesza prace
class ThreadProcessing : IProcessingMethod // <- tu to samo, rozszerzamy
{
    public void Calculate(Integral integral, Action<int> repProgress) // powtarzamy Calculate takie same jak w interfejsie
    {
        // tutaj z klasy integral przekazanym w wywolaniu funkcji bierzemy wartosci poprzez integral.jakas_wartosc
        // i od razu obliczamy ile krokow bedziemy wykonywac przy obliczaniu pola pod wykresem (calki)
        int steps = (int)((integral._boundB - integral._boundA) / integral._resolution);


        int numThreads = 12;
        int completed = 0;

        // tutaj tworzymy tablice z double i threads
        // W Double zapisujemy obliczenia z jednego watku (watek nr 1 rzuca obliczenia do localAreas[1] (wiem ze 0 cicho))
        // a watek nr 2 wrzuca obliczenia do localAreas[2] (wiem ze 1, cicho)
        // Threads tworzy nam watki ktore potem beda obliczac nam pole
        double[] localAreas = new double[numThreads];
        Thread[] threads = new Thread[numThreads];

        // For dla kazdego watku
        for (int t = 0; t < threads.Length; t++)
        {
            // ThreadIndex do podzielenia na indeksy wartosci do obliczen
            int threadIndex = t;

            // Tutaj sa tworzone watki. Nie musimy czekac na zakonczenie pracy watku, sa one tworzone baaardzo szybko
            // za pomoca for.
            threads[t] = new Thread(() =>
            {
                // Tu obliczamy poczatek i koniec dla kazdego watku
                // watek1 moze miec przedzial od 0 do 10
                // watek2 od 11 do 20
                // watek3 od 21 do 30 itd
                // chodzi o to zeby jeden watek nie obliczal od 0 do 100
                int start = threadIndex * steps / numThreads;
                int end = (threadIndex + 1) * steps / numThreads;
                double localArea = 0;

                // tutaj obliczanie calki
                for(int i = start; i < end; i++)
                {
                    double x1 = integral._boundA + i * integral._resolution;
                    double x2 = x1 + integral._resolution;

                    // tu mamy _function. ono zostaje przypisane przy tworzeniu integral, i wybiery jest przez to
                    // wzor, np 2 * x, 2*x^2 itp
                    double y1 = integral._function.Calculate(x1);
                    double y2 = integral._function.Calculate(x2);

                    // wzor na pole trapeza
                    localArea += (y1 + y2) * integral._resolution / 2;
                        
                }
                // przypisujemy obliczenia do tablicy z wszystkich watkow
                localAreas[threadIndex] = localArea;

                // idealnie byloby tu lock (_lock){} ale to spowalnia mocno prace watkow wiec zrezygnowalem z tego
            });
        }

        // cos ala for ale dla kazdego z elementow. tutaj startujemy wszystkie watki, nie jestem pewnien co robi Join
        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) thread.Join();

        double finalArea = localAreas.Sum();
        Console.WriteLine(finalArea.ToString());
        
    }
}
// Swoja droga to powinienem to zrobic tak, zeby nie przepisywac poraz drugi tego samego
// Bo w integral jest to samo ale bez watkow



// klasa integral
class Integral
{
    public readonly float _resolution;
    public readonly float _boundA;
    public readonly float _boundB;
    public readonly float _result;
    public readonly int _choice;
    object _lock = new object(); // tutaj niewykorzystywane ale boje sie usunac XD

    // tutaj tworzymy interfejs
    public ICalculateMethod _function;

    public Integral()
    {
        Console.WriteLine("Welcome to Integral calculator!" +
            "Please select calculation:\n" +
            "1) 2x^2\n" +
            "2) x^2 + 3\n" +
            "3) 5x^2 - 2x + 10");
        _choice = int.Parse(Console.ReadLine());

        // tutaj dla _function przypisujemy wybrana przez uzytkownika funkcje f(x)
        _function = _choice switch
        {
            1 => new FirstMethod(),
            2 => new SecondMethod(),
            3 => new ThirdMethod(),
            _ => //throw new Exception("ERROR: Selected method is not valid")
            new ZeroMethod()
            // tutaj dalem prosta metode zeby sprawdzac poprawnosc dzialania kodu
        };

        Console.WriteLine("Select bound (start and end)");
        _boundA = float.Parse(Console.ReadLine());
        _boundB = float.Parse(Console.ReadLine());

        Console.WriteLine("Select Resolution");
        _resolution = float.Parse(Console.ReadLine());
    }

    /*public void Calculate(Action<int> progress)
    {
        int steps = (int)((_boundB - _boundA) / _resolution);

        double area = 0;
        int completed = 0;

        for (int i = 0; i <= steps; i++)
        {
            double x1 = _boundA + i * _resolution;
            double x2 = x1 + _resolution;

            double y1 = _function.Calculate(x1);
            double y2 = _function.Calculate(x2);
            area += (y1 + y2) * _resolution / 2;
            lock (_lock)
            {
                completed++;
                if (completed % (steps * 100) == 0)
                {
                    progress(completed);
                }
            }

        }
        Console.WriteLine(area);
    }*/


    // Znowu pojawia sie Action<int>, przypominam ze to nam wyrzuca progress z petli for
    public void Calculate(Action<int> progress)
    {
        // to samo co w ThreadProcessing
        int steps = (int)((_boundB - _boundA) / _resolution);
        double area = 0;
        int completed = 0;

        for (int i = 0; i <= steps; i++)
        {
            double x1 = _boundA + i * _resolution;
            double x2 = x1 + _resolution;

            double y1 = _function.Calculate(x1);
            double y2 = _function.Calculate(x2);
            area += (y1 + y2) * _resolution / 2;
            completed++;


            if (completed % (steps / 100) == 0) // Raportowanie co 1%
            {
                progress((int)((double)completed / steps * 100)); // wyrzucenie statusu w %, (Action<int> progress){...
            }
        }

        Console.WriteLine(area);
    }

    // buba, nie dziala
    public void CalculateParralel()
    {
        int steps = (int)((_boundB - _boundA) / _resolution);
        double area = 0;

        Parallel.For(0, steps, i =>
        {

        });



        /*Parallel.For(0, steps, i =>
        {
            double x1 = _boundA + i * _resolution;
            double x2 = x1 + _resolution;

            double y1 = _function.Calculate(x1);
            double y2 = _function.Calculate(x2);
            lock (_lock)
            {
                area += (y1 + y2) * _resolution / 2;
            }

        });*/
        Console.WriteLine($"Area: {area}");
    }
}


// interfejs do wyboru wzoru, dalej sa wzory
public interface ICalculateMethod
{
    double Calculate(double x);
}

public class FirstMethod : ICalculateMethod
{
    public double Calculate(double x) => 2* Math.Pow(x, 2);
}
public class SecondMethod : ICalculateMethod
{
    public double Calculate(double x) => Math.Pow(x, 2) + 3;
}

public class ThirdMethod : ICalculateMethod
{
    public double Calculate(double x) => 5 * Math.Pow(x, 2) - 2 * x + 10;
}

public class ZeroMethod : ICalculateMethod
{
    public double Calculate(double x) => x * 2;
}


class main
{
    public static void Main(string[] args)
    {
        // zeby sprawdzic czas dzialania, kliknijcie 3, 0, 100 i 0,00001
        Integral integral = new Integral();

        // Stopwatch do pomiaru czasu
        Stopwatch sw1 = new Stopwatch();

        sw1.Start(); // Wlaczamy zegarek

        // a tu chyba mozna bez try catch to zrobic, ale jes jak jes
        // poza tym tutaj zwykle obliczenia, bez zadnych przyspieszaczy
        try
        {
            integral.Calculate(progress =>
            {
                Console.WriteLine($"Progress: {progress}");
            });
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        sw1.Stop();

        /*IProcessingMethod i = new TPLProcessing();
        Stopwatch sw2 = new Stopwatch();

        sw2.Start();
        i.Calculate(integral);
        sw2.Stop();*/


        IProcessingMethod threadprocess = new ThreadProcessing();
        Stopwatch sw3 = new Stopwatch();


        // tutaj przyspieszacz w postaci threadprocessing, nie dziala progress (wyswietlanie w % postepu)
        // bo jak daje zeby raportowalo progress to ta metoda dziala wolniej (blokuje sie kazdy watek po obliczeniu)
        sw3.Start();
        try
        {
            threadprocess.Calculate(integral, progress =>
            {
                Console.WriteLine($"Progress: {progress}");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine (ex.ToString());
        }
        sw3.Stop();

        Console.WriteLine($"sw1: {sw1.Elapsed.ToString()}\n" +
            //$"sw2: {sw2.Elapsed.ToString()}\n" +
            $"sw3: {sw3.Elapsed.ToString()}");
    }
}