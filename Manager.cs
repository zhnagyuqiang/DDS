using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDS
{

    class Manager
    {
        public static File fileOperator = new File();

        
        
    }

    [Serializable]
    public struct POINT3D
    {
        [NonSerialized]
        public float x;

        [NonSerialized]
        public float y;

        [NonSerialized]
        public float z;
    }

    [Serializable]
    public struct Triangle
    {
        [NonSerialized]
        public POINT3D points1;
        [NonSerialized]
        public POINT3D points2;
        [NonSerialized]
        public POINT3D points3;
        //float vector[3];
    };

    [Serializable]
    public struct PointE57
    {
        [NonSerialized]
        public double x, y, z;
        [NonSerialized]
        public double intensity;
        [NonSerialized]
        public int red, green, blue;
    };
}
