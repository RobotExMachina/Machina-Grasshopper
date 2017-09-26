using System;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Machina;

namespace MachinaGrasshopper
{

    //  ███╗   ███╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗
    //  ████╗ ████║██╔═══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║
    //  ██╔████╔██║██║   ██║   ██║   ██║██║   ██║██╔██╗ ██║
    //  ██║╚██╔╝██║██║   ██║   ██║   ██║██║   ██║██║╚██╗██║
    //  ██║ ╚═╝ ██║╚██████╔╝   ██║   ██║╚██████╔╝██║ ╚████║
    //  ╚═╝     ╚═╝ ╚═════╝    ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                     
    public class Motion : GH_Component
    {
        public Motion() : base(
            "Motion", 
            "Motion",
            "Sets the current type of motion to be applied to future translation actions. This can be \"linear\" (default) for straight line movements in euclidean space, or \"joint\" for smooth interpolation between joint angles. NOTE: \"joint\" motion may produce unexpected trajectories resulting in reorientations or collisions. Use with caution!", 
            "Machina", 
            "Actions") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("1a97b12b-0422-46aa-945f-373f9afdc39a");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Type", "T", "\"linear\" or \"joint\"", GH_ParamAccess.item, "linear");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Motion Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string type = "";

            if (!DA.GetData(0, ref type)) return;

            MotionType t = MotionType.Undefined;

            type = type.ToLower();
            if (type.Equals("linear"))
            {
                t = MotionType.Linear;
            }
            else if (type.Equals("joint"))
            {
                t = MotionType.Joint;
            }

