namespace Utilities.CollectionInterfaces;

public interface IMyDeque<T> : IMyQueue<T>
{
    T GetFirst();

    T GetLast();

    T Pop();

    void Push(T value);

    void AddFirst(T value);

    void AddLast(T value);

    T RemoveFirst();

    T RemoveLast();

    bool OfferFirst(T value);

    bool OfferLast(T value);

    bool TryPeekFirst(out T headValue);

    bool TryPeekLast(out T tailValue);

    bool TryPoolFirst(out T headValue);

    bool TryPoolLast(out T tailValue);

    bool RemoveFirstOccurence(T value);

    bool RemoveLastOccurence(T value);
}