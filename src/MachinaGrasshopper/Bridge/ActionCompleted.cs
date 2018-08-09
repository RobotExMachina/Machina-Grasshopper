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
    //   █████╗  ██████╗████████╗██╗ ██████╗ ███╗   ██╗                              
    //  ██╔══██╗██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║                              
    //  ███████║██║        ██║   ██║██║   ██║██╔██╗ ██║                              
    //  ██╔══██║██║        ██║   ██║██║   ██║██║╚██╗██║                              
    //  ██║  ██║╚██████╗   ██║   ██║╚██████╔╝██║ ╚████║                              
    //  ╚═╝  ╚═╝ ╚═════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝                              
    //                                                                               
    //   ██████╗ ██████╗ ███╗   ███╗██████╗ ██╗     ███████╗████████╗███████╗██████╗ 
    //  ██╔════╝██╔═══██╗████╗ ████║██╔══██╗██║     ██╔════╝╚══██╔══╝██╔════╝██╔══██╗
    //  ██║     ██║   ██║██╔████╔██║██████╔╝██║     █████╗     ██║   █████╗  ██║  ██║
    //  ██║     ██║   ██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝     ██║   ██╔══╝  ██║  ██║
    //  ╚██████╗╚██████╔╝██║ ╚═╝ ██║██║     ███████╗███████╗   ██║   ███████╗██████╔╝
    //   ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝   ╚═╝   ╚══════╝╚═════╝ 
    //                                                                               

    public class ActionCompleted : GH_Component
    {

        private bool _updateOutputs;
        private int _lastRem, _currentRem;
        private string _lastAction, _currentAction;
        private int _ticks = 0;

        private List<string> _lastMessages;
        private int _lastLogCheck;

        public ActionCompleted() : base(
            "ActionCompleted", 
            "ActionCompleted",
            "Will update every time a new Action has been completed by a robot",
            "Machina", 
            "Bridge")
        {
            _updateOutputs = true;
            //_lastAction = "foobar";
            //_lastRem = int.MaxValue;
            _lastLogCheck = 0;
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("1758e345-cd99-4ff7-b19c-72f3528f10d5");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bridge", "MB", "The (websocket) object managing connection to the Machina Bridge", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Autoupdate", "AUTO", "Keep listenting while connection alive?", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Interval", "Int", "Refresh interval in milliseconds.", GH_ParamAccess.item, 66);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("lastAction", "last", "Last Action that was executed by the robot.", GH_ParamAccess.item);
            pManager.AddNumberParameter("remainingActions", "rem", "How many Actions are left in the robot queue to be executed?", GH_ParamAccess.item);
            pManager.AddNumberParameter("ticks", "ticks", "", GH_ParamAccess.item);
        }

        protected override void ExpireDownStreamObjects()
        {
            if (_updateOutputs)
            {
                Params.Output[0].ExpireSolution(false);
                Params.Output[1].ExpireSolution(false);
            }

            // Always expire the ticks...
            Params.Output[2].ExpireSolution(false);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
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

            DA.SetData(0, _lastAction);
            DA.SetData(1, _lastRem);
            DA.SetData(2, _ticks++);

            if (ms == null || ms.socket == null || !ms.socket.IsAlive)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not valid Bridge connection.");
                //if (autoUpdate)
                //{
                //    this.OnPingDocument().ScheduleSolution(millis, doc => {
                //        this.ExpireSolution(false);
                //    });
                //}
                return;
            }

            // If on the second solution, stop checking.
            if (_updateOutputs)
            {
                _updateOutputs = false;
                _lastRem = _currentRem;
                _lastAction = _currentAction;

                if (autoUpdate)
                {
                    this.OnPingDocument().ScheduleSolution(millis, doc => {
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
                UpdateOutputs(ms);
                _lastLogCheck = ms.logged;

                // We may be receiving the same action multiple times (like the user is sending
                // "Move(5, 0, 0);" one hundred times, or we may receive sero rem actions multiple
                // times if user is sending them on buffer empty... So if any value is different, 
                // we update everything. 
                // @TODO: both situations may be happening simultaneously, so perhaps the events 
                // should come with an id to make sure they are new...?

                if (_lastRem != _currentRem || !string.Equals(_lastAction, _currentAction))
                {
                    _updateOutputs = true;
                    _lastRem = _currentRem;
                    _lastAction = _currentAction;

                    //// Schedule a new solution right away filtered by _updateOutputs necessity
                    //var doc = OnPingDocument();
                    //// doc?.ScheduleSolution(5, Callback);
                    //doc?.ScheduleSolution(5, document => this.ExpireSolution(false));
                    rescheduleRightAway = true;
                }
            }

            if (rescheduleRightAway)
            {
                this.OnPingDocument().ScheduleSolution(millis, doc => {
                    this.ExpireSolution(false);
                });
            }
            else if (autoUpdate)
            {
                this.OnPingDocument().ScheduleSolution(millis, doc => {
                    this.ExpireSolution(false);
                });
            }
        }

        private void Callback(GH_Document doc)
        {
            // The logic is all in our expiration method, but we do have 
            // to expire this component.
            if (_updateOutputs)
                ExpireSolution(false);
        }

        private void UpdateOutputs(MachinaBridgeSocket ms)
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
                
                if (eType.Equals("action-completed"))
                {
                    _currentRem = json["rem"];
                    _currentAction = json["last"];
                    break;
                }
            }

        }

    }
}