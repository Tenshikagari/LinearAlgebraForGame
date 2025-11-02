using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public class LA_MatrixEVD
    { 
        LA_Vector _EigenValues;
        LA_Matrix _EigenVectors; 
         
        public LA_MatrixEVD(LA_Matrix _EigenVectors, LA_Vector _EigenValues)   // 按特征值大小进行排序
        {
            this._EigenVectors = LA_MatrixBuildHelper.Clone(_EigenVectors);
            this._EigenVectors.SetIsInternal();
            this._EigenValues = LA_VectorBuildHelper.Clone(_EigenValues);
            this._EigenValues.SetIsInternal();
        }
         
        public string Log()
        { 
            return $"特征值\n{_EigenValues} \n特征向量：\n{_EigenVectors}";
        }

        public LA_Matrix GetEigenVectorGroup()
        {
            return _EigenVectors;
        }

        public LA_Vector GetEigenValueGroup()
        {
            return _EigenValues;
        }

        public void Recycle()
        {   
            if (_EigenVectors != null) LA_ObjectPool.RecycleMatrix(_EigenVectors);
            if (_EigenValues != null) LA_ObjectPool.RecycleVector(_EigenValues); 
        }
    }
}
