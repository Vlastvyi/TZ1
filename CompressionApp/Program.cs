using System;
using System.Text;

class Program
{
    static void Main()
    {
        // Тестовые данные
        string[] testCases = {
            "aaabbbcccdde",
            "abc",
            "aaaa",
            "aabbaa",
            "a",
            "aa",
            "aabbcc"
        };

        Console.WriteLine("=== ТЕСТЫ СЖАТИЯ И РАСПАКОВКИ ===\n");

        foreach (string input in testCases)
        {
            string compressed = Compress(input);
            string decompressed = Decompress(compressed);

            Console.WriteLine($"Исходная:     {input}");
            Console.WriteLine($"Сжатая:       {compressed}");
            Console.WriteLine($"Распакованная: {decompressed}");
            Console.WriteLine($"✅ Корректно?  {input == decompressed}\n");
        }
    }

    // Реализуйте метод сжатия
    static string Compress(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var sb = new StringBuilder();
        char currentChar = input[0];
        int count = 1;

        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == currentChar)
            {
                count++;
            }
            else
            {
                AppendToResult(sb, currentChar, count);
                currentChar = input[i];
                count = 1;
            }
        }

        // Не забудь последнюю группу
        AppendToResult(sb, currentChar, count);

        return sb.ToString();
    }

    // Вспомогательный метод — добавляет символ и его количество в результат
    static void AppendToResult(StringBuilder sb, char c, int count)
    {
        if (count == 1)
        {
            sb.Append(c);
        }
        else
        {
            sb.Append(c).Append(count);
        }
    }

    // Реализуйте метод распаковки
    static string Decompress(string compressed)
    {
        if (string.IsNullOrEmpty(compressed)) return compressed;

        var sb = new StringBuilder();
        int i = 0;

        while (i < compressed.Length)
        {
            char c = compressed[i];
            i++;

            // Если следующий символ — цифра, читаем число
            if (i < compressed.Length && char.IsDigit(compressed[i]))
            {
                int count = 0;
                while (i < compressed.Length && char.IsDigit(compressed[i]))
                {
                    count = count * 10 + (compressed[i] - '0');
                    i++;
                }
                sb.Append(c, count);
            }
            else
            {
                // Одиночный символ
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}