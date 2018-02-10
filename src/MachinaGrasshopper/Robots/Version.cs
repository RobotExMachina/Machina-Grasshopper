using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Robots
{
    //  ██╗   ██╗███████╗██████╗ ███████╗██╗ ██████╗ ███╗   ██╗
    //  ██║   ██║██╔════╝██╔══██╗██╔════╝██║██╔═══██╗████╗  ██║
    //  ██║   ██║█████╗  ██████╔╝███████╗██║██║   ██║██╔██╗ ██║
    //  ╚██╗ ██╔╝██╔══╝  ██╔══██╗╚════██║██║██║   ██║██║╚██╗██║
    //   ╚████╔╝ ███████╗██║  ██║███████║██║╚██████╔╝██║ ╚████║
    //    ╚═══╝  ╚══════╝╚═╝  ╚═╝╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                         
    public class Version : GH_Component
    {
        public Version() : base(
            "Version",
            "Version",
            "Returns version and build numbers of the Machina Core library and Grasshopper API.",
            "Machina",
            "Robots")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("eb166123-3876-44a3-b2d0-d8bb3146a561");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Robots_Version;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Core", "C", "Version and build number of Machina's core library", GH_ParamAccess.item);
            pManager.AddTextParameter("GrasshopperAPI", "GH", "Version of Machina's Grasshopper API", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, Machina.Robot.Version);
            DA.SetData(1, MachinaGrasshopper.MachinaGrasshopperInfo.MachinaGrasshopperAPIVersion());
        }
    }
}
