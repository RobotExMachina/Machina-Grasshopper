using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //  ██╗    ██╗██████╗ ██╗████████╗███████╗██████╗ ██╗ ██████╗ ██╗████████╗ █████╗ ██╗     
    //  ██║    ██║██╔══██╗██║╚══██╔══╝██╔════╝██╔══██╗██║██╔════╝ ██║╚══██╔══╝██╔══██╗██║     
    //  ██║ █╗ ██║██████╔╝██║   ██║   █████╗  ██║  ██║██║██║  ███╗██║   ██║   ███████║██║     
    //  ██║███╗██║██╔══██╗██║   ██║   ██╔══╝  ██║  ██║██║██║   ██║██║   ██║   ██╔══██║██║     
    //  ╚███╔███╔╝██║  ██║██║   ██║   ███████╗██████╔╝██║╚██████╔╝██║   ██║   ██║  ██║███████╗
    //   ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝   ╚═╝   ╚══════╝╚═════╝ ╚═╝ ╚═════╝ ╚═╝   ╚═╝   ╚═╝  ╚═╝╚══════╝
    //                                                                                        
    public class WriteDigital : GH_Component
    {
        public WriteDigital() : base(
            "WriteDigital",
            "WriteDigital",
            "Activate/deactivate digital output.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("a08ed4f1-1913-4f32-8d43-0c98fd1e5bd4");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_WriteDigital;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("DigitalPin", "N", "Digital pin name or number", GH_ParamAccess.item, "1");
            pManager.AddBooleanParameter("On", "ON", "Turn on?", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("ToolPin", "t", "Is this pin on the robot's tool?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "WriteDigital Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "1";
            bool on = false;
            bool tool = false;

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref on)) return;
            if (!DA.GetData(2, ref tool)) return;

            DA.SetData(0, new ActionIODigital(name, on, tool));
        }
    }
}
