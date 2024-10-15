using Requests;

internal class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Введите количество шагов добавления заявок в очередь:");
            int countOfSteps = Convert.ToInt32(Console.ReadLine());
            if (countOfSteps < 0) throw new ArgumentException("Количество шагов должно быть положительным");
            if (countOfSteps == 0)
            {
                Console.WriteLine("Заявок не было, т.к. количество шагов 0");
                return;
            }
            using (StreamWriter writer = new StreamWriter("log.txt"))
            {
                RequestHandler requestHandler = new RequestHandler(writer);
                (Request lastRemovedRequest, int time) = requestHandler.Handle(countOfSteps);

                Console.WriteLine(
                    $"Информация о последней заявке:" +
                    $"\nВремя: {time}" +
                    $"\nНомер: {lastRemovedRequest.Number}" +
                    $"\nПриоритет: {lastRemovedRequest.Priority}" +
                    $"\nШаг: {lastRemovedRequest.Step}"
                );
            }
        }
        catch (Exception ex) { 
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}