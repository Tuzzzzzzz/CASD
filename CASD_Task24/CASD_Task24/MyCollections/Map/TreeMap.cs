namespace MyCollections.Map;

using RBTree;

internal class TreeMap<K, V> where K : IComparable<K>
{
    private RBTree<K, V> _tree;

    private IComparer<K> _comparer;

    private int _size = 0;


    public int Size() => _size;

    public bool IsEmpty() => _size == 0;

    public IComparer<K> Comparer => _comparer;

    public TreeMap()
    {
        _comparer = Comparer<K>.Default;
        _tree = new RBTree<K, V>(_comparer);
    }

    public TreeMap(IComparer<K>? comparer)
    {
        if (comparer is null)
            throw new ArgumentNullException();

        _comparer = comparer;
        _tree = new RBTree<K, V>(comparer);
    }

    public V this[K? key]
    {
        get => Get(key);
        set => Put(key, value);
    }

    public Iterator<(K, V)> Iterator() 
        => new TreeMapItr<K, V>(_tree.Iterator())!;

    public Iterator<(K, V)> ReverseIterator() 
        => new TreeMapItr<K, V>(_tree.ReverseIterator())!;

    public void Put(K? key, V? value)
    {
        if (key is null || value is null)
            throw new ArgumentNullException();

        _tree!.Insert(key, value);
        _size++;
    }

    public bool Remove(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        if (_tree.Remove(key))
        {
            _size--;
            return true;
        }
        return false;
    }

    public V Get(K? key)
    {
        if (key == null) throw new ArgumentNullException();
        var node = _tree.Search(key);
        if (node == null) throw new Exception();
        return node.Value!;
    }

    public bool TryGet(K? key, out V? value)
    {
        if (key == null) throw new ArgumentNullException();
        var node = _tree.Search(key);
        if (node == null)
        {
            value = default;
            return false;
        }
        value = node.Value;
        return true;
    }

    public bool ContainsKey(K? key) {
        if (key is null)
            throw new ArgumentNullException();

        return _tree.ContainsKey(key);
    }

    public bool ContainsValue(V? value) {

        if (value is null)
            throw new ArgumentNullException();

        return _tree.ContainsValue(value);
    }

    public K FirstKey() 
    {
        if (IsEmpty()) throw new Exception();
        return _tree.MinNode()!.Key!;
    }

    public K LastKey()
    {
        if (IsEmpty()) throw new Exception();
        return _tree.MaxNode()!.Key!;
    }

    public TreeMap<K, V> HeadMap(K? end)
    {
        if (end is null) 
            throw new ArgumentNullException();

        var newMap = new TreeMap<K, V>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, end) < 0)
                newMap.Put(node.Key!, node.Value);
        }
        return newMap;
    }

    public TreeMap<K, V> SubMap(K? start, K? end)
    {
        if (start is null || end is null)
            throw new ArgumentNullException();

        var newMap = new TreeMap<K, V>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, start) > 0 && _comparer.Compare(node.Key, end) < 0)
                newMap.Put(node.Key!, node.Value);
        }
        return newMap;
    }

    public TreeMap<K, V> TailMap(K? start)
    {
        if (start is null)
            throw new ArgumentNullException();

        var newMap = new TreeMap<K, V>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, start) > 0)
                newMap.Put(node.Key!, node.Value);
        }
        return newMap;
    }

    public (K, V?)? LowerEntry(K? key) {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.LowerEntry(key);
        return node is null ? null : (node.Key!, node.Value);
    }

    public (K, V?)? FloorEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.FloorEntry(key);
        return node is null ? null : (node.Key!, node.Value);
    }

    public (K, V?)? HigherEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.HigherEntry(key);
        return node is null ? null : (node.Key!, node.Value);
    }


    public (K, V?)? CeilingEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.CeilingEntry(key);
        return node is null ? null : (node.Key!, node.Value);
    }

    public bool TryLowerKey(K? key, out K? result)
    {
        if (key is null)
            throw new ArgumentNullException();

        var keyValue = LowerEntry(key);
        if (keyValue is null)
        {
            result = default;
            return false;
        }
        result = keyValue.Value.Item1;
        return true;
    }
    public bool TryHigherKey(K? key, out K? result) 
    {
        if (key is null)
            throw new ArgumentNullException();

        var keyValue = LowerEntry(key);
        if (keyValue is null)
        {
            result = default;
            return false;
        }
        result = keyValue.Value.Item1;
        return true;
    }

    public bool TryFloorKey(K? key, out K? result) 
    {
        if (key is null)
            throw new ArgumentNullException();

        var keyValue = LowerEntry(key);
        if (keyValue is null)
        {
            result = default;
            return false;
        }
        result = keyValue.Value.Item1;
        return true;
    }
    public bool TryCeilingKey(K? key, out K? result) 
    {
        if (key is null)
            throw new ArgumentNullException();

        var keyValue = LowerEntry(key);
        if (keyValue is null)
        {
            result = default;
            return false;
        }
        result = keyValue.Value.Item1;
        return true;
    }

    public (K, V?) PollFirstEntry()
    {
        if (IsEmpty()) throw new Exception();
        var minNode = _tree.MinNode();
        _tree.Remove(minNode!.Key);
        _size--;
        return (minNode!.Key!, minNode.Value);
    }

    public (K, V?) PollLastEntry()
    {
        if (IsEmpty()) throw new Exception();
        var maxNode = _tree.MaxNode();
        _tree.Remove(maxNode!.Key);
        _size--;
        return (maxNode.Key!, maxNode.Value);
    }

    public (K, V?) FirstEntry()
    {
        if (IsEmpty()) throw new Exception();
        var minNode = _tree.MinNode();
        return (minNode!.Key!, minNode.Value);
    }

    public (K, V?) LastEntry()
    {
        if (IsEmpty()) throw new Exception();
        var maxNode = _tree.MaxNode();
        return (maxNode!.Key!, maxNode.Value);
    }

    public void Clear()
    {
        _tree.Clear();
        _size = 0;
    }
    public (K, V?)[] ToArray()
    {
        var array = new (K, V?)[_size];
        var it = Iterator();
        int i = 0;
        while (it.HasNext())
        { 
            array[i++] = it.Next();
        }
        return array;
    }

    public override string ToString()
    {
        var result = "[";
        var it = Iterator();
        while (it.HasNext())
        {
            var tuple = it.Next();
            result += $"{tuple}, ";
        }
        result = result.Substring(0, result.Length-2);
        return result+"]";
    }
}
