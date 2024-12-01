using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// Klasa do kolejkowania komunikatów i sekwencyjnego wypisywania ich w konsoli.
public static class MessageQueue
{
    private static readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();

    static MessageQueue()
    {
        // Wątek przetwarzający kolejkę komunikatów
        Task.Run(() =>
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                Console.WriteLine(message);
            }
        });
    }

    // Dodaje komunikat do kolejki.
    public static void Enqueue(string message)
    {
        _messageQueue.Add(message);
    }

    // Zamyka kolejkę i oczekuje na przetworzenie wszystkich komunikatów.
    public static void Complete()
    {
        _messageQueue.CompleteAdding();
    }

    // Oczekuje na przetworzenie wszystkich komunikatów.
    public static void Flush()
    {
        while (!_messageQueue.IsCompleted) { }
    }
}
