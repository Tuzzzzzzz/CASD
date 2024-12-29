using Utilities.CollectionInterfaces;

public interface IMyNavigableSet<V> : IMySortedSet<V>
{
    bool TryLower(V bound, out V value);

    bool TryHigher(V bound, out V value);

    bool TryFloor(V bound, out V value);

    bool TryCeiling(V bound, out V value);

    V PoolFirst();

    V PoolLast();
}