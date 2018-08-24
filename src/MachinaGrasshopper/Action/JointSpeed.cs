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
    //       ██╗ ██████╗ ██╗███╗   ██╗████████╗███████╗██████╗ ███████╗███████╗██████╗ 
    //       ██║██╔═══██╗██║████╗  ██║╚══██╔══╝██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗
    //       ██║██║   ██║██║██╔██╗ ██║   ██║   ███████╗██████╔╝█████╗  █████╗  ██║  ██║
    //  ██   ██║██║   ██║██║██║╚██╗██║   ██║   ╚════██║██╔═══╝ ██╔══╝  ██╔══╝  ██║  ██║
    //  ╚█████╔╝╚██████╔╝██║██║ ╚████║   ██║   ███████║██║     ███████╗███████╗██████╔╝
    //   ╚════╝  ╚═════╝ ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝╚═╝     ╚══════╝╚══════╝╚═════╝ 
    //                                                                                 
    public class JointSpeed : GH_MutableComponent
    {
        public JointSpeed() : base(
            "JointSpeed",
            "JointSpeed",
            "Change the maximum joint angular rotation speed value. Movement will be constrained so that the fastest joint rotates below this threshold.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("b39c9746-43f0-4fdb-bab4-37b920690dbc");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_JointSpeed;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "JointSpeedTo", "JointSpeedTo", "Set the maximum joint angular rotation speed value. Movement will be constrained so that the fastest joint rotates below this threshold.");
            mpManager.AddParameter(false, typeof(Param_Number), "JointSpeed", "JS", "Maximum joint angular rotation speed value in deg/s. Decreasing the total to zero or less will reset it back to the robot's default.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "JointSpeed", "JointSpeed", "Increase the maximum joint angular rotation speed value. Movement will be constrained so that the fastest joint rotates below this threshold.");
            mpManager.AddParameter(true, typeof(Param_Number), "JointSpeedInc", "JS", "Maximum joint angular rotation speed increment in deg/s. Decreasing the total to zero or less will reset it back to the robot's default.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "JointSpeed Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double jointSpeed = 0;

            if (!DA.GetData(0, ref jointSpeed)) return;

            DA.SetData(0, new ActionJointSpeed((int)Math.Round(jointSpeed), this.Relative));
        }
    }
}
