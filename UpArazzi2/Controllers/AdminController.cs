using PagedList;
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
    [AdminAuthentication]
    [DefaultAuthentication]

    public class AdminController : BaseController
    {
        UpArazziDBEntities db = new UpArazziDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Brokers(string tur)
        {
            BrokersVM vm = new BrokersVM();
            vm.Brokers = db.danismen.Where(x => x.Broker == true).ToList();
            vm.Portfoys = db.portfoys.Where(x => x.IsDeleted == false && x.Onay == true).ToList();

            switch (tur)
            {
                case "aktif":
                    vm.Brokers = vm.Brokers.Where(x => x.IsDeleted == false).ToList();
                    break;
                case "pasif":
                    vm.Brokers = vm.Brokers.Where(x => x.IsDeleted == true).ToList();
                    break;
            }

            return View(vm);
        }

        public ActionResult BrokerEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BrokerEkle(danisman b, HttpPostedFileBase profil, HttpPostedFileBase logo, HttpPostedFileBase myk)
        {
            Random rnd = new Random();

            b.IsDeleted = false;
            b.Password = "UP" + rnd.Next(1000, 100000);
            b.CreatedDate = DateTime.Now;
            b.Broker = true;

            if (profil != null)
            {
                b.PhotoPath = ResimBelgeEkle(profil);
            }
            if (logo != null)
            {
                b.BrokerLogo = ResimBelgeEkle(logo);
            }
            if (myk != null)
            {
                b.Yeterlilik = ResimBelgeEkle(myk);
            }

            db.danismen.Add(b);
            db.SaveChanges();




            ViewBag.Mesaj = " * Broker Eklenmiştir.";
            return View();
        }

        public ActionResult EditBroker(int id)
        {
            danisman d = db.danismen.Find(id);
            return View(d);
        }

        [HttpPost]
        public ActionResult EditBroker(danisman b, HttpPostedFileBase profil, HttpPostedFileBase logo, HttpPostedFileBase myk)
        {
            danisman d = db.danismen.Find(b.Id);

            d.Ad = b.Ad;
            d.Email = b.Email;
            d.Telefon = b.Telefon;
            d.CompanyName = b.CompanyName;
            d.VergiDairesi = b.VergiDairesi;
            d.VergiPhone = b.VergiPhone;
            d.BirthDay = b.BirthDay;
            d.Bolge = b.Bolge;
            d.UparazziName = b.UparazziName;
            d.OfficeContact = b.OfficeContact;
            d.Ozgecmis = b.Ozgecmis;



            if (profil != null)
            {
                d.PhotoPath = ResimBelgeEkle(profil);
            }
            if (logo != null)
            {
                d.BrokerLogo = ResimBelgeEkle(logo);
            }
            if (myk != null)
            {
                d.Yeterlilik = ResimBelgeEkle(myk);
            }


            db.SaveChanges();




            ViewBag.Mesaj = " * Broker Güncellenmiştir.";
            return View(d);
        }

        public void BayiSil(int id)
        {
            danisman b = db.danismen.Find(id);
            b.IsDeleted = b.IsDeleted == true ? false : true;
            db.SaveChanges();
        }

        public void YonetimOnay(int id)
        {
            islem i = db.islems.Find(id);
            i.YonetimOnay = i.YonetimOnay == true ? false : true;
            db.SaveChanges();
        }

        public ActionResult PersonelListesi()
        {
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false && x.Broker != true && x.DanismanMi != true).ToList();
            return View(d);
        }

        public ActionResult PersonelDuzenle(int id)
        {
            danisman d = db.danismen.Find(id);
            return View(d);
        }

        [HttpPost]
        public ActionResult PersonelDuzenle(danisman d, HttpPostedFileBase Resim)
        {
            danisman dn = db.danismen.Find(d.Id);

            dn.Ad = d.Ad;
            dn.Email = d.Email;
            dn.Gorevi = d.Gorevi;
            dn.Telefon = d.Telefon;
            dn.BirthDay = d.BirthDay;
            dn.Admin = d.Admin;
            dn.Mudur = d.Mudur;
            dn.Asistan = d.Asistan;

            if (Resim != null)
            {
                dn.PhotoPath = ResimBelgeEkle(Resim);
            }

            ViewBag.Mesaj = " * Personel Güncellenmiştir.";
            db.SaveChanges();

            return View(dn);
        }

        public ActionResult PersonelEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PersonelEkle(danisman d, HttpPostedFileBase profil)
        {
            d.CreatedDate = DateTime.Now;
            d.IsDeleted = false;
            d.Broker = false;
            d.DanismanMi = false;
            d.Kabul = false;
            d.PhotoPath = ResimBelgeEkle(profil);
            db.danismen.Add(d);
            db.SaveChanges();
            ViewBag.Mesaj = " * Personel Eklenmiştir. ";

            return View();
        }

        public void ChangeDanisman(int did, int pid)
        {
            portfoy p = db.portfoys.Find(pid);
            p.DanismanId = did;
            db.SaveChanges();
        }

        public ActionResult PortfoyAktar()
        {
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false).ToList();
            return View(d);
        }

        [HttpPost]
        public ActionResult PortfoyAktar(int kimden, int kime)
        {
            List<portfoy> p = db.portfoys.Where(x => x.DanismanId == kimden).ToList();
            foreach (portfoy item in p)
            {
                item.DanismanId = kime;
                db.SaveChanges();
            }
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false).ToList();

            ViewBag.Mesaj = " * Portföylerin Tamamı Aktarılmıştır.";
            return View(d);
        }
    }
}