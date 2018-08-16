using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Reflection;
using MediaPortal.BL;
using MediaPortal.BL.Interface;

namespace MediaPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string AzureStorageBlobLink;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AzureStorageBlobLink = ConfigurationManager.AppSettings["azureStorageBlobLink"];
        }
    }
}
