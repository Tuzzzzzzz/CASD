using MyStack;

internal class Program
{
    public static void Main(string[] args)
    {
        MyStack<int> stack1 = new MyStack<int>([1, 2, 3]);
        MyStack<int> stack = new MyStack<int>(stack1);
        for (int i = 0; i < 5; i++)
        {
            stack.Push(i);
            Console.WriteLine($"stack.Push({i})");
            Console.WriteLine($"stack.ToString(): {stack}\n");
        }
        Console.WriteLine($"stack.Pop(): {stack.Pop()}\n");
        Console.WriteLine($"stack.Peek(): {stack.Peek()}\n");
        Console.WriteLine($"stack.IsEmpty(): {stack.IsEmpty()} \n");
        Console.WriteLine($"stack.Search(2): {stack.Search(2)} \n");
        Console.WriteLine($"stack.Search(10): {stack.Search(10)}");
    }
}