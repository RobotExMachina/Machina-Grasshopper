using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;

using Machina;
using MachinaGrasshopper.GH_Utils;

namespace MachinaGrasshopper.Action
{
    //  ██████╗ ██████╗ ███████╗ ██████╗██╗███████╗██╗ ██████╗ ███╗   ██╗
    //  ██╔══██╗██╔══██╗██╔════╝██╔════╝██║██╔════╝██║██╔═══██╗████╗  ██║
    //  ██████╔╝██████╔╝█████╗  ██║     ██║███████╗██║██║   ██║██╔██╗ ██║
    //  ██╔═══╝ ██╔══██╗██╔══╝  ██║     ██║╚════██║██║██║   ██║██║╚██╗██║
    //  ██║     ██║  ██║███████╗╚██████╗██║███████║██║╚██████╔╝██║ ╚████║
    //  ╚═╝     ╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //  
    public class Precision : GH_MutableComponent
    {
        public Precision() : base(
            "Precision",
            "Precision",
            "Change the precision at which future actions will execute. Precision is measured as the radius of the smooth interpolation between motion targets. This is usually refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("00aa0408-b8d7-4a47-a09e-ad70f56f62be");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Precision;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "PrecisionTo", "PrecisionTo", "Set the precision at which future actions will execute. Precision is measured as the radius of the smooth interpolation between motion targets. This is usually refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.");
            mpManager.AddParameter(false, typeof(Param_Number), "Radius", "R", "Radius value in mm.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Precision", "Precision", "Increase the precision at which future actions will execute. Precision is measured as the radius of the smooth interpolation between motion targets. This is usually refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.");
            mpManager.AddParameter(true, typeof(Param_Number), "RadiusInc", "RInc", "Radius increment in mm.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Precision Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radiusInc = 0;

            if (!DA.GetData(0, ref radiusInc)) return;

            DA.SetData(0, new ActionPrecision((int)Math.Round(radiusInc), this.Relative));
        }
    }
}
