using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MachinaGrasshopper.Program
{
    //  ███████╗ █████╗ ██╗   ██╗███████╗██████╗ ██████╗  ██████╗  ██████╗ ██████╗  █████╗ ███╗   ███╗
    //  ██╔════╝██╔══██╗██║   ██║██╔════╝██╔══██╗██╔══██╗██╔═══██╗██╔════╝ ██╔══██╗██╔══██╗████╗ ████║
    //  ███████╗███████║██║   ██║█████╗  ██████╔╝██████╔╝██║   ██║██║  ███╗██████╔╝███████║██╔████╔██║
    //  ╚════██║██╔══██║╚██╗ ██╔╝██╔══╝  ██╔═══╝ ██╔══██╗██║   ██║██║   ██║██╔══██╗██╔══██║██║╚██╔╝██║
    //  ███████║██║  ██║ ╚████╔╝ ███████╗██║     ██║  ██║╚██████╔╝╚██████╔╝██║  ██║██║  ██║██║ ╚═╝ ██║
    //  ╚══════╝╚═╝  ╚═╝  ╚═══╝  ╚══════╝╚═╝     ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝
    //                                                                                                
    public class Save : GH_Component
    {
        public Save() : base(
            "SaveProgram", 
            "SaveProgram",
            "Saves the Robot Program files to a destination on your system.",
            "Machina", 
            "Program")
        { }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("7a2d5912-5dbd-4be1-a94c-64f1d9498384");
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "The Robot instance that compiled this program", GH_ParamAccess.item);
            pManager.AddGenericParameter("RobotProgram", "P", "A compiled Robot Program.", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "F", "Path to a folder where to save this program.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "M", "Acknowledgement message.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Robot bot = null;
            Machina.Types.Data.RobotProgram program = null;
            string folderPath = "";

            if (!DA.GetData(0, ref bot)) return;
            if (!DA.GetData(1, ref program)) return;
            if (!DA.GetData(2, ref folderPath)) return;

            bool success = bot.SaveProgram(program, folderPath);

            DA.SetData(0, success ?
                $"Robot program saved to {folderPath}" :
                $"Something went wrong saving the program, please check the Logger");
        }
    }
}