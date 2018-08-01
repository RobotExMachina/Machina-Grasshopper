using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;

using Machina;
using MachinaGrasshopper.GH_Utils;

namespace MachinaGrasshopper.Actions
{
    //  ███████╗██╗  ██╗████████╗███████╗██████╗ ███╗   ██╗ █████╗ ██╗      █████╗ ██╗  ██╗███████╗███████╗
    //  ██╔════╝╚██╗██╔╝╚══██╔══╝██╔════╝██╔══██╗████╗  ██║██╔══██╗██║     ██╔══██╗╚██╗██╔╝██╔════╝██╔════╝
    //  █████╗   ╚███╔╝    ██║   █████╗  ██████╔╝██╔██╗ ██║███████║██║     ███████║ ╚███╔╝ █████╗  ███████╗
    //  ██╔══╝   ██╔██╗    ██║   ██╔══╝  ██╔══██╗██║╚██╗██║██╔══██║██║     ██╔══██║ ██╔██╗ ██╔══╝  ╚════██║
    //  ███████╗██╔╝ ██╗   ██║   ███████╗██║  ██║██║ ╚████║██║  ██║███████╗██║  ██║██╔╝ ██╗███████╗███████║
    //  ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝
    //                                                                                                     
    public class ExternalAxes : GH_MutableComponent
    {
        public ExternalAxes() : base(
            "ExternalAxes",
            "ExternalAxes",
            "Change the values of the robot's external axes. Values expressed in degrees or milimeters, depending on the nature of the external axis. Use null for inactive axes. Note that the effect of this change of external axis will go in effect on the next motion Action.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("b304ce32-0ab1-4d8e-b20a-857b6fc1a55e");
        protected override System.Drawing.Bitmap Icon => null;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "ExternalAxesTo", "ExternalAxesTo", "Set the values of the robot's external axes.");
            mpManager.AddParameter(false, typeof(Param_Number), "ExternalAxis1", "EA1", "Value of external axis 1. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(false, typeof(Param_Number), "ExternalAxis2", "EA2", "Value of external axis 2. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(false, typeof(Param_Number), "ExternalAxis3", "EA3", "Value of external axis 3. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(false, typeof(Param_Number), "ExternalAxis4", "EA4", "Value of external axis 4. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(false, typeof(Param_Number), "ExternalAxis5", "EA5", "Value of external axis 5. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(false, typeof(Param_Number), "ExternalAxis6", "EA6", "Value of external axis 6. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);

            // Relative
            mpManager.AddComponentNames(true, "ExternalAxes", "ExternalAxes", "Increase the values of the robot's external axes.");
            mpManager.AddParameter(true, typeof(Param_Number), "ExternalAxis1", "EA1", "Increment on external axis 1. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(true, typeof(Param_Number), "ExternalAxis2", "EA2", "Increment on external axis 2. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(true, typeof(Param_Number), "ExternalAxis3", "EA3", "Increment on external axis 3. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(true, typeof(Param_Number), "ExternalAxis4", "EA4", "Increment on external axis 4. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(true, typeof(Param_Number), "ExternalAxis5", "EA5", "Increment on external axis 5. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
            mpManager.AddParameter(true, typeof(Param_Number), "ExternalAxis6", "EA6", "Increment on external axis 6. Leave disconnected for inactive axes.", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "ExternalAxes Action", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double? e1 = null,
                e2 = null,
                e3 = null,
                e4 = null,
                e5 = null, 
                e6 = null;

            // Allow data fetching to not work and keep null values.
            DA.GetData(0, ref e1);
            DA.GetData(1, ref e2);
            DA.GetData(2, ref e3);
            DA.GetData(3, ref e4);
            DA.GetData(4, ref e5);
            DA.GetData(5, ref e6);

            DA.SetData(0, new ActionExternalAxes(e1, e2, e3, e4, e5, e6, this.Relative));
        }
    }
}
