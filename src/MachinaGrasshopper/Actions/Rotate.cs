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
using MachinaGrasshopper.MACHINAGH_Utils;

namespace MachinaGrasshopper.Actions
{

    //  ██████╗  ██████╗ ████████╗ █████╗ ████████╗███████╗
    //  ██╔══██╗██╔═══██╗╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
    //  ██████╔╝██║   ██║   ██║   ███████║   ██║   █████╗  
    //  ██╔══██╗██║   ██║   ██║   ██╔══██║   ██║   ██╔══╝  
    //  ██║  ██║╚██████╔╝   ██║   ██║  ██║   ██║   ███████╗
    //  ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    //        
    public class Rotate : GH_MutableComponent
    {

        public Rotate() : base(
            "Rotate",
            "Rotate",
            "Rotates the device to a specified orientation, or a specified angle in degrees along the specified vector.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("c48d908d-3e0d-4600-90de-1330b9dc7973");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Rotate;
        protected override bool ShallowInputMutation => false;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "RotateTo", "RotateTo", "Rotates the device to a specified orientation.");
            mpManager.AddParameter(false, typeof(Param_Plane), "Plane", "Pl", "Target spatial orientation.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Rotate", "Rotate", "Rotates the device a specified angle in degrees along the specified vector.");
            mpManager.AddParameter(true, typeof(Param_Vector), "Axis", "V", "Rotation axis, with positive rotation direction is defined by the right-hand rule.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "Angle", "A", "Rotation angle in degrees.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Rotate Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (this.Relative)
            {
                Vector3d v = Vector3d.Zero;
                double ang = 0;

                if (!DA.GetData(0, ref v)) return;
                if (!DA.GetData(1, ref ang)) return;

                DA.SetData(0, new ActionRotation(new Machina.Rotation(v.X, v.Y, v.Z, ang), true));
            }
            else
            {
                Plane pl = Plane.Unset;

                if (!DA.GetData(0, ref pl)) return;

                DA.SetData(0, new ActionRotation(new Machina.Orientation(pl.XAxis.X, pl.XAxis.Y, pl.XAxis.Z, pl.YAxis.X, pl.YAxis.Y, pl.YAxis.Z), false));
            }
        }
    }
}
