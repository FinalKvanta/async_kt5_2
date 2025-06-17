using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async IAsyncEnumerable<int> ProcessFileLinesAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);

        string line;
        int lineNumber = 0;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            int sum = 0;
            bool success = true;
            string errorMessage = null;

            try
            {
                sum = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(numStr => int.Parse(numStr))
                         .Sum();
            }
            catch (FormatException)
            {
                errorMessage = $"Ошибка формата в строке {lineNumber}: '{line}'";
                success = false;
            }
            catch (OverflowException)
            {
                errorMessage = $"Число слишком большое в строке {lineNumber}";
                success = false;
            }

            if (!success)
            {
                Console.WriteLine(errorMessage);
                sum = 0; 
            }

            await Task.Delay(1000); 
            yield return sum;
        }
    }

    static async Task ProcessFileAsync(string filePath)
    {
        try
        {
            await foreach (var sum in ProcessFileLinesAsync(filePath))
            {
                Console.WriteLine($"Сумма чисел в строке: {sum}");
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Файл не найден: {filePath}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка ввода-вывода: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
        }
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("Асинхронная обработка файла с числами");

        var filePath = "numbers.txt";

        try
        {
            if (!File.Exists(filePath))
            {
                await File.WriteAllLinesAsync(filePath, new[] {
                    "1 2 3",
                    "4 5",
                    "10 20 30",
                    "a b c",
                    "100 200"
                });
            }

            await ProcessFileAsync(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.WriteLine("Обработка завершена");
    }
}