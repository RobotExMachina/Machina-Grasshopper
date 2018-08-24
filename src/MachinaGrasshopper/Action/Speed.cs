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

    //  ███████╗██████╗ ███████╗███████╗██████╗ 
    //  ██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗
    //  ███████╗██████╔╝█████╗  █████╗  ██║  ██║
    //  ╚════██║██╔═══╝ ██╔══╝  ██╔══╝  ██║  ██║
    //  ███████║██║     ███████╗███████╗██████╔╝
    //  ╚══════╝╚═╝     ╚══════╝╚══════╝╚═════╝ 
    //            
    public class Speed : GH_MutableComponent
    {
        public Speed() : base(
            "Speed",
            "Speed",
            "Changes the speed at which future actions will execute.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("b8663444-298a-4879-8dc9-128f86cf8268");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Speed;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "SpeedTo", "SpeedTo", "Increases the TCP speed at which future actions will execute.");
            mpManager.AddParameter(false, typeof(Param_Number), "Speed", "S", "Speed value in mm/s.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Speed", "Speed", "Sets the TCP speed at which future actions will execute.");
            mpManager.AddParameter(true, typeof(Param_Number), "SpeedInc", "S", "Speed increment in mm/s.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Speed Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double speed = 0;

            if (!DA.GetData(0, ref speed)) return;

            DA.SetData(0, new ActionSpeed((int)Math.Round(speed), this.Relative));
        }
    }
}
