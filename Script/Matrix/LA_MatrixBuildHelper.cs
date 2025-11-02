using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public static class LA_MatrixBuildHelper
    {

        public static LA_Matrix BuildIdentity(int rank)
        {
            return BuildMatrixByFunc(rank, rank, (i, j) => i == j ? 1f : 0f);
        }

        public static LA_Matrix BuildZero(int row, int col)
        {
            return BuildMatrixByFunc(row, col, (i, j) => 0);
        }

        public static LA_Matrix BuildMatrixByFunc(int rows, int columns, Func<int, int, float> getValueFunction = null)
        {
            getValueFunction ??= (i, j) => 0f;

            var mtxData = LA_ObjectPool.GetMatrixData(rows, columns);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    mtxData[i][j] = getValueFunction(i, j);
                }
            }

            LA_Matrix matrix = LA_ObjectPool.GetMatrix();
            matrix.InitByData(mtxData, rows, columns);
            return matrix;
        }

        public static LA_Matrix BuildMatrixByTable(List<List<float>> mtxData)
        {
            if (mtxData == null || mtxData.Count == 0)
                throw new ArgumentException(LA_Log.getException("矩阵数据不能为空或零长度"));

            int numRows = mtxData.Count;
            int numCols = mtxData[0].Count;

            foreach (var row in mtxData)
            {
                if (row == null || row.Count != numCols)
                    throw new ArgumentException(("所有矩阵行必须具有相同的列数"));
            }

            LA_Matrix matrix = LA_ObjectPool.GetMatrix();
            matrix.InitByData(mtxData, numRows, numCols);
            return matrix;
        }

        public static LA_Matrix RandomMatrix(int rows, int columns, int min = 1, int max = 10)
        {
            var mtxData = LA_ObjectPool.GetMatrixData(rows, columns);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    mtxData[i][j] = UnityEngine.Random.Range(min, max + 1);
                }
            }
            LA_Matrix matrix = LA_ObjectPool.GetMatrix();
            matrix.InitByData(mtxData, rows, columns);
            return matrix;
        }

        public static LA_Matrix Clone(LA_Matrix mtrix)
        {
            var matrix = LA_MatrixBuildHelper.BuildMatrixByFunc(mtrix.GetRow(), mtrix.GetColumn(), (row, col) => mtrix.GetValue(row, col));
            return matrix;
        }



    }
}