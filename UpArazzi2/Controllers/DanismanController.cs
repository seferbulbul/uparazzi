using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpArazzi2.Authentication;
using UpArazzi2.Models;
using UpArazzi2.Models.Message;

namespace UpArazzi2.Controllers
{

    [DanismanAuthentication]
    [DefaultAuthentication]
    public class DanismanController : BaseController
    {
        UpArazziDBEntities db = new UpArazziDBEntities();


        public ActionResult PortfoyEkle()
        {
            ViewBag.Ozellik = db.ozelliks.Where(x => x.Kategori == null).ToList();
            ViewBag.Ozellik2 = db.ozelliks.Where(x => x.Kategori == "Altyapi").ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PortfoyEkle(portfoy p, HttpPostedFileBase Resim, HttpPostedFileBase[] Resimler, int[] ozelliks, HttpPostedFileBase fileupload, int yetkisuresi)
        {
            p.DanismanId = CurrentUser.Id;
            p.IsDeleted = false;
            p.BittiTarih = DateTime.Now.AddMonths(yetkisuresi);
            p.Onay = false;
            p.Kimden = "uparazzi";
            p.Baslik = p.Baslik.Replace("'", " ");

            p.TapuResim = ResimBelgeEkle(Resim);


            if (fileupload != null)
            {
                string fileName = "/Video/" + Guid.NewGuid() + Path.GetFileName(fileupload.FileName);

                fileupload.SaveAs(Server.MapPath(fileName));

                p.Video = fileName;

            }





            db.portfoys.Add(p);
            db.SaveChanges();

            if (p.Id > 999)
            {
                p.IlanNo = "UP" + p.Id;
            }
            else if (p.Id > 99)
            {
                p.IlanNo = "UP0" + p.Id;
            }
            else
            {
                p.IlanNo = "UP00" + p.Id;
            }

            db.SaveChanges();
            TempData["Id"] = p.Id;
            ViewBag.Mesaj = "* Portföy başarıyla eklenmiştir.";
            int i = 0;
            foreach (HttpPostedFileBase item in Resimler)
            {
                if (item != null)
                {
                    fotograf f = new fotograf();
                    f.Path = ResimEkleWithMark(item);
                    f.PortfoyId = p.Id;
                    f.PhotoOrder = i++;
                    db.fotografs.Add(f);
                    db.SaveChanges();


                }
            }

            if (ozelliks != null)
            {
                foreach (int item in ozelliks)
                {
                    portfoyozellik po = new portfoyozellik();
                    po.OzellikId = item;
                    po.PortfoyId = p.Id;
                    db.portfoyozelliks.Add(po);
                    db.SaveChanges();

                }
            }

            neighborhood n = db.neighborhoods.Find(p.NeighborhoodId);

            var liste = db.filtres.Where(x => (x.UnitPriceMin.HasValue && x.UnitPriceMin <= p.Fiyat) || (x.UnitPriceMax.HasValue && x.UnitPriceMax >= p.Fiyat) || (x.m2Min.HasValue && x.m2Min <= p.M2) || (x.m2Max.HasValue && x.m2Max >= p.M2) || (x.Nitelik.Length > 1 && x.Nitelik == p.IlanTipi) || (x.IlId.HasValue && x.IlId == n.district.town.CityID) || (x.IlceId.HasValue && x.IlceId == n.district.TownID) || (x.MahId.HasValue && x.MahId == p.NeighborhoodId) || (x.Satilik.HasValue && x.Satilik == p.Satilik)).Select(x=>x.danisman);

            string konu = "Yeni Arayış Bulundu !";
            string mesaj = "Sisteme yeni portföyler eklendi ve sizin daha önce arayış içerisinde olduğunuz portföyler ile eşleşenleri bulduk. Panelinize giderek detayları görebilirsiniz.";

            foreach (danisman item in liste.Distinct())
            {              
                MailSender.Send("bulbul.sefer@gmail.com", subject: konu, body: EmailHtml(konu, mesaj, item));
            }

            ViewBag.Ozellik = db.ozelliks.Where(x => x.Kategori == null).ToList();
            ViewBag.Ozellik2 = db.ozelliks.Where(x => x.Kategori == "Altyapi").ToList();

            LogEkle($"{CurrentUser.Ad} tarafından sisteme yeni bir {p.IlanTipi} eklenmiştir. ",false);

            return View();
        }

        public ActionResult PortfoyGuncelle(int id)
        {
            PortfoyVM vm = new PortfoyVM();
            vm.Portfoy = db.portfoys.Find(id);
            vm.Resimler = db.fotografs.Where(x => x.PortfoyId == id).OrderBy(x => x.PhotoOrder).ToList();
            vm.Ozellikler = db.ozelliks.ToList();
            vm.Portfoyozelliks = db.portfoyozelliks.Where(x => x.PortfoyId == id).ToList();
            return View(vm);

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PortfoyGuncelle(PortfoyVM p, HttpPostedFileBase Resim, HttpPostedFileBase[] Resimler, int[] ozelliks, HttpPostedFileBase fileupload, string IlanTipi, bool Satilik, DateTime IlanTarihi, bool? TakasMi, bool? KatKarsiligi, bool? KrediyeUygunluk, bool? KadastralYol, int NeighborhoodId)
        {         
            portfoy po = db.portfoys.Find(p.Portfoy.Id);

            if (po.Fiyat != p.Portfoy.Fiyat)
            {
                LogEkle($"{CurrentUser.Ad} tarafından {po.IlanNo} numaralı ilanın {po.Fiyat.Value.ToString("N0")} fiyatı {p.Portfoy.Fiyat.Value.ToString("N0")} olarak güncellenmiştir. ", false);
            }

            po.Baslik = p.Portfoy.Baslik.Replace("'", " ");
            po.Aciklama = p.Portfoy.Aciklama;
            po.AdaNo = p.Portfoy.AdaNo;
            po.Emsal = p.Portfoy.Emsal;
            po.Fiyat = p.Portfoy.Fiyat;
            po.Gabari = p.Portfoy.Gabari;
            po.IlanTarihi = IlanTarihi;
            po.IlanTipi = IlanTipi;
            po.NeighborhoodId = NeighborhoodId;
            po.Imar = p.Portfoy.Imar;
            po.KadastralYol = KadastralYol;
            po.KatKarsiligi = KatKarsiligi;
            po.KrediyeUygunluk = KrediyeUygunluk;
            po.Latitude = p.Portfoy.Latitude;
            po.Longtitude = p.Portfoy.Longtitude;
            po.M2 = p.Portfoy.M2;
            po.PaftaTo = p.Portfoy.PaftaTo;
            po.ParselNo = p.Portfoy.ParselNo;
            po.Satilik = Satilik;
            po.TakasMi = TakasMi;
            po.TapuDurumu = p.Portfoy.TapuDurumu;
            
            db.SaveChanges();

            if (Resim != null)
            {
                po.TapuResim = ResimBelgeEkle(Resim);
                db.SaveChanges();

            }

            if (fileupload != null)
            {
                string fileName = "/Video/" + Guid.NewGuid() + Path.GetFileName(fileupload.FileName);

                fileupload.SaveAs(Server.MapPath(fileName));

                po.Video = fileName;
                db.SaveChanges();

            }

            int i = 0;
            try
            {
                i = db.fotografs.Where(x => x.PortfoyId == po.Id).Count() > 0 ? db.fotografs.Where(x => x.PortfoyId == po.Id).Max(x => x.PhotoOrder).Value : 0;

            }
            catch (Exception)
            {


            }

            if (Resimler != null)
            {
                foreach (HttpPostedFileBase item in Resimler)
                {
                    if (item != null)
                    {
                        fotograf f = new fotograf();
                        f.Path = ResimEkleWithMark(item);
                        f.PortfoyId = p.Portfoy.Id;
                        f.PhotoOrder = ++i;
                        db.fotografs.Add(f);
                        db.SaveChanges();
                    }
                }

                LogEkle($"{CurrentUser.Ad} tarafından {p.Portfoy.IlanNo} numaralı ilana yeni resimler eklenmiştir.", false);
            }


            List<portfoyozellik> portfoyozellik = db.portfoyozelliks.Where(x => x.PortfoyId == p.Portfoy.Id).ToList();
            foreach (portfoyozellik item in portfoyozellik)
            {
                db.portfoyozelliks.Remove(item);
                db.SaveChanges();
            }

            if (ozelliks != null)
            {
                foreach (int item in ozelliks)
                {
                    portfoyozellik por = new portfoyozellik();
                    por.OzellikId = item;
                    por.PortfoyId = p.Portfoy.Id;
                    db.portfoyozelliks.Add(por);
                    db.SaveChanges();

                }
            }

            db.SaveChanges();
            ViewBag.Mesaj = " * Portföy Başarıyla Güncellenmiştir.";

            p.Resimler = db.fotografs.Where(x => x.PortfoyId == p.Portfoy.Id).ToList();
            p.Ozellikler = db.ozelliks.ToList();
            p.Portfoyozelliks = db.portfoyozelliks.Where(x => x.PortfoyId == p.Portfoy.Id).ToList();
            p.Portfoy = db.portfoys.Find(p.Portfoy.Id);

            return View(p);

        }

        public ActionResult AddMedia(int id)
        {
            List<fotograf> f = db.fotografs.Where(x => x.PortfoyId == id).OrderBy(x => x.PhotoOrder).ToList();
            return View(f);
        }

        [HttpPost]
        public ActionResult AddMedia(int id, HttpPostedFileBase[] Resimler)
        {
            int i = 0;
            try
            {
                i = db.fotografs.Where(x => x.PortfoyId == id).Max(x => x.PhotoOrder).Value;
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
                    f.PortfoyId = id;
                    f.PhotoOrder = ++i;
                    db.fotografs.Add(f);
                    db.SaveChanges();
                }
            }

            ViewBag.Mesaj = " * Resimler Eklenmiştir..";

            List<fotograf> p = db.fotografs.Where(x => x.PortfoyId == id).OrderBy(x => x.PhotoOrder).ToList();
            return View(p);
        }

        public void Sirala(int id, int sira)
        {
            fotograf p = db.fotografs.Find(id);
            p.PhotoOrder = sira;
            db.SaveChanges();
        }

        [HttpPost]
        public void FotoSirala(string[] positions)
        {
            foreach (string item in positions)
            {
                int id = Convert.ToInt32(item.Split(',')[0]);
                int order = Convert.ToInt32(item.Split(',')[1]);
                fotograf p = db.fotografs.Find(id);
                p.PhotoOrder = order;
                db.SaveChanges();
            }
        }

        public void FotoSil(int id)
        {
            db.fotografs.Remove(db.fotografs.Find(id));
            db.SaveChanges();
        }

        public void ServisSil(int id)
        {
            service s = db.services.Find(id);
            s.IsDeleted = true;
            db.SaveChanges();
        }



        public ActionResult DanismanGuncelle()
        {
            danisman d = db.danismen.Find(CurrentUser.Id);
            return View(d);
        }

        [HttpPost]
        public ActionResult DanismanGuncelle(danisman d, HttpPostedFileBase Resim, string pw1, string pw2, string pwd, HttpPostedFileBase Yeterlilik)
        {
            danisman dn = db.danismen.Find(d.Id);

            if (pwd == "pwd")
            {
                if (d.Password == dn.Password && pw1 == pw2)
                {
                    dn.Password = pw1;
                    ViewBag.Mesaj = " * Şifre Değiştirilmiştir.";
                }
                else
                {
                    ViewBag.Mesaj = " * Şifre Değiştirilememiştir. Tekrar Deneyiniz.";

                }
            }
            else
            {
                dn.Ad = d.Ad;
                dn.Bolge = d.Bolge;
                dn.Gorevi = d.Gorevi;
                dn.Ozgecmis = d.Ozgecmis;
                dn.Telefon = d.Telefon;


                if (Resim != null)
                {
                    dn.PhotoPath = ResimBelgeEkle(Resim);
                }

                if (Yeterlilik != null)
                {
                    d.Yeterlilik = ResimBelgeEkle(Yeterlilik);

                }


                ViewBag.Mesaj = " * Hesabınız Güncellenmiştir.";
            }






            db.SaveChanges();


            return View(dn);
        }



        public ActionResult BitmisIslem(int id)
        {
            portfoy p = db.portfoys.Find(id);
            islem b = new islem();
            b.PortfoyId = id;
            b.Satilik = p.Satilik;
            b.PortfoyNo = p.IlanNo;
            b.Il = p.neighborhood.district.town.city.CityName;
            b.Ilce = p.neighborhood.district.town.TownName;
            b.AdaNo = p.AdaNo;
            b.Parsel = p.ParselNo;
            b.Nitelik = p.IlanTipi;
            b.m2 = p.M2.Value.ToString();
            b.portfoy = p;
            return View(b);
        }

        [HttpPost]
        public ActionResult BitmisIslem(islem i)
        {
            i.IsDeleted = false;
            db.islems.Add(i);
            db.SaveChanges();

            ViewBag.Mesaj = " * Rapor kaydedilmiştir, Broker onayından sonra merkeze gönderilecektir.";

            i.portfoy = db.portfoys.Find(i.PortfoyId);

            return View(i);
        }

        public void SureUzat(int id, int sure)
        {
            portfoy p = db.portfoys.Find(id);
            p.BittiTarih = DateTime.Now.AddMonths(sure);
            db.SaveChanges();

            LogEkle($"{CurrentUser.Ad} tarafından {p.IlanNo} numaralı ilanın süresi {sure} ay daha uzatılmıştır.", false);

        }

        public ActionResult TumIlanlar()
        {
            List<portfoy> p = db.portfoys.Where(x => x.IsDeleted == false && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.danisman.BrokerId == CurrentUser.BrokerId).OrderByDescending(x => x.Id).ToList();

            return View(p);
        }
        public ActionResult Danismanlar()
        {
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false && x.DanismanMi == true && x.BrokerId == CurrentUser.BrokerId).ToList();
            return View(d.OrderByDescending(x => x.portfoys.Count).ToList());
        }

