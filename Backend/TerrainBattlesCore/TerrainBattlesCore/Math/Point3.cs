using System;

namespace TerrainBattlesCore.Math
{
    [Serializable]
    public struct Point3
    {
        public float x, y, z;

        public static float Distance(Point3 a, Point3 b)
        {
            return (float)System.Math.Sqrt(System.Math.Pow(a.x - b.x, 2) + System.Math.Pow(a.y - b.y, 2) + System.Math.Pow(a.z - b.z, 2));
        }
        
        public static bool operator ==(Point3 a, Point3 b)
        {
            return System.Math.Abs(a.x - b.x) < .01f && System.Math.Abs(a.y - b.y) < .01f && System.Math.Abs(a.z - b.z) < .01f;
        }

        public static bool operator !=(Point3 a, Point3 b) 
        {
            return !(System.Math.Abs(a.x - b.x) < .01f && System.Math.Abs(a.y - b.y) < .01f && System.Math.Abs(a.z - b.z) < .01f);
        }
    }
}