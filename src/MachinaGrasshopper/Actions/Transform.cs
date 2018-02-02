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

namespace MachinaGrasshopper
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
            "Performs a compound relative rotation + translation transformation in a single action. Note that when performing relative transformations, the R+T versus T+R order matters.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("0e2b1417-5959-43df-9968-e6dc842a816c");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Transform;

        protected override bool ShallowInputMutation => false;

        //protected override void RegisterInputParams(GH_InputParamManager pManager)
        //{
        //    pManager.AddVectorParameter("Direction", "V", "Translation vector", GH_ParamAccess.item, Vector3d.Zero);
        //    pManager.AddVectorParameter("Axis", "V", "Rotation axis (positive rotation direction is defined by the right-hand rule).", GH_ParamAccess.item, Vector3d.XAxis);
        //    pManager.AddNumberParameter("Angle", "A", "Rotation angle in degrees", GH_ParamAccess.item, 0);
        //    pManager.AddBooleanParameter("TranslationFirst", "t", "Apply translation first? Note that when performing relative transformations, the R+T versus T+R order matters.", GH_ParamAccess.item, true);
        //}

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Relative
            mpManager.AddComponentNames(true, "Transform", "Transform", "Performs a compound relative transformation as a rotation + translation. Note that when performing relative transformations, the R+T versus T+R order matters.");
            mpManager.AddParameter(true, typeof(Param_Vector), "Direction", "TV", "Translation vector.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Vector), "Axis", "RV", "Rotation axis.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "Angle", "A", "Rotation angle in degrees.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Vector), "Translation First", "t", "Apply translation first? Note that when performing relative transformations, the R+T versus T+R order matters.", GH_ParamAccess.item);
            
            // Absolute
            mpManager.AddComponentNames(false, "TransformTo", "TransformTo", "Performs a compound absolute transformation to target Plane. The device's new absolute position and orientation will be those of the plane.");
            mpManager.AddParameter(false, typeof(Param_Plane), "Plane", "P", "Target Plane to transform to", GH_ParamAccess.item);
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
                    new Machina.Vector(dir.X, dir.Y, dir.Z),
                    new Rotation(axis.X, axis.Y, axis.Z, ang),
                    true,
                    trans));
            }
            else
            {
                Plane pl = Plane.Unset;

                if (!DA.GetData(0, ref pl)) return;

                DA.SetData(0, new ActionTransformation(
                    new Machina.Vector(pl.Origin.X, pl.Origin.Y, pl.Origin.Z),
                    new Machina.Orientation(pl.XAxis.X, pl.XAxis.Y, pl.XAxis.Z, pl.YAxis.X, pl.YAxis.Y, pl.YAxis.Z),
                    false,
                    true));
            }
        }

        
    }
    

}
