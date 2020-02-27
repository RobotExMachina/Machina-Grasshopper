using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaGrasshopper.Program
{
    //  ██████╗ ██╗███████╗██████╗ ██╗      █████╗ ██╗   ██╗██████╗ ██████╗  ██████╗  ██████╗ ██████╗  █████╗ ███╗   ███╗
    //  ██╔══██╗██║██╔════╝██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝██╔══██╗██╔══██╗██╔═══██╗██╔════╝ ██╔══██╗██╔══██╗████╗ ████║
    //  ██║  ██║██║███████╗██████╔╝██║     ███████║ ╚████╔╝ ██████╔╝██████╔╝██║   ██║██║  ███╗██████╔╝███████║██╔████╔██║
    //  ██║  ██║██║╚════██║██╔═══╝ ██║     ██╔══██║  ╚██╔╝  ██╔═══╝ ██╔══██╗██║   ██║██║   ██║██╔══██╗██╔══██║██║╚██╔╝██║
    //  ██████╔╝██║███████║██║     ███████╗██║  ██║   ██║   ██║     ██║  ██║╚██████╔╝╚██████╔╝██║  ██║██║  ██║██║ ╚═╝ ██║
    //  ╚═════╝ ╚═╝╚══════╝╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝     ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝
    //                                                                                                                   
    public class Display : GH_Component
    {
        public Display() : base(
            "DisplayProgram",
            "DisplayProgram",
            "Returns a string representation of a robotic program in its native language.",
            "Machina",
            "Program")
        { }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("2d842a4d-070c-4a06-925a-ea01b32fd610");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Program_Display;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RobotProgram", "P", "A compiled Robot Program.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Code", "C", "Program code in the robot's native language.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Types.Data.RobotProgram program = null;

            if (!DA.GetData(0, ref program)) return;
            
            List<string> code = new List<string>();

            DA.SetDataList(0, program.ToStringList());
        }
    }
}
