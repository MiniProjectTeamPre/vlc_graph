using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace vlc_graph {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            File.WriteAllText("vlc_graph_result.txt", "PASS\r\n" + textBox2.Text + "\r\n" + textBox3.Text);
            close_program();
        }

        private string cal_m = "";
        private string cal_c = "";
        private void timer1_Tick(object sender, EventArgs e) {
            timer1.Enabled = false;
            double sumx = 0;
            double sumy = 0;
            double sumxy = 0;
            double sumx2 = 0;
            double xba = 0;
            double yba = 0;
            double beta1 = 0;
            double beta0 = 0;
            double n = 0;
            try {
                string ss = File.ReadAllText("vlc_graph_value_atmega.txt");
                ss = File.ReadAllText("vlc_graph_value_dmm.txt");
            } catch { File.WriteAllText("vlc_graph_result.txt", "FAIL\r\nfile err"); }
            string[] value_atMega = File.ReadAllLines("vlc_graph_value_atmega.txt");
            string[] value_Dmm = File.ReadAllLines("vlc_graph_value_dmm.txt");
            File.Delete("vlc_graph_value_atmega.txt");
            File.Delete("vlc_graph_value_dmm.txt");
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            foreach (string ss in value_atMega) x.Add(Convert.ToDouble(ss));
            foreach (string ss in value_Dmm) y.Add(Convert.ToDouble(ss));

            foreach (double xa in x) { sumx += xa; }
            foreach (double ya in y) { sumy += ya; }
            for (int i = 0; i < x.Count; i++) {
                sumxy += x[i] * y[i];
                sumx2 += x[i] * x[i];
            }
            n = x.Count;
            xba = sumx / n;
            yba = sumy / n;
            beta1 = (sumxy - (sumx * sumy) / n) / (sumx2 - (n * xba * xba));
            beta0 = yba - (beta1 * xba);
            cal_m = beta1.ToString("0.00");
            cal_c = beta0.ToString("0.00");
            if (beta0 > 0) textBox1.Text = "Y = " + cal_m + "X + " + cal_c;
            else textBox1.Text = "Y = " + cal_m + "X - " + Math.Abs(beta0).ToString("0.00");
            textBox2.Text = cal_m;
            textBox3.Text = cal_c;

            chart1.Series.Clear();
            chart1.Series.Add("Data");
            chart1.Series.Add(textBox1.Text);
            chart1.Series[0].ChartType = SeriesChartType.Point;
            chart1.Series[1].ChartType = SeriesChartType.Line;

            for (int i = 0; i < x.Count; i++) {
                chart1.Series[0].Points.AddXY(x[i], y[i]);
            }
            double minX = x.ToList().Min();
            double maxX = x.ToList().Max();
            chart1.Series[1].Points.AddXY(minX, beta0 + minX * beta1);
            chart1.Series[1].Points.AddXY(maxX, beta0 + maxX * beta1);
            button1.Focus();
        }

        private void button2_Click(object sender, EventArgs e) {
            File.WriteAllText("vlc_graph_result.txt", "FAIL\r\ncancel");
            close_program();
        }
        private void close_program() {
            this.Close();
            Application.Exit();
        }
    }
}
