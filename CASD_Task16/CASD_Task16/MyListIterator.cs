public interface MyListIterator<T> : MyIterator<T>
{
    public bool HasPrevious();

    public T Previous();

    public int NextIndex();

    public int PreviousIndex();

    public void Set(T value);

    public void Add(T value);
}
