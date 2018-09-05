using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;


namespace MachinaGrasshopper.GH_Utils
{
    /// <summary>
    /// Some utility helpers.
    /// </summary>
    internal class Helpers
    {
        internal static string EnumerateList(string[] strings, string closing)
        {
            switch(strings.Length)
            {
                case 0:
                    return "";

                case 1:
                    return strings[0];

                default:
                    string str = "";
                    for (int i = 0; i < strings.Length; i++)
                    {
                        str += "\"" + strings[i] + "\"";
                        if (i < strings.Length - 2)
                        {
                            str += ", ";
                        }
                        else if (i == strings.Length - 2)
                        {
                            str += " " + closing + " ";
                        }
                    }
                    return str;
            }
        }

        /// <summary>
        /// Given an array of generic objects, returns a string representation using their own .ToString()
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        internal static string ObjectArrayToString(object[] objs)
        {
            if (objs == null) return null;

            string str = "";
            for (int i = 0; i < objs.Length; i++)
            {
                str += objs[i]?.ToString() ?? "null";
                if (i < objs.Length - 1)
                {
                    str += ',';
                }
            }
            return str;
        }

        /// <summary>
        /// Takes an array of objects, and tries to covnert them to doubles.
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        internal static double?[] NullableDoublesFromObjects(object[] objs)
        {
            if (objs == null) return null;

            double?[] d = new double?[objs.Length];

            for (int i = 0; i < d.Length; i++)
            {
                try
                {
                    d[i] = Convert.ToDouble(objs);
                }
                catch
                {
                    d[i] = null;
                }
            }

            return d;
        }




        internal static bool AreSimilar(Point3d a, Point3d b, double epsilon)
        {
            return Math.Abs(a.X - b.X) < epsilon && Math.Abs(a.Y - b.Y) < epsilon && Math.Abs(a.Z - b.Z) < epsilon;
        }

        internal static bool AreSimilar(Vector3d a, Vector3d b, double epsilon)
        {
            return Math.Abs(a.X - b.X) < epsilon && Math.Abs(a.Y - b.Y) < epsilon && Math.Abs(a.Z - b.Z) < epsilon;
        }

        internal static bool AreSimilar(Line a, Line b, double epsilon)
        {
            return AreSimilar(a.From, b.From, epsilon) && AreSimilar(a.To, b.To, epsilon);
        }

        internal static bool AreSimilar(Plane a, Plane b, double epsilon)
        {
            return AreSimilar(a.Origin, b.Origin, epsilon) && AreSimilar(a.XAxis, b.XAxis, epsilon) && AreSimilar(a.YAxis, b.YAxis, epsilon);
        }

        internal static bool AreSimilar(double?[] a, double?[] b, double epsilon)
        {
            if (a.Length != b.Length) return false;

            double diff;
            for (int i = 0; i < a.Length; i++)
            {
                try
                {
                    if (a[i] == null && b[i] == null) continue;
                    diff = (double)(a[i] - b[i]);
                    if (Math.Abs(diff) > epsilon)
                        return false;
                }
                catch
                {
                    // Something went wrong with casting...
                    return false;
                }
            }

            return true;
        }

    }
    
}
