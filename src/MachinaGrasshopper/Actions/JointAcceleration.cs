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

namespace MachinaGrasshopper.Actions
{

    //       ██╗ ██████╗ ██╗███╗   ██╗████████╗ █████╗  ██████╗ ██████╗███████╗██╗     ███████╗██████╗  █████╗ ████████╗██╗ ██████╗ ███╗   ██╗
    //       ██║██╔═══██╗██║████╗  ██║╚══██╔══╝██╔══██╗██╔════╝██╔════╝██╔════╝██║     ██╔════╝██╔══██╗██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║
    //       ██║██║   ██║██║██╔██╗ ██║   ██║   ███████║██║     ██║     █████╗  ██║     █████╗  ██████╔╝███████║   ██║   ██║██║   ██║██╔██╗ ██║
    //  ██   ██║██║   ██║██║██║╚██╗██║   ██║   ██╔══██║██║     ██║     ██╔══╝  ██║     ██╔══╝  ██╔══██╗██╔══██║   ██║   ██║██║   ██║██║╚██╗██║
    //  ╚█████╔╝╚██████╔╝██║██║ ╚████║   ██║   ██║  ██║╚██████╗╚██████╗███████╗███████╗███████╗██║  ██║██║  ██║   ██║   ██║╚██████╔╝██║ ╚████║
    //   ╚════╝  ╚═════╝ ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═════╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                                                                                                        
    public class JointAcceleration : GH_MutableComponent
    {
        public JointAcceleration() : base(
            "JointAcceleration",
            "JointAcceleration",
            "Change the maximum joint angular rotation acceleration value. Movement will be constrained so that the fastest joint accelerates below this threshold.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("eb2e55d0-2cc3-46b5-86e6-d5beaf9cf3c8");
        protected override System.Drawing.Bitmap Icon => null;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "JointAccelerationTo", "JointAccelerationTo", "Set the maximum joint angular rotation acceleration value. Movement will be constrained so that the fastest joint accelerates below this threshold.");
            mpManager.AddParameter(false, typeof(Param_Number), "JointAccelerationInc", "JA", "Maximum joint angular rotation acceleration increment in deg/s^2. Decreasing the total to zero or less will reset it back to the robot's default.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "JointAcceleration", "JointAcceleration", "Increase the maximum joint angular rotation acceleration value. Movement will be constrained so that the fastest joint accelerates below this threshold.");
            mpManager.AddParameter(true, typeof(Param_Number), "JointAccelerationInc", "JA", "Maximum joint angular rotation acceleration increment in deg/s^2. Decreasing the total to zero or less will reset it back to the robot's default.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "JointAcceleration Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double jointAcceleration = 0;

            if (!DA.GetData(0, ref jointAcceleration)) return;

            DA.SetData(0, new ActionJointAcceleration((int)Math.Round(jointAcceleration), this.Relative));
        }
    }
}
