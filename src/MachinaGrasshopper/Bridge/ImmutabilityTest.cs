using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MachinaGrasshopper.Bridge
{
    public class ImmutabilityTest : GH_Component
    {
        string immutable = "immutable";
        string output = "";

        /// <summary>
        /// Initializes a new instance of the ImmutabilityTest class.
        /// </summary>
        public ImmutabilityTest()
          : base("ImmutabilityTest", "Nickname",
              "Description",
              "Machina",
              "Bridge")
        {
        }
        
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("mutableIn", "mutableIn", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("mutableOut", "mutableOut", "", GH_ParamAccess.item);
            pManager.AddTextParameter("immutableOut", "immutableOut", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double input = 0;

            if (!DA.GetData(0, ref input)) return;

            DA.SetData(0, input);
            //if (!output.Equals(immutable, StringComparison.Ordinal))
            //{
                DA.SetData(1, immutable);
                //output = immutable;
            //}
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
            get { return new Guid("e8f5d837-a1dc-4331-8fc1-8cf21320b5a7"); }
        }
    }
}