        public ActionResult Arayislarim()
        {
            ArayisVM vm = new ArayisVM();
            vm.Filtres = db.filtres.Where(x => x.DanismanId == CurrentUser.Id).ToList();
            vm.Portfoys = db.portfoys.Where(x => x.danisman.IsDeleted == false && x.IsDeleted == false && x.Onay == true && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).ToList();

            return View(vm);
        }

        [HttpPost]
        public ActionResult ArayisEkle(decimal? UnitPriceMin, decimal? UnitPriceMax, int? m2Min, int? m2Max, string Nitelik, int? IlId, bool? Satilik, string Title)
        {
            filtre f = new filtre();
            f.CreatedDate = DateTime.Now;
            f.DanismanId = CurrentUser.Id;
            f.IlId = IlId;
            f.m2Max = m2Max;
            f.m2Min = m2Min;
            f.Nitelik = Nitelik;
            f.Satilik = Satilik;
            f.Title = Title;
            f.UnitPriceMax = UnitPriceMax;
            f.UnitPriceMin = UnitPriceMin;
            db.filtres.Add(f);
            db.SaveChanges();
            return RedirectToAction("Arayislarim");
        }
        public void ArayisSil(int id)
        {
            db.filtres.Remove(db.filtres.Find(id));
            db.SaveChanges();
        }

