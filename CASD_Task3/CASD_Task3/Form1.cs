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
        private double[][] timeData;
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
            powerCnt = [4, 5, 6];
            zedGraphControl1.GraphPane.Title.Text = "Зависимость времени выполнения сортировок от размера массива";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "количество элементов сортируемых массивов, шт";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "время, мс";
        }

     
        private async Task /*void*/ RunTimeTest(Func<int, int, int[]> generateArray, params Func<int[], int[]>[] sorts)
        {
            int arrayLenght = 10;
            timeData = new double[sorts.Length][];
            for (int i = 0; i < sorts.Length; i++) timeData[i] = new double[powerCnt[(int)typeOfSorts]];
            var tasks = new Task<(double, int[])>[sorts.Length];

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
                        //var(totalTime, sortedArray) = GetSortExecTime(sort, copiedArray);
                        //timeData[j][i] = totalTime;
                        //arrayWritter.WriteLine($"{sortNames[(int)typeOfSorts][j]}: [{string.Join(", ", sortedArray)}] за время {totalTime}мс");
                    }
                    var results = await Task.WhenAll(tasks);
                    for (int j = 0; j < results.Length; j++)
                    {
                        var (totalTime, sortedArray) = results[j];
                        timeData[j][i] = totalTime;
                        arrayWritter.WriteLine($"{sortNames[(int)typeOfSorts][j]}: [{string.Join(", ", sortedArray)}] за время {totalTime}мс");
                    }
                    arrayLenght *= 10;
                }
            }

        }

        private (double, int[]) GetSortExecTime(Func<int[], int[]> sort, int[] array)
        {
            double totalTime = 0;
            int[] sortedArray = null;
            Stopwatch stopwatch = new Stopwatch();

            for (int k = 0; k < 20; k++)
            {
                int[] clonedArray = (int[])array.Clone();
                stopwatch.Start();
                sortedArray = sort(clonedArray);
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalMilliseconds;
                stopwatch.Reset();
            }
            totalTime /= 20;
            return (totalTime, sortedArray);
        }


        private async Task /*void*/ SelectAndRunTimeTest()
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
                    await RunTimeTest(generateArray, Sorts.BubbleSort, Sorts.InsertionSort,
                        Sorts.SelectionSort, Sorts.ShakerSort, Sorts.GnomeSort);
                    break;
                case TypeOfSorts.Type2:
                    await RunTimeTest(generateArray, Sorts.BitonicSort, Sorts.ShellSort, Sorts.TreeSort);
                    break;
                case TypeOfSorts.Type3:
                    await RunTimeTest(generateArray, Sorts.CombSort, Sorts.HeapSort,
                        Sorts.QuickSort, Sorts.MergeSort, Sorts.CountingSort, Sorts.RadixSort);
                    break;
            }
        }

        private void MakePlot()
        {
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
            pane.XAxis.Scale.Min = 0;
            pane.XAxis.Scale.Max = Math.Pow(10, timeData[0].Length);
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

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            await SelectAndRunTimeTest();
            MakePlot();
            button1.Enabled = true;
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
