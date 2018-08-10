using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using MachinaGrasshopper.Utils;

namespace MachinaGrasshopper.Bridge
{
    //  ███████╗██╗  ██╗███████╗ ██████╗██╗   ██╗████████╗██╗ ██████╗ ███╗   ██╗
    //  ██╔════╝╚██╗██╔╝██╔════╝██╔════╝██║   ██║╚══██╔══╝██║██╔═══██╗████╗  ██║
    //  █████╗   ╚███╔╝ █████╗  ██║     ██║   ██║   ██║   ██║██║   ██║██╔██╗ ██║
    //  ██╔══╝   ██╔██╗ ██╔══╝  ██║     ██║   ██║   ██║   ██║██║   ██║██║╚██╗██║
    //  ███████╗██╔╝ ██╗███████╗╚██████╗╚██████╔╝   ██║   ██║╚██████╔╝██║ ╚████║
    //  ╚══════╝╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚═════╝    ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                                          
    //  ██╗   ██╗██████╗ ██████╗  █████╗ ████████╗███████╗                      
    //  ██║   ██║██╔══██╗██╔══██╗██╔══██╗╚══██╔══╝██╔════╝                      
    //  ██║   ██║██████╔╝██║  ██║███████║   ██║   █████╗                        
    //  ██║   ██║██╔═══╝ ██║  ██║██╔══██║   ██║   ██╔══╝                        
    //  ╚██████╔╝██║     ██████╔╝██║  ██║   ██║   ███████╗                      
    //   ╚═════╝ ╚═╝     ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝                      
    //                                                                          


    public class ExecutionUpdate : GH_Component
    {
        private object[] _lastPosObj, _currPosObj;
        private string _lastPosStr, _currPosStr;
        private object[] _lastOriObj, _currOriObj;
        private string _lastOriStr, _currOriStr;
        private Plane _lastTCP;

        private object[] _lastAxesObj, _currAxesObj;
        private string _lastAxesStr, _currAxesStr;
        private List<double?> _lastAxes; 

        private object[] _lastExtaxObj, _currExtaxObj;
        private string _lastExtaxStr, _currExtaxStr;
        private List<double?> _lastExternalAxes;

        private bool[] _updateOutputs;
        private int _ticks = 0;
        private List<string> _lastMessages;
        private int _lastLogCheck;

