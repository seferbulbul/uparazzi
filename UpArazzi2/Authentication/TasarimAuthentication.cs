using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpArazzi2.Models;

namespace UpArazzi2.Authentication
{
    public class TasarimAuthentication:AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            danisman u = httpContext.Session["User"] as danisman;
            if (u != null && u.Tasarim == true)
            {
                return true;
            }
            httpContext.Response.Redirect("/Home/Index");
            return false;

        }
    }
}