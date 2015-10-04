using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Subscrio.Client.Services
{
    internal static class DirectoryService
    {
        public static string AssemblyDirectory
        {
            get
            {
                //allow for override
                var overridenPath = ConfigurationManager.AppSettings["Subscrio.FilePath"];
                if (overridenPath != null) return overridenPath;

                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
            }
        }


    }
}
