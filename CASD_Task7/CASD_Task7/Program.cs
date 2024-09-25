namespace CASD.Task7;
using MyVector;
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
                        streamWriter.Write($"{IPs[i]} ");
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
    public static MyVector<string> ParseToIP(string stringData)
    {
        MyVector<string> IPs = new MyVector<string>(
            stringData.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
        );
        for (int i = 0; i < IPs.Size(); i++) {
            if (!IsIP(IPs[i]))
            {
                IPs.RemoveAt(i);
                i--;
            }
        }
        return IPs;
    }

    public static bool IsIP(string IP) {
        if (Count(IP, '.') != 3) return false;

        bool ok = false;
        string currNumber = "";
        foreach (char c in IP)
        {
            if (c == '.')
            {
                ok = IsIPNumber(currNumber);
                currNumber = "";
                if (!ok) break;
            }
            else
            {
                currNumber += c;
            }
        }
        ok = IsIPNumber(currNumber);
        return ok;
    }

    public static bool IsIPNumber(string number)
        => IsNumber(number) 
        && Convert.ToInt32(number) >= 0 
        && Convert.ToInt32(number) <= 255;

    public static bool IsNumber(string data)
    {
        bool isNumber = false;
        foreach (char c in data)
        {
            isNumber = Char.IsDigit(c);
            if (!isNumber) break;
        }
        return isNumber;
    }

    public static int Count(string data, char symbol)
    {
        int cnt = 0;
        foreach (char c in data)
            if (c == symbol) cnt++;
        return cnt;
    }
}