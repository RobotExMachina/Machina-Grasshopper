using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MachinaGrasshopper
{
    public class ExportCode : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExportCode class.
        /// </summary>
        public ExportCode()
          : base("ExportCode", "Export",
              "Returns a representation of these Actions written on the device's native language. This is the code you would typically save as a file and manually load on the device's controller.",
              "Machina", "Programs")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "The Robot instance that will export this program", GH_ParamAccess.item);
            pManager.AddGenericParameter("Actions", "A", "A program in the form of a list of Actions", GH_ParamAccess.list);
            pManager.AddBooleanParameter("InLineTargets", "i", "If true, targets will be declared inline with the instruction. Otherwise, the will be declared and used as independent variables", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Comments", "c", "If true, Machina-style comments with code information will be added to the end of the code instructions", GH_ParamAccess.item, true);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Code", "C", "Device-specific program code", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Robot bot = null;
            List<Machina.Action> actions = new List<Machina.Action>();
            bool inline = false;
            bool comments = false;

            if (!DA.GetData(0, ref bot)) return;
            if (!DA.GetDataList(1, actions)) return;
            if (!DA.GetData(2, ref inline)) return;
            if (!DA.GetData(3, ref comments)) return;

            //Machina.Robot bot = (Machina.Robot)botObj;
            //Machina.Action act = (Machina.Action)actObj;

            bot.Mode(Machina.ControlMode.Offline);

            foreach (Machina.Action a in actions)
            {
                bot.Do(a);
            }

            List<string> codeLines = bot.Export(inline, comments);

            DA.SetDataList(0, codeLines);
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
            get { return new Guid("8355fbed-7aa0-4a29-bd9a-c8dca15f2bfb"); }
        }
    }
}