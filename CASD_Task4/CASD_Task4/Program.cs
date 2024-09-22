using MyArrayList;

namespace CASD_Task4;
internal class Program
{
    public static void Main(string[] args)
    {
        var arrayList = new MyArrayList<char>(['a', 'b', 'c', 'c', 'd']);
        arrayList.RetainAll(['a', 'c']);
        Console.WriteLine(arrayList);
    }
}