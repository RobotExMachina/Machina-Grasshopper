using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //   █████╗ ████████╗████████╗ █████╗  ██████╗██╗  ██╗
    //  ██╔══██╗╚══██╔══╝╚══██╔══╝██╔══██╗██╔════╝██║  ██║
    //  ███████║   ██║      ██║   ███████║██║     ███████║
    //  ██╔══██║   ██║      ██║   ██╔══██║██║     ██╔══██║
    //  ██║  ██║   ██║      ██║   ██║  ██║╚██████╗██║  ██║
    //  ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝
    //                                                    
    public class AttachTool : GH_Component
    {
        public AttachTool() : base(
            "AttachTool",
            "AttachTool",
            "Attach a Tool to the flange of the Robot, replacing whichever tool was on it before. Note that the Tool Center Point (TCP) will be translated/rotated according to the tool change.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("5598bf85-6887-40b4-a29b-efff6af0864f");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_AttachTool;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ToolName", "T", "The name of the Tool to attach to the Robot flange. Please note it should have been previously defined with \"DefineTool\".", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "AttachTool Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string toolName = ""; 

            if (!DA.GetData(0, ref toolName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No tool name specified...");
                return;
            }

            DA.SetData(0, new ActionAttachTool(toolName));
        }
    }
}
