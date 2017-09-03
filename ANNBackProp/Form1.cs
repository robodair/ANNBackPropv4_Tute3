using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ANNShell
{
    public partial class Form1 : Form
    {
        string dir = @"D:\SC2016\SC2016ANNv3\SC2016ANNv3\ANNBackProp\ANNBackProp\DataFiles\";

        public Form1()
        {
            InitializeComponent();
        }

        class UI : UserInterface
        {
            public Form1 form;

            public UI(Form1 formQ)
            {
                form = formQ;
            }

            public override void error(string s)
            {
                form.textBox2.Text = form.textBox2.Text + "ERROR>>" + s + "\r\n";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show("ERROR>>" + s, "Error", buttons);
            }
            public override void clear(string s)
            {
                form.textBox2.Text = "";
            }
            public override void warning(string s)
            {
                form.textBox2.Text = form.textBox2.Text + "WARNING>>" + s + "\r\n";
            }
            public override void note(string s)
            {
                form.textBox2.Text = form.textBox2.Text + "NOTE>>" + s + "\r\n";
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTest1_Click(object sender, EventArgs e)
        {
            DataClass d = new DataClass();
            d.readFromFile(dir, "test1.txt");
            string s = d.showData();
            textBox1.Text = s;
            d.writeToFile(dir, "temp.txt");
            d.normalize(0, 1);
            string ss = d.showData();
            textBox1.Text = textBox1.Text +"\r\n\r\n" +ss;
            DataClass d1=null;
            DataClass d2=null;
            d.extractSplit(out d1, out d2, 4, new Random());
            string s1 = d1.showData();
            string s2 = d2.showData();
            textBox1.Text = textBox1.Text + "\r\n\r\n" + s1;
            textBox1.Text = textBox1.Text + "\r\n\r\n" + s2;

        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            DataClass d = new DataClass();
            d.readFromFile(dir, "test1.txt");
            string s = d.showData();
            textBox1.Text = s;
            textBox1.Text = textBox1.Text + "\r\n\r\n";

            DataClass dd = d.makeExemplar(2, 3, 1);
            string ss = dd.showData();
            textBox1.Text = textBox1.Text + ss;
            textBox1.Text = textBox1.Text + "\r\n\r\n";
        }

        private void btnIris_Click(object sender, EventArgs e)
        {
            // this is code for the run button
            string datafile = "IrisDataOriginalNum.txt";
            int inputs = 4;
            int outputs = 3;
            
            // iris button
            nnChart.Series["Training"].Points.Clear();
            nnChart.Series["Testing"].Points.Clear();

            textBox2.Clear(); // clear previous messages



            DataClass irisRaw = new DataClass(dir, datafile, new UI(this));
            string s = irisRaw.showDataPart(5, inputs+1, "F4", "4000 e 10");
            textBox1.AppendText(s);
            textBox1.AppendText("\r\n\r\n");

            irisRaw.normalize(0, 1);
            string ss = irisRaw.showDataPart(5, inputs+1, "F4", "4000 Data Normalised");
            textBox1.AppendText(ss);
            textBox1.AppendText("\r\n\r\n");

            DataClass irisExemplar = irisRaw.makeExemplar(inputs, outputs, 1);
            string se = irisExemplar.showDataPart(5, inputs+outputs, "F4", "4000 Exemplar Data");
            textBox1.AppendText(se);
            textBox1.AppendText("\r\n\r\n");

            DataClass trainData = new DataClass();
            DataClass testData = new DataClass();
            DataClass valData = new DataClass();
            DataClass tempData = new DataClass();

            Random rnd1 = new Random(103);
            irisExemplar.extractSplit(out trainData, out tempData, 50, rnd1);
            tempData.extractSplit(out testData, out valData, 50, rnd1);
            trainData.writeToFile(dir, "tempTrain.txt"); // debug
            testData.writeToFile(dir, "tempTest.txt");
            valData.writeToFile(dir, "tempVal.txt");
            
            string s1 = trainData.showDataPart(5, inputs+outputs, "F4", "Training Data");
            textBox1.AppendText(s1);
            textBox1.AppendText("\r\n\r\n");

            string s2 = testData.showDataPart(5, inputs+outputs, "F4", "Testing Data");
            textBox1.AppendText(s2);
            textBox1.AppendText("\r\n\r\n");

            string s3 = valData.showDataPart(5, inputs+outputs, "F4", "Validation Data");
            textBox1.AppendText(s3);
            textBox1.AppendText("\r\n\r\n");

            NeuralNetwork nn = new NeuralNetwork(inputs, 4, outputs, new UI(this), rnd1);
            nn.InitializeWeights(rnd1);
            //Console.WriteLine("\nBeginning training using incremental back-propagation\n");
            nn.train(trainData.data, testData.data, 200, 0.05, dir+"nnlog.txt", nnChart, nnProgressBar);
            //Console.WriteLine("Training complete");

            double trainAcc = nn.Accuracy(trainData.data,dir+"trainOut.txt");
            string ConfusionTrain = nn.showConfusion(dir+"trainConfusion.txt");
            double testAcc = nn.Accuracy(testData.data,dir+"testOut.txt");
            string ConfusionTest = nn.showConfusion(dir + "testConfusion.txt");
            double valAcc = nn.Accuracy(valData.data,dir+"valOut.txt");
            string ConfusionVal = nn.showConfusion(dir + "valConfusion.txt");

            // convert accuracy to percents
            trainAcc = trainAcc * 100;
            testAcc = testAcc * 100;
            valAcc = valAcc * 100;
            textBox1.AppendText("Training Accuracy   = " + trainAcc.ToString("F2") + "\r\n");
            textBox1.AppendText("Testing Accuracy    = " + testAcc.ToString("F2") + "\r\n");
            textBox1.AppendText("Validation Accuracy = " + valAcc.ToString("F2") + "\r\n");
            textBox1.AppendText("\r\n\r\n");

            textBox1.AppendText("Training Confusion Matrix \r\n"+ConfusionTrain+ "\r\n\r\n");
            textBox1.AppendText("Testing Confusion Matrix \r\n"+ConfusionTest+ "\r\n\r\n");
            textBox1.AppendText("Validation Confusion Matrix \r\n"+ConfusionVal+ "\r\n\r\n");
        }

        private void nnProgressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
