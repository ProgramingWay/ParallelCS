using System.Numerics;

public class Fibonacci{
    BigInteger previous { get; set; } = 0;
    BigInteger current { get; set; } = 1;

    private BigInteger GetNext(){
        BigInteger tmp = current;
        current = current + previous;

        previous = tmp;
        return current;
    }

    public void GetFibonacciSequence(BigInteger startAt, BigInteger endAt){
        Console.WriteLine($"\nCiąg Fibonacciego zaczynający się od {startAt} a kończący na {endAt} prezentuje się następująco:\n");
        if (startAt <= 1)
        {
            Console.Write("1: 0, 2: 1, ");
        }
        if (startAt == 2)
        {
            Console.Write("2: 1, ");
        }

        for (int i = 0; i < endAt - 2; i++)
        {
            BigInteger current = GetNext();
            if(i >= startAt - 3)
            {
                Console.Write($"{i + 3}: {current},  ");
            }
        }
        Console.WriteLine("\n");
    }
}