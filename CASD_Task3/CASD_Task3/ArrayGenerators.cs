using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArrayGenerators;
using Sorts;
static internal class ArrayGenerators
{
    public static int[] GetRandomArray(int size, int module = 1000)
    {
        int[] array = new int[size];
        Random rand = new Random();
        for (int i = 0; i < size; i++) 
            array[i] = rand.Next(0, module);
        return array;
    }


    public static int[] GetSortedSubarrayArray(int size, int module = 1000)
    {
        int[] array = GetRandomArray(size, module);
        Random rand = new Random();
        int subarrayCnt = rand.Next(5,10);
        int begin = 0;
        bool exit = false;
        for (int k = 0; !exit && k < subarrayCnt; k++)
        {
            int itemCnt = rand.Next(0, size/2);
            if (begin + itemCnt > array.Length)
            {
                exit = true;
                itemCnt = array.Length - begin - 1;
            } 
            array = Sorts.QuickSort(array, begin, itemCnt);
            begin += itemCnt;
        }
        if(begin < array.Length) 
            array = Sorts.QuickSort(array, begin, array.Length - begin - 1);
        return array;
    }


    public static int[] GetSortArrayWithSwap(int size, int module = 1000)
    {
        int[] array = GetRandomArray(size, module);
        array = Sorts.QuickSort(array);
        Random rand = new Random();
        int swapCnt = rand.Next(size/10, size);
        for(int k = 0; k < swapCnt; k++)
        {
            int i = rand.Next(0, size);
            int j = size - rand.Next(1, size);
            Sorts.Swap(ref array[i], ref array[j]);
        }
        return array;
    }


    public static int[] GetRandomArrayWithReapeat(int size, int module = 1000) 
    {
        int[] array = GetRandomArray(size, module);
        Random rand = new Random();
        int reapeatItem = array[rand.Next(0, size - 1)];
        int cnt = rand.Next(size / 10, size / 9 * 10);
        for(int i = rand.Next(0, size / 10); cnt != 0; i += rand.Next(0, size / 10))
        {
            if (i >= size)
            {
                i = rand.Next(0, size / 10);
                continue;
            }
            array[i] = reapeatItem;
            cnt--;
        }
        return array;
    }
}