namespace Utilities.CollectionInterfaces;

public interface IMySortedMap<K, V> : IMyMap<K, V>
{
    IComparer<K> Comparer { get; }

    K FirstKey();

    K LastKey();

    IMySortedMap<K, V> SubMap(K lowerBound, bool lowIncl, K upperBound, bool highIncl);

    IMySortedMap<K, V> HeadMap(K upperBound, bool incl);

    IMySortedMap<K, V> TailMap(K lowerBound, bool incl);
}