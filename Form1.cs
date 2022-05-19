using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCL;


namespace DDS
{
    public partial class Form1 : Form
    {
        string PonitCloudPath;
        string PonitCloudSavePath;
        string FilterPointPath;
        string FilterSavePointPath;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PonitCloudPath = "";
            PonitCloudSavePath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "文本文件(*.txt*)|*.txt*|pcd文件(*.pcd*)|*.pcd*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                PonitCloudPath = fileDialog.FileName;//返回文件的完整路径
            }
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
            {
                PonitCloudSavePath = fb.SelectedPath + textBox1.Text + ".pcd";
            }
            PCL.Algorithm.TranfPoint(PonitCloudPath, PonitCloudSavePath);
            fileDialog.Dispose();
            fb.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            POINT3D[] p_out = new POINT3D[MainForm.pointIn.Length];
            int num = 0;
            IntPtr intPtr = Algorithm.VoxelGrid_filtering_realTime(MainForm.pointIn.Length, ref MainForm.pointIn[0], float.Parse(textBox6.Text), float.Parse(textBox2.Text), float.Parse(textBox3.Text), ref p_out[0], out num);
            POINT3D pp1 = new POINT3D();
            MainForm.pointOut = new POINT3D[num];
            for (int i = 0; i < num; i++)
            {
                pp1 = (POINT3D)Marshal.PtrToStructure(new IntPtr(intPtr.ToInt64() + i * Marshal.SizeOf(pp1)), pp1.GetType());
                MainForm.pointOut[i] = pp1;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            POINT3D[] p_out = new POINT3D[MainForm.pointIn.Length];
            int num = 0;
            IntPtr intPtr = Algorithm.Statistical_filtering_realTime(MainForm.pointIn.Length, ref MainForm.pointIn[0], float.Parse(textBox8.Text), float.Parse(textBox7.Text), ref p_out[0], out num);
            POINT3D pp1 = new POINT3D();
            MainForm.pointOut = new POINT3D[num];
            for (int i=0;i<num;i++)
            {
                pp1 = (POINT3D)Marshal.PtrToStructure(new IntPtr(intPtr.ToInt64()+i* Marshal.SizeOf(pp1)), pp1.GetType());
                MainForm.pointOut[i] = pp1;
            }  
        }

    }
}
