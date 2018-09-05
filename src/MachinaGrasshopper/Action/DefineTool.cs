using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Machina;


namespace MachinaGrasshopper.Action
{
    public class DefineTool : GH_Component
    {
        public DefineTool() : base(
            "DefineTool",
            "DefineTool",
            "Define a Tool object on the Robot's internal library to make it avaliable for future Attach/Detach actions.",
            "Machina",
            "Action")
        { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("d5b12964-3990-4434-974d-9283471ea1d1");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Action_DefineTool;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "T", "Tool name", GH_ParamAccess.item, "ToolExMachina");
            pManager.AddPlaneParameter("BasePlane", "BP", "Base Plane where the Tool will be attached to the Robot", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddPlaneParameter("TCPPlane", "TP", "Plane of the Tool Tip Center (TCP)", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Weight", "W", "Tool weight in Kg", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Action", "A", "DefineTool Action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            Plane bpl = Plane.WorldXY;
            Plane tcppl = Plane.WorldXY;
            double w = 0;

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref bpl)) return;
            if (!DA.GetData(2, ref tcppl)) return;
            if (!DA.GetData(3, ref w)) return;

            // Create a TCP plane as 
            Rhino.Geometry.Transform rel = Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, bpl);
            if (!tcppl.Transform(rel))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid input planes");
                return;
            }

            Point3d cog = 0.5 * tcppl.Origin;
            ActionDefineTool adf = new ActionDefineTool(name,
                tcppl.OriginX, tcppl.OriginY, tcppl.OriginZ,
                tcppl.XAxis.X, tcppl.XAxis.Y, tcppl.XAxis.Z,
                tcppl.YAxis.X, tcppl.YAxis.Y, tcppl.YAxis.Z,
                w,
                cog.X, cog.Y, cog.Z);

            DA.SetData(0, adf);

        }
    }
}
