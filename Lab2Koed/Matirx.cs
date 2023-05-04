using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Statistics;
using AsciiChart;
namespace Lab2Koed
{
    public static class MatrixStatistics
    {
        /// <summary>
        /// Данный статический метод делает 1 пунк лаб работы 
        /// </summary>
        /// <param name="Z"></param>
        /// <param name="means">массив средних по столбцам, дисперсий по столбцам</param>
        /// <param name="variances">дисперсия по столбцам</param>
        /// <param name="standardized">стандартизованная матрица</param>
        /// <param name="covariance">ковариационная матрица</param>
        /// <param name="correlation">корреляционная матрица</param>
        public static void CalculateStatistics(double[,] Z, out double[] means, out double[] variances,
            out double[,] standardized, out double[,] covariance, out double[,] correlation)
        {
            int N = Z.GetLength(0); // число строк
            int p = Z.GetLength(1); // число столбцов

            // Вычисление средних по столбцам
            means = new double[p];
            for (int j = 0; j < p; j++)
            {
                double sum = 0;
                for (int i = 0; i < N; i++)
                {
                    sum += Z[i, j];
                }

                means[j] = sum / N;
            }

            // Вычисление дисперсий по столбцам
            variances = new double[p];
            for (int j = 0; j < p; j++)
            {
                double sum = 0;
                for (int i = 0; i < N; i++)
                {
                    sum += (Z[i, j] - means[j]) * (Z[i, j] - means[j]);
                }

                variances[j] = sum / (N - 1);
            }

            // Вычисление стандартизованной матрицы
            standardized = new double[N, p];
            for (int j = 0; j < p; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    standardized[i, j] = (Z[i, j] - means[j]) / Math.Sqrt(variances[j]);
                }
            }

            // Вычисление ковариационной матрицы
            covariance = new double[p, p];
            for (int j1 = 0; j1 < p; j1++)
            {
                for (int j2 = 0; j2 < p; j2++)
                {
                    double sum = 0;
                    for (int i = 0; i < N; i++)
                    {
                        sum += (Z[i, j1] - means[j1]) * (Z[i, j2] - means[j2]);
                    }

                    covariance[j1, j2] = sum / (N - 1);
                }
            }

