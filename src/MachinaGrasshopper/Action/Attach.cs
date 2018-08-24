using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //   █████╗ ████████╗████████╗ █████╗  ██████╗██╗  ██╗
    //  ██╔══██╗╚══██╔══╝╚══██╔══╝██╔══██╗██╔════╝██║  ██║
    //  ███████║   ██║      ██║   ███████║██║     ███████║
    //  ██╔══██║   ██║      ██║   ██╔══██║██║     ██╔══██║
    //  ██║  ██║   ██║      ██║   ██║  ██║╚██████╗██║  ██║
    //  ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝
    //                                                    
    public class Attach : GH_Component
    {
        public Attach() : base(
            "Attach",
            "Attach",
            "Attach a Tool to the flange of the Robot, replacing whichever tool was on it before. Note that the Tool Center Point (TCP) will be translated/rotated according to the tool change.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("5598bf85-6887-40b4-a29b-efff6af0864f");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Attach;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Tool", "T", "A Tool object to attach to the Robot flange", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Attach Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Tool tool = Machina.Tool.Unset;

            if (!DA.GetData(0, ref tool))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Tool specified, using default \"NoTool\" object");
            }

            DA.SetData(0, new ActionAttach(tool));
        }
    }
}
