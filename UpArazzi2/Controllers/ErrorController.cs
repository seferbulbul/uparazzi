using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpArazzi2.Models;

namespace UpArazzi2.Controllers
{
    public class ErrorController : BaseController
    {
        UpArazziDBEntities db = new UpArazziDBEntities();

        public void HataKaydet(string aspxerrorpath,string code)
        {
            errortable e = new errortable();
            e.code = code;
            e.CreatedDate = DateTime.Now;
            e.Page = aspxerrorpath;
            string user = "Giriş Yapmayan Bir Kullanıcı";
            if (CurrentUser != null)
            {
                user = CurrentUser.Ad;
            }          
            e.appuser = user;
            db.errortables.Add(e);
            db.SaveChanges();
        }
        public ActionResult Hata()
        {
            Response.TrySkipIisCustomErrors = true;
            return View();
        }
        public ActionResult Page404(string aspxerrorpath)
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.Kaynak = aspxerrorpath;

            HataKaydet(aspxerrorpath, "404"); 

            return View("Hata");
        }
        public ActionResult Page403(string aspxerrorpath)
        {
            Response.StatusCode = 403;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.Kaynak = aspxerrorpath;
            HataKaydet(aspxerrorpath, "403");
            return View("Hata");
        }
        public ActionResult Page500(string aspxerrorpath)
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.Kaynak = aspxerrorpath;
            HataKaydet(aspxerrorpath, "500");
            return View("Hata");
        }
    }
}