using MyLinkedList;

var list = new MyLinkedList<int>([1,2,3,4]);
foreach(var el in list.Where(p => p%2 == 0))
{
    Console.WriteLine(el);
}


