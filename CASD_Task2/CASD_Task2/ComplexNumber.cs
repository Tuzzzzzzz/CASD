using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASD_Task2
{
    internal class ComplexNumber
    {
        private double RealPath;
        private double ImaginaryPath;

        public ComplexNumber(double realPath, double imaginaryPath)
        {
            RealPath = realPath;
            ImaginaryPath = imaginaryPath;
        }

        public double GetRealPath() => RealPath;

        public double GetImaginaryPath() => ImaginaryPath;

        public override string ToString() 
            => $"{RealPath} + {ImaginaryPath}*i";

        public ComplexNumber Add(ref readonly ComplexNumber other)
        {
            RealPath += other.RealPath;
            ImaginaryPath += other.ImaginaryPath;
            return this;
        }

        public ComplexNumber Substract(ref readonly ComplexNumber other)
        {
            RealPath -= other.RealPath;
            ImaginaryPath -= other.ImaginaryPath;
            return this;
        }

        public ComplexNumber Multiply(ref readonly ComplexNumber other)
        {
            double a1 = RealPath;
            double a2 = other.RealPath;
            double b1 = ImaginaryPath;
            double b2 = other.ImaginaryPath;

            RealPath = a1 * a2 - b1 * b2;
            ImaginaryPath = a1 * b2 + a2 * b1;

            return this;
        }

        public ComplexNumber Divide(ref readonly ComplexNumber other)
        {
            double a1 = RealPath;
            double a2 = other.RealPath;
            double b1 = ImaginaryPath;
            double b2 = other.ImaginaryPath;

            double denominator = a2 * a2 + b2 * b2;

            RealPath = (a1 * a2 + b1 * b2) / denominator;
            ImaginaryPath = (-a1 * b2 + b1 * a2) / denominator;

            return this;
        }

        public double GetModule() 
            => Math.Sqrt(RealPath * RealPath + ImaginaryPath * ImaginaryPath);

        public double GetArg()
        {
            //argument is in radians
            double a = RealPath;
            double b = ImaginaryPath;

            if (a == 0)
            {
                if (b == 0) throw new ArgumentException("Uncertainty");
                else if (b > 0) return Math.PI / 2;
                else return -(Math.PI / 2);
            }
            else if (a > 0)
            {
                if (b == 0) return 0;
                else return Math.Atan(a / b);
            }
            else
            {
                if (b == 0) return Math.PI;
                else if (b > 0) return Math.PI - Math.Atan(a / b);
                else return -(Math.PI) + Math.Atan(a / b);
            }
        }
    }
}
