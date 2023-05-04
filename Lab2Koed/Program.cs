using Lab2Koed;
using System;

var Z = ExcelToArray.GetRangeData(@$"{Directory.GetCurrentDirectory()}/Data1.xlsx", "исходные данные", "C3", "E80");
var NZ = ExcelToArray.JaggedToMultidimensional(Z);
/*
 * Console.WriteLine("Default matrix");
ExcelToArray.Display(NZ);
Console.WriteLine();
MatrixStatistics.CalculateStatistics(NZ, out double[] means, out double[] variances, out double[,] standardized, out double[,] covariance, out double[,] correlation);
MatrixStatistics.Display(means, variances, standardized, covariance, correlation);
 */
//MatrixStatistics.MHK(NZ);
//MatrixStatistics.MHK2(NZ);
Lab4.Execute(Z);


