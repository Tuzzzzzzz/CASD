using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CASD_Task3
{
    using Sorts;
    using ArrayGenerators;
    using System.Diagnostics;
    using System.IO;
    using ZedGraph;
    using System.Threading;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

    public partial class Form1 : Form
    {
        private TypeOfSorts typeOfSorts;
        private TypeOfArray typeOfArray;
        private string arrayFilePath;
        private int[] powerCnt;
        private long[][] timeData;
        private string[][] sortNames;

        public Form1()
        {
            InitializeComponent();
            arrayFilePath = "arrays.txt";
            sortNames =
                [
                ["Пузырьковая", "Вставками", "Выбором", "Шейкерная", "Гномья"],
                ["Битонная", "Шелла", "Деревом"],
                ["Расчёской", "Пирамидальная", "Быстрая", "Слиянием", "Подсчётом", "Поразрядная"]
                ];
            powerCnt = [5, 5, 5];//10^5 элементов для всех сортировок
            zedGraphControl1.GraphPane.Title.Text = "CASD Task 3";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "количество элементов";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "время, мс";
        }

        //написал асинхронный метод, но он не хочет замерять корректное время
        //матрица timeData заполняется нулями и ничем кроме
        //ниже него есть синхронная реализация - сейчас на запуске она
        private async void RunTimeTest2(Func<int, int, int[]> generateArray, params Func<int[], int[]>[] sorts)
        {
            Stopwatch stopwatch = new Stopwatch();
            int arrayLenght = 10;
            timeData = new long[sorts.Length][];
            for (int i = 0; i < sorts.Length; i++) timeData[i] = new long[powerCnt[(int)typeOfSorts]];
            var tasks = new Task<(long, int[])>[sorts.Length];

            using (var arrayWritter = new StreamWriter(arrayFilePath))
            {

                for (int i = 0; i < powerCnt[(int)typeOfSorts]; i++)
                {
                    int[] array = generateArray(arrayLenght, 1000);
                    arrayWritter.WriteLine($"#Неотсортированный: [{string.Join(", ", array)}]");
                    for (int j = 0; j < sorts.Length; j++)
                    {
                        int[] copiedArray = (int[])array.Clone();
                        var sort = sorts[j];
                        tasks[j] = Task.Run(
                            () => GetSortExecTime(sort, copiedArray)
                        );
                    }
                    var results = await Task.WhenAll(tasks);
                    for (int j = 0; j < results.Length; j++)
                    {
                        var (totalTime, sortedArray) = results[j];
                        timeData[j][i] = totalTime;
                        arrayWritter.WriteLine($"{sortNames[(int)typeOfSorts][j]}: [{string.Join(", ", sortedArray)}]");
                    }
                    arrayLenght *= 10;
                }
            }

        }

        private (long, int[]) GetSortExecTime(Func<int[], int[]> sort, int[] array)
        {
            long totalTime = 0;
            int[] sortedArray = null;
            Stopwatch stopwatch = new Stopwatch();
            for (int k = 0; k < 20; k++)
            {
                stopwatch.Start();
                sortedArray = sort((int[])array.Clone());
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }
            totalTime /= 20;
            return (totalTime, sortedArray);
        }

        private void RunTimeTest(Func<int, int, int[]> generateArray, params Func<int[], int[]>[] sorts)
        {
            Stopwatch stopwatch = new Stopwatch();
            int arrayLenght = 10;
            timeData = new long[sorts.Length][];
            for (int i = 0; i < sorts.Length; i++) timeData[i] = new long[powerCnt[(int)typeOfSorts]];
            using (var arrayWritter = new StreamWriter(arrayFilePath))
            {

                for (int i = 0; i < powerCnt[(int)typeOfSorts]; i++)
                {
                    int[] array = generateArray(arrayLenght, 1000);
                    arrayWritter.WriteLine($"#Неотсортированный: [{string.Join(", ", array)}]");
                    for (int j = 0; j < sorts.Length; j++)
                    {
                        int[] copiedArray = (int[])array.Clone();
                        var (totalTime, sortedArray) = GetSortExecTime(sorts[j], copiedArray);
                        timeData[j][i] = totalTime;
                        arrayWritter.WriteLine($"{sortNames[(int)typeOfSorts][j]}: [{string.Join(", ", sortedArray)}]");
                    }
                    arrayLenght *= 10;
                }
            }
        }

        private void SelectAndRunTimeTest()
        {
            Func<int, int, int[]> generateArray = null;
            switch (typeOfArray)
            {
                case TypeOfArray.Type1:
                    generateArray = ArrayGenerators.GetRandomArray;
                    break;
                case TypeOfArray.Type2:
                    generateArray = ArrayGenerators.GetSortedSubarrayArray;
                    break;
                case TypeOfArray.Type3:
                    generateArray = ArrayGenerators.GetSortArrayWithSwap;
                    break;
                case TypeOfArray.Type4:
                    generateArray = ArrayGenerators.GetRandomArrayWithReapeat;
                    break;
            }
            switch (typeOfSorts)
            {
                case TypeOfSorts.Type1:
                    RunTimeTest(generateArray, Sorts.BubbleSort, Sorts.InsertionSort,
                        Sorts.SelectionSort, Sorts.ShakerSort, Sorts.GnomeSort);
                    break;
                case TypeOfSorts.Type2:
                    RunTimeTest(generateArray, Sorts.BitonicSort, Sorts.ShellSort, Sorts.TreeSort);
                    break;
                case TypeOfSorts.Type3:
                    RunTimeTest(generateArray, Sorts.CombSort, Sorts.HeapSort,
                        Sorts.QuickSort, Sorts.MergeSort, Sorts.CountingSort, Sorts.RadixSort);
                    break;
            }
        }

        private void MakePlot()
        {
            if (timeData.All(arr => arr.All(el => el == 0)))
            {
                MessageBox.Show("Нет данных для рисования графиков.");
                return;
            }

            var pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();
            Color[] colors = { Color.Red, Color.Orange, Color.Gray, Color.Green, Color.Blue, Color.Violet };

            for (int i = 0; i < timeData.Length; i++)
            {
                int arrayLenght = 10;
                PointPairList points = new PointPairList();
                for (int j = 0; j < timeData[0].Length; j++)
                {
                    points.Add(arrayLenght, timeData[i][j]);
                    arrayLenght *= 10;
                }
                pane.AddCurve(sortNames[(int)typeOfSorts][i], points, colors[i], SymbolType.None);
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            typeOfArray = (TypeOfArray)comboBox1.SelectedIndex;
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            typeOfSorts = (TypeOfSorts)comboBox2.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectAndRunTimeTest();
            MakePlot();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFile(arrayFilePath);
        }

        private void OpenFile(string filePath)
        {
            Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });
        }
    }
}
