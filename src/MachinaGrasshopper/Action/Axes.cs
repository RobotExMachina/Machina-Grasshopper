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
using Machina.Types.Geometry;
using MachinaGrasshopper.GH_Utils;

namespace MachinaGrasshopper.Action
{


    //   █████╗ ██╗  ██╗███████╗███████╗
    //  ██╔══██╗╚██╗██╔╝██╔════╝██╔════╝
    //  ███████║ ╚███╔╝ █████╗  ███████╗
    //  ██╔══██║ ██╔██╗ ██╔══╝  ╚════██║
    //  ██║  ██║██╔╝ ██╗███████╗███████║
    //  ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝
    //                                  
    public class Axes : GH_MutableComponent
    {
        public Axes() : base(
            "Axes",
            "Axes",
            "Changes the axes' rotation angle in degrees at the joints of mechanical devices, specially robotic arms.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("573731ca-6c12-433e-bd35-7cc704c3563d");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Axes;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "AxesTo", "AxesTo", "Set the axes' rotation angle in degrees at the joints of mechanical devices, specially robotic arms.");
            mpManager.AddParameter(false, typeof(Param_Number), "A1", "A1", "Angular value in degrees for Axis 1", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_Number), "A2", "A2", "Angular value in degrees for Axis 2", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_Number), "A3", "A3", "Angular value in degrees for Axis 3", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_Number), "A4", "A4", "Angular value in degrees for Axis 4", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_Number), "A5", "A5", "Angular value in degrees for Axis 5", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_Number), "A6", "A6", "Angular value in degrees for Axis 6", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Axes", "Axes", "Increase the axes' rotation angle in degrees at the joints of mechanical devices, specially robotic arms.");
            mpManager.AddParameter(true, typeof(Param_Number), "A1Inc", "A1", "Rotational increment in degrees for Axis 1", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "A2Inc", "A2", "Rotational increment in degrees for Axis 2", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "A3Inc", "A3", "Rotational increment in degrees for Axis 3", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "A4Inc", "A4", "Rotational increment in degrees for Axis 4", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "A5Inc", "A5", "Rotational increment in degrees for Axis 5", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "A6Inc", "A6", "Rotational increment in degrees for Axis 6", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Axes Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double a4 = 0;
            double a5 = 0;
            double a6 = 0;

            if (!DA.GetData(0, ref a1)) return;
            if (!DA.GetData(1, ref a2)) return;
            if (!DA.GetData(2, ref a3)) return;
            if (!DA.GetData(3, ref a4)) return;
            if (!DA.GetData(4, ref a5)) return;
            if (!DA.GetData(5, ref a6)) return;

            DA.SetData(0, new ActionAxes(new Joints(a1, a2, a3, a4, a5, a6), this.Relative));
        }
    }
    

  
}
