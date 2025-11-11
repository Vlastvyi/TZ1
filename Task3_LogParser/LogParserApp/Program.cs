using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string[] inputFiles = { "input1.txt", "input2.txt" };
        string outputFileName = "output.txt";
        string problemsFileName = "problems.txt";

        List<string> validLines = new();
        List<string> invalidLines = new();

        foreach (string inputFile in inputFiles)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"❌ Файл не найден: {inputFile}");
                continue;
            }

            string[] lines = File.ReadAllLines(inputFile);

            foreach (string line in lines)
            {
                var result = ParseLine(line);
                if (result != null)
                {
                    validLines.Add(result);
                }
                else
                {
                    invalidLines.Add(line);
                }
            }
        }

        // Запись в output.txt
        File.WriteAllLines(outputFileName, validLines);
        Console.WriteLine($"✅ Успешно записано {validLines.Count} строк в {outputFileName}");

        // Запись в problems.txt
        if (invalidLines.Count > 0)
        {
            File.WriteAllLines(problemsFileName, invalidLines);
            Console.WriteLine($"⚠️  Невалидных строк: {invalidLines.Count}, записаны в {problemsFileName}");
        }
        else
        {
            Console.WriteLine("✅ Все строки валидны, problems.txt не создан.");
        }
    }

    static string ParseLine(string line)
    {
        // Проверка на пустую строку
        if (string.IsNullOrWhiteSpace(line)) return null;

        // Формат 1: DD.MM.YYYY HH:MM:SS.SSS LEVEL Сообщение
        var match1 = Regex.Match(line, @"^(\d{2}\.\d{2}\.\d{4})\s+(\d{2}:\d{2}:\d{2}\.\d+)\s+(INFORMATION|INFO|WARN|ERROR|DEBUG)\s+(.+)$");
        if (match1.Success)
        {
            string date = match1.Groups[1].Value; // DD.MM.YYYY
            string time = match1.Groups[2].Value; // HH:MM:SS.SSS
            string level = NormalizeLevel(match1.Groups[3].Value);
            string message = match1.Groups[4].Value.Trim();

            // Парсим метод из сообщения (если есть)
            string method = ExtractMethod(message);
            string remainingMessage = RemoveMethodFromMessage(message, method);

            return $"{date}\t{time}\t{level}\t{method}\t{remainingMessage}";
        }

        // Формат 2: YYYY-MM-DD HH:MM:SS.SSS [LEVEL][ID]|Метод| Сообщение
        var match2 = Regex.Match(line, @"^(\d{4}-\d{2}-\d{2})\s+(\d{2}:\d{2}:\d{2}\.\d+)\s+\[(INFORMATION|INFO|WARN|ERROR|DEBUG)\]\[\d+\]\|([^|]+)\|\s*(.+)$");
        if (match2.Success)
        {
            string date = match2.Groups[1].Value; // YYYY-MM-DD → преобразуем в DD.MM.YYYY
            string time = match2.Groups[2].Value; // HH:MM:SS.SSS
            string level = NormalizeLevel(match2.Groups[3].Value);
            string method = match2.Groups[4].Value.Trim();
            string message = match2.Groups[5].Value.Trim();

            // Преобразуем дату в DD.MM.YYYY
            string formattedDate = FormatDate(date);

            return $"{formattedDate}\t{time}\t{level}\t{method}\t{message}";
        }

        return null; // Не удалось распарсить
    }

    static string NormalizeLevel(string level)
    {
        return level.ToUpper() switch
        {
            "INFORMATION" => "INFO",
            "WARN" => "WARN",
            "ERROR" => "ERROR",
            "DEBUG" => "DEBUG",
            _ => "INFO" // По умолчанию
        };
    }

    static string ExtractMethod(string message)
    {
        // Ищем шаблон |Метод|
        int start = message.IndexOf('|');
        if (start == -1) return "DEFAULT";

        int end = message.IndexOf('|', start + 1);
        if (end == -1) return "DEFAULT";

        return message.Substring(start + 1, end - start - 1).Trim();
    }

    static string RemoveMethodFromMessage(string message, string method)
    {
        int start = message.IndexOf('|');
        if (start == -1) return message;

        int end = message.IndexOf('|', start + 1);
        if (end == -1) return message;

        return message.Substring(end + 1).TrimStart();
    }

    static string FormatDate(string yyyyMmDd)
    {
        var parts = yyyyMmDd.Split('-');
        if (parts.Length == 3)
        {
            return $"{parts[2]}.{parts[1]}.{parts[0]}"; // DD.MM.YYYY
        }
        return yyyyMmDd; // Возвращаем как есть, если не удалось преобразовать
    }
}