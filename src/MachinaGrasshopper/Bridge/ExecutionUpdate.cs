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
        private bool[] _updateOutputs;

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


        JavaScriptSerializer ser;

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

            ser = new JavaScriptSerializer();
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("f9b4a612-9b28-4557-ab1a-f8ca020765a8");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Bridge_ExecutionUpdate;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("BridgeMessage", "BM", "The last message received from the Machina Bridge.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("LastTCP", "TCP", "Last known position of the TCP.", GH_ParamAccess.item);
            pManager.AddNumberParameter("LastAxes", "axes", "Last known rotational values of robot axes.", GH_ParamAccess.list);
            pManager.AddNumberParameter("LastExternalAxes", "extax", "Last known values of external axes.", GH_ParamAccess.list);
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
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // This stops the component from assigning nulls 
            // if we don't assign anything to an output.
            DA.DisableGapLogic();

            string msg = null;

            if (!DA.GetData(0, ref msg)) return;

            // Output the values precomputed in the last solution.
            DA.SetData(0, _lastTCP);
            DA.SetDataList(1, _lastAxes);
            DA.SetDataList(2, _lastExternalAxes);

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
                return;
            }

            // Otherwise, search for updated values (only if new messages have been received 
            // by the Listener), and schedule a new solution if they are new.
            bool rescheduleRightAway = false;
            if (true)
            {
                UpdateCurrentValues(msg);

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

        }


        private void UpdateCurrentValues(string msg)
        {
            dynamic json = ser.Deserialize<dynamic>(msg);
            string eType = json["event"];

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


    }
}