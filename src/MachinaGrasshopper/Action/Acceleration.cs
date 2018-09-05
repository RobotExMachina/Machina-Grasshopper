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

    //   █████╗  ██████╗ ██████╗███████╗██╗     ███████╗██████╗  █████╗ ████████╗██╗ ██████╗ ███╗   ██╗
    //  ██╔══██╗██╔════╝██╔════╝██╔════╝██║     ██╔════╝██╔══██╗██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║
    //  ███████║██║     ██║     █████╗  ██║     █████╗  ██████╔╝███████║   ██║   ██║██║   ██║██╔██╗ ██║
    //  ██╔══██║██║     ██║     ██╔══╝  ██║     ██╔══╝  ██╔══██╗██╔══██║   ██║   ██║██║   ██║██║╚██╗██║
    //  ██║  ██║╚██████╗╚██████╗███████╗███████╗███████╗██║  ██║██║  ██║   ██║   ██║╚██████╔╝██║ ╚████║
    //  ╚═╝  ╚═╝ ╚═════╝ ╚═════╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                                                                 
    public class Acceleration : GH_MutableComponent
    {
        public Acceleration() : base(
            "Acceleration",
            "Acceleration",
            "Changes the acceleration at which new Actions will be executed. This value will be applied to linear motion in mm/s^2, and rotational or angular motion in deg/s^2.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("e34fff2b-ce23-4aa2-902c-0451fd88db87");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Acceleration;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "AccelerationTo", "AccelerationTo", "Increases the acceleration at which new Actions will be executed.");
            mpManager.AddParameter(false, typeof(Param_Number), "Acceleration", "A", "Acceleration value for linear motion in mm/s^2, and rotational or angular motion in deg/s^2", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Acceleration", "Acceleration", "Sets the acceleration at which new Actions will be executed.");
            mpManager.AddParameter(true, typeof(Param_Number), "AccelerationInc", "A", "Acceleration increment for linear motion in mm/s^2, and rotational or angular motion in deg/s^2", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Acceleration Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double acceleration = 0;

            if (!DA.GetData(0, ref acceleration)) return;

            DA.SetData(0, new ActionAcceleration((int)Math.Round(acceleration), this.Relative));
        }
    }
}
