namespace Utilities.CollectionInterfaces;

public interface Iterator<T>
{
    public T Next();

    public bool HasNext();

    public void Remove();
}