using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Configuration;
using System.Data.SqlClient;
using UILAB.Filters;
using System.Globalization;
using System.Threading;

namespace UILAB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            CultureInfo newCultureInfo = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCultureInfo.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            //newCultureInfo.DateTimeFormat.LongDatePattern = "yyyy/MM/dd";
            //newCultureInfo.DateTimeFormat.FullDateTimePattern = "yyyy/MM/dd";
            newCultureInfo.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = newCultureInfo;
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new UserAuditFilter());
            SqlDependency.Start(ConfigurationManager.ConnectionStrings["DBConn"].ToString());
        }

        void Application_Error(object sender, EventArgs e)
        {

            Exception ex = Server.GetLastError();
            if (ex == null || ex.Message.StartsWith("File"))
            {
                return;
            }
            try
            {
                Server.ClearError();
                Response.Redirect("~/ErrorPage/LinkError");
            }
            finally
            {
                ex = null;
            }

        }
    }
}
