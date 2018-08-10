using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace MachinaGrasshopper.Bridge
{
    //  ███████╗██╗███╗   ██╗ ██████╗ ██╗     ███████╗
    //  ██╔════╝██║████╗  ██║██╔════╝ ██║     ██╔════╝
    //  ███████╗██║██╔██╗ ██║██║  ███╗██║     █████╗  
    //  ╚════██║██║██║╚██╗██║██║   ██║██║     ██╔══╝  
    //  ███████║██║██║ ╚████║╚██████╔╝███████╗███████╗
    //  ╚══════╝╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝
    //                                                
    // From: https://discourse.mcneel.com/t/how-to-trigger-updates-down-only-selected-outputs-of-component/68441
    public sealed class SmartUpdateComponent : GH_Component
    {
        public SmartUpdateComponent()
          : base("Smart Update", "Smupdate", "Only trigger an update if a value changes.", "Machina", "Test")
        {
            UpdateOutput = true;
            PreviousData = "none";
        }
        public override Guid ComponentGuid => new Guid("{60F1F671-78F5-4A23-87EA-CC2BF6B6C296}");
        public override GH_Exposure Exposure => GH_Exposure.hidden;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Input", "In", "Data input.", GH_ParamAccess.tree);
            pManager[0].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Output", "Out", "Data output.", GH_ParamAccess.tree);
            pManager.AddTextParameter("desc", "desc", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Gets or sets whether the immutable output ought to be assigned.
        /// </summary>
        private bool UpdateOutput { get; set; }
        /// <summary>
        /// Gets or sets the cached data from last time.
        /// </summary>
        private string PreviousData { get; set; }

        /// <summary>
        /// Override the behavior of when outputs are expired
        /// </summary>
        protected override void ExpireDownStreamObjects()
        {
            if (UpdateOutput)
            {
                Params.Output[0].ExpireSolution(false);
                Params.Output[1].ExpireSolution(false);
            }
        }

        protected override void SolveInstance(IGH_DataAccess access)
        {
            // This stops the component from assigning nulls 
            // if we don't assign anything to an output.
            access.DisableGapLogic();

            GH_Structure<GH_Integer> tree;

            // Get the current tree and immediately assign it to the output.
            // Better safe than sorry. Since we only selectively expire the
            // output we should still prevent updates, however there is no
            // reason to not have the most recent data always in the output.
            access.GetDataTree(0, out tree);
            access.SetDataTree(0, tree);
            
            string currentData = tree.DataDescription(false, true);
            access.SetData(1, currentData);

            // If we were supposed to update the output (meaning it was expired), 
            // then we know for sure that we don't have to update again.
            if (UpdateOutput)
            {
                UpdateOutput = false;
                PreviousData = currentData;
                return;
            }

            // If the current data differs from the last time,
            // we need to remember that the output needs updating and
            // we need to schedule a new solution so we can actually do this.
            if (!string.Equals(PreviousData, currentData))
            {
                UpdateOutput = true;
                PreviousData = currentData;

                var doc = OnPingDocument();
                doc?.ScheduleSolution(5, Callback);
            }
        }
        private void Callback(GH_Document doc)
        {
            // The logic is all in our expiration method, but we do have 
            // to expire this component.
            if (UpdateOutput)
                ExpireSolution(false);
        }
    }



    //  ███╗   ███╗██╗   ██╗██╗  ████████╗██╗██████╗ ██╗     ███████╗
    //  ████╗ ████║██║   ██║██║  ╚══██╔══╝██║██╔══██╗██║     ██╔════╝
    //  ██╔████╔██║██║   ██║██║     ██║   ██║██████╔╝██║     █████╗  
    //  ██║╚██╔╝██║██║   ██║██║     ██║   ██║██╔═══╝ ██║     ██╔══╝  
    //  ██║ ╚═╝ ██║╚██████╔╝███████╗██║   ██║██║     ███████╗███████╗
    //  ╚═╝     ╚═╝ ╚═════╝ ╚══════╝╚═╝   ╚═╝╚═╝     ╚══════╝╚══════╝
    //                                                               
    // From: https://discourse.mcneel.com/t/how-to-trigger-updates-down-only-selected-outputs-of-component/68441
    public sealed class SmartUpdateComponentMultipleInputs : GH_Component
    {
        /// <summary>
        /// Number of IOs this component will have. 
        /// Must be hardcoded in the component since RegisterInputParams runs before the constructor.
        /// </summary>
        private const int IOCount = 4;

        /// <summary>
        /// Gets or sets whether the immutable output ought to be assigned.
        /// </summary>
        public bool[] UpdateOutput { get; set; }

        /// <summary>
        /// Gets or sets the cached data from last time.
        /// </summary>
        public string[] PreviousData { get; set; }

        public SmartUpdateComponentMultipleInputs()
          : base("Smart Update Multiple Inputs", "SmupdateMI", "Only trigger an update if a value changes.", "Machina", "Test")
        {
            UpdateOutput = new bool[IOCount];
            PreviousData = new string[IOCount];
            for (int i = 0; i < IOCount; i++)
            {
                UpdateOutput[i] = true;
                PreviousData[i] = "nastideplasti";
            }
        }
        public override Guid ComponentGuid => new Guid("3736547e-360a-4fff-9ed5-a05406cc43c1");
        public override GH_Exposure Exposure => GH_Exposure.hidden;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            for (int i = 0; i < IOCount; i++)
            {
                pManager.AddIntegerParameter("in" + i, "in" + i, "Data Input " + i, GH_ParamAccess.tree);
                pManager[i].Optional = true;
            }
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            for (int i = 0; i < IOCount; i++)
            {
                pManager.AddIntegerParameter("out" + i, "out" + i, "Data Output " + i, GH_ParamAccess.tree);
                pManager[i].Optional = true;
            }
        }


        /// <summary>
        /// Override the behavior of when outputs are expired
        /// </summary>
        protected override void ExpireDownStreamObjects()
        {
            for (int i = 0; i < IOCount; i++)
            {
                if (UpdateOutput[i])
                {
                    Params.Output[i].ExpireSolution(false);
                }
            }

        }

        protected override void SolveInstance(IGH_DataAccess access)
        {
            // This stops the component from assigning nulls 
            // if we don't assign anything to an output.
            access.DisableGapLogic();


            // Get the current tree and immediately assign it to the output.
            // Better safe than sorry. Since we only selectively expire the
            // output we should still prevent updates, however there is no
            // reason to not have the most recent data always in the output.
            bool doneWithUpdates = false;
            GH_Structure<GH_Integer> tree;
            string[] currentData = new string[IOCount];
            for (int i = 0; i < IOCount; i++)
            {
                // Check if any input was flagged for an update.
                doneWithUpdates |= UpdateOutput[i];

                access.GetDataTree(i, out tree);
                access.SetDataTree(i, tree);

                currentData[i] = tree.DataDescription(false, true);

                // Unflag inputs that were due for updates. 
                if (UpdateOutput[i])
                {
                    UpdateOutput[i] = false;
                    PreviousData[i] = currentData[i];
                }

            }

            // If any input was flagged for an update, the program is
            // executing the second solution and should stop solving,
            // e.g. if we were supposed to update the output (meaning it 
            // was expired), then we know for sure that we don't have to 
            // update again.
            if (doneWithUpdates)
            {
                return;
            }

            // If the current data differs from the last time,
            // we need to remember that the output needs updating and
            // we need to schedule a new solution so we can actually do this.
            bool scheduleSolution = false;
            for (int i = 0; i < IOCount; i++)
            {
                // Compare int trees by using string description including tree info
                if (!string.Equals(PreviousData[i], currentData[i]))
                {
                    UpdateOutput[i] = true;
                    PreviousData[i] = currentData[i];
                }

                // If flagged any UpdateOutput, we will need to schedule a new solution.
                scheduleSolution |= UpdateOutput[i];
            }

            // Schedule new solution if any ouput needed an update
            if (scheduleSolution)
            {
                var doc = OnPingDocument();
                doc?.ScheduleSolution(5, Callback);
            }
        }

        private void Callback(GH_Document doc)
        {
            // The logic is all in our expiration method, but we do have 
            // to expire this component.
            bool expire = false;
            foreach (var up in UpdateOutput)
            {
                if (up)
                {
                    expire = up;
                    break;
                }
            }

            if (expire)
                ExpireSolution(false);
        }
    }
}