using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Machina;

namespace MachinaGrasshopper.Graveyard
{
    //   ██████╗ ██████╗  █████╗ ██╗   ██╗███████╗██╗   ██╗ █████╗ ██████╗ ██████╗ 
    //  ██╔════╝ ██╔══██╗██╔══██╗██║   ██║██╔════╝╚██╗ ██╔╝██╔══██╗██╔══██╗██╔══██╗
    //  ██║  ███╗██████╔╝███████║██║   ██║█████╗   ╚████╔╝ ███████║██████╔╝██║  ██║
    //  ██║   ██║██╔══██╗██╔══██║╚██╗ ██╔╝██╔══╝    ╚██╔╝  ██╔══██║██╔══██╗██║  ██║
    //  ╚██████╔╝██║  ██║██║  ██║ ╚████╔╝ ███████╗   ██║   ██║  ██║██║  ██║██████╔╝
    //   ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝  ╚═══╝  ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ 
    //                                                                             
    /// <summary>
    /// A shrine of peace and remembrance for the heroes of the past, an ode to our proud legacy!
    /// 
    /// Components will be marked 'OLD' if the classname contains the string "obsolete" or the class
    /// has been decorated with the ObsoleteAttribute.
    /// </summary>


    //  ██╗    ██╗ ██████╗ 
    //  ██║   ██╔╝██╔═══██╗
    //  ██║  ██╔╝ ██║   ██║
    //  ██║ ██╔╝  ██║   ██║
    //  ██║██╔╝   ╚██████╔╝
    //  ╚═╝╚═╝     ╚═════╝ 
    //                     
    [Obsolete("Deprecated", false)]
    public class TurnOn : GH_Component
    {
        public TurnOn() : base(
            "TurnOn",
            "TurnOn",
            "Turn digital output on. Alias for `WriteDigital(ioNum, true)`",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("8bd5bc0d-249e-4744-8530-cf8fced77492");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("DigitalPinNumber", "N", "Digital pin number", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "TurnOn Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 1;

            if (!DA.GetData(0, ref id)) return;

            DA.SetData(0, new ActionIODigital(id, true));
        }
    }
    [Obsolete("Deprecated", false)]
    public class TurnOff : GH_Component
    {
        public TurnOff() : base(
            "TurnOff",
            "TurnOff",
            "Turn digital output off. Alias for `WriteDigital(ioNum, false)`",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("15d234aa-2f63-488e-a95e-cc89ffcca3b6");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("DigitalPinNumber", "N", "Digital pin number", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "TurnOff Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 1;

            if (!DA.GetData(0, ref id)) return;

            DA.SetData(0, new ActionIODigital(id, false));
        }
    }



    //  ██████╗ ██████╗ ███████╗ ██████╗██╗███████╗██╗ ██████╗ ███╗   ██╗
    //  ██╔══██╗██╔══██╗██╔════╝██╔════╝██║██╔════╝██║██╔═══██╗████╗  ██║
    //  ██████╔╝██████╔╝█████╗  ██║     ██║███████╗██║██║   ██║██╔██╗ ██║
    //  ██╔═══╝ ██╔══██╗██╔══╝  ██║     ██║╚════██║██║██║   ██║██║╚██╗██║
    //  ██║     ██║  ██║███████╗╚██████╗██║███████║██║╚██████╔╝██║ ╚████║
    //  ╚═╝     ╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                                   
    [Obsolete("Updated", false)]
    public class Precision : GH_Component
    {
        public Precision() : base(
            "Precision",
            "Precision",
            "Increase the default precision value new actions will be given. Precision is measured as the radius of the smooth interpolation between motion targets. This is refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("eaadd1fc-caa9-442b-af5e-273bc3961b73");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Precision;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("RadiusIncrement", "RI", "Smoothing radius increment in mm", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Precision Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radiusInc = 0;

            if (!DA.GetData(0, ref radiusInc)) return;

            DA.SetData(0, new ActionPrecision((int)Math.Round(radiusInc), true));
        }
    }