        public ActionResult ArayisPortfoylerim(int id)
        {
            filtre f = db.filtres.Find(id);

            var p = db.portfoys.AsQueryable();

            if (f.UnitPriceMin != null)
            {
                p = p.Where(x => x.Fiyat >= f.UnitPriceMin);
            }
            if (f.UnitPriceMax != null)
            {
                p = p.Where(x => x.Fiyat <= f.UnitPriceMax);
            }
            if (f.m2Min != null)
            {
                p = p.Where(x => x.M2 >= f.m2Min);
            }
            if (f.m2Max != null)
            {
                p = p.Where(x => x.M2 <= f.m2Max);
            }
            if (f.IlId != null)
            {
                p = p.Where(x => x.neighborhood.district.town.CityID == f.IlId);
            }
            if (f.Nitelik != "")
            {
                p = p.Where(x => x.IlanTipi == f.Nitelik);
            }
            if (f.Satilik != null)
            {
                p = p.Where(x => x.Satilik == f.Satilik);
            }

            return View(p.Where(x => x.danisman.IsDeleted == false && x.IsDeleted == false && x.Onay == true && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).ToList());

        }

        public ActionResult ServisEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ServisEkle(service s)
        {
            s.DanismanId = CurrentUser.Id;
            s.IsDeleted = false;
            db.services.Add(s);
            db.SaveChanges();
            ViewBag.Mesaj = " * Servis Başarıyla Kaydedilmiştir.";

            return View();
        }



        public void KapakYap(int id, int pid)
        {
            fotograf f = db.fotografs.Find(id);
            f.PhotoOrder = db.fotografs.Where(x => x.PortfoyId == pid).Min(x => x.PhotoOrder).Value - 1;
            db.SaveChanges();
        }

    }
}