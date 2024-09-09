using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CASD_Task2
{
    internal class Program
    {
        static ComplexNumber OfferNewComplexNumber()
        {
            Console.WriteLine("Введите действительную часть комплексного числа");
            double a = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Введите мнимую часть комплексного числа");
            double b = Convert.ToDouble(Console.ReadLine());
            return new ComplexNumber(a, b);
        }

        static void Main(string[] args)
        {
            const string menu = 
                """
                q или Q - выход из программы
                1) вывод действительной части
                2) вывод мнимой части
                3) вывод комплексного числа
                4) сложение
                5) вычитание
                6) умножение
                7) деление
                8) модуль комплексного числа
                9) аргумент комплексного числа
                10) смена опорного комплексного числа
                """;
            try
            {

                ComplexNumber complexNumber = OfferNewComplexNumber();
                ComplexNumber newComplexNumber;

                Console.WriteLine(menu);

                bool isRunning = true;

                while (isRunning)
                {
                    Console.WriteLine("\nВведите комманду:");
                    string commmand = Console.ReadLine();
                    switch (commmand)
                    {
                        case "q" or "Q":
                            isRunning = false;
                            break;
                        case "1":
                            Console.WriteLine("Результат: " + complexNumber.GetRealPath());
                            break;
                        case "2":
                            Console.WriteLine("Результат: " + complexNumber.GetImaginaryPath());
                            break;
                        case "3":
                            Console.WriteLine("Результат: " + complexNumber);
                            break;
                        case "4":
                            newComplexNumber = OfferNewComplexNumber();
                            Console.WriteLine("Результат: " + complexNumber.Add(ref newComplexNumber));
                            break;
                        case "5":
                            newComplexNumber = OfferNewComplexNumber();
                            Console.WriteLine("Результат: " + complexNumber.Substract(ref newComplexNumber));
                            break;
                        case "6":
                            newComplexNumber = OfferNewComplexNumber();
                            Console.WriteLine("Результат: " + complexNumber.Multiply(ref newComplexNumber));
                            break;
                        case "7":
                            newComplexNumber = OfferNewComplexNumber();
                            Console.WriteLine("Результат: " + complexNumber.Divide(ref newComplexNumber));
                            break;
                        case "8":
                            Console.WriteLine("Результат: " + complexNumber.GetModule());
                            break;
                        case "9":
                            try
                            {
                                Console.WriteLine("Результат: " + $"{complexNumber.GetArg()} PI");
                            }
                            catch (ArgumentException argEx)
                            {
                                Console.WriteLine("Результат: " + "Неопределённость");
                            }
                            break;
                        case "10":
                            complexNumber = OfferNewComplexNumber();
                            break;
                        default:
                            Console.WriteLine("Неизвестная комманда");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка!");
            }
        }
    }

}