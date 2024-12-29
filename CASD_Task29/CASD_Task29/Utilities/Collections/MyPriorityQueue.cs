namespace Utilities.Collections;

using System.Collections;
using System.Text;
using Utilities.CollectionInterfaces;


public class LambdaComparer<T> : IComparer<T>
{
    private Func<T?, T?, int> cmpFunc;
    public LambdaComparer(Func<T?, T?, int> cmpFunc)
    {
        this.cmpFunc = cmpFunc;
    }
    public int Compare(T? a, T? b) => cmpFunc(a, b);
}


public class MyPriorityQueue<T> : IMyQueue<T>
    where T : IComparable<T>
{
    private T[] _items;
    private int _size = 0;
    private IComparer<T> _comparer;


    public int Size() => _size;

    public bool IsEmpty() => _size == 0;


    public MyPriorityQueue(int initialCapacity, IComparer<T>? comparer)
    {
        if (comparer == null) _comparer = Comparer<T>.Default;
        else _comparer = comparer;
        _items = new T[initialCapacity];
    }

    public MyPriorityQueue(int initialCapacity, Func<T?, T?, int>? cmpFunc)
    {
        if (cmpFunc == null) _comparer = Comparer<T>.Default;
        else _comparer = new LambdaComparer<T>(cmpFunc);
        _items = new T[initialCapacity];
    }

    public MyPriorityQueue(int initialCapacity) : this(initialCapacity, (IComparer<T>?)null) { }

    public MyPriorityQueue() : this(11) { }

    public MyPriorityQueue(IMyCollection<T> collection, IComparer<T>? comparer)
    {
        if (comparer == null)
            _comparer = Comparer<T>.Default;
        else
            _comparer = comparer;

        if (collection == null)
            _items = new T[16];
        else
        {
            _items = collection.ToArray();
            _size = collection.Size();
        }

        OrderAll();
    }

    public MyPriorityQueue(IMyCollection<T> collection, Func<T?, T?, int>? cmpFunc)
    {
        if (cmpFunc == null)
            _comparer = Comparer<T>.Default;
        else
            _comparer = new LambdaComparer<T>(cmpFunc);

        if (collection == null)
            _items = new T[16];
        else
        {
            _items = collection.ToArray();
            _size = collection.Size();
        }

        OrderAll();
    }

    public MyPriorityQueue(IMyCollection<T> collection) : this(collection, (IComparer<T>?)null) { }


    private class Itr : Iterator<T>
    {
        private MyPriorityQueue<T> _queue;

        private MyPriorityQueue<T> _copyOfQueue;

        private T _lastReturned;

        private bool _deletionWas = true;

        public Itr(MyPriorityQueue<T> queue)
        {
            _queue = queue;
            _copyOfQueue = new MyPriorityQueue<T>(queue);
        }

        public bool HasNext() => _copyOfQueue.Size() > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException();

            _deletionWas = false;
            _lastReturned = _copyOfQueue.RemoveMax();
            return _lastReturned;
        }

        public void Remove()
        {
            if (_deletionWas)
                throw new InvalidOperationException();

            _deletionWas = true;
            _queue.Remove(_lastReturned);
        }
    }

    public Iterator<T> Iterator() => new Itr(this);

    public virtual IEnumerator<T> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();) yield return it.Next();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    private void UnorderedAdd(T value)
    {
        if (_size == _items.Length)
        {
            AllocateMemory();
        }
        _items[_size++] = value;
    }

    public void Add(T value)
    {
        UnorderedAdd(value);
        Emerge(_size - 1);
    }

    public void AddAll(IMyCollection<T> collection)
    {
        if (collection == null) return;

        foreach (T el in collection.ToArray()) Add(el);
    }

    private void AllocateMemory()
    {
        T[] newItems = (_size < 64) ?
            new T[_items.Length * 2] : new T[(int)(_items.Length * 1.5)];

        Array.Copy(_items, newItems, _items.Length);
        _items = newItems;
    }

    private T UnorderedRemove()
    {
        if (IsEmpty())
            throw new InvalidOperationException();

        T removedItem = _items[_size - 1];
        _items[_size - 1] = default;
        _size--;
        return removedItem;
    }

    private T RemoveAt(int index)
    {
        if (index < 0 || index >= _size)
            throw new IndexOutOfRangeException();

        T removedItem = _items[index];
        if (_size > 1)
        {
            _items[index] = UnorderedRemove();
            if (_size > 1) Heapify(index);
        }
        else
        {
            _items[index] = default;
            _size--;
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

    public bool RemoveAll(IMyCollection<T> collection)
    {
        if (collection == null) return false;

        bool allRemoved = false;
        foreach (T el in collection.ToArray())
        {
            int itemIndex = IndexOf(el);
            allRemoved = itemIndex != -1;
            if (itemIndex != -1)
            {
                RemoveAt(itemIndex);
            }
        }
        return allRemoved;
    }

    public void RetainAll(IMyCollection<T> collection)
    {
        if (collection == null)
        {
            Clear();
            return;
        }

        for (int i = 0; i < _size; i++)
        {
            bool delete = true;
            foreach (T el in collection.ToArray())
            {
                if (_items[i].Equals(el))
                {
                    delete = false;
                    break;
                }
            }
            if (delete)
            {
                RemoveAt(i);
                i--;
            };
        }
    }

    public T Element() 
        => IsEmpty() ? throw new InvalidOperationException() : _items[0];

    public bool Offer(T value)
    {
        Add(value);
        return true;
    }

    public bool TryPeek(out T headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = _items[0];
        return true;
    }

    public bool TryPool(out T headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = RemoveMax();
        return true;
    }

    private T RemoveMax() => RemoveAt(0);

    private void OrderAll()
    {
        for (int i = _size / 2; i >= 0; i--) Heapify(i);
    }

    private void Emerge(int valueIndex)
    {
        if (valueIndex == 0) return;

        int parentIndex = (valueIndex - 1) / 2;
        while (valueIndex >= 0)
        {
            if (_comparer.Compare(_items[valueIndex], _items[parentIndex]) > 0)
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

            if (leftChildIndex < _size && _comparer.Compare(_items[leftChildIndex], _items[largestItemIndex]) > 0)
                largestItemIndex = leftChildIndex;

            if (rightChildIndex < _size && _comparer.Compare(_items[rightChildIndex], _items[largestItemIndex]) > 0)
                largestItemIndex = rightChildIndex;

            if (largestItemIndex == valueIndex) break;

            Swap(valueIndex, largestItemIndex);

            valueIndex = largestItemIndex;
        }
    }

    private void Swap(int index1, int index2)
    {
        if (index1 < 0 || index2 < 0 || index1 >= _size || index2 >= _size)
            throw new IndexOutOfRangeException();

        T tmp = _items[index1];
        _items[index1] = _items[index2];
        _items[index2] = tmp;
    }

    public bool Contains(T value) => IndexOf(value) != -1;

    public bool ContainsAll(IMyCollection<T> collection)
    {
        if (collection == null) throw new ArgumentNullException();

        bool allThere = true;
        foreach (T el in collection.ToArray())
        {
            if (!Contains(el))
            {
                allThere = false;
                break;
            }
        }
        return allThere;
    }

    private int IndexOf(T value)
        => Array.IndexOf(_items, value, 0, _size);

    public void Clear()
    {
        Array.Clear(_items);
        _size = 0;
    }

    public T[] ToArray()
    {
        T[] newItemData = new T[_size];
        Array.Copy(_items, newItemData, _size);
        return newItemData;
    }

    public T[] ToArray(T[] array)
    {
        if (array == null || array.Length < _size) return ToArray();

        Array.Copy(_items, array, _size);
        return array;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 3 + 2);
        sb.Append('[');
        foreach (T item in _items)
        {
            sb.Append($"{item}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}
