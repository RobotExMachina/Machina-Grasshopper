using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
