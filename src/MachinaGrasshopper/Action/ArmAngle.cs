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
    //   █████╗ ██████╗ ███╗   ███╗ █████╗ ███╗   ██╗ ██████╗ ██╗     ███████╗
    //  ██╔══██╗██╔══██╗████╗ ████║██╔══██╗████╗  ██║██╔════╝ ██║     ██╔════╝
    //  ███████║██████╔╝██╔████╔██║███████║██╔██╗ ██║██║  ███╗██║     █████╗  
    //  ██╔══██║██╔══██╗██║╚██╔╝██║██╔══██║██║╚██╗██║██║   ██║██║     ██╔══╝  
    //  ██║  ██║██║  ██║██║ ╚═╝ ██║██║  ██║██║ ╚████║╚██████╔╝███████╗███████╗
    //  ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝
    //                     
                                                       
    /// <summary>
    /// For the time being, there is only Absolute mode, so no mutating component.
    /// </summary>
    public class ArmAngleTo : GH_Component
    {
        public ArmAngleTo() : base(
            "ArmAngleTo",
            "ArmAngleTo",
            "Set the value of the arm-angle parameter. This value represents the planar offset around the 7th axis for 7-dof robotic arms.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("4171a404-7166-4281-ad33-cbf796baa7bd");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_ArmAngle;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "v", "Angular value in degrees.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "ArmAngle Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double v = 0;

            if (!DA.GetData(0, ref v)) return;

            DA.SetData(0, new ActionArmAngle(v, false));
        }
    }
}
