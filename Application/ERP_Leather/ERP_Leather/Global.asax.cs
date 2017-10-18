using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ERP.EntitiesModel.BaseModel;

namespace ERP_Leather
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application.Lock();

            DatabaseConfiguration.DatabaseProvider = "System.Data.SqlClient";

            DatabaseConfiguration.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AdoConnectionString"].ConnectionString;
            Application.UnLock();
        }
    }
}
