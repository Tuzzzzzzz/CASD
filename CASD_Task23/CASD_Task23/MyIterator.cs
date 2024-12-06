public interface MyIterator<T>
{
    public T Next();

    public bool HasNext();

    public void Remove();
}