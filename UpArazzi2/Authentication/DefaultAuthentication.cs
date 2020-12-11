using UpArazzi2.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace UpArazzi2.Authentication
{
    public class DefaultAuthentication:AuthorizeAttribute
    {
        UpArazziDBEntities db = new UpArazziDBEntities();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["User"] == null && httpContext.Request.Cookies["User"] != null)
            {
                HttpCookie kayitlicerez = httpContext.Request.Cookies["User"];
                int id = Convert.ToInt32(kayitlicerez["userId"]);
                danisman u = db.danismen.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
                if (u!=null)
                {
                    httpContext.Session["User"] = u;
                }
            }            
           
            return true;
        }
    }
}