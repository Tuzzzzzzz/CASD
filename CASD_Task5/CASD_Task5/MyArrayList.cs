namespace MyArrayList;
internal class MyArrayList<T>
{
    private T[] storage;
    private int size;

    public MyArrayList() : this(10) { }

    public MyArrayList(T[] array) : this()
    {
        foreach (T element in array) Add(element);
    }

    public MyArrayList(int capacity)
    {
        if (capacity < 0) throw new ArgumentException();
        storage = new T[capacity];
        size = 0;
    }

    public int Size() => size;

    public bool isEmpty() => size == 0;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= size) throw new IndexOutOfRangeException();
            return storage[index];
        }
        set
        {
            if (index < 0 || index >= size) throw new IndexOutOfRangeException();
            storage[index] = value;
        }
    }

    public void Add(T value)
    {
        if (size == storage.Length) AllocateMemory();
        storage[size] = value;
        size++;
    }

    public void AddAll(T[] array)
    {
        foreach (T item in array) Add(item);
    }

    public void Insert(int index, T value)
    {
        if (index < 0 || index >= size) throw new IndexOutOfRangeException();
        if (size == storage.Length) AllocateMemory();
        for (int i = size; i > index; i--)
        {
            storage[i] = storage[i - 1];
        }
        storage[index] = value;
        size++;
    }

    public void InsertAll(int index, T[] array)
    {
        for (int i = size - 1; i >= 0; i--) Insert(index, array[i]);
    }

    private void AllocateMemory()
    {
        T[] newStorage = new T[(int)(storage.Length * 1.5) + 1];
        Array.Copy(storage, newStorage, storage.Length);
        storage = newStorage;
    }

    public bool RemoveAll(T[] array)
    {
        bool allDeleted = true;
        foreach(T item in array) 
            if(!Remove(item)) allDeleted = false;
        return allDeleted;
    }

    public bool Remove(object obj)
    {
        int index = IndexOf(obj);
        if (index == -1) return false;
        RemoveAt(index);
        return true;
    }

    public T RemoveAt(int index)
    {
        if (index < 0 || index >= size) throw new IndexOutOfRangeException();
        T removeElem = storage[index];
        for (int i = index; i < size - 1; i++)
        {
            storage[i] = storage[i + 1];
        }
        storage[--size] = default;
        return removeElem;
    }

    public void RetainAll(T[] array)
    {
        for(int i = 0; i < size; i++)
        {
            bool toDelete = true;
            foreach(T item in array)
            {
                if (storage[i].Equals(item))
                {
                    toDelete = false;
                    break;
                }
            }
            if (toDelete)
            {
                RemoveAt(i);
                i--;
            }
        }
    }

    public T[] SubList(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex >= size
            || toIndex >= size || fromIndex > toIndex) throw new ArgumentException();
        T[] newArray = new T[toIndex - fromIndex];
        Array.Copy(storage, toIndex, newArray, 0, newArray.Length);
        return newArray;
    }

    public int IndexOf(object obj) 
    { 
        for (int i = 0; i < size; i++)
            if(obj.Equals(storage[i])) return i;
        return -1;
    }

    public int LastIndexOf(object obj)
    {
        for (int i = size-1; i >= 0; i--)
            if (obj.Equals(storage[i])) return i;
        return -1;
    }

    public bool Contains(object value) => IndexOf(value) != -1;

    public bool ContainsAll(T[] array)
    {
        foreach(T item in array) 
            if(!Contains(item)) return false;
        return true;
    }

    public T[] ToArray()
    {
        T[] newStorage = new T[size];
        Array.Copy(storage, newStorage, size);
        return newStorage;
    }

    public void ToArray(T[] array, int itemCnt, int storageBegin = 0, int arrayBegin = 0)
    {
        Array.Copy(storage, storageBegin, array, arrayBegin, itemCnt);
    }

    public void Clear()
    {
        Array.Clear(storage, 0, size);
        size = 0;
    }

    public override string ToString()
        => $"[{string.Join(", ", ToArray())}]";
}
