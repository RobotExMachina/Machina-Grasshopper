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
        public static string MachinaGrasshopperAPIVersion() => "0.7.0";


        // Standard GHInfo overrides
        public override string Name
        {
            get
            {
                return "Machina";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("cb8be1a9-b80c-4edf-84f9-077d5ba8ca8a");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Jose Luis Garcia del Castillo";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "personal@garciadelcastillo.es";
            }
        }

        public override string Version
        {
            get
            {
                return MachinaGrasshopperAPIVersion();
            }
        }
    }
}
