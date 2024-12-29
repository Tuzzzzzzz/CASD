namespace Utilities.Collections;

using System.Collections;
using System.Text;
using Utilities.CollectionInterfaces;


public class MyArrayDeque<T> : IMyDeque<T>
{
    private T[] _items;
    private int _head = 0;
    private int _tail = 0;


    public MyArrayDeque() : this(null) { }

    //initial capacity 16, but one slot with default value
    //it done so that indexes of head and tail are in same range
    public MyArrayDeque(IMyCollection<T> collection)
    {
        int capacity = collection == null ? 17 : collection.Size() + 1;
        _items = new T[capacity];
        AddAll(collection);
    }

    public MyArrayDeque(int capacity)
    {
        if (capacity <= 0) 
            throw new ArgumentException();

        _items = new T[capacity];
    }


    public int Size()
    {
        if (_head <= _tail) 
            return _tail - _head;

        return _tail + _items.Length - _head;
    }

    public bool IsEmpty() => Size() == 0;


    private class Itr : Iterator<T>
    {
        private MyArrayDeque<T> _deque;
        private int _nextIndex;
        private int _indexOfLastReturned = -1;

        public Itr(MyArrayDeque<T> deque)
        {
            _deque = deque;
            _nextIndex = deque._head;
        }

        public bool HasNext()
            => _nextIndex != _deque._tail;

        public T Next()
        {
            if (!HasNext()) 
                throw new InvalidOperationException();

            _indexOfLastReturned = _nextIndex;
            _nextIndex = _deque.Increment(_nextIndex);
            return _deque._items[_indexOfLastReturned];
        }

        public void Remove()
        {
            if (_indexOfLastReturned == -1) 
                throw new InvalidOperationException();

            RemoveAt(_indexOfLastReturned, out bool _shiftBackWas);
            if (_shiftBackWas) _nextIndex = _deque.Decrement(_nextIndex);
            _indexOfLastReturned = -1;
        }

        private void RemoveAt(int index, out bool shiftBackWas)
        {
            if (index == _deque._head)
            {
                shiftBackWas = false;
                _deque.RemoveFirst();
                return;
            }

            if (index == _deque.Decrement(_deque._tail))
            {
                shiftBackWas = true;
                _deque.RemoveLast();
                return;
            }

            for (int i = index; _deque.Increment(i) != _deque._tail; i = _deque.Increment(i))
            {
                _deque._items[i] = _deque._items[_deque.Increment(i)];
            }
            _deque._tail = _deque.Decrement(_deque._tail);
            _deque._items[_deque._tail] = default;
            shiftBackWas = true;
        }
    }

    public Iterator<T> Iterator() => new Itr(this);

    public IEnumerator<T> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public T GetFirst()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException();

