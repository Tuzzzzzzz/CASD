using MyVector;
using static IP;
internal class Program
{
    public static void Main(string[] args)
    {
        string fileInputName = "input.txt";
        string fileOutputName = "output.txt";

        string stringData;
        try
        {
            using (var streamReader = new StreamReader(fileInputName))
            {
                stringData = streamReader.ReadToEnd();
            }
            if (stringData != "")
            {
                MyVector<string> IPs = ParseToIP(stringData);
                using (var streamWriter = new StreamWriter(fileOutputName))
                {
                    for(int i = 0; i < IPs.Size(); i++)
                        streamWriter.WriteLine(IPs[i]);
                }
                Console.WriteLine($"Программа успешно завершила работу с результатом: {IPs}");
            }
            else
            {
                Console.WriteLine("Файл для ввода данных пуст");
            }

        }
        catch (FileNotFoundException ex) {
            Console.WriteLine($"Файл не найден: {ex.Message}"); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
        }
    }
}