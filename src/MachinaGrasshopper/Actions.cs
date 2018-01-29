using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Machina;
using System.Windows.Forms;

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




    //  ███╗   ███╗ ██████╗ ██╗   ██╗███████╗
    //  ████╗ ████║██╔═══██╗██║   ██║██╔════╝
    //  ██╔████╔██║██║   ██║██║   ██║█████╗  
    //  ██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██╔══╝  
    //  ██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ███████╗
    //  ╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚══════╝
    //                                       
    public class Move : GH_Component//, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Relative Action?
        /// </summary>
        private bool relative = false;
        
        public Move() : base(
            "Move",
            "Move",
            "Moves the device to an absolute location or along a speficied vector relative to its current position.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("b028192a-e2d1-449e-899d-a79a16a8de3e");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Move;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Point", "P", "Target Point", GH_ParamAccess.item);

            // Do some tricks with the names of the mutable input (is this the right place to put this?)
            Grasshopper.CentralSettings.CanvasFullNamesChanged += OnCanvasFullNamesChanged;
            this.UpdateMessage();
        }
        
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Move Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object obj = null;

            if (!DA.GetData(0, ref obj)) return;
            if (obj == null) return;
           
            // Be verbose about data types
            if (obj is GH_Point)
            {
                if (this.relative)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "WARNING: using Point in Relative mode, did you mean to work in Absolute mode instead? This Move action will take the Point as a Vector for relative motion.");
                }
                GH_Point p = obj as GH_Point;
                DA.SetData(0, new ActionTranslation(new Machina.Vector(p.Value.X, p.Value.Y, p.Value.Z), this.relative));
            }

            else if (obj is GH_Plane)
            {
                if (this.relative)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "WARNING: using Plane in Relative mode, did you mean to work in Absolute mode instead? This Move action will take the Plane's origin Point as a Vector for relative motion.");
                }
                GH_Plane p = obj as GH_Plane;
                DA.SetData(0, new ActionTranslation(new Machina.Vector(p.Value.OriginX, p.Value.OriginY, p.Value.OriginZ), this.relative));
            }

            else if (obj is GH_Vector)
            {
                if (!this.relative)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "WARNING: using Vector in Absolute mode, did you mean to work in Relative mode instead? This Move action will take the Vector's coordinates as the target Point for absolute motion.");
                }
                GH_Vector p = obj as GH_Vector;
                DA.SetData(0, new ActionTranslation(new Machina.Vector(p.Value.X, p.Value.Y, p.Value.Z), this.relative));
            }

            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"ERROR: Can't take {obj.GetType()} as argument for a Move action; please use Point, Vector or Plane.");
                //DA.SetData(0, null);  // not necessary
            }

        }
        
        /// <summary>
        /// Add the Rel/Abs option tag
        /// </summary>
        /// <param name="menu"></param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            var checkbox = Menu_AppendItem(menu, "Relative Action", AbsoluteToggle, null, true, this.relative);
            checkbox.ToolTipText = "Should the input be taken as absolute coordinates or relative motion?";
        }

        /// <summary>
        /// Event handler for the menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AbsoluteToggle(Object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            //item.Checked = !item.Checked;  // no need for the Check (and I don't even think it worked), the WPF element is linked to the variable
            this.relative = !this.relative;
            
            // Update (and redraw? this component
            this.UpdateInputNames();
            this.UpdateMessage();
            this.ExpireSolution(true);
        }

        /// <summary>
        /// Update the message tab under the component
        /// </summary>
        protected void UpdateMessage() => this.Message = this.relative ? "Relative" : "Absolute";

        /// <summary>
        /// A workaround to the nicknames problem: https://discourse.mcneel.com/t/changing-input-parameter-names-always-shows-nickname/54071/3
        /// </summary>
        private void OnCanvasFullNamesChanged() => UpdateInputNames();

        /// <summary>
        /// Takes care of updating the input to the correct name
        /// </summary>
        protected void UpdateInputNames()
        {
            // Since the input is Generic, no need to do Parameter changes, just change its face
            var param = this.Params.Input[0];
            if (this.relative)
            {
                param.Name = "Vector";
                param.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Vector" : "V";  // Only nicknames will show up after rename, so a little trick here
                param.Description = "Translation Vector";
            }
            else
            {
                param.Name = "Point";
                param.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Point" : "P";
                param.Description = "Target Point";
            }
        }

        
        // THIS IS FOR COMPONENTS WHERE THE USER CAN ADD PARAMETERS MANUALY (ZOOM IN AND STUFF) 

        //public bool CanInsertParameter(GH_ParameterSide side, int index)
        //{
        //    // Users cannot manually add params
        //    return false;
        //}

        //public bool CanRemoveParameter(GH_ParameterSide side, int index)
        //{
        //    // Users cannot manually remove params
        //    return false;
        //}

        //public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        //{
        //    //throw new NotImplementedException();
        //    //this.Params.RegisterInputParam()
        //    var p = new Grasshopper.Kernel.Parameters.Param_Point();
        //    p.Name = "P";
        //    p.NickName = "Point";
        //    p.Description = "fooabrbaz";
        //    p.Access = GH_ParamAccess.item;
        //    this.Params.RegisterInputParam(p, index);
        //    return null;
        //}

        //public bool DestroyParameter(GH_ParameterSide side, int index)
        //{
        //    //throw new NotImplementedException();
        //    return true;
        //}

        //public void VariableParameterMaintenance()
        //{
        //    //throw new NotImplementedException();
        //}
    }












































































    //  ███╗   ███╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗
    //  ████╗ ████║██╔═══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║
    //  ██╔████╔██║██║   ██║   ██║   ██║██║   ██║██╔██╗ ██║
    //  ██║╚██╔╝██║██║   ██║   ██║   ██║██║   ██║██║╚██╗██║
    //  ██║ ╚═╝ ██║╚██████╔╝   ██║   ██║╚██████╔╝██║ ╚████║
    //  ╚═╝     ╚═╝ ╚═════╝    ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
    //                                                     
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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_MotionType;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("f60026ed-e66f-4cba-8592-5d3efc9362bf");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_PushSettings;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("82a53cd1-c33c-4cfa-907c-94288058130e");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_PopSettings;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("4ec5c686-0ca9-4b60-a99e-8eaf4fe594ac");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Wait;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("2675e57a-5b6f-441a-9f94-69bb155b7b59");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Message;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("a3fc9af6-04ab-49e9-a0fe-d224f4e7e9bf");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Comment;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("5598bf85-6887-40b4-a29b-efff6af0864f");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Attach;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("f3195b55-742a-429f-bf66-94fce5497bc9");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Detach;

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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("a08ed4f1-1913-4f32-8d43-0c98fd1e5bd4");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_WriteDigital;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("DigitalPinNumber", "N", "Digital pin number", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("On", "ON", "Turn on?", GH_ParamAccess.item, false);
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
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        public override Guid ComponentGuid => new Guid("ace7ecb7-2a7a-4a39-b181-73d00c412b82");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_WriteAnalog;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("AnalogPinNumber", "N", "Analog pin number", GH_ParamAccess.item, 1);
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


}
