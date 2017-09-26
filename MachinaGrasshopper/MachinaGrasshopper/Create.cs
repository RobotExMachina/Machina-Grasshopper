using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MachinaGrasshopper
{
    public class Create : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Create()
          : base("Create Robot", "Robot.Create",
              "Create a new Robot object.",
              "Machina", "Robots")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "A name for this Robot", GH_ParamAccess.item);
            pManager.AddTextParameter("Brand", "B", "Input \"ABB\", \"UR\", \"KUKA\", or \"HUMAN\" (if you only need a human-readable representation of the actions of this Robot...)", GH_ParamAccess.item, "HUMAN");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Your brand new Machina Robot object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            string brand = "";

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref brand)) return;

            DA.SetData(0, new Machina.Robot(name, brand));
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
            get { return new Guid("b33bbc79-be3f-4d92-b7dd-317fc04bf9ee"); }
        }
    }
}