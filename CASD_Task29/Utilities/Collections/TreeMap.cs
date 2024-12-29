namespace Utilities.Collections;

using CollectionInterfaces;
using RBTree;
using System.Collections;
using System.Text;


public class TreeMap<K, V> : IMyNavigableMap<K, V>
    where K : IComparable<K>
{
    private RBTree<K, V> _tree;

    private IComparer<K> _comparer;

    private int _size = 0;


    public int Size() => _size;

    public bool IsEmpty() => _size == 0;

    public IComparer<K> Comparer => _comparer;


    public TreeMap() : this((IComparer<K>?)null) { }

    public TreeMap(IComparer<K>? comparer) : this(null, comparer) { }

    public TreeMap(IMyMap<K, V> map, IComparer<K>? comparer = null)
    {
        if (comparer == null)
            _comparer = Comparer<K>.Default;
        else
            _comparer = comparer;

        _tree = new RBTree<K, V>(_comparer);

        PutAll(map);
    }

    public TreeMap(IMySortedMap<K, V> sortedMap)
    {
        if (sortedMap == null)
            _comparer = Comparer<K>.Default;
        else
            _comparer = sortedMap.Comparer;

        _tree = new RBTree<K, V>(_comparer);

        PutAll(sortedMap);
    }


    public IMySet<(K, V)> EntrySet()
    {
        var newSet = new HashSet<(K, V)>();
        foreach (var keyValue in this) newSet.Add(keyValue);
        return (IMySet<(K, V)>)newSet;
    }

    public IMySet<K> KeySet()
    {
        var newSet = new HashSet<K>();
        foreach (var (key, _) in this) newSet.Add(key);
        return (IMySet<K>)newSet;
    }

    public IMySet<V> ValueSet()
    {
        var newSet = new HashSet<V>();
        foreach (var (_, value) in this) newSet.Add(value);
        return (IMySet<V>)newSet;
    }

    public IMySortedMap<K, V> HeadMap(K upperBound, bool incl)
    {
        if (upperBound == null)
            throw new ArgumentNullException();

        Func<K, bool> includeUpperBound = incl
        ? key => _comparer.Compare(key, upperBound) == 0
        : key => false;

        var newMap = new TreeMap<K, V>(_comparer);

        foreach (var(key, value) in this)
        {
            if (_comparer.Compare(key, upperBound) < 0 || includeUpperBound(key))
                newMap.Put(key, value);
        }
        return newMap;
    }

    public IMySortedMap<K, V> SubMap(K lowerBound, bool lowIncl, K upperBound, bool highIncl)
    {
        if (lowerBound == null || upperBound == null)
            throw new ArgumentNullException();

        Func<K, bool> includeLowerBound = lowIncl
        ? key => _comparer.Compare(key, lowerBound) == 0
        : key => false;

        Func<K, bool> includeUpperBound = highIncl
        ? key => _comparer.Compare(key, upperBound) == 0
        : key => false;

        var newMap = new TreeMap<K, V>(_comparer);

        foreach(var(key, value) in this) 
        {
            if ((_comparer.Compare(key, lowerBound) > 0 || includeLowerBound(key))
                && (_comparer.Compare(key, upperBound) < 0 || includeUpperBound(key)))
                newMap.Put(key, value);
        }
        return newMap;
    }

    public IMySortedMap<K, V> TailMap(K lowerBound, bool incl)
    {
        if (lowerBound == null)
            throw new ArgumentNullException();

        Func<K, bool> includeLowerBound = incl
        ? key => _comparer.Compare(key, lowerBound) == 0
        : key => false;

        var newMap = new TreeMap<K, V>(_comparer);

        foreach (var(key, value) in this)
        { 
            if (_comparer.Compare(key, lowerBound) > 0 || includeLowerBound(key))
                newMap.Put(key, value);
        }
        return newMap;
    }

    private V Get(K key)
    {
        if (key == null) throw new ArgumentNullException();

        var node = _tree.Search(key);

        if (node == null) throw new KeyNotFoundException();

        return node.Value;
    }

    private void Set(K key, V value) => Put(key, value);

    public V this[K key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public Iterator<(K, V)> Iterator() 
        => new TreeMapItr<K, V>(_tree.Iterator());

    public Iterator<(K, V)> ReverseIterator() 
        => new TreeMapItr<K, V>(_tree.ReverseIterator());

    public IEnumerator<(K, V)> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Put(K key, V value)
    {
        if (key == null || value == null)
            throw new ArgumentNullException();

        _tree.Insert(key, value);
        _size++;
    }

    public void PutAll(IMyMap<K, V> otherMap)
    {
        if (otherMap == null) return;

        foreach(var(key, value) in otherMap) Put(key, value);
    }

    public V Remove(K key)
    {
        var keyValue = _tree.Search(key);

        if (keyValue == null) 
            throw new KeyNotFoundException();

        _tree.Remove(key);
        return keyValue.Value;
    }

    public bool SafeRemove(K key)
    {
        if (key == null) return false;

        if (_tree.Remove(key))
        {
            _size--;
            return true;
        }
        return false;
    }

    public bool TryGet(K key, out V value)
    {
        if (key == null)
        {
            value = default;
            return false;
        }

        var node = _tree.Search(key);

        if (node == null)
        {
            value = default;
            return false;
        }

        value = node.Value;
        return true;
    }

    public bool ContainsKey(K key) {
        if (key == null) return false;

        return _tree.ContainsKey(key);
    }

    public bool ContainsValue(V value) => _tree.ContainsValue(value);

    public K FirstKey() 
    {
        if (IsEmpty()) 
            throw new InvalidOperationException();

        return _tree.MinNode()!.Key!;
    }

    public K LastKey()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException();

        return _tree.MaxNode()!.Key!;
    }

    public (K, V)? LowerEntry(K bound)
    {
        if (bound == null) return null;

        var node = _tree.LowerEntry(bound);
        return node == null ? null : (node.Key!, node.Value);
    }

    public (K, V)? FloorEntry(K bound)
    {
        if (bound == null) return null;

        var node = _tree.FloorEntry(bound);
        return node == null ? null : (node.Key!, node.Value);
    }

    public (K, V)? HigherEntry(K bound)
    {
        if (bound == null) return null;

        var node = _tree.HigherEntry(bound);
        return node == null ? null : (node.Key!, node.Value);
    }


    public (K, V)? CeilingEntry(K bound)
    {
        if (bound == null) return null;

        var node = _tree.CeilingEntry(bound);
        return node == null ? null : (node.Key!, node.Value);
    }

    public bool TryLowerKey(K bound, out K key)
    {
        if (bound == null)
        {
            key = default;
            return false;
        }

        var keyValue = LowerEntry(bound);

        if (keyValue == null)
        {
            key = default;
            return false;
        }

        key = keyValue.Value.Item1;
        return true;
    }
    public bool TryHigherKey(K bound, out K key) 
    {
        if (bound == null)
        {
            key = default;
            return false;
        }

        var keyValue = HigherEntry(bound);
        if (keyValue == null)
        {
            key = default;
            return false;
        }

        key = keyValue.Value.Item1;
        return true;
    }

    public bool TryFloorKey(K bound, out K key) 
    {
        if (bound == null)
        {
            key = default;
            return false;
        }

        var keyValue = FloorEntry(bound);

        if (keyValue == null)
        {
            key = default;
            return false;
        }

        key = keyValue.Value.Item1;
        return true;
    }
    public bool TryCeilingKey(K bound, out K key) 
    {
        if (bound == null)
        {
            key = default;
            return false;
        }

        var keyValue = CeilingEntry(bound);

        if (keyValue == null)
        {
            key = default;
            return false;
        }

        key = keyValue.Value.Item1;
        return true;
    }

    public (K, V)? PollFirstEntry()
    {
        if (IsEmpty()) return null;

        var minNode = _tree.MinNode();
        _tree.Remove(minNode!.Key);
        _size--;
        return (minNode!.Key!, minNode.Value);
    }

    public (K, V)? PollLastEntry()
    {
        if (IsEmpty()) return null;

        var maxNode = _tree.MaxNode();
        _tree.Remove(maxNode!.Key);
        _size--;
        return (maxNode.Key!, maxNode.Value);
    }

    public (K, V)? FirstEntry()
    {
        if (IsEmpty()) return null;

        var minNode = _tree.MinNode();
        return (minNode!.Key!, minNode.Value);
    }

    public (K, V)? LastEntry()
    {
        if (IsEmpty()) return null;

        var maxNode = _tree.MaxNode();
        return (maxNode!.Key!, maxNode.Value);
    }

    public void Clear()
    {
        _tree.Clear();
        _size = 0;
    }
    public (K, V)[] ToArray()
    {
        var array = new (K, V)[Size()];
        int i = 0;
        foreach (var keyValue in this)
        { 
            array[i++] = keyValue;
        }
        return array;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 8 + 2);
        sb.Append("[");
        for(var it = Iterator(); it.HasNext();)
        {
            var(key, value) = it.Next();
            sb.Append($"({key}, {value}), ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}
