using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;

using Machina;

namespace MachinaGrasshopper.Robot
{
    public class Logger : GH_Component
    {

        private List<string> _messages;
        private int _level;
        private int _maxCount;
        private int _refreshRate;

        public Logger() : base(
            "Logger",
            "Logger",
            "Logs relevant information about what's happening inside Machina",
            "Machina",
            "Robot")
        {
            _messages = new List<string>();
            _maxCount = 10;

            Machina.Logger.WriteLine += Logger_WriteLine;
        }

        private void Logger_WriteLine(string msg)
        {
            _messages.Add(msg);

            int diff = _messages.Count - _maxCount;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    _messages.RemoveAt(0);
                }
            }
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("10e50733-d8df-4313-a1da-64632c359ec3");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Robot_Logger;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Level", "L", "Define the level of logging desired for the WriteLine logger: 0 None, 1 Error, 2 Warning, 3 Info (default), 4 Verbose or 5 Debug.", GH_ParamAccess.item, 3);
            pManager.AddIntegerParameter("MaxMessages", "C", "Max number of displayed log messages. If 0, messages will be permanent", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("UpdateInterval", "I", "Refresh interval in milliseconds. 0 or less will disable autoupdate", GH_ParamAccess.item, 250);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LogMessages", "msgs", "Machina log messages", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int level = 3;

            if (!DA.GetData(0, ref level)) return;
            if (!DA.GetData(1, ref _maxCount)) return;
            if (!DA.GetData(2, ref _refreshRate)) return;

            // Sanity
            if (_refreshRate > 0 && _refreshRate < 33)
            {
                _refreshRate = 33;
            }

            if (level != _level) 
            {
                if (level < (int)LogLevel.NONE || level > (int)LogLevel.DEBUG)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Log level out of bounds, please choose one of: 0 None, 1 Error, 2 Warning, 3 Info (default), 4 Verbose or 5 Debug"); ;
                    return;
                }

                _level = level;
                Machina.Logger.SetLogLevel(_level);
            }

            DA.SetDataList(0, _messages);

            if (_refreshRate > 0)
            {
                this.OnPingDocument().ScheduleSolution(_refreshRate, doc =>
                {
                    this.ExpireSolution(false);
                });
            }

        }

    }
}
