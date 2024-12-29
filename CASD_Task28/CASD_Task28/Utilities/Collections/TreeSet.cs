namespace Utilities.Collections;

using RBTree;
using System.Collections;
using System.Text;
using Utilities.CollectionInterfaces;


file class ReverseComparer<V>: IComparer<V>
    where V: IComparable<V>
{
    private IComparer<V> _comparer;

    public ReverseComparer(IComparer<V> comparer)
    {
        _comparer = comparer;
    }
    public int Compare(V? x, V? y)
    {
        return (-1)*_comparer.Compare(x, y);
    }
}


public class TreeSet<V> : IMyNavigableSet<V>
    where V : IComparable<V>
{
    private RBTree<V, object> _tree;

    private IComparer<V> _comparer;

    private int _size = 0;


    public int Size() => _size;

    public bool IsEmpty() => _size == 0;

    public IComparer<V> Comparer => _comparer;


    public TreeSet() : this((IComparer<V>?)null) { }

    public TreeSet(IComparer<V>? comparer) : this (null, comparer) { }

    public TreeSet(IMySortedSet<V> sortedSet)
    {
        if (sortedSet == null)
            _comparer = Comparer<V>.Default;
        else
            _comparer = sortedSet.Comparer;

        _tree = new RBTree<V, object>(_comparer);

        AddAll(sortedSet);
    }

    public TreeSet(IMyCollection<V> collection, IComparer<V>? comparer = null)
    {
        if (comparer == null)
            _comparer = Comparer<V>.Default;
        else
            _comparer = comparer;

        _tree = new RBTree<V, object>(_comparer);

        AddAll(collection);
    }


    public Iterator<V> Iterator()
        => new TreeSetItr<V>(_tree.Iterator());

    public Iterator<V> ReverseIterator()
        => new TreeSetItr<V>(_tree.ReverseIterator());

    public IEnumerator<V> GetEnumerator()
    {
        for (var it = Iterator(); it.HasNext();)
        {
            yield return it.Next();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(V value)
    {
        if (value == null)
            throw new ArgumentNullException();

        _tree!.Insert(value, null);
        _size++;
    }

    public void AddAll(IMyCollection<V> collection)
    {
        if (collection == null) return;

        foreach (V el in collection)
        {
            if (el == null)
                throw new ArgumentNullException();
            Add(el);
        }
    }

    public bool Remove(V value)
    {
        if (value == null) return false;

        if (_tree.Remove(value))
        {
            _size--;
            return true;
        }
        return false;
    }

    public bool RemoveAll(IMyCollection<V> collection)
    {
        if (collection == null) return false;
        
        bool allDeleted = true;
        foreach(V el in collection)
        {
            if (!Remove(el)) allDeleted = false;
        }
        return allDeleted;
    }

    public bool Contains(V value)
    {
        if (value == null) return false;

        return _tree.ContainsKey(value);
    }

    public bool ContainsAll(IMyCollection<V> collection)
    {
        if (collection == null) return false;

        foreach (V el in collection)
        {
            if (!Contains(el)) return false;
        }
        return true;
    }

    public void RetainAll(IMyCollection<V> collection)
    {
        if (collection == null)
        {
            Clear();
            return;
        }

        foreach (V item in this)
        {
            bool delete = true;
            foreach (V value in collection)
            {
                if (value == null) continue;

                if (_comparer.Compare(value, item) == 0)
                {
                    delete = false;
                    break;
                }
            }
            if (delete) Remove(item);
        }
    }

    public IMySortedSet<V> HeadSet(V upperBound, bool incl)
    {
        if (upperBound == null)
            throw new ArgumentNullException();

        Func<V, bool> includeUpperBound = incl
        ? value => _comparer.Compare(value, upperBound) == 0
        : value => false;

        var newSet = new TreeSet<V>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, upperBound) < 0 || includeUpperBound(node.Key!))
                newSet.Add(node.Key!);
        }
        return newSet;
    }

    public IMySortedSet<V> SubSet(V lowerBound, bool lowIncl, V upperBound, bool highIncl)
    {
        if (lowerBound == null || upperBound == null)
            throw new ArgumentNullException();

        Func<V, bool> includeLowerBound = lowIncl
        ? value => _comparer.Compare(value, lowerBound) == 0
        : value => false;

        Func<V, bool> includeUpperBound = highIncl
        ? value => _comparer.Compare(value, upperBound) == 0
        : value => false;

        var newSet = new TreeSet<V>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if ((_comparer.Compare(node.Key, lowerBound) > 0 || includeLowerBound(node.Key!))
                && (_comparer.Compare(node.Key, upperBound) < 0 || includeUpperBound(node.Key!)))
                newSet.Add(node.Key!);
        }
        return newSet;
    }

    public IMySortedSet<V> TailSet(V lowerBound, bool incl)
    {
        if (lowerBound == null)
            throw new ArgumentNullException();

        Func<V, bool> includeLowerBound = incl
        ? value => _comparer.Compare(value, lowerBound) == 0
        : value => false;

        var newSet = new TreeSet<V>(_comparer);

        var it = _tree.Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            if (_comparer.Compare(node.Key, lowerBound) > 0 || includeLowerBound(node.Key!))
                newSet.Add(node.Key!);
        }
        return newSet;
    }

    public bool TryLower(V bound, out V value)
    {
        if (bound == null)
            throw new ArgumentNullException();

        var node = _tree.LowerEntry(bound);
        if (node is null)
        {
            value = default;
            return false;
        }
        value = node.Key!;
        return true;
    }

    public bool TryHigher(V bound, out V value)
    {
        if (bound == null)
            throw new ArgumentNullException();

        var node = _tree.HigherEntry(bound);
        if (node is null)
        {
            value = default;
            return false;
        }
        value = node.Key!;
        return true;
    }

    public bool TryFloor(V bound, out V value)
    {
        if (bound == null)
            throw new ArgumentNullException();

        var node = _tree.FloorEntry(bound);
        if (node is null)
        {
            value = default;
            return false;
        }
        value = node.Key!;
        return true;
    }

    public bool TryCeiling(V bound, out V value)
    {
        if (bound == null)
            throw new ArgumentNullException();

        var node = _tree.CeilingEntry(bound);
        if (node is null)
        {
            value = default;
            return false;
        }
        value = node.Key!;
        return true;
    }

    public V First()
    {
        if (IsEmpty()) throw new Exception();
        return _tree.MinNode()!.Key!;
    }

    public V Last()
    {
        if (IsEmpty()) throw new Exception();
        return _tree.MaxNode()!.Key!;
    }

    public static TreeSet<V> operator *(TreeSet<V> set1, TreeSet<V> set2)
    {
        if (set1 == null || set2 == null)
            throw new ArgumentNullException();

        var comparer = set1.Comparer;

        var set3 = new TreeSet<V>(comparer);

        var it1 = set1.Iterator();
        var it2 = set2.Iterator();
        if (!(it1.HasNext() && it2.HasNext())) return set3;

        bool map1HasNext = true, map2HasNext = true;
        V item1 = it1.Next(), item2 = it2.Next();
        while (true)
        {
            if (map1HasNext) item1 = it1.Next();
            if (map2HasNext) item2 = it2.Next();
            map1HasNext = false;
            map2HasNext = false;

            var cmpResult = comparer.Compare(item1, item2);

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
                set3.Add(item1);
                if (!(it1.HasNext() && it2.HasNext())) break;
                map1HasNext = true;
                map2HasNext = true;
            }
        }

        return set3;
    }

    public static TreeSet<V> operator +(TreeSet<V> set1, TreeSet<V> set2)
    {
        if (set1 == null || set2 == null)
            throw new ArgumentNullException();

        var set3 = new TreeSet<V>(set1);
        set3.AddAll(set2);
        return set3;
    }


    public static TreeSet<V> operator -(TreeSet<V> set1, TreeSet<V> set2)
    {
        if (set1 == null || set2 == null)
            throw new ArgumentNullException();

        var set3 = new TreeSet<V>(set1);
        set3.RemoveAll(set2);
        return set3;
    }

    public V PoolFirst()
    {
        if(IsEmpty()) throw new Exception();
        var value = _tree.MinNode()!.Key;
        _tree.Remove(value);
        _size--;
        return value!;
    }

    public V PoolLast()
    {
        if (IsEmpty()) throw new Exception();
        var value = _tree.MaxNode()!.Key;
        _tree.Remove(value);
        _size--;
        return value!;
    }

    public TreeSet<V> ReverseSet()
    {
        var reverseSet = new TreeSet<V>(
            new ReverseComparer<V>(_comparer)
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

    public V[] ToArray()
    {
        var array = new V[Size()];
        int i = 0;
        foreach (V item in this)
        {
            array[i++] = item;
        }
        return array;
    }

    public V[] ToArray(V[] array)
    {
        if (array == null || array.Length < Size()) return ToArray();

        int i = 0;
        foreach (V item in array) array[i++] = item;
        return array;
    }

    public override string ToString()
    {
        if (IsEmpty()) return "[]";

        var sb = new StringBuilder(Size() * 3 + 2);
        sb.Append("[");
        foreach (V item in this)
        {
            sb.Append($"({item}, )");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append("]");
        return sb.ToString();
    }
}


