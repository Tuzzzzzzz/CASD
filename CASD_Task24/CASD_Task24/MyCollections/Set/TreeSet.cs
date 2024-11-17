namespace MyCollections.Set;

using RBTree;

file class ReverseComparer<K>: IComparer<K>
    where K: IComparable<K>
{
    private IComparer<K> _comparer;

    public ReverseComparer(IComparer<K> comparer)
    {
        _comparer = comparer;
    }
    public int Compare(K? x, K? y)
    {
        return (-1)*_comparer.Compare(x, y);
    }
}

internal class TreeSet<K> where K : IComparable<K>
{
    private RBTree<K, object> _tree;

    private IComparer<K> _comparer;

    private int _size = 0;

    public int Size() => _size;

    public bool IsEmpty() => _size == 0;

    public IComparer<K> Comparer => _comparer;

    public TreeSet()
    {
        _comparer = Comparer<K>.Default;
        _tree = new RBTree<K, object>(_comparer);
    }

    public TreeSet(IComparer<K>? comparer)
    {
        if (comparer is null)
            throw new ArgumentNullException();

        _comparer = comparer;
        _tree = new RBTree<K, object>(comparer);
    }

    public TreeSet(TreeSet<K> set)
    {
        _comparer = set.Comparer;
        _tree = new RBTree<K, object>(_comparer);
        _size = set.Size();
        var it = set.Iterator();
        while (it.HasNext())
        {
            _tree.Insert(it.Next(), null);
        }
    }

    public TreeSet(K?[]? array)
    {
        _comparer = Comparer<K>.Default;
        _tree = new RBTree<K, object>(_comparer);
        if (array is null)
            throw new ArgumentNullException();
        foreach (var item in array)
        {
            if (item is null) 
                throw new ArgumentNullException();
            Add(item);
        }
    }

    public Iterator<K> Iterator()
        => new TreeSetItr<K>(_tree.Iterator())!;

    public Iterator<K> ReverseIterator()
        => new TreeSetItr<K>(_tree.ReverseIterator())!;

    public void Add(K? value)
    {
        if (value is null)
            throw new ArgumentNullException();

        _tree!.Insert(value, null);
        _size++;

    }

    public void AddAll(K?[]? array)
    {
        if (array is null)
            throw new ArgumentNullException();

        foreach (var item in array)
        {
            if (item is null)
                throw new ArgumentNullException();
            Add(item);
        }
    }

    public bool Remove(K? value)
    {
        if (value is null)
            throw new ArgumentNullException();

        if (_tree.Remove(value))
        {
            _size--;
            return true;
        }
        return false;
    }

    public bool RemoveAll(K?[]? array)
    {
        if (array is null)
            throw new ArgumentNullException();
        
        bool flag = true;
        foreach(var item in array)
        {
            if (item is null)
                throw new ArgumentNullException();
            if (!Remove(item)) flag = false;
        }
        return flag;
    }

    public bool Contains(K? value)
    {
        if (value is null)
            throw new ArgumentNullException();

        return _tree.ContainsKey(value);
    }

    public bool ContainsAll(K?[]? array)
    {
        if (array is null)
            throw new ArgumentNullException();

        foreach (var item in array)
        {
            if (item is null) 
                throw new ArgumentNullException();
            if (!Contains(item)) return false;
        }
        return true;
    }

    public void RetainAll(K?[]? array)
    {
        if (array is null)
            throw new ArgumentNullException();

        foreach (var item in array)
        {
            if (item is null)
                throw new ArgumentNullException();
        }

        var it = Iterator();
        while (it.HasNext())
        {
            var needToDelete = true;
            var value = it.Next();
            foreach (var item in array)
            {
                if (_comparer.Compare(value, item) == 0)
                {
                    needToDelete = false;
                    break;
                }
            }
            if (needToDelete) Remove(value);
        }
    }

