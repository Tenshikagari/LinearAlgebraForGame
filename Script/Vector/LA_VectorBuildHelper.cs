using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public static class LA_VectorBuildHelper
    { 
        public static LA_Vector BuildVectorByDataList(List<float> dataList,int size)
        {
            LA_Vector v = LA_ObjectPool.GetVector();
            v.InitByData(dataList, size);
            return v;
        }

        public static LA_Vector BuildVectorByFun( int size,Func<int, float> buildFun)
        {
            LA_Vector v = LA_ObjectPool.GetVector();
            var vData = LA_ObjectPool.GetVectorData(size);
            for (int i = 0; i < size; i++)
            {
                vData[i] = buildFun(i);
            } 
            v.InitByData(vData, size);
            LA_ObjectPool.RecycleVectorData(vData); 
            return v;
        }

        public static LA_Vector BuildZeroVector(int size)
        { 
            return BuildVectorByFun(size, (i) => 0); 
        }

        public static LA_Vector Clone(LA_Vector vector)
        {
            var cloneVector = LA_VectorBuildHelper.BuildVectorByFun(vector.Size(), (index) => vector.GetValue(index));
            return cloneVector;
        }
    }
}
