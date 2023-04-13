using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                    // Вычисление критического значения t-статистики
                    double t_critical = GetCriticalValue(alpha / 2, df);

                    // Вычисление квадрата t-статистики
                    double t_squared = r * r * df / (1 - r * r);

                    // Сравнение квадрата t-статистики с квадратом критического значения
                    if (t_squared > t_critical * t_critical)
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
    }
}
