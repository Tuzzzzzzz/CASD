using System.Collections;

namespace MyLinkedList;

public class Node<T>
{
    public Node<T>? Pref { get; set; }

    public Node<T>? Next { get; set; }

    public T? Value { get; set; }

    public Node(Node<T>? pref, T? value, Node<T>? next)
    {
        Value = value;
        Pref = pref;
        Next = next;
    }
}

internal class MyLinkedList<T>: IEnumerable<T?>
{
    private Node<T>? 
        _first = null,
        _last = null;

    private int _size = 0;

    public MyLinkedList() { }

    public MyLinkedList(T?[]? array) => AddAll(array);

    public bool IsEmpty() => _size == 0;

    public int Size() => _size;

    private void CheckPositionIndex(int index)
    {
        if (index < 0 || index >= _size)
            throw new IndexOutOfRangeException();
    }

    public T? this[int index]
    {
        get => Get(index);

        set => Set(index, value);
    }

    public T? Get(int index)
    {
        CheckPositionIndex(index);

        if (index < _size/2)
        {
            int i = 0;
            for (var current = _first; current != null; current = current.Next)
            {
                if (i == index)
                    return current.Value;
                i++;
            }
        }
        else
        {
            int i = _size - 1;
            for (var current = _last; current != null; current = current.Pref)
            {
                if (i == index)
                    return current.Value;
                i--;
            }
        }
        throw new Exception();
    }

    public void Set(int index, T? value)
    {
        CheckPositionIndex(index);

        int i = 0;
        for (var current = _first; current != null; current = current.Next)
        {
            if (i == index)
            {
                current.Value = value;
                return;
            }
            i++;
        }
    }

    //Iterator
    private class Itr : MyListIterator<T?>
    {
        private MyLinkedList<T> _list;
        private int _nextIndex;
        private Node<T>? _lastReturnedNode = null;
        private Node<T>? _nextNode;

        public Itr(MyLinkedList<T> list, int index) 
        { 
            list.CheckPositionIndex(index);
            _list = list;
            //null is the flag of the end of this list
            _nextNode = (index == list._size-1) ? null : list.NodeAt(index);
            _nextIndex = index;
        }

        public bool HasNext() => _nextIndex < _list.Size();

        public bool HasPrevious() => _nextIndex >= 0;
        
        public int NextIndex() => _nextIndex;

        public int PreviousIndex() => _nextIndex;

        public T? Next() 
        {
            if (!HasNext())
                throw new Exception();
            //current index is _nextIndex-1
            _nextIndex++;
            if (_nextNode == null)
                _nextNode = _list._last;
            _lastReturnedNode = _nextNode;
            _nextNode = _nextNode!.Next;
            return _lastReturnedNode!.Value;
        }

        public T? Previous() {
            if (!HasPrevious())
                throw new Exception();
            //current index is _nextIndex
            _nextIndex--;
            _nextNode = (_nextNode == null) ? _list._last : _nextNode.Pref;
            _lastReturnedNode = _nextNode;
            return _lastReturnedNode!.Value;
        }

        public void Remove()
        {
            //Remove() cannot be called twice or after Add(T? value)
            if (_lastReturnedNode == null)
                throw new Exception();
            //if the last one was the use of Next(), not Previous()
            if (_lastReturnedNode != _nextNode) _nextIndex--;
            _list.Remove(_lastReturnedNode);
            _lastReturnedNode = null;
        }

        public void Add(T? value)
        {
            if (_nextNode == null)
                _list.AddLast(value);
            else
                _list.AddBefore(_nextNode, value);
            _lastReturnedNode = null;
            _nextIndex++;
        }

        public void Set(T? value)
        {
            if (_lastReturnedNode == null)
                throw new Exception();
            _lastReturnedNode.Value = value;
        }
    }

    public MyListIterator<T?> ListIterator() => new Itr(this, 0);

    public MyListIterator<T?> ListIterator(int index) => new Itr(this, index);

