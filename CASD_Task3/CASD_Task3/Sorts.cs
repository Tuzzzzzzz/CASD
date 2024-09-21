using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sorts;
using BinaryTree;
using BinaryHeap;
using MyArrayList;
static internal class Sorts
{
    public static void Swap(ref int a, ref int b)
    {
        int tmp = a;
        a = b;
        b = tmp;
    }

    public static int[] BubbleSort(int[] array)
    {
        array = (int[])array.Clone();

        for (int i = 0; i < array.Length - 1; i++)
            for (int j = i + 1; j < array.Length; j++)
                if (array[i] > array[j]) Swap(ref array[i], ref array[j]);

        return array;
    }


    public static int[] ShakerSort(int[] array)
    {
        array = (int[])array.Clone();

        int leftPtr = 0, rightPtr = array.Length - 1, lastPtr;

        while (leftPtr < rightPtr)
        {
            lastPtr = -1;

            for (int i = leftPtr; i < rightPtr; i++)
            {
                if (array[i] > array[i + 1])
                {
                    Swap(ref array[i], ref array[i + 1]);
                    lastPtr = i;
                }
            }

            if (lastPtr == -1) break;

            rightPtr = lastPtr;

            for (int i = rightPtr; i >= leftPtr; i--)
            {
                if (array[i] > array[i + 1])
                {
                    Swap(ref array[i], ref array[i + 1]);
                    lastPtr = i;
                }
            }

            leftPtr = lastPtr + 1;

        }
        return array;
    }


    public static int[] CombSort(int[] array)
    {
        array = (int[])array.Clone();

        double step = array.Length - 1;
        bool isSwap = true;

        while (step > 1 || isSwap)
        {
            isSwap = false;
            step /= 1.25;
            int intStep = (int)step;

            for (int i = 0; i + intStep < array.Length; i++)
            {
                if (array[i] > array[i + intStep])
                {
                    Swap(ref array[i], ref array[i + intStep]);
                    isSwap = true;
                }
            }
        }
        return array;
    }


    public static int[] InsertionSort(int[] array)
    {
        array = (int[])array.Clone();

        for (int i = 1; i < array.Length; i++)
        {
            int j = i - 1;
            int cmpElement = array[i];

            while (j >= 0 && array[j] > cmpElement)
            {
                array[j + 1] = array[j];
                j--;
            }

            array[j + 1] = cmpElement;
        }
        return array;
    }


    public static int[] ShellSort(int[] array)
    {
        array = (int[])array.Clone();

        for (int s = array.Length / 2; s > 0; s /= 2)
            for (int i = 0; i + s < array.Length; i++)
                for (int j = i; j >= 0 && array[j] > array[j + s]; j -= s)
                    Swap(ref array[j], ref array[j + s]);
                 
        return array;
    }


    public static int[] TreeSort(int[] array)
    {
        array = (int[])array.Clone();
        BynaryTree tree = new BynaryTree(array);
        return tree.ToArray();
    }


    public static int[] GnomeSort(int[] array)
    {
        array = (int[])array.Clone();
        int ptr = 0;
        while(ptr < array.Length)
        {
            if (ptr == 0 || array[ptr-1] <= array[ptr]) ptr++;
            else
            {
                Swap(ref array[ptr-1], ref array[ptr]); 
                ptr--;
            }
        }
        return array;
    }


    public static int[] SelectionSort(int[] array)
    {
        array = (int[])array.Clone();
        for (int i = 0; i < array.Length - 1; i++)
        {
            int min = array[i]; int indMin = i;
            for (int j = i + 1; j < array.Length; j++)
            {
                if (array[j] < min)
                {
                    min = array[j]; indMin = j;
                }
            }
            Swap(ref array[indMin], ref array[i]);
        }
        return array;
    }


    public static int[] HeapSort(int[] array)
    {
        array = (int[])array.Clone();
        BinaryHeap heap = new BinaryHeap(array);

        for (int i = array.Length - 1; i >= 0; i--) 
            array[i] = heap.GetMax();

        return array;
    }
    public static int[] QuickSort(int[] array)
    {
        return QuickSort(array, 0, array.Length);
    }

    public static int[] QuickSort(int[] array, int begin, int cnt)
    {
        void RecursionQuickSort(int begin, int end, int[] array)
        {
            if (begin >= end) return;

            int cmpElemIndex = new Random().Next(begin, end + 1);
            int cmpElem = array[cmpElemIndex];

            Swap(ref array[begin], ref array[cmpElemIndex]);
            int lastLessIndex = begin;

            for (int i = begin + 1; i <= end; i++)
            {
                if (array[i] < cmpElem)
                    Swap(ref array[i], ref array[++lastLessIndex]);
            }

            Swap(ref array[begin], ref array[lastLessIndex]);

            RecursionQuickSort(lastLessIndex + 1, end, array);
            RecursionQuickSort(begin, lastLessIndex - 1, array);
        }

        array = (int[])array.Clone();
        RecursionQuickSort(begin, begin + cnt - 1, array);
        return array;
    }


