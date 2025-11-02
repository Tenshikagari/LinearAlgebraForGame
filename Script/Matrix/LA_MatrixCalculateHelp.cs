using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinearAlgebraForGame
{
    static public class LA_MatrixCalculateHelp
    {
        public static LA_Matrix Add(LA_Matrix a, LA_Matrix b, LA_Matrix outC)
        {
            if (a.GetRow() != b.GetRow() || a.GetColumn() != b.GetColumn())
            {
                throw new InvalidOperationException(LA_Log.getException("矩阵A和矩阵B的大小不同，不能相加"));
            }
            var result = LA_ObjectPool.GetMatrixData(a.GetRow(), a.GetColumn());
            for (int i = 0; i < a.GetRow(); i++)
            {
                for (int j = 0; j < a.GetColumn(); j++)
                {
                    result[i][j] = a.GetValue(i, j) + b.GetValue(i, j);
                }
            }
            outC.InitByData(result, a.GetRow(), a.GetColumn());
            LA_ObjectPool.RecycleMatrixData(result);
            return outC;
        }
        public static LA_Matrix Sub(LA_Matrix a, LA_Matrix b, LA_Matrix outC)
        {
            if (a.GetRow() != b.GetRow() || a.GetColumn() != b.GetColumn())
            {
                throw new InvalidOperationException(LA_Log.getException("矩阵A和矩阵B的大小不同，不能相减"));
            }

            var result = LA_ObjectPool.GetMatrixData(a.GetRow(), a.GetColumn());

            for (int i = 0; i < a.GetRow(); i++)
            {
                for (int j = 0; j < a.GetColumn(); j++)
                {
                    result[i][j] = a.GetValue(i, j) - b.GetValue(i, j);
                }
            }

            outC.InitByData(result, a.GetRow(), a.GetColumn());
            LA_ObjectPool.RecycleMatrixData(result);
            return outC;
        }
        public static LA_Matrix Mul(LA_Matrix a, LA_Matrix b, LA_Matrix outC)
        {
            if (a.GetColumn() != b.GetRow())
            {
                throw new InvalidOperationException(LA_Log.getException($"矩阵A的列数与矩阵B的行数不同，不能相乘  a:{a}  b:{b}"));
            }

            var result = LA_ObjectPool.GetMatrixData(a.GetRow(), b.GetColumn());

            for (int i = 0; i < a.GetRow(); i++)
            {
                for (int j = 0; j < b.GetColumn(); j++)
                {
                    float sum = 0;
                    for (int k = 0; k < a.GetColumn(); k++)
                    {
                        sum += a.GetValue(i, k) * b.GetValue(k, j);
                    }
                    result[i][j] = sum;
                }
            }
            outC.InitByData(result, a.GetRow(), b.GetColumn());
            LA_ObjectPool.RecycleMatrixData(result);
            return outC;
        }
        public static LA_Matrix MulNumber(LA_Matrix a, float scalar, LA_Matrix outC)
        {
            var result = LA_ObjectPool.GetMatrixData(a.GetRow(), a.GetColumn());

            for (int i = 0; i < a.GetRow(); i++)
            {
                for (int j = 0; j < a.GetColumn(); j++)
                {
                    result[i][j] = a.GetValue(i, j) * scalar;
                }
            }

            outC.InitByData(result, a.GetRow(), a.GetColumn());
            LA_ObjectPool.RecycleMatrixData(result);
            return outC;
        }

        public static LA_Vector MulVector(LA_Matrix matrix, LA_Vector vec, LA_Vector outV)
        {
            int row = matrix.GetRow();
            int column = matrix.GetColumn();
            if (column != vec.Size())
            {
                throw new InvalidOperationException(LA_Log.getException("矩阵的列数必须与向量的大小相匹配"));
            }

            List<float> data = LA_ObjectPool.GetVectorData(row);
            for (int i = 0; i < row; i++)
            {
                float sum = 0;
                for (int j = 0; j < column; j++)
                {
                    sum += matrix.GetValue(i, j) * vec.GetValue(j);
                }
                data[i] = (sum);
            }

            outV.InitByData(data, row);
            LA_ObjectPool.RecycleVectorData(data);
            return outV;
        }


        public static LA_Matrix SubMatrix(LA_Matrix sourceMatrix, int rowIndex, int rowCount, int columnIndex, int columnCount, LA_Matrix outMatrix)
        {
            if (rowIndex < 0 || rowIndex >= sourceMatrix.GetRow())
                throw new ArgumentOutOfRangeException(LA_Log.getException("行超出"));

            if (columnIndex < 0 || columnIndex >= sourceMatrix.GetColumn())
                throw new ArgumentOutOfRangeException(LA_Log.getException("列超出"));

            if (rowCount <= 0)
                throw new ArgumentOutOfRangeException(LA_Log.getException("输入行错误"));

            if (columnCount <= 0)
                throw new ArgumentOutOfRangeException(LA_Log.getException("输入列错误"));

            if (rowIndex + rowCount > sourceMatrix.GetRow())
                throw new ArgumentOutOfRangeException(LA_Log.getException("子矩阵 裁剪行超出索引"));

            if (columnIndex + columnCount > sourceMatrix.GetColumn())
                throw new ArgumentOutOfRangeException(LA_Log.getException("子矩阵 裁剪列超出索引"));

            List<List<float>> subMatrixData = LA_ObjectPool.GetMatrixData(rowCount, columnCount);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    subMatrixData[i][j] = sourceMatrix.GetValue(rowIndex + i, columnIndex + j);
                }
            }
            outMatrix.InitByData(subMatrixData, rowCount, columnCount);
            LA_ObjectPool.RecycleMatrixData(subMatrixData);
            return outMatrix;
        }


        #region QR分解
        private static LA_Matrix _CreateHouseholderMatrix(LA_Vector inputV)
        {
            float normV = inputV.L2Norm();
            LA_Vector w = LA_VectorBuildHelper.BuildVectorByFun(inputV.Size(), (index) =>
            {
                if (index == 0)
                    return inputV.GetValue(0) >= 0 ? -normV : normV;  // ѡ��һ���ȽϺõķ���
                else
                    return 0;
            });

            var u = inputV.Subtract(w);
            var uuT = u.Normalize().OutProduct(u.Normalize());
            var uuT_X2 = uuT.MulNumber(2);
            var IMatrix = LA_MatrixBuildHelper.BuildIdentity(inputV.Size());
            var H = IMatrix.Sub(uuT_X2);

            LA_ObjectPool.RecycleVector(w);
            LA_ObjectPool.RecycleVector(u);
            LA_ObjectPool.RecycleMatrix(uuT);
            LA_ObjectPool.RecycleMatrix(uuT_X2);
            LA_ObjectPool.RecycleMatrix(IMatrix);
            return H;
        }

        static LA_Matrix _ApplyBlockHouseholder(LA_Matrix inputMatrix, LA_Matrix houseHold, int startRow, int startCol)
        {
            int m = inputMatrix.GetRow();
            int n = inputMatrix.GetColumn();

            if (startRow == 0 && startCol == 0)
            {
                return houseHold.Mul(inputMatrix);
            }
            else
            {
 
                LA_Matrix result = LA_MatrixBuildHelper.BuildZero(inputMatrix.GetRow(), inputMatrix.GetColumn());
                LA_Matrix topLeft = inputMatrix.GetSubMatrix(0, startRow, 0, startCol);
                LA_Matrix topRight = inputMatrix.GetSubMatrix(0, startRow, startCol, n - startCol);
                LA_Matrix bottomLeft = inputMatrix.GetSubMatrix(startRow, m - startRow, 0, startCol);
                LA_Matrix bottomRight = inputMatrix.GetSubMatrix(startRow, m - startRow, startCol, n - startCol);

                LA_Matrix transformedBottomRight = houseHold.Mul(bottomRight);

                result.SetSubMatrix(0, 0, topLeft);
                result.SetSubMatrix(0, startCol, topRight);
                result.SetSubMatrix(startRow, 0, bottomLeft);
                result.SetSubMatrix(startRow, startCol, transformedBottomRight);

                LA_ObjectPool.RecycleMatrix(topLeft);
                LA_ObjectPool.RecycleMatrix(topRight);
                LA_ObjectPool.RecycleMatrix(bottomLeft);
                LA_ObjectPool.RecycleMatrix(bottomRight);
                LA_ObjectPool.RecycleMatrix(transformedBottomRight);
                return result;
            }
        }


        private static (LA_Matrix, LA_Matrix) HouseholderQR(LA_Matrix inputA)
        {
            LA_Matrix R = LA_MatrixBuildHelper.Clone(inputA);
            LA_Matrix Q = LA_MatrixBuildHelper.BuildIdentity(R.GetRow());

            for (int i = 0; i < Math.Min(R.GetRow(), R.GetColumn()); i++)  //  household  R.GetColumnVector(i).SubVector(i, R.RowCount() - i)
            {
                LA_Vector columVector = LA_VectorBuildHelper.BuildVectorByFun(R.GetRow() - i,
                (index) =>
                {
                    return R.GetValue(i + index, i);
                });

                LA_Matrix houseHoldSubMatrix = _CreateHouseholderMatrix(columVector);  // 这里消耗是下面的2倍 50*50 一次约10ms
                LA_Matrix newR = _ApplyBlockHouseholder(R, houseHoldSubMatrix, i, i);

                LA_Matrix fullHouseHold = LA_MatrixBuildHelper.BuildIdentity(R.GetRow());  //  50*50 一次约6ms
                fullHouseHold.SetSubMatrix(i, i, houseHoldSubMatrix);
                LA_Matrix newQ = fullHouseHold.Mul(Q);

                LA_ObjectPool.RecycleMatrix(houseHoldSubMatrix);
                LA_ObjectPool.RecycleMatrix(R);
                R = newR;
                LA_ObjectPool.RecycleMatrix(Q);
                Q = newQ;
                LA_ObjectPool.RecycleMatrix(fullHouseHold);
                LA_ObjectPool.RecycleVector(columVector);
            }

            var outQ = LA_MatrixBuildHelper.Clone(Q.T());
            var outR = R;
            LA_ObjectPool.RecycleMatrix(Q);
            return (outQ, outR); 
        }


        private static (LA_Matrix, LA_Matrix) GramSchmidtQR(LA_Matrix inputA)
        {
            int rowCount = inputA.GetRow();
            int columnCount = inputA.GetColumn();
             
            LA_Matrix Q = LA_MatrixBuildHelper.BuildZero(3,3);
            LA_Matrix R = LA_MatrixBuildHelper.BuildZero(3, 3);


            for (int column =0 ; column < columnCount; column++)  // 处理每一个列向量。
            { 
                LA_Vector lA_Vector = LA_VectorBuildHelper.Clone(inputA.GetColumnVector(column));

                for (int previousColumn = 0; previousColumn < column; previousColumn++)
                {
                    var previousQVector = Q.GetColumnVector(previousColumn);
                    float projectValue = lA_Vector.DotProduct(previousQVector);
                    R.SetValue(previousColumn, column, projectValue);  // 当前列向量A，在之前的标准正交上的投影值,即为R的系数。
                      
                    var subVector = previousQVector.Multiply(projectValue);
                    var newlA_Vector  = lA_Vector.Subtract(subVector);  // 删除标准正交部分上的分量。
                    LA_ObjectPool.RecycleVector(lA_Vector);
                    LA_ObjectPool.RecycleVector(subVector);
                    lA_Vector = newlA_Vector; 
                    //LA_Log.Log($"正交过程  {previousColumn}  {column}  => {Mathf.Abs( lA_Vector.DotProduct(previousQVector)  ) < 0.001}");
                }


                LA_Vector qVector = lA_Vector.Normalize();
                if (Mathf.Abs(lA_Vector.L2Norm()) < 0.001)
                { 
                    R.SetValue(column, column, 0);
                }
                else
                {
                    Q.SetColumnsVector(column, qVector);
                    R.SetValue(column, column, lA_Vector.L2Norm());
                }

                //for (int i = 0; i < column; i++)
                //{
                //    LA_Log.Log($"正交化测试{i}     单位化前 =>{lA_Vector.ToString()} = {Mathf.Abs(Q.GetColumnVector(i).DotProduct(lA_Vector)) < 0.001}");
                //    LA_Log.Log($"正交化测试{i}     单位化后 =>{lA_Vector.Normalize().ToString()} = {Mathf.Abs(Q.GetColumnVector(i).DotProduct(lA_Vector.Normalize())) < 0.001}");
                //}
 
                LA_ObjectPool.RecycleVector(lA_Vector);  
            } 
            return (Q, R);
        }

        public static (LA_Matrix, LA_Matrix)  GetQR(LA_Matrix inputA, LA_MatrixEnum.QR methonType = LA_MatrixEnum.QR.Householder)
        {
            switch (methonType)
            {
                case LA_MatrixEnum.QR.GramSchmidt:
                    return GramSchmidtQR(inputA);
                    break;
                case LA_MatrixEnum.QR.Householder:
                    return HouseholderQR(inputA);
                    break;
                default:
                    return HouseholderQR(inputA); 
                    break;
            } 
        }



        //// 第一步时间统计
        //sw.Start();

        //        LA_Matrix houseHoldSubMatrix = CreateHouseholderMatrix(columVector);
        //LA_Matrix newR = ApplyBlockHouseholder(R, houseHoldSubMatrix, i, i);

        //sw.Stop();
        //        long elapsedMs1 = sw.ElapsedMilliseconds;
        //LA_Log.Log($"创建Householder子矩阵和应用BlockHouseholder的时间: {elapsedMs1} ms");

        //        // 第二步时间统计
        //        sw.Restart();

        //        LA_Matrix fullHouseHold = LA_MatrixBuildHelper.BuildIdentity(R.Rows());
        //fullHouseHold.SetSubMatrix(i, i, houseHoldSubMatrix);
        //        LA_Matrix newQ = fullHouseHold.Mul(Q);

        //sw.Stop();
        //        long elapsedMs2 = sw.ElapsedMilliseconds;
        //LA_Log.Log($"创建完整的Householder矩阵和与Q相乘的时间: {elapsedMs2} ms");

        //        LA_ObjectPool.RecycleMatrix(houseHoldSubMatrix);
        //        LA_ObjectPool.RecycleMatrix(R);
        //        R = newR;
        //        LA_ObjectPool.RecycleMatrix(Q);
        //        Q = newQ;
        //        LA_ObjectPool.RecycleMatrix(fullHouseHold);
        //        LA_ObjectPool.RecycleVector(columVector);
        //    }


    #endregion

        #region  特征值
    /// <summary>
    ///   
    /// </summary>
    /// <param name="A"></param>
    /// <param name="errorRate">误差百分比  上次和这次做差看看改变了多少。</param>
    /// <param name="errorAbs">误差值，小于阈值就不继续</param>
    public static LA_MatrixEVD QRMethodEigenValue(LA_Matrix A ,float errorRate = 0.01f, float errorAbs = 0.01f)
        {
            var maxIterations = 4000;
            float lastCheck = float.MaxValue;
            var CurIterMatrix = LA_MatrixBuildHelper.Clone(A);
            var eigenvectorMatrix = LA_MatrixBuildHelper.BuildIdentity(A.GetRow()); // 特征向量的跟踪矩阵
            int i = 0;
            for (; i < maxIterations; i++)
            {
                LA_Matrix Q = CurIterMatrix.QR(LA_EnumQR.Q);
                LA_Matrix R = CurIterMatrix.QR(LA_EnumQR.R);
                LA_Matrix newIterMatrix = R.Mul(Q);
                LA_Matrix neweigenvectorMatrix = eigenvectorMatrix.Mul(Q);  // 更新特征向量的跟踪矩阵

                float check = 0;
                for (int checkDiagnal = 0; checkDiagnal < CurIterMatrix.GetRow(); checkDiagnal++)
                {
                    var _check = Math.Abs(CurIterMatrix.GetValue(checkDiagnal, checkDiagnal) - newIterMatrix.GetValue(checkDiagnal, checkDiagnal));
                    check = Math.Max(check, _check);
                }

                LA_ObjectPool.RecycleMatrix(CurIterMatrix);
                CurIterMatrix = newIterMatrix;

                LA_ObjectPool.RecycleMatrix(eigenvectorMatrix);
                eigenvectorMatrix = neweigenvectorMatrix;

                //LA_Log.Log($"QR  {check} -  {lastCheck} - {Mathf.Abs(check - lastCheck) / lastCheck}"); 
                if (Mathf.Abs(check - lastCheck) / lastCheck < errorRate || Mathf.Abs(check - lastCheck) < 0.01f)
                    break;

                lastCheck = check;
            }

            LA_Vector sortEigenValueVector;
            LA_Matrix sortEigenvectorMatrix;
            sortEvd(CurIterMatrix, eigenvectorMatrix, out sortEigenValueVector, out sortEigenvectorMatrix);
            LA_Log.Log($"迭代次数{ i}");
            LA_Log.Log($"{sortEigenvectorMatrix}   \n {sortEigenValueVector}");

            var evd = new LA_MatrixEVD(sortEigenvectorMatrix, sortEigenValueVector);
            LA_ObjectPool.RecycleMatrix(CurIterMatrix);
            LA_ObjectPool.RecycleMatrix(eigenvectorMatrix);
            LA_ObjectPool.RecycleMatrix(sortEigenvectorMatrix);
            LA_ObjectPool.RecycleVector(sortEigenValueVector);
            return evd;
        }

        private static void sortEvd(LA_Matrix CurIterMatrix, LA_Matrix eigenvectorMatrix, out LA_Vector sortEigenValueVector, out LA_Matrix sortEigenvectorMatrix)
        {
            var sortMatrixData = LA_ObjectPool.GetMatrixData(CurIterMatrix.GetRow(), 2);         // 想让特征值从小到大排序，用第二个元素跟踪特征向量
            for (int index = 0; index < CurIterMatrix.GetColumn(); index++)
            {
                sortMatrixData[index][0] = CurIterMatrix.GetValue(index, index);
                sortMatrixData[index][1] = index;
            }
            sortMatrixData.Sort((e1, e2) =>  Math.Abs(e2[0]).CompareTo(Math.Abs(e1[0])));
            sortEigenValueVector = LA_VectorBuildHelper.BuildVectorByFun(CurIterMatrix.GetColumn(), (index) => sortMatrixData[index][0]);
            sortEigenvectorMatrix = LA_MatrixBuildHelper.BuildMatrixByFunc(eigenvectorMatrix.GetRow(), eigenvectorMatrix.GetColumn(), (row, columns) =>
            {
                var trueColumns = (int)sortMatrixData[columns][1];
                return eigenvectorMatrix.GetValue(row, trueColumns);
            });
            LA_ObjectPool.RecycleMatrixData(sortMatrixData);
        }
        #endregion

        #region 奇异值
        public static LA_MatrixSVD LA_MatrixSVD(LA_Matrix matrix)
        {

            //  因为无法跟踪两组特征向量的符号。 这用的数学推导来的结果 用AAT 去求的特征向量和特征值 然后推另一个ATA的
            //  A = UΣVT      U=AVΣ−1   Σ不一定可逆，所以这里取伪逆。
            //  伪逆只需要要求有秩部分相乘为1即可，其余补0.由于 Σ是对角矩阵 所以很好球 
            var ATA = matrix.T().Mul(matrix);  
            var vEigenValueStruct = ATA.EVD();


            LA_Matrix V = vEigenValueStruct.GetEigenVectorGroup();
            LA_Vector eigenValues = vEigenValueStruct.GetEigenValueGroup();
 
            // 奇异值对角阵
            var S = LA_MatrixBuildHelper.BuildMatrixByFunc(eigenValues.Size(), eigenValues.Size(),(row,column)=> { 
                
                if (row!= column )  // 对角阵
                {
                    return 0;
                }
                if (eigenValues.GetValue(row) < 0)  // 对称矩阵不会出现负特征值 出现了就是迭代不稳定  
                { 
                    return 0; 
                } 
                return (float)Math.Sqrt(eigenValues.GetValue(row));
            });

            // 奇异值对角阵伪逆
            var fakeInverseS = LA_MatrixBuildHelper.BuildMatrixByFunc(eigenValues.Size(), eigenValues.Size(), (row, column) => {
                if (row != column)
                {
                    return 0;
                }
                if (eigenValues.GetValue(row) < 0)
                {
                    return 0;

                }
                return 1/(float)Math.Sqrt(eigenValues.GetValue(row));
            });

            //U = AVΣ−1
            var AV = matrix.Mul(V);
            var U = AV.Mul(fakeInverseS);  //A_V_fakeInverseS 这里就是U了 
            var SVD = new LA_MatrixSVD(U, S, V);
            LA_ObjectPool.RecycleMatrix(ATA);
            LA_ObjectPool.RecycleMatrix(S);
            LA_ObjectPool.RecycleMatrix(fakeInverseS);
            LA_ObjectPool.RecycleMatrix(AV);
            LA_ObjectPool.RecycleMatrix(U); 
            return SVD; 
        }
        #endregion
         
        #region 矩阵逆  
        public static LA_Matrix CalMatrixInverse(LA_Matrix matrix)
        {
            if (matrix.GetRow() != matrix.GetColumn())
            {
                throw new InvalidOperationException(LA_Log.getException("矩阵逆必须是方阵"));
            }
             
            // 通过QR分解获得Q和R矩阵
            LA_Matrix Q = matrix.QR(LA_EnumQR.Q);
            LA_Matrix R = matrix.QR(LA_EnumQR.R); 
            LA_Matrix RInverse = InverseUpperTriangular(R);
            LA_Matrix matrixInverse = RInverse.Mul(Q.T()); 
            LA_ObjectPool.RecycleMatrix(RInverse);
            return matrixInverse;
        }

        private static LA_Matrix InverseUpperTriangular(LA_Matrix R)
        {
            int n = R.GetRow(); 

            List<List<float>> matrixData =  LA_ObjectPool.GetMatrixData(n, n); 
            for (int i = 0; i < n; i++)
            {
                matrixData[i][i] = 1.0f / R.GetValue(i, i); 
                for (int j = i - 1; j >= 0; j--)
                {
                    float sum = 0;
                    for (int k = j + 1; k <= i; k++)
                    {
                        sum += R.GetValue(j, k) * matrixData[k][i];
                    }

                    matrixData[j][i] = -sum / R.GetValue(j, j);
                }
            }
 
            LA_Matrix invR = LA_MatrixBuildHelper.BuildMatrixByTable(matrixData);
            LA_ObjectPool.RecycleMatrixData(matrixData); 
            return invR;
        }
        #endregion

        public static LA_Vector MatrixSolve_B(LA_Matrix matrix,LA_Vector b)
        {
            if (matrix.GetRow() != matrix.GetColumn())
            {
                throw new InvalidOperationException(LA_Log.getException("求解必须是方阵"));
            }
            if (matrix.Rank() != matrix.GetRow())
            {
                throw new InvalidOperationException(LA_Log.getException("方程组求解必须满秩"));
            }
            return matrix.Inverse().MulVector(b);
        }

        internal static float MatrixDet(LA_Matrix matrix)
        {
            // QR分解后  Q是标准正交矩阵 Q的行列式的值的1或-1  R是上三角矩阵，行列式是主对角线的值。故只需要R的主对角线相乘
            // Q是标准正交矩阵 Q的行列式的值的1或-1:  Q QT = I  性质: 矩阵转置行列式值不变 故 detQ^2 =1   +1 -1
            if (matrix.GetRow() != matrix.GetColumn())
            {
                throw new InvalidOperationException(LA_Log.getException("行列式求解必须方阵"));
            }

            float det = 1;
            for (int i = 0; i < matrix.GetRow(); i++)
            {
                det *= matrix.QR(LA_EnumQR.R).GetValue(i, i);
            }
            return det;
        }
 
        public static LA_Vector LeastSquaresFit(LA_Matrix matrix, LA_Vector b)
        {
            if (matrix.GetRow() < matrix.GetColumn())
            {
                throw new InvalidOperationException(LA_Log.getException("最小二乘法必须列满秩"));
            }
             
            LA_Matrix ATA = matrix.T().Mul(matrix);
            if (ATA.Rank() < ATA.GetRow())
            {
                LA_Log.Log($" ATA { ATA }");
                LA_Log.Log($" R { ATA.QR(LA_EnumQR.R) }");
                LA_Log.Log($" Q { ATA.QR(LA_EnumQR.Q) }");

                LA_Log.Log(  $" rebuild { ATA.QR(LA_EnumQR.Q).Mul(ATA.QR(LA_EnumQR.R))}");
                throw new InvalidOperationException(LA_Log.getException($"列不满秩序 导致ATA 不可逆 {ATA}   \n R矩阵 {ATA.QR( LA_EnumQR.R)}"));
            }

            LA_Vector newB = matrix.T().MulVector(b);
            var resultB = ATA.Inverse().MulVector(newB);
            LA_ObjectPool.RecycleMatrix(ATA);
            LA_ObjectPool.RecycleVector(newB);
            return resultB;
        }
    }
}