using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DDS;

namespace PCL
{
    class Algorithm
    {


        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Statistical_filtering(string filename, string savefilename, float meanK, float Scalefactor);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void VoxelGrid_filtering(string filename, string savefilename, float Length, float Width, float height);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TranfPoint(string filename, string savefilename);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Statistical_filtering_realTime(int num,ref POINT3D p1, float meanK, float Scalefactor, ref POINT3D p2,out int num1);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr VoxelGrid_filtering_realTime(int num, ref POINT3D p1, float Length, float Width, float height, ref POINT3D p2, out int num1);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Point_cloud_gridding( int num, ref POINT3D p1,float radius,float mu,int nearb,out int num_out);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Get_E573DSize(string filename, out int num);

        [DllImport("Filter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Getdata_E573D(string filename,int index);
    }
}
