using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryHeap;
using MyArrayList;
internal class BinaryHeap
{
    private MyArrayList<int> arrayList;

    public BinaryHeap(int[] array)
    {
        arrayList = new MyArrayList<int>(array);

        for (int i = Size()/2; i >= 0; i--)
        {
            Heapify(i);
        }
    }

    public int Size() => arrayList.Size();

    public void Add(int value)
    {
        arrayList.Add(value);
        int i = Size()-1;
        int j = (i - 1) / 2;
        while(j >= 0)
        {
            if (arrayList[j] < arrayList[i])
            {
                int tmp = arrayList[j];
                arrayList[j] = arrayList[i];
                arrayList[i] = tmp;

                i = j;
                j = (i - 1) / 2;
            }
            else break;
        }
    }

    private void Heapify(int i)
    {
        while(true)
        {
            int leftChildIndex = i * 2 + 1;
            int rightChildIndex = i * 2 + 2;
            int largestElemIndex = i;

            if (leftChildIndex < Size() && arrayList[leftChildIndex] > arrayList[largestElemIndex])
                largestElemIndex = leftChildIndex;

            if (rightChildIndex < Size() && arrayList[rightChildIndex] > arrayList[largestElemIndex]) 
                largestElemIndex = rightChildIndex;

            if(largestElemIndex == i) break;
            
            int tmp = arrayList[largestElemIndex];
            arrayList[largestElemIndex] = arrayList[i];
            arrayList[i] = tmp;

            i = largestElemIndex;
        }
    }

    public int GetMax()
    {
        int maxElem = arrayList[0];
        int lastElem = arrayList.RemoveAt(Size()-1);
        if (Size() > 0)
        {
            arrayList[0] = lastElem;
            Heapify(0);
        }
        return maxElem;
    }
}
