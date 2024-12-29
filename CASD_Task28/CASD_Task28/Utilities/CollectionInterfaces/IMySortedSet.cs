namespace Utilities.CollectionInterfaces;

public interface IMySortedSet<V> : IMySet<V>
{
    IComparer<V> Comparer { get; }

    V First();

    V Last();

    IMySortedSet<V> SubSet(V lowerBound, bool lowIncl, V upperBound, bool highIncl);

    IMySortedSet<V> HeadSet(V upperBound, bool incl);

    IMySortedSet<V> TailSet(V lowerBound, bool incl);
}