        return _items[_head];
    }

    public T GetLast()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException();

        return _items[Decrement(_tail)];
    }

    private int Increment(int index)
    {
        if (index == _items.Length - 1)
            return 0;

        return index + 1;
    }

    private int Decrement(int index)
    {
        if (index == 0)
            return _items.Length - 1;

        return index - 1;
    }

    public void Add(T value)
    {
        int newTail = Increment(_tail);
        if (newTail == _head) 
            AllocateAndAddLast(value);
        else
        {
            _items[_tail] = value;
            _tail = newTail;
        }
    }

    public void AddAll(IMyCollection<T> collection)
    {
        if (collection == null) return;

        foreach (T el in collection) Add(el);
    }

    private void AllocateAndAddLast(T value)
    {
        int newCapacity = _items.Length * 2;
        var newArray = new T[newCapacity];
        int j = 0;
        for (int i = _head; i != _tail; i = Increment(i), j++)
        {
            newArray[j] = _items[i];
        }
        newArray[j] = value;
        _head = 0;
        _tail = j + 1;
        _items = newArray;
    }

    private void AllocateAndAddFirst(T value)
    {
        int newCapacity = _items.Length * 2;
        var newArray = new T[newCapacity];
        int j = 1;
        for (int i = _head; i != _tail; i = Increment(i), j++)
        {
            newArray[j] = _items[i];
        }
        newArray[0] = value;
        _head = 0;
        _tail = j;
        _items = newArray;
    }

    public void AddFirst(T value)
    {
        int newHead = Decrement(_head);
        if (newHead == _tail) AllocateAndAddFirst(value);
        else
        {
            _items[newHead] = value;
            _head = newHead;
        }
    }

    public void AddLast(T value) => Add(value);

    public T RemoveFirst()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException();

        T removedValue = _items[_head];
        _items[_head] = default;
        _head = Increment(_head);
        return removedValue;
    }

    //private void CheckPositionIndex(int index)
    //{
    //    if (IsEmpty())
    //        throw new IndexOutOfRangeException();
    //    if (index < 0 || index >= _array.Length)
    //        throw new IndexOutOfRangeException();
    //    if (_head < _tail)
    //    {
    //        if (index < _head || index >= _tail)
    //            throw new IndexOutOfRangeException();
    //    }
    //    else
    //    {
    //        if (index >= _tail || index < _head)
    //            throw new IndexOutOfRangeException();
    //    }
    //}

    private T RemoveAt(int index)
    {
        if (index == _head) return RemoveFirst();

        if (index == Decrement(_tail)) return RemoveLast();

        var removedValue = _items[index];
        for (int i = index; Increment(i) != _tail; i = Increment(i))
        {
            _items[i] = _items[Increment(i)];
        }
        _tail = Decrement(_tail);
        _items[_tail] = default;
        return removedValue;
    }

    public T RemoveLast()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException();

        int newTail = Decrement(_tail);
        var removedValue = _items[newTail];
        _items[newTail] = default;
        _tail = newTail;
        return removedValue;
    }

    public bool Remove(T value)
    {
        if (value == null)
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (_items[i] == null)
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        else
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (value.Equals(_items[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveAll(IMyCollection<T> collection)
    {
        if (collection == null) return false;

        bool allDeleted = true;
        foreach (T el in collection)
        {
            if (!Remove(el))
                allDeleted = false;
        }
        return allDeleted;
    }

    public bool RemoveFirstOccurence(T value) => Remove(value);

    public bool RemoveLastOccurence(T value)
    {
        int ptr = Size() - 1;
        if (value == null)
        {
            for (int i = Decrement(_tail); ptr != -1; i = Decrement(i), ptr--)
            {
                if (_items[i] == null)
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        else
        {
            for (int i = Decrement(_tail); ptr != -1; i = Decrement(i), ptr--)
            {
                if (value.Equals(_items[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    public void RetainAll(IMyCollection<T> collection)
    {
        if (collection == null)
        {
            Clear();
            return;
        }

        for (int i = _head; i != _tail; i = Increment(i))
        {
            bool delete = true;
            if (_items[i] == null)
            {
                foreach (T el in collection)
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
                foreach (T el in collection)
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

    public bool Contains(T value)
    {
        if (value == null)
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (_items[i] == null) return true;
            }
        }
        else
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (value.Equals(_items[i])) return true;
            }
        }
        return false;
    }

    public bool ContainsAll(IMyCollection<T> collection)
    {
        if (collection == null) return false;

        foreach (T el in collection)
        {
            if (!Contains(el))
                return false;
        }
        return true;
    }

    public T Element() => GetFirst();

    public bool Offer(T value)
    {
        AddLast(value);
        return true;
    }

    public bool TryPeek(out T headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = GetFirst();
        return true;
    }

    public bool TryPool(out T headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = GetFirst();
        RemoveFirst();
        return true;
    }

    public bool OfferFirst(T value)
    {
        AddFirst(value);
        return true;
    }

    public bool OfferLast(T value) => Offer(value);

    public void Push(T value) => AddLast(value);

    public T Pop() => RemoveFirst();

    public bool TryPeekFirst(out T headValue) => TryPeek(out headValue);

    public bool TryPeekLast(out T tailValue)
    {
        if (IsEmpty())
        {
            tailValue = default;
            return false;
        }
        tailValue = GetLast();
        return true;
    }

    public bool TryPoolFirst(out T headValue) => TryPool(out headValue);

    public bool TryPoolLast(out T tailValue)
    {
        if (IsEmpty())
        {
            tailValue = default;
            return false;
        }
        tailValue = GetLast();
        RemoveLast();
        return true;
    }

    public void Clear()
    {
        Array.Clear(_items);
        _head = 0;
        _tail = 0;
    }

    public T[] ToArray()
    {
        var newArray = new T[Size()];
        for (int i = _head, j = 0; i != _tail; i = Increment(i), j++)
        {
            newArray[j] = _items[i];
        }
        return newArray;
    }

    public T[] ToArray(T[] array)
    {
        if (array == null || array.Length < Size()) return ToArray();

        for (int i = _head, j = 0; i != _tail; i = Increment(i), j++)
        {
            array[j] = _items[i];
        }
        return array;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 3 + 2);
        sb.Append('[');
        for (int i = _head; i != _tail; i = Increment(i))
        {
            sb.Append($"{_items[i]}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}