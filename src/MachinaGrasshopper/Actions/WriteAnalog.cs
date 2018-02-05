using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Actions
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
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("ace7ecb7-2a7a-4a39-b181-73d00c412b82");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_WriteAnalog;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("AnalogPinNumber", "N", "Analog pin number", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Value", "V", "Value to send to pin", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "WriteAnalog Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 1;
            double val = 0;

            if (!DA.GetData(0, ref id)) return;
            if (!DA.GetData(1, ref val)) return;

            DA.SetData(0, new ActionIOAnalog(id, val));
        }
    }
}
