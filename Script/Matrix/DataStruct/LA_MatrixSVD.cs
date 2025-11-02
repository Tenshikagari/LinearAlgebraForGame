using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public class LA_MatrixSVD
    {
        private LA_Matrix _U;
        private LA_Matrix _SingularValues;
        private LA_Matrix _VT;

        public LA_MatrixSVD(LA_Matrix u, LA_Matrix singularValues, LA_Matrix v)
        {
            this._U = LA_MatrixBuildHelper.Clone(u);
            this._U.SetIsInternal();

            this._SingularValues = LA_MatrixBuildHelper.Clone(singularValues);
            this._SingularValues.SetIsInternal();

            this._VT = LA_MatrixBuildHelper.Clone(v.T());
            this._VT.SetIsInternal();
        }

        public LA_Matrix U()
        {
            return _U;
        }

        public LA_Matrix S()
        {
            return _SingularValues;
        }

        public LA_Matrix VT()
        {
            return _VT;
        }


        public LA_Matrix LowRankReconstruct(int rank = -1)
        {
            if (rank > Mathf.Max(_SingularValues.GetColumn(), _SingularValues.GetRow()))
            {
                throw new ArgumentException(LA_Log.getException("秩太大了")); 
            }

            if (rank == -1)
            {
                var temp = _U.Mul(_SingularValues);
                var result = temp.Mul(_VT);
                LA_ObjectPool.RecycleMatrix(temp);
                return result;
            }
            else
            {
                // 後續優化 先這樣吧
                //for (int i = 0; i < rank; i++)
                //{
                //    var singleValue = _SingularValues.GetValue(i, i);

                //    for (int r = 0; r < rows; r++)
                //    {
                //        for (int c = 0; c < cols; c++)
                //        {
                //            sumMatrix.SetValue(r, c, sumMatrix.GetValue(r, c) + _U.GetValue(r, i) * _VT.GetValue(c, i) * singleValue);
                //        }
                //    }
                //}
                var rows = _U.GetRow();
                var cols = _VT.GetColumn(); 

                var sumMatrix = LA_MatrixBuildHelper.BuildZero(_U.GetRow(), _VT.GetColumn());

                for (int i = 0; i < rank; i++)
                { 
                    var U_Vector = LA_VectorBuildHelper.BuildVectorByFun(rows, rowIndex =>
                    {
                        return U().GetValue(rowIndex, i);
                    });
                     
                    var singleValue = S().GetValue(i, i);
                     
                    var V_Vector = LA_VectorBuildHelper.BuildVectorByFun(cols, colIndex =>
                    {
                        return VT().GetValue(i, colIndex);
                    });
                     
                    var rank1Matrix = U_Vector.OutProduct(V_Vector);
                    var needRank1Matrix = rank1Matrix.MulNumber(singleValue);
                    var newSum = sumMatrix.Add(needRank1Matrix);
                    LA_ObjectPool.RecycleMatrix(rank1Matrix);
                    LA_ObjectPool.RecycleMatrix(needRank1Matrix);
                    LA_ObjectPool.RecycleMatrix(sumMatrix);
                    LA_ObjectPool.RecycleVector(U_Vector);
                    LA_ObjectPool.RecycleVector(V_Vector); 
                    sumMatrix = newSum; 
                }
                return sumMatrix; 
            }
        } 

        public void Recycle()
        {
            LA_ObjectPool.RecycleMatrix(_U);
            LA_ObjectPool.RecycleMatrix(_SingularValues);
            LA_ObjectPool.RecycleMatrix(_VT);
        }

        public string Log()
        {
            return $"U \n{_U} \n 奇异值\n{_SingularValues} \nV \n{_VT}";
        }

        public override string ToString()
        {
            return Log();
        }
    }

}
