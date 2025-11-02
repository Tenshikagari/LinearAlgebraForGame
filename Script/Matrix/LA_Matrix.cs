using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public class LA_Matrix : LA_BaseData
    {
        private List<List<float>> _data;
        private int _rows;
        private int _cols;
        private LA_Matrix _transposeMatrix;

        private LA_Matrix _Q;
        private LA_Matrix _R;
        private LA_Matrix _Inverse;
        private List<LA_Vector> columnsVectorList = new List<LA_Vector>();
        public LA_Matrix(long guid):base(guid){
            RegisterRecycleInternalDeriveFun(_RecycleT);
            RegisterRecycleInternalDeriveFun(_RecyclEVD);
            RegisterRecycleInternalDeriveFun(_RecycleSVD);
            RegisterRecycleInternalDeriveFun(_RecycleQR);
            RegisterRecycleInternalDeriveFun(_RecycleInverse); 
            RegisterRecycleInternalDeriveFun(_RecycleColumnsVector);
        }
        public void _RecycleAll() 
        {
            _internalDerivedRecycleActionList?.Invoke();
        }

        protected override bool isInrecycle()
        {
            return GetRow() == -1 || GetColumn() == -1;
        }
         
        bool _checkSetAble()
        { 
            if (isInrecycle())
            {
                LA_Log.LogError(LA_Log.getException(LA_Log.InInrecycleLog));
                return true;
            }
            if (this._isInternal)
            {
                LA_Log.LogError(LA_Log.getException(LA_Log.InInternalMatrixLog));
                return true;
            } 
            return false;

        }

        bool _checkInrecycle()
        {
            if (isInrecycle())
            {
                LA_Log.LogError(LA_Log.getException(LA_Log.InInrecycleLog));
                return true;
            } 
            return false;
        }


        public void InitByData(List<List<float>> data, int rows, int cols)
        {
            if (data == null)
                throw new ArgumentNullException("初始化data不能为空");

            if (_data != null)
            {
                LA_ObjectPool.RecycleMatrixData(_data);
            }

            _data = LA_ObjectPool.GetMatrixData(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _data[i][j] = data[i][j];
                }
            }
            _rows = rows;
            _cols = cols;

            columnsVectorList = new List<LA_Vector>(cols);
            this._isInternal = false;
            _RecycleInternalVar();
        }

        override public void Recycle()
        {
            if (_data != null)
                LA_ObjectPool.RecycleMatrixData(_data);

            _data = null;
            _rows = 0;
            _cols = 0;
            this._isInternal = false;
            _RecycleInternalVar();
        }



        #region 获得和设置元素/ 向量 
        public float GetValue(int row, int col)
        {
            if (_checkInrecycle()) return 0f;


            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
            {
                throw new IndexOutOfRangeException(LA_Log.getException("索引超出矩阵的边界"));
            }
            return _data[row][col];
        }

        public void SetValue(int row, int col, float value)
        {
            if (_checkSetAble()) return;
    

            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
            {
                throw new IndexOutOfRangeException(LA_Log.getException("索引超出矩阵的边界"));
            }
            _data[row][col] = value;
            _RecycleInternalVar();
        }

   
        public void SetColumnsVector(int col,LA_Vector vector)
        {
            if (_checkSetAble()) return;

            for (int i = 0; i < GetRow(); i++)
            {
               _data[i][col] = vector.GetValue(i);
            }
            _RecycleInternalVar(); 
        }
         
        public LA_Vector GetColumnVector(int j)
        {
            if (columnsVectorList.Count == 0)
            {
                for (int i = 0; i < GetColumn(); i++)
                {
                    columnsVectorList.Add(null);
                }
            }

            if (columnsVectorList[j] == null)
            {
                columnsVectorList[j] = LA_VectorBuildHelper.BuildVectorByFun(GetColumn(), (i) => GetValue(i, j));
                columnsVectorList[j].SetIsInternal();
            }
            return columnsVectorList[j];
        }

        public void _RecycleColumnsVector()
        {
            if (columnsVectorList != null)
            {
                for (int i = 0; i < columnsVectorList.Count; i++)
                {
                    if (columnsVectorList[i] != null)
                    {
                        LA_ObjectPool.RecycleVector(columnsVectorList[i]);
                    }
                }
                columnsVectorList.Clear();
            }
        } 
        #endregion
         

        #region 行列数 秩
        public int GetRow()
        {
            return _rows;
        }
        public int GetColumn()
        {
            return _cols;
        }

        public int Rank()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (_R == null)
            {
                QR(LA_EnumQR.R);
            }

            int min = Math.Min(_R.GetColumn(), _R.GetColumn());
            int rankCout = 0;
            for (int i = 0; i < min; i++)
            {
                if (Math.Abs( _R.GetValue(i, i)) <= 0.00001f)
                {
                    break;
                }
                rankCout++;
            }
            return rankCout;
        }
        #endregion 
        #region 普通计算
        public LA_Matrix Add(LA_Matrix b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.Add(this, b, LA_ObjectPool.GetMatrix());
        }
        public void AddNoAlloc(LA_Matrix b, LA_Matrix outC)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            LA_MatrixCalculateHelp.Add(this, b, outC);
        } 
        public LA_Matrix Sub(LA_Matrix b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.Sub(this, b, LA_ObjectPool.GetMatrix());
        }
        public void SubNoAlloc(LA_Matrix b, LA_Matrix outC)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            LA_MatrixCalculateHelp.Sub(this, b, outC);
        }

        public LA_Matrix Mul(LA_Matrix b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.Mul(this, b, LA_ObjectPool.GetMatrix());
        }
        public void MulNoAlloc(LA_Matrix b, LA_Matrix outC)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            LA_MatrixCalculateHelp.Mul(this, b, outC);
        }

        public LA_Matrix MulNumber(float scalar)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (GetRow() <= 0 || GetColumn() <= 0)  throw new ArgumentOutOfRangeException("矩阵乘行列索引不对"); 
     
            return LA_MatrixCalculateHelp.MulNumber(this, scalar, LA_ObjectPool.GetMatrix());
        }
        public void MulNumberNoAlloc(float scalar, LA_Matrix outC)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            LA_MatrixCalculateHelp.MulNumber(this, scalar, outC);
        }
         
        public LA_Vector MulVector(LA_Vector vec)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            var outV = LA_ObjectPool.GetVector(); 
            return LA_MatrixCalculateHelp.MulVector(this, vec, outV);
        }
         
        public LA_Vector MulVectorNoAlloc(LA_Vector vec, LA_Vector outV)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.MulVector(this, vec, outV);
        }
        #endregion
        #region 分块
        public LA_Matrix GetSubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            LA_Matrix resultMatrix = LA_ObjectPool.GetMatrix();
            return LA_MatrixCalculateHelp.SubMatrix(this, rowIndex, rowCount, columnIndex, columnCount, resultMatrix);
        }
        public LA_Matrix GetSubMatrixNoAlloc(int rowIndex, int rowCount, int columnIndex, int columnCount, LA_Matrix outMatrix)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.SubMatrix(this, rowIndex, rowCount, columnIndex, columnCount, outMatrix);
        } 
        public void SetSubMatrix(int rowIndex, int columnIndex, LA_Matrix matrix)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (this._isInternal)
            {
                throw new InvalidOperationException("内部矩阵不允许再操作");
            }
             
            for (int i = 0; i < matrix.GetRow(); i++)
            {
                for (int j = 0; j < matrix.GetColumn(); j++)
                {
                    //LA_Log.Log($"{i} {j} submatrix {matrix}  this {this}  ");  
                    _data[rowIndex + i][columnIndex + j] = matrix.GetValue(i, j);
                }
            }
            _RecycleInternalVar();
        }
        #endregion
        #region 行列式
        public float Det()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.MatrixDet(this);
        }
        #endregion
        #region 转置
        public LA_Matrix T()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (_transposeMatrix == null)
            { 
                _transposeMatrix = LA_MatrixBuildHelper.BuildMatrixByFunc(GetColumn(), GetRow(), (originColumns, originRow) => GetValue(originRow, originColumns));
            } 
            return _transposeMatrix;
        }  
        void _RecycleT()
        {
            if (_transposeMatrix != null)
            {
                LA_ObjectPool.RecycleMatrix(_transposeMatrix);
                _transposeMatrix = null;
            }
        }
        #endregion
        #region 解方程组
        public LA_Vector Solve_B(LA_Vector vector)  // Ax = b
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.MatrixSolve_B(this, vector);
        }
        #endregion 
        #region 逆
        public LA_Matrix Inverse()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (_Inverse == null)
            {
                _Inverse = LA_MatrixCalculateHelp.CalMatrixInverse(this);
            }
            return _Inverse;
        } 
        void _RecycleInverse()
        {
            if (_Inverse != null)
            {
                LA_ObjectPool.RecycleMatrix(_Inverse); 
            }
        }
        #endregion 
        #region QR 分解
        public LA_Matrix QR(LA_EnumQR index)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (_Q==null || _R == null)
            {
                _RecycleQR();
                var result = LA_MatrixCalculateHelp.GetQR(this);
                _Q  = result.Item1;
                _R  = result.Item2;
                _Q.SetIsInternal();
                _R.SetIsInternal();
            } 
            if (index== LA_EnumQR.Q) return _Q;
            if (index== LA_EnumQR.R) return _R;
            return null;
        } 
        void _RecycleQR()
        {
            if (_Q != null)
            {
                LA_ObjectPool.RecycleMatrix(_Q);
                _Q = null;
            }
            if (_R != null)
            {
                LA_ObjectPool.RecycleMatrix(_R);
                _R = null;
            }
        }
        #endregion
        #region EVD 特征值
        private LA_MatrixEVD _evd;
        public LA_MatrixEVD EVD()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (_evd == null)
            {
                _evd = LA_MatrixCalculateHelp.QRMethodEigenValue(this);
            }
            return _evd;
        }
        void _RecyclEVD()
        {
            if (_evd!= null)
            {
                _evd.Recycle();
            }
        }
        #endregion
        #region SVD 奇异值分解
        LA_MatrixSVD _SVD;
        public LA_MatrixSVD SVD()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (_SVD == null)
            {
                _SVD = LA_MatrixCalculateHelp.LA_MatrixSVD(this);
            }
            return _SVD;
        }

        void _RecycleSVD()
        {
            if (_SVD != null)
            {
                _SVD.Recycle();
            }
        }
        #endregion 
        #region 最小二乘解
        public LA_Vector LeastSquaresFit(LA_Vector b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_MatrixCalculateHelp.LeastSquaresFit(this, b); 
        }


        #endregion

         
        //public LA_Matrix Inverse()
        //{
        //    if (_transposeMatrix == null)
        //    {
        //        _transposeMatrix = LA_MatrixBuildHelper.BuildMatrixByFunc(Rows(), Columns(), (row, col) => GetValue(col, row));
        //    }
        //    return _transposeMatrix;
        //}


        #region log
        public override string ToString()
        {
            return LogMatrix(3);
        }

        public string LogMatrix(int precision)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            int[] maxLenInColumn = new int[_cols];
            List<List<string>> formattedMatrix = new List<List<string>>();
            for (int i = 0; i < _rows; i++)
            {
                formattedMatrix.Add(new List<string>());
                for (int j = 0; j < _cols; j++)
                {
                    string formattedValue = Math.Round(_data[i][j], precision).ToString();
                    formattedMatrix[i].Add(formattedValue);
                    maxLenInColumn[j] = Math.Max(maxLenInColumn[j], formattedValue.Length);
                }
            }

            for (int i = 0; i < _rows; i++)
            {
                List<string> row = new List<string>();
                for (int j = 0; j < _cols; j++)
                {
                    row.Add(formattedMatrix[i][j].PadLeft(maxLenInColumn[j]));
                }
                sb.AppendLine("[" + string.Join(", ", row) + "]");
            }

            return sb.ToString();
        } 
        #endregion
 
    }
}
