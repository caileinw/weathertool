using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace WeatherTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        BackGround bc = new BackGround();
        WebView wv = new WebView();
        OnDuty od = new OnDuty();
        WeatherForecast wf = new WeatherForecast();
        WeatherTrend wo = new WeatherTrend();

        private void Form_Load(object sender, EventArgs e)
        {
            label1.Hide();
            int timer = Convert.ToInt32(ConfigurationManager.AppSettings["timer"].ToString());
            timer1.Interval = timer;
            timer1.Start();
        }

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        /// <summary>
        /// 打开值班窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (od.Visible)
                od.Hide();
            else
                od.Show();
        }

        /// <summary>
        /// 打开天气趋势窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (wo.Visible)
                wo.Hide();
            else
                wo.Show();
        }

        /// <summary>
        /// 打开天气预报窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (wf.Visible)
                wf.Hide();
            else
                wf.Show();
        }

        /// <summary>
        /// 打开浏览器窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (wv.Visible)
                wv.Hide();
            else
                wv.Show();
        }

        /// <summary>
        /// 打开背景窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (bc.Visible)
                bc.Hide();
            else
                bc.Show();
        }

        /// <summary>
        /// 下载并解析天气文档数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            initWeather();
            MessageBox.Show("天气数据处理成功");
        }

        /// <summary>
        /// 上传值班excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "表格文件(*.xlsx;*.xls)|*.xlsx;*.xls";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    UtilTools.ImportExcel(openFileDialog.FileName);
            //}
            initDuty();
            MessageBox.Show("值班数据处理成功");
        }

        /// <summary>
        /// 一键初始化数据（值班数据、天气数据下载读取）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            label1.Show();
            label1.Refresh();

            this.DownWord();
            this.initWeather();
            this.initDuty();

            label1.Hide();
            MessageBox.Show("初始化数据处理成功");
        }

        /// <summary>
        /// 下载文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            this.DownWord();
            MessageBox.Show("文档下载成功");
        }

        #region 数据处理逻辑
        /// <summary>
        /// 初始化值班数据
        /// </summary>
        private void initDuty()
        {
            try
            {
                string xlsname = ConfigurationManager.AppSettings["xlsname"].ToString();
                string file = Environment.CurrentDirectory + "\\files\\" + xlsname;
                using (ExcelHelper excelHelper = new ExcelHelper(file))
                {
                    //绑定数据
                    DataTable dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    this.dataGridView1.DataSource = dt;

                    //窗口数据处理
                    DateTime now = DateTime.Now;
                    string strNow = now.ToString("yyyy年M月d日");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0].ToString() == strNow)
                        {
                            string name1 = dr[1].ToString();
                            string name2 = dr[2].ToString();
                            name1 = name1.Length == 2 ? name1.Insert(1, "  ") : name1;
                            name2 = name2.Length == 2 ? name2.Insert(1, "  ") : name2;
                            od.label1.Text = name1;
                            od.label2.Text = name2;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 初始化天气数据
        /// </summary>
        private void initWeather()
        {
            try
            {
                //根据日期匹配文档并解析
                List<string> ls = UtilTools.SpireDocProcess();
                if (ls.Count > 0)
                {
                    #region 天气预报数据
                    string forecast = ls[0];
                    string[] casts = forecast.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    wf.label1.Text = casts[0];
                    wf.label6.Text = casts[1];
                    wf.label11.Text = casts[2];
                    wf.label2.Text = casts[3];
                    wf.label5.Text = casts[4];
                    wf.label12.Text = casts[5];
                    wf.label3.Text = casts[6];
                    wf.label8.Text = casts[7];
                    wf.label13.Text = casts[8];
                    wf.label4.Text = casts[9];
                    wf.label9.Text = casts[10];
                    wf.label14.Text = casts[11];
                    wf.label5.Text = casts[12];
                    wf.label10.Text = casts[13];
                    wf.label15.Text = casts[14];
                    #endregion

                    #region 天气趋势数据
                    string head = "      ";
                    wo.label1.Text = head + ls[1];
                    wo.label2.Text = head + ls[2];
                    wo.label3.Text = head + ls[3];
                    #endregion
                }
            }
            catch (Exception ex) { };
        }

        /// <summary>
        /// 从网站下载文档
        /// </summary>
        private void DownWord()
        {
            string file = UtilTools.initFileName();
            string baseurl = ConfigurationManager.AppSettings["baseurl"].ToString();
            string url = baseurl + HttpUtility.UrlEncode(file);
            string dir = Environment.CurrentDirectory + "\\files\\";
            try
            {
                UtilTools.DownloadWord(url, dir);
            }
            catch (Exception ex) { }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                DownWord();
                initDuty();
                initWeather();
            }
            catch (Exception ex) { }

        }
    }
}