    [Obsolete("Updated", false)]
    public class PrecisionTo : GH_Component
    {
        public PrecisionTo() : base(
            "PrecisionTo",
            "PrecisionTo",
            "Set the default precision value new actions will be given. Precision is measured as the radius of the smooth interpolation between motion targets. This is refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("f7127638-e4bc-4cd1-904d-ad301bd63d9a");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Precision;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Radius", "R", "Smoothing radius value in mm", GH_ParamAccess.item, 5);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "PrecisionTo Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radius = 0;

            if (!DA.GetData(0, ref radius)) return;

            DA.SetData(0, new ActionPrecision((int)Math.Round(radius), false));
        }
    }


    //  ███████╗██████╗ ███████╗███████╗██████╗ 
    //  ██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗
    //  ███████╗██████╔╝█████╗  █████╗  ██║  ██║
    //  ╚════██║██╔═══╝ ██╔══╝  ██╔══╝  ██║  ██║
    //  ███████║██║     ███████╗███████╗██████╔╝
    //  ╚══════╝╚═╝     ╚══════╝╚══════╝╚═════╝ 
    //
    [Obsolete("Updated", false)]
    public class Speed : GH_Component
    {
        public Speed() : base(
            "Speed",
            "Speed",
            "Increases the speed in mm/s at which future transformation actions will run.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("5ce2951b-fdee-4d67-ab2b-bb97204bfdc7");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Speed;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("SpeedIncrement", "SI", "Speed increment in mm/s", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Speed Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double speedInc = 0;

            if (!DA.GetData(0, ref speedInc)) return;

            DA.SetData(0, new ActionSpeed((int)Math.Round(speedInc), true));
        }
    }
    [Obsolete("Updated", false)]
    public class SpeedTo : GH_Component
    {
        public SpeedTo() : base(
            "SpeedTo",
            "SpeedTo",
            "Sets the speed in mm/s at which future transformation actions will run.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("3067745a-9183-4f51-96af-95efec967888");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Speed;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Speed", "S", "Speed value in mm/s", GH_ParamAccess.item, 20);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "SpeedTo Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double speed = 0;

            if (!DA.GetData(0, ref speed)) return;

            if (speed < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The value of the speed cannot be negative");
            }

            DA.SetData(0, new ActionSpeed((int)Math.Round(speed), false));
        }
    }


    //  ███╗   ███╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗
    //  ████╗ ████║██╔═══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║
    //  ██╔████╔██║██║   ██║   ██║   ██║██║   ██║██╔██╗ ██║
    //  ██║╚██╔╝██║██║   ██║   ██║   ██║██║   ██║██║╚██╗██║
    //  ██║ ╚═╝ ██║╚██████╔╝   ██║   ██║╚██████╔╝██║ ╚████║
    //  ╚═╝     ╚═╝ ╚═════╝    ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                     
    [Obsolete("Updated", false)]
    public class MotionType : GH_Component
    {
        public MotionType() : base(
            "MotionType",  // the name that shows up on the tab, on yellow bar on toolip, on component on 'Draw Full Names'
            "MotionType",  // the name that shows up on the non-icon component with 'DFN' off, and in parenthesis after the main name on the yellow bar on tooltip
            "Sets the current type of motion to be applied to future translation actions. This can be \"linear\" (default) for straight line movements in euclidean space, or \"joint\" for smooth interpolation between joint angles. NOTE: \"joint\" motion may produce unexpected trajectories resulting in reorientations or collisions. Use with caution!",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("1a97b12b-0422-46aa-945f-373f9afdc39a");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_ActionMode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Type", "T", "\"linear\" or \"joint\"", GH_ParamAccess.item, "linear");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "MotionType Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string type = "";

            if (!DA.GetData(0, ref type)) return;

            Machina.MotionType t = Machina.MotionType.Undefined;

            type = type.ToLower();
            if (type.Equals("linear"))
            {
                t = Machina.MotionType.Linear;
            }
            else if (type.Equals("joint"))
            {
                t = Machina.MotionType.Joint;
            }

