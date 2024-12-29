using Utilities.Collections;
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(new MyVector<int>(new MyFixedArrayList<int>([1, 2, 3, 4, 5])));
    }
}
