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

namespace MachinaGrasshopper.Actions
{
    //  ██████╗ ██╗   ██╗███████╗██╗  ██╗███████╗███████╗████████╗████████╗██╗███╗   ██╗ ██████╗ ███████╗
    //  ██╔══██╗██║   ██║██╔════╝██║  ██║██╔════╝██╔════╝╚══██╔══╝╚══██╔══╝██║████╗  ██║██╔════╝ ██╔════╝
    //  ██████╔╝██║   ██║███████╗███████║███████╗█████╗     ██║      ██║   ██║██╔██╗ ██║██║  ███╗███████╗
    //  ██╔═══╝ ██║   ██║╚════██║██╔══██║╚════██║██╔══╝     ██║      ██║   ██║██║╚██╗██║██║   ██║╚════██║
    //  ██║     ╚██████╔╝███████║██║  ██║███████║███████╗   ██║      ██║   ██║██║ ╚████║╚██████╔╝███████║
    //  ╚═╝      ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝   ╚═╝      ╚═╝   ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝
    //                                                                                                   
    public class PushSettings : GH_Component
    {
        public PushSettings() : base(
            "PushSettings",
            "PushSettings",
            "Stores current state settings to a buffer, so that temporary changes can be made, and settings can be reverted to the stored state later with PopSettings().",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("f60026ed-e66f-4cba-8592-5d3efc9362bf");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_PushSettings;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "PushSettings Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, new ActionPushPop(true));
        }
    }
}
