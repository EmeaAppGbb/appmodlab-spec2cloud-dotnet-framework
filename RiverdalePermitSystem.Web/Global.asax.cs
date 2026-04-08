using System;
using System.Web;
using System.Web.Routing;

namespace RiverdalePermitSystem.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            System.Diagnostics.Debug.WriteLine($"Application Error: {ex?.Message}");
        }

        void Session_Start(object sender, EventArgs e)
        {
            Session["UserRole"] = "Applicant";
            Session["UserId"] = Guid.NewGuid().ToString();
        }
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("PermitApplication", "apply", "~/Pages/PermitApplication.aspx");
            routes.MapPageRoute("PermitSearch", "search", "~/Pages/PermitSearch.aspx");
            routes.MapPageRoute("Dashboard", "dashboard", "~/Pages/Dashboard.aspx");
        }
    }
}
