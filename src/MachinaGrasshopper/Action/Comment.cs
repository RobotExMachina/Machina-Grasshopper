using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //   ██████╗ ██████╗ ███╗   ███╗███╗   ███╗███████╗███╗   ██╗████████╗
    //  ██╔════╝██╔═══██╗████╗ ████║████╗ ████║██╔════╝████╗  ██║╚══██╔══╝
    //  ██║     ██║   ██║██╔████╔██║██╔████╔██║█████╗  ██╔██╗ ██║   ██║   
    //  ██║     ██║   ██║██║╚██╔╝██║██║╚██╔╝██║██╔══╝  ██║╚██╗██║   ██║   
    //  ╚██████╗╚██████╔╝██║ ╚═╝ ██║██║ ╚═╝ ██║███████╗██║ ╚████║   ██║   
    //   ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝   ╚═╝   
    //                                                                    
    public class Comment : GH_Component
    {
        public Comment() : base(
            "Comment",
            "Comment",
            "Displays an internal comment in a program compilation. This is useful for internal annotations or reminders, but has no effect on the Robot's behavior.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("a3fc9af6-04ab-49e9-a0fe-d224f4e7e9bf");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Comment;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Comment", "T", "Comment to be displayed on code compilation", GH_ParamAccess.item, "Comment goes here");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Comment Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string comment = "";

            if (!DA.GetData(0, ref comment)) return;

            DA.SetData(0, new ActionComment(comment));
        }
    }
}