    public TreeSet<K> HeadSet(K? upperBound, bool incl)
    {
        if (upperBound is null)
            throw new ArgumentNullException();

        Func<K, bool> includeUpperBound = incl
        ? value => _comparer.Compare(value, upperBound) == 0
        : value => false;

        var newSet = new TreeSet<K>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, upperBound) < 0 || includeUpperBound(node.Key!))
                newSet.Add(node.Key);
        }
        return newSet;
    }

    public TreeSet<K> SubMap(K? lowerBound, bool lowIncl, K? upperBound, bool highIncl)
    {
        if (lowerBound is null || upperBound is null)
            throw new ArgumentNullException();

        Func<K, bool> includeLowerBound = lowIncl
        ? value => _comparer.Compare(value, lowerBound) == 0
        : value => false;

        Func<K, bool> includeUpperBound = highIncl
        ? value => _comparer.Compare(value, upperBound) == 0
        : value => false;

        var newSet = new TreeSet<K>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if ((_comparer.Compare(node.Key, lowerBound) > 0 || includeLowerBound(node.Key!))
                && (_comparer.Compare(node.Key, upperBound) < 0 || includeUpperBound(node.Key!)))
                newSet.Add(node.Key);
        }
        return newSet;
    }

    public TreeSet<K> TailMap(K? lowerBound, bool incl)
    {
        if (lowerBound is null)
            throw new ArgumentNullException();

        Func<K, bool> includeLowerBound = incl
        ? value => _comparer.Compare(value, lowerBound) == 0
        : value => false;

        var newSet = new TreeSet<K>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, lowerBound) > 0 || includeLowerBound(node.Key!))
                newSet.Add(node.Key);
        }
        return newSet;
    }

    public bool TryLower(K? key, out K? result)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.LowerEntry(key);
        if (node is null)
        {
            result = default;
            return false;
        }
        result = node.Key;
        return true;
    }
    public bool TryHigher(K? key, out K? result)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.HigherEntry(key);
        if (node is null)
        {
            result = default;
            return false;
        }
        result = node.Key;
        return true;
    }

    public bool TryFloor(K? key, out K? result)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.FloorEntry(key);
        if (node is null)
        {
            result = default;
            return false;
        }
        result = node.Key;
        return true;
    }
    public bool TryCeiling(K? key, out K? result)
    {
        if (key is null)
            throw new ArgumentNullException();

        var node = _tree.CeilingEntry(key);
        if (node is null)
        {
            result = default;
            return false;
        }
        result = node.Key;
        return true;
    }

    public K PollFirst()
    {
        if (IsEmpty()) throw new Exception();
        var minNode = _tree.MinNode();
        _tree.Remove(minNode!.Key);
        return minNode.Key!;
    }

    public K PollLastEntry()
    {
        if (IsEmpty()) throw new Exception();
        var maxNode = _tree.MaxNode();
        _tree.Remove(maxNode!.Key);
        return maxNode.Key!;
    }

    public K First()
    {
        if (IsEmpty()) throw new Exception();
        return _tree.MinNode()!.Key!; ;
    }

    public K Last()
    {
        if (IsEmpty()) throw new Exception();
        return _tree.MaxNode()!.Key!;
    }

    public static TreeSet<K> operator *(TreeSet<K> set1, TreeSet<K> set2)
    {
        if (set1 is null || set2 is null)
            throw new ArgumentNullException();

        var comparer = set1.Comparer;

        var set3 = new TreeSet<K>(comparer);

        var it1 = set1.Iterator();
        var it2 = set2.Iterator();
        if (!it1.HasNext() && it2.HasNext()) return set3;

        var map1HasNext = true;
        var map2HasNext = true;
        while (true)
        {
            if (map1HasNext) it1.Next();
            if (map2HasNext) it2.Next();
            map1HasNext = false;
            map2HasNext = false;

            var cmpResult = comparer.Compare(it1.Current(), it2.Current());

            if (cmpResult > 0)
            {
                if (!it2.HasNext()) break;
                map2HasNext = true;
            }
            else if (cmpResult < 0)
            {
                if (!it1.HasNext()) break;
                map1HasNext = true;
            }
            else
            {
                set3.Add(it1.Current());
                if (!(it1.HasNext() && it2.HasNext())) break;
                map1HasNext = true;
                map2HasNext = true;
            }
        }

        return set3;
    }

    public static TreeSet<K> operator +(TreeSet<K> set1, TreeSet<K> set2)
    {
        if (set1 is null || set2 is null)
            throw new ArgumentNullException();

        var set3 = new TreeSet<K>(set1);

        var it = set2.Iterator();
        while (it.HasNext())
        {
            set3.Add(it.Next());
        }

        return set3;
    }


    public static TreeSet<K> operator -(TreeSet<K> set1, TreeSet<K> set2)
    {
        if (set1 is null || set2 is null)
            throw new ArgumentNullException();

        var set3 = new TreeSet<K>(set1);

        var it = set2.Iterator();
        while (it.HasNext())
        {
            set3.Remove(it.Next());
        }

        return set3;
    }

    public K PoolFirst()
    {
        if(IsEmpty()) throw new Exception();
        var value = _tree.MinNode()!.Key;
        _tree.Remove(value);
        _size--;
        return value!;
    }

    public K PoolLast()
    {
        if (IsEmpty()) throw new Exception();
        var value = _tree.MaxNode()!.Key;
        _tree.Remove(value);
        _size--;
        return value!;
    }

    public TreeSet<K> ReverseSet()
    {
        var reverseSet = new TreeSet<K>(
            new ReverseComparer<K>(_comparer)
        );
        var it = Iterator();
        while (it.HasNext())
        {
            reverseSet.Add(it.Next());
        }
        return reverseSet;
    }

    public void Clear()
    {
        _tree.Clear();
        _size = 0;
    }

    public K[] ToArray()
    {
        var array = new K[_size];
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
            result += $"{it.Next()}, ";
        }
        if(result.Length > 1) 
            result = result.Substring(0, result.Length - 2);
        return result + "]";
    }
}


