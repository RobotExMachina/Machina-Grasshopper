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
    //   ██████╗ ██████╗ ███╗   ██╗███╗   ██╗███████╗ ██████╗████████╗
    //  ██╔════╝██╔═══██╗████╗  ██║████╗  ██║██╔════╝██╔════╝╚══██╔══╝
    //  ██║     ██║   ██║██╔██╗ ██║██╔██╗ ██║█████╗  ██║        ██║   
    //  ██║     ██║   ██║██║╚██╗██║██║╚██╗██║██╔══╝  ██║        ██║   
    //  ╚██████╗╚██████╔╝██║ ╚████║██║ ╚████║███████╗╚██████╗   ██║   
    //   ╚═════╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═══╝╚══════╝ ╚═════╝   ╚═╝   
    //                                                                
    public class Connect : GH_Component
    {
        private MachinaBridgeSocket _ms;

        public Connect() : base(
            "Connect",
            "Connect",
            "Attempt to connect to the Machina Bridge.",
            "Machina",
            "Bridge")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("c72d426f-cf9c-4606-8023-f4d928ad88e6");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("URL", "URL", "The URL of the Machina Bridge App. Leave to default unless you know what you are doing ;)", GH_ParamAccess.item, "ws://127.0.0.1:6999/Bridge");
            pManager.AddTextParameter("Name", "Name", "The name of this connecting client", GH_ParamAccess.item, "Grasshopper");
            pManager.AddBooleanParameter("Connect?", "C", "Connect to Machina Bridge App?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Connected?", "C", "Is the connection to the Bridge live?", GH_ParamAccess.item);
            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string url = "";
            string clientName = "";
            bool connect = false;

            if (!DA.GetData(0, ref url)) return;
            if (!DA.GetData(1, ref clientName)) return;
            if (!DA.GetData(2, ref connect)) return;

            url += "?name=" + clientName;

            _ms = new MachinaBridgeSocket(clientName);

            bool connectedResult;
            // @TODO: move all socket management inside the wrapper
            if (connect)
            {
                if (_ms.socket == null || !_ms.socket.IsAlive)
                {
                    _ms.socket = new WebSocket(url);
                    _ms.socket.Connect();
                    _ms.socket.OnMessage += (sender, e) => _ms.Log(e.Data);
                }
                connectedResult = _ms.socket.IsAlive;

                if (!connectedResult)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not connect to Machina Bridge app");
                    return;
                }
            }
            else
            {
                if (_ms.socket != null)
                {
                    _ms.socket.Close();
                    _ms.Flush();
                }
                connectedResult = false;
            }

            DA.SetData(0, connectedResult);
            DA.SetData(1, connectedResult ? _ms : null);
        }
    }
}
