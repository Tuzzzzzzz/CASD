namespace MyVector;

internal class MyVector<T>
{
    private T[] ItemData;
    private int ItemCnt;
    private int CapacityIncrement;

    public MyVector(int initialCapacity, int capacityIncrement)
    {
        if (initialCapacity < 0 || capacityIncrement < 0) throw new ArgumentOutOfRangeException();

        ItemCnt = 0;
        CapacityIncrement = capacityIncrement;
        ItemData = new T[initialCapacity];
    }

    public MyVector(int initialCapacity) : this(initialCapacity, 0) { }

    public MyVector() : this(10) { }

    public MyVector(T[] array)
    {
        if (array == null) throw new ArgumentNullException();

        ItemCnt = array.Length;
        CapacityIncrement = 0;
        ItemData = (T[])array.Clone();
    }

    public MyVector(MyVector<T> vector) : this(vector.ToArray()) { }

    public int Size() => ItemCnt;

    public bool IsEmpty() => Size() == 0;

    public void Add(T value)
    {
        if (ItemCnt == ItemData.Length)
        {
            AllocateMemory();
        }
        ItemData[ItemCnt++] = value;
    }

    public void AddAll(T[] array)
    {
        if (array == null) throw new ArgumentNullException();

        foreach (T value in array) Add(value);
    }

    public void Insert(int index, T value)
    {
        if ((index < 0) || (index >= Size())) throw new IndexOutOfRangeException();

        if (ItemCnt == ItemData.Length)
        {
            AllocateMemory();
        }
        for (int i = Size(); i > index; i--) 
        {
            ItemData[i] = ItemData[i - 1];
        }
        ItemData[index] = value;
        ItemCnt++;
    }

    public void InsertAll(int index, T[] array) 
    {
        if (array == null) throw new ArgumentNullException();

        for (int i = array.Length - 1; i >= 0; i--)
            Insert(index, array[i]);
    }

    public void AllocateMemory()
    {
        T[] newItemData = (CapacityIncrement == 0) ?
            new T[ItemData.Length * 2] : new T[ItemData.Length + CapacityIncrement];

        Array.Copy(ItemData, newItemData, ItemData.Length);
        ItemData = newItemData;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Size()) throw new IndexOutOfRangeException();
            return ItemData[index];
        }
        set
        {
            if (index < 0 || index >= Size()) throw new IndexOutOfRangeException();
            ItemData[index] = value;
        }
    }

    public T FirstItem
    {
        get
        {
            if(IsEmpty()) throw new IndexOutOfRangeException();
            return ItemData[0];
        }
        set
        {
            if (IsEmpty()) throw new IndexOutOfRangeException();
            ItemData[0] = value;
        }
    }

    public T LastItem
    {
        get
        {
            if (IsEmpty()) throw new IndexOutOfRangeException();
            return ItemData[Size()-1];
        }

        set
        {
            if (IsEmpty()) throw new IndexOutOfRangeException();
            ItemData[Size()-1] = value;
        }
    }

    public T RemoveAt(int index)
    {
        if ((index < 0) || (index >= Size())) throw new IndexOutOfRangeException();

        T removedItem = ItemData[index];
        for (int i = index; i < Size() - 1; i++)
        {
            ItemData[i] = ItemData[i + 1];
        }
        ItemData[--ItemCnt] = default(T);
        return removedItem;
    }

    public T Remove() => RemoveAt(Size() - 1);

    public bool Remove(T value)
    {
        int index = IndexOf(value);
        if (index == -1) return false;
        RemoveAt(index);
        return true;
    }

    public bool RemoveAll(T[] array)
    {
        if (array == null) throw new ArgumentNullException();

        bool allRemoved = false;
        foreach (T value in array)
        {
            allRemoved = Remove(value);
        }
        return allRemoved;
    }

    public void RemoveRange(int begin, int end)
    {
        if ((begin < 0) || (end > Size())) throw new IndexOutOfRangeException();

        for (int k = end - begin - 1; k > 0; k--)
        {
            RemoveAt(begin);
        }
    }

    public void RetainAll(T[] array)
    {
        if (array == null) throw new ArgumentNullException();

        for (int i = 0; i < Size(); i++)
        {
            bool isWaste = true;
            foreach (T value in array)
            {
                if (IsIndexOf(i, value))
                {
                    isWaste = false;
                    break;
                }
            }
            if (isWaste) {
                RemoveAt(i);
                i--;
            };
        }
    }

    public T[] SubList(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex >= Size()
            || toIndex >= Size() || fromIndex > toIndex) throw new IndexOutOfRangeException();

        T[] newArray = new T[toIndex - fromIndex];
        Array.Copy(ItemData, fromIndex, newArray, 0, newArray.Length);
        return newArray;
    }


    public bool Contains(T value)
        => IndexOf(value) != -1;

    public bool ContainsAll(T[] array)
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

    private bool IsIndexOf(int index, T value)
        => Array.IndexOf(ItemData, value, index, 1) != -1;

    public int IndexOf(T value)
        => Array.IndexOf(ItemData, value, 0, Size());

    public int LastIndexOf(T value)
        => Array.LastIndexOf(ItemData, value, Size() - 1, Size());

    public void Clear()
    {
        Array.Clear(ItemData, 0, Size());
        ItemCnt = 0;
    }

    public T[] ToArray()
    {
        T[] newItemData = new T[Size()];
        Array.Copy(ItemData, newItemData, Size());
        return newItemData;
    }

    public void ToArray(T[] array)
    {
        if (array == null) throw new ArgumentNullException();
        if (array.Length < Size()) throw new ArgumentException();

        Array.Copy(ItemData, array, Size());
    }
    public override string ToString()
        => $"[{string.Join(", ", ToArray())}]";
}
