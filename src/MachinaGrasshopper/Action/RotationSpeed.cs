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

    //  ██████╗  ██████╗ ████████╗ █████╗ ████████╗██╗ ██████╗ ███╗   ██╗███████╗██████╗ ███████╗███████╗██████╗ 
    //  ██╔══██╗██╔═══██╗╚══██╔══╝██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗
    //  ██████╔╝██║   ██║   ██║   ███████║   ██║   ██║██║   ██║██╔██╗ ██║███████╗██████╔╝█████╗  █████╗  ██║  ██║
    //  ██╔══██╗██║   ██║   ██║   ██╔══██║   ██║   ██║██║   ██║██║╚██╗██║╚════██║██╔═══╝ ██╔══╝  ██╔══╝  ██║  ██║
    //  ██║  ██║╚██████╔╝   ██║   ██║  ██║   ██║   ██║╚██████╔╝██║ ╚████║███████║██║     ███████╗███████╗██████╔╝
    //  ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝╚═╝     ╚══════╝╚══════╝╚═════╝ 
    //                                                                                                           
    public class RotationSpeed : GH_MutableComponent
    {
        public RotationSpeed() : base(
            "RotationSpeed",
            "RotationSpeed",
            "Changes the TCP angular rotation speed value new Actions will be ran at.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("fa13c9cf-f136-4d12-ae90-89baa41ce928");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_RotationSpeed;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "RotationSpeedTo", "RotationSpeedTo", "Increases the TCP angular rotation speed value new Actions  will run at.");
            mpManager.AddParameter(false, typeof(Param_Number), "RotationSpeedInc", "RS", "TCP angular rotation speed increment in deg/s. Decreasing the total to zero or less will reset it back to the robot's default.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "RotationSpeed", "RotationSpeed", "Sets the TCP angular rotation speed value new Actions will run at.");
            mpManager.AddParameter(true, typeof(Param_Number), "RotationSpeedInc", "RS", "TCP angular rotation speed value in deg/s. Setting this value to zero or less will reset it back to the robot's default.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "RotationSpeed Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double rotSpeed = 0;

            if (!DA.GetData(0, ref rotSpeed)) return;

            DA.SetData(0, new ActionRotationSpeed((int)Math.Round(rotSpeed), this.Relative));
        }
    }
}