    public static int[] MergeSort(int[] array)
    {
        int[] RecursionMergeSort(int[] array)
        {
            int length = array.Length;
            if (length == 0 || length == 1) return array;

            int leftArrayLenght = length / 2;

            int[] leftArray = new int[leftArrayLenght];
            Array.Copy(array, 0, leftArray, 0, leftArrayLenght);
            leftArray = RecursionMergeSort(leftArray);

            int[] rightArray = new int[length - leftArrayLenght];
            Array.Copy(array, leftArrayLenght, rightArray, 0, length - leftArrayLenght);
            rightArray = RecursionMergeSort(rightArray);

            int[] resultArray = new int[length];
            int leftArrayIndex = 0;
            int rightArrayIndex = 0;
            int resultArrayIndex = 0;
            while (leftArrayIndex < leftArrayLenght && rightArrayIndex < length - leftArrayLenght)
            {
                if (leftArray[leftArrayIndex] <= rightArray[rightArrayIndex])
                {
                    resultArray[resultArrayIndex] = leftArray[leftArrayIndex];
                    leftArrayIndex++;
                }
                else
                {
                    resultArray[resultArrayIndex] = rightArray[rightArrayIndex];
                    rightArrayIndex++;
                }
                resultArrayIndex++;
            }
            while (leftArrayIndex < leftArrayLenght)
            {
                resultArray[resultArrayIndex] = leftArray[leftArrayIndex];
                leftArrayIndex++;
                resultArrayIndex++;
            }
            while (rightArrayIndex < length - leftArrayLenght)
            {
                resultArray[resultArrayIndex] = rightArray[rightArrayIndex];
                rightArrayIndex++;
                resultArrayIndex++;
            }
            return resultArray;
        }

        return RecursionMergeSort(array);
    }


    public static int[] CountingSort(int[] array) 
    {
        /*Сортировка не подходит для массивов
         * с большим разбросом значений элементов
         * с отрицательнами элементами
         * с числами с плавающей запятой*/

        array = (int[])array.Clone();
        int minElem = array.Min();
        int maxElem = array.Max();
        int[] helperArray = new int[maxElem - minElem+1];
        for(int i = 0; i < array.Length; i++)
        {
            helperArray[array[i] - minElem]++;
        }
        int ind = 0;
        for(int i = 0; i < helperArray.Length; i++)
        {
            int cnt = helperArray[i];
            while (cnt != 0)
            {
                cnt--;
                array[ind++] = i + minElem;
            }
        }
        return array;
    }


    public static int[] RadixSort(int[] array)
    {
        /*Сортировка не подходит для массивов
         * с отрицательнами элементами
         * с числами с плавающей запятой*/

        int cntDigit(int number)
        {
            int cnt = 0;
            foreach (char _ in number.ToString()) cnt++;
            return cnt;
        }

        int DigitAt(int number, int index)
            => (number / (int)Math.Pow(10, index)) % 10;

        array = (int[])array.Clone();
        int maxCntDigit = cntDigit(array.Max());
        MyArrayList<int>[] radix = new MyArrayList<int>[10];
        for(int i = 0; i < radix.Length; i++) radix[i] = new MyArrayList<int>();

        for(int i = 0; i < maxCntDigit; i++) 
        {
            foreach (MyArrayList<int> arrayList in radix) arrayList.Clear();

            foreach (int number in array) radix[DigitAt(number, i)].Add(number);

            int k = 0;
            foreach (MyArrayList<int> numbers in radix)
            {
                for(int j = 0; j < numbers.Size(); j++)
                {
                    array[k] = numbers[j];
                    k++;
                }
            }
        }
        return array;
    }

    public static int[] BitonicSort(int[] array)
    {
        bool IsNaturalPowerOfTwo(int cnt, out int n)
        {
            n = 2; while (n < cnt) n *= 2;
            return n == cnt;
        }

        void GrowthDeclineSort(int begin, int end, int[] array, bool isGrow)
        {
            if (end - begin < 2) return;
            for (int i = begin, j = begin + (end - begin) / 2; j < end; i++, j++) 
                if (array[i] > array[j] && isGrow || array[i] < array[j] && !isGrow) Swap(ref array[i], ref array[j]);
            GrowthDeclineSort(begin, end - (end - begin) / 2, array, isGrow); 
            GrowthDeclineSort(begin + (end - begin) / 2, end, array, isGrow);
        }

        int lenght = array.Length; int begin = 0;
        int newLength;
        int[] newArray;

        if (!IsNaturalPowerOfTwo(array.Length, out newLength))
        {
            begin = newLength - array.Length;
            int min = array.Min(); newArray = new int[newLength];
            Array.Copy(array, 0, newArray, begin, lenght); for (int i = 0; i < begin; i++) newArray[i] = min;
        }
        else newArray = (int[])array.Clone();

        int step = 2;
        while (step <= newArray.Length)
        {
            bool isGrowth = true;
            for (int i = 0, j = step; j <= newArray.Length; i += step, j += step)
            {
                GrowthDeclineSort(i, j, newArray, isGrowth);
                if (isGrowth) isGrowth = false; else isGrowth = true;
            }
            step *= 2;
        }

        array = new int[lenght]; 
        Array.Copy(newArray, begin, array, 0, lenght);
        return array;
    }
}


