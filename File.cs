using AnyCAD.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDS;

namespace DDS
{
    class File
    {
        public void LoadData(string fileName,out Float32Buffer mPositions,out Float32Buffer mColors)
        {
            mPositions = new Float32Buffer(0);
            mColors = new Float32Buffer(0);
            StreamReader sr = new StreamReader(fileName);
            string str;
            string scanDate = sr.ReadLine();
            sr.ReadLine();            
            while ((str = sr.ReadLine()) != null)
            {
                string[] subs = str.Split(' ');
                if (subs.Length == 4)
                {
                    mPositions.Append(float.Parse(subs[0]));
                    mPositions.Append(float.Parse(subs[1]));
                    mPositions.Append(float.Parse(subs[2]));

                    mColors.Append(1);
                    mColors.Append(0);
                    mColors.Append(0);
                }
            }

        }
        public void LoadData2(POINT3D[] pointIn, out Float32Buffer mPositions, out Float32Buffer mColors)
        {
            mPositions = new Float32Buffer(0);
            mColors = new Float32Buffer(0);
            
            for(int i=0;i<pointIn.Count();i++)
            {
                mPositions.Append(pointIn[i].x);
                mPositions.Append(pointIn[i].y);
                mPositions.Append(pointIn[i].z);
                mColors.Append(1);
                mColors.Append(0);
                mColors.Append(0);     
            }

        }
        public void LoadDataE57(PointE57[] pointIn, out Float32Buffer mPositions, out Float32Buffer mColors)
        {
            mPositions = new Float32Buffer(0);
            mColors = new Float32Buffer(0);

            for (int i = 0; i < pointIn.Count(); i++)
            {                
                    mPositions.Append((float)pointIn[i].x);
                    mPositions.Append((float)pointIn[i].y);
                    mPositions.Append((float)pointIn[i].z);
                    mColors.Append((float)pointIn[i].red);
                    mColors.Append((float)pointIn[i].green);
                    mColors.Append((float)pointIn[i].blue);
            }
        }
    }
}
