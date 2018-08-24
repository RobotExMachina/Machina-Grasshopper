using Grasshopper.Kernel;
using Machina;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaGrasshopper.Programs
{
    //   ██████╗ ██████╗ ███╗   ███╗██████╗ ██╗██╗     ███████╗██████╗ ██████╗  ██████╗  ██████╗ ██████╗  █████╗ ███╗   ███╗
    //  ██╔════╝██╔═══██╗████╗ ████║██╔══██╗██║██║     ██╔════╝██╔══██╗██╔══██╗██╔═══██╗██╔════╝ ██╔══██╗██╔══██╗████╗ ████║
    //  ██║     ██║   ██║██╔████╔██║██████╔╝██║██║     █████╗  ██████╔╝██████╔╝██║   ██║██║  ███╗██████╔╝███████║██╔████╔██║
    //  ██║     ██║   ██║██║╚██╔╝██║██╔═══╝ ██║██║     ██╔══╝  ██╔═══╝ ██╔══██╗██║   ██║██║   ██║██╔══██╗██╔══██║██║╚██╔╝██║
    //  ╚██████╗╚██████╔╝██║ ╚═╝ ██║██║     ██║███████╗███████╗██║     ██║  ██║╚██████╔╝╚██████╔╝██║  ██║██║  ██║██║ ╚═╝ ██║
    //   ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝     ╚═╝╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝
    //                                                                                                                      
    public class CompileProgram : GH_Component
    {
        public CompileProgram() : base(
            "CompileProgram",
            "CompileProgram",
            "Compiles a list of Actions into the device's native language. This is the code you would typically need to load into the device's controller to run the program.",
            "Machina",
            "Programs")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("8355fbed-7aa0-4a29-bd9a-c8dca15f2bfb");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Programs_CompileProgram;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "The Robot instance that will export this program", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actions", "A", "A program in the form of a list of Actions", GH_ParamAccess.list);
            pManager.AddBooleanParameter("InLineTargets", "i", "If true, targets will be declared inline with the instruction. Otherwise, the will be declared and used as independent variables", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Comments", "c", "If true, Machina-style comments with code information will be added to the end of the code instructions", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Code", "C", "Device-specific program code", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Robot bot = null;
            List<Machina.Action> actions = new List<Machina.Action>();
            bool inline = false;
            bool comments = false;

            if (!DA.GetData(0, ref bot)) return;
            if (!DA.GetDataList(1, actions)) return;
            if (!DA.GetData(2, ref inline)) return;
            if (!DA.GetData(3, ref comments)) return;

            // Create a new instance to avoid inheriting robot states between different compilations
            // https://github.com/RobotExMachina/Machina-Grasshopper/issues/3
            Robot compiler = Robot.Create(bot.Name, bot.Brand);

            compiler.ControlMode(ControlType.Offline);
            foreach (Machina.Action a in actions)
            {
                compiler.Do(a);
            }

            List<string> codeLines = compiler.Compile(inline, comments);

            // I forgot why I chose to spit out one single string, but this makes the panel super freaking heavy. Reverting.
            //StringWriter writer = new StringWriter();
            //for (var i = 0; i < codeLines.Count; i++)
            //{
            //    writer.WriteLine(codeLines[i]);
            //}
            //string code = writer.ToString();

            DA.SetDataList(0, codeLines);
        }
    }
}
