using System.Configuration;

namespace SubscriptionApp.Client.Services
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
