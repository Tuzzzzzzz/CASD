using System.Collections;

namespace MyArrayDeque;

class MyArrayDeque<T> : IEnumerable<T?>
{
    private T?[] _array;
    private int _head = 0;
    private int _tail = 0;

    //initial capacity 16, but one slot with default value
    //it done so that indexes of head and tail are in same range
    public MyArrayDeque()
    {
        _array = new T?[17];
    }

    public MyArrayDeque(T?[]? array)
    {
        if (array == null)
            _array = new T?[17];
        else 
            _array = (T?[])array.Clone();
    }

    public MyArrayDeque(int numElements)
    {
        if (numElements < 0) throw new ArgumentException();
        _array = new T[numElements];
    }

    public int Size()
    {
        if (_head <= _tail) return _tail - _head;
        return _tail + _array.Length - _head;
    }

    public bool IsEmpty() => Size() == 0;

    private class Itr : MyIterator<T?>
    {
        private MyArrayDeque<T> _deque;
        private int _next;
        private int _indexOfLastReterned;
        bool _deletionWas = false;

        public Itr(MyArrayDeque<T> deque)
        {
            _deque = deque;
            _next = deque._head;
        }

        public bool HasNext()
            => _next != _deque._tail;

        public T? Next()
        {
            if (!HasNext()) throw new Exception();

            _deletionWas = false;
            _indexOfLastReterned = _next;
            _next = _deque.Increment(_next);
            return _deque._array[_indexOfLastReterned];
        }

        public void Remove()
        {
            if (_deletionWas) throw new Exception();

            RemoveAt(_indexOfLastReterned, out bool _stopNext);
            if (_stopNext) _next = _deque.Decrement(_next);
            _deletionWas = true;
        }

        private void RemoveAt(int index, out bool stopNext)
        {
            if (index == _deque._head)
            {
                stopNext = false;
                _deque.RemoveFirst();
                return;
            }

            if (index == _deque.Decrement(_deque._tail))
            {
                stopNext = true;
                _deque.RemoveLast();
                return;
            }

            for (int i = index; _deque.Increment(i) != _deque._tail; i = _deque.Increment(i))
            {
                _deque._array[i] = _deque._array[_deque.Increment(i)];
            }
            _deque._tail = _deque.Decrement(_deque._tail);
            _deque._array[_deque._tail] = default;
            stopNext = true;
        }
    }

    public MyIterator<T?> Iterator() => new Itr(this);

    public IEnumerator<T?> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    T? GetFirst()
    {
        if (IsEmpty()) throw new Exception();
        return _array[_head];
    }

    T? GetLast()
    {
        if (IsEmpty()) throw new Exception();
        return _array[Decrement(_tail)];
    }

    private int Increment(int index)
    {
        if (index == _array.Length - 1)
            return 0;
        return index + 1;
    }

    private int Decrement(int index)
    {
        if (index == 0)
            return _array.Length - 1;
        return index - 1;
    }

    public void Add(T? value)
    {
        int newTail = Increment(_tail);
        if (newTail == _head) AllocateAndAddLast(value);
        else
        {
            _array[_tail] = value;
            _tail = newTail;
        }
    }

    public void AddAll(T[]? array)
    {
        if (array == null) return;
        foreach (T item in array) Add(item);
    }

    private void AllocateAndAddLast(T? value)
    {
        int newCapacity = _array.Length * 2;
        var newArray = new T?[newCapacity];
        int j = 0;
        for (int i = _head; i != _tail; i = Increment(i), j++)
        {
            newArray[j] = _array[i];
        }
        newArray[j] = value;
        _head = 0;
        _tail = j + 1;
        _array = newArray;
    }

    private void AllocateAndAddFirst(T? value)
    {
        int newCapacity = _array.Length * 2;
        var newArray = new T?[newCapacity];
        int j = 1;
        for (int i = _head; i != _tail; i = Increment(i), j++)
        {
            newArray[j] = _array[i];
        }
        newArray[0] = value;
        _head = 0;
        _tail = j;
        _array = newArray;
    }

    public void AddFirst(T? value)
    {
        int newHead = Decrement(_head);
        if (newHead == _tail) AllocateAndAddFirst(value);
        else
        {
            _array[newHead] = value;
            _head = newHead;
        }
    }

    public void AddLast(T? value) => Add(value);

