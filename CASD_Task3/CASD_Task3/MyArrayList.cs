using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MyArrayList;
internal class MyArrayList<T>
{
    private T[] storage;
    private int size;

    public MyArrayList()
    {
        storage = new T[10];
        size = 0;
    }

    public MyArrayList(T[] array) : this() 
    {
        foreach (T element in array) Add(element);
    }

    public int Size() => size;

    public void Add(T value)
    {
        if (size == storage.Length) AllocateMemory();
        storage[size] = value;
        size++;
    }

    public void Insert(int index, T value)
    {
        if (index < 0 || index >= size) throw new IndexOutOfRangeException(); 
        if (size == storage.Length) AllocateMemory();
        for (int i = size; i > index; i--)
        {
            storage[i] = storage[i-1];
        }
        storage[index] = value;
        size++;
    }

    public T this[int index] { 
        get {
            if (index < 0 || index >= size) throw new IndexOutOfRangeException();
            return storage[index];
        }
        set { 
            if (index < 0 || index >= size) throw new IndexOutOfRangeException();
            storage[index] = value;
        }
    }

    private void AllocateMemory()
    {
        T[] newStorage = new T[storage.Length * 2];
        Array.Copy(storage, newStorage, storage.Length);
        storage = newStorage;
    } 

    public T RemoveAt(int index)
    {
        if (index < 0 || index >= size) throw new IndexOutOfRangeException();
        T removeElem = storage[index];
        for (int i = index; i < size - 1; i++)
        {
            storage[i] = storage[i+1];
        }
        storage[--size] = default;
        return removeElem;
    }

    public T[] ToArray() 
    {
        T[] newStorage = new T[size];
        Array.Copy(storage, newStorage, size);
        return newStorage;
    }

    public void Clear()
    {
        Array.Clear(storage, 0, size);
        size = 0;
    }

    public override string  ToString() 
        => $"[{string.Join(", ", ToArray())}]";
}
