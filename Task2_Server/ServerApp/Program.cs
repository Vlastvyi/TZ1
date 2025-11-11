using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== ТЕСТЫ СТАТИЧЕСКОГО СЕРВЕРА ===\n");

        // Запускаем несколько читателей и писателей
        var tasks = new Task[10];

        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() => Reader());
        }

        for (int i = 5; i < 10; i++)
        {
            tasks[i] = Task.Run(() => Writer(i - 4));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"\n✅ Итоговое значение count: {Server.GetCount()}");
    }

    static void Reader()
    {
        for (int i = 0; i < 3; i++)
        {
            int current = Server.GetCount();
            Console.WriteLine($"Читатель {Task.CurrentId}: прочитал {current}");
            Thread.Sleep(100); // имитация работы
        }
    }

    static void Writer(int value)
    {
        for (int i = 0; i < 2; i++)
        {
            Server.AddToCount(value);
            Console.WriteLine($"Писатель {Task.CurrentId}: добавил {value}, теперь count = {Server.GetCount()}");
            Thread.Sleep(200); // имитация работы
        }
    }
}

// ⬇️ Реализация статического сервера с потокобезопасным доступом
static class Server
{
    private static int _count = 0;
    private static readonly object _lock = new();

    public static int GetCount()
    {
        lock (_lock)
        {
            return _count;
        }
    }

    public static void AddToCount(int value)
    {
        lock (_lock)
        {
            _count += value;
        }
    }
}