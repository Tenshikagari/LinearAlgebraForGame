using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public static class LA_ObjectPool  
    {
        private static readonly Queue<List<List<float>>> _matrixDataPool = new Queue<List<List<float>>>();
        private static readonly Queue<LA_Matrix> _matrixPool = new Queue<LA_Matrix>();

        private static readonly Queue<List<float>> _vectorDataPool = new Queue<List<float>>();
        private static readonly Queue<LA_Vector> _vectorPool = new Queue<LA_Vector>();

        static long _guidVectorCount = 0;
        static long _guidMatrixCount = 0;
        static Dictionary<int, bool> _guidVectorTraceDict = new Dictionary<int, bool>();
        static SortedSet<long> _guidMatrixTraceDict = new SortedSet<long>();
        static public bool tracePool = false;
         
        static long GetVectorGuid()
        {
            return _guidVectorCount++;
        }

        static long GetMatrixGuid()
        {
            return _guidMatrixCount++;
        }

        public static List<float> GetVectorData(int size)
        {
            if (_vectorDataPool.Count == 0)
            {
                var newVector = new List<float>(size);
                for (int i = 0; i < size; i++)
                {
                    newVector.Add(0f);
                }
                return newVector;
            }
            else
            {
                List<float> vector = _vectorDataPool.Dequeue();
                if (vector.Count < size)
                {
                    for (int i = vector.Count; i < size; i++)
                    {
                        vector.Add(0f);
                    }
                }
                return vector;
            }
        }

        public static void RecycleVectorData(List<float> vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException("向量为空，无法被回收");
            }
            _vectorDataPool.Enqueue(vector); 
        }
         
        public static LA_Vector GetVector()
        {
            LA_Vector vector;
            if (_vectorPool.Count == 0)
            {
                vector = new LA_Vector(GetVectorGuid());
            }
            else
            {
                vector = _vectorPool.Dequeue(); 
            }
            if (tracePool)  LA_Log.Log($"获得向量guid:{vector.GUID() }");
            return vector;
        }

        public static void RecycleVector(LA_Vector vector)
        {
            if (tracePool)  LA_Log.Log($"回收向量guid:{vector.GUID() }"); 
            vector.Recycle(); 
            _vectorPool.Enqueue(vector);
        }
         
        public static List<List<float>> GetMatrixData(int rows, int cols)
        {

            if (_matrixDataPool.Count == 0)
            {
                return CreateMatrixData(rows,  cols);
            }
            else
            {
                List<List<float>> matrixData = _matrixDataPool.Dequeue();
                EnsureMatrixSize(matrixData, rows, cols);
                return matrixData;
            }  
        }

        public static LA_Matrix GetMatrix()
        {
            LA_Matrix matrix;
            if (_matrixPool.Count == 0)
            {
                matrix = new LA_Matrix(GetMatrixGuid());
            }
            else
            {
                matrix = _matrixPool.Dequeue();
            }


            if (tracePool)
            {
                _guidMatrixTraceDict.Add(matrix.GUID());
                LA_Log.Log($"获得矩阵 guid:{matrix.GUID() }");
            }
            return matrix;
        }

        public static void RecycleMatrixData(List<List<float>> matrixData)
        {
            if (matrixData == null)
            {
                throw new ArgumentNullException(LA_Log.getException("矩阵数据位空，无法再被回收"));
            } 
            _matrixDataPool.Enqueue(matrixData);
        }

 

        public static void RecycleMatrix(LA_Matrix matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(LA_Log.getException("矩阵数据位空，无法再被回收"));
            }
            matrix.Recycle();

            if (tracePool)
            {
                _guidMatrixTraceDict.Remove(matrix.GUID());
                LA_Log.Log($"回收矩阵 guid:【{matrix.GUID() }】，   此時有{_guidMatrixTraceDict.Count}个正在使用");
            }
            _matrixPool.Enqueue(matrix);
        }
         
        public static void LogMatrixUseTraceData()
        {
            string log = "";
            foreach (var item in _guidMatrixTraceDict)
            {
                log += $" {item}";
            }
            LA_Log.Log($" 矩阵回收信息 guid:【{log}】");
        }
         
        private static List<List<float>> CreateMatrixData(int rows, int cols)
        {
            var matrixData = new List<List<float>>(rows);
            for (int i = 0; i < rows; i++)
            {
                var rowData = new List<float>(cols);
                for (int j = 0; j < cols; j++)
                {
                    rowData.Add(0f);
                }
                matrixData.Add(rowData);
            }
            return matrixData;
        }

        private static void EnsureMatrixSize(List<List<float>> matrixData, int rows, int cols)
        {
            while (matrixData.Count < rows)
            {
                var newRow = new List<float>(cols);
                for (int j = 0; j < cols; j++)
                {
                    newRow.Add(0f);
                }
                matrixData.Add(newRow);
            }

            foreach (var row in matrixData)
            {
                while (row.Count < cols)
                {
                    row.Add(0f);
                }
            }
        }
    }    
}