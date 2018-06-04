using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Robots
{
    //  ██████╗  ██████╗ ██████╗  ██████╗ ████████╗ ██████╗██████╗ ███████╗ █████╗ ████████╗███████╗
    //  ██╔══██╗██╔═══██╗██╔══██╗██╔═══██╗╚══██╔══╝██╔════╝██╔══██╗██╔════╝██╔══██╗╚══██╔══╝██╔════╝
    //  ██████╔╝██║   ██║██████╔╝██║   ██║   ██║   ██║     ██████╔╝█████╗  ███████║   ██║   █████╗  
    //  ██╔══██╗██║   ██║██╔══██╗██║   ██║   ██║   ██║     ██╔══██╗██╔══╝  ██╔══██║   ██║   ██╔══╝  
    //  ██║  ██║╚██████╔╝██████╔╝╚██████╔╝   ██║██╗╚██████╗██║  ██║███████╗██║  ██║   ██║   ███████╗
    //  ╚═╝  ╚═╝ ╚═════╝ ╚═════╝  ╚═════╝    ╚═╝╚═╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝
    //                                                                                              
    public class RobotCreate : GH_Component
    {
        public RobotCreate() : base(
            "Robot.Create",
            "Robot.Create",
            "Creates a new Robot instance.",
            "Machina",
            "Robots")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("b33bbc79-be3f-4d92-b7dd-317fc04bf9ee");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Robots_Create;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "A name for this Robot", GH_ParamAccess.item, "RobotExMachina");
            pManager.AddTextParameter("Brand", "B", "Input \"ABB\", \"UR\", \"KUKA\", \"ZMorph\" or \"HUMAN\" (if you only need a human-readable representation of the actions of this Robot...)", GH_ParamAccess.item, "HUMAN");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Your brand new Machina Robot object.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            string brand = "";

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref brand)) return;

            DA.SetData(0, Robot.Create(name, brand));
        }
    }
}