            // Вычисление корреляционной матрицы
            correlation = new double[p, p];
            for (int j1 = 0; j1 < p; j1++)
            {
                for (int j2 = 0; j2 < p; j2++)
                {
                    correlation[j1, j2] = covariance[j1, j2] / (Math.Sqrt(variances[j1]) * Math.Sqrt(variances[j2]));
                }
            }
        }
        /// <summary>
        /// 2 пунк лабораторной 
        /// </summary>
        /// <param name="correlation">матрица которую проверяем</param>
        /// <param name="alpha"></param>
        /// <returns>если гипотеза верна то true иначе false</returns>
        public static bool TestCorrelationSignificance(double[,] correlation, double alpha)
        {
            int p = correlation.GetLength(0);

            // Проверяем значимость каждого коэффициента корреляции
            for (int j1 = 0; j1 < p - 1; j1++)
            {
                for (int j2 = j1 + 1; j2 < p; j2++)
                {
                    double r = correlation[j1, j2]; // коэффициент корреляции
                    int df = correlation.GetLength(0) - 2; // число степеней свободы


                    double t_table = GetCriticalValue(alpha / 2, df);


                    double t = Math.Sqrt(r * Math.Sqrt(df)) / Math.Sqrt(1 - Math.Pow(r, 2));

                    // Сравнение квадрата t-статистики с квадратом критического значения
                    if (Math.Abs(t) > t_table)
                    {
                        // Коэффициент корреляции значимо отличается от нуля
                        return true;
                    }
                }
            }

            // Коэффициенты корреляции не значимо отличаются от нуля
            return false;
            //локальный метод для вычисления критического значения t-статистики
            static double GetCriticalValue(double alpha, int degreesOfFreedom)
            {
                // Вычисление критического значения t-статистики по таблице Стьюдента
                // Для упрощения используем таблицу из 1 степени свободы
                double[] tTable = { 0, 12.706, 4.303, 3.182, 2.776, 2.571, 2.447, 2.365, 2.306, 2.262,
                2.228, 2.201, 2.179, 2.160, 2.145, 2.131, 2.120, 2.110, 2.101, 2.093,
                2.086, 2.080, 2.074, 2.069, 2.064, 2.060, 2.056, 2.052, 2.048, 2.045 };
                int index = degreesOfFreedom <= 29 ? degreesOfFreedom : 29; // Индекс критического значения t-статистики в таблице
                return tTable[index] * Math.Sqrt(degreesOfFreedom / (double)(degreesOfFreedom - 2)) * alpha;
            }
        }
        /// <summary>
        /// Вывод всех данных на консоль
        /// </summary>
        /// <param name="means"></param>
        /// <param name="variances"></param>
        /// <param name="standardized"></param>
        /// <param name="covariance"></param>
        /// <param name="correlation"></param>
        public static void Display(double[] means, double[] variances, double[,] standardized, double[,] covariance, double[,] correlation)
        {
            Console.WriteLine("Means:");
            for (int i = 0; i < means.Length; i++)
            {
                Console.WriteLine($"Column {i + 1}: {means[i]:F4}");
            }

            Console.WriteLine("\nVariances:");
            for (int i = 0; i < variances.Length; i++)
            {
                Console.WriteLine($"Column {i + 1}: {variances[i]:F4}");
            }

            Console.WriteLine("\nStandardized matrix:");
            for (int i = 0; i < standardized.GetLength(0); i++)
            {
                for (int j = 0; j < standardized.GetLength(1); j++)
                {
                    Console.Write($"{standardized[i, j]:F4}\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nCovariance matrix:");
            for (int i = 0; i < covariance.GetLength(0); i++)
            {
                for (int j = 0; j < covariance.GetLength(1); j++)
                {
                    Console.Write($"{covariance[i, j]:F4}\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nCorrelation matrix:");
            for (int i = 0; i < correlation.GetLength(0); i++)
            {
                for (int j = 0; j < correlation.GetLength(1); j++)
                {
                    Console.Write($"{correlation[i, j]:F4}\t");
                }
                Console.WriteLine();
            }
        }
        public static T[] GetFirstColumn<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            T[] column = new T[rows];

            for (int i = 0; i < rows; i++)
            {
                column[i] = array[i, 0];
            }

            return column;
        }
        public static T[,] GetOtherColumns<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);
            T[,] otherColumns = new T[rows, columns - 1];

            for (int i = 1; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    otherColumns[j, i - 1] = array[j, i];
                }
            }

            return otherColumns;
        }
        public static void PrintArray<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write(array[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

        public static double[,] AddColumnOfOnesToEnd(double[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            // Создаем новый массив с дополнительной колонкой из единиц
            double[,] newArray = new double[rows, cols + 1];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    newArray[i, j] = array[i, j];
                }
                newArray[i, cols] = 1;
            }

            return newArray;
        }

        public static void MHK2(double[,] Z)
        {
            double[] y = GetFirstColumn<double>(Z);
            double[,] X1 = GetOtherColumns<double>(Z);

            double[,] X = AddColumnOfOnesToEnd(X1);
            // Вычисляем МНК-оценку вектора коэффициентов a
            double[,] Xt = Transpose(X);
            double[,] XtX = Multiply(Xt, X);
            double[,] invXtX = Inverse(XtX);
            double[,] invXtX_Xt = Multiply(invXtX, Xt);
            double[] a = Multiply(invXtX_Xt, y);

            // Вычисляем  значения Y
            double[] y_pred = Multiply(X, a);

            // Вычисляем среднее значение фактических и расчетных значений Y
            double y_mean = Mean(y);
            double y_pred_mean = Mean(y_pred);

            // Вычисляем коэффициент детерминации R
            double r = R(y, y_pred);

            // Выводим результаты на консоль
            Console.WriteLine("МНК-оценка коэффициентов: ");
            PrintVector(a);
            Console.WriteLine($"Среднее значение Y: {y_mean:N11}");
            Console.WriteLine($"Среднее значение прогнозных Y: {y_pred_mean:N11}");
            Console.WriteLine($"Коэффициент детерминации R: {r}");
        }
        // Печатает элементы вектора arr в консоль
        public static void PrintVector(double[] arr)
        {
            Console.Write("[");
            for (int i = 0; i < arr.Length - 1; i++)
            {
                Console.Write(arr[i] + ", ");
            }
            Console.Write(arr[arr.Length - 1]);
            Console.WriteLine("]");
        }

        // Умножает матрицу matrix на столбец vector
        // matrix: матрица размера n на m
        // vector: столбец размера m
        // Возвращает столбец размера n
        public static double[] Multiply(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            if (vector.Length != m)
            {
                throw new ArgumentException("Размер столбца должен соответствовать количеству столбцов матрицы.");
            }

            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += matrix[i, j] * vector[j];
                }
                result[i] = sum;
            }

