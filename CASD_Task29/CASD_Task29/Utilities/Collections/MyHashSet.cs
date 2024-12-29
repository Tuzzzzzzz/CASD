namespace Utilities.Collections;

using System.Collections;
using System.Text;
using Utilities.CollectionInterfaces;


public class MyHashSet<V>: IMySet<V>
{
    private MyHashMap<V, object> _map;


    public MyHashSet() => _map = new MyHashMap<V, object>();

    public MyHashSet(IMyCollection<V> collection) : this() => AddAll(collection);

    public MyHashSet(int initialCapacity, float loadFactor)
        => _map = new MyHashMap<V, object>(initialCapacity, loadFactor);

    public MyHashSet(int initialCapacity) 
        => _map = new MyHashMap<V, object>(initialCapacity);


    public int Size() => _map.Size();

    public bool IsEmpty() => _map.IsEmpty();


    private class Itr: Iterator<V>
    {
        private Iterator<(V, object)> _mapItr;

        public Itr(MyHashSet<V> set)
        {
            _mapItr = set._map.Iterator();
        }

        public bool HasNext() => _mapItr.HasNext();

        public V Next() => _mapItr.Next().Item1;

        public void Remove() => _mapItr.Remove();
    }

    public Iterator<V> Iterator() => new Itr(this);

    public IEnumerator<V> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(V value) => _map.Put(value, null);

    public void AddAll(IMyCollection<V> collection)
    {
        if (collection == null) return;
        foreach (V el in collection) _map.Put(el, null);
    }

    public bool Remove(V value) => _map.TryRemove(value, out _);

    public bool RemoveAll(IMyCollection<V> collection)
    {
        if (collection == null) return false;
        bool allDeleted = true;
        foreach (V el in collection)
            if (!_map.TryRemove(el, out _))
                allDeleted = false;
        return allDeleted;
    }

    public void RetainAll(IMyCollection<V> collection)
    {
        if (collection == null)
        {
            Clear();
            return;
        }

        foreach(var(value, _) in _map)
        {
            bool delete = true;
            if(value == null)
            {
                foreach(V el in collection)
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
                foreach (V el in collection)
                {
                    if (value.Equals(el))
                    {
                        delete = false;
                        break;
                    }
                }
            }
            if (delete) _map.Remove(value);
        }
    }

    public bool Contains(V value) => _map.ContainsKey(value);

    public bool ContainsAll(IMyCollection<V> collection)
    {
        if (collection == null) return false;

        foreach (V el in collection)
        {
            if (!Contains(el)) return false;
        }
        return true;
    }

    public void Clear() => _map.Clear();

    public V[] ToArray()
    {
        var newArray = new V[Size()];
        int i = 0;
        foreach (var (value, _) in _map)
        {
            newArray[i++] = value;
        }
        return newArray;
    }

    public V[] ToArray(V[] array)
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
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size()*3 + 2);
        sb.Append("[");
        foreach (var (value, _) in _map)
        {
             sb.Append($"{value}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}