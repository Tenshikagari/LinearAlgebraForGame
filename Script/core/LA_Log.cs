using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinearAlgebraForGame
{
    public static class LA_Log
    {
        public static string InInrecycleLog = "已被回收，依然尝试访问或使用";
        public static string InInternalMatrixLog = "内部矩阵不允许再操作";
 
        static public string getException(string str)
        { 
            return "LA Exception: " + str; 
        }

        static public void Log(string str)
        {
            Debug.Log("LA:" + str);
        }

        static public void PoolTrackLog(string str)
        {
            Debug.Log("LA: poolTrack" + str);
        }

        static public void LogWarn(string str)
        {
            Debug.LogWarning("LA Warn: " + str); 
        }

        static public void LogError(string str)
        {
            Debug.LogError("LA Error: "+ str);
        }

    }


}
