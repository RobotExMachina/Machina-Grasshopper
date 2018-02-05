using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using MachinaGrasshopper.GH_Utils;
using Machina;

namespace MachinaGrasshopper.Actions
{
    //  ████████╗███████╗███╗   ███╗██████╗ ███████╗██████╗  █████╗ ████████╗██╗   ██╗██████╗ ███████╗
    //  ╚══██╔══╝██╔════╝████╗ ████║██╔══██╗██╔════╝██╔══██╗██╔══██╗╚══██╔══╝██║   ██║██╔══██╗██╔════╝
    //     ██║   █████╗  ██╔████╔██║██████╔╝█████╗  ██████╔╝███████║   ██║   ██║   ██║██████╔╝█████╗  
    //     ██║   ██╔══╝  ██║╚██╔╝██║██╔═══╝ ██╔══╝  ██╔══██╗██╔══██║   ██║   ██║   ██║██╔══██╗██╔══╝  
    //     ██║   ███████╗██║ ╚═╝ ██║██║     ███████╗██║  ██║██║  ██║   ██║   ╚██████╔╝██║  ██║███████╗
    //     ╚═╝   ╚══════╝╚═╝     ╚═╝╚═╝     ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝╚══════╝
    //                                                                                                
    public class Temperature : GH_MutableComponent
    {
        public Temperature() : base(
            "Temperature",
            "Temperature",
            "Changes the working temperature of the device's parts. Useful for 3D printing operations.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("e4b98069-260e-4c59-8a76-3dfe23f25111");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Temperature;

        protected override bool ShallowInputMutation => true;

        protected override void RegisterMutableInputParams(GH_MutableInputParamManager mpManager)
        {
            // Relative
            mpManager.AddComponentNames(true, "Temperature", "Temperature", "Increments the working temperature of one of the device's parts. Useful for 3D printing operations.");
            mpManager.AddParameter(true, typeof(Param_Number), "TemperatureInc", "TInc", "Temperature increment in °C", GH_ParamAccess.item);
            mpManager.AddParameter(true, typeof(Param_String), "DevicePart", "P", "Device's part that will change temperature, e.g. \"extruder\", \"bed\", etc.", GH_ParamAccess.item, "extruder");
            mpManager.AddParameter(true, typeof(Param_Boolean), "WaitToReachTemp", "w", "If true, execution will wait for the part to heat up and resume when reached the target.", GH_ParamAccess.item, true);

            // Absolute
            mpManager.AddComponentNames(false, "TemperatureTo", "TemperatureTo", "Sets the working temperature of one of the device's parts. Useful for 3D printing operations.");
            mpManager.AddParameter(false, typeof(Param_Number), "Temperature", "T", "Temperature value in °C", GH_ParamAccess.item);
            mpManager.AddParameter(false, typeof(Param_String), "DevicePart", "P", "Device's part that will change temperature, e.g. \"extruder\", \"bed\", etc.", GH_ParamAccess.item, "extruder");
            mpManager.AddParameter(false, typeof(Param_Boolean), "WaitToReachTemp", "w", "If true, execution will wait for the part to heat up and resume when reached the target.", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "Temperature Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double temp = 0;
            string part = "";
            bool wait = true;

            if (!DA.GetData(0, ref temp)) return;
            if (!DA.GetData(1, ref part)) return;
            if (!DA.GetData(2, ref wait)) return;

            RobotPartType tt;
            try
            {
                tt = (RobotPartType)Enum.Parse(typeof(RobotPartType), part, true);
                if (Enum.IsDefined(typeof(RobotPartType), tt))
                {
                    DA.SetData(0, new ActionTemperature(temp, tt, wait, this.Relative));
                }
            }
            catch
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, 
                    $"\"{part}\" is not a valid target part for temperature changes, please specify one of the following: {GH_Utils.GH_Utils.EnumerateList(Enum.GetNames(typeof(RobotPartType)), "or")}.");
                return;
            }
            
        }
    }

}
