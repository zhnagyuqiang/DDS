using AnyCAD.Forms;
using AnyCAD.Foundation;
using PCL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDS;
using System.Threading;
using static DDS.Progress_Bar;

namespace DDS
{
    public partial class MainForm : Form
    {
        public static RenderControl mRenderView;        
        public static int Load_nums = 0;
        private String m_NodeName = null;
        public static POINT3D[] pointIn;
        public static POINT3D[] pointOut;
        public static PointE57[] pointE57In;
        public static PointE57[] pointE57Out;
        public static Dictionary<int,POINT3D[]> hashMap = new Dictionary<int,POINT3D[]>();
        public static Dictionary<int, PointE57[]> hashMapE57 = new Dictionary<int, PointE57[]>();
        public static Triangle[] triangles_out;        
        public TreeNode currentNode = new TreeNode();
        public TreeNode root = new TreeNode();

        public MainForm()
        {            
            InitializeComponent();
            mRenderView = new RenderControl(this.splitContainer1.Panel2);
            root.Name = "root";
            root.Text = "载入点云";
            treeView1.Nodes.Add(root);
        }
  

        //初始化目录树
        private void InitTree(string name)
        {
            root.Nodes.Clear();
            TreeNode pointcloud = new TreeNode();
            TreeNode mesh_point = new TreeNode();
            TreeNode filter = new TreeNode();
            filter.Name = "filter";
            filter.Text = "滤波";
            TreeNode mesh = new TreeNode();
            mesh.Name = "mesh";
            mesh.Text = "网格化";
            TreeNode volume = new TreeNode();
            volume.Name = "volume";
            volume.Text = "计算体积";
            TreeNode parameter_settings = new TreeNode();
            parameter_settings.Name = "filter_set";
            parameter_settings.Text = "参数设置";
            TreeNode parameter_settings1 = new TreeNode();
            parameter_settings1.Name = "mesh_set";
            parameter_settings1.Text = "参数设置";
            TreeNode parameter_settings2 = new TreeNode();
            parameter_settings2.Name = "volume_set";
            parameter_settings2.Text = "参数设置";
            TreeNode show = new TreeNode();
            show.Name = "show_point";
            show.Text = "显示";
            TreeNode show1 = new TreeNode();
            show1.Name = "show_point";
            show1.Text = "显示";
            TreeNode show2 = new TreeNode();
            show2.Name = "show_point";  
            show2.Text = "显示";
            filter.Nodes.Add(parameter_settings);
            filter.Nodes.Add(show);
            mesh.Nodes.Add(parameter_settings1);
            mesh.Nodes.Add(show1);
            volume.Nodes.Add(parameter_settings2);
            pointcloud.Name = Load_nums.ToString();
            pointcloud.Text = name;
            pointcloud.Nodes.Add(filter);
            pointcloud.Nodes.Add(mesh);
            root.Nodes.Add(pointcloud);    
        }

        private void InitE57(string path,int num)
        {
            TreeNode E57 = new TreeNode();
            E57.Text = path;
            E57.Name = "E57";
            for(int i=0;i<num;i++)
            {
                TreeNode E573D = new TreeNode();
                E573D.Text = "点云" + i.ToString();
                E573D.Name = "E573D";
                E57.Nodes.Add(E573D);
            }
            root.Nodes.Add(E57);
        }
      

