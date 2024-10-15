using System.Collections;

namespace MyPriorityQueue;

internal class LambdaComparer<T> : IComparer<T>
{
    private Func<T?, T?, int> cmpFunc;
    public LambdaComparer(Func<T?, T?, int> cmpFunc)
    {
        this.cmpFunc = cmpFunc;
    }
    public int Compare(T? a, T? b) => cmpFunc(a, b);
}

internal class MyPriorityQueue<T> : IEnumerable<T>, ICloneable
    where T: IComparable<T>
{
    private T[] itemData;
    private int count = 0;
    private IComparer<T> comparer;

    public int Count => count;

    public bool Empty => count == 0;

    public T Element => Empty ? throw new ArgumentOutOfRangeException() : itemData[0];

    public MyPriorityQueue(int initialCapacity, IComparer<T>? comparer)
    {
        if (comparer == null) this.comparer = Comparer<T>.Default;
        else this.comparer = comparer;
        itemData = new T[initialCapacity];
    }

    public MyPriorityQueue(int initialCapacity, Func<T?, T?, int>? cmpFunc)
    {
        if (cmpFunc == null) comparer = Comparer<T>.Default;
        else comparer = new LambdaComparer<T>(cmpFunc);
        itemData = new T[initialCapacity];
    }

    public MyPriorityQueue(int initialCapacity) : this(initialCapacity, (IComparer<T>?)null) { }

    public MyPriorityQueue() : this(11) { }

    public MyPriorityQueue(T[]? array, IComparer<T>? comparer) 
    {
        if (array == null) throw new ArgumentNullException();
        if (comparer == null) this.comparer = Comparer<T>.Default;
        else this.comparer = comparer;
        itemData = (T[])array.Clone();
        count = array.Length;
        OrderAll();
    }

    public MyPriorityQueue(T[]? array, Func<T?, T?, int>? cmpFunc)
    {
        if (array == null) throw new ArgumentNullException();
        if (cmpFunc == null) comparer = Comparer<T>.Default;
        else comparer = new LambdaComparer<T>(cmpFunc);
        itemData = (T[])array.Clone();
        count = array.Length;
        OrderAll();
    }

    public MyPriorityQueue(T[]? array) : this(array, (IComparer<T>?)null) { }

    public MyPriorityQueue(IEnumerable<T>? collection)
    {
        if (collection == null) throw new ArgumentNullException();
        foreach (T item in collection) UnorderedAdd(item);
        OrderAll();
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < count; i++) yield return itemData[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public object Clone() => new MyPriorityQueue<T>(ToArray());

    private void UnorderedAdd(T value)
    {
        if (count == itemData.Length)
        {
            AllocateMemory();
        }
        itemData[count++] = value;
    }
    public void Add(T value)
    {
        UnorderedAdd(value);
        Emerge(count - 1);
    }

    public void AddAll(T[]? array)
    {
        if (array == null) throw new ArgumentNullException();
        foreach (T value in array) Add(value);
    }

    private void AllocateMemory()
    {
        T[] newItemData = (count < 64) ?
            new T[itemData.Length * 2] : new T[(int)(itemData.Length * 1.5)];

        Array.Copy(itemData, newItemData, itemData.Length);
        itemData = newItemData;
    }

    private T UnorderedRemove() {
        if (Empty) throw new IndexOutOfRangeException();
        T removedItem = itemData[count-1];
        itemData[count - 1] = default;
        count--;
        return removedItem;
    }

    private T RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        T removedItem = itemData[index];
        if (count > 1)
        {
            itemData[index] = UnorderedRemove();
            if (count > 1) Heapify(index);
        }
        else { 
            itemData[index] = default;
            count--;
        }
        return removedItem;
    }

    public bool Remove(T value)
    {
        int index = IndexOf(value);
        if (index == -1) return false;
        RemoveAt(index);
        return true;
    }

    public bool RemoveAll(T[]? array)
    {
        if (array == null) throw new ArgumentNullException();

        bool allRemoved = false;
        foreach (T value in array)
        {
            int itemIndex = IndexOf(value);
            allRemoved = itemIndex != -1;
            if (itemIndex != -1)
            {
                RemoveAt(itemIndex);
            }
        }
        return allRemoved;
    }

    public void RetainAll(T[]? array)
    {
        if (array == null) throw new ArgumentNullException();

        for (int i = 0; i < count; i++)
        {
            bool isWaste = true;
            foreach (T value in array)
            {
                if (itemData[i].Equals(value))
                {
                    isWaste = false;
                    break;
                }
            }
            if (isWaste)
            {
                RemoveAt(i);
                i--;
            };
        }
    }

    public bool Offer(T value)
    {
        Add(value);
        return true;
    }

    public T? Peek()
    {
        if (Empty) return default;
        return itemData[0];
    }

    public T? Pool()
    {
        if (Empty) return default;
        return RemoveMax();
    }

    private T RemoveMax() => RemoveAt(0);

    private void OrderAll()
    {
        for (int i = count / 2; i >= 0; i--) Heapify(i);
    }

    private void Emerge(int valueIndex)
    {
        if (valueIndex == 0) return;
        int parentIndex = (valueIndex - 1) / 2;
        while (valueIndex >= 0)
        {
            if (comparer.Compare(itemData[valueIndex], itemData[parentIndex]) > 0)
            {
                Swap(valueIndex, parentIndex);

                valueIndex = parentIndex;
                parentIndex = (valueIndex - 1) / 2;
            }
            else break;
        }
    }

    private void Heapify(int valueIndex)
    {
        while (true)
        {
            int leftChildIndex = valueIndex * 2 + 1;
            int rightChildIndex = valueIndex * 2 + 2;
            int largestItemIndex = valueIndex;

            if (leftChildIndex < count && comparer.Compare(itemData[leftChildIndex], itemData[largestItemIndex]) > 0)
                largestItemIndex = leftChildIndex;

            if (rightChildIndex < count && comparer.Compare(itemData[rightChildIndex], itemData[largestItemIndex]) > 0)
                largestItemIndex = rightChildIndex;

            if (largestItemIndex == valueIndex) break;

            Swap(valueIndex, largestItemIndex);

            valueIndex = largestItemIndex;
        }
    }

    private void Swap(int index1, int index2)
    {
        if (index1 < 0 || index2 < 0 || index1 >= count || index2 >= count)
            throw new ArgumentOutOfRangeException();
        T tmp = itemData[index1];
        itemData[index1] = itemData[index2];
        itemData[index2] = tmp;
    }

    public bool Contains(T value)
        => IndexOf(value) != -1;

    public bool ContainsAll(T[]? array)
    {
        if (array == null) throw new ArgumentNullException();

        bool allThere = true;
        foreach (T value in array)
        {
            if (!Contains(value))
            {
                allThere = false;
                break;
            }
        }
        return allThere;
    }

    private int IndexOf(T value)
        => Array.IndexOf(itemData, value, 0, count);

    public void Clear()
    {
        Array.Clear(itemData, 0, count);
        count = 0;
    }

    public T[] ToArray()
    {
        T[] newItemData = new T[count];
        Array.Copy(itemData, newItemData, count);
        return newItemData;
    }

    public void ToArray(T[]? array)
    {
        if (array == null) throw new ArgumentNullException();
        if (array.Length < count) throw new ArgumentException();

        Array.Copy(itemData, array, count);
    }

    public override string ToString()
        => $"[{string.Join(", ", (IEnumerable<T>)ToArray())}]";
}
