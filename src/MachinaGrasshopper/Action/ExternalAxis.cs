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

namespace MachinaGrasshopper.Action
{
    //  ███████╗██╗  ██╗████████╗███████╗██████╗ ███╗   ██╗ █████╗ ██╗      █████╗ ██╗  ██╗███████╗███████╗
    //  ██╔════╝╚██╗██╔╝╚══██╔══╝██╔════╝██╔══██╗████╗  ██║██╔══██╗██║     ██╔══██╗╚██╗██╔╝██╔════╝██╔════╝
    //  █████╗   ╚███╔╝    ██║   █████╗  ██████╔╝██╔██╗ ██║███████║██║     ███████║ ╚███╔╝ █████╗  ███████╗
    //  ██╔══╝   ██╔██╗    ██║   ██╔══╝  ██╔══██╗██║╚██╗██║██╔══██║██║     ██╔══██║ ██╔██╗ ██╔══╝  ╚════██║
    //  ███████╗██╔╝ ██╗   ██║   ███████╗██║  ██║██║ ╚████║██║  ██║███████╗██║  ██║██╔╝ ██╗███████╗███████║
    //  ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝
    //                                                                                                     
    public class ExternalAxis : GH_MutableComponent
    {
        public ExternalAxis() : base(
            "ExternalAxis",
            "ExternalAxis",
            "Increase the value of one of the robot's external axis. Values expressed in degrees or milimeters, depending on the nature of the external axis. Note that the effect of this change of external axis will go in effect on the next motion Action.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("b304ce32-0ab1-4d8e-b20a-857b6fc1a55e");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_ExternalAxis;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Absolute
            mpManager.AddComponentNames(false, "ExternalAxisTo", "ExternalAxisTo", "Set the values of one of the robot's external axes.");
            mpManager.AddParameter(false, typeof(Param_Integer), "AxisNumber", "EAid", "Axis number from 1 to 6.", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_Number), "Value", "v", "Increment value in mm or degrees.", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_String), "Target", "t", "Which set of External Axes to target? \"All\", \"Cartesian\" or \"Joint\"?", GH_ParamAccess.item, "All");

            // Relative
            mpManager.AddComponentNames(true, "ExternalAxis", "ExternalAxis", "Increase the values of one of the robot's external axes.");
            mpManager.AddParameter(true, typeof(Param_Integer), "AxisNumber", "EAid", "Axis number from 1 to 6.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_Number), "Increment", "inc", "New value in mm or degrees.", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_String), "Target", "t", "Which set of External Axes to target? \"All\", \"Cartesian\" or \"Joint\"?", GH_ParamAccess.item, "All");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "ExternalAxis Action", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int axisNumber = 1;
            double val = 0;
            string externalAxesTarget = "All";

            if (!DA.GetData(0, ref axisNumber)) return;
            if (!DA.GetData(1, ref val)) return;
            if (!DA.GetData(2, ref externalAxesTarget)) return;

            if (axisNumber < 1 || axisNumber > 6)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "AxisNumber must be between 1 and 6");
            }

            ExternalAxesTarget eat;
            try
            {
                eat = (ExternalAxesTarget)Enum.Parse(typeof(ExternalAxesTarget), externalAxesTarget, true);
                if (Enum.IsDefined(typeof(ExternalAxesTarget), eat))
                {
                    DA.SetData(0, new ActionExternalAxis(axisNumber, val, ExternalAxesTarget.All, this.Relative));
                }
            }
            catch
            {
                string targets = "";
                foreach (string str in Enum.GetNames(typeof(ExternalAxesTarget)))
                {
                    targets += "\"" + str + "\" ";
                }
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    $"{externalAxesTarget} is not a valid ExternalAxesTarget type, please specify one of the following: " + targets);
            }

        }
    }
}
