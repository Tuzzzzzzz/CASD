using System.Runtime.CompilerServices;

namespace Utilities.CollectionInterfaces;

public interface IMyList<T> : IMyCollection<T>
{
    T this[int index] { get; set; }

    void Add(int index, T value);

    void AddAll(int index, IMyCollection<T> collection);

    void RemoveAt(int index);

    int IndexOf(T value);

    int LastIndexOf(T value);

    ListIterator<T> ListIterator();

    ListIterator<T> ListIterator(int index);

    IMyList<T> SubList(int fromIndex, int toIndex);
}