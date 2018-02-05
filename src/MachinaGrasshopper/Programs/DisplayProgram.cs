using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaGrasshopper.Programs
{
    //  ██████╗ ██╗███████╗██████╗ ██╗      █████╗ ██╗   ██╗██████╗ ██████╗  ██████╗  ██████╗ ██████╗  █████╗ ███╗   ███╗
    //  ██╔══██╗██║██╔════╝██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝██╔══██╗██╔══██╗██╔═══██╗██╔════╝ ██╔══██╗██╔══██╗████╗ ████║
    //  ██║  ██║██║███████╗██████╔╝██║     ███████║ ╚████╔╝ ██████╔╝██████╔╝██║   ██║██║  ███╗██████╔╝███████║██╔████╔██║
    //  ██║  ██║██║╚════██║██╔═══╝ ██║     ██╔══██║  ╚██╔╝  ██╔═══╝ ██╔══██╗██║   ██║██║   ██║██╔══██╗██╔══██║██║╚██╔╝██║
    //  ██████╔╝██║███████║██║     ███████╗██║  ██║   ██║   ██║     ██║  ██║╚██████╔╝╚██████╔╝██║  ██║██║  ██║██║ ╚═╝ ██║
    //  ╚═════╝ ╚═╝╚══════╝╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝     ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝
    //                                                                                                                   
    public class DisplayProgram : GH_Component
    {
        public DisplayProgram() : base(
            "DisplayProgram",
            "DisplayProgram",
            "Returns a human-readable representation of a list of Actions.",
            "Machina",
            "Programs")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("4f53f8c0-014b-4fd2-b764-71e9e49cf67d");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Programs_DisplayProgram;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Actions", "A", "The list of Actions that conforms a program.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Program", "P", "Human-readable representation of the program", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Action a = null;

            if (!DA.GetData(0, ref a)) return;

            DA.SetData(0, a.ToString());
        }
    }
}
