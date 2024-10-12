using MyPriorityQueue;

var pq = new MyPriorityQueue<int>(
    [1, 5, 10, 8, 2, 2, 2, 11],
    //сортировка по остаткам от пяти
    (a, b) => a % 5 > b % 5 ? 1 : -1
);

while (!pq.Empty) {
    var d = pq.Pool();
    Console.WriteLine(
        $"Значение: {d} Остаток: {d%5}"
    );
};