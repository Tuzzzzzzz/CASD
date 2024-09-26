namespace MyStack;
using MyVector;

internal class MyStack<T> : MyVector<T>
{
    public MyStack(int initialCapacity, int capacityIncrement)
        : base(initialCapacity, capacityIncrement) { }

    public MyStack(int initialCapacity) : base(initialCapacity) { }

    public MyStack() : base() { }

    public MyStack(T[] array) : base(array) { }

    public MyStack(MyVector<T> vector) : base(vector) { }

    public void Push(T value) => Add(value);

    public T Pop() => Remove();

    public T Peek() => LastItem;

    public int Search(T item)
    {
        int index = LastIndexOf(item);
        if (index == -1) return -1;
        return Size() - index;
    }
}
