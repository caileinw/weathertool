﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WeatherTool
{
    public partial class WebView : Form
    {
        public WebView()
        {
            InitializeComponent();
        }

        int flag = 1;
        const int Guying_HTLEFT = 10;
        const int Guying_HTRIGHT = 11;
        const int Guying_HTTOP = 12;
        const int Guying_HTTOPLEFT = 13;
        const int Guying_HTTOPRIGHT = 14;
        const int Guying_HTBOTTOM = 15;
        const int Guying_HTBOTTOMLEFT = 0x10;
        const int Guying_HTBOTTOMRIGHT = 17;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0084:
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                    (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                        else m.Result = (IntPtr)Guying_HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)Guying_HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)Guying_HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)Guying_HTBOTTOM;
                    break;
                case 0x0201: //鼠标左键按下的消息
                    m.Msg = 0x00A1; //更改消息为非客户区按下鼠标
                    m.LParam = IntPtr.Zero; //默认值
                    m.WParam = new IntPtr(2);//鼠标放在标题栏内
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WebView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Hide();
        }

        private void WebView_Load(object sender, EventArgs e)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:00:00");
            string url = string.Format(ConfigurationManager.AppSettings["temperatureurl"].ToString(), now);
            webBrowser1.Url = new Uri(url);

            timer1.Interval = 10 * 1000;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:00:00");
            string temperatureurl = string.Format(ConfigurationManager.AppSettings["temperatureurl"].ToString(), now);
            string rainurl = string.Format(ConfigurationManager.AppSettings["rainurl"].ToString(), now);

            flag++;
            if (flag % 2 == 0)
                webBrowser1.Url = new Uri(rainurl);
            else
                webBrowser1.Url = new Uri(temperatureurl);

            if (flag == 10000)
                flag = 1;

        }
    }
}
