﻿using FamilyBudget.v3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyBudget.v3.App_CodeBase
{
    public class TrendCalculator : ITrendCalculator
    {
        private double[,] MakeSystem(double[,] xyTable, int basis, int rowCount)
        {
            double[,] matrix = new double[basis, basis + 1];
            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    double sumA = 0, sumB = 0;
                    for (int k = 0; k < rowCount; k++)
                    {
                        sumA += Math.Pow(xyTable[0, k], i) * Math.Pow(xyTable[0, k], j);
                        sumB += xyTable[1, k] * Math.Pow(xyTable[0, k], i);
                    }
                    matrix[i, j] = sumA;
                    matrix[i, basis] = sumB;
                }
            }
            return matrix;
        }

        private double[] Gauss(double[,] matrix, int rowCount, int colCount)
        {
            int i;
            int[] mask = new int[colCount - 1];
            for (i = 0; i < colCount - 1; i++) mask[i] = i;
            if (GaussDirectPass(ref matrix, ref mask, colCount, rowCount))
            {
                double[] answer = GaussReversePass(ref matrix, mask, colCount, rowCount);
                return answer;
            }
            else return null;
        }

        private bool GaussDirectPass(ref double[,] matrix, ref int[] mask,
            int colCount, int rowCount)
        {
            int i, j, k, maxId, tmpInt;
            double maxVal, tempDouble;
            for (i = 0; i < rowCount; i++)
            {
                maxId = i;
                maxVal = matrix[i, i];
                for (j = i + 1; j < colCount - 1; j++)
                    if (Math.Abs(maxVal) < Math.Abs(matrix[i, j]))
                    {
                        maxVal = matrix[i, j];
                        maxId = j;
                    }
                if (maxVal == 0) return false;
                if (i != maxId)
                {
                    for (j = 0; j < rowCount; j++)
                    {
                        tempDouble = matrix[j, i];
                        matrix[j, i] = matrix[j, maxId];
                        matrix[j, maxId] = tempDouble;
                    }
                    tmpInt = mask[i];
                    mask[i] = mask[maxId];
                    mask[maxId] = tmpInt;
                }
                for (j = 0; j < colCount; j++) matrix[i, j] /= maxVal;
                for (j = i + 1; j < rowCount; j++)
                {
                    double tempMn = matrix[j, i];
                    for (k = 0; k < colCount; k++)
                        matrix[j, k] -= matrix[i, k] * tempMn;
                }
            }
            return true;
        }

        private double[] GaussReversePass(ref double[,] matrix, int[] mask,
            int colCount, int rowCount)
        {
            int i, j, k;
            for (i = rowCount - 1; i >= 0; i--)
                for (j = i - 1; j >= 0; j--)
                {
                    double tempMn = matrix[j, i];
                    for (k = 0; k < colCount; k++)
                        matrix[j, k] -= matrix[i, k] * tempMn;
                }
            double[] answer = new double[rowCount];
            for (i = 0; i < rowCount; i++) answer[mask[i]] = matrix[i, colCount - 1];
            return answer;
        }

        public List<TrendLineMonthDefinition> CalculateTrend(List<TrendLineMonthDefinition> data)
        {
            double[,] xyTable = new double[2, data.Count];
            try
            {
                foreach (var dataItem in data)
                {
                    xyTable[0, data.IndexOf(dataItem)] = Convert.ToDouble(data.IndexOf(dataItem));
                    xyTable[1, data.IndexOf(dataItem)] = Convert.ToDouble(dataItem.Value);
                }
            }
            catch
            {
                return data;
            }

            int basis = 2;

            double[,] matrix = MakeSystem(xyTable, basis, data.Count);
            double[] result = Gauss(matrix, basis, basis + 1);
            if (result == null)
            {
                return data;
            }

            foreach (var dataItem in data)
            {
                double x = Convert.ToDouble(data.IndexOf(dataItem));

                double y = 0;
                for (int i = 0; i < basis; i++)
                {
                    y += result[i] * Math.Pow(x, i);
                }

                dataItem.TrendValue = Convert.ToDecimal(Math.Round(y, 5));
            }

            return data;
        }
    }
}