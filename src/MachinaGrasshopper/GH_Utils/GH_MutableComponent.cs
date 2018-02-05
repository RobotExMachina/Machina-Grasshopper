using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
using System.Drawing;
using System.Globalization;

namespace MachinaGrasshopper.GH_Utils
{
    //  ██████╗ ███████╗██╗         ██╗ █████╗ ██████╗ ███████╗                                   
    //  ██╔══██╗██╔════╝██║        ██╔╝██╔══██╗██╔══██╗██╔════╝                                   
    //  ██████╔╝█████╗  ██║       ██╔╝ ███████║██████╔╝███████╗                                   
    //  ██╔══██╗██╔══╝  ██║      ██╔╝  ██╔══██║██╔══██╗╚════██║                                   
    //  ██║  ██║███████╗███████╗██╔╝   ██║  ██║██████╔╝███████║                                   
    //  ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝    ╚═╝  ╚═╝╚═════╝ ╚══════╝                                   
    //                                                                                            
    //   ██████╗ ██████╗ ███╗   ███╗██████╗  ██████╗ ███╗   ██╗███████╗███╗   ██╗████████╗███████╗
    //  ██╔════╝██╔═══██╗████╗ ████║██╔══██╗██╔═══██╗████╗  ██║██╔════╝████╗  ██║╚══██╔══╝██╔════╝
    //  ██║     ██║   ██║██╔████╔██║██████╔╝██║   ██║██╔██╗ ██║█████╗  ██╔██╗ ██║   ██║   ███████╗
    //  ██║     ██║   ██║██║╚██╔╝██║██╔═══╝ ██║   ██║██║╚██╗██║██╔══╝  ██║╚██╗██║   ██║   ╚════██║
    //  ╚██████╗╚██████╔╝██║ ╚═╝ ██║██║     ╚██████╔╝██║ ╚████║███████╗██║ ╚████║   ██║   ███████║
    //   ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝      ╚═════╝ ╚═╝  ╚═══╝╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝
    //                                                                                            
    /// <summary>
    /// A middleware class for components with Relative/Absolute states. 
    /// Takes care of muting it based on state, i.e. dynamically replacing inputs and changing the component's name to match the Core API.
    /// Provides a custom set of overrides to deal with these situations.
    /// <see cref="Move.Move"/>
    /// </summary>
    public abstract class GH_MutableComponent : GH_Component
    {
        /// <summary>
        /// Bounce construction one level up.
        /// </summary>
        public GH_MutableComponent(string name, string nickname, string description, string categories, string subCategory) :
            base(name, nickname, description, categories, subCategory)
        { }

        /// <summary>
        /// Is this Component in Relative mode? Bundles a bunch of component updates on the setter.
        /// </summary>
        public bool Relative
        {
            get { return m_relative; }
            set
            {
                m_relative = value;
                this.Message = m_relative ? "Relative" : "Absolute";
                //this.ClearRuntimeMessages();  // "failed ot collect data from input" messages stay, pointing at all inputs... it is confusing...
                this.UpdateComponentNames();
                this.UpdateInputParameters();
                //this.UpdateInputNames();  // comes with UpdateInputParameters()
                this.ExpireSolution(true);
            }
        }
        private bool m_relative = false;

        /// <summary>
        /// Temp debugging msg list, be displayed after recompute of solution
        /// </summary>
        public List<string> debugMsgs = new List<string>();  

        /// <summary>
        /// A custom parameter manager to keep track of mutable inputs and compoennt names
        /// </summary>
        public GH_MutableInputParamManager mpManager = new GH_MutableInputParamManager();

        /// <summary>
        /// Is component's mutation shallow? If true, input parameters won't really change, only get names rewritten. 
        /// If false, input parameters will be replaced upon mode switch, with wires unplugging.
        /// </summary>
        protected abstract bool ShallowInputMutation { get; }

        /// <summary>
        /// This component's flavor of `RegisterInputParams`, to be used by the component to populate inputs in both states.
        /// Use the `mpManager` `AddComponentNames` and `AddParameter` methods. 
        /// </summary>
        /// <param name="mpManager"></param>
        protected abstract void RegisterMutableInputParams(GH_MutableInputParamManager mpManager);

        /// <summary>
        /// The local override, that initializes the component with the parameters specified in the `RegisterMutableInputParams` override. 
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Let the user fill in the data
            this.RegisterMutableInputParams(this.mpManager);

            // If the inputs mutate shallowy (no real param replacement), check they share count and types
            if (this.ShallowInputMutation)
            {
                if (mpManager.inputs[true].Count != mpManager.inputs[false].Count)
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Components with shallow mutable inputs must have the same number of inputs in both modes.");
                    //debugMsgs.Add("Components with shallow mutable inputs must have the same number of inputs in both modes.");

                for (var i = 0; i < mpManager.inputs[true].Count; i++)
                {
                    if (mpManager.inputs[true][i].dataType != mpManager.inputs[false][i].dataType)
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Components with shallow mutable inputs must have the same types in both modes.");
                    //debugMsgs.Add("Components with shallow mutable inputs must have the same types in both modes.");
                }
            }

