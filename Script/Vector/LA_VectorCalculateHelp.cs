using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public static class LA_VectorCalculateHelp
    {
        public static LA_Vector Add(LA_Vector a, LA_Vector b, LA_Vector outC)
        {
            if (a == null || b == null)
            {
                throw new ArgumentNullException(LA_Log.getException("向量为空"));
            }
            if (a.Size() <= 0 || b.Size() <= 0) throw new ArgumentOutOfRangeException(LA_Log.getException("向量没初始化"));
            if (a.Size() != b.Size())
            {
                throw new InvalidOperationException(LA_Log.getException("向量维度不一样，无法加"));
            }

            var vectorData = LA_ObjectPool.GetVectorData(a.Size());

            for (int i = 0; i < a.Size(); i++)
            {
                vectorData[i] = a.GetValue(i) + b.GetValue(i);
            }
            outC.InitByData(vectorData, a.Size());
            LA_ObjectPool.RecycleVectorData(vectorData);
            return outC;
        }

        public static LA_Vector Subtract(LA_Vector a, LA_Vector b, LA_Vector outC)
        {
            if (a == null || b == null)
            {
                throw new ArgumentNullException(LA_Log.getException("向量为空"));
            }
            if (a.Size() <= 0 || b.Size() <= 0) throw new ArgumentOutOfRangeException(LA_Log.getException("向量没初始化"));

            if (a.Size() != b.Size())
            {
                throw new InvalidOperationException(LA_Log.getException("向量维度不一样，无法减"));
            }

            var vectorData = LA_ObjectPool.GetVectorData(a.Size());
            for (int i = 0; i < a.Size(); i++)
            {
                vectorData[i] = a.GetValue(i) - b.GetValue(i);
            }
            outC.InitByData(vectorData, a.Size());
            LA_ObjectPool.RecycleVectorData(vectorData);
            return outC;
        }

        public static LA_Vector Multiply(LA_Vector a, float scalar, LA_Vector outC)
        {
            if (a == null)
            {
                throw new ArgumentNullException(LA_Log.getException("向量为空"));
            }
            if (a.Size() <= 0) throw new ArgumentOutOfRangeException(LA_Log.getException("向量没初始化"));

            var vectorData = LA_ObjectPool.GetVectorData(a.Size());
            for (int i = 0; i < a.Size(); i++)
            {
                vectorData[i] = a.GetValue(i) * scalar;
            }
            outC.InitByData(vectorData, a.Size());
            LA_ObjectPool.RecycleVectorData(vectorData);
            return outC;
        }

        public static LA_Vector Divide(LA_Vector a, float scalar, LA_Vector outC)
        {
            if (a == null)
            {
                throw new ArgumentNullException(LA_Log.getException("向量为空"));
            }
            if (a.Size() <= 0) throw new ArgumentOutOfRangeException(LA_Log.getException("向量没初始化"));

            if (scalar == 0)
            {
                throw new InvalidOperationException(LA_Log.getException("除0错误"));
            }
            var vectorData = LA_ObjectPool.GetVectorData(a.Size());
            for (int i = 0; i < a.Size(); i++)
            {
                vectorData[i] = a.GetValue(i) / scalar;
            }
            outC.InitByData(vectorData, a.Size());
            LA_ObjectPool.RecycleVectorData(vectorData);
            return outC;
        }

        public static float DotProduct(LA_Vector a, LA_Vector b)
        {
            if (a == null || b == null)
            {
                throw new ArgumentNullException(LA_Log.getException("向量为空"));
            }
            if (a.Size() <= 0 || b.Size() <= 0) throw new ArgumentOutOfRangeException(LA_Log.getException("向量没初始化"));

            if (a.Size() != b.Size())
            {
                throw new InvalidOperationException(LA_Log.getException("向量维度不一样，无法相乘"));
            }
            float sum = 0;
            for (int i = 0; i < a.Size(); i++)
            {
                sum += a.GetValue(i) * b.GetValue(i);
            }
            return sum;
        }

        public static LA_Matrix OuterProduct(LA_Vector a, LA_Vector b, LA_Matrix outMatrix)
        {
            if (a == null || b == null)
            {
                throw new ArgumentNullException(LA_Log.getException("向量为空"));
            }
            if (a.Size() <= 0 || b.Size() <= 0)
            {
                throw new ArgumentOutOfRangeException(LA_Log.getException("向量没初始化"));
            }

            List<List<float>> result = LA_ObjectPool.GetMatrixData(a.Size(), b.Size());

            for (int i = 0; i < a.Size(); i++)
            {
                for (int j = 0; j < b.Size(); j++)
                {
                    float product = a.GetValue(i) * b.GetValue(j);
                    result[i][j] = product;
                }
            }

            outMatrix.InitByData(result, a.Size(), b.Size());
            LA_ObjectPool.RecycleMatrixData(result);

            return outMatrix;
        }
    }
}