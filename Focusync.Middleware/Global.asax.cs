using Focusync.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Focusync.Middleware
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            Global.AquaforestSDKPath = Server.MapPath(ConfigurationManager.AppSettings.Get("AquaforestLib"));
            
            Global.AquaforestConfidenceScore = Convert.ToByte(ConfigurationManager.AppSettings.Get("AquaforestConfidenceScore"));
            Global.OcrTempFolderPath = Convert.ToString(ConfigurationManager.AppSettings.Get("OCRTempFolder"));
            Global.AquaforestSDKLogsPath = Convert.ToString(ConfigurationManager.AppSettings.Get("OCRLogFolder"));
            if (!Directory.Exists(Global.OcrTempFolderPath))
            {
                throw new Exception($"OcrTempPathNotFound = {Global.OcrTempFolderPath}");
            }

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
