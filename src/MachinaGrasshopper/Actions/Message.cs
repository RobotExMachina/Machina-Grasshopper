using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Actions
{
    //  ███╗   ███╗███████╗███████╗███████╗ █████╗  ██████╗ ███████╗
    //  ████╗ ████║██╔════╝██╔════╝██╔════╝██╔══██╗██╔════╝ ██╔════╝
    //  ██╔████╔██║█████╗  ███████╗███████╗███████║██║  ███╗█████╗  
    //  ██║╚██╔╝██║██╔══╝  ╚════██║╚════██║██╔══██║██║   ██║██╔══╝  
    //  ██║ ╚═╝ ██║███████╗███████║███████║██║  ██║╚██████╔╝███████╗
    //  ╚═╝     ╚═╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝
    //                                                              
    public class Message : GH_Component
    {
        public Message() : base(
            "Message",
            "Message",
            "Displays a text message on the device. This will depend on the device's configuration, e.g. ABB robots it will display it on the FlexPendant's log window.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("2675e57a-5b6f-441a-9f94-69bb155b7b59");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Message;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "T", "Text message to display", GH_ParamAccess.item, "Hello Machina!");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Message Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string msg = "";

            if (!DA.GetData(0, ref msg)) return;

            DA.SetData(0, new ActionMessage(msg));
        }
    }
}
