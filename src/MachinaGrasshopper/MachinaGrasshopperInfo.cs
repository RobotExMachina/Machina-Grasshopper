using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace MachinaGrasshopper
{
    //  ███╗   ███╗ █████╗  ██████╗██╗  ██╗██╗███╗   ██╗ █████╗                                   
    //  ████╗ ████║██╔══██╗██╔════╝██║  ██║██║████╗  ██║██╔══██╗                                  
    //  ██╔████╔██║███████║██║     ███████║██║██╔██╗ ██║███████║                                  
    //  ██║╚██╔╝██║██╔══██║██║     ██╔══██║██║██║╚██╗██║██╔══██║                                  
    //  ██║ ╚═╝ ██║██║  ██║╚██████╗██║  ██║██║██║ ╚████║██║  ██║                                  
    //  ╚═╝     ╚═╝╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝                                  
    //                                                                                            
    //  ███████╗ ██████╗ ██████╗                                                                  
    //  ██╔════╝██╔═══██╗██╔══██╗                                                                 
    //  █████╗  ██║   ██║██████╔╝                                                                 
    //  ██╔══╝  ██║   ██║██╔══██╗                                                                 
    //  ██║     ╚██████╔╝██║  ██║                                                                 
    //  ╚═╝      ╚═════╝ ╚═╝  ╚═╝                                                                 
    //                                                                                            
    //   ██████╗ ██████╗  █████╗ ███████╗███████╗██╗  ██╗ ██████╗ ██████╗ ██████╗ ███████╗██████╗ 
    //  ██╔════╝ ██╔══██╗██╔══██╗██╔════╝██╔════╝██║  ██║██╔═══██╗██╔══██╗██╔══██╗██╔════╝██╔══██╗
    //  ██║  ███╗██████╔╝███████║███████╗███████╗███████║██║   ██║██████╔╝██████╔╝█████╗  ██████╔╝
    //  ██║   ██║██╔══██╗██╔══██║╚════██║╚════██║██╔══██║██║   ██║██╔═══╝ ██╔═══╝ ██╔══╝  ██╔══██╗
    //  ╚██████╔╝██║  ██║██║  ██║███████║███████║██║  ██║╚██████╔╝██║     ██║     ███████╗██║  ██║
    //   ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚═╝     ╚═╝     ╚══════╝╚═╝  ╚═╝
    //                                                                                            
    public class MachinaGrasshopperInfo : GH_AssemblyInfo
    {
        /// <summary>
        /// Quick and dirty version tracking.
        /// </summary>
        /// <returns></returns>
        public static string MachinaGrasshopperAPIVersion() => "0.8.1";



        // Standard GHInfo overrides
        public override string Name => "Machina";

        public override Bitmap Icon => Properties.Resources.Machina_Robot;

        public override string Description => "Real-time programming and control of robots"; 

        public override Guid Id => new Guid("cb8be1a9-b80c-4edf-84f9-077d5ba8ca8a");

        public override string AuthorName => "Jose Luis Garcia del Castillo y Lopez";

        public override string AuthorContact => "https://github.com/RobotExMachina";

        public override string Version =>  MachinaGrasshopperAPIVersion();
    }
}
