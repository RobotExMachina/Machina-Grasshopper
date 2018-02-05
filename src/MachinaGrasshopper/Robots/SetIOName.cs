using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Machina;

namespace MachinaGrasshopper.Robots
{
    //  ███████╗███████╗████████╗██╗ ██████╗ ███╗   ██╗ █████╗ ███╗   ███╗███████╗
    //  ██╔════╝██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║██╔══██╗████╗ ████║██╔════╝
    //  ███████╗█████╗     ██║   ██║██║   ██║██╔██╗ ██║███████║██╔████╔██║█████╗  
    //  ╚════██║██╔══╝     ██║   ██║██║   ██║██║╚██╗██║██╔══██║██║╚██╔╝██║██╔══╝  
    //  ███████║███████╗   ██║   ██║╚██████╔╝██║ ╚████║██║  ██║██║ ╚═╝ ██║███████╗
    //  ╚══════╝╚══════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝
    //                                                                            
    public class SetIOName : GH_Component
    {
        public SetIOName() : base(
            "SetIOName",
            "SetIOName",
            "Change the name of a Robot's IO pins.",
            "Machina",
            "Robots")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("7c3fe2f8-bc12-4eaa-92c1-27a6729504ac");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Robots_SetIOName;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Robot to change the io name to", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "New IO name", GH_ParamAccess.item, "Digital_IO_1");
            pManager.AddIntegerParameter("Pin", "N", "Pin number", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Digital", "d", "Is this a digital pin?", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot", "R", "Robot with named IO", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Machina.Robot bot = null;
            string name = "";
            int pin = 1;
            bool digital = true;

            if (!DA.GetData(0, ref bot)) return;
            if (!DA.GetData(1, ref name)) return;
            if (!DA.GetData(2, ref pin)) return;
            if (!DA.GetData(3, ref digital)) return;

            bot.SetIOName(name, pin, digital);
            DA.SetData(0, bot);
        }
    }
}