            if (t == Machina.MotionType.Undefined)
            {
                base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid motion type: please input \"linear\" or \"joint\" as String.");
                return;
            }

            DA.SetData(0, new ActionMotion(t));
        }
    }



    [Obsolete("Updated", false)]
    public class Move : GH_Component
    {
        public Move() : base(
            "Move",
            "Move",
            "Moves the device along a speficied vector relative to its current position.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("1969ae91-3dad-4af2-991b-b23e60724873");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Move;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Vector", "V", "Translation vector", GH_ParamAccess.item, Vector3d.Zero);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Move Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Vector3d v = Vector3d.Zero;

            if (!DA.GetData(0, ref v)) return;

            DA.SetData(0, new ActionTranslation(new Machina.Vector(v.X, v.Y, v.Z), true));
        }
    }

    [Obsolete("Updated", false)]
    public class MoveTo : GH_Component
    {
        public MoveTo() : base(
            "MoveTo",
            "MoveTo",
            "Moves the device to an absolute position in global coordinates.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("c90a1258-1dd2-4d14-ab13-8dc53a47b547");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Move;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Target position", GH_ParamAccess.item, Point3d.Origin);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "MoveTo Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d p = Point3d.Origin;

            if (!DA.GetData(0, ref p)) return;

            DA.SetData(0, new ActionTranslation(new Machina.Point(p.X, p.Y, p.Z), false));
        }
    }


    //  ██████╗  ██████╗ ████████╗ █████╗ ████████╗███████╗
    //  ██╔══██╗██╔═══██╗╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
    //  ██████╔╝██║   ██║   ██║   ███████║   ██║   █████╗  
    //  ██╔══██╗██║   ██║   ██║   ██╔══██║   ██║   ██╔══╝  
    //  ██║  ██║╚██████╔╝   ██║   ██║  ██║   ██║   ███████╗
    //  ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    //                                                     
    public class Rotate : GH_Component
    {
        public Rotate() : base(
            "Rotate",
            "Rotate",
            "Rotates the device a specified angle in degrees along the specified vector.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("db2e3c56-5973-4f07-8d6a-ba31c659704d");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Rotate;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Axis", "V", "Rotation axis (positive rotation direction is defined by the right-hand rule).", GH_ParamAccess.item, Vector3d.XAxis);
            pManager.AddNumberParameter("Angle", "A", "Rotation angle in degrees", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Rotate Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Vector3d v = Vector3d.Zero;
            double ang = 0;

            if (!DA.GetData(0, ref v)) return;
            if (!DA.GetData(1, ref ang)) return;

            DA.SetData(0, new ActionRotation(new Rotation(v.X, v.Y, v.Z, ang), true));
        }
    }

    public class RotateTo : GH_Component
    {
        public RotateTo() : base(
            "RotateTo",
            "RotateTo",
            "Rotate the devices to an absolute orientation defined by the two main X and Y axes of specified Plane.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("9410b629-1016-486f-8464-85ecfd9500f7");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Rotate;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Target spatial orientation", GH_ParamAccess.item, Plane.WorldXY);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "RotateTo Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane pl = Plane.Unset;

            if (!DA.GetData(0, ref pl)) return;

            DA.SetData(0, new ActionRotation(new Machina.Orientation(pl.XAxis.X, pl.XAxis.Y, pl.XAxis.Z, pl.YAxis.X, pl.YAxis.Y, pl.YAxis.Z), false));
        }
    }



    //  ████████╗██████╗  █████╗ ███╗   ██╗███████╗███████╗ ██████╗ ██████╗ ███╗   ███╗
    //  ╚══██╔══╝██╔══██╗██╔══██╗████╗  ██║██╔════╝██╔════╝██╔═══██╗██╔══██╗████╗ ████║
    //     ██║   ██████╔╝███████║██╔██╗ ██║███████╗█████╗  ██║   ██║██████╔╝██╔████╔██║
    //     ██║   ██╔══██╗██╔══██║██║╚██╗██║╚════██║██╔══╝  ██║   ██║██╔══██╗██║╚██╔╝██║
    //     ██║   ██║  ██║██║  ██║██║ ╚████║███████║██║     ╚██████╔╝██║  ██║██║ ╚═╝ ██║
    //     ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝
    //                                                                                 
    public class Transform : GH_Component
    {
        public Transform() : base(
            "Transform",
            "Transform",
            "Performs a compound relative rotation + translation transformation in a single action. Note that when performing relative transformations, the R+T versus T+R order matters.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("81226a72-37d6-4dad-a7a3-711adb51b26e");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Transform;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Direction", "V", "Translation vector", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddVectorParameter("Axis", "V", "Rotation axis (positive rotation direction is defined by the right-hand rule).", GH_ParamAccess.item, Vector3d.XAxis);
            pManager.AddNumberParameter("Angle", "A", "Rotation angle in degrees", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("TranslationFirst", "t", "Apply translation first? Note that when performing relative transformations, the R+T versus T+R order matters.", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Transform Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
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
    }

    public class TransformTo : GH_Component
    {
        public TransformTo() : base(
            "TransformTo",
            "TransformTo",
            "Performs a compound absolute rotation + translation transformation, or in other words, sets both a new absolute position and orientation for the device in the same action, based on specified Plane.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("7bf2965f-7fa6-4cf0-84ac-4948b777b478");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Transform;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Target Plane to transform to", GH_ParamAccess.item, Plane.WorldXY);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "TransformTo Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
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



    //   █████╗ ██╗  ██╗███████╗███████╗
    //  ██╔══██╗╚██╗██╔╝██╔════╝██╔════╝
    //  ███████║ ╚███╔╝ █████╗  ███████╗
    //  ██╔══██║ ██╔██╗ ██╔══╝  ╚════██║
    //  ██║  ██║██╔╝ ██╗███████╗███████║
    //  ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝
    //                                  
    public class Axes : GH_Component
    {
        public Axes() : base(
            "Axes",
            "Axes",
            "Increase the axes' rotation angle in degrees at the joints of mechanical devices, specially robotic arms.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("15ea7b44-c5d8-470b-9edb-867cc4c0b1aa");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Axes;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A1Inc", "A1", "Rotational increment in degrees for Axis 1", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A2Inc", "A2", "Rotational increment in degrees for Axis 2", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A3Inc", "A3", "Rotational increment in degrees for Axis 3", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A4Inc", "A4", "Rotational increment in degrees for Axis 4", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A5Inc", "A5", "Rotational increment in degrees for Axis 5", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A6Inc", "A6", "Rotational increment in degrees for Axis 6", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Axis Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a1inc = 0;
            double a2inc = 0;
            double a3inc = 0;
            double a4inc = 0;
            double a5inc = 0;
            double a6inc = 0;

            if (!DA.GetData(0, ref a1inc)) return;
            if (!DA.GetData(1, ref a2inc)) return;
            if (!DA.GetData(2, ref a3inc)) return;
            if (!DA.GetData(3, ref a4inc)) return;
            if (!DA.GetData(4, ref a5inc)) return;
            if (!DA.GetData(5, ref a6inc)) return;

            DA.SetData(0, new ActionAxes(new Joints(a1inc, a2inc, a3inc, a4inc, a5inc, a6inc), true));
        }
    }

    public class AxesTo : GH_Component
    {
        public AxesTo() : base(
            "AxesTo",
            "AxesTo",
            "Set the axes' rotation angle in degrees at the joints of mechanical devices, specially robotic arms.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("5b9bc63f-a0f1-4d66-b6a6-679c38ed8014");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Axes;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A1", "A1", "Angular value in degrees for Axis 1", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A2", "A2", "Angular value in degrees for Axis 2", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A3", "A3", "Angular value in degrees for Axis 3", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A4", "A4", "Angular value in degrees for Axis 4", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A5", "A5", "Angular value in degrees for Axis 5", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("A6", "A6", "Angular value in degrees for Axis 6", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Axis Action", GH_ParamAccess.item);
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

            DA.SetData(0, new ActionAxes(new Joints(a1, a2, a3, a4, a5, a6), false));
        }
    }
}
