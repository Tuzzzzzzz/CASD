using RPN;

internal class Program
{
    public static void Main(string[] args)
    {
        Dictionary<string, double> vars = null;
        double? result = null;
        while (true) {
            Console.WriteLine("Введите выражение для вычисления:");
            string? input = Console.ReadLine();
            if (input == "" || input == null) {
                Console.WriteLine("Конец выполнения программы");
                break;
            }
            try
            {
                result = SmartRPN.CalculateWithInputedVariables(input, vars);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                result = null;
            }
            finally {
                if (result != null)
                {
                    Console.WriteLine($"result = {result}");
                    vars = new() { { "result", (double)result } };
                }
                else vars = null;
                Console.WriteLine(); 
            }
        }
    }
}