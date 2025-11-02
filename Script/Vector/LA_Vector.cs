using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public class LA_Vector: LA_BaseData
    {
        private List<float> _data;
        private LA_Vector _normalize;
        private int _size = -1;  

        public LA_Vector(long guid):base(guid)
        {
        }

        protected override bool isInrecycle()
        {
            return Size() == -1;
        }

        public int Size()
        {
            return _size;
        } 
        public LA_Vector Normalize()
        {
            if (isInrecycle())  throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            if ( _normalize == null)
            {
                float sum = 0;
                for (int i = 0; i < _size; i++)
                {
                    sum += GetValue(i) * GetValue(i);
                }

                sum = Mathf.Sqrt(sum);

                _normalize = LA_VectorBuildHelper.BuildVectorByFun(Size(), (index) =>
                {
                    return GetValue(index) / sum;
                });
            }
            _normalize.SetIsInternalVector();
            return _normalize;
        }

        public void InitByData(List<float> data,int size)
        {
            if (_data!= null)
                Recycle();

            if (data == null)
                throw new ArgumentNullException("向量初始化数据为空");
 
            _data = LA_ObjectPool.GetVectorData(data.Count); 
            for (int i = 0; i < data.Count; i++)
            {
                _data[i] = data[i];
            }

            _isInternal = false;
            _size = size;
        }

        public float GetValue(int index)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            if (index < 0 || index >= _size)
            {
                throw new IndexOutOfRangeException("�������������ı߽硣");
            }
            return _data[index];
        }

        public void SetValue(int index,float value)
        {
            if (!_isInternal)
            {
                this._data[index] = value;
            }
            else
            {
                LA_Log.LogError("内部向量不允许被修改");
            }
        }

        public void SetIsInternalVector()
        {
            _isInternal = true;
        }
 
        #region 向量运算操作
        public LA_Vector SubtractNoAlloc(LA_Vector b, LA_Vector outC)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            return LA_VectorCalculateHelp.Subtract(this, b, outC);
        }
        public LA_Vector Subtract(LA_Vector b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            var c =  LA_VectorBuildHelper.BuildZeroVector(b.Size()); 
            var result = LA_VectorCalculateHelp.Subtract(this, b, c); 
            return result;
        }

        public LA_Vector MultiplyNoAlloc(float scalar, LA_Vector outC   )
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            return LA_VectorCalculateHelp.Multiply(this, scalar, outC);
        }
        public LA_Vector Multiply(float scalar)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            var c = LA_ObjectPool.GetVector();
            return MultiplyNoAlloc(scalar, c);
        }
 
        public LA_Vector DivideNoAlloc(float scalar, LA_Vector outC  )
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            return LA_VectorCalculateHelp.Divide(this, scalar, outC);
        }

        public LA_Vector DivideNoAlloc(float scalar )
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            var c = LA_ObjectPool.GetVector(); 
            return LA_VectorCalculateHelp.Divide(this, scalar, c);
        }
         
        public LA_Matrix OutProductNoAlloc(LA_Vector b,LA_Matrix outMatrix)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            return LA_VectorCalculateHelp.OuterProduct(this, b, outMatrix);
        }

        public LA_Matrix OutProduct(LA_Vector b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            var m =  LA_ObjectPool.GetMatrix();
            m = OutProductNoAlloc(b, m);
            return m;
        }
 
        public float DotProduct(LA_Vector b)
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog)); 

            return LA_VectorCalculateHelp.DotProduct(this, b);
        }

        public float L1Norm()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));
            
            float norm = 0;
            for (int i = 0; i < _size; i++)
            {
                norm += Math.Abs(_data[i]);
            }
            return norm;
        } 
        public float L2Norm()
        {
            if (isInrecycle()) throw new ArgumentNullException(LA_Log.getException(LA_Log.InInrecycleLog));

            float norm = 0;
            for (int i = 0; i < _size; i++)
            {
                norm += _data[i] * _data[i];
            }
            return (float)Math.Sqrt(norm);
        }

        #endregion

        public override void Recycle()
        {
            _size = -1;
            LA_ObjectPool.RecycleVectorData(_data);
            if (_normalize !=null)
            {
                LA_ObjectPool.RecycleVector(_normalize);
                _normalize = null;
            }
            _data = null;
            _isInternal = false;  
        }
        public override string ToString()
        {
            return LogVector(3);
        }
        public string LogVector(int precision)
        {
            var result = "[ ";
            for (int i = 0; i < this.Size(); i++)
            {
                result += this.GetValue(i) + " ";
            }
            result += "]";
            return result;
        }
         
    }
}
