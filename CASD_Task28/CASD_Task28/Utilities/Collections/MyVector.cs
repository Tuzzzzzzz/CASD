namespace Utilities.Collections;

using System.Collections;
using System.Text;
using CollectionInterfaces;


public class MyVector<T>: IMyList<T>
{
    private T[] _items;
    private int _size = 0;
    private int _capacityIncrement = 0;


    public MyVector(int initialCapacity, int capacityIncrement)
    {
        if (initialCapacity <= 0 || capacityIncrement < 0) 
            throw new ArgumentOutOfRangeException();

        _capacityIncrement = capacityIncrement;
        _items = new T[initialCapacity];
    }

    public MyVector(int initialCapacity) : this(initialCapacity, 0) { }

    public MyVector() : this(10) { }

    public MyVector(IMyCollection<T> collection)
    {
        if (collection == null)
        {
            _items = new T[10];
            return;
        }

        _items = collection.ToArray();
        _size = collection.Size();
    }


    public T this[int index]
    {
        get 
        {
            CheckPositionIndex(index);
            return _items[index];
        }

        set
        {
            CheckPositionIndex(index);
            _items[index] = value;
        }
    }

    private class ListItr : ListIterator<T>
    {
        private MyVector<T> _vector;

        private int _nextIndex = 0;

        private int _indexOfLastReturned = -1;

        public ListItr(MyVector<T> vector, int index)
        {
            vector.CheckPositionIndex(index);
            _vector = vector;
            _nextIndex = index;
        }

        public void Add(T value)
        {
            _vector.Add(_nextIndex++, value);
            _indexOfLastReturned = -1;
        }

        public bool HasNext() => _nextIndex < _vector.Size();

        public bool HasPrevious() => _nextIndex >= 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException();

            _indexOfLastReturned = _nextIndex++;
            return _vector[_indexOfLastReturned];
        }

        public int NextIndex() => _nextIndex;

        public T Previous()
        {
            if (!HasPrevious())
                throw new InvalidOperationException();

            _indexOfLastReturned = _nextIndex--;
            return _vector[_indexOfLastReturned];
        }

        public int PreviousIndex() => _nextIndex;

        public void Remove()
        {
            if (_indexOfLastReturned == -1) 
                throw new InvalidOperationException();

            _vector.RemoveAt(_indexOfLastReturned);
            _nextIndex--;
            _indexOfLastReturned = -1;
        }

        public void Set(T value)
        {
            if (_indexOfLastReturned == -1)
                throw new InvalidOperationException();

            _vector[_indexOfLastReturned] = value;
        }
    }

    public ListIterator<T> ListIterator(int index) => new ListItr(this, index);

    public ListIterator<T> ListIterator() => ListIterator(0);

    public Iterator<T> Iterator() => ListIterator();

    public virtual IEnumerator<T> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();) yield return it.Next();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void CheckPositionIndex(int index)
    {
        if (index < 0 || index >= _size)
            throw new IndexOutOfRangeException();
    }

    public int Size() => _size;

    public bool IsEmpty() => Size() == 0;

    public void Add(T value)
    {
        if (Size() == _items.Length) AllocateMemory();

        _items[_size++] = value;
    }

    public void AddAll(IMyCollection<T> collection) 
        => AddAll(Size() - 1, collection);

    public void Add(int index, T value)
    {
        CheckPositionIndex(index);

        if (Size() == _items.Length)
        {
            AllocateMemory();
        }
        for (int i = Size(); i > index; i--) 
        {
            _items[i] = _items[i - 1];
        }
        _items[index] = value;
        _size++;
    }

    public void AddAll(int index, IMyCollection<T> collection) 
    {
        if (collection == null) return;

        var array = collection.ToArray();
        for (int i = array.Length - 1; i >= 0; i--) Add(index, array[i]);
    }

    public void AllocateMemory()
    {
        T[] newItems = (_capacityIncrement == 0) ?
            new T[_items.Length * 2] : new T[_items.Length + _capacityIncrement];

        Array.Copy(_items, newItems, _items.Length);
        _items = newItems;
    }

    public void RemoveAt(int index)
    {
        CheckPositionIndex(index);

        for (int i = index; i < Size() - 1; i++)
        {
            _items[i] = _items[i + 1];
        }
        _items[--_size] = default;
    }

    public T Remove() { 
        T removedItem = _items[Size() - 1];
        RemoveAt(Size() - 1); 
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
            allRemoved = Remove(el);
        }
        return allRemoved;
    }

    public void RemoveRange(int begin, int end)
    {
        CheckPositionIndex(begin);
        CheckPositionIndex(end);
        if (begin > end) throw new ArgumentException();

        for (int k = end - begin - 1; k > 0; k--)
        {
            RemoveAt(begin);
        }
    }

    public void RetainAll(IMyCollection<T> collection)
    {
        if (collection == null)
        {
            Clear();
            return;
        }

        for (int i = 0; i < Size(); i++)
        {
            bool delete = true;
            if (_items[i] == null)
            {
                foreach (T el in collection.ToArray())
                {
                    if (el == null)
                    {
                        delete = false;
                        break;
                    }
                }
            }
            else
            {
                foreach (T el in collection.ToArray())
                {
                    if (_items[i]!.Equals(el))
                    {
                        delete = false;
                        break;
                    }
                }
            }
            if (delete) RemoveAt(i);
        }
    }

    public IMyList<T> SubList(int fromIndex, int toIndex)
    {
        CheckPositionIndex(fromIndex);
        CheckPositionIndex(toIndex);
        if (fromIndex > toIndex) throw new IndexOutOfRangeException();

        var newVector = new MyVector<T>(toIndex - fromIndex);
        for (int i = fromIndex; i < toIndex; i++) 
            newVector.Add(_items[i]);
        return newVector;
    }

    public bool Contains(T value) => IndexOf(value) != -1;

    public bool ContainsAll(IMyCollection<T> collection)
    {
        if (collection == null) return false;

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

    public int IndexOf(T value)
    {
        if (IsEmpty()) return -1;

        if (value == null)
        {
            for(int i = 0; i < Size(); i++)
            {
                if (_items[i] == null) return i;
            }
        }
        else
        {
            for (int i = 0; i < Size(); i++)
            {
                if (value.Equals(_items[i])) return i;
            }
        }
        return -1;
    }

    public int LastIndexOf(T value)
    {
        if (IsEmpty()) return -1;

        if (value == null)
        {
            for (int i = Size() - 1; i > 0; i--)
            {
                if (_items[i] == null) return i;
            }
        }
        else
        {
            for (int i = Size() - 1; i > 0; i--)
            {
                if (value.Equals(_items[i])) return i;
            }
        }
        return -1;
    }

    public void Clear()
    {
        Array.Clear(_items);
        _size = 0;
    }

    public T[] ToArray()
    {
        T[] newArray = new T[Size()];
        Array.Copy(_items, newArray, Size());
        return newArray;
    }

    public T[] ToArray(T[] array)
    {
        if (array == null || array.Length < Size()) return ToArray();

        Array.Copy(_items, array, Size());
        return array;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 3 + 2);
        sb.Append("[");
        for (int i = 0; i < Size(); i++)
        {
            sb.Append($"{_items[i]}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}
