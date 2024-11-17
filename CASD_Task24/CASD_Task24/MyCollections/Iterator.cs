namespace MyCollections;

public interface Iterator<T>
{
    public bool HasNext();

    public T Next();

    public T Current();

    public void Remove();
}
