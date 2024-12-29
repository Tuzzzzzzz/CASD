namespace Utilities.CollectionInterfaces;

public interface IMyQueue<T> : IMyCollection<T>
{
    T Element();

    bool Offer(T value);

    bool TryPeek(out T headValue);

    bool TryPool(out T headValue);
}