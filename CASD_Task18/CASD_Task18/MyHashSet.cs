namespace MyHashSet;

using MyHashMap;
using System.Collections;

internal class MyHashSet<V> : IEnumerable<V?>
{
    private MyHashMap<V, object> _map;

    public MyHashSet() => _map = new MyHashMap<V, object>();

    public MyHashSet(V?[]? array) : this() => AddAll(array);

    public MyHashSet(int initialCapacity, float loadFactor)
        => _map = new MyHashMap<V, object>(initialCapacity, loadFactor);

    public MyHashSet(int initialCapacity)
        => _map = new MyHashMap<V, object>(initialCapacity);

    public int Size() => _map.Size();

    public bool IsEmpty() => _map.IsEmpty();

    private class Itr : MyIterator<V?>
    {
        private MyIterator<(V?, object?)> _mapItr;
        public Itr(MyHashSet<V> set)
        {
            _mapItr = set._map.Iterator();
        }

        public bool HasNext() => _mapItr.HasNext();

        public V? Next() => _mapItr.Next().Item1;

        public void Remove() => _mapItr.Remove();
    }

    public MyIterator<V?> Iterator() => new Itr(this);

    public IEnumerator<V?> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(V? value) => _map.Put(value, null);

    public void AddAll(V?[]? array)
    {
        if (array == null) return;
        foreach (var item in array) _map.Put(item, null);
    }

    public bool Remove(V? value) => _map.TryRemove(value, out _);

    public bool RemoveAll(V?[]? array)
    {
        if (array == null) return false;
        bool allWasDeleted = true;
        foreach (var item in array)
            if (!_map.TryRemove(item, out _))
                allWasDeleted = false;
        return allWasDeleted;
    }

    public void RetainAll(V?[]? array)
    {
        if (array == null)
        {
            Clear();
            return;
        }
        foreach (var (value, _) in _map)
        {
            bool toDelete = true;
            if (value == null)
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
                    if (value.Equals(item))
                    {
                        toDelete = false;
                        break;
                    }
                }
            }
            if (toDelete) _map.Remove(value);
        }
    }

    public bool Contains(V? value) => _map.ContainsKey(value);

    public bool ContainsAll(V?[]? array)
    {
        if (array == null) return false;
        foreach (var item in array)
        {
            if (!Contains(item)) return false;
        }
        return true;
    }

    public void Clear() => _map.Clear();

    public V?[] ToArray()
    {
        var newArray = new V?[Size()];
        int i = 0;
        foreach (var (value, _) in _map)
        {
            newArray[i++] = value;
        }
        return newArray;
    }

    public V?[] ToArray(V?[]? array)
    {
        if (array == null || array.Length < Size()) return ToArray();

        int i = 0;
        foreach (var (value, _) in _map)
        {
            array[i++] = value;
        }
        return array;
    }

    public override string ToString()
    {
        string content = "";
        foreach (var (value, _) in _map)
        {
            content += $"{value}, ";
        }
        if (content.Length > 0)
            content = content.Substring(0, content.Length - 2);
        return $"[{content}]";
    }
}