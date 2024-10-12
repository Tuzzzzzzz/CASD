using System;

namespace MaxHeap;

internal class MaxHeap<T> where T : IComparable
{
    private T[] itemData;
    private int count = 0;

    public MaxHeap(T[] array)
    {
        if(array == null) throw new ArgumentNullException();

        itemData = (T[])array.Clone();
        count = array.Length;
        OrderAll();
    }

    public int Count => count;

    public void Add(T value)
    {
        if (count == itemData.Length)
        {
            AllocateMemory();
        }
        itemData[count++] = value;
        Emerge(count - 1);
    }

    public void Add(MaxHeap<T> heap)
    {
        T[] array = heap.ToArray();
        foreach (T item in array) Add(item);
    }

    private void AllocateMemory()
    {
        T[] newItemData = (count < 64) ?
            new T[itemData.Length * 2] : new T[(int)(itemData.Length * 1.5)];

        Array.Copy(itemData, newItemData, itemData.Length);
        itemData = newItemData;
    }

    public void IncreaseAt(int index, T newValue)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (newValue.CompareTo(itemData[index]) < 0) throw new ArgumentException();

        itemData[index] = newValue;
        Emerge(index);
    }

    public T RemoveMax()
    {
        if(count == 0) throw new IndexOutOfRangeException();

        T removedItem = itemData[0];
        if (count > 1)
        {
            itemData[0] = itemData[count - 1];
            itemData[count - 1] = default;
            if (count > 1) Heapify(0);
        }
        else itemData[0] = default;
        count--;
        return removedItem;
    }

    public T GetMax() => itemData[0];

    private void OrderAll()
    {
        for (int i = count / 2; i >= 0; i--) Heapify(i);
    }

    private void Emerge(int valueIndex)
    {
        if (valueIndex == 0) return;
        int parentIndex = (valueIndex - 1) / 2;
        while (valueIndex >= 0)
        {
            if (itemData[valueIndex].CompareTo(itemData[parentIndex]) > 0)
            {
                Swap(valueIndex, parentIndex);

                valueIndex = parentIndex;
                parentIndex = (valueIndex - 1) / 2;
            }
            else break;
        }
    }

    private void Heapify(int valueIndex)
    {
        while (true)
        {
            int leftChildIndex = valueIndex * 2 + 1;
            int rightChildIndex = valueIndex * 2 + 2;
            int largestItemIndex = valueIndex;

            if (leftChildIndex < count && itemData[leftChildIndex].CompareTo(itemData[largestItemIndex]) > 0)
                largestItemIndex = leftChildIndex;

            if (rightChildIndex < count && itemData[rightChildIndex].CompareTo(itemData[largestItemIndex]) > 0)
                largestItemIndex = rightChildIndex;

            if (largestItemIndex == valueIndex) break;

            Swap(valueIndex, largestItemIndex);

            valueIndex = largestItemIndex;
        }
    }

    private void Swap(int index1, int index2)
    {
        if (index1 < 0 || index2 < 0 || index1 >= count || index2 >= count)
            throw new ArgumentOutOfRangeException();
        T tmp = itemData[index1];
        itemData[index1] = itemData[index2];
        itemData[index2] = tmp;
    }

    public T[] ToArray()
    {
        T[] newItemData = new T[count];
        Array.Copy(itemData, newItemData, count);
        return newItemData;
    }

    public override string ToString()
        => $"[{string.Join(", ", ToArray())}]";
}