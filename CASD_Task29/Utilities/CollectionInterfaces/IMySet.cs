using Utilities.Collections;

namespace Utilities.CollectionInterfaces;


public interface IMySet<V> : IMyCollection<V>
{
    IMySet<V> MakeSetIntersect(IMySet<V> otherSet)
    {
        if (otherSet == null) return new MyHashSet<V>();

        var newSet = new MyHashSet<V>();
        foreach (V value in otherSet)
        {
            if (Contains(value)) newSet.Add(value);
        }
        return newSet;
    }

    IMySet<V> MakeSetUnion(IMySet<V> otherSet)
    {
        if (otherSet == null) return new MyHashSet<V>(this);

        var newSet = new MyHashSet<V>(this);
        newSet.AddAll(otherSet);
        return newSet;
    }

    IMySet<V> MakeSetDifference(IMySet<V> otherSet)
    {
        if (otherSet == null) return new MyHashSet<V>(this);

        var newSet = new MyHashSet<V>(this);
        newSet.RemoveAll(otherSet);
        return newSet;
    }
}