namespace Utilities.CollectionInterfaces;

public interface IMyMap<K, V> : Iterable<(K, V)>
{
    int Size();

    bool IsEmpty();

    V this[K key] { get; set; }

    void Put(K key, V value);

    void PutAll(IMyMap<K, V> map);

    V Remove(K key);

    bool ContainsKey(K key);

    bool ContainsValue(V value);

    IMySet<(K, V)> EntrySet();

    IMySet<K> KeySet();

    IMySet<V> ValueSet();

    void Clear();

    (K, V)[] ToArray();
}