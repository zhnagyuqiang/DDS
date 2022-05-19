using AnyCAD.Foundation;
using PCL;
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
using DDS;

namespace DDS
{
    public partial class Form2 : Form
    {
        public bool IsFilter_sf = false;
        private Rectangle tabArea;
        private RectangleF tabTextArea;
        public Form2()
        {
            InitializeComponent();
        }
        private void TabSetMode()
        {
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.Alignment = TabAlignment.Left;
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            tabArea = tabControl1.GetTabRect(e.Index);
            tabTextArea = tabArea;
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            Font font = this.tabControl1.Font;
            SolidBrush brush = new SolidBrush(Color.Black);
            g.DrawString(((TabControl)(sender)).TabPages[e.Index].Text, font, brush, tabTextArea, sf);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.Load_nums++;
            POINT3D[] p_out = new POINT3D[MainForm.pointIn.Length];
            int num = 0;
            IntPtr intPtr = Algorithm.Statistical_filtering_realTime(MainForm.pointIn.Length, ref MainForm.pointIn[0], float.Parse("10"), float.Parse("1.0"), ref p_out[0], out num);
            POINT3D pp1 = new POINT3D();
            MainForm.pointOut = new POINT3D[num];
            for (int i = 0; i < num; i++)
            {
                pp1 = (POINT3D)Marshal.PtrToStructure(new IntPtr(intPtr.ToInt64() + i * Marshal.SizeOf(pp1)), pp1.GetType());
                MainForm.pointOut[i] = pp1;
            }
            MainForm.hashMap.Add(MainForm.Load_nums, MainForm.pointOut);
            Float32Buffer mPositions;
            Float32Buffer mColors;
            Manager.fileOperator.LoadData2(MainForm.pointOut, out mPositions, out mColors);
            var material = BasicMaterial.Create("point-material");
            material.GetTemplate().SetVertexColors(true);
            var geometry = GeometryBuilder.CreatePoints(new Float32Array(mPositions), new Float32Array(mColors));
            var node = new PrimitiveSceneNode(geometry, material);
            //node.SetPickable(false);
            MainForm.mRenderView.ClearAll();
            MainForm.mRenderView.ShowSceneNode(node);
            MainForm.mRenderView.ZoomAll();
            
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
