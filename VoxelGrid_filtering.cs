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

namespace DDS
{
    public partial class VoxelGrid_filtering : Form
    {
        public VoxelGrid_filtering()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.Load_nums++;
            POINT3D[] p_out = new POINT3D[MainForm.pointIn.Length];
            int num = 0;
            IntPtr intPtr = Algorithm.VoxelGrid_filtering_realTime(MainForm.pointIn.Length, ref MainForm.pointIn[0], float.Parse(textBox1.Text), float.Parse(textBox2.Text), float.Parse(textBox4.Text), ref p_out[0], out num);
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
    }
}
