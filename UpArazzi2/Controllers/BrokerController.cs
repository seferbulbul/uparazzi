using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpArazzi2.Authentication;
using UpArazzi2.Models;

namespace UpArazzi2.Controllers
{
    [BayiAuthentication]
    public class BrokerController : BaseController
    {
        UpArazziDBEntities db = new UpArazziDBEntities();
       

        public ActionResult DanismanEkle()
        {          
            return View();
        }

        [HttpPost]
        public ActionResult DanismanEkle(danisman d, HttpPostedFileBase profil, HttpPostedFileBase myk)
        {
            Random rnd = new Random();
            d.CreatedDate = DateTime.Now;
            d.Password = "UP" + rnd.Next(1000, 100000);
            d.IsDeleted = false;
            d.Onay = false;
            d.Broker = false;
            d.DanismanMi = true;
            d.Admin = false;
            d.Kabul = false;
            d.PhotoPath = ResimBelgeEkle(profil);
            if (myk != null)
            {
                d.Yeterlilik = ResimBelgeEkle(myk);
            }        
            d.BrokerId = CurrentUser.Id;
            d.Gorevi = "Arsa ve Arazi Yatırım Uzmanı";

            db.danismen.Add(d);
            db.SaveChanges();
            ViewBag.Mesaj = " * Uzman Eklenmiştir. ";

            LogEkle($"{d.Ad}, yeni uzman olarak sisteme eklenmiştir.", false);

            return View();
        }

        public ActionResult DanismanGuncelle(int id)
        {
            danisman d = db.danismen.Find(id);
            return View(d);
        }

        [HttpPost]
        public ActionResult DanismanGuncelle(danisman d, HttpPostedFileBase profil, HttpPostedFileBase myk)
        {
            danisman da = db.danismen.Find(d.Id);
            if (profil != null)
            {
                d.PhotoPath = ResimBelgeEkle(profil);
            }
            else
            {
                d.PhotoPath = da.PhotoPath;
            }
            
            if (myk != null)
            {
                d.Yeterlilik = ResimBelgeEkle(myk);
            }
            else
            {
                d.Yeterlilik = da.Yeterlilik;
            }
            d.Showroom = da.Showroom;
            d.Password = da.Password;
            d.IsDeleted = da.IsDeleted;
            d.Onay = da.Onay;
            d.CreatedDate = da.CreatedDate;
            d.Kabul = da.Kabul;
            d.DanismanMi = da.DanismanMi;
            d.BrokerId = da.BrokerId;


            db.Entry(da).CurrentValues.SetValues(d);
           
            db.SaveChanges();
            ViewBag.Mesaj = " * Uzman Güncellenmiştir. ";

            return View(da);
        }

        public void BrokerOnay(int id)
        {
            islem i = db.islems.Find(id);
            i.BrokerOnay = i.BrokerOnay == true ? false : true;
            db.SaveChanges();
        }


    }
}