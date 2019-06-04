using System;
using System.IO;

namespace Wizard
{
    class MatrixFormula
    {
        public static float[,] MultiplyElementwise(float[,] x, float[,] y)
        {
            if (x.GetLength(0) == y.GetLength(0) && x.GetLength(1) == y.GetLength(1))
            {
                float[,] z = new float[x.GetLength(0), x.GetLength(1)];
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        z[i, j] = x[i, j] * y[i, j];
                    }
                }
                return z;
            }
            else
            {
                throw new SystemException("shapes are not equal");
            }
        }
        public static float[,] MultiplyScalar(float scalar, float[,] x) 
        {
            float[,] y = new float[x.GetLength(0), x.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    y[i, j] = x[i, j] * scalar;
                }
            }
            return y;
        }
        public static float[,] Multiply(float[,] x, float[,] y)
        {
            if (x.GetLength(1) == y.GetLength(0))
            {
                float[,] z = new float[x.GetLength(0), y.GetLength(1)];
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < y.GetLength(1); j++)
                    {
                        for (int k = 0; k < x.GetLength(1); k++)
                        {
                            z[i, j] += x[i, k] * y[k, j];
                        }
                    }
                }
                return z;
            }
            else
            {
                throw new SystemException("shapes are not correct");
            }
        }

        public static float[,] SubtractElementwise(float[,] x, float[,] y)
        {
            if (x.GetLength(0) == y.GetLength(0) && x.GetLength(1) == y.GetLength(1))
            {
                float[,] z = new float[x.GetLength(0), x.GetLength(1)];
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        z[i, j] = x[i, j] - y[i, j];
                    }
                }
                return z;
            }
            else
            {
                throw new SystemException("shapes are not equal");
            }
        }
        public static float[,] AddElementwise(float[,] x, float[,] y)
        {
            if (x.GetLength(0) == y.GetLength(0) && x.GetLength(1) == y.GetLength(1))
            {
                float[,] z = new float[x.GetLength(0), x.GetLength(1)];
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        z[i, j] = x[i, j] + y[i, j];
                    }
                }
                return z;
            }
            else
            {
                throw new SystemException("shapes are not equal");
            }
        }
        public static float[,] Transpose(float[,] x)
        {
            float[,] y = new float[x.GetLength(1), x.GetLength(0)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    y[j, i] = x[i, j];
                }
            }
            return y;
        }
        public static float[,] Ones(int rows, int columns)
        {
            float[,] x = new float[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i, j] = 1;
                }
            }
            return x;
        }
        public static float[,] AddOneLeft(float[,] x)
        {
            float[,] y = new float[x.GetLength(0), x.GetLength(1) + 1];
            for (int i = 0; i < y.GetLength(0); i++)
            {
                for (int j = 0; j < y.GetLength(1); j++)
                {
                    if (j == 0)
                        y[i, j] = 1;
                    else
                        y[i, j] = x[i, j - 1];
                }
            }
            return y;
        }
        public static float[,] AddOneUp(float[,] x)
        {
            float[,] y = new float[x.GetLength(0) + 1, x.GetLength(1)];
            for (int i = 0; i < y.GetLength(0); i++)
            {
                for (int j = 0; j < y.GetLength(1); j++)
                {
                    if (i == 0)
                        y[i, j] = 1;
                    else
                        y[i, j] = x[i - 1, j];
                }
            }
            return y;
        }
        public static float[,] RemoveHead(float[,] x)  
        {
            float[,] y = new float[x.GetLength(0) - 1, x.GetLength(1)];
            for (int i = 0; i < y.GetLength(0); i++)
            {
                for (int j = 0; j < y.GetLength(1); j++)
                {
                    if (i != 0)
                        y[i - 1, j] = x[i, j];
                }
            }
            return y;
        }
        public static float[,] RemoveLeftAddzero(float[,] x)
        {
            float[,] y = new float[x.GetLength(0), x.GetLength(1)];
            for (int i = 0; i < y.GetLength(0); i++)
            {
                for (int j = 0; j < y.GetLength(1); j++)
                {
                    if (j == 0)
                        y[i, j] = 0;
                    else
                        y[i, j] = x[i, j];
                }
            }
            return y;
        }
        public static float[,] ReadDataToMatrix(string filename)
        {
            StreamReader sr;
            sr = new StreamReader(filename);
            int dataCount = 0, dataRows = 0, dataColumns = 0;
            string temp;
            string[] dataTemp;
            dataColumns = sr.ReadLine().Split(',').Length;
            dataRows++;
            while (true)
            {
                temp = sr.ReadLine();
                if (temp == null)
                    break;
                else
                    dataRows++;
            }
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            float[,] dataList = new float[dataRows, dataColumns];
            while (true)
            {
                temp = sr.ReadLine();
                if (temp == null)
                    break;
                else
                {
                    dataTemp = temp.Split(',');

                    for (int count = 0; count < dataTemp.Length; count++)
                    {
                        dataList[dataCount, count] = float.Parse(dataTemp[count]);
                    }
                    dataCount++;
                }
            }
            sr.Close();
            return dataList;

        }

    }
}
