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
    //  ███╗   ███╗ ██████╗ ██╗   ██╗███████╗
    //  ████╗ ████║██╔═══██╗██║   ██║██╔════╝
    //  ██╔████╔██║██║   ██║██║   ██║█████╗  
    //  ██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██╔══╝  
    //  ██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ███████╗
    //  ╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚══════╝
    //                                       
    public class Move : GH_MutableComponent
    {

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

        // You must specify if this component will mutate shallowly (inputs are not replaced, only renamed)
        // or deeply (wires are disconnected, inputs replaced and names changed).
        protected override bool ShallowInputMutation => true;

        // For mutable components, register parameters using `RegisterMutableInputParams()` instead
        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "MoveTo", "MoveTo", "Moves the device to an absolute location.");
            mpManager.AddParameter(false, typeof(Param_GenericObject), "Point", "P", "Target Point.", GH_ParamAccess.item);

            // Relative
            mpManager.AddComponentNames(true, "Move", "Move", "Moves the device along a speficied vector relative to its current position.");
            mpManager.AddParameter(true, typeof(Param_GenericObject), "Vector", "V", "Translation Vector.", GH_ParamAccess.item);
        }

        // For outputs, keep using the base `RegisterOutputParams()`
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Move Action", GH_ParamAccess.item);
        }

        // Implement `Solveinstance` as usual
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //this.ClearRuntimeMessages();

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
            
        }

    }
}
