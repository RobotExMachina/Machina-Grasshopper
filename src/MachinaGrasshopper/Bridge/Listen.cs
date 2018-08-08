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

        private List<string> _lastMessages;
        private Plane _lastTCP;
        private List<double?> _lastAxes;
        private string _lastAction;
        private int _actionsPending;

        private int _lastLogCheck = 0;

        private List<string> _console = new List<string>();

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
            pManager.AddIntegerParameter("Interval", "Int", "Refresh interval in milliseconds.", GH_ParamAccess.item, 66);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("lastMessages", "Msg", "Last few messages received from the bridge.", GH_ParamAccess.list);

            pManager.AddPlaneParameter("lastTCP", "TCP", "Last known position of the TCP.", GH_ParamAccess.item);
            pManager.AddNumberParameter("lastAxes", "Q", "Last known rotational values of robot axes.", GH_ParamAccess.list);
            pManager.AddTextParameter("lastAction", "A", "Last Action that was executed by the robot.", GH_ParamAccess.item);
            pManager.AddNumberParameter("remainingActions", "r", "How many Actions are left in the robot queue to be executed?", GH_ParamAccess.item);

            pManager.AddTextParameter("log", "log", "console", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            MachinaBridgeSocket ms = null;
            
            bool autoUpdate = true;
            int millis = 1000;

            if (!DA.GetData(0, ref ms)) return;
            if (!DA.GetData(1, ref autoUpdate)) return;
            if (!DA.GetData(2, ref millis)) return;

            if (ms == null || ms.socket == null || !ms.socket.IsAlive)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
                if (autoUpdate) this.ExpireSolution(true);
                return;
            }

            if (_lastLogCheck != ms.logged)
            {
                UpdateOutputs(ms);
                _lastLogCheck = ms.logged;
            }

            
            if (autoUpdate)
            {
                this.OnPingDocument().ScheduleSolution(millis, doc => {
                    this.ExpireSolution(false);
                });
            }

            DA.SetDataList(0, _lastMessages);
            DA.SetData(1, _lastTCP);
            DA.SetDataList(2, _lastAxes);
            DA.SetData(3, _lastAction);
            DA.SetData(4, _actionsPending);
            DA.SetDataList(5, _console);
        }


        private void UpdateOutputs(MachinaBridgeSocket ms)
        {
            _lastMessages = ms.receivedMessages;

            JavaScriptSerializer ser = new JavaScriptSerializer();

            bool foundEU = false, 
                 foundAC = false;
            string msg, eType;
            dynamic json;
            _lastAxes = new List<double?>();
            for (int i = _lastMessages.Count - 1; i >= 0; i--)
            {
                msg = _lastMessages[i];
                json = ser.Deserialize<dynamic>(msg);
                eType = json["event"];

                // Search once for each event type if not found before
                if (!foundEU && eType.Equals("execution-update", StringComparison.Ordinal))
                {
                    // Try get pose
                    try
                    {
                        object[] pos = json["pos"];
                        object[] ori = json["ori"];
                        if (pos != null && ori != null)
                        {
                            _lastTCP = new Plane(
                                new Point3d(Convert.ToDouble(pos[0]), Convert.ToDouble(pos[1]), Convert.ToDouble(pos[2])),
                                new Vector3d(Convert.ToDouble(ori[0]), Convert.ToDouble(ori[1]), Convert.ToDouble(ori[2])),
                                new Vector3d(Convert.ToDouble(ori[3]), Convert.ToDouble(ori[4]), Convert.ToDouble(ori[5]))
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        //_console.Add("Exception: " + ex.ToString());
                        _lastTCP = Plane.Unset;
                    }

                    // Try get axes
                    try
                    {
                        object[] axes = json["axes"];
                        if (axes != null)
                        {
                            foreach (var obj in axes)
                            {
                                _lastAxes.Add(Convert.ToDouble(obj));
                            }
                        }
                    }
                    catch
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            _lastAxes.Add(null);
                        }

                    }

                    foundEU = true;
                }


                else if (!foundAC && eType.Equals("action-completed", StringComparison.Ordinal))
                {
                    _actionsPending = json["rem"];
                    _lastAction = json["last"];

                    foundAC = true;
                }



                if (foundEU && foundAC)
                {
                    break;  // stop searching
                }
            }

        }
    }
}
