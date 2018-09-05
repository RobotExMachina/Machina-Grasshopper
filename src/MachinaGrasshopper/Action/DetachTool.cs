using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //  ██████╗ ███████╗████████╗ █████╗  ██████╗██╗  ██╗
    //  ██╔══██╗██╔════╝╚══██╔══╝██╔══██╗██╔════╝██║  ██║
    //  ██║  ██║█████╗     ██║   ███████║██║     ███████║
    //  ██║  ██║██╔══╝     ██║   ██╔══██║██║     ██╔══██║
    //  ██████╔╝███████╗   ██║   ██║  ██║╚██████╗██║  ██║
    //  ╚═════╝ ╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝
    //                                                   
    public class DetachTool : GH_Component
    {
        public DetachTool() : base(
            "DetachTool",
            "DetachTool",
            "Detach any Tool currently attached to the Robot. Note that the Tool Center Point (TCP) will now be transformed to the Robot's flange.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("f3195b55-742a-429f-bf66-94fce5497bc9");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_DetachTool;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }  // no info needed

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "DetachTool Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, new ActionDetachTool());
        }
    }
}
