using MyCollections.Map;

public class Program
{
    public static void Main(string[] args)
    {
        var map = new TreeMap<string, int>();
        map["a"] = 1;
        map["b"] = -3;
        map["c"] = 4;
        map["d"] = -100;

        Console.WriteLine(map);

        var it = map.Iterator();
        while (it.HasNext())
        {
            if(it.Next().Item1 == "b")
                it.Remove();
        }

        Console.WriteLine(map);
    }
}