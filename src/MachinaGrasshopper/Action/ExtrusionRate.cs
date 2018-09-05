using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using MachinaGrasshopper.GH_Utils;
using Machina;

namespace MachinaGrasshopper.Action
{
    //  ███████╗██╗  ██╗████████╗██████╗ ██╗   ██╗███████╗██╗ ██████╗ ███╗   ██╗██████╗  █████╗ ████████╗███████╗
    //  ██╔════╝╚██╗██╔╝╚══██╔══╝██╔══██╗██║   ██║██╔════╝██║██╔═══██╗████╗  ██║██╔══██╗██╔══██╗╚══██╔══╝██╔════╝
    //  █████╗   ╚███╔╝    ██║   ██████╔╝██║   ██║███████╗██║██║   ██║██╔██╗ ██║██████╔╝███████║   ██║   █████╗  
    //  ██╔══╝   ██╔██╗    ██║   ██╔══██╗██║   ██║╚════██║██║██║   ██║██║╚██╗██║██╔══██╗██╔══██║   ██║   ██╔══╝  
    //  ███████╗██╔╝ ██╗   ██║   ██║  ██║╚██████╔╝███████║██║╚██████╔╝██║ ╚████║██║  ██║██║  ██║   ██║   ███████╗
    //  ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝
    //                                                                                                           
    public class ExtrusionRate : GH_MutableComponent
    {
        public ExtrusionRate() : base(
            "ExtrusionRate",
            "ExtrusionRate",
            "Changes the extrusion rate of filament for 3D printers.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.senary;
        public override Guid ComponentGuid => new Guid("c5d99548-65d8-4c5a-ba16-bb7543474013");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_ExtrusionRate;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Relative
            mpManager.AddComponentNames(true, "ExtrusionRate", "ExtrusionRate", "Increases the extrusion rate of filament for 3D printers.");
            mpManager.AddParameter(true, typeof(Param_Number), "RateInc", "RInt", "Increment of mm of filament per mm of movement", GH_ParamAccess.item);

            // Absolute
            mpManager.AddComponentNames(false, "ExtrusionRateTo", "ExtrusionRateTo", "Sets the extrusion rate of filament for 3D printers.");
            mpManager.AddParameter(false, typeof(Param_Number), "Rate", "R", "Value of mm of filament per mm of movement", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "ExtrusionRate Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double rate = 0;

            if (!DA.GetData(0, ref rate)) return;

            DA.SetData(0, new ActionExtrusionRate(rate, this.Relative));            
        }
    }

}
