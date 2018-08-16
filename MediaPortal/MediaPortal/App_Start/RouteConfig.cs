using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MediaPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "View",
                url: "Home/ViewFile/{fileSystemId}/{left}/{right}",
                defaults: new { controller = "Home", action = "ViewFile", fileSystemId = UrlParameter.Optional, left = UrlParameter.Optional, right = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Download",
                url: "Home/DownloadFileSystem/{fileSystemId}/{fileSystemName}",
                defaults: new { controller = "Home", action = "DownloadFileSystem" }
            );

            routes.MapRoute(
                name: "DownloadProcess",
                url: "Home/DownloadProcess/{fileSystemName}",
                defaults: new { controller = "Home", action = "DownloadProcess", fileSystemName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ZIPisReady",
                url: "Home/ZIPisReady/{fileSystemName}",
                defaults: new { controller = "Home", action = "ZIPisReady", fileSystemName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{folderID}/{folderName}",
                defaults: new { controller = "Home", action = "UserFiles", folderID = UrlParameter.Optional,folderName = UrlParameter.Optional }
            );
        }
    }
}
