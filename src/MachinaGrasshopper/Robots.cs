using System;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Machina;

namespace MachinaGrasshopper
{

    //  ██████╗  ██████╗ ██████╗  ██████╗ ████████╗███████╗
    //  ██╔══██╗██╔═══██╗██╔══██╗██╔═══██╗╚══██╔══╝██╔════╝
    //  ██████╔╝██║   ██║██████╔╝██║   ██║   ██║   ███████╗
    //  ██╔══██╗██║   ██║██╔══██╗██║   ██║   ██║   ╚════██║
    //  ██║  ██║╚██████╔╝██████╔╝╚██████╔╝   ██║   ███████║
    //  ╚═╝  ╚═╝ ╚═════╝ ╚═════╝  ╚═════╝    ╚═╝   ╚══════╝
    //                                                     
    /// <summary>
    /// Robot-related components go here
    /// </summary>
    


    public class Create : GH_Component
    {
        public Create() : base(
            "Create",
            "Create",
            "Create a new Robot object.", 
            "Machina", 
            "Robots") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("b33bbc79-be3f-4d92-b7dd-317fc04bf9ee");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "T", "A name for this Robot", GH_ParamAccess.item, "MachinaRobot");
            pManager.AddTextParameter("Brand", "B", "Input \"ABB\", \"UR\", \"KUKA\", or \"HUMAN\" (if you only need a human-readable representation of the actions of this Robot...)", GH_ParamAccess.item, "HUMAN");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Your brand new Machina Robot object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            string brand = "";

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref brand)) return;

            DA.SetData(0, new Machina.Robot(name, brand));
        }
    }

    public class Version : GH_Component
    {
        public Version() : base(
            "Version",
            "Version",
            "Checks version and build numbers for the core Machina library.",
            "Machina",
            "Robots")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("eb166123-3876-44a3-b2d0-d8bb3146a561");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }  

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Version", "V", "Version and build number", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, Machina.Robot.Version);
        }
    }

    public class SetIOName : GH_Component
    {
        public SetIOName() : base(
            "SetIOName",
            "SetIOName",
            "Change the name of a Robot's IO pin.",
            "Machina",
            "Robots")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("7c3fe2f8-bc12-4eaa-92c1-27a6729504ac");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Robot to change the io name to", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "T", "New IO name", GH_ParamAccess.item, "Digital_IO_1");
            pManager.AddIntegerParameter("Pin", "N", "Pin number", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Digital", "B", "Is this a digital pin?", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Modified Robot", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Robot bot = null;
            string name = "";
            int pin = 1;
            bool digital = true;

            if (!DA.GetData(0, ref bot)) return;
            if (!DA.GetData(1, ref name)) return;
            if (!DA.GetData(2, ref pin)) return;
            if (!DA.GetData(3, ref digital)) return;

            bot.SetIOName(name, pin, digital);
            DA.SetData(0, bot);
        }
    }


}
