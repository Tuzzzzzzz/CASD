using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    static class VectorCalculator
    {
        private static bool IsSymmetrycal(double[][] matrix)
        {
            int length = matrix.Length;

            for (int i = 1; i < length; i++)
                for (int j = 0; j < i; j++)
                    if (matrix[i][j] != matrix[j][i]) return false;

            return true;
        }

        public static double CalculateVectorLength(double[][] matrix, double[] vector)
        {
            int length = matrix.Length;

            if (!IsSymmetrycal(matrix)) throw new Exception("Not symmetrical");

            double[] newVector = new double[length];

            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    newVector[i] += vector[j] * matrix[i][j];

            double resultOfMultiplication = 0;

            for (int i = 0; i < length; i++)
                resultOfMultiplication += newVector[i] * vector[i];

            return Math.Sqrt(resultOfMultiplication);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            const string fileName = "file.txt";

            try
            {
                var streamReader = new StreamReader(fileName);
                int sizeOfMatrix = Convert.ToInt32(streamReader.ReadLine());

                double[][] matrix = new double[sizeOfMatrix][];
                double[] vector = new double[sizeOfMatrix];

                for (int i = 0; i < sizeOfMatrix; i++)
                    matrix[i] = streamReader.ReadLine().Split().Select(x => Convert.ToDouble(x)).ToArray();

                vector = streamReader.ReadLine().Split().Select(x => Convert.ToDouble(x)).ToArray();

                Console.WriteLine($"Результат выполнения: {VectorCalculator.CalculateVectorLength(matrix, vector)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
