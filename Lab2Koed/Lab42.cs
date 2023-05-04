using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2Koed
{
    internal class Lab42
    {
        public static void Execute(double[,] Z1) {
            var N=Z1.GetLength(0);
            var P=Z1.GetLength(1);
            var Z = ExcelToArray.MultidimensionalToJagged(Z1);
            var z_average = AverageValueJCol(Z, N, P);
            Console.WriteLine("Среднее значение j-го признака");
            PrintArray(z_average);
            var S_variance = EstimationVarianceJCol(Z, z_average, N, P);
            Console.WriteLine("Оценка дисперсии j-го столбца");
            PrintArray(S_variance);
            var X = StandMatrix(Z, z_average, S_variance, N, P);
            Console.WriteLine("Стандартизованная матрица");
            PrintJaggedArray(X);
            var x_average = AverageValueJCol(X, N, P);
            var sigma = GetSigma(X, x_average, N, P);
            Console.WriteLine("Ковариационная матрица");
            PrintJaggedArray(sigma);

            var d=GetD(sigma,N, P);
            var degree_freedom = P * (P - 1) / 2;
            var hi = 2.0141034;
            Console.WriteLine($"d:{d}");
            Console.WriteLine($"Степень свободы:{degree_freedom}");
            Console.WriteLine($"Хи-квадрат{ChiSquared.InvCDF(degree_freedom,0.95)}");

            if (d <= hi) Console.WriteLine("Принимаем гипотезу H0");
            else Console.WriteLine("Принимаем гипотезу H1");
            //тестовый пример
            var test_a = new double[][]
            {
                new double[] {1.00, 0.42, 0.54, 0.66},
                new double[] {0.42, 1.00, 0.32, 0.44},
                new double[] {0.54, 0.32, 1.00, 0.22},
                new double[] {0.66, 0.44, 0.22, 1.00}
            };
            Console.WriteLine("Тестовая матрица А:\n");
            PrintJaggedArray(test_a);
            var test=Jacobi(test_a);
            Console.WriteLine($"Лямблы\n");
            PrintArray(test.Item1);
            Console.WriteLine($"Матрица T\n");
            PrintJaggedArray(test.Item2);
            //исходная задача 
            var property_R=Jacobi(sigma);
            var eigenvalues=property_R.Item1.OrderByDescending(x => x).ToArray();
            var eigenvectors = SortMatrix(property_R.Item2, P, P);
            Console.WriteLine("Собственные числа корреляционной матрицы:");
            PrintArray(eigenvalues);
            Console.WriteLine("Собственные вектора корреляционной матрицы:");
            PrintJaggedArray(eigenvectors);
            var mgk = DiagonalMatrix(eigenvalues, P);
            Console.WriteLine("Ковариационная матрица главных компонент:");
            PrintJaggedArray(mgk);
            var c_average=AverageValueJCol(eigenvectors,P, P);
            var c_variance = EstimationVarianceJCol(eigenvectors, c_average, P, P);
            var load_matrix=StandMatrix(eigenvectors,c_average,c_variance,P,P);
            Console.WriteLine("Матрица нагрузок на главные компоненты:");
            PrintJaggedArray(load_matrix);
            var Y = ExcelToArray.MultidimensionalToJagged(MatrixStatistics.Multiply(ExcelToArray.JaggedToMultidimensional(X), ExcelToArray.JaggedToMultidimensional(eigenvectors)));
            Console.WriteLine("Проекции объектов на главные компоненты:");
            PrintJaggedArray(Y);
            var disp_x=VarianceByColumn(X,N,P);
            var disp_y=VarianceByColumn(Y,N,P);
            var a =RoundedSum(disp_x);
            var b =RoundedSum(disp_y)-1;

            Console.WriteLine($"Сумма выборочнах дисперсий исходных признаков Х:{a}");
            Console.WriteLine($"Сумма выборочных дисперсий проекций объектов на главные компоненты Y:{b}");

            var new_p = NumberOfNewFeatures(eigenvalues, P);
            Console.WriteLine($"Число новых признаков:{new_p}");
            var I= eigenvalues.Take(new_p).Sum() / eigenvalues.Sum();
            Console.WriteLine($"I(p`):{I}");
        }
        public static double RoundedSum(double[] array)
        {
            double sum = 0;
            foreach (double d in array)
            {
                sum += Math.Round(d);
            }
            return sum;
        }
        public static double[] AverageValueJCol(double[][] z, int n, int p)
        {
            double[] zAverage = new double[p];
            for (int j = 0; j < p; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    zAverage[j] += z[i][j];
                }
                zAverage[j] /= n;
            }
            return zAverage;
        }

        public static double[] EstimationVarianceJCol(double[][] z, double[] zAverage, int n, int p)
        {
            double[] sVariance = new double[p];
            for (int j = 0; j < p; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    sVariance[j] += Math.Pow(z[i][j] - zAverage[j], 2);
                }
                sVariance[j] /= n;
            }
            return sVariance;
        }

        public static double[][] GetSigma(double[][] z, double[] zAverage, int n, int p)
        {
            double[][] sigma = new double[p][];
            for (int i = 0; i < p; i++)
            {
                sigma[i] = new double[p];
                for (int j = 0; j < p; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        sigma[i][j] += (z[k][i] - zAverage[i]) * (z[k][j] - zAverage[j]);
                    }
                    sigma[i][j] /= n;
                }
            }
            return sigma;
        }

        public static double[][] StandMatrix(double[][] z, double[] zAverage, double[] sVariance, int n, int p)
        {
            double[][] x = new double[n][];
            for (int i = 0; i < n; i++)
            {
                x[i] = new double[p];
                for (int j = 0; j < p; j++)
                {
                    x[i][j] = (z[i][j] - zAverage[j]) / Math.Sqrt(sVariance[j]);
                }
            }
            return x;
        }

        public static double GetD(double[][] sigma, int n, int p)
        {
            double d = 0;
            for (int i = 0; i < p; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    if (i != j)
                    {
                        d += Math.Pow(sigma[i][j], 2);
                    }
                }
            }
            d *= n;
            return d;
        }
        public static (double[],double[][]) Jacobi(double[][] table1)
        {
            var table=ExcelToArray.JaggedToMultidimensional(table1);
            // точность
            double eps = 0.0001;

            int N = table.GetLength(0);
            Console.WriteLine(N);
            double[,] A = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    A[i, j] = table[i, j];
                }
            }

            // Шаг 1
            double[,] T = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    // единичная матрица
                    T[i, j] = (i == j) ? 1 : 0;
                }
            }

            // Шаг 2
            double sum = 0;
            for (int j = 0; j < N; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    if (i != j)
                    {
                        sum = sum + Math.Pow(A[i, j], 2);
                    }
                }
            }

            double a0 = Math.Sqrt(sum) / N;
            double a_k = a0;

            bool continue1 = true;
            while (continue1)
            {
                // Шаг 3
                double abs_max_not_diagonal = 0;
                int p = -1;
                int q = -1;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j && Math.Abs(A[i, j]) > Math.Abs(abs_max_not_diagonal))
                        {
                            p = i;
                            q = j;
                            abs_max_not_diagonal = A[i, j];
                        }
                    }
                }

                // Шаг 4
                if (Math.Abs(abs_max_not_diagonal) > a_k)
                {
                    double y_ = (A[p, p] - A[q, q]) / 2;
                    double x_ = (y_ == 0) ? -1 : (-Math.Sign(y_) * A[p, q] / Math.Sqrt(Math.Pow(A[p, q], 2) + Math.Pow(y_, 2)));
                    double s = x_ / Math.Sqrt(2 * (1 + Math.Sqrt(1 - Math.Pow(x_, 2))));
                    double c = Math.Sqrt(1 - Math.Pow(s, 2));

                    for (int i = 0; i < N; i++)
                    {
                        if (i != p && i != q)
                        {
                            double Z1 = A[i, p];
                            double Z2 = A[i, q];

                            A[q, i] = Z1 * s + Z2 * c;
                            A[i, q] = A[q, i];
                            A[i, p] = Z1 * c - Z2 * s;
                            A[p, i] = A[i, p];
                        }
                    }

                    double Z5 = Math.Pow(s, 2);
                    double Z6 = Math.Pow(c, 2);
                    double Z7 = s * c;
                    double V1 = A[p, p];
                    double V2 = A[p, q];
                    double V3 = A[q, q];

                    A[p, p] = V1 * Z6 + V3 * Z5 - 2 * V2 * Z7;
                    A[q, q] = V1 * Z5 + V3 * Z6 + 2 * V2 * Z7;
                    A[p, q] = (V1 - V3) * Z7 + V2 * (Z6 - Z5);
                    A[q, p] = A[p, q];

                    for (int i = 0; i < N; i++)
                    {
                        double Z3 = T[i, p];
                        double Z4 = T[i, q];

                        T[i, q] = Z3 * s + Z4 * c;
                        T[i, p] = Z3 * c - Z4 * s;
                    }
                }

                // Шаг 5
                continue1 = false;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j && Math.Abs(A[i, j]) > eps * a0)
                        {
                            continue1 = true;
                        }
                    }
                }

                a_k = a_k / Math.Pow(N, 2);
            }

            double[] lambdas = new double[N];
            for (int i = 0; i < N; i++)
            {
                lambdas[i] = A[i, i];
            }
            if (lambdas.Length >2)
            {
                MoveToEnd(lambdas, 1);
                SendColumnsToEnd(T, 1);
                ChangeSignOfSecondColumnVariables(T);
            }
            return (lambdas,ExcelToArray.MultidimensionalToJagged(T));
        }
        public static void ChangeSignOfSecondColumnVariables(double[,] arr)
        {
            int numCols = arr.GetLength(1);

            for (int j = 1; j < numCols; j += 2)
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    arr[i, j] = -arr[i, j];
                }
            }
        }
        public static void SendColumnsToEnd(double[,] array, params int[] columnIndices)
        {
            int numColumns = array.GetLength(1);
            int numRows = array.GetLength(0);

            int numColumnsToMove = columnIndices.Length;
            int[] columnsToMove = columnIndices.OrderBy(x => x).ToArray();

            int numColumnsToKeep = numColumns - numColumnsToMove;

            double[,] newArray = new double[numRows, numColumns];

            for (int i = 0; i < numRows; i++)
            {
                int columnIndex = 0;
                int newColumnIndex = 0;

                while (columnIndex < numColumns)
                {
                    if (columnsToMove.Contains(columnIndex))
                    {
                        columnIndex++;
                        continue;
                    }

                    newArray[i, newColumnIndex] = array[i, columnIndex];
                    newColumnIndex++;
                    columnIndex++;
                }

                for (int j = 0; j < numColumnsToMove; j++)
                {
                    newArray[i, newColumnIndex] = array[i, columnsToMove[j]];
                    newColumnIndex++;
                }
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    array[i, j] = newArray[i, j];
                }
            }
        }

        public static void MoveToEnd(double[] arr, int index)
        {
            double temp = arr[index];    // сохраняем значение элемента
            for (int i = index; i < arr.Length - 1; i++)
            {
                arr[i] = arr[i + 1];     // сдвигаем элементы влево
            }
            arr[arr.Length - 1] = temp;  // вставляем элемент в конец массива
        }
        public static void SwapColumns(double[][] array, int col1, int col2)
        {
            for (int i = 0; i < array.Length; i++)
            {
                double temp = array[i][col1];
                array[i][col1] = array[i][col2];
                array[i][col2] = temp;
            }
        }
        public static double[][] SortMatrix(double[][] A, int n, int m)
        {
            for (int j = 0; j < m; j++)
            {
                List<double> tmp_list = new List<double>();
                for (int i = 0; i < n; i++)
                {
                    tmp_list.Add(A[i][j]);
                }
                tmp_list.Sort();
                tmp_list.Reverse();
                for (int i = 0; i < n; i++)
                {
                    A[i][j] = tmp_list[i];
                }
            }
            return A;
        }
        // функция вычисления дисперсий матрицы по столбцам
        public static double[] VarianceByColumn(double[][] A, int n, int m)
        {
            double[] var_A = new double[m];
            for (int j = 0; j < m; j++)
            {
                List<double> tmp_list = new List<double>();
                for (int i = 0; i < n; i++)
                {
                    tmp_list.Add(A[i][j]);
                }
                var_A[j] = tmp_list.Variance();
            }
            return var_A;
        }
        public static int NumberOfNewFeatures(double[] eigenvalues, int p)
        {
            int new_p = 1; // число новых признаков
            for (int i = 0; i < p; i++)
            {
                double sum_lamda = 0;
                for (int j = 0; j < i; j++)
                {
                    sum_lamda += eigenvalues[j];
                }
                new_p = i;
                double part = sum_lamda / eigenvalues.Sum();
                if (part > 0.95)
                {
                    break;
                }
            }
            return new_p;
        }
        public static double[][] DiagonalMatrix(double[] A, int n)
        {
            double[][] B = new double[n][];
            for (int i = 0; i < n; i++)
            {
                B[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    B[i][j] = 0;
                }
                B[i][i] = A[i];
            }
            return B;
        }
        static void PrintArray(double[] array)
        {
            foreach (double item in array)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }

        // Метод вывода на экран зубчатого массива
        static void PrintJaggedArray(double[][] array)
        {
            foreach (double[] innerArray in array)
            {
                foreach (double item in innerArray)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine();
            }
        }

    }
}
