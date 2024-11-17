using MyCollections.Set;

public class Program
{
    public static void Main(string[] args)
    {
        var set = new TreeSet<int>();
        set.Add(1);
        set.Add(-9);
        set.Add(4);
        set.Add(-5);

        Console.WriteLine(set);

        var rSet = set.ReverseSet();

        Console.WriteLine(rSet);

        Console.WriteLine(set-rSet.ReverseSet());
    }
}