            return result;
        }

        static double[,] Transpose(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] result = new double[cols, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }
            return result;
        }
        // Функция для перемножения матриц
        static double[,] Multiply(double[,] matrix1, double[,] matrix2)
        {
            int rows1 = matrix1.GetLength(0);
            int cols1 = matrix1.GetLength(1);
            int rows2 = matrix2.GetLength(0);
            int cols2 = matrix2.GetLength(1);
            if (cols1 != rows2)
            {
                throw new ArgumentException("Неверные размеры матриц");
            }
            double[,] result = new double[rows1, cols2];
            for (int i = 0; i < rows1; i++)
            {
                for (int j = 0; j < cols2; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < cols1; k++)
                    {
                        sum += matrix1[i, k] * matrix2[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }

        // Функция для нахождения обратной матрицы
        public static double[,] Inverse(double[,] matrix)
        {
            int n = matrix.GetLength(0);

            // создаем расширенную матрицу
            double[,] augMatrix = new double[n, 2 * n];

            // заполняем расширенную матрицу из исходной
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augMatrix[i, j] = matrix[i, j];
                }
                augMatrix[i, n + i] = 1;
            }

            // прямой ход метода Гаусса
            for (int i = 0; i < n; i++)
            {
                // делаем главный элемент равным 1
                double temp = augMatrix[i, i];
                for (int j = i; j < 2 * n; j++)
                {
                    augMatrix[i, j] /= temp;
                }

                // вычитаем текущую строку из оставшихся
                for (int j = i + 1; j < n; j++)
                {
                    double mult = augMatrix[j, i];
                    for (int k = i; k < 2 * n; k++)
                    {
                        augMatrix[j, k] -= mult * augMatrix[i, k];
                    }
                }
            }

            // обратный ход метода Гаусса
            for (int i = n - 1; i >= 0; i--)
            {
                // вычитаем текущую строку из оставшихся
                for (int j = i - 1; j >= 0; j--)
                {
                    double mult = augMatrix[j, i];
                    for (int k = i; k < 2 * n; k++)
                    {
                        augMatrix[j, k] -= mult * augMatrix[i, k];
                    }
                }
            }

            // выделяем обратную матрицу
            double[,] invMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    invMatrix[i, j] = augMatrix[i, n + j];
                }
            }

            return invMatrix;
        }

        // Функция для вычисления среднего значения вектора
        static double Mean(double[] vector)
        {
            int n = vector.Length;
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += vector[i];
            }
            return sum / n;
        }
        public static double R(double[] y, double[] y_pred)
        {
            double y_mean = Mean(y); // вычисляем среднее значение y
            double ssr = 0; // сумма квадратов регрессии
            double sst = 0; // общая сумма квадратов
            for (int i = 0; i < y.Length; i++)
            {
                ssr += Math.Pow(y_pred[i] - y_mean, 2); // добавляем квадрат отклонения прогноза от среднего значения
                sst += Math.Pow(y[i] - y_mean, 2); // добавляем квадрат отклонения фактического значения от среднего значения
            }
            return 1 - (ssr / sst); // возвращаем отношение суммы квадратов регрессии к общей сумме квадратов
        }
       
        }

        public static class Lab4
        {
            public static void Execute(double[][] data)
            {
                
                var datanew = ExcelToArray.JaggedToMultidimensional(data);
                MatrixStatistics.CalculateStatistics(datanew, out double[] _, out double[] _, out double[,] _, out double[,] cov, out double[,] _);
                var covarianceMatrix = ExcelToArray.MultidimensionalToJagged(cov);
                // Находим собственные значения и собственные векторы
                double[] eigenValues;
                double[][] eigenVectors;
                CalculateEigen(covarianceMatrix, out eigenValues, out eigenVectors);

                // Выводим результаты
                Console.WriteLine("Eigenvalues:");
                foreach (double eigenValue in eigenValues)
                {
                    Console.WriteLine(eigenValue);
                }

                Console.WriteLine("Eigenvectors:");
                foreach (double[] eigenVector in eigenVectors)
                {
                    Console.WriteLine(string.Join(", ", eigenVector));
                }
            }
            public static void CalculateEigen(double[][] matrix, out double[] eigenValues, out double[][] eigenVectors)
            {
                int n = matrix.Length;
                eigenVectors = new double[n][];
                for (int i = 0; i < n; i++)
                {
                    eigenVectors[i] = new double[n];
                    eigenVectors[i][i] = 1.0;
                }
                eigenValues = new double[n];

                double epsilon = double.Epsilon;
                int maxIterations = n * n;
                int iterations = 0;

                while (true)
                {
                    // Находим максимальный внедиагональный элемент
                    double maxOffDiagonal = 0.0;
                    int p = 0;
                    int q = 0;
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = i + 1; j < n; j++)
                        {
                            double offDiagonal = Math.Abs(matrix[i][j]);
                            if (offDiagonal > maxOffDiagonal)
                            {
                                maxOffDiagonal = offDiagonal;
                                p = i;
                                q = j;
                            }
                        }
                    }

                    // Если максимальный внедиагональный элемент меньше порога, выходим из цикла
                    if (maxOffDiagonal < epsilon || iterations >= maxIterations)
                    {
                        break;
                    }

                    // Вычисляем угол поворота
                    double theta = 0.5 * (matrix[q][q] - matrix[p][p]) / matrix[p][q];
                    double t = Math.Sign(theta) / (Math.Abs(theta) + Math.Sqrt(theta * theta + 1.0));
                    double c = 1.0 / Math.Sqrt(t * t + 1.0);
                    double s = c * t;

                    // Обновляем матрицу и вектора
                    for (int k = 0; k < n; k++)
                    {
                        double tmp = matrix[p][k];
                        matrix[p][k] = c * tmp - s * matrix[q][k];
                        matrix[q][k] = s * tmp + c * matrix[q][k];
                        tmp = eigenVectors[k][p];
                        eigenVectors[k][p] = c * tmp - s * eigenVectors[k][q];
                        eigenVectors[k][q] = s * tmp + c * eigenVectors[k][q];
                    }

                    iterations++;
                }

                // Записываем собственные значения в массив eigenValues
                for (int i = 0; i < n; i++)
                {
                    eigenValues[i] = matrix[i][i];
                }
            }
        }

}

