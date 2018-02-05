using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Actions
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
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("a08ed4f1-1913-4f32-8d43-0c98fd1e5bd4");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_WriteDigital;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("DigitalPinNumber", "N", "Digital pin number", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("On", "ON", "Turn on?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "WriteDigital Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 1;
            bool on = false;

            if (!DA.GetData(0, ref id)) return;
            if (!DA.GetData(1, ref on)) return;

            DA.SetData(0, new ActionIODigital(id, on));
        }
    }
}
