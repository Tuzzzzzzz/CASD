namespace MyStack;
using MyVector;
using System.Collections;

internal class MyStack<T> : MyVector<T>, IEnumerable<T>
{
    public MyStack(int initialCapacity, int capacityIncrement)
        : base(initialCapacity, capacityIncrement) { }

    public MyStack(int initialCapacity) : base(initialCapacity) { }

    public MyStack() : base() { }

    public MyStack(T[] array) : base(array) { }

    public MyStack(IEnumerable<T> collection) : base(collection) { }

    public void Push(T value) => Add(value);

    public T Pop() => Remove();

    public T Peek() => LastItem;

    public int Search(T item)
    {
        int index = LastIndexOf(item);
        if (index == -1) return -1;
        return Size() - index;
    }

    public override IEnumerator<T> GetEnumerator()
    {
        for(int i = Size()-1; i >= 0; i--) yield return base[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
