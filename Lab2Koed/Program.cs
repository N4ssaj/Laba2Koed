using Lab2Koed;
using MathNet.Numerics.LinearAlgebra;
using System;

var Z = ExcelToArray.GetRangeData(@$"{Directory.GetCurrentDirectory()}/Data1.xlsx", "исходные данные", "C3", "F80");
var NZ = ExcelToArray.JaggedToMultidimensional(Z);
/*
 * Console.WriteLine("Default matrix");
ExcelToArray.Display(NZ);
Console.WriteLine();
MatrixStatistics.CalculateStatistics(NZ, out double[] means, out double[] variances, out double[,] standardized, out double[,] covariance, out double[,] correlation);
MatrixStatistics.Display(means, variances, standardized, covariance, correlation);
 */
//MatrixStatistics.MHK2(NZ);
Lab42.Execute(NZ);