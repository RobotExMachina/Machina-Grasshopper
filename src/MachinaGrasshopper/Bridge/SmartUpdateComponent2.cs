using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace MachinaGrasshopper.Bridge
{
    public class SmartUpdateComponent2 : GH_Component
    {
        private bool[] _mustUpdate = new bool[2];
        private string[] _prevData = new string[2];

        /// <summary>
        /// Initializes a new instance of the SmartUpdateComponent2 class.
        /// </summary>
        public SmartUpdateComponent2()
          : base("SmartUpdateComponent2", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("int0", "int0", "", GH_ParamAccess.tree);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("int1", "int1", "", GH_ParamAccess.tree);
            pManager[1].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("outInt0", "outInt0", "", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("outInt1", "outInt1", "", GH_ParamAccess.tree);
        }

        protected override void ExpireDownStreamObjects()
        {
            if (_mustUpdate[0])
            {
                Params.Output[0].ExpireSolution(false);
            }

            if (_mustUpdate[1])
            {
                Params.Output[1].ExpireSolution(false);
            }
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess access)
        {
            access.DisableGapLogic();

            GH_Structure<GH_Integer> intIn0, intIn1;

            access.GetDataTree(0, out intIn0);
            access.GetDataTree(1, out intIn1);

            access.SetDataTree(0, intIn0);
            access.SetDataTree(1, intIn1);

            string currentData0 = intIn0.DataDescription(false, true);
            bool stopCheckingUpdates = _mustUpdate[0] || _mustUpdate[1];
            if (_mustUpdate[0])
            {
                _mustUpdate[0] = false;
                _prevData[0] = currentData0;
                //return;
            }
            string currentData1 = intIn1.DataDescription(false, true);
            if (_mustUpdate[1])
            {
                _mustUpdate[1] = false;
                _prevData[1] = currentData1;
            }
            // If both inputs are done with their scheduled updates, stop scheduling new solutions
            if (stopCheckingUpdates)
            {
                return;
            }

            // Otherwise, let's check if they need updates and schedule a new solution
            if (!string.Equals(_prevData[0], currentData0))
            {
                _mustUpdate[0] = true;
                _prevData[0] = currentData0;

                //var doc = OnPingDocument();
                //doc?.ScheduleSolution(5, Callback);
            }
            if (!string.Equals(_prevData[1], currentData1))
            {
                _mustUpdate[1] = true;
                _prevData[1] = currentData1;
            }

            // If anyone needs update, schedule the solution:
            if (_mustUpdate[0] || _mustUpdate[1])
            {
                var doc = OnPingDocument();
                doc?.ScheduleSolution(5, Callback);
            }
        }

        private void Callback(GH_Document doc)
        {
            // The logic is all in our expiration method, but we do have 
            // to expire this component.
            if (_mustUpdate[0] || _mustUpdate[1])
                ExpireSolution(false);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9429f28f-a7a2-497b-86e5-223f3536007c"); }
        }
    }
}