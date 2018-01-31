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

namespace MachinaGrasshopper
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

        //private int mType = (int) MotionType.Linear;  // can we serialize this?
        private bool linear = true;

        private Dictionary<MotionType, bool> mtToggles = new Dictionary<MotionType, bool>();
        private MotionType currentmode = MotionType.Linear;

        private bool[] toggles = new bool[Enum.GetNames(typeof(MotionType)).Length];



        public MotionMode() : base(
            "MotionMode",
            "MotionMode",
            "Sets the current motion mode to be applied to future actions. This can be \"linear\" (default) for straight line movements in euclidean space, or \"joint\" for smooth interpolation between axes angles. NOTE: \"joint\" motion may produce unexpected trajectories resulting in reorientations or collisions; use with caution!",
            "Machina",
            "Actions")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("b7681714-6734-474c-b5d5-6fb8003f674f");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Actions_Attach;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // There will be no input, mode changes from item menu
            //pManager.AddTextParameter("Type", "T", "\"linear\" or \"joint\"", GH_ParamAccess.item, "linear");

            foreach (MotionType t in Enum.GetValues(typeof(MotionType)))
            {
                mtToggles.Add(t, t == currentmode);
            }

            this.Message = Enum.GetName(typeof(MotionType), currentmode);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "MotionMode Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Machina.MotionType t = linear ? MotionType.Linear : MotionType.Joint;
            DA.SetData(0, new ActionMotion(currentmode));
        }


        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            //var lin = Menu_AppendItem(menu, "Linear", ModeToggle, null, true, mtToggles[MotionType.Linear]);
            //lin.ToolTipText = "Straight linear motions in euclidean space.";
            //var jnt = Menu_AppendItem(menu, "Joint", ModeToggle, null, true, mtToggles[MotionType.Joint]);
            //jnt.ToolTipText = "Smooth joint interpolation between axes angles. NOTE: \"joint\" motion may produce unexpected trajectories resulting in reorientations or collisions; use with caution!";

            var it = 0;
            foreach (var name in Enum.GetNames(typeof(MotionType)))
            {
                var item = Menu_AppendItem(menu, name, ModeToggle, true, toggles[it++]);
                item.ToolTipText = "Item " + (it - 1);
            }

        }


        protected void ModeToggle(Object sender, EventArgs e)
        {
            var msgs = new List<string>();

            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;
            this.RecordUndoEvent("currentMode");

            msgs.Add(toggles[0] + " " + toggles[1] + " " + toggles[2]);

            for (var i = 0; i < toggles.Length; i++) toggles[i] = false;

            var it = 0;
            foreach (var name in Enum.GetNames(typeof(MotionType)))
            {
                if (name.Equals(item.Text))
                {
                    toggles[it] = true;
                    msgs.Add("Changed " + name);
                }
                it++;
            }





                //foreach (var toggle in mtToggles)
                //{
                //    if (toggle.Value == true)
                //    {
                //        currentmode = toggle.Key;
                //        break;
                //    }
                //}


                //foreach (ToolStripMenuItem i in item.Owner.Items)
                //{
                //    i.Checked = false;
                //}

                //msgs.Add(item.Text + " prev state " + item.Checked);
                //item.Checked = !item.Checked;
                //msgs.Add(item.Text + " new state " + item.Checked);

                //var items = item.Owner.Items;
                ////foreach (var i in items.)

                //foreach (var otherItems in (from object i in item.Owner.Items.GetEnumerator()
                //                            let otherItems = i as ToolStripMenuItem
                //                            where otherItems != null
                //                            where i.Equals(item)
                //                            select i)) 
                //{
                //    //otherItem
                //}

                //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, currentmode.ToString());

                //this.Message = linear ? "Linear" : "Joint";
            this.Message = item.Text;
            this.ExpireSolution(true);

            foreach (var s in msgs)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, s);
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            //writer.SetBoolean("linear", linear);
            writer.SetInt32("MotionMode", (int)currentmode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            //mType = reader.GetInt32("MotionType");
            //linear = reader.GetBoolean("linear");
            currentmode = (MotionType)reader.GetInt32("MotionMode");
            this.Message = Enum.GetName(typeof(MotionType), currentmode);
            this.ExpireSolution(true);
            return base.Read(reader);
        }

        
    }
}
