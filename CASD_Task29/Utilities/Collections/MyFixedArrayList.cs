namespace Utilities.Collections;

using CollectionInterfaces;
using System.Collections;
using System.Text;


public class MyFixedArrayList<T> : IMyList<T>
{
    private T[] _items;


    public MyFixedArrayList(T[] array)
    {
        if (array == null)
            throw new ArgumentNullException();

        _items = array;
    }


    public int Size() => _items.Length;

    public bool IsEmpty() => _items.Length == 0;


    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Size())
                throw new IndexOutOfRangeException();
            return _items[index];
        }

        set
        {
            if (index < 0 || index >= Size())
                throw new IndexOutOfRangeException();
            _items[index] = value;
        }
    }

    public void Add(int index, T value)
    {
        throw new NotSupportedException();
    }

    public void Add(T value)
    {
        throw new NotSupportedException();
    }

    public void AddAll(int index, IMyCollection<T> collection)
    {
        throw new NotSupportedException();
    }

    public void AddAll(IMyCollection<T> collection)
    {
        throw new NotSupportedException();
    }

    public bool Remove(T value)
    {
        throw new NotSupportedException();
    }

    public bool RemoveAll(IMyCollection<T> collection)
    {
        throw new NotSupportedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    public void RetainAll(IMyCollection<T> collection)
    {
        throw new NotSupportedException();
    }

    public bool Contains(T value)
    {
        if (value == null)
        {
            foreach (var item in _items)
                if (item == null) return true;
        }
        else
        {
            foreach (var item in _items)
                if (value!.Equals(item)) return true;
        }
        return false;
    }

    public bool ContainsAll(IMyCollection<T> collection)
    {
        if (collection == null) return false;

        foreach (T el in collection)
            if (!Contains(el)) return false;

        return true;
    }

    private class ListItr : ListIterator<T>
    {
        public T[] _array;

        private int _nextIndex;

        private int _indexOfLastReturned = -1;

        public ListItr(T[] array, int index)
        {
            if (index < 0 || index >= array!.Length) 
                throw new ArgumentOutOfRangeException();

            _array = array;
            _nextIndex = index;
        }

        public void Add(T value)
        {
            throw new NotSupportedException();
        }

        public bool HasNext() => _nextIndex < _array.Length;

        public bool HasPrevious() => _nextIndex >= 0;

        public T Next()
        {
            if (!HasNext()) 
                throw new InvalidOperationException();

            return _array[_nextIndex++];
        }

        public int NextIndex() => _nextIndex;

        public T Previous()
        {
            if (!HasPrevious()) 
                throw new InvalidOperationException();

            return _array[_nextIndex--];
        }

        public int PreviousIndex() => _nextIndex;

        public void Remove()
        {
            throw new NotSupportedException();
        }

        public void Set(T value)
        {
            if (_indexOfLastReturned == -1) 
                throw new InvalidOperationException();

            _array[_indexOfLastReturned] = value;
        }

    }

    public Iterator<T> Iterator() => ListIterator();

    public ListIterator<T> ListIterator() => new ListItr(_items, 0);

    public ListIterator<T> ListIterator(int index) => new ListItr(_items, index);

    public IEnumerator<T> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T value)
    {
        if (IsEmpty()) return -1;

        if (value == null)
        {
            for (int i = 0; i < _items.Length; i++)
                if (_items[i] == null) return i;
        }
        else
        {
            for (int i = 0; i < _items.Length; i++)
                if (value.Equals(_items[i])) return i;
        }
        return -1;
    }

    public int LastIndexOf(T value)
    {
        if (IsEmpty()) return -1;

        if (value == null)
        {
            for (int i = _items.Length - 1; i >= 0; i--)
                if (_items[i] == null) return i;
        }
        else
        {
            for (int i = _items.Length - 1; i >= 0; i--)
                if (value.Equals(_items[i])) return i;
        }
        return -1;
    }

    public IMyList<T> SubList(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _items.Length 
            || toIndex < 0 || toIndex >= _items.Length 
            || toIndex < fromIndex) throw new ArgumentOutOfRangeException();

        int newLength = toIndex - fromIndex;
        var newArray = new T[newLength];
        Array.Copy(_items, fromIndex, newArray, 0, newLength);

        return new MyFixedArrayList<T>(newArray);
    }

    public void Clear()
    {
        throw new NotSupportedException();
    }

    public T[] ToArray()
    {
        var newArray = new T[_items.Length];
        Array.Copy(_items, newArray, _items.Length); 
        return newArray;
    }

    public T[] ToArray(T[] array)
    {
        if (array == null || array.Length < _items.Length) return ToArray();

        Array.Copy(_items, array, _items.Length);
        return array;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 3 + 2);
        sb.Append('[');
        foreach (T item in _items) sb.Append($"{item}, ");
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}