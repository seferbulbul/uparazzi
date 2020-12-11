using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpArazzi2.Models;

namespace UpArazzi2.Authentication
{
    public class AdminAuthentication : AuthorizeAttribute
    {
        UpArazziDBEntities db = new UpArazziDBEntities();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            danisman u = httpContext.Session["User"] as danisman;
            if (u.Admin == true)
            {
                return true;
            }
            httpContext.Response.Redirect("/Home/Index");
            return false;

        }
    }
}