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
    //  ███████╗███████╗███╗   ██╗██████╗           
    //  ██╔════╝██╔════╝████╗  ██║██╔══██╗          
    //  ███████╗█████╗  ██╔██╗ ██║██║  ██║          
    //  ╚════██║██╔══╝  ██║╚██╗██║██║  ██║          
    //  ███████║███████╗██║ ╚████║██████╔╝          
    //  ╚══════╝╚══════╝╚═╝  ╚═══╝╚═════╝           
    //                                              

    public class Send : GH_Component
    {

        public Send() : base(
            "Send",
            "Send",
            "Send a list of Actions to the Bridge.",
            "Machina",
            "Bridge")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("bfe58c17-f4b3-4ace-b31c-e695001f23a8");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actions", "A", "A program in the form of a list of Actions", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Send", "S", "Send Actions?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Sent?", "ok", "Correctly sent?", GH_ParamAccess.item);
            pManager.AddTextParameter("Instructions", "I", "Streamed Instructions", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            MachinaBridgeSocket ms = null;
            List<Machina.Action> actions = new List<Machina.Action>();
            bool send = false;
            
            if (!DA.GetData(0, ref ms)) return;
            if (!DA.GetDataList(1, actions)) return;
            if (!DA.GetData(2, ref send)) return;

            if (ms.socket == null || !ms.socket.IsAlive)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
                return;
            }

            List<string> instructions = new List<string>();
            
            if (send)
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
                        ms.socket.Send(ins);
                    }

                    ins = a.ToInstruction();
                    instructions.Add(ins);
                    ms.socket.Send(ins);
                }

                DA.SetData(0, "Sent!");
            }
            else
            {
                DA.SetData(0, "Nothing sent");
            }

            DA.SetDataList(1, instructions);
        }
    }
}
