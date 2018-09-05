using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;

namespace MachinaGrasshopper.Utils
{
    /// <summary>
    /// Adds some custom props to the Tab on which this plugin with show up.
    /// https://www.grasshopper3d.com/forum/topics/add-an-icon-image-to-grasshopper-toolbar-tabs
    /// </summary>
    public class TabProperties : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            var server = Grasshopper.Instances.ComponentServer;

            server.AddCategoryShortName("Machina", "Mach");
            server.AddCategorySymbolName("Machina", 'M');
            server.AddCategoryIcon("Machina", Properties.Resources.Machina_Head);

            return GH_LoadingInstruction.Proceed;
        }

    }
}
