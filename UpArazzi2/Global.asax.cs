using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UpArazzi2.App_Start;
using UpArazzi2.Tasks.Triggers;

namespace UpArazzi2
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            HatirlatmaTrigger.Baslat();        
            HatirlatmaTrigger.Baslat2();        
            YetkiSuresiTrigger.Baslat();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http://uparazzi.com.tr"))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("http://uparazzi.com.tr", "https://www.uparazzi.com.tr"));
            }
            else if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("https://uparazzi.com.tr"))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("https://uparazzi.com.tr", "https://www.uparazzi.com.tr"));


            }
            else if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http://www.uparazzi.com.tr"))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("http://www.uparazzi.com.tr", "https://www.uparazzi.com.tr"));


            }
        }
    }
}