    public T? RemoveFirst()
    {
        if (IsEmpty()) throw new Exception();
        var removedValue = _array[_head];
        _array[_head] = default;
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

    private T? RemoveAt(int index)
    {
        if (index == _head) return RemoveFirst();

        if (index == Decrement(_tail)) return RemoveLast();

        var removedValue = _array[index];
        for (int i = index; Increment(i) != _tail; i = Increment(i))
        {
            _array[i] = _array[Increment(i)];
        }
        _tail = Decrement(_tail);
        _array[_tail] = default;
        return removedValue;
    }

    public T? RemoveLast()
    {
        if (IsEmpty()) throw new Exception();
        int newTail = Decrement(_tail);
        var removedValue = _array[newTail];
        _array[newTail] = default;
        _tail = newTail;
        return removedValue;
    }

    bool Remove(T? value)
    {
        if (value == null)
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (_array[i] == null)
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
                if (value.Equals(_array[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    bool RemoveAll(T?[]? array)
    {
        if (array == null)
            return false;
        bool allWasDeleted = true;
        foreach (var item in array)
        {
            if (!Remove(item))
                allWasDeleted = false;
        }
        return allWasDeleted;
    }

    public bool RemoveFirstOccurance(T? value) => Remove(value);

    public bool RemoveLastOccurance(T? value)
    {
        int pos = Size() - 1;
        if (value == null)
        {
            for (int i = Decrement(_tail); pos != -1; i = Decrement(i), pos--)
            {
                if (_array[i] == null)
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        else
        {
            for (int i = Decrement(_tail); pos != -1; i = Decrement(i), pos--)
            {
                if (value.Equals(_array[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    void RetainAll(T?[]? array)
    {
        if (array == null)
        {
            Clear();
            return;
        }

        for (int i = _head; i != _tail; i = Increment(i))
        {
            bool toDelete = true;
            if (_array[i] == null)
            {
                foreach (var item in array)
                {
                    if (item == null)
                    {
                        toDelete = false;
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in array)
                {
                    if (_array[i]!.Equals(item))
                    {
                        toDelete = false;
                        break;
                    }
                }
            }
            if (toDelete) RemoveAt(i);
        }
    }

    public bool Contains(T? value)
    {
        if (value == null)
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (_array[i] == null) return true;
            }
        }
        else
        {
            for (int i = _head; i != _tail; i = Increment(i))
            {
                if (value.Equals(_array[i])) return true;
            }
        }
        return false;
    }

    public bool ContainsAll(T?[]? array)
    {
        if (array == null)
            return false;
        foreach (var item in array)
        {
            if (!Contains(item))
                return false;
        }
        return true;
    }

    public T? Element() => GetFirst();

    public bool Offer(T? value)
    {
        Add(value);
        return true;
    }

    public bool TryPeek(out T? headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = GetFirst();
        return true;
    }

    public bool TryPool(out T? headValue)
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

    public bool OfferFirst(T? value)
    {
        AddFirst(value);
        return true;
    }

    public bool OfferLast(T? value) => Offer(value);

    public T? Pop() => RemoveFirst();

    public bool TryPeekFirst(out T? headValue) => TryPeek(out headValue);

    public bool TryPeekLast(out T? tailValue)
    {
        if (IsEmpty())
        {
            tailValue = default;
            return false;
        }
        tailValue = GetLast();
        return true;
    }

    public bool TryPoolFirst(out T? headValue) => TryPool(out headValue);

    public bool TryPoolLast(out T? tailValue)
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
        Array.Clear(_array);
        _head = 0;
        _tail = 0;
        _array = new T?[16];
    }

    public T?[] ToArray()
    {
        var newArray = new T?[Size()];
        for (int i = _head, j = 0; i != _tail; i = Increment(i), j++)
        {
            newArray[j] = _array[i];
        }
        return newArray;
    }

    public T?[] ToArray(T?[]? array)
    {
        if (array == null || array.Length < Size()) return ToArray();

        for (int i = _head, j = 0; i != _tail; i = Increment(i), j++)
        {
            array[j] = _array[i];
        }
        return array;
    }

    public override string ToString()
    {
        string content = "";
        for (int i = _head; i != _tail; i = Increment(i))
        {
            content += $"{_array[i]}, ";
        }
        if (content.Length > 0)
            content = content.Substring(0, content.Length - 2);
        return $"[{content}]";
    }
}