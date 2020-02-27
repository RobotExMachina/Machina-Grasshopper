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
using MPoint = Machina.Types.Geometry.Point;
using MVector = Machina.Types.Geometry.Vector;
using MOrientation = Machina.Types.Geometry.Orientation;
using MachinaGrasshopper.GH_Utils;

namespace MachinaGrasshopper.Action
{

    //  ████████╗██████╗  █████╗ ███╗   ██╗███████╗███████╗ ██████╗ ██████╗ ███╗   ███╗
    //  ╚══██╔══╝██╔══██╗██╔══██╗████╗  ██║██╔════╝██╔════╝██╔═══██╗██╔══██╗████╗ ████║
    //     ██║   ██████╔╝███████║██╔██╗ ██║███████╗█████╗  ██║   ██║██████╔╝██╔████╔██║
    //     ██║   ██╔══██╗██╔══██║██║╚██╗██║╚════██║██╔══╝  ██║   ██║██╔══██╗██║╚██╔╝██║
    //     ██║   ██║  ██║██║  ██║██║ ╚████║███████║██║     ╚██████╔╝██║  ██║██║ ╚═╝ ██║
    //     ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝
    //                                                                                 
    public class Transform : GH_MutableComponent
    {
        public Transform() : base(
            "Transform",
            "Transform",
            "Performs a compound transformation, composed of a translation and a rotation.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("0e2b1417-5959-43df-9968-e6dc842a816c");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_Transform;

        protected override bool ShallowInputMutation => false;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "TransformTo", "TransformTo", "Performs a compound absolute transformation to target Plane. The device's new absolute position and orientation will be those of the Plane.");
            mpManager.AddParameter(false, typeof(Param_Plane), "Plane", "P", "Target Plane to transform to", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Transform", "Transform", "Performs a compound relative transformation as a rotation + translation. Note that when performing relative transformations, the R+T versus T+R order matters.");
            mpManager.AddParameter(true, typeof(Param_Vector), "Direction", "TV", "Translation vector.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Vector), "Axis", "RV", "Rotation axis.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "Angle", "A", "Rotation angle in degrees.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Vector), "Translation First", "t", "Apply translation first? Note that when performing relative transformations, the R+T versus T+R order matters.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Transform Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (this.Relative)
            {
                Vector3d dir = Vector3d.Zero;
                Vector3d axis = Vector3d.Zero;
                double ang = 0;
                bool trans = false;

                if (!DA.GetData(0, ref dir)) return;
                if (!DA.GetData(1, ref axis)) return;
                if (!DA.GetData(2, ref ang)) return;
                if (!DA.GetData(3, ref trans)) return;

                DA.SetData(0, new ActionTransformation(
                    new MVector(dir.X, dir.Y, dir.Z),
                    new Rotation(axis.X, axis.Y, axis.Z, ang),
                    true,
                    trans));
            }
            else
            {
                Plane pl = Plane.Unset;

                if (!DA.GetData(0, ref pl)) return;

                DA.SetData(0, new ActionTransformation(
                    new MVector(pl.Origin.X, pl.Origin.Y, pl.Origin.Z),
                    new MOrientation(pl.XAxis.X, pl.XAxis.Y, pl.XAxis.Z, pl.YAxis.X, pl.YAxis.Y, pl.YAxis.Z),
                    false,
                    true));
            }
        }

        
    }
    

}