        //设置树单选,就是只能有一个树节点被选中
        private void SetNodeCheckStatus(TreeNode tn, TreeNode node)
        {
            if (tn == null)
                return;
            if (tn != node)
            {
                tn.Checked = false;
            }
            // Check children nodes
            foreach (TreeNode tnChild in tn.Nodes)
            {
                if (tnChild != node)
                {
                    tnChild.Checked = false;
                }
                SetNodeCheckStatus(tnChild, node);
            }
        }
        private void openFile()
        {
            
            hashMap.Clear();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "点云格式(*.*)|*.*";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                InitTree(dialog.FileName);

                if (dialog.FileName.Contains("e57"))
                {
                    List<PointE57[]> E573DDatas=ReadPointE57(dialog.FileName,  pointE57In);
                    
                    TreeNode node_point = new TreeNode();
                    node_point.Name = Load_nums.ToString();//设置点云名称为载入次数
                    node_point.Text = dialog.FileName;                
                    hashMap.Add(Load_nums, pointIn);
                    Load_nums++;
                    
                }
                else
                {
                    Float32Buffer mPositions;
                    Float32Buffer mColors;
                    ReadPoint(dialog.FileName, out pointIn);
                    Manager.fileOperator.LoadData(dialog.FileName, out mPositions, out mColors);
                    var material = BasicMaterial.Create("point-material");
                    material.GetTemplate().SetVertexColors(true);
                    var geometry = GeometryBuilder.CreatePoints(new Float32Array(mPositions), new Float32Array(mColors));
                    var node = new PrimitiveSceneNode(geometry, material);
                    //node.SetPickable(false);
                    mRenderView.ClearAll();
                    mRenderView.ShowSceneNode(node);
                    mRenderView.ZoomAll();
                    TreeNode node_point = new TreeNode();
                    node_point.Name = Load_nums.ToString();//设置点云名称为载入次数
                    node_point.Text = dialog.FileName;
                    //CurrentNode.Nodes.Add(node_point);
                    hashMap.Add(Load_nums, pointIn);
                    Load_nums++;
                }
                
                //operator_num++;
            }
        }

    

        public void ReadPoint(string path, out POINT3D[] pointIn)
        {
            StreamReader sr = new StreamReader(path);
            sr.ReadLine();
            sr.ReadLine();
            List<POINT3D> p3 = new List<POINT3D>();
            string line = sr.ReadLine();
            while (line != null)
            {
                string[] line1 = line.Split(new char[]{' ',',' });
                POINT3D pt = new POINT3D();
                pt.x = float.Parse(line1[0]);
                pt.y = float.Parse(line1[1]);
                pt.z = float.Parse(line1[2]);
                p3.Add(pt);
                line = sr.ReadLine();
            }
            pointIn = new POINT3D[p3.Count()];
            for (int i = 0; i < p3.Count(); i++)
            {
                pointIn[i] = p3[i];
            }
        }

        public List<PointE57[]> ReadPointE57(string path,  PointE57[] pointE57)
        {
            Progress_Bar.ProgressBarService.CreateBarForm("点云读取中", "scan point[0]", 100, 0);
            
            Float32Buffer mPositions;
            Float32Buffer mColors;
            int num = 0;
            int sum_points = 0;
            PointE57 pE57 = new PointE57();
            List<PointE57[]> pointE57s = new List<PointE57[]>();
            IntPtr size = PCL.Algorithm.Get_E573DSize(path, out num);
            InitE57(path,num);
            List<int> pointsSize = new List<int>();
            int num_singlePoint = new int();
            
            for (int i = 0; i < num; i++)
            {
                Progress_Bar.ProgressBarService.SetBarFormCaption("scan point["+i+"]");
                
                num_singlePoint = (int)Marshal.PtrToStructure((size + i * Marshal.SizeOf(num_singlePoint)), num_singlePoint.GetType());
                for (int k = 0; k < 100; k++)
                {
                    Thread.Sleep(num_singlePoint/50000);
                    ProgressBarService.UpdateProgress(k);
                }
                IntPtr points = PCL.Algorithm.Getdata_E573D(path,i);
                pointE57 = new PointE57[num_singlePoint];
                for (int j=0;j< num_singlePoint;j++)
                {
                    
                    pE57 = (PointE57)Marshal.PtrToStructure((points + j * Marshal.SizeOf(pE57)), pE57.GetType());
                    pointE57[j] = pE57;
                   
                }
                hashMapE57.Add(i, pointE57);
                pointsSize.Add(num_singlePoint);
                sum_points += num_singlePoint;
                Manager.fileOperator.LoadDataE57(pointE57,out mPositions,out mColors);
                var material = BasicMaterial.Create("point-material");
                material.GetTemplate().SetVertexColors(true);
                var geometry = GeometryBuilder.CreatePoints(new Float32Array(mPositions), new Float32Array(mColors));
                var node = new PrimitiveSceneNode(geometry, material);
                //node.SetPickable(false);
                mRenderView.ClearAll();
                mRenderView.ShowSceneNode(node);
                mRenderView.ZoomAll();
                pointE57s.Add(pointE57);
            }
            //IntPtr points = PCL.Algorithm.Getdata_E573D(path);
            
            
            //int ikj = Marshal.SizeOf(pE57);
            //pointE57 = new PointE57[sum_points];
            //int size4 = Marshal.SizeOf(pE57);
            //int count = 0;
            //for (int i = 0; i < num; i++)
            //{
            //    pointE57 = new PointE57[pointsSize[i]];
            //    for (int j = 0; j < pointsSize[i];j++)
            //    {
                    
            //        ++count;
            //        pointE57[i] = (pE57);
            //    }
            //    pointE57s.Add(pointE57);
            //}
            return pointE57s;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //打开文件
            //var path = "";
            this.Cursor = Cursors.WaitCursor;
            openFile();
            Progress_Bar.ProgressBarService.CloseBarForm();        //事件完成后关闭等待窗体
            this.Cursor = Cursors.Default;

        }
        public void AddTreeNode()
        {
            TreeNode node_point = new TreeNode();
            node_point.Name = Load_nums.ToString();//设置点云名称为0
            node_point.Text = treeView1.SelectedNode.Text;
            treeView1.SelectedNode.Parent.Nodes.Add(node_point);
        }

