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
        // Since both outputs should change together, use one flag for both.
        private bool _updateOutputs;
        private int _lastRem, _currentRem;
        private string _lastAction, _currentAction;
        private JavaScriptSerializer ser;

        public ActionCompleted() : base(
            "ActionCompleted",
            "ActionCompleted",
            "Will update every time a new Action has been completed by a robot",
            "Machina",
            "Bridge")
        {
            _updateOutputs = true;
            ser = new JavaScriptSerializer();
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("e3697a88-3441-4185-a1cc-6ccd6bd04050");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Bridge_ActionCompleted;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("BridgeMessage", "BM", "The last message received from the Machina Bridge.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LastAction", "last", "Last Action that was executed by the robot.", GH_ParamAccess.item);
            pManager.AddNumberParameter("RemainingActions", "rem", "How many Actions are left in the robot queue to be executed?", GH_ParamAccess.item);
            //pManager.AddNumberParameter("ticks", "ticks", "", GH_ParamAccess.item);
        }

        protected override void ExpireDownStreamObjects()
        {
            if (_updateOutputs)
            {
                Params.Output[0].ExpireSolution(false);
                Params.Output[1].ExpireSolution(false);
            }
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

            string msg = null;

            if (!DA.GetData(0, ref msg)) return;

            // Output the values precomputed in the last solution.
            DA.SetData(0, _lastAction);
            DA.SetData(1, _lastRem);

            // If on the second solution, stop checking.
            if (_updateOutputs)
            {
                _updateOutputs = false;
                return;
            }

            // Otherwise, search for updated values (only if new messages have been received 
            // by the Listener), and schedule a new solution if they are new.
            bool rescheduleRightAway = false;
            if (true)
            {
                UpdateCurrentValues(msg);

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

                    rescheduleRightAway = true;
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

        /// <summary>
        /// Parse most up-to-date values from parsed message.
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateCurrentValues(string msg)
        {
            dynamic json = ser.Deserialize<dynamic>(msg);
            string eType = json["event"];
            if (eType.Equals("action-completed"))
            {
                _currentRem = json["rem"];
                _currentAction = json["last"];
            }
        }

    }
}