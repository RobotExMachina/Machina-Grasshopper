using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Script.Serialization;

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

        private bool _updateOut;
        private string _lastMsg;

        public Listen() : base(
            "Listen",
            "Listen",
            "Listen to messages from the Machina Bridge.",
            "Machina",
            "Bridge")
        {
            _updateOut = true;
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("8281b3ed-8d72-4e0a-8d11-efecd3b49954");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Bridge_Listen;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Autoupdate", "AUTO", "Keep listening while connection alive? The alternative is connecting a timer to this component.", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Interval", "Int", "Refresh interval in milliseconds.", GH_ParamAccess.item, 66);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("BridgeMessage", "Msg", "Last messags received from the bridge. Will only update once per received message.", GH_ParamAccess.list);
        }

        protected override void ExpireDownStreamObjects()
        {
            // Only expire the output if it needs an update
            if (_updateOut)
            {
                Params.Output[0].ExpireSolution(false);
            }
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // This stops the component from assigning nulls 
            // if we don't assign anything to an output.
            DA.DisableGapLogic();

            MachinaBridgeSocket ms = null;
            bool autoUpdate = true;
            int millis = 1000;

            if (!DA.GetData(0, ref ms)) return;
            if (!DA.GetData(1, ref autoUpdate)) return;
            if (!DA.GetData(2, ref millis)) return;
            
            // Some sanity
            if (millis < 10) millis = 10;
            if (ms == null || ms.socket == null || !ms.socket.IsAlive)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
                return;
            }

            // Output the last received message from the last cycle
            DA.SetData(0, _lastMsg);

            // Stop triggering expiration updates if the buffer is empty
            int size = ms.BufferSize();
            if (_updateOut && size == 0)
            {
                _updateOut = false;

                // And go back to regular autoupdate
                if (autoUpdate)
                {
                    this.OnPingDocument().ScheduleSolution(millis, doc => {
                        this.ExpireSolution(false);
                    });
                }

                return;
            }
            
            // If there are messagges logged by the MS, trigger a chain of expiration updates
            if (size > 0)
            {
                _updateOut = true;
                _lastMsg = ms.FetchFirst(true);

                // Schedule a new solution right away
                this.OnPingDocument().ScheduleSolution(5, doc => {
                    this.ExpireSolution(false);
                });

                return;
            }

            // Otherwise, back to reguar autoupdate
            if (autoUpdate)
            {
                this.OnPingDocument().ScheduleSolution(millis, doc =>
                {
                    this.ExpireSolution(false);
                });
            }
        }

    }
}
