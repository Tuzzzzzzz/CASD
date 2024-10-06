using MyStack;

internal class Program
{
    public static void Main(string[] args)
    {
        MyStack<int> stack1 = new MyStack<int>([1, 2, 3 ]);
        List<int> list = new List<int>(stack1);
        foreach (int i in list) Console.WriteLine(i);
    }
}