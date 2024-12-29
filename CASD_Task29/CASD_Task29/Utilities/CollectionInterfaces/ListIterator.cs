namespace Utilities.CollectionInterfaces;

public interface ListIterator<T> : Iterator<T>
{
    public int NextIndex();

    public int PreviousIndex();

    public bool HasPrevious();

    public T Previous();

    public void Add(T value);

    public void Set(T value);

}
