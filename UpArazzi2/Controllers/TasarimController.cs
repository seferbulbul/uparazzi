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
    [TasarimAuthentication]
    [DefaultAuthentication]
    public class TasarimController : BaseController
    {

        UpArazziDBEntities db = new UpArazziDBEntities();

        public ActionResult AddSlider()
        {
            List<slider> s = db.sliders.OrderBy(x=>x.SliderOrder).ToList();
            return View(s);
        }

        [HttpPost]
        public ActionResult AddSlider(HttpPostedFileBase[] Resim)
        {
            foreach (HttpPostedFileBase item in Resim)
            {
                if (item != null)
                {
                    slider sl = new slider();
                    sl.Path = ResimBelgeEkle(item);
                    db.sliders.Add(sl);
                    db.SaveChanges();
                }
            }

            ViewBag.Mesaj = " * Resimler Eklenmiştir. ";

            List<slider> s = db.sliders.OrderBy(x => x.SliderOrder).ToList();

            return View(s);
        }

        public void DeleteSlider(int id)
        {
            slider s = db.sliders.Find(id);
            db.sliders.Remove(s);
            db.SaveChanges();
        }

        public ActionResult AddUpHaber()
        {
            List<haber> h = db.habers.ToList();
            return View(h);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddUpHaber(int id,HttpPostedFileBase resim,string title, string text)
        {
            haber h = db.habers.Find(id);
            if (resim != null)
            {
                h.PhotoPath = ResimBelgeEkle(resim);
            }
           
            h.Title = title;
            h.Text = text;
            db.SaveChanges();

            ViewBag.Mesaj = " * Haber Güncellenmiştir..";

            DateTime d = DateTime.Now;
            bool ha = db.histories.Any(x => x.CreatedDate.Value.Year == d.Year && x.CreatedDate.Value.Month == d.Month && x.CreatedDate.Value.Day == d.Day && x.Action.Contains("UpHaber kısmında yeni güncellemeler olmuştur"));
            if (!ha)
            {
                LogEkle($"UpHaber kısmında yeni güncellemeler olmuştur.", false);
            }
            

            List<haber> hl = db.habers.ToList();
            return View(hl);
        }

        public void HaberSil(int id)
        {
            db.habers.Remove(db.habers.Find(id));
            db.SaveChanges();
        }

        public void BlogSil(int id)
        {
            db.blogs.Remove(db.blogs.Find(id));
            db.SaveChanges();
        }

        public ActionResult Blog(bool? SSS)
        {            
            List<blog> b = db.blogs.Where(x=>x.SSS==SSS).ToList();
            ViewBag.SSS = SSS==true ? true : false;
            return View(b);
        }

        public ActionResult AddBlog(bool? SSS)
        {
            ViewBag.SSS = SSS == true ? true : false;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBlog(blog b, HttpPostedFileBase[] Resim, bool? SSS)
        {
            b.CreatedDate = DateTime.Now;
            b.ReadCount = 0;
            b.SSS = SSS;
            db.blogs.Add(b);
            db.SaveChanges();

            if (Resim != null)
            {
                foreach (HttpPostedFileBase item in Resim)
                {
                    if (item != null)
                    {
                        fotograf f = new fotograf();
                        f.BlogId = b.Id;
                        f.Path = ResimBelgeEkle(item);
                        db.fotografs.Add(f);
                        db.SaveChanges();
                    }

                }
            }
            


            ViewBag.SSS = SSS;
            ViewBag.Mesaj = " * Başarıyla Eklenmiştir..";
            return View();
        }

        public ActionResult EditBlog(int id)
        {
            blog b = db.blogs.Find(id);
            return View(b);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBlog(blog bl, HttpPostedFileBase[] Resim)
        {
            blog b = db.blogs.Find(bl.Id);
            b.Title = bl.Title;
            b.Text = bl.Text;
            b.Category = bl.Category;

            if (Resim != null)
            {
                foreach (HttpPostedFileBase item in Resim)
                {
                    if (item != null)
                    {
                        fotograf f = new fotograf();
                        f.BlogId = b.Id;
                        f.Path = ResimBelgeEkle(item);
                        db.fotografs.Add(f);
                        db.SaveChanges();
                    }

                }
            }
           


            db.SaveChanges();

            ViewBag.Mesaj = " * Başarıyla Güncellenmiştir..";
            return View(b);
        }

        public ActionResult BlogMedia(int id)
        {
            List<fotograf> f = db.fotografs.Where(x => x.BlogId == id).OrderBy(x => x.PhotoOrder).ToList();
            return View(f);
        }

        [HttpPost]
        public ActionResult BlogMedia(int id, HttpPostedFileBase[] Resimler)
        {
            int i = 0;
            try
            {
                i = db.fotografs.Where(x => x.BlogId == id).Max(x => x.PhotoOrder).Value;
            }
            catch (Exception)
            {


            }


            foreach (HttpPostedFileBase item in Resimler)
            {
                if (item != null)
                {
                    fotograf f = new fotograf();
                    f.Path = ResimBelgeEkle(item);
                    f.BlogId = id;
                    f.PhotoOrder = ++i;
                    db.fotografs.Add(f);
                    db.SaveChanges();
                }
            }

            ViewBag.Mesaj = " * Resimler Eklenmiştir..";

            List<fotograf> p = db.fotografs.Where(x => x.BlogId == id).OrderBy(x => x.PhotoOrder).ToList();
            return View(p);
        }

        public void Sirala(int id, int sira)
        {
            fotograf p = db.fotografs.Find(id);
            p.PhotoOrder = sira;
            db.SaveChanges();
        }

        public void FotoSil(int id)
        {
            db.fotografs.Remove(db.fotografs.Find(id));
            db.SaveChanges();
        }

        public void MMSil(int id)
        {
            db.mms.Remove(db.mms.Find(id));
            db.SaveChanges();
        }

        public ActionResult MutluMusteriler()
        {
            List<mm> b = db.mms.OrderBy(x => x.MMOrder).ToList();
            return View(b);
        }

        public ActionResult AddMM()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddMM(mm b, HttpPostedFileBase Resim)
        {

            if (Resim != null)
            {
                b.PhotoPath = ResimBelgeEkle(Resim);
            }

            db.mms.Add(b);
            db.SaveChanges();

            ViewBag.Mesaj = " * Mutlu Müşteri Eklenmiştir..";
            return View();
        }

        public ActionResult EditMM(int id)
        {
            mm b = db.mms.Find(id);
            return View(b);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMM(mm bl, HttpPostedFileBase Resim)
        {
            mm b = db.mms.Find(bl.Id);
            b.Title = bl.Title;
            if (Resim != null)
            {
                b.PhotoPath = ResimBelgeEkle(Resim);
            }
            db.SaveChanges();

            ViewBag.Mesaj = " * Mutlu Müşteri Güncellenmiştir..";
            return View(b);
        }

        public ActionResult GelenMailler(string tur)
        {
            List<mail> m = db.mails.Where(x => x.Type.Contains(tur)).OrderByDescending(x=>x.Id).ToList();
            return View(m);
        }

        [HttpPost]
        public ActionResult KurumsalEkle(string Title,HttpPostedFileBase File, HttpPostedFileBase Resim)
        {
            kurumsal k = new kurumsal();
            k.CreaterDate = DateTime.Now;
            k.Title = Title;
            k.PdfPath = ResimBelgeEkle(File);
            k.PhotoPath = ResimBelgeEkle(Resim);
            db.kurumsals.Add(k);
            db.SaveChanges();
            return RedirectToAction("Kurumsal", "Home");
        }

        public void KurumsalSil(int id)
        {
            db.kurumsals.Remove(db.kurumsals.Find(id));
            db.SaveChanges();
        }

        public ActionResult Ekip()
        {
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false).OrderBy(x=> x.EkipOrder).ToList();
            return View(d);
        }

        public void EkipSirala(string[] positions)
        {
            foreach (string item in positions)
            {
                int id = Convert.ToInt32(item.Split(',')[0]);
                int order = Convert.ToInt32(item.Split(',')[1]);
                danisman d = db.danismen.Find(id);
                d.EkipOrder = order;
                db.SaveChanges();
            }
         
        }

        public void MMShow(int id)
        {
            mm d = db.mms.Find(id);
            d.Showroom = d.Showroom==true ? false : true;
            db.SaveChanges();
        }

        public void EkipShow(int id)
        {
            danisman d = db.danismen.Find(id);
            d.Showroom = d.Showroom == true ? false : true;
            db.SaveChanges();
        }

        public void MMSirala(string[] positions)
        {
            foreach (string item in positions)
            {
                int id = Convert.ToInt32(item.Split(',')[0]);
                int order = Convert.ToInt32(item.Split(',')[1]);
                mm d = db.mms.Find(id);
                d.MMOrder = order;
                db.SaveChanges();
            }
           
        }

        public void SliderSirala(string[] positions)
        {
            foreach (string item in positions)
            {
                int id = Convert.ToInt32(item.Split(',')[0]);
                int order = Convert.ToInt32(item.Split(',')[1]);
                slider d = db.sliders.Find(id);
                d.SliderOrder = order;
                db.SaveChanges();
            }

        }
    }
}