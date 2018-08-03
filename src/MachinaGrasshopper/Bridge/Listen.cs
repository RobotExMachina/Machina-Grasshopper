using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Machina;
using WebSocketSharp;
using MachinaGrasshopper.Utils;

namespace MachinaGrasshopper.Bridge
{
    //  ██████╗ ██████╗ ██╗██████╗  ██████╗ ███████╗  
    //  ██╔══██╗██╔══██╗██║██╔══██╗██╔════╝ ██╔════╝  
    //  ██████╔╝██████╔╝██║██║  ██║██║  ███╗█████╗    
    //  ██╔══██╗██╔══██╗██║██║  ██║██║   ██║██╔══╝    
    //  ██████╔╝██║  ██║██║██████╔╝╚██████╔╝███████╗  
    //  ╚═════╝ ╚═╝  ╚═╝╚═╝╚═════╝  ╚═════╝ ╚══════╝  
    //                                                
    //  ██╗     ██╗███████╗████████╗███████╗███╗   ██╗
    //  ██║     ██║██╔════╝╚══██╔══╝██╔════╝████╗  ██║
    //  ██║     ██║███████╗   ██║   █████╗  ██╔██╗ ██║
    //  ██║     ██║╚════██║   ██║   ██╔══╝  ██║╚██╗██║
    //  ███████╗██║███████║   ██║   ███████╗██║ ╚████║
    //  ╚══════╝╚═╝╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═══╝
    //                                                

    public class Listen : GH_Component
    {

        public Listen() : base(
            "Listen",
            "Listen",
            "Listen to messages from the Machina Bridge.",
            "Machina",
            "Bridge")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("d5c0770c-3c48-4f19-bf01-2c4ad5b18efb");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Autoupdate", "AUTO", "Keep listenting connection alive? The alternative is connecting a timer to this component.", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Interval", "Int", "Listen interval in milliseconds.", GH_ParamAccess.item, 66);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LastMessage", "Msg", "Last messages received.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            MachinaBridgeSocket ms = null;
            
            bool autoUpdate = true;
            int millis = 1000;

            if (!DA.GetData(0, ref ms)) return;
            if (!DA.GetData(1, ref autoUpdate)) return;
            if (!DA.GetData(2, ref millis)) return;

            if (ms.socket == null || !ms.socket.IsAlive)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
                if (autoUpdate) this.ExpireSolution(true);
                return;
            }

            List<string> messages = ms.receivedMessage;

            if (autoUpdate)
            {
                this.OnPingDocument().ScheduleSolution(millis, doc => {
                    this.ExpireSolution(false);
                });
            }

            DA.SetDataList(0, messages);
        }
    }
}