            if (t == MotionType.Undefined)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid motion type: please input \"linear\" or \"joint\"");
                return;
            }

            DA.SetData(0, new ActionMotion(t));
        }
    }


    //   ██████╗ ██████╗  ██████╗ ██████╗ ██████╗ ██╗███╗   ██╗ █████╗ ████████╗███████╗███████╗
    //  ██╔════╝██╔═══██╗██╔═══██╗██╔══██╗██╔══██╗██║████╗  ██║██╔══██╗╚══██╔══╝██╔════╝██╔════╝
    //  ██║     ██║   ██║██║   ██║██████╔╝██║  ██║██║██╔██╗ ██║███████║   ██║   █████╗  ███████╗
    //  ██║     ██║   ██║██║   ██║██╔══██╗██║  ██║██║██║╚██╗██║██╔══██║   ██║   ██╔══╝  ╚════██║
    //  ╚██████╗╚██████╔╝╚██████╔╝██║  ██║██████╔╝██║██║ ╚████║██║  ██║   ██║   ███████╗███████║
    //   ╚═════╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═════╝ ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚══════╝
    //                                                                                          
    public class Coordinates : GH_Component
    {
        public Coordinates() : base(
            "Coordinates",
            "Coordinates",
            "Sets the coordinate system that will be used for future relative actions. This can be \"global\" or \"world\" (default) to refer to the system's global reference coordinates, or \"local\" to refer to the device's local reference frame. For example, for a robotic arm, the \"global\" coordinate system will be the robot's base, and the \"local\" one will be the coordinates of the tool tip.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("e59ecc48-7247-4892-bb6d-1e56680ade74");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Type", "T", "\"global\" or \"local\"", GH_ParamAccess.item, "global");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Coordinates Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string type = "";

            if (!DA.GetData(0, ref type)) return;

            ReferenceCS refcs;
            type = type.ToLower();
            if (type.Equals("global") || type.Equals("world"))
            {
                refcs = ReferenceCS.World;
            }
            else if (type.Equals("local"))
            {
                refcs = ReferenceCS.Local;
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid reference coordinate system: please input \"global\" or \"local\"");
                return;
            }

            DA.SetData(0, new ActionCoordinates(refcs));
        }
    }


    //  ███████╗██████╗ ███████╗███████╗██████╗ 
    //  ██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗
    //  ███████╗██████╔╝█████╗  █████╗  ██║  ██║
    //  ╚════██║██╔═══╝ ██╔══╝  ██╔══╝  ██║  ██║
    //  ███████║██║     ███████╗███████╗██████╔╝
    //  ╚══════╝╚═╝     ╚══════╝╚══════╝╚═════╝ 
    //                                          
    public class Speed : GH_Component
    {
        public Speed() : base(
            "Speed",
            "Speed",
            "Increases the speed in mm/s at which future transformation actions will run.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("5ce2951b-fdee-4d67-ab2b-bb97204bfdc7");
        protected override System.Drawing.Bitmap Icon => null;

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

    public class SpeedTo : GH_Component
    {
        public SpeedTo() : base(
            "SpeedTo",
            "SpeedTo",
            "Sets the speed in mm/s at which future transformation actions will run.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("3067745a-9183-4f51-96af-95efec967888");
        protected override System.Drawing.Bitmap Icon => null;

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



    //  ██████╗ ██████╗ ███████╗ ██████╗██╗███████╗██╗ ██████╗ ███╗   ██╗
    //  ██╔══██╗██╔══██╗██╔════╝██╔════╝██║██╔════╝██║██╔═══██╗████╗  ██║
    //  ██████╔╝██████╔╝█████╗  ██║     ██║███████╗██║██║   ██║██╔██╗ ██║
    //  ██╔═══╝ ██╔══██╗██╔══╝  ██║     ██║╚════██║██║██║   ██║██║╚██╗██║
    //  ██║     ██║  ██║███████╗╚██████╗██║███████║██║╚██████╔╝██║ ╚████║
    //  ╚═╝     ╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                                   
    public class Precision : GH_Component
    {
        public Precision() : base(
            "Precision",
            "Precision",
            "Increase the default precision value new actions will be given. Precision is measured as the radius of the smooth interpolation between motion targets. This is refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("eaadd1fc-caa9-442b-af5e-273bc3961b73");
        protected override System.Drawing.Bitmap Icon => null;

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

            DA.SetData(0, new ActionZone((int)Math.Round(radiusInc), true));
        }
    }

    public class PrecisionTo : GH_Component
    {
        public PrecisionTo() : base(
            "PrecisionTo",
            "PrecisionTo",
            "Set the default precision value new actions will be given. Precision is measured as the radius of the smooth interpolation between motion targets. This is refered to as \"Zone\", \"Approximate Positioning\" or \"Blending Radius\" in different platforms.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("f7127638-e4bc-4cd1-904d-ad301bd63d9a");
        protected override System.Drawing.Bitmap Icon => null;

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

            DA.SetData(0, new ActionZone((int)Math.Round(radius), false));
        }
    }



    //  ██████╗ ██╗   ██╗███████╗██╗  ██╗    ██╗██████╗  ██████╗ ██████╗ 
    //  ██╔══██╗██║   ██║██╔════╝██║  ██║   ██╔╝██╔══██╗██╔═══██╗██╔══██╗
    //  ██████╔╝██║   ██║███████╗███████║  ██╔╝ ██████╔╝██║   ██║██████╔╝
    //  ██╔═══╝ ██║   ██║╚════██║██╔══██║ ██╔╝  ██╔═══╝ ██║   ██║██╔═══╝ 
    //  ██║     ╚██████╔╝███████║██║  ██║██╔╝   ██║     ╚██████╔╝██║     
    //  ╚═╝      ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═╝    ╚═╝      ╚═════╝ ╚═╝     
    //                                                                   
    public class PushSettings : GH_Component
    {
        public PushSettings() : base(
            "PushSettings",
            "PushSettings",
            "Stores current state settings to a buffer, so that temporary changes can be made, and settings can be reverted to the stored state later with PopSettings().",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("f60026ed-e66f-4cba-8592-5d3efc9362bf");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }
           
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "PushSettings Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, new ActionPushPop(true));
        }
    }

    public class PopSettings : GH_Component
    {
        public PopSettings() : base(
            "PopSettings",
            "PopSettings",
            "Reverts current settings to the state store by the last call to PushSettings().",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("82a53cd1-c33c-4cfa-907c-94288058130e");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "PopSettings Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, new ActionPushPop(false));
        }
    }




    //  ███╗   ███╗ ██████╗ ██╗   ██╗███████╗
    //  ████╗ ████║██╔═══██╗██║   ██║██╔════╝
    //  ██╔████╔██║██║   ██║██║   ██║█████╗  
    //  ██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██╔══╝  
    //  ██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ███████╗
    //  ╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚══════╝
    //                                       
    public class Move : GH_Component
    {
        public Move() : base(
            "Move",
            "Move",
            "Moves the device along a speficied vector relative to its current position.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("1969ae91-3dad-4af2-991b-b23e60724873");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Direction", "V", "Translation vector", GH_ParamAccess.item, Vector3d.Zero);
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

    public class MoveTo : GH_Component
    {
        public MoveTo() : base(
            "MoveTo",
            "MoveTo",
            "Moves the device to an absolute position in global coordinates.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("c90a1258-1dd2-4d14-ab13-8dc53a47b547");
        protected override System.Drawing.Bitmap Icon => null;

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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("db2e3c56-5973-4f07-8d6a-ba31c659704d");
        protected override System.Drawing.Bitmap Icon => null;

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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("9410b629-1016-486f-8464-85ecfd9500f7");
        protected override System.Drawing.Bitmap Icon => null;

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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("81226a72-37d6-4dad-a7a3-711adb51b26e");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Direction", "V", "Translation vector", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddVectorParameter("Axis", "V", "Rotation axis (positive rotation direction is defined by the right-hand rule).", GH_ParamAccess.item, Vector3d.XAxis);
            pManager.AddNumberParameter("Angle", "A", "Rotation angle in degrees", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("TranslationFirst", "T", "Apply translation first? Note that when performing relative transformations, the R+T versus T+R order matters.", GH_ParamAccess.item, true);
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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("7bf2965f-7fa6-4cf0-84ac-4948b777b478");
        protected override System.Drawing.Bitmap Icon => null;

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




}
