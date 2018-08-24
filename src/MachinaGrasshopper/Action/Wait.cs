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
    //  ██╗    ██╗ █████╗ ██╗████████╗
    //  ██║    ██║██╔══██╗██║╚══██╔══╝
    //  ██║ █╗ ██║███████║██║   ██║   
    //  ██║███╗██║██╔══██║██║   ██║   
    //  ╚███╔███╔╝██║  ██║██║   ██║   
    //   ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝   ╚═╝   
    //                                
    public class Wait : GH_Component
    {
        public Wait() : base(
            "Wait",
            "Wait",
            "Pause program execution for a specified amount of time.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("4ec5c686-0ca9-4b60-a99e-8eaf4fe594ac");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Wait;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Time", "T", "Pause time in milliseconds", GH_ParamAccess.item, 1000);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Wait Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double t = 0;

            if (!DA.GetData(0, ref t)) return;

            DA.SetData(0, new ActionWait((long)Math.Round(t)));
        }
    }
}
