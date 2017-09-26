using System;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Machina;

namespace MachinaGrasshopper
{

    //   █████╗  ██████╗████████╗██╗ ██████╗ ███╗   ██╗███████╗
    //  ██╔══██╗██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║██╔════╝
    //  ███████║██║        ██║   ██║██║   ██║██╔██╗ ██║███████╗
    //  ██╔══██║██║        ██║   ██║██║   ██║██║╚██╗██║╚════██║
    //  ██║  ██║╚██████╗   ██║   ██║╚██████╔╝██║ ╚████║███████║
    //  ╚═╝  ╚═╝ ╚═════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝
    //                                                         
    /// <summary>
    /// All Action-generator components
    /// </summary>




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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("15ea7b44-c5d8-470b-9edb-867cc4c0b1aa");
        protected override System.Drawing.Bitmap Icon => null;

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

            DA.SetData(0, new ActionJoints(new Joints(a1inc, a2inc, a3inc, a4inc, a5inc, a6inc), true));
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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("5b9bc63f-a0f1-4d66-b6a6-679c38ed8014");
        protected override System.Drawing.Bitmap Icon => null;

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

            DA.SetData(0, new ActionJoints(new Joints(a1, a2, a3, a4, a5, a6), false));
        }
    }

    //  ██╗    ██╗ █████╗ ██╗████████╗
    //  ██║    ██║██╔══██╗██║╚══██╔══╝
    //  ██║ █╗ ██║███████║██║   ██║   
    //  ██║███╗██║██╔══██║██║   ██║   
    //  ╚███╔███╔╝██║  ██║██║   ██║   
    //   ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝   ╚═╝   
    //                                
    public class Wait : GH_Component
    {
        public Wait() : base(
            "Wait",
            "Wait",
            "Pause program execution for a specified amount of time.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("4ec5c686-0ca9-4b60-a99e-8eaf4fe594ac");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Time", "T", "Pause time in milliseconds", GH_ParamAccess.item, 1000);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Wait Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double t = 0;

            if (!DA.GetData(0, ref t)) return;

            DA.SetData(0, new ActionWait((long)Math.Round(t)));
        }
    }

    //  ███╗   ███╗███████╗███████╗███████╗ █████╗  ██████╗ ███████╗
    //  ████╗ ████║██╔════╝██╔════╝██╔════╝██╔══██╗██╔════╝ ██╔════╝
    //  ██╔████╔██║█████╗  ███████╗███████╗███████║██║  ███╗█████╗  
    //  ██║╚██╔╝██║██╔══╝  ╚════██║╚════██║██╔══██║██║   ██║██╔══╝  
    //  ██║ ╚═╝ ██║███████╗███████║███████║██║  ██║╚██████╔╝███████╗
    //  ╚═╝     ╚═╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝
    //                                                              
    public class Message : GH_Component
    {
        public Message() : base(
            "Message",
            "Message",
            "Displays a text message on the device. This will depend on the device's configuration. For example, for ABB robots it will display it on the FlexPendant's log window.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("2675e57a-5b6f-441a-9f94-69bb155b7b59");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "T", "Text message to display", GH_ParamAccess.item, "Hello World!");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Message Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string msg = "";

            if (!DA.GetData(0, ref msg)) return;

            DA.SetData(0, new ActionMessage(msg));
        }
    }



    //   ██████╗ ██████╗ ███╗   ███╗███╗   ███╗███████╗███╗   ██╗████████╗
    //  ██╔════╝██╔═══██╗████╗ ████║████╗ ████║██╔════╝████╗  ██║╚══██╔══╝
    //  ██║     ██║   ██║██╔████╔██║██╔████╔██║█████╗  ██╔██╗ ██║   ██║   
    //  ██║     ██║   ██║██║╚██╔╝██║██║╚██╔╝██║██╔══╝  ██║╚██╗██║   ██║   
    //  ╚██████╗╚██████╔╝██║ ╚═╝ ██║██║ ╚═╝ ██║███████╗██║ ╚████║   ██║   
    //   ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝   ╚═╝   
    //                                                                    
    public class Comment : GH_Component
    {
        public Comment() : base(
            "Comment",
            "Comment",
            "Displays an internal comment in a program compilation. This is useful for internal annotations or reminders, but has no effect on the Robot's behavior.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("a3fc9af6-04ab-49e9-a0fe-d224f4e7e9bf");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Comment", "T", "The comment to be displayed on code compilation", GH_ParamAccess.item, "This is a comment");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Comment Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string comment = "";

            if (!DA.GetData(0, ref comment)) return;

            DA.SetData(0, new ActionComment(comment));
        }
    }


    //  ██████╗ ███████╗    ██╗ █████╗ ████████╗████████╗ █████╗  ██████╗██╗  ██╗
    //  ██╔══██╗██╔════╝   ██╔╝██╔══██╗╚══██╔══╝╚══██╔══╝██╔══██╗██╔════╝██║  ██║
    //  ██║  ██║█████╗    ██╔╝ ███████║   ██║      ██║   ███████║██║     ███████║
    //  ██║  ██║██╔══╝   ██╔╝  ██╔══██║   ██║      ██║   ██╔══██║██║     ██╔══██║
    //  ██████╔╝███████╗██╔╝   ██║  ██║   ██║      ██║   ██║  ██║╚██████╗██║  ██║
    //  ╚═════╝ ╚══════╝╚═╝    ╚═╝  ╚═╝   ╚═╝      ╚═╝   ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝
    //                                                                           
    public class Attach : GH_Component
    {
        public Attach() : base(
            "Attach",
            "Attach",
            "Attach a Tool to the flange of the object, replacing whichever tool was on it before. Note that the Tool Center Point (TCP) will be translated/rotated according to the tool change.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("5598bf85-6887-40b4-a29b-efff6af0864f");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Tool", "T", "A Tool object to attach to the Robot flange", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Attach Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Tool tool = Tool.Unset;

            if (!DA.GetData(0, ref tool))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Tool specified, using default \"NoTool\" object");
            }

            DA.SetData(0, new ActionAttach(tool));
        }
    }

    public class Detach : GH_Component
    {
        public Detach() : base(
            "Detach",
            "Detach",
            "Detach any Tool currently attached to the Robot. Note that the Tool Center Point (TCP) will now be transformed to the Robot's flange.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("f3195b55-742a-429f-bf66-94fce5497bc9");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }  // no info needed

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Detach Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA) 
        {
            DA.SetData(0, new ActionDetach());
        }
    }



    //  ██╗    ██╗ ██████╗ 
    //  ██║   ██╔╝██╔═══██╗
    //  ██║  ██╔╝ ██║   ██║
    //  ██║ ██╔╝  ██║   ██║
    //  ██║██╔╝   ╚██████╔╝
    //  ╚═╝╚═╝     ╚═════╝ 
    //                     
    public class WriteDigital : GH_Component
    {
        public WriteDigital() : base(
            "WriteDigital",
            "WriteDigital",
            "Activate/deactivate digital output.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("a08ed4f1-1913-4f32-8d43-0c98fd1e5bd4");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("DPin", "N", "Digital pin number", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("On", "B", "Turn on?", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "WriteDigital Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 1;
            bool on = false;

            if (!DA.GetData(0, ref id)) return;
            if (!DA.GetData(1, ref on)) return;

            DA.SetData(0, new ActionIODigital(id, on));
        }
    }

    public class WriteAnalog : GH_Component
    {
        public WriteAnalog() : base(
            "WriteAnalog",
            "WriteAnalog",
            "Send a value to analog output.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("ace7ecb7-2a7a-4a39-b181-73d00c412b82");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("APin", "N", "Analog pin number", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Value", "V", "Value to send to pin", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "WriteAnalog Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int id = 1;
            double val = 0;

            if (!DA.GetData(0, ref id)) return;
            if (!DA.GetData(1, ref val)) return;

            DA.SetData(0, new ActionIOAnalog(id, val));
        }
    }

    public class TurnOn : GH_Component
    {
        public TurnOn() : base(
            "TurnOn",
            "TurnOn",
            "Turn digital output on. Alias for `WriteDigital(ioNum, true)`",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("8bd5bc0d-249e-4744-8530-cf8fced77492");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("DPin", "N", "Digital pin number", GH_ParamAccess.item, 1);
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

    public class TurnOff : GH_Component
    {
        public TurnOff() : base(
            "TurnOff",
            "TurnOff",
            "Turn digital output off. Alias for `WriteDigital(ioNum, false)`",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("15d234aa-2f63-488e-a95e-cc89ffcca3b6");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("APin", "N", "Analog pin number", GH_ParamAccess.item, 1);
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


}
