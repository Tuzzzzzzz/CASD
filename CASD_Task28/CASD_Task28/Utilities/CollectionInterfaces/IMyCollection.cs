namespace Utilities.CollectionInterfaces;

public interface IMyCollection<T> : Iterable<T>
{
    int Size();

    bool IsEmpty();

    void Add(T value);

    void AddAll(IMyCollection<T> collection);

    bool Contains(T value);

    bool ContainsAll(IMyCollection<T> collection);

    bool Remove(T value);

    bool RemoveAll(IMyCollection<T> collection);

    void RetainAll(IMyCollection<T> collection);

    T[] ToArray();

    T[] ToArray(T[] array);

    void Clear();
}