using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public abstract class LA_BaseData  
    {

        abstract protected bool isInrecycle(); 

        protected long _guid=-1;
        public long GUID()
        {
            return _guid;
        }
        public  LA_BaseData(long guid)
        {
            _guid = guid; 
        }
 



        protected bool _isInternal = false;
        /// <summary>
        /// 只能内部使用
        /// </summary>
        public void SetIsInternal()
        {
            _isInternal = true;
        }
         
        protected  Action  _internalDerivedRecycleActionList;
        virtual protected void RegisterRecycleInternalDeriveFun(Action fun)
        {
            _internalDerivedRecycleActionList += fun;
        }
        abstract public void Recycle();

        /// <summary>
        /// 回收内部变量
        /// </summary>
        virtual public void _RecycleInternalVar()
        {
            _internalDerivedRecycleActionList?.Invoke();
        }
    }
}
