using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;

namespace MachinaGrasshopper.GH_Utils
{
    /// <summary>
    /// A class to mimic the GH_InputParamManager class for mutable components.
    /// Add component input parameters and name attributes with `AddComponentNames()` and `AddParameter()`.
    /// </summary>
    public class GH_MutableInputParamManager
    {
        public Dictionary<bool, List<GH_InputParamProps>> inputs;
        public Dictionary<bool, GH_ComponentNames> componentNames;

        public GH_MutableInputParamManager()
        {
            inputs = new Dictionary<bool, List<GH_InputParamProps>>();
            this.ClearInputParams();
            componentNames = new Dictionary<bool, GH_ComponentNames>();
        }

        /// <summary>
        /// Adds a mutable input parameter to this component. 
        /// </summary>
        /// <param name="relative">Is this parameter for relative or absolute mode?</param>
        /// <param name="dataType">GH_Param type, as in `typeof(Param_Plane)`</param>
        /// <param name="name">Parameter name</param>
        /// <param name="nickname">Parameter nickname</param>
        /// <param name="description">Parameter description</param>
        /// <param name="access">Access level</param>
        public void AddParameter(bool relative, Type dataType, string name, string nickname, string description, GH_ParamAccess access)
        {
            this.AddParameter(relative, dataType, name, nickname, description, access, null, false);
        }

        /// <summary>
        /// Adds a mutable input parameter to this component. 
        /// </summary>
        /// <param name="relative">Is this parameter for relative or absolute mode?</param>
        /// <param name="dataType">GH_Param type, as in `typeof(Param_Plane)`</param>
        /// <param name="name">Parameter name</param>
        /// <param name="nickname">Parameter nickname</param>
        /// <param name="description">Parameter description</param>
        /// <param name="access">Access level</param>
        /// <param name="isOptional">Must this input have a connected value?</param>
        public void AddParameter(bool relative, Type dataType, string name, string nickname, string description, GH_ParamAccess access, bool isOptional)
        {
            this.AddParameter(relative, dataType, name, nickname, description, access, null, isOptional);
        }

        /// <summary>
        /// Adds a mutable input parameter to this component. 
        /// </summary>
        /// <param name="relative">Is this parameter for relative or absolute mode?</param>
        /// <param name="dataType">GH_Param type, as in `typeof(Param_Plane)`</param>
        /// <param name="name">Parameter name</param>
        /// <param name="nickname">Parameter nickname</param>
        /// <param name="description">Parameter description</param>
        /// <param name="access">Access level</param>
        /// <param name="defaultValue">Default value</param>
        public void AddParameter(bool relative, Type dataType, string name, string nickname, string description, GH_ParamAccess access, object defaultValue)
        {
            this.AddParameter(relative, dataType, name, nickname, description, access, defaultValue, false);
        }


        /// <summary>
        /// The generic overload "contructor"
        /// </summary>
        internal void AddParameter(bool relative, Type dataType, string name, string nickname, string description, GH_ParamAccess access, object defaultValue, bool isOptional)
        {
            GH_InputParamProps p = new GH_InputParamProps(dataType, name, nickname, description, access, defaultValue, isOptional);
            inputs[relative].Add(p);
        }



        /// <summary>
        /// Add mutable component names to the constructor. These names will show up when the component is dropped on the document,
        /// and whenever it toggles state. The base component names and description (the one showing on the main tab) will be determined
        /// by those used in the constructor. Do not use this method if the component is meant to not change names. 
        /// </summary>
        /// <param name="relative"></param>
        /// <param name="name"></param>
        /// <param name="nickname"></param>
        /// <param name="description"></param>
        public void AddComponentNames(bool relative, string name, string nickname, string description)
        {
            GH_ComponentNames n = new GH_ComponentNames(name, nickname, description);
            componentNames[relative] = n;
        }

        protected void ClearInputParams()
        {
            inputs[true] = new List<GH_InputParamProps>();
            inputs[false] = new List<GH_InputParamProps>();
        }

    }
}
