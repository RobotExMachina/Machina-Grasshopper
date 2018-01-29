using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaGrasshopper
{
    public class MACHINA_MutableInputParamManager
    {
        public Dictionary<bool, List<MACHINA_InputParameteProperties>> inputs;
        public Dictionary<bool, MACHINA_ComponentNames> componentNames;

        public MACHINA_MutableInputParamManager()
        {
            inputs = new Dictionary<bool, List<MACHINA_InputParameteProperties>>();
            this.ClearInputParams();
            componentNames = new Dictionary<bool, MACHINA_ComponentNames>();
        }
        
        public void AddParameter(bool relative, Type dataType, string name, string nickname, string description, GH_ParamAccess access)
        {
            MACHINA_InputParameteProperties p = new MACHINA_InputParameteProperties(dataType, name, nickname, description, access);
            inputs[relative].Add(p);
        }

        public void AddComponentNames(bool relative, string name, string nickname, string description)
        {
            MACHINA_ComponentNames n = new MACHINA_ComponentNames(name, nickname, description);
            componentNames[relative] = n;
        }

        public void ClearInputParams()
        {
            inputs[true] = new List<MACHINA_InputParameteProperties>();
            inputs[false] = new List<MACHINA_InputParameteProperties>();
        }
        
    }

    public class MACHINA_InputParameteProperties
    {
        public Type dataType;
        public string name, nickname, description;
        public GH_ParamAccess access;

        public MACHINA_InputParameteProperties(Type type, string name, string nickname, string description, GH_ParamAccess access)
        {
            this.dataType = type;
            this.name = name;
            this.nickname = nickname;
            this.description = description;
            this.access = access;
        }
    }

    public class MACHINA_ComponentNames
    {
        public string name, nickname, description;

        public MACHINA_ComponentNames(string name, string nickname, string description)
        {
            this.name = name;
            this.nickname = nickname;
            this.description = description;
        }
    }
}
