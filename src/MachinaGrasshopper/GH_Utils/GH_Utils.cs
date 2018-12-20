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
    internal class GH_Helpers
    {

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

    }
    
}
