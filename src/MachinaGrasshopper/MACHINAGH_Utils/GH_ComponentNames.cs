using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaGrasshopper.MACHINAGH_Utils
{
    /// <summary>
    /// A class to store component names for mutability.
    /// </summary>
    public class GH_ComponentNames
    {
        public string name, nickname, description;

        public GH_ComponentNames(string name, string nickname, string description)
        {
            this.name = name;
            this.nickname = nickname;
            this.description = description;
        }
    }
}