            // Y U NO WORK?!?!
            //// Add each parameter using the mapped function from pManager
            //foreach (var input in mpManager.inputs[this.Relative]) AddParameterFunctionMap[input.dataType](pManager, input);

            // Please don't look at this... 
            foreach (var p in mpManager.inputs[this.Relative]) RegisterTypedParam(pManager, p); 
            

            // Do some tricks with the names of the mutable input (is this the right place to put this?)
            Grasshopper.CentralSettings.CanvasFullNamesChanged += OnCanvasFullNamesChanged;

            //this.UpdateComponentNames();  // this makes the component read the first replaced name (like "MoveTo" instead of "Move") on the catergory tab
            //this.Relative = false;  // this too, because it incorporates .UpdateComponentNames()

            this.Message = this.Relative ? "Relative" : "Absolute";  // so just do it manually
        }


        // :(
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
        //    { typeof (Param_Transform),         (pm, p) => pm.AddTransformParameter(p.name, p.nickname, p.description, p.access) },
        //    { typeof (Param_Vector),            (pm, p) => pm.AddVectorParameter(p.name, p.nickname, p.description, p.access) }
        //};

        //// Wanted to do this programmatically with a dictionary of delegates, but couldn't really make it work... :(
        //AddParameterFunctionMap[input.dataType](pManager, input);
        protected void RegisterTypedParam(GH_InputParamManager pManager, GH_InputParamProps p)
        {
            // I am so embarrased about having to do this... urgh X(
            if (p.defaultValue == null)
            {
                try
                {
                    // Sorted by popularity order, estimated by my guts
                    if (p.dataType == typeof(Param_Number)) pManager.AddNumberParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Plane)) pManager.AddPlaneParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Point)) pManager.AddPointParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Vector)) pManager.AddVectorParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_String)) pManager.AddTextParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Boolean)) pManager.AddBooleanParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_GenericObject)) pManager.AddGenericParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Integer)) pManager.AddIntegerParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_FilePath)) pManager.AddPathParameter(p.name, p.nickname, p.description, p.access);

                    // I don't think Machina uses any of these so far... 
                    else if (p.dataType == typeof(Param_Arc)) pManager.AddArcParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Box)) pManager.AddBoxParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Brep)) pManager.AddBrepParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Circle)) pManager.AddCircleParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Colour)) pManager.AddColourParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Complex)) pManager.AddComplexNumberParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Culture)) pManager.AddCultureParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Curve)) pManager.AddCurveParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Field)) pManager.AddFieldParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Geometry)) pManager.AddGeometryParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Group)) pManager.AddGroupParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Interval2D)) pManager.AddInterval2DParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Interval)) pManager.AddIntervalParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Line)) pManager.AddLineParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Matrix)) pManager.AddMatrixParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_MeshFace)) pManager.AddMeshFaceParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Mesh)) pManager.AddMeshParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Rectangle)) pManager.AddRectangleParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_ScriptVariable)) pManager.AddScriptVariableParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Surface)) pManager.AddSurfaceParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Time)) pManager.AddTimeParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Transform)) pManager.AddTransformParameter(p.name, p.nickname, p.description, p.access);

                    else this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot register parameter of type " + p.dataType);
                }
                catch
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Something went wrong registering input parameter {p.dataType}");
                }
            } 
            else
            {
                try
                {
                    // Sorted by popularity order, estimated by my guts
                    if (p.dataType == typeof(Param_Number)) pManager.AddNumberParameter(p.name, p.nickname, p.description, p.access, (double) p.defaultValue);
                    else if (p.dataType == typeof(Param_Plane)) pManager.AddPlaneParameter(p.name, p.nickname, p.description, p.access, (Plane) p.defaultValue);
                    else if (p.dataType == typeof(Param_Point)) pManager.AddPointParameter(p.name, p.nickname, p.description, p.access, (Point3d) p.defaultValue);
                    else if (p.dataType == typeof(Param_Vector)) pManager.AddVectorParameter(p.name, p.nickname, p.description, p.access, (Vector3d) p.defaultValue);
                    else if (p.dataType == typeof(Param_String)) pManager.AddTextParameter(p.name, p.nickname, p.description, p.access, (string) p.defaultValue);
                    else if (p.dataType == typeof(Param_Boolean)) pManager.AddBooleanParameter(p.name, p.nickname, p.description, p.access, (bool) p.defaultValue);
                    else if (p.dataType == typeof(Param_GenericObject)) pManager.AddGenericParameter(p.name, p.nickname, p.description, p.access);  // generic objects don't accept default values...
                    else if (p.dataType == typeof(Param_Integer)) pManager.AddIntegerParameter(p.name, p.nickname, p.description, p.access, (int)p.defaultValue);
                    else if (p.dataType == typeof(Param_FilePath)) pManager.AddPathParameter(p.name, p.nickname, p.description, p.access);  // no def value accepted

                    // I don't think Machina uses any of these so far... 
                    else if (p.dataType == typeof(Param_Arc)) pManager.AddArcParameter(p.name, p.nickname, p.description, p.access, (Arc) p.defaultValue);
                    else if (p.dataType == typeof(Param_Box)) pManager.AddBoxParameter(p.name, p.nickname, p.description, p.access, (Box) p.defaultValue);
                    else if (p.dataType == typeof(Param_Brep)) pManager.AddBrepParameter(p.name, p.nickname, p.description, p.access);  // no def value accepted
                    else if (p.dataType == typeof(Param_Circle)) pManager.AddCircleParameter(p.name, p.nickname, p.description, p.access, (Circle) p.defaultValue);
                    else if (p.dataType == typeof(Param_Colour)) pManager.AddColourParameter(p.name, p.nickname, p.description, p.access, (Color) p.defaultValue);
                    else if (p.dataType == typeof(Param_Complex)) pManager.AddComplexNumberParameter(p.name, p.nickname, p.description, p.access, (GH_ComplexNumber)p.defaultValue);
                    else if (p.dataType == typeof(Param_Culture)) pManager.AddCultureParameter(p.name, p.nickname, p.description, p.access, (CultureInfo)p.defaultValue);
                    else if (p.dataType == typeof(Param_Curve)) pManager.AddCurveParameter(p.name, p.nickname, p.description, p.access);  // no def value
                    else if (p.dataType == typeof(Param_Field)) pManager.AddFieldParameter(p.name, p.nickname, p.description, p.access);  // no def val 
                    else if (p.dataType == typeof(Param_Geometry)) pManager.AddGeometryParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Group)) pManager.AddGroupParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Interval2D)) pManager.AddInterval2DParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Interval)) pManager.AddIntervalParameter(p.name, p.nickname, p.description, p.access, (Interval) p.defaultValue);
                    else if (p.dataType == typeof(Param_Line)) pManager.AddLineParameter(p.name, p.nickname, p.description, p.access, (Line) p.defaultValue);
                    else if (p.dataType == typeof(Param_Matrix)) pManager.AddMatrixParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_MeshFace)) pManager.AddMeshFaceParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Mesh)) pManager.AddMeshParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Rectangle)) pManager.AddRectangleParameter(p.name, p.nickname, p.description, p.access, (Rectangle3d) p.defaultValue);
                    else if (p.dataType == typeof(Param_ScriptVariable)) pManager.AddScriptVariableParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Surface)) pManager.AddSurfaceParameter(p.name, p.nickname, p.description, p.access);
                    else if (p.dataType == typeof(Param_Time)) pManager.AddTimeParameter(p.name, p.nickname, p.description, p.access, (DateTime) p.defaultValue);
                    else if (p.dataType == typeof(Param_Transform)) pManager.AddTransformParameter(p.name, p.nickname, p.description, p.access);

                    else this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot register parameter of type " + p.dataType);
                }
                catch
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Something went wrong registering input parameter {p.dataType} with value {p.defaultValue}");
                }
            }

        }

        /// <summary>
        /// Unregister all input parameters from this component.
        /// </summary>
        protected void UnregisterAllInputs()
        {
            for (var i = this.Params.Input.Count - 1; i >= 0; i--)
            {
                var param = this.Params.Input[i];
                this.Params.UnregisterInputParameter(param, true);  // what is isolate??
            }
        }


        /// <summary>
        /// Replaces/renames input parameters according to current rel mode. 
        /// </summary>
        /// <remarks><see href="https://discourse.mcneel.com/t/replicating-explode-tree-bang-component-in-ghpython/39221/15">This</see> was quite helpful.</remarks>
        protected void UpdateInputParameters()
        {
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
                        this.Params.RegisterInputParam(p);  // is p taken as IGH_Param or as its subclass due to ctor? --> Looks like it gets the deep type
                    }
                    catch
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong when registering inputs...");
                        //debugMsgs.Add("Something went wrong when registering inputs...");
                    }
                }

                // Notify the component of the changes 
                this.Params.OnParametersChanged();
            }

            // Update names according to mpManager data
            this.UpdateInputNames();
        }




        /// <summary>
        /// Updates the input parameters' names according to the current abs/rel state.
        /// </summary>
        protected void UpdateInputNames()
        {
            // Some sanity
            if (this.Params.Input.Count != mpManager.inputs[this.Relative].Count)
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong with UpdateInputNames");

            // Take each input and replace namings
            int it = 0;
            foreach (var pProps in mpManager.inputs[this.Relative])
            {
                var p = this.Params.Input[it];
                p.Name = pProps.name;
                p.Description = pProps.description;
                // Workaround to the nicknames problem: https://discourse.mcneel.com/t/changing-input-parameter-names-always-shows-nickname/54071/3
                p.NickName = Grasshopper.CentralSettings.CanvasFullNames ? pProps.name : pProps.nickname; 
                it++;
            }
        }

        /// <summary>
        /// Updates the component's name on the document to match the Core API depending on abs/rel state.
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

        // Serialize the Relative attribute for correct load/save
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
        /// Add the Rel/Abs option menu item. Is executed on first right-click on the component.
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

        /// <summary>
        /// A quick helper for debugging
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            foreach (string str in debugMsgs)this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, str);
        }
    }
}
