using System.Numerics;
using System.Runtime.CompilerServices;

public class Fibonacci{
    BigInteger previous { get; set; } = 0;
    BigInteger current { get; set; } = 1;

    public void Start() 
    {
        Write("Podaj liczbę ciągu od której ma zacząć się ciąg fibonacciego ");
        BigInteger startingIndex = BigInteger.Parse(Console.ReadLine()!);

        Write("Podaj liczbę ciągu na której ma zostać zakończony ciąg fibonacciego ");
        BigInteger endingIndex = BigInteger.Parse(Console.ReadLine()!);
        GetFibonacciSequence(startAt: startingIndex, endAt: endingIndex);
    }

    private BigInteger GetNext(){
        BigInteger tmp = current;
        current = current + previous;

        previous = tmp;
        return current;
    }

    private void GetFibonacciSequence(BigInteger startAt, BigInteger endAt){
        WriteLine($"\nCiąg Fibonacciego zaczynający się od {startAt} a kończący na {endAt} prezentuje się następująco:\n");
        if (startAt <= 1)
        {
            Write("1: 0, 2: 1, ");
        }
        if (startAt == 2)
        {
            Write("2: 1, ");
        }

        for (int i = 0; i < endAt - 2; i++)
        {
            BigInteger current = GetNext();
            if(i >= startAt - 3)
            {
                Write($"{i + 3}: {current},  ");
            }
        }
        WriteLine("\n");
    }
}