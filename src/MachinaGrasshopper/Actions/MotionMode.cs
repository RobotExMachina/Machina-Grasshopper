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

    //  ███╗   ███╗ ██████╗ ████████╗██╗ ██████╗ ███╗   ██╗███╗   ███╗ ██████╗ ██████╗ ███████╗
    //  ████╗ ████║██╔═══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║████╗ ████║██╔═══██╗██╔══██╗██╔════╝
    //  ██╔████╔██║██║   ██║   ██║   ██║██║   ██║██╔██╗ ██║██╔████╔██║██║   ██║██║  ██║█████╗  
    //  ██║╚██╔╝██║██║   ██║   ██║   ██║██║   ██║██║╚██╗██║██║╚██╔╝██║██║   ██║██║  ██║██╔══╝  
    //  ██║ ╚═╝ ██║╚██████╔╝   ██║   ██║╚██████╔╝██║ ╚████║██║ ╚═╝ ██║╚██████╔╝██████╔╝███████╗
    //  ╚═╝     ╚═╝ ╚═════╝    ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝     ╚═╝ ╚═════╝ ╚═════╝ ╚══════╝
    //                                                                                         
    public class MotionMode : GH_Component
    {

        // Not very programmatic, but need to get this out... 
        private Dictionary<MotionType, bool> mtToggles = new Dictionary<MotionType, bool>()
        {
            { MotionType.Linear, false },
            { MotionType.Joint, false }
        };

        private MotionType m_currentmm = MotionType.Linear;
        protected MotionType CurrentMotionMode
        {
            get { return m_currentmm; }
            set
            {
                // Uncheck old mode
                mtToggles[m_currentmm] = false;

                // Check new one
                m_currentmm = value;
                mtToggles[m_currentmm] = true;

                // Update component look
                this.Message = Enum.GetName(typeof(MotionType), m_currentmm);
                this.ExpireSolution(true);
            }
        }


        public MotionMode() : base(
            "MotionMode",
            "MotionMode",
            "Sets the current motion mode to be applied to future actions. This can be \"linear\" (default) for straight line movements in euclidean space, or \"joint\" for smooth interpolation between axes angles. NOTE: \"joint\" motion may produce unexpected trajectories resulting in reorientations or collisions; use with caution! Right-click to set MotionMode.",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("b7681714-6734-474c-b5d5-6fb8003f674f");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_MotionMode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // There will be no input, mode changes from item menu
            //pManager.AddTextParameter("Type", "T", "\"linear\" or \"joint\"", GH_ParamAccess.item, "linear");

            //// Initialize dict programmatically
            //foreach (MotionType t in Enum.GetValues(typeof(MotionType)))
            //{
            //    mtToggles.Add(t, t == currentmode);
            //}
            //this.Message = Enum.GetName(typeof(MotionType), currentmode);

            this.CurrentMotionMode = MotionType.Linear;  // Init
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "MotionMode Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, new ActionMotion(CurrentMotionMode));
        }

        /// <summary>
        /// This is  regenerated every time the user right-clicks on the component: https://discourse.mcneel.com/t/manually-un-checking-additional-component-menu-items/54309/2
        /// </summary>
        /// <param name="menu"></param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            //// This would be ideal, but need a way to implement description text programmatically in Core...
            //Type mt = typeof(MotionType);
            //foreach (MotionType type in Enum.GetValues(mt))
            //{
            //    var item = Menu_AppendItem(menu, Enum.GetName(mt, type), ModeToggle, true, mtToggles[type]);
            //    item.ToolTipText = Enum.GetName(mt, type);
            //    item.Tag = type;  // store the type as data tag
            //}

            var lin = Menu_AppendItem(menu, "Linear", ModeToggle, null, true, mtToggles[MotionType.Linear]);
            lin.ToolTipText = "Straight linear motions in euclidean space.";
            lin.Tag = MotionType.Linear;

            var jnt = Menu_AppendItem(menu, "Joint", ModeToggle, null, true, mtToggles[MotionType.Joint]);
            jnt.ToolTipText = "Smooth joint interpolation between axes angles. NOTE: \"joint\" motion may produce unexpected trajectories resulting in reorientations or collisions; use with caution!";
            jnt.Tag = MotionType.Joint;
        }


        protected void ModeToggle(Object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            // Save some computing
            if ((MotionType)item.Tag == CurrentMotionMode) return;
            
            // Record undo Mode
            this.RecordUndoEvent("currentMode");  // Improve this: undo resets message, but not menu

            // The setter will take care of updating the menus
            this.CurrentMotionMode = (MotionType)item.Tag;
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("MotionMode", (int)CurrentMotionMode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            CurrentMotionMode = (MotionType)reader.GetInt32("MotionMode");
            return base.Read(reader);
        }

    }
}
