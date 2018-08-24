using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Action
{
    //   ██████╗██╗   ██╗███████╗████████╗ ██████╗ ███╗   ███╗ ██████╗ ██████╗ ██████╗ ███████╗
    //  ██╔════╝██║   ██║██╔════╝╚══██╔══╝██╔═══██╗████╗ ████║██╔════╝██╔═══██╗██╔══██╗██╔════╝
    //  ██║     ██║   ██║███████╗   ██║   ██║   ██║██╔████╔██║██║     ██║   ██║██║  ██║█████╗  
    //  ██║     ██║   ██║╚════██║   ██║   ██║   ██║██║╚██╔╝██║██║     ██║   ██║██║  ██║██╔══╝  
    //  ╚██████╗╚██████╔╝███████║   ██║   ╚██████╔╝██║ ╚═╝ ██║╚██████╗╚██████╔╝██████╔╝███████╗
    //   ╚═════╝ ╚═════╝ ╚══════╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝ ╚═════╝ ╚═════╝ ╚═════╝ ╚══════╝
    //                                                                                         
    public class CustomCode : GH_Component
    {
        public CustomCode() : base(
            "CustomCode",
            "CustomCode",
            "Insert a line of custom code directly into a compiled program. This is useful for obscure instructions that are not covered by Machina's API. Note that this Action cannot be checked for validity by Machina, and you are responsible for correct syntax. This Action is non-streamable. ",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("c76eccb6-20fb-45e4-b8f4-fcc88f6cd9fe");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_CustomCode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Statement", "line", "Code in the machine's native language.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("isDeclaration", "dec", "Is this a declaration, like a variable or a workobject? If so, this statement will be placed at the beginning of the program.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "CustomCode Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string line = "";
            bool isDec = false;

            if (!DA.GetData(0, ref line)) return;
            if (!DA.GetData(1, ref isDec)) return;

            DA.SetData(0, new ActionCustomCode(line, isDec));
        }
    }
}
