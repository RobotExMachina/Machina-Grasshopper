using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaGrasshopper.GH_Utils
{
    public class GH_Utils
    {
        public static string EnumerateList(string[] strings, string closing)
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
    }
}
