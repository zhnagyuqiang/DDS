using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDS
{
    public partial class Progress_Bar : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">窗体标题</param>
        /// <param name="customText">滚动条上方显示的文字提示</param>
        /// <param name="total">总加载点云数</param>
        /// <param name="current">当前加载的点云序号</param>
        public Progress_Bar(string title, string customText, int total, int current)
        {
            InitializeComponent();
            this.Text = title;
            SetText(customText);
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
        }

        private delegate void SetTextHandler(string text);
        public void SetText(string text)
        {
            if (label2.InvokeRequired)
                this.Invoke(new SetTextHandler(SetText), text);
            else
            {
                if (text.ToUpper() == "CLOSE")
                    this.Close();
                else
                    label2.Text = text;
            }
        }

        private delegate void UpdateUI(int step);

        /// <summary>
        /// 更新UI
        /// </summary>
        /// <param name="step"></param>
        public void UpdataUIStatus(int step)
        {
            try
            {
                if (this.IsDisposed)
                    return;
                if (InvokeRequired)
                {
                    this.Invoke(new UpdateUI(delegate (int s)
                    {
                        this.progressBar1.Value = s;
                        //this.progressBar1.Value += s;
                        this.label1.Text = this.progressBar1.Value.ToString() + "/" + this.progressBar1.Maximum.ToString();
                        if (this.progressBar1.Value == this.progressBar1.Maximum)
                            this.Close();
                    }), step);
                }
                else
                {
                    if (this.progressBar1.Value >= this.progressBar1.Maximum)
                        this.progressBar1.Value = 0;
                    this.progressBar1.Value = step;
                    //this.progressBar1.Value += step;
                    this.label1.Text = this.progressBar1.Value.ToString() + "/" + this.progressBar1.Maximum.ToString();
                }
            }
            catch (Exception ex) { }
        }

        public class ProgressBarService
        {
            private Thread thred = null;
            private Progress_Bar bar = null;

            public ProgressBarService() { }

            private static ProgressBarService _instance;
            private static readonly Object syncLock = new Object();
            public static ProgressBarService Instance
            {
                get
                {
                    if (ProgressBarService._instance == null)
                    {
                        lock (syncLock)
                        {
                            if (ProgressBarService._instance == null)
                            {
                                ProgressBarService._instance = new ProgressBarService();
                            }
                        }
                    }
                    return ProgressBarService._instance;
                }
            }

            #region 创建等待窗口
            /// <summary>
            /// 创建等待窗口并写入提示文字
            /// </summary>
            /// <param name="title">标题</param>
            /// <param name="str">提示文字</param>
            /// <param name="step">步长</param>
            /// <param name="showCustomClose">是否显示自定义的那个关闭窗口按钮(true显示;false隐藏)，不传值默认fasle</param>
            public static void CreateBarForm(string title, string text, int step, int current)
            {
                ProgressBarService.Instance.CreateForm(title, text, step, current);
            }

            private void CreateForm(string title, string text, int step,  int current)
            {
                if (thred != null)
                {
                    try
                    {
                        thred = null;
                        bar = null;
                    }
                    catch (Exception)
                    {
                    }
                }

                thred = new Thread(new ThreadStart(delegate ()
                {
                    bar = new Progress_Bar(title, text, step, current);
                    System.Windows.Forms.Application.Run(bar);
                }));
                thred.Start();

            }
            #endregion

            #region 关闭等待窗口
            /// <summary>
            /// 关闭窗口
            /// </summary>
            public static void CloseBarForm()
            {
                ProgressBarService.Instance.CloseForm();
            }

            public void CloseForm()
            {
                if (thred != null)
                {
                    try
                    {
                        bar.SetText("CLOSE");
                        thred = null;
                        bar = null;
                    }
                    catch (Exception) { }
                }
            }
            #endregion

            #region 修改等待窗体的提示文字
            /// <summary>
            /// 修改提示文字
            /// </summary>
            /// <param name="text">提示文字</param>
            public static void SetBarFormCaption(string text)
            {
                ProgressBarService.Instance.SetFormCaption(text);
            }

            private void SetFormCaption(string text)
            {
                if (bar != null)
                {
                    try
                    {
                        bar.SetText(text);
                    }
                    catch (Exception) { }
                }
            }
            #endregion

            #region 更新进度条
            /// <summary>
            /// 更新进度
            /// </summary>
            /// <param name="step"></param>
            public static void UpdateProgress(int step)
            {
                ProgressBarService.Instance.SetProgress(step);
            }

            private void SetProgress(int step)
            {
                if (bar != null)
                {
                    try
                    {
                        bar.UpdataUIStatus(step);
                    }
                    catch (Exception) { }
                }
            }
            #endregion
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Progress_Bar_Load(object sender, EventArgs e)
        {

        }
    }
}