        public ExecutionUpdate() : base(
            "ExecutionUpdate",
            "ExecutionUpdate",
            "Will update every time there is a change in the state of the robot execution.",
            "Machina",
            "Bridge")
        {
            _updateOutputs = new bool[3];
            for (int i = 0; i < _updateOutputs.Length; i++)
            {
                _updateOutputs[i] = true;
            }

            // Initialize the lists as empty so that they trigger ineuqality with null persed objs?
            _lastAxes = new List<double?>();
            _lastExternalAxes = new List<double?>();

            _lastLogCheck = 0;
        }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("41c6c58c-c5e9-449d-8675-fff9a4351f55");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Autoupdate", "AUTO", "Keep listenting connection alive? The alternative is connecting a timer to this component.", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Interval", "Int", "Refresh interval in milliseconds.", GH_ParamAccess.item, 66);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("lastTCP", "TCP", "Last known position of the TCP.", GH_ParamAccess.item);
            pManager.AddNumberParameter("lastAxes", "axes", "Last known rotational values of robot axes.", GH_ParamAccess.list);
            pManager.AddNumberParameter("lastExternalAxes", "extax", "Last known values of external axes.", GH_ParamAccess.list);
            pManager.AddNumberParameter("ticks", "ticks", "", GH_ParamAccess.item);
        }

        protected override void ExpireDownStreamObjects()
        {
            for (int i = 0; i < _updateOutputs.Length; i++)
            {
                if (_updateOutputs[i])
                {
                    Params.Output[i].ExpireSolution(false);
                }
            }

            // Always expire the ticks...
            Params.Output[_updateOutputs.Length].ExpireSolution(false);
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
            DA.GetData(1, ref autoUpdate);
            DA.GetData(2, ref millis);

            // Some sanity
            if (millis < 10) millis = 10;

            DA.SetData(0, _lastTCP);
            DA.SetDataList(1, _lastAxes);
            DA.SetDataList(2, _lastExternalAxes);
            DA.SetData(3, _ticks++);

            if (ms == null || ms.socket == null || !ms.socket.IsAlive)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
                return;
            }

            // Was any output flagged for an update?
            bool doneWithUpdates = false;
            for (int i = 0; i < _updateOutputs.Length; i++)
            {
                doneWithUpdates |= _updateOutputs[i];
                if (_updateOutputs[i])
                {
                    _updateOutputs[i] = false;
                }
            }

            // If on the second solution, stop checking and go back to autoupdate
            if (doneWithUpdates)
            {
                if (autoUpdate)
                {
                    this.OnPingDocument().ScheduleSolution(millis, doc =>
                    {
                        this.ExpireSolution(false);
                    });
                }

                return;
            }

            // Otherwise, search for updated values (only if new messages have been received 
            // by the Listener), and schedule a new solution if they are new.
            bool rescheduleRightAway = false;
            if (_lastLogCheck != ms.logged)
            {
                UpdateCurrentValues(ms);

                _lastLogCheck = ms.logged;

                // Check and flag each output individually
                bool equals;

                // Compare TCP by its string representation (urgh...)
                equals = string.Equals(_lastPosStr, _currPosStr) && string.Equals(_lastOriStr, _currOriStr);  // if nulls, this will also work
                rescheduleRightAway |= equals;
                if (!equals)
                {
                    _updateOutputs[0] = true;

                    _lastPosObj = _currPosObj;
                    _lastOriObj = _currOriObj;
                    _lastPosStr = _currPosStr;
                    _lastOriStr = _currOriStr;
                    _lastTCP = PlaneFromDoubleObjects(_lastPosObj, _lastOriObj);
                }

                // Compare axes by string representation too
                equals = string.Equals(_lastAxesStr, _currAxesStr);
                rescheduleRightAway |= equals;
                if (!equals)
                {
                    _updateOutputs[1] = true;

                    _lastAxesObj = _currAxesObj;
                    _lastAxesStr = _currAxesStr;

                    _lastAxes = ListFromNullableDoubleObjects(_lastAxesObj);
                }

                // Compare external axes by string representation... OMG I hope no one ever reads this code...
                equals = string.Equals(_lastExtaxStr, _currExtaxStr);
                rescheduleRightAway |= equals;
                if (!equals)
                {
                    _updateOutputs[2] = true;

                    _lastExtaxObj = _currExtaxObj;
                    _lastExtaxStr = _currExtaxStr;

                    _lastExternalAxes = ListFromNullableDoubleObjects(_lastExtaxObj);
                }

            }

            if (rescheduleRightAway)
            {
                this.OnPingDocument().ScheduleSolution(5, doc =>
                {
                    this.ExpireSolution(false);
                });
            }
            else if (autoUpdate)
            {
                this.OnPingDocument().ScheduleSolution(millis, doc =>
                {
                    this.ExpireSolution(false);
                });
            }

        }

        private List<double?> ListFromNullableDoubleObjects(object[] objs)
        {
            List<double?> list = new List<double?>();
            if (objs == null)
            {
                // if received data is null, we want a list of 6 nulls representing it...
                list.Add(null);  // so lazy... XD

                return list;
            }

            foreach (var obj in objs)
            {
                try
                {
                    list.Add(Convert.ToDouble(obj));
                }
                catch
                {
                    list.Add(null);
                }
            }

            return list;
        }


        private Plane PlaneFromDoubleObjects(object[] pos, object[] ori)
        {
            if (pos == null || ori == null)
            {
                return Plane.Unset;
            }

            return new Plane(
                new Point3d(Convert.ToDouble(pos[0]), Convert.ToDouble(pos[1]), Convert.ToDouble(pos[2])),
                new Vector3d(Convert.ToDouble(ori[0]), Convert.ToDouble(ori[1]), Convert.ToDouble(ori[2])),
                new Vector3d(Convert.ToDouble(ori[3]), Convert.ToDouble(ori[4]), Convert.ToDouble(ori[5]))
            );
        }

        private string DoubleObjectArrayToString(object[] objs)
        {
            if (objs == null) return null;

            string str = "";
            for (int i = 0; i < objs.Length; i++)
            {
                str += objs[i]?.ToString() ?? "null";
                if (i < objs.Length - 1)
                {
                    str += ',';
                }
            }
            return str;
        }

        private void UpdateCurrentValues(MachinaBridgeSocket ms)
        {
            _lastMessages = ms.receivedMessages;

            JavaScriptSerializer ser = new JavaScriptSerializer();

            string msg, eType;
            dynamic json;
            for (int i = _lastMessages.Count - 1; i >= 0; i--)
            {
                msg = _lastMessages[i];
                json = ser.Deserialize<dynamic>(msg);
                eType = json["event"];

                // Search once for each event type if not found before
                if (eType.Equals("execution-update"))
                {
                    // Try get pose
                    _currPosObj = json["pos"];
                    _currOriObj = json["ori"];
                    _currPosStr = DoubleObjectArrayToString(_currPosObj);
                    _currOriStr = DoubleObjectArrayToString(_currOriObj);

                    // Try get axes
                    _currAxesObj = json["axes"];
                    _currAxesStr = DoubleObjectArrayToString(_currAxesObj);

                    // Try get external axes
                    _currExtaxObj = json["extax"];
                    _currExtaxStr = DoubleObjectArrayToString(_currExtaxObj);
                    
                    // Stop searching any other events.
                    break;
                }
            }

        }


    }
}