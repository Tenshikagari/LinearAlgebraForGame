using LinearAlgebraForGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LA_MatrixQRTest : MonoBehaviour
{

    public LA_MatrixEnum.QR testType;
    [ContextMenu("Q")]
    void QRTestF()
    {

        LA_Matrix A = LA_MatrixBuildHelper.BuildMatrixByFunc(3, 3, (i, j) =>
        {
            float[,] demoValues = {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };
            return demoValues[i, j];
        });

          //A = LA_MatrixBuildHelper.RandomMatrix(5, 10);

        print($"原矩阵  {A } 秩{A.Rank()}"); 

        var QRResult = LA_MatrixCalculateHelp.GetQR(A, testType);
        LA_Matrix Q = QRResult.Item1;
        LA_Matrix R = QRResult.Item2; 
        print($"\n Q: \n {Q.ToString()}   \n R: \n{R.ToString()}");

        print($"\n 还原 {Q.Mul(R)}");
        print($"\n QQT {Q.Mul(Q.T())}"); 
    }
}
