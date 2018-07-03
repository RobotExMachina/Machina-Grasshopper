﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace MachinaGrasshopper.Programs
{
    public class SendToBridge : GH_Component
    {
        //  ███████╗███████╗███╗   ██╗██████╗ ████████╗ ██████╗ ██████╗ ██████╗ ██╗██████╗  ██████╗ ███████╗
        //  ██╔════╝██╔════╝████╗  ██║██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗██╔══██╗██║██╔══██╗██╔════╝ ██╔════╝
        //  ███████╗█████╗  ██╔██╗ ██║██║  ██║   ██║   ██║   ██║██████╔╝██████╔╝██║██║  ██║██║  ███╗█████╗  
        //  ╚════██║██╔══╝  ██║╚██╗██║██║  ██║   ██║   ██║   ██║██╔══██╗██╔══██╗██║██║  ██║██║   ██║██╔══╝  
        //  ███████║███████╗██║ ╚████║██████╔╝   ██║   ╚██████╔╝██████╔╝██║  ██║██║██████╔╝╚██████╔╝███████╗
        //  ╚══════╝╚══════╝╚═╝  ╚═══╝╚═════╝    ╚═╝    ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚═╝╚═════╝  ╚═════╝ ╚══════╝
        //                                                       

        private bool _sentOnce = false;
        private WebSocket _ws;
        private string _url;
                                                       
        public SendToBridge() : base(
            "SendToBridge",
            "SendToBridge",
            "Send a list of Actions to the Machina Bridge App to be streamed to a robot.",
            "Machina",
            "Programs")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("4442d8a9-3b6e-4197-ad58-93caa6c83e3e");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("BridgeURL", "URL", "The URL of the Machina Bridge App.", GH_ParamAccess.item, "ws://127.0.0.1:6999/Bridge");
            pManager.AddBooleanParameter("Connect?", "C", "Connect to Machina Bridge App?", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("Actions", "A", "A program in the form of a list of Actions", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Send", "S", "Send Actions?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Connected?", "C", "Is the connection to the Bridge live?", GH_ParamAccess.item);
            pManager.AddTextParameter("Sent?", "ok", "Correctly sent?", GH_ParamAccess.item);
            pManager.AddTextParameter("Instructions", "I", "Streamed Instructions", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string url = "";
            bool connect = false;
            List<Machina.Action> actions = new List<Machina.Action>();
            bool send = false;

            if (!DA.GetData(0, ref url)) return;
            if (!DA.GetData(1, ref connect)) return;
            if (!DA.GetDataList(2, actions)) return;
            if (!DA.GetData(3, ref send)) return;

            List<string> instructions = new List<string>();

            bool connectedResult;
            if (connect)
            {
                if (_ws == null || !_ws.IsAlive)
                {
                    _ws = new WebSocket(url);
                    _ws.Connect();
                }
                connectedResult = _ws.IsAlive;

                if (!connectedResult)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not connect to Machina Bridge App");
                    return;
                }
            }
            else
            {
                if (_ws != null)
                {
                    _ws.Close();
                }
                connectedResult = _ws.IsAlive;
            }
            DA.SetData(0, connectedResult);
            
            if (send && connectedResult)
            {
                string ins = "";
                    
                foreach (Machina.Action a in actions)
                {
                    // If attaching a tool, send the tool description first.
                    // This is quick and dirty, a result of this component not taking the robot object as an input.
                    // How coud this be improved...? Should tool creation be an action?
                    if (a.type == Machina.ActionType.Attach)
                    {
                        ActionAttach aa = (ActionAttach)a;
                        ins = aa.tool.ToInstruction();
                        instructions.Add(ins);
                        _ws.Send(ins);
                    }

                    ins = a.ToInstruction();
                    instructions.Add(ins);
                    _ws.Send(ins);
                }
                DA.SetData(1, "Sent!");
            }
            else
            {
                DA.SetData(1, "Nothing sent");
            }

            DA.SetDataList(2, instructions);
        }
    }
}
