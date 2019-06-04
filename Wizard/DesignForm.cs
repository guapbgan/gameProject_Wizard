using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace Wizard
{
    public partial class DesignForm : Form
    {
        public DesignForm()
        {
            InitializeComponent();
        }

        Panel pan;
        BPN bpn;



        private void Form1_Load(object sender, EventArgs e)
        {
            pan = new Panel(this,385,50);
            bpn = new BPN(pan.dataSize, 5, 5, float.Parse(textBox2.Text));
            //bpn = new BPN("w12", "w23", "w34");

        }

        int countD, countA, countH, countP, countM = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            pan.SaveData(new float[,] { { 1f }, { 0 }, { 0 }, { 0 }, { 0 } }, pan.Read(), "traingingData.txt");
            pan.Initialize();
            countD++;
            labelD.Text = countD.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pan.SaveData(new float[,] { { 0 }, { 1f }, { 0 }, { 0 }, { 0 } }, pan.Read(), "traingingData.txt");
            pan.Initialize();
            countA++;
            labelA.Text = countA.ToString();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            pan.SaveData(new float[,] { { 0 }, { 0 }, { 1f }, { 0 }, { 0 } }, pan.Read(), "traingingData.txt");
            pan.Initialize();
            countH++;
            labelH.Text = countH.ToString();
        }
        private void button10_Click(object sender, EventArgs e)
        {

            pan.SaveData(new float[,] { { 0 }, { 0 }, { 0 }, { 1f }, { 0 } }, pan.Read(), "traingingData.txt");
            pan.Initialize();
            countP++;
            labelP.Text = countP.ToString();

        }
        private void button11_Click(object sender, EventArgs e)
        {

            pan.SaveData(new float[,] { { 0 }, { 0 }, { 0 }, { 0 }, { 1f } }, pan.Read(), "traingingData.txt");
            pan.Initialize();
            countM++;
            labelM.Text = countM.ToString();

        }
        private void button2_Click(object sender, EventArgs e)
        {
            float[,] result = bpn.Feedforward(pan.Read());
            textBox1.Text = result[0, 0].ToString() + ", " + result[1, 0].ToString() + ", " + result[2, 0].ToString() + ", " +
                            result[3, 0].ToString() + ", " + result[4, 0].ToString();
            pan.Initialize();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pan.SetPattern(pan.patternA, Color.Red);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pan.SetPattern(pan.patternD, Color.Red);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pan.SetPattern(pan.patternH, Color.Red);
        }
        private void button12_Click(object sender, EventArgs e)
        {
            pan.SetPattern(pan.patternP, Color.Red);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bpn.SaveWeight("w12.txt", bpn.w12);
            bpn.SaveWeight("w23.txt", bpn.w23); // group it if free
            bpn.SaveWeight("w34.txt", bpn.w34);
        }
        bool flag = false;
        float[,] dataList;
        private void button9_Click(object sender, EventArgs e)// fix it if free
        {
            float[,] x, y;
            x = new float[1, 400];
            y = new float[5, 1];
            bpn.eta = float.Parse(textBox2.Text);
            if (flag == false)
            {
                dataList = MatrixFormula.ReadDataToMatrix("traingingData.txt");
                flag = true;
            }
            for (int times = 0; times < int.Parse(textBox3.Text); times++)
            {
                for (int i = 0; i < dataList.GetLength(0); i++)
                {
                    for (int j = 0; j < dataList.GetLength(1); j++)
                    {
                        if (j < 5)
                            y[j, 0] = dataList[i, j];
                        else
                            x[0, j - 5] = dataList[i, j];
                    }
                    bpn.Learning(x, y);
                    bpn.Learning(pan.Read(), new float[5, 1]);
                }
            }
            MessageBox.Show("done");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            pan.SetPattern(pan.patternM, Color.Red);
        }
    }
}
