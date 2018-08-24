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
            "Changes the TCP acceleration at which future actions will execute.",
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
            mpManager.AddComponentNames(false, "AccelerationTo", "AccelerationTo", "Increases the TCP acceleration at which future actions will execute.");
            mpManager.AddParameter(false, typeof(Param_Number), "Acceleration", "A", "TCP acceleration value in mm/s^2. Decreasing the total to zero or less will reset it back the robot's default.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Acceleration", "Acceleration", "Sets the Acceleration at which future actions will execute.");
            mpManager.AddParameter(true, typeof(Param_Number), "AccelerationInc", "A", "TCP acceleration increment in mm/s^2. Decreasing the total to zero or less will reset it back the robot's default.", GH_ParamAccess.item);
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
