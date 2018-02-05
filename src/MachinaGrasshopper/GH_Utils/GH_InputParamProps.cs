using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;

namespace MachinaGrasshopper.MACHINAGH_Utils
{
    /// <summary>
    /// A class to store the attributes of an input parameter, for mutability.
    /// </summary>
    public class GH_InputParamProps
    {
        public Type dataType;
        public string name, nickname, description;
        public GH_ParamAccess access;

        public GH_InputParamProps(Type type, string name, string nickname, string description, GH_ParamAccess access)
        {
            this.dataType = type;
            this.name = name;
            this.nickname = nickname;
            this.description = description;
            this.access = access;
        }

        public override string ToString()
        {
            return $"{name} {nickname} {description}";
        }
    }
}
