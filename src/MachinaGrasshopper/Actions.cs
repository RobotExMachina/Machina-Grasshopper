using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Machina;
using System.Windows.Forms;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
using System.Collections.Generic;
using System.Reflection;

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



    public abstract class GH_MiddleWare : GH_Component
    {
        public abstract bool Foo { get; }

        public GH_MiddleWare(string s1, string s2, string s3, string s4, string s5) : base(s1, s2, s3, s4, s5) { }


    }

    public class NewMove : GH_MiddleWare
    {

        // This will be the base names that will be used on the component's description on the ribbon
        public NewMove() : base(
            "NewMove",  // the name that shows up on the tab, on yellow bar on toolip, on component on 'Draw Full Names'
            "NewMove",  // the name that shows up on the non-icon component with 'DFN' off, and in parenthesis after the main name on the yellow bar on tooltip
            "Moves the device to an absolute location or along a speficied vector relative to its current position.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("882d97de-6b55-468d-83f3-cbd03d62336f");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_PopSettings;
        
        public override bool Foo { get { return true;} }

        
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("msg", "M", "create a msg", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("greeting", "S", "the msg", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            if (!DA.GetData(0, ref name)) return;

            DA.SetData(0, $"Hello {name}!");
        }
    }




















    public abstract class MACHINA_AbsRelMutableComponent : GH_Component
    {
        /// <summary>
        /// Bounce construction one level up.
        /// </summary>
        public MACHINA_AbsRelMutableComponent(string name, string nickname, string description, string categories, string subCategory) :
            base(name, nickname, description, categories, subCategory)
        { }

        /// <summary>
        /// Relative Action?
        /// </summary>
        public bool Relative
        {
            get { return m_relative; }
            set
            {
                m_relative = value;
                rel_ticks++;
                this.Message = m_relative ? "Relative" : "Absolute";
                //this.ClearRuntimeMessages();  // "failed ot collect data from input" messages stay, pointing at all inputs... it is confusing...
                this.UpdateComponentNames();
                this.UpdateInputParameters();
                //this.UpdateInputNames();  // comes with UpdateInputParameters()
                this.ExpireSolution(true);
            }
        }
        private bool m_relative = false;
        protected int rel_ticks = 0;

        public List<string> debugMsgs = new List<string>();

        public MACHINA_MutableInputParamManager mpManager = new MACHINA_MutableInputParamManager();

        protected abstract bool ShallowInputMutation { get; }

        protected abstract void RegisterMutableInputParams(MACHINA_MutableInputParamManager mpManager);


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Let the user fill in the data
            this.RegisterMutableInputParams(this.mpManager);

            // If the inputs mutate shallowy (no real param replacement), check they share count and types
            if (this.ShallowInputMutation)
            {
                if (mpManager.inputs[true].Count != mpManager.inputs[false].Count)
                    //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Components with shallow mutable inputs must have the same number of inputs in both modes.");
                    debugMsgs.Add("Components with shallow mutable inputs must have the same number of inputs in both modes.");

                for (var i = 0; i < mpManager.inputs[true].Count; i++)
                {
                    if (mpManager.inputs[true][i].dataType != mpManager.inputs[false][i].dataType)
                        //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Components with shallow mutable inputs must have the same types in both modes.");
                        debugMsgs.Add("Components with shallow mutable inputs must have the same types in both modes.");
                }
            }

            //// Add each parameter using the mapped function from pManager
            //foreach (var input in mpManager.inputs[this.Relative]) AddParameterFunctionMap[input.dataType](pManager, input);

            foreach (var p in mpManager.inputs[this.Relative])
            {
                debugMsgs.Add($"Adding this input: {p} {p.dataType}");

                RegisterTypedParam(pManager, p);
            }

            //pManager.AddGenericParameter("Point", "P", "Target Point", GH_ParamAccess.item);

            // Do some tricks with the names of the mutable input (is this the right place to put this?)
            Grasshopper.CentralSettings.CanvasFullNamesChanged += OnCanvasFullNamesChanged;

            //this.UpdateComponentNames();  // this makes the component read MoveTo on the catergory tab
            //this.Relative = false;  // this too, because it incorporates .UpdateComponentNames()
            this.Message = this.Relative ? "Relative" : "Absolute";
        }

        //// This object MUST be static?? 
        //static Dictionary<Type, Func<GH_InputParamManager, MACHINA_InputParameProps, int>> AddParameterFunctionMap =
        //    new Dictionary<Type, Func<GH_InputParamManager, MACHINA_InputParameProps, int>>()
        //{
        //    //{ typeof (),                        (pm, p) => pm.AddAngleParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Arc),               (pm, p) => pm.AddArcParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Boolean),           (pm, p) => pm.AddBooleanParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Box),               (pm, p) => pm.AddBoxParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Brep),              (pm, p) => pm.AddBrepParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Circle),            (pm, p) => pm.AddCircleParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Colour),            (pm, p) => pm.AddColourParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Complex),           (pm, p) => pm.AddComplexNumberParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Culture),           (pm, p) => pm.AddCultureParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Curve),             (pm, p) => pm.AddCurveParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Field),             (pm, p) => pm.AddFieldParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_GenericObject),     (pm, p) => pm.AddGenericParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Geometry),          (pm, p) => pm.AddGeometryParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Group),             (pm, p) => pm.AddGroupParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Integer),           (pm, p) => pm.AddIntegerParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Interval2D),        (pm, p) => pm.AddInterval2DParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Interval),          (pm, p) => pm.AddIntervalParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Line),              (pm, p) => pm.AddLineParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Matrix),            (pm, p) => pm.AddMatrixParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_MeshFace),          (pm, p) => pm.AddMeshFaceParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Mesh),              (pm, p) => pm.AddMeshParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Number),            (pm, p) => pm.AddNumberParameter(p.name, p.nickname, p.description, p.access) },
        //    //{ typeof (),                        (pm, p) => pm.AddParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_FilePath),          (pm, p) => pm.AddPathParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Plane),             (pm, p) => pm.AddPlaneParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Point),             (pm, p) => pm.AddPointParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Rectangle),         (pm, p) => pm.AddRectangleParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_ScriptVariable),    (pm, p) => pm.AddScriptVariableParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Surface),           (pm, p) => pm.AddSurfaceParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_String),            (pm, p) => pm.AddTextParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Time),              (pm, p) => pm.AddTimeParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Time),              (pm, p) => pm.AddTransformParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Vector),            (pm, p) => pm.AddVectorParameter(p.name, p.nickname, p.description, p.access) }
        //};

        protected void RegisterTypedParam(GH_InputParamManager pManager, MACHINA_InputParameProps p)
        {
            //// Wanted to do this programmatically with a dictionary of delegates, but couldn't really make it work... :(
            //AddParameterFunctionMap[input.dataType](pManager, input);

            // I am so embarrased about having to do this... urgh X(
            if (p.dataType == typeof(Param_Point)) pManager.AddPointParameter(p.name, p.nickname, p.description, p.access);
            else if (p.dataType == typeof(Param_Vector)) pManager.AddVectorParameter(p.name, p.nickname, p.description, p.access);
            else if (p.dataType == typeof(Param_GenericObject)) pManager.AddGenericParameter(p.name, p.nickname, p.description, p.access);
            else if (p.dataType == typeof(Param_Number)) pManager.AddNumberParameter(p.name, p.nickname, p.description, p.access);
            else if (p.dataType == typeof(Param_Plane)) pManager.AddPlaneParameter(p.name, p.nickname, p.description, p.access);
        }


        protected void UnregisterAllInputs()
        {
            for (var i = this.Params.Input.Count - 1; i >= 0; i--)
            {
                var param = this.Params.Input[i];
                this.Params.UnregisterInputParameter(param, true);  // what is isolate??
            }
        }
        

        /// <summary>
        /// This was quite helpful: https://discourse.mcneel.com/t/replicating-explode-tree-bang-component-in-ghpython/39221/15
        /// </summary>
        protected void UpdateInputParameters()
        {
            //for (var i = this.Params.Input.Count - 1; i >= 0; i--)
            //{
            //    var param = this.Params.Input[i];
            //    this.Params.UnregisterInputParameter(param, true);  // what is isolate?
            //}

            //if (this.Relative)
            //{
            //    var vec = new Grasshopper.Kernel.Parameters.Param_Vector();
            //    vec.Access = GH_ParamAccess.item;
            //    this.Params.RegisterInputParam(vec);

            //    var ang = new Grasshopper.Kernel.Parameters.Param_Number();
            //    ang.Access = GH_ParamAccess.item;
            //    this.Params.RegisterInputParam(ang);
            //}
            //else
            //{
            //    var pln = new Grasshopper.Kernel.Parameters.Param_Plane();
            //    pln.Access = GH_ParamAccess.item;
            //    this.Params.RegisterInputParam(pln);

            //}

            //this.Params.OnParametersChanged();
            //this.UpdateInputNames();


            

            // If shallow mutation, skip to just changing names
            if (!this.ShallowInputMutation)
            {
                this.UnregisterAllInputs();

                var inputs = mpManager.inputs[this.Relative];
                foreach (var input in inputs)
                {
                    try
                    {
                        // Wow, this is abstract... https://stackoverflow.com/a/3255716/1934487 and https://stackoverflow.com/a/142362/1934487
                        ConstructorInfo ctor = input.dataType.GetConstructor(Type.EmptyTypes);
                        var p = ctor.Invoke(new object[0]) as IGH_Param;  // or null instead of new object[0]?
                        p.Access = input.access;
                        this.Params.RegisterInputParam(p);  // is p taken as IGH_Param or as its subclass due to ctor?
                    }
                    catch
                    {
                        //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong when registering inputs...");
                        debugMsgs.Add("Something went wrong when registering inputs...");
                    }

                }

                this.Params.OnParametersChanged();
            }
             

            this.UpdateInputNames();
        }

        


        /// <summary>
        /// Takes care of updating the input parameters' names according to the current abs/rel state.
        /// </summary>
        protected void UpdateInputNames()
        {
            //if (this.Relative)
            //{
            //    var vec = this.Params.Input[0];
            //    vec.Name = "Axis";
            //    vec.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Axis" : "V";
            //    vec.Description = "Rotation axis, with positive rotation direction is defined by the right-hand rule.";

            //    var ang = this.Params.Input[1];
            //    ang.Name = "Angle";
            //    ang.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Angle" : "A";
            //    ang.Description = "Rotation angle in degrees";
            //}
            //else
            //{
            //    var pln = this.Params.Input[0];
            //    pln.Name = "Plane";
            //    pln.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Plane" : "Pl";
            //    pln.Description = "Target spatial orientation";
            //}






            //if (this.Params.Input.Count != mpManager.inputs[this.Relative].Count)
            //    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong with UpdateInputNames");

            //debugMsgs.Add($"Starting name rev");
            //debugMsgs.Add($"input count {mpManager.inputs[this.Relative].Count}");
            int it = 0;
            foreach (var pProps in mpManager.inputs[this.Relative])
            {
                var p = this.Params.Input[it];
                //debugMsgs.Add($"adding {it} {p.Name}");
                p.Name = pProps.name;
                p.NickName = Grasshopper.CentralSettings.CanvasFullNames ? pProps.name : pProps.nickname;  // Workaround to the nicknames problem: https://discourse.mcneel.com/t/changing-input-parameter-names-always-shows-nickname/54071/3
                p.Description = pProps.description;
                //debugMsgs.Add($"Now {p.Name}");
                it++;
            }


            //// Since the input is Generic, no need to do Parameter changes, just change its face
            //var param = this.Params.Input[0];
            //if (this.Relative)
            //{
            //    param.Name = "Vector";
            //    param.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Vector" : "V";  // Only nicknames will show up after rename, so a little trick here
            //    param.Description = "Translation Vector";
            //}
            //else
            //{
            //    param.Name = "Point";
            //    param.NickName = Grasshopper.CentralSettings.CanvasFullNames ? "Point" : "P";
            //    param.Description = "Target Point";
            //}

        }

        /// <summary>
        /// Takes care of updating the component name on the document to match the Core API depending on abs/rel state.
        /// </summary>
        protected void UpdateComponentNames()
        {
            
            var label = mpManager.componentNames[this.Relative];

            if (label != null)
            {
                this.Name = label.name;
                this.NickName = label.nickname;
                this.Description = label.description;
            }
        }

        // Serialize the Relative attribute for correct
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Relative", Relative);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            Relative = reader.GetBoolean("Relative");
            return base.Read(reader);
        }


        /// <summary>
        /// Trigger change in the face of the component name to match the Machina Core API. 
        /// </summary>
        /// <param name="document"></param>
        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);
            this.UpdateComponentNames();
        }



        /// <summary>
        /// Add the Rel/Abs option tag. Is executed on first right-click on the component.
        /// </summary>
        /// <param name="menu"></param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            var item = Menu_AppendItem(menu, "Relative Action", AbsoluteToggle, null, true, this.Relative);
            item.ToolTipText = "Should the input be taken as absolute coordinates or relative motion?";
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

            this.RecordUndoEvent("Relative");
            this.Relative = !this.Relative;
        }

        /// <summary>
        /// A workaround to the nicknames problem: https://discourse.mcneel.com/t/changing-input-parameter-names-always-shows-nickname/54071/3
        /// </summary>
        private void OnCanvasFullNamesChanged() => this.UpdateInputNames();

    }















    //  ███╗   ███╗ ██████╗ ██╗   ██╗███████╗
    //  ████╗ ████║██╔═══██╗██║   ██║██╔════╝
    //  ██╔████╔██║██║   ██║██║   ██║█████╗  
    //  ██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██╔══╝  
    //  ██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ███████╗
    //  ╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚══════╝
    //                                       
    public class Move : MACHINA_AbsRelMutableComponent
    {

        /// <summary>
        /// Relative Action?
        /// </summary>
        //private bool relative = false;  // moved to base class

        // This will be the base names that will be used on the component's description on the ribbon
        public Move() : base(
            "Move",  // the name that shows up on the tab, on yellow bar on toolip, on component on 'Draw Full Names'
            "Move",  // the name that shows up on the non-icon component with 'DFN' off, and in parenthesis after the main name on the yellow bar on tooltip
            "Moves the device to an absolute location or along a speficied vector relative to its current position.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("b028192a-e2d1-449e-899d-a79a16a8de3e");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Move;

        protected override bool ShallowInputMutation => true;  // parameters will not change (and wires not disconnect), only change the names

        protected override void RegisterMutableInputParams(MACHINA_MutableInputParamManager mpManager)
        {
            // Relative
            mpManager.AddComponentNames(false, "MoveTo", "MoveTo", "Moves the device to an absolute location.");
            mpManager.AddParameter(false, typeof(Param_GenericObject), "Point", "P", "Target Point.", GH_ParamAccess.item);

            // Absolute
            mpManager.AddComponentNames(true, "Move", "Move", "Moves the device along a speficied vector relative to its current position.");
            mpManager.AddParameter(true, typeof(Param_GenericObject), "Vector", "V", "Translation Vector.", GH_ParamAccess.item);

            //// Relative
            //mpManager.AddComponentNames(false, "RotateTo", "RotateTo", "Rotates the device to a specified orientation.");
            //mpManager.AddParameter(false, typeof(Param_Plane), "Plane", "Pl", "Target spatial orientation.", GH_ParamAccess.item);

            //// Absolute
            //mpManager.AddComponentNames(true, "Rotate", "Rotate", "Rotates the device a specified angle in degrees along the specified vector.");
            //mpManager.AddParameter(true, typeof(Param_Vector), "Axis", "V", "Rotation axis, with positive rotation direction is defined by the right-hand rule.", GH_ParamAccess.item);
            //mpManager.AddParameter(true, typeof(Param_Number), "Angle", "A", "Rotation angle in degrees.", GH_ParamAccess.item);
        }


        
        //protected override void RegisterInputParams(GH_InputParamManager pManager)
        //{
        //    pManager.AddGenericParameter("Point", "P", "Target Point", GH_ParamAccess.item);

        //    // Do some tricks with the names of the mutable input (is this the right place to put this?)
        //    Grasshopper.CentralSettings.CanvasFullNamesChanged += OnCanvasFullNamesChanged;

        //    //this.UpdateComponentNames();  // this makes the component read MoveTo on the catergory tab
        //    this.Relative = false;
        //}

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Move Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.ClearRuntimeMessages();

            object obj = null;

            if (!DA.GetData(0, ref obj)) return;
            if (obj == null) return;

            // Be verbose about data types
            if (obj is GH_Point)
            {
                if (this.Relative)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "WARNING: using Point in Relative mode, did you mean to work in Absolute mode instead? This Move action will take the Point as a Vector for relative motion.");
                }
                GH_Point p = obj as GH_Point;
                DA.SetData(0, new ActionTranslation(new Machina.Vector(p.Value.X, p.Value.Y, p.Value.Z), this.Relative));
            }

            else if (obj is GH_Plane)
            {
                if (this.Relative)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "WARNING: using Plane in Relative mode, did you mean to work in Absolute mode instead? This Move action will take the Plane's origin Point as a Vector for relative motion.");
                }
                GH_Plane p = obj as GH_Plane;
                DA.SetData(0, new ActionTranslation(new Machina.Vector(p.Value.OriginX, p.Value.OriginY, p.Value.OriginZ), this.Relative));
            }

            else if (obj is GH_Vector)
            {
                if (!this.Relative)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "WARNING: using Vector in Absolute mode, did you mean to work in Relative mode instead? This Move action will take the Vector's coordinates as the target Point for absolute motion.");
                }
                GH_Vector p = obj as GH_Vector;
                DA.SetData(0, new ActionTranslation(new Machina.Vector(p.Value.X, p.Value.Y, p.Value.Z), this.Relative));
            }

            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"ERROR: Can't take {obj.GetType()} as argument for a Move action; please use Point, Vector or Plane.");
                //DA.SetData(0, null);  // not necessary
            }

            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"Hi there! {this.RunCount} ticks: {rel_ticks}");

            foreach (string str in debugMsgs)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, str);
            }
        }
















        

        

        
    }





    ////  ██████╗  ██████╗ ████████╗ █████╗ ████████╗███████╗
    ////  ██╔══██╗██╔═══██╗╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
    ////  ██████╔╝██║   ██║   ██║   ███████║   ██║   █████╗  
    ////  ██╔══██╗██║   ██║   ██║   ██╔══██║   ██║   ██╔══╝  
    ////  ██║  ██║╚██████╔╝   ██║   ██║  ██║   ██║   ███████╗
    ////  ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    ////                                                     
    //public class Rotate : MACHINA_AbsRelMutableComponent
    //{
    //    /// <summary>
    //    /// Relative Action?
    //    /// </summary>
    //    //private bool relative = false;


    //    public Rotate() : base(
    //        "Rotate",
    //        "Rotate",
    //        "Rotates the device to a specified orientation, or a specified angle in degrees along the specified vector.",
    //        "Machina",
    //        "Actions")
    //    { }
    //    public override GH_Exposure Exposure => GH_Exposure.primary;
    //    public override Guid ComponentGuid => new Guid("c48d908d-3e0d-4600-90de-1330b9dc7973");
    //    protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Rotate;

    //    protected override void RegisterMutableInputParams(MACHINA_MutableInputParamManager mpManager)
    //    {
    //        // Relative
    //        mpManager.AddComponentNames(false, "RotateTo", "RotateTo", "Rotates the device to a specified orientation.");
    //        mpManager.AddParameter(false, typeof(Param_Plane), "Plane", "Pl", "Target spatial orientation.", GH_ParamAccess.item);

    //        // Absolute
    //        mpManager.AddComponentNames(true, "Rotate", "Rotate", "Rotates the device a specified angle in degrees along the specified vector.");
    //        mpManager.AddParameter(true, typeof(Param_Vector), "Axis", "V", "Rotation axis, with positive rotation direction is defined by the right-hand rule.", GH_ParamAccess.item);
    //        mpManager.AddParameter(true, typeof(Param_Number), "Angle", "A", "Rotation angle in degrees.", GH_ParamAccess.item);

    //    }

    //    protected override bool ShallowInputMutation() => false;  // parameters will not change (and wires not disconnected), only change the names

    //    //protected override void RegisterInputParams(GH_InputParamManager pManager)
    //    //{
    //    //    pManager.AddPlaneParameter("Plane", "P", "Target spatial orientation", GH_ParamAccess.item);

    //    //    Grasshopper.CentralSettings.CanvasFullNamesChanged += OnCanvasFullNamesChanged;
    //    //    this.UpdateMessage();
    //    //}

    //    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    //    {
    //        pManager.AddGenericParameter("Action", "A", "Rotate Action", GH_ParamAccess.item);
    //    }

    //    protected override void SolveInstance(IGH_DataAccess DA)
    //    {
    //        if (this.Relative)
    //        {
    //            Vector3d v = Vector3d.Zero;
    //            double ang = 0;

    //            if (!DA.GetData(0, ref v)) return;
    //            if (!DA.GetData(1, ref ang)) return;

    //            DA.SetData(0, new ActionRotation(new Machina.Rotation(v.X, v.Y, v.Z, ang), true));
    //        }
    //        else
    //        {
    //            Plane pl = Plane.Unset;

    //            if (!DA.GetData(0, ref pl)) return;

    //            DA.SetData(0, new ActionRotation(new Machina.Orientation(pl.XAxis.X, pl.XAxis.Y, pl.XAxis.Z, pl.YAxis.X, pl.YAxis.Y, pl.YAxis.Z), false));
    //        }
    //    }





    //}





































































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
        public override GH_Exposure Exposure => GH_Exposure.primary;
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
