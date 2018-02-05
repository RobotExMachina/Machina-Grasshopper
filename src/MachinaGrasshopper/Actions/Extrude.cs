using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using MachinaGrasshopper.GH_Utils;
using Machina;

namespace MachinaGrasshopper.Actions
{
    //  ███████╗██╗  ██╗████████╗██████╗ ██╗   ██╗██████╗ ███████╗
    //  ██╔════╝╚██╗██╔╝╚══██╔══╝██╔══██╗██║   ██║██╔══██╗██╔════╝
    //  █████╗   ╚███╔╝    ██║   ██████╔╝██║   ██║██║  ██║█████╗  
    //  ██╔══╝   ██╔██╗    ██║   ██╔══██╗██║   ██║██║  ██║██╔══╝  
    //  ███████╗██╔╝ ██╗   ██║   ██║  ██║╚██████╔╝██████╔╝███████╗
    //  ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═════╝ ╚══════╝
    //                                                            
    public class Extrude : GH_Component
    {
        public Extrude() : base(
            "Extrude",
            "Extrude",
            "Turns extrusion in 3D printers on/off.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("290f7093-e14a-4ca5-b3e5-9e4648f82d53");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Extrude;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("On", "On", "True/false for on/off", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Extrude Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool extrude = false;

            if (!DA.GetData(0, ref extrude)) return;

            DA.SetData(0, new ActionExtrusion(extrude));
        }
    }

}