    public IEnumerator<T?> GetEnumerator() {
        for (var it = ListIterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Node<T>? NodeAt(int index)
    {
        if (index < _size / 2)
        {
            int i = 0;
            for (var current = _first; current != null; current = current.Next)
            {
                if (index == i) return current;
                i++;
            }
        }
        else
        {
            int i = _size - 1;
            for (var current = _last; current != null; current = current.Pref)
            {
                if (index == i) return current;
                i--;
            }
        }
        throw new Exception();
    }

    private void AddBefore(Node<T>? current, T? value)
    {
        if (current == null) { 
            AddFirst(value); 
            return; 
        }
        var newNode = new Node<T>(current.Pref, value, current);
        if(current.Pref != null)
            current.Pref!.Next = newNode;
        current.Pref = newNode;
        _size++;
    }

    public void Add(T? value)
    {
        var newNode = new Node<T>(_last, value, null);

        if (IsEmpty())
            _first = newNode;
        else
            _last!.Next = newNode;

        _last = newNode;
        _size++;
    }

    public bool AddAll(T?[]? array)
    {
        if (array is null)
            return false;

        foreach (var item in array)
        {
            Add(item);
        }
        return true;
    }

    public void Add(int index, T? value)
    {
        if (index < 0 && index > _size)
            throw new ArgumentOutOfRangeException();

        if (index == _size)
        {
            Add(value);
            return;
        }

        Node<T>? current = null;
        if (index < _size / 2)
        {
            int i = 0;
            for (current = _first; current != null; current = current.Next)
            {
                if (i == index)
                {
                    AddBefore(current, value);
                    return;
                } 
                i++;
            }
        }
        else
        {
            int i = _size - 1;
            for (current = _last; current != null; current = current.Pref)
            {
                if (i == index)
                {
                    AddBefore(current, value);
                    return;
                }
                i--;
            }
        }
    }

    public void AddAll(int index, T?[]? array)
    {
        if (array == null) return;

        for(int i = 0; i < array.Length; i++)
        {
            Add(index+i, array[i]);
        }
    }

    private void Unlink(Node<T> node)
    {
        if (node.Pref is null)
            _first = node.Next;
        else
            node.Pref.Next = node.Next;

        if (node.Next is null)
            _last = node.Pref;
        else
            node.Next.Pref = node.Pref;
    }

    public bool Remove(T? value)
    {
        if (value is null)
        {
            for(var current = _first; current != null; current = current.Next)
            {
                if (current.Value == null)
                {
                    Remove(current);
                    return true;
                }
            }
        }
        else
        {
            for (var current = _first; current != null; current = current.Next)
            {
                if (value.Equals(current.Value))
                {
                    Remove(current);
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveAll(T?[]? array)
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

    public void RemoveAt(int index)
    {
        CheckPositionIndex(index);

        if (index < _size / 2)
        {
            int i = 0;
            for (var current = _first; current != null; current = current.Next)
            {
                if (i == index)
                {
                    Remove(current);
                    return;
                }
                i++;
            }
        }
        else
        {
            int i = _size - 1;
            for (var current = _last; current != null; current = current.Pref)
            {
                if (i == index)
                {
                    Remove(current);
                    return;
                }
                i--;
            }
        }
    }

    private void Remove(Node<T> node)
    {
        Unlink(node);
        _size--;
    }

    public void RetainAll(T?[]? array)
    {
        if (array == null)
        {
            Clear();
            return;
        }

        for (var current = _first; current != null; current = current.Next)
        {
            bool toDelete = true;
            var nodeToDelete = current;
            if (current.Value == null)
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
                    if (current.Value.Equals(item))
                    {
                        toDelete = false;
                        break;
                    }
                }
            }
            if (toDelete) Remove(nodeToDelete);
        }
    }

    public bool Contains(T? value)
    {
        if (value == null)
        {
            for (var current = _first; current != null; current = current.Next)
            {
                if (current.Value == null)
                {
                    return true;
                }
            }
        }
        else
        {
            for (var current = _first; current != null; current = current.Next)
            {
                if (value.Equals(current.Value))
                {
                    return true;
                }
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

    public int IndexOf(T? value)
    {
        if (value == null)
        {
            int i = 0;
            for (var current = _first; current != null; current = current.Next)
            {
                if (current.Value == null)
                    return i;
                i++;
            }
        }
        else
        {
            int i = 0;
            for (var current = _first; current != null; current = current.Next)
            {
                if (value.Equals(current.Value))
                    return i;
                i++;
            }
        }
        return -1;
    }

    public int LastIndexOf(T? value)
    {
        if (value == null)
        {
            int i = _size-1;
            for (var current = _last; current != null; current = current.Pref)
            {
                if (current.Value == null)
                    return i;
                i--;
            }
        }
        else
        {
            int i = _size-1;
            for (var current = _last; current != null; current = current.Pref)
            {
                if (value.Equals(current.Value))
                    return i;
                i--;
            }
        }
        return -1;
    }

    public MyLinkedList<T> SubList(int fromIndex, int toIndex)
    {
        CheckPositionIndex(fromIndex);
        CheckPositionIndex(toIndex);

        var newList = new MyLinkedList<T>();

        int i = 0;
        for (var current = _first; current != null; current = current.Next)
        {
            if (i >= fromIndex && i < toIndex)
                newList.Add(current.Value);
            i++;
        }
        return newList;
    }

    public T? Element()
    {
        if (IsEmpty())
            throw new Exception();
        return _first!.Value;
    }

    public T? GetFirst()
    {
        if (!IsEmpty())
            throw new Exception();
        return _first!.Value;
    }

    public T? GetLast()
    {
        if (!IsEmpty())
            throw new Exception();
        return _last!.Value;
    }

    public bool Offer(T? value)
    {
        Add(value);
        return true;
    }

    public bool OfferFirst(T? value)
    {
        AddFirst(value);
        return true;
    }

    public bool OfferLast(T? value)
    {
        AddLast(value);
        return true;
    }

    public bool TryPeek(out T? headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = _first!.Value;
        return true;
    }

    public bool TryPeekFirst(out T? headValue) => TryPeek(out headValue);

    public bool TryPeekLast(out T? tailValue)
    {
        if (IsEmpty())
        {
            tailValue = default;
            return false;
        }
        tailValue = _last!.Value;
        return true;
    }

    public bool TryPool(out T? headValue)
    {
        if (IsEmpty())
        {
            headValue = default;
            return false;
        }
        headValue = _first!.Value;
        Remove(_first);
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
        tailValue = _last!.Value;
        Remove(_last);
        return true;
    }

    public T? RemoveFirst()
    {
        if (IsEmpty())
            throw new Exception();
        var headValue = _first!.Value;
        Remove(_first);
        return headValue;
    }

    public T? RemoveLast()
    {
        if (IsEmpty())
            throw new Exception();
        var headValue = _last!.Value;
        Remove(_last);
        return headValue;
    }

    public bool RemoveFirstOccurence(T? value) => Remove(value);

    public bool RemoveLastOccurence(T? value)
    {
        if (value is null)
        {
            for (var current = _last; current != null; current = current.Pref)
            {
                if (current.Value == null)
                {
                    Remove(current);
                    return true;
                }
            }
        }
        else
        {
            for (var current = _last; current != null; current = current.Pref)
            {
                if (value.Equals(current.Value))
                {
                    Remove(current);
                    return true;
                }
            }
        }
        return false;
    }

    public void AddFirst(T? value)
    {
        var newNode = new Node<T>(null, value, _first);
        if (IsEmpty())
            _last = newNode;
        else
            _first!.Pref = newNode;
        _first = newNode;
        _size++;
    }

    public void AddLast(T? value) => Add(value);

    public T? Pop() => RemoveFirst();

    public void Push(T? value) => AddFirst(value);

    public void Clear()
    {
        _size = 0;
        for (var current = _first; current != null; current = current.Next)
        {
            current.Pref = null;
            current.Value = default;
        }
        _first = null;
        _last = null;
    }

    public T?[] ToArray()
    {
        var newArray = new T?[_size];

        int i = 0;
        for (var current = _first; current != null; current = current.Next)
        {
            newArray[i] = current.Value;
            i++;
        }
        return newArray;
    }

    public T?[] ToArray(T?[] array)
    {
        if (array == null || array.Length < _size)
        {
            return ToArray();
        }

        int i = 0;
        for (var current = _first; current != null; current = current.Next)
        {
            array[i] = current.Value;
            i++;
        }
        return array;
    }

    public override string ToString()
    {
        string content = "";
        
        for (var current = _first; current != null; current = current.Next)
        {
            content += $"{current.Value}, ";
        }

        if (content.Length > 0)
            content = content.Substring(0, content.Length - 2);
        return $"[{content}]";
    }

}