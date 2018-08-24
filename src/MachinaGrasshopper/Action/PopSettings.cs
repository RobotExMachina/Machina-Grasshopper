using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;

using Machina;
using MachinaGrasshopper.GH_Utils;

namespace MachinaGrasshopper.Action
{
    //  ██████╗  ██████╗ ██████╗ ███████╗███████╗████████╗████████╗██╗███╗   ██╗ ██████╗ ███████╗
    //  ██╔══██╗██╔═══██╗██╔══██╗██╔════╝██╔════╝╚══██╔══╝╚══██╔══╝██║████╗  ██║██╔════╝ ██╔════╝
    //  ██████╔╝██║   ██║██████╔╝███████╗█████╗     ██║      ██║   ██║██╔██╗ ██║██║  ███╗███████╗
    //  ██╔═══╝ ██║   ██║██╔═══╝ ╚════██║██╔══╝     ██║      ██║   ██║██║╚██╗██║██║   ██║╚════██║
    //  ██║     ╚██████╔╝██║     ███████║███████╗   ██║      ██║   ██║██║ ╚████║╚██████╔╝███████║
    //  ╚═╝      ╚═════╝ ╚═╝     ╚══════╝╚══════╝   ╚═╝      ╚═╝   ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝
    //                                                                                           
    public class PopSettings : GH_Component
    {
        public PopSettings() : base(
            "PopSettings",
            "PopSettings",
            "Reverts current settings to the state store by the last call to PushSettings().",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("82a53cd1-c33c-4cfa-907c-94288058130e");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_PopSettings;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "PopSettings Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, new ActionPushPop(false));
        }
    }
}