        public void AddTreeNode_mesh()
        {
            TreeNode node_point = new TreeNode();
            node_point.Name = "2";//设置点云名称为0
            node_point.Text = "mesh";
            treeView1.SelectedNode.Parent.Nodes.Add(node_point);
        }
        private void treeView1_Cheacked(object sender, TreeViewEventArgs e)
        {
            //过滤不是鼠标选中的其它事件，防止死循环
            if (e.Action != TreeViewAction.Unknown)
            {
                //Event call by mouse or key-press
                foreach (TreeNode tnChild in treeView1.Nodes)
                    SetNodeCheckStatus(tnChild, e.Node);
                string sName = e.Node.Text;
            }
        }
        //获得选择节点
        private void GetSelectNode(TreeNode tn)
        {
            if (tn == null)
                return;
            if (tn.Checked == true)
            {
                m_NodeName = tn.Text;
                return;
            }
        }
        private void button100_Click(object sender, EventArgs e)
        {
           //TreeNode node = null;
            foreach (TreeNode tnChild in treeView1.Nodes)
            {
                GetSelectNode(tnChild);
            }
            string sName = m_NodeName;
        }
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //打开文件
            //var path = "";
            this.Cursor = Cursors.WaitCursor;
            openFile();
            Progress_Bar.ProgressBarService.CloseBarForm();        //事件完成后关闭等待窗体
            this.Cursor = Cursors.Default;
        }
        //选择树的节点并点击右键，触发事件
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                Point ClickPoint = new Point(e.X, e.Y);
                currentNode = treeView1.GetNodeAt(ClickPoint);
                if (currentNode != null)//判断你点的是不是一个节点
                {
                    Point point = treeView1.PointToClient(Control.MousePosition);
                    currentNode = treeView1.GetNodeAt(point);
                    switch (currentNode.Name)//根据不同节点显示不同的右键菜单，当然你可以让它显示一样的菜单
                    {
                        
                        case "root":
                            currentNode.ContextMenuStrip = contextMenuStrip1;
                            break;
                        case "2":
                            currentNode.ContextMenuStrip = contextMenuStrip3;
                            break;
                        case "E573D":
                            currentNode.ContextMenuStrip = contextMenuStrip4;
                            break;
                        default:
                            //currentNode.ContextMenuStrip = contextMenuStrip2;
                            break;
                    }
                    
                }
                else
                { }                                
                
            }
            else if (e.Button == MouseButtons.Left&&e.Clicks==2)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                currentNode = treeView1.GetNodeAt(ClickPoint);
                if (currentNode != null)//判断你点的是不是一个节点
                {
                    //Point point = treeView1.PointToClient(Control.MousePosition);
                    //CurrentNode = treeView1.GetNodeAt(point);
                    switch (currentNode.Name)//根据不同节点显示不同的右键菜单，当然你可以让它显示一样的菜单
                    {

                        case "filter_set":
                            int i = int.Parse(currentNode.Parent.Parent.Name);
                            if (hashMap.ContainsKey(i))
                            {
                                pointIn = hashMap[i];
                            }
                            Form2 f = new Form2();
                            f.ShowDialog();
                            //CurrentNode.ContextMenuStrip = contextMenuStrip1;
                            break;
                        case "2":
                            //CurrentNode.ContextMenuStrip = contextMenuStrip3;
                            break;
                        default:
                            //currentNode.ContextMenuStrip = contextMenuStrip2;
                            break;
                    }

                }
            }
        }

        private void statisticalfilteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = int.Parse(currentNode.Name);
            if (hashMap.ContainsKey(i))
            {
                pointIn = hashMap[i];
            }
            Form2 f = new Form2();
            f.ShowDialog();
            AddTreeNode();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int i = int.Parse(treeView1.SelectedNode.Name);
            if (hashMap.ContainsKey(i))
            {
                pointIn = hashMap[i];
            }
            VoxelGrid_filtering vf = new VoxelGrid_filtering();
            vf.ShowDialog();
            AddTreeNode();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void greedyProjectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = int.Parse(treeView1.SelectedNode.Name);
            if (hashMap.ContainsKey(i))
            {
                pointIn = hashMap[i];
            }
            Greedy_projection f = new Greedy_projection();
            f.ShowDialog();
            AddTreeNode_mesh();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = int.Parse(treeView1.SelectedNode.Name);
            if (hashMap.ContainsKey(i))
            {
                pointIn = hashMap[i];
            }
            Float32Buffer mPositions;
            Float32Buffer mColors;
            //ReadPoint(dialog.FileName, out pointIn);
            Manager.fileOperator.LoadData2(pointIn, out mPositions, out mColors);
            var material = BasicMaterial.Create("point-material");
            material.GetTemplate().SetVertexColors(true);
            var geometry = GeometryBuilder.CreatePoints(new Float32Array(mPositions), new Float32Array(mColors));
            var node = new PrimitiveSceneNode(geometry, material);
            //node.SetPickable(false);
            mRenderView.ClearAll();
            mRenderView.ShowSceneNode(node);
            mRenderView.ZoomAll();
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {

        }

        private void calculateTheVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 选择显示颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog()==DialogResult.OK)
            {
                int name =int.Parse(currentNode.Text.Remove(0,2));
                if(hashMapE57.Keys.Contains(name))
                {
                    pointE57In = hashMapE57[name];
                }
                for(int i=0;i<pointE57In.Count();i++)
                {
                    pointE57In[i].red = colorDialog1.Color.R;
                    pointE57In[i].blue = colorDialog1.Color.B;
                    pointE57In[i].green = colorDialog1.Color.G;
                }
                Float32Buffer mPositions;
                Float32Buffer mColors;
                Manager.fileOperator.LoadDataE57(pointE57In,out mPositions,out mColors);
                var material = BasicMaterial.Create("point-material");
                material.GetTemplate().SetVertexColors(true);
                var geometry = GeometryBuilder.CreatePoints(new Float32Array(mPositions), new Float32Array(mColors));
                var node = new PrimitiveSceneNode(geometry, material);
                //node.SetPickable(false);
                mRenderView.ClearAll();
                mRenderView.ShowSceneNode(node);
                mRenderView.ZoomAll();
            }
        }
    }
}
