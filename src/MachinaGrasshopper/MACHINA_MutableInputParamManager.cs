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
        public Dictionary<bool, List<MACHINA_InputParameProps>> inputs;
        public Dictionary<bool, MACHINA_ComponentNames> componentNames;

        public MACHINA_MutableInputParamManager()
        {
            inputs = new Dictionary<bool, List<MACHINA_InputParameProps>>();
            this.ClearInputParams();
            componentNames = new Dictionary<bool, MACHINA_ComponentNames>();
        }
        
        public void AddParameter(bool relative, Type dataType, string name, string nickname, string description, GH_ParamAccess access)
        {
            MACHINA_InputParameProps p = new MACHINA_InputParameProps(dataType, name, nickname, description, access);
            inputs[relative].Add(p);
        }

        public void AddComponentNames(bool relative, string name, string nickname, string description)
        {
            MACHINA_ComponentNames n = new MACHINA_ComponentNames(name, nickname, description);
            componentNames[relative] = n;
        }

        public void ClearInputParams()
        {
            inputs[true] = new List<MACHINA_InputParameProps>();
            inputs[false] = new List<MACHINA_InputParameProps>();
        }
        
    }

    public class MACHINA_InputParameProps
    {
        public Type dataType;
        public string name, nickname, description;
        public GH_ParamAccess access;

        public MACHINA_InputParameProps(Type type, string name, string nickname, string description, GH_ParamAccess access)
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
