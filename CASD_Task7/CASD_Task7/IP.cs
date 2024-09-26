using MyVector;

internal static class IP
{
    public static MyVector<string> ParseToIP(string stringData)
    {
        MyVector<string> IPs = new MyVector<string>(
            stringData.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
        );
        for (int i = 0; i < IPs.Size(); i++)
        {
            if (!IsIP(IPs[i]))
            {
                IPs.RemoveAt(i);
                i--;
            }
        }
        return IPs;
    }

    public static bool IsIP(string IP)
    {
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

    private static bool IsNumber(string data)
    {
        bool isNumber = false;
        foreach (char c in data)
        {
            isNumber = Char.IsDigit(c);
            if (!isNumber) break;
        }
        return isNumber;
    }

    private static int Count(string data, char symbol)
    {
        int cnt = 0;
        foreach (char c in data)
            if (c == symbol) cnt++;
        return cnt;
    }
}