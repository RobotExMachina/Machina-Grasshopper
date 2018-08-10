//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.Web.Script.Serialization;

//using Grasshopper.Kernel;
//using Rhino.Geometry;

//using Machina;
//using WebSocketSharp;
//using MachinaGrasshopper.Utils;

//namespace MachinaGrasshopper.Bridge
//{
//    //  ██████╗ ██████╗ ██╗██████╗  ██████╗ ███████╗  
//    //  ██╔══██╗██╔══██╗██║██╔══██╗██╔════╝ ██╔════╝  
//    //  ██████╔╝██████╔╝██║██║  ██║██║  ███╗█████╗    
//    //  ██╔══██╗██╔══██╗██║██║  ██║██║   ██║██╔══╝    
//    //  ██████╔╝██║  ██║██║██████╔╝╚██████╔╝███████╗  
//    //  ╚═════╝ ╚═╝  ╚═╝╚═╝╚═════╝  ╚═════╝ ╚══════╝  
//    //                                                
//    //  ██╗     ██╗███████╗████████╗███████╗███╗   ██╗
//    //  ██║     ██║██╔════╝╚══██╔══╝██╔════╝████╗  ██║
//    //  ██║     ██║███████╗   ██║   █████╗  ██╔██╗ ██║
//    //  ██║     ██║╚════██║   ██║   ██╔══╝  ██║╚██╗██║
//    //  ███████╗██║███████║   ██║   ███████╗██║ ╚████║
//    //  ╚══════╝╚═╝╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═══╝
//    //                                                

//    public class Listen : GH_Component
//    {

//        private List<string> _lastMessages;

//        private int _lastLogCheck = 0;

//        private List<string> _console = new List<string>();

//        public Listen() : base(
//            "Listen",
//            "Listen",
//            "Listen to messages from the Machina Bridge.",
//            "Machina",
//            "Bridge")
//        { }
//        public override GH_Exposure Exposure => GH_Exposure.primary;
//        public override Guid ComponentGuid => new Guid("d5c0770c-3c48-4f19-bf01-2c4ad5b18efb");
//        protected override System.Drawing.Bitmap Icon => null;

//        protected override void RegisterInputParams(GH_InputParamManager pManager)
//        {
//            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
//            pManager.AddBooleanParameter("Autoupdate", "AUTO", "Keep listening while connection alive? The alternative is connecting a timer to this component.", GH_ParamAccess.item, true);
//            pManager.AddIntegerParameter("Interval", "Int", "Refresh interval in milliseconds.", GH_ParamAccess.item, 66);
//        }

//        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
//        {
//            pManager.AddTextParameter("ReceivedMessages", "Msgs", "Last few messages received from the bridge.", GH_ParamAccess.list);
//        }

//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//            MachinaBridgeSocket ms = null;
            
//            bool autoUpdate = true;
//            int millis = 1000;

//            if (!DA.GetData(0, ref ms)) return;
//            if (!DA.GetData(1, ref autoUpdate)) return;
//            if (!DA.GetData(2, ref millis)) return;

//            // Some sanity
//            if (millis < 10) millis = 10;

//            if (ms == null || ms.socket == null || !ms.socket.IsAlive)
//            {
//                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
//                return;
//            }

//            if (_lastLogCheck != ms.logged)
//            {
//                _lastLogCheck = ms.logged;
//            }

//            _lastMessages = ms.receivedMessages;
//            DA.SetDataList(0, _lastMessages);
            
//            if (autoUpdate)
//            {
//                this.OnPingDocument().ScheduleSolution(millis, doc => {
//                    this.ExpireSolution(false);
//                });
//            }
//        }

//    }
//}
