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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Bridge_Connect;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("URL", "URL", "The URL of the Machina Bridge App. Leave to default unless you know what you are doing ;)", GH_ParamAccess.item, "ws://127.0.0.1:6999/Bridge");
            pManager.AddTextParameter("Name", "Name", "The name of this connecting client", GH_ParamAccess.item, "Grasshopper");
            pManager.AddBooleanParameter("Connect?", "C", "Connect to Machina Bridge App?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Messages", "msg", "What's going on?", GH_ParamAccess.list);
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

            _ms = _ms ?? new MachinaBridgeSocket(clientName);

            bool connectedResult = false;
            List<string> msgs = new List<string>();

            // @TODO: move all socket management inside the wrapper
            if (connect)
            {
                if (_ms.socket == null)
                {
                    _ms.socket = new WebSocket(url);
                }

                if (!_ms.socket.IsAlive)
                {
                    _ms.socket.Connect();
                    _ms.socket.OnMessage += Socket_OnMessage;
                    _ms.socket.OnClose += Socket_OnClose;
                }

                connectedResult = _ms.socket.IsAlive;

                if (connectedResult)
                {
                    msgs.Add("Connected to Machina Bridge");
                } 
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not connect to Machina Bridge app");
                    return;
                }
            }
            else
            {
                if (_ms.socket != null)
                {
                    _ms.socket.Close(CloseStatusCode.Normal, "k thx bye!");
                    _ms.socket = null;
                    _ms.Flush();

                    msgs.Add("Disconnected from the bridge");
                }
                connectedResult = false;
            }

            DA.SetDataList(0, msgs);
            DA.SetData(1, connectedResult ? _ms : null);
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            _ms.Log(e.Data);
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            // Was getting duplicate logging when connecting/disconneting/connecting again...
            // When closing, remove all handlers.
            // Apparently, this is safe (although not thread-safe) even if no handlers were attached: https://stackoverflow.com/a/7065771/1934487
            _ms.socket.OnMessage -= Socket_OnMessage;
            _ms.socket.OnClose -= Socket_OnClose;
        }
    }
}
