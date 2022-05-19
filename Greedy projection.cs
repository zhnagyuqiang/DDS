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
    public partial class Greedy_projection : Form
    {
        public Greedy_projection()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = 0;
            IntPtr mesh = Algorithm.Point_cloud_gridding(MainForm.pointIn.Length, ref MainForm.pointIn[0], float.Parse(textBox1.Text), float.Parse(textBox3.Text), int.Parse(textBox2.Text),out num);
            Triangle tri = new Triangle();
            MainForm.triangles_out = new Triangle[num];
            for (int i = 0; i < num; i++)
            {
                tri = (Triangle)Marshal.PtrToStructure(new IntPtr(mesh.ToInt64() + i * Marshal.SizeOf(tri)), tri.GetType());
                MainForm.triangles_out[i] = tri;
            }
            uint idex = 0;
            Vector3 color_v = new Vector3(1.0f, 0.0f, 0.0f);
            Float32Buffer mPositions = new Float32Buffer(0);
            Float32Buffer mColors = new Float32Buffer(0);
            for (int i=0;i<num;i++)
            {
                mPositions.Append(MainForm.triangles_out[i].points1.x);
                mPositions.Append(MainForm.triangles_out[i].points1.y);
                mPositions.Append(MainForm.triangles_out[i].points1.z);
                mColors.Append(1);
                mColors.Append(0);
                mColors.Append(0);
                mPositions.Append(MainForm.triangles_out[i].points2.x);
                mPositions.Append(MainForm.triangles_out[i].points2.y);
                mPositions.Append(MainForm.triangles_out[i].points2.z);
                mColors.Append(1);
                mColors.Append(0);
                mColors.Append(0);
                mPositions.Append(MainForm.triangles_out[i].points3.x);
                mPositions.Append(MainForm.triangles_out[i].points3.y);
                mPositions.Append(MainForm.triangles_out[i].points3.z);
                mColors.Append(1);
                mColors.Append(0);
                mColors.Append(0);
            }
            var material = MeshPhongMaterial.Create("stl-material");
            material.GetTemplate().SetVertexColors(true);
            material.SetFaceSide(EnumFaceSide.DoubleSide);
            var position = BufferAttribute.Create(EnumAttributeSemantic.Position, EnumAttributeComponents.Three, mPositions);
            var color = BufferAttribute.Create(EnumAttributeSemantic.Color, EnumAttributeComponents.Three, mColors);
            BufferGeometry geometry = new BufferGeometry();
            geometry.AddAttribute(position);
            geometry.AddAttribute(color);

            NormalCalculator.ComputeVertexNormals(geometry);

            var node = new PrimitiveSceneNode(geometry, EnumPrimitiveType.TRIANGLES, material);

            node.SetPickable(false);

            //PaletteWidget pw = new PaletteWidget();
            MainForm.mRenderView.ClearAll();

            //MainForm.mRenderView.ShowSceneNode(pw);

            MainForm.mRenderView.ShowSceneNode(node);
        }
    }
}
