namespace Utilities.CollectionInterfaces;

public interface IMyNavigableMap<K, V> : IMySortedMap<K, V>
{
    (K, V)? FirstEntry();

    (K, V)? LastEntry();

    (K, V)? LowerEntry(K bound);

    (K, V)? HigherEntry(K bound);

    (K, V)? FloorEntry(K bound);

    (K, V)? CeilingEntry(K bound);

    (K, V)? PollFirstEntry();

    (K, V)? PollLastEntry();

    bool TryLowerKey(K bound, out K key);

    bool TryHigherKey(K bound, out K key);

    bool TryFloorKey(K bound, out K key);

    bool TryCeilingKey(K bound, out K key);
}