using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //  ██╗    ██╗██████╗ ██╗████████╗███████╗ █████╗ ███╗   ██╗ █████╗ ██╗      ██████╗  ██████╗ 
    //  ██║    ██║██╔══██╗██║╚══██╔══╝██╔════╝██╔══██╗████╗  ██║██╔══██╗██║     ██╔═══██╗██╔════╝ 
    //  ██║ █╗ ██║██████╔╝██║   ██║   █████╗  ███████║██╔██╗ ██║███████║██║     ██║   ██║██║  ███╗
    //  ██║███╗██║██╔══██╗██║   ██║   ██╔══╝  ██╔══██║██║╚██╗██║██╔══██║██║     ██║   ██║██║   ██║
    //  ╚███╔███╔╝██║  ██║██║   ██║   ███████╗██║  ██║██║ ╚████║██║  ██║███████╗╚██████╔╝╚██████╔╝
    //   ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝ ╚═════╝  ╚═════╝ 
    //                                                                                            
    public class WriteAnalog : GH_Component
    {
        public WriteAnalog() : base(
            "WriteAnalog",
            "WriteAnalog",
            "Send a value to analog output.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("ace7ecb7-2a7a-4a39-b181-73d00c412b82");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_WriteAnalog;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("AnalogPin", "N", "Analog pin name or number", GH_ParamAccess.item, "1");
            pManager.AddNumberParameter("Value", "V", "Value to send to pin", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("ToolPin", "t", "Is this pin on the robot's tool?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "WriteAnalog Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "1";
            double val = 0;
            bool tool = false;

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref val)) return;
            if (!DA.GetData(2, ref tool)) return;

            DA.SetData(0, new ActionIOAnalog(name, val, tool));
        }
    }
}
