namespace Utilities.Collections;

using System.Collections;
using System.Text;
using CollectionInterfaces;


internal class Entry<K, V>
{
    public K Key { get; set; }

    public V Value { get; set; }

    public Entry<K, V> Next { get; set; }

    public Entry(K key, V value, Entry<K, V> next)
    {
        Key = key;
        Value = value;
        Next = next;
    }
}


public class MyHashMap<K, V> : IMyMap<K, V>
{
    private Entry<K, V>[] _table;
    private int _size = 0;
    private float _loadFactor;


    public MyHashMap(int initialCapacity = 16, float loadFactor = 0.75f)
    {
        if (initialCapacity <= 0 || loadFactor <= 0 || loadFactor > 1) 
            throw new ArgumentException();

        _loadFactor = loadFactor;
        _table = new Entry<K, V>[initialCapacity];
    }


    public int Size() => _size;

    public bool IsEmpty() => _size == 0;


    public IMySet<(K, V)> EntrySet()
    {
        var set = new MyHashSet<(K, V)>();
        foreach (var tuple in this) set.Add(tuple);
        return set;
    }

    public IMySet<K> KeySet()
    {
        var set = new MyHashSet<K>();
        foreach (var(key, _) in this) set.Add(key);
        return set;
    }

    public IMySet<V> ValueSet()
    {
        var set = new MyHashSet<V>();
        foreach (var(_, value) in this) set.Add(value);
        return set;
    }

    private V Get(K key)
    {
        if (TryGet(key, out V value)) return value;
        throw new KeyNotFoundException();
    }

    private void Set(K key, V value) => Put(key, value);

    public V this[K key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    private class Itr : Iterator<(K, V)>
    {
        private MyHashMap<K, V> _map;
        private Entry<K, V> _nextEntry = null;
        private int _nextTableIndex = -1;
        private Entry<K, V> _lastReturnedEntry = null;

        public Itr(MyHashMap<K, V> map)
        {
            _map = map;
            _nextTableIndex = NextTableIndex();
        }

        public bool HasNext() => _nextTableIndex < _map._table.Length;

        public (K, V) Next()
        {
            if (!HasNext())
                throw new InvalidOperationException();

            if (_nextEntry == null)
            {
                _lastReturnedEntry = _map._table[_nextTableIndex];
            }
            else
            {
                _lastReturnedEntry = _nextEntry;
            }

            _nextEntry = _lastReturnedEntry!.Next;
            if (_nextEntry == null)
            {
                _nextTableIndex = NextTableIndex();
            }

            return (_lastReturnedEntry.Key, _lastReturnedEntry.Value);
        }

        private int NextTableIndex()
        {
            int index = _nextTableIndex + 1;
            while (index < _map._table.Length && _map._table[index] == null)
            {
                index++;
            }
            return index;
        }

        public void Remove()
        {
            if (_lastReturnedEntry == null)
                throw new InvalidOperationException();

            _map.TryRemove(_lastReturnedEntry.Key, out _);
            _lastReturnedEntry = null;
        }
    }

    public Iterator<(K, V)> Iterator() => new Itr(this);

    public IEnumerator<(K, V)> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private int Hash(K? key) 
        => (key == null) ? 0 : Math.Abs(key.GetHashCode()) % _table.Length;

    public void Put(K key, V value)
    {
        int hash = Hash(key);

        if (_table[hash] == null)
        {
            _table[hash] = new Entry<K, V>(key, value, null);
            _size++;
        }

        else if (key == null)
        {
            bool isNewKey = true;

            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (current.Key == null)
                {
                    current.Value = value;
                    isNewKey = false;
                    break;
                }
            }
            if (isNewKey)
            {
                var nextNode = _table[hash];
                _table[hash] = new Entry<K, V>(key, value, nextNode);
                _size++;
            }
        }

        else
        {
            bool isNewKey = true;

            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (key.Equals(current.Key))
                {
                    current.Value = value;
                    isNewKey = false;
                    break;
                }
            }
            if (isNewKey)
            {
                var nextNode = _table[hash];
                _table[hash] = new Entry<K, V>(key, value, nextNode);
                _size++;
            }
        }

        if (_size > _loadFactor * _table.Length) Realocation();
    }

    public void PutAll(IMyMap<K, V> otherMap)
    {
        if (otherMap == null) return;

        foreach (var(key, value) in otherMap) Put(key, value);
    }

    private void Realocation()
    {
        _size = 0;
        var copiedTable = _table;
        _table = new Entry<K, V>[_table.Length * 2];

        foreach (var basket in copiedTable)
        {
            if (basket != null)
            {
                for (var current = basket; current != null; current = current.Next)
                {
                    Put(current.Key, current.Value);
                }
            }
        }
    }

    public bool TryRemove(K key, out V value)
    {
        int hash = Hash(key);

        if (_table[hash] == null)
        {
            value = default;
            return false;
        }

        Entry<K, V> previous = null;
        if (key == null)
        {
            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (current.Key == null)
                {
                    value = current.Value;
                    UnlinkEntry(hash, previous, current);
                    _size--;
                    return true;
                }
                previous = current;
            }
        }
        else
        {
            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (current.Key!.Equals(key))
                {
                    value = current.Value;
                    UnlinkEntry(hash, previous, current);
                    _size--;
                    return true;
                }
                previous = current;
            }
        }

        value = default;
        return false;
    }

    public V Remove(K key)
    {
        if (TryRemove(key, out V value)) return value;

        throw new KeyNotFoundException();
    }

    private void UnlinkEntry(int hash, Entry<K, V> previous, Entry<K, V> current)
    {
        if (_table[hash] == current)
        {
            _table[hash] = current.Next;
        }
        else
        {
            previous!.Next = current.Next;
        }
    }

    public bool TryGet(K key, out V value)
    {
        int hash = Hash(key);

        if (_table[hash] == null)
        {
            value = default;
            return false;
        }

        if (key == null)
        {
            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (current.Key == null)
                {
                    value = current.Value;
                    return true;
                }
            }
        }

        else
        {
            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (key.Equals(current.Key))
                {
                    value = current.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    public bool ContainsKey(K key)
    {
        int hash = Hash(key);
        if (_table[hash] == null)
        {
            return false;
        }

        if (key == null)
        {
            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (current.Key == null)
                {
                    return true;
                }
            }
        }
        else
        {
            for (var current = _table[hash]; current != null; current = current.Next)
            {
                if (key.Equals(current.Key))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ContainsValue(V value)
    {
        if (value == null)
        {
            foreach (var basket in _table)
            {
                if (basket != null)
                {
                    for (var current = basket; current != null; current = current.Next)
                    {
                        if (current.Value == null) return true;
                    }
                }
            }
        }
        else
        {
            foreach (var basket in _table)
            {
                if (basket != null)
                {
                    for (var current = basket; current != null; current = current.Next)
                    {
                        if (value.Equals(current.Value)) return true;
                    }
                }
            }
        }
        return false;
    }

    public void Clear()
    {
        _size = 0;
        Array.Clear(_table);
    }

    public (K, V)[] ToArray()
    {
        var newArray = new (K, V)[Size()];
        int i = 0;
        foreach (var basket in _table)
        {
            if (basket != null)
            {
                for (var current = basket; current != null; current = current.Next)
                {
                    newArray[i] = (current.Key, current.Value);
                    i++;
                }
            }
        }
        return newArray;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 8 + 2);
        sb.Append("[");
        foreach (var basket in _table)
        {
            if (basket != null)
            {
                for (var current = basket; current != null; current = current.Next)
                {
                    sb.Append($"({current.Key}, {current.Value}), ");
                }
            }
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}