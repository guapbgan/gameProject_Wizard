using System;
using System.IO;
using static Wizard.MatrixFormula;


namespace Wizard
{
    class BPN
    {
        public float[,] w12, w23, w34, a1, a2, a3, a4, z2, z3, z4;
        public float eta;
        public BPN(int input_node, int hidden_node, int output_node, float eta)
        {
            w12 = InitialWeight(input_node, hidden_node);
            w23 = InitialWeight(hidden_node, hidden_node);
            w34 = InitialWeight(hidden_node, output_node);
            this.eta = eta;
        }
        public BPN(string weight12,string weight23, string weight34)
        {
            w12 = ReadDataToMatrix(weight12);
            w23 = ReadDataToMatrix(weight23);
            w34 = ReadDataToMatrix(weight34);
        }
        public void SaveWeight(string filename, float[,] w)
        {
            FileInfo finfo = new FileInfo(filename);
            if (!finfo.Exists)
            {
                if (!Directory.Exists(finfo.DirectoryName))
                {
                    Directory.CreateDirectory(finfo.DirectoryName);
                }
                StreamWriter sw = finfo.CreateText();
                for (int i = 0; i < w.GetLength(0); i++)
                {
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        sw.Write(w[i, j].ToString());
                        if (j == w.GetLength(1) - 1)
                            sw.WriteLine("");
                        else
                            sw.Write(",");
                    }
                }
                sw.Flush();
                sw.Close();
            }
            else
                throw new SystemException(filename + " exists"); ;
            

        }
        public static float[,] Sigmoid(float[,] x)
        {
            float[,] y = new float[x.GetLength(0), x.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    y[i, j] = 1 / (1 + (float)Math.Pow(Math.E, -x[i, j]));
                }
            }
            return y;
        }
        public static float[,] SigmoidGradient(float[,] x)
        {
            int rows = x.GetLength(0);
            int columns = x.GetLength(1);
            float[,] y = new float[rows, columns];
            y = MultiplyElementwise(Sigmoid(x), SubtractElementwise(Ones(rows, columns), Sigmoid(x)));
            return y;
        }
        public float[,] InitialWeight(int leftLayerNode, int rightLayerNode)
        {
            Random rnd = new Random();
            float epsilon = (float)(Math.Sqrt(6) / Math.Sqrt(leftLayerNode + rightLayerNode));
            float[,] w = new float[rightLayerNode, leftLayerNode + 1];
            for (int i = 0; i < w.GetLength(0); i++)
            {
                for (int j = 0; j < w.GetLength(1); j++)
                {
                    w[i, j] = (float)(rnd.NextDouble() * 2 * epsilon - epsilon);

                }
            }
            return w;
        }
        public float[,] Feedforward(float[,] data)
        {
            a1 = Transpose(AddOneLeft(data));
            z2 = Multiply(w12, a1);
            a2 = AddOneUp(Sigmoid(z2));
            z3 = Multiply(w23, a2);
            a3 = AddOneUp(Sigmoid(z3));
            z4 = Multiply(w34, a3);
            a4 = Sigmoid(z4);
            return a4;
        }
        public void Learning(float[,] data, float[,] target)
        {
            float[,] delta4 = MultiplyElementwise(SubtractElementwise(target, Feedforward(data)), SigmoidGradient(z4));
            float[,] delta3 = MultiplyElementwise(Multiply(Transpose(w34), delta4), SigmoidGradient(AddOneUp(z3)));
            float[,] delta2 = MultiplyElementwise(Multiply(Transpose(w23), RemoveHead(delta3)), SigmoidGradient(AddOneUp(z2)));
            float[,] w34Gradient = MultiplyScalar(eta, Multiply(delta4, Transpose(a3)));
            float[,] w23Gradient = MultiplyScalar(eta, Multiply(RemoveHead(delta3), Transpose(a2)));
            float[,] w12Gradient = MultiplyScalar(eta, Multiply(RemoveHead(delta2), Transpose(a1)));

            w34 = AddElementwise(w34, w34Gradient);
            w23 = AddElementwise(w23, w23Gradient);
            w12 = AddElementwise(w12, w12Gradient);

        }

    }

}
