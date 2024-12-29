namespace Utilities.CollectionInterfaces;

public interface Iterable<T>: IEnumerable<T>
{
    Iterator<T> Iterator();
}