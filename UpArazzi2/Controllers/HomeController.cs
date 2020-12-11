using OfficeOpenXml;
using PagedList;
using QRCoder;
using Rotativa;
using S22.Imap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using UpArazzi2.Authentication;
using UpArazzi2.Models;
using UpArazzi2.Models.Message;
using static UpArazzi2.App_Start.FilterConfig;

namespace UpArazzi2.Controllers
{
    [DefaultAuthentication]
    public class HomeController : BaseController
    {
        UpArazziDBEntities db = new UpArazziDBEntities();

        [Compress]
        public ActionResult Index()
        {
           
            return View();
        }

        [Route("sitemap.xml")]
        public ActionResult SitemapXml()
        {

            var siteUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            Response.Clear();
            Response.ContentType = "text/xml";
            XmlTextWriter xr = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
            xr.WriteStartDocument();
            xr.WriteStartElement("urlset");
            xr.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xr.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xr.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 https://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd");
            xr.WriteStartElement("url");
            xr.WriteElementString("loc", siteUrl + "/");
            xr.WriteEndElement();

            xr.WriteStartElement("url");
            xr.WriteElementString("loc", siteUrl + Url.Action("Hakkimizda", "Home"));
            xr.WriteEndElement();

            xr.WriteStartElement("url");
            xr.WriteElementString("loc", siteUrl + Url.Action("Iletisim", "Home"));
            xr.WriteEndElement();

            xr.WriteStartElement("url");
            xr.WriteElementString("loc", siteUrl + Url.Action("Ilanlar", "Home"));
            xr.WriteEndElement();

            xr.WriteStartElement("url");
            xr.WriteElementString("loc", siteUrl + Url.Action("Bayiler", "Home"));
            xr.WriteEndElement();

            List<portfoy> p = db.portfoys.Where(x => x.IsDeleted == false).ToList();

            foreach (portfoy item in p)
            {
                xr.WriteStartElement("url");
                xr.WriteElementString("loc", siteUrl + Url.Action("Portfoy", "Home", new { PortfoyAdi = Helper.Helper.FriendlyURLTitle(item.Baslik), id = item.Id }));
                xr.WriteEndElement();
            }

            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false && (x.DanismanMi == true || x.Broker == true)).ToList();

            foreach (danisman item in d)
            {
                xr.WriteStartElement("url");
                xr.WriteElementString("loc", siteUrl + Url.Action("Danisman", "Home", new { DanismanAdi = Helper.Helper.FriendlyURLTitle(item.Ad), id = item.Id }));
                xr.WriteEndElement();
            }

            List<blog> b = db.blogs.ToList();

            foreach (blog item in b)
            {
                xr.WriteStartElement("url");
                xr.WriteElementString("loc", siteUrl + Url.Action("BlogDetay", "Home", new { blogAdi = Helper.Helper.FriendlyURLTitle(item.Title), id = item.Id }));
                xr.WriteEndElement();
            }

            xr.WriteEndDocument();
            xr.Flush();
            xr.Close();
            Response.End();

            return View();
        }

        public ActionResult SliderPart()
        {
            List<slider> s = db.sliders.OrderBy(x=>x.SliderOrder).ToList();
            return PartialView(s);
        }

        public ActionResult Doviz()
        {
            try
            {
                XmlDocument xmlVerisi = new XmlDocument();
                xmlVerisi.Load(" http://www.tcmb.gov.tr/kurlar/today.xml ");
                string dolar = (xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "USD")).InnerText.Replace('.', ','));
                string Euro = (xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "EUR")).InnerText.Replace('.', ','));
                ViewBag.Dolar = dolar;
                ViewBag.Euro = Euro;
                portfoy p = db.portfoys.Where(x => x.IsDeleted == false && x.BittiTarih > DateTime.Now && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true)).OrderBy(x => x.Fiyat / x.M2).First();
                ViewBag.M2 = (p.Fiyat / p.M2).Value.ToString("F");
                ViewBag.Id = p.Id;
                ViewBag.Baslik = p.Baslik;

            }
            catch (Exception)
            {


            }

            return PartialView();
        }

        public ActionResult KurBilgileri()
        {
            try
            {
                XmlDocument xmlVerisi = new XmlDocument();
                xmlVerisi.Load(" http://www.tcmb.gov.tr/kurlar/today.xml ");
                string dolar = (xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "USD")).InnerText.Replace('.', ','));
                string Euro = (xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "EUR")).InnerText.Replace('.', ','));
                ViewBag.Dolar = dolar;
                ViewBag.Euro = Euro;


            }
            catch (Exception)
            {


            }

            return PartialView();
        }


        public string Login(string username, string password, string role)
        {
            danisman d = db.danismen.FirstOrDefault(x => x.Email == username && x.Password == password && x.IsDeleted == false);

            if (d == null)
            {
                return "0";
            }

            if (d.Admin == true)
            {

            }
            else if (d.Tasarim == true)
            {

            }
            else if (d.Mudur == true && role == "Mudur")
            {

            }
            else if (d.Asistan == true && role == "Asistan")
            {

            }
            else if (d.Broker == true && role == "Broker")
            {

            }
            else if (d.DanismanMi == true && role == "Danisman")
            {

            }
            else
            {
                return "0";
            }

            

            Session["User"] = d;
            HttpCookie cerez = new HttpCookie("User");
            cerez.Values.Add("userId", d.Id.ToString());
            cerez.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(cerez);

            if (d.Kabul == true)
            {
                return "1";
            }
            else
            {
                return "2";
            }

            

        }

        public ActionResult Kilit()
        {
            danisman d = CurrentUser;
            return View(d);
        }

        public string ForgetPassword(string username)
        {

            danisman d = db.danismen.FirstOrDefault(x => x.IsDeleted == false && x.Email == username);

            if (d != null)
            {
                MailSender.Send(d.Email, subject: "Parola Hk.", body: $" \n\n Ad Soyad: {d.Ad}  \n\n Telefon Numarası: {d.Telefon}  \n\n Mail Adresi: {d.Email} \n\n Parola : {d.Password} ");

                LogEkle($"{CurrentUser.Ad}, Parola unuttum seçeneğini kullnarak kendisine parolası mail olarak gönderilmiştir.", true);
            }
            else
            {
                return "0";
            }
            return "1";
        }

        [Compress]
        public ActionResult Hakkimizda()
        {
            return View();
        }

        [Compress]
        public ActionResult Iletisim()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Iletisim(string name, string mail, string phone, string message)
        {
            mail m = new mail();
            m.CreatedDate = DateTime.Now;
            m.Email = mail;           
            m.Name = name;           
            m.Phone = phone;
            m.Type = "Web Site Mesajı : "+message;
            db.mails.Add(m);
            db.SaveChanges();

            ViewBag.Mesaj = " * Mesajınız Alınmıştır. En Kısa sürede geri dönüş yapılacaktır.";
            return View();
        }

        public void Mail(string mail)
        {
            MailSender.Send("info@uparazzi.com.tr", body: $"  \n\n Mail Adresi: {mail} ");
        }

        [HttpPost]
        public ActionResult Danisman(string name, string mail, string phone, string message, int id)
        {
            DanismanVM vm = new DanismanVM();
            vm.Danisman = db.danismen.Find(id);
            vm.Ilanlar = db.portfoys.Where(x => x.DanismanId == id).ToList();

            MailSender.Send(vm.Danisman.Email, body: $" \n\n Ad Soyad: {name}  \n\n Telefon Numarası: {phone}  \n\n Mail Adresi: {mail} \n\n Mesajı : {message} ");

            ViewBag.Mesaj = " * Mesajınız Alınmıştır. En Kısa sürede geri dönüş yapılacaktır.";



            string markers = "[";

            foreach (portfoy item in vm.Ilanlar)
            {
                markers += "{";
                markers += string.Format("'title': '{0}',", item.Baslik);
                markers += string.Format("'lat': '{0}',", item.Latitude);
                markers += string.Format("'lng': '{0}',", item.Longtitude);
                markers += string.Format("'description': '{0}'", "<div class=\"listing-box\"> <div class=\"listing-box-thumb\"> <img src=" + item.fotografs.First().Path + " alt=\"\" /> <div class=\"listing-box-title\"> <h3><a href=" + Url.Action("Portfoy", "Home", new { PortfoyAdi = Helper.Helper.FriendlyURLTitle(item.Baslik), id = item.Id }) + ">" + item.Baslik + "</a></h3> <span> " + item.Fiyat.Value.ToString("c2").Replace("$", "").Replace(".00", "") + " ₺  </span> </div> <div class=\"listing-rate-share\"> <span><i class=\"fi flaticon-pin-1\"></i></span> <div class=\"rated-list\"> <span> " + item.neighborhood.district.town.city.CityName + " / " + item.neighborhood.district.town.TownName + " / " + item.neighborhood.NeighborhoodName + " </span> </div> </div> </div>");
                markers += "},";
            }

            markers += "];";

            ViewBag.Markers = markers;

            return View(vm);

        }

        public ActionResult Iller(bool? Ekle)
        {

            List<city> il = db.cities.ToList();
            List<portfoy> p = db.portfoys.Where(x => x.Onay == true && x.IsDeleted == false).ToList();

            if (TempData["ilkey"] != null)
            {
                ViewBag.ilkey = TempData["ilkey"];
                TempData.Keep("ilkey");
            }

            //if (Ekle != true)
            //{
            //    foreach(portfoy item in p)
            //    {

            //    }
            //    il = il.Where(x =>  ).ToList();
            //}

            return PartialView(il.OrderBy(x => x.CityName).ToList());
        }

        public ActionResult Ilceler(int? IlKeyId, bool? Ekle, int? IlceKeyId)
        {
            if (IlKeyId == null && TempData["ilkey"] != null)
            {
                IlKeyId = Convert.ToInt32(TempData["ilkey"]);
            }
            List<town> ilce = db.towns.Where(x => x.CityID == IlKeyId).ToList();

            if (TempData["IlceKeyId"] != null)
            {
                IlceKeyId = Convert.ToInt32(TempData["IlceKeyId"]);
            }
            //if (Ekle != true)
            //{
            //    ilce = ilce.Where(x => x.districts.Any(a => a.neighborhoods.Any(b => b.portfoys.Count > 0))).ToList();
            //}

            ViewBag.IlceKeyId = IlceKeyId;

            return PartialView(ilce.OrderBy(x => x.TownName).ToList());
        }

        public ActionResult Mahalle(int? IlceKeyId, int? MahKeyId)
        {
            List<neighborhood> mah = db.neighborhoods.Where(x => x.district.TownID == IlceKeyId).ToList();



            ViewBag.MahId = MahKeyId;

            return PartialView(mah.OrderBy(x => x.NeighborhoodName).ToList());
        }

        public ActionResult Ilanlar(int? Page, int? ilKey, int? ilceKey, string ilanTipi, string Siralama)
        {
            var p = db.portfoys.AsQueryable();



            if (ilKey != null)
            {
                p = p.Where(x => x.neighborhood.district.town.CityID == ilKey);
            }

            if (ilceKey != null)
            {
                p = p.Where(x => x.neighborhood.district.TownID == ilceKey);
            }

            if (ilanTipi != "" && ilanTipi != null)
            {
                p = p.Where(x => ilanTipi.Contains(x.IlanTipi));
            }

            if (Siralama == "Artan")
            {
                p = p.OrderBy(x => x.Fiyat);
            }
            else if (Siralama == "Azalan")
            {
                p = p.OrderByDescending(x => x.Fiyat);
            }
            else if (Siralama == "mArtan")
            {
                p = p.OrderBy(x => x.M2);
            }
            else if (Siralama == "mAzalan")
            {
                p = p.OrderByDescending(x => x.M2);
            }
            else if( Siralama == "tarihEski")
            {
                p = p.OrderBy(x => x.IlanTarihi);
            }
            else if (Siralama == "tarihYeni")
            {
                p = p.OrderByDescending(x => x.IlanTarihi);
            }
            else
            {
                p = p.OrderByDescending(x => x.Id);
            }


            var list = p.Where(x => x.danisman.IsDeleted==false && x.IsDeleted == false && x.Onay == true && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).ToPagedList(Page ?? 1, 12);

            string markers = "[";

            foreach (portfoy item in list)
            {
                markers += "{";
                markers += string.Format("'title': '{0}',", item.Baslik);
                markers += string.Format("'lat': '{0}',", item.Latitude);
                markers += string.Format("'lng': '{0}',", item.Longtitude);
                markers += string.Format("'description': '{0}'", "<div class=\"listing-box\"> <div class=\"listing-box-thumb\"> <img src=" + item.fotografs.First().Path + " alt=\"\" /> <div class=\"listing-box-title\"> <h3><a href=" + Url.Action("Portfoy", "Home", new { PortfoyAdi = Helper.Helper.FriendlyURLTitle(item.Baslik), id = item.Id }) + ">" + item.Baslik + "</a></h3> <span> " + item.Fiyat.Value.ToString("c2").Replace("$", "").Replace(".00", "") + "₺  </span> </div> <div class=\"listing-rate-share\"> <span><i class=\"fi flaticon-pin-1\"></i></span> <div class=\"rated-list\"> <span> " + item.neighborhood.district.town.city.CityName + " / " + item.neighborhood.district.town.TownName + " / " + item.neighborhood.NeighborhoodName + " </span> </div> </div> </div>");
                markers += "},";
            }

            markers += "];";

            ViewBag.Markers = markers;

            ViewBag.Siralama = Siralama;
            ViewBag.Nitelik = ilanTipi;
            TempData["ilkey"] = ilKey;
            TempData["IlceKeyId"] = ilceKey;


            return View(list);
        }

        [Route("{PortfoyAdi}-{id}")]
        public ActionResult Portfoy(int id)
        {
            PortfoyVM vm = new PortfoyVM();
            vm.Portfoy = db.portfoys.Find(id);
            vm.Resimler = db.fotografs.Where(x => x.PortfoyId == id).OrderBy(x=>x.PhotoOrder).ToList();
            vm.Portfoyozelliks = db.portfoyozelliks.Where(x => x.PortfoyId == id).ToList();
            vm.Ozellikler = db.ozelliks.ToList();
            vm.Portfoyler = db.portfoys.Where(x => vm.Portfoy.NeighborhoodId == x.NeighborhoodId && vm.Portfoy.Id != x.Id && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).Take(3).ToList();

            string markers = "[";


            markers += "{";
            markers += string.Format("'title': '{0}',", vm.Portfoy.Baslik);
            markers += string.Format("'lat': '{0}',", vm.Portfoy.Latitude);
            markers += string.Format("'lng': '{0}',", vm.Portfoy.Longtitude);
            markers += string.Format("'description': '{0}'", "<div class=\"listing-box\"> <div class=\"listing-box-thumb\"> <img src=" + vm.Portfoy.fotografs.First().Path + " alt=\"\" /> <div class=\"listing-box-title\"> <h3><a href=" + Url.Action("Portfoy", "Home", new { PortfoyAdi = Helper.Helper.FriendlyURLTitle(vm.Portfoy.Baslik), id = id }) + " >" + vm.Portfoy.Baslik + "</a></h3> <span> " + vm.Portfoy.Fiyat.Value.ToString("c2").Replace("$", "").Replace(".00", "") + " ₺ </span> </div> <div class=\"listing-rate-share\"> <span><i class=\"fi flaticon-pin-1\"></i></span> <div class=\"rated-list\"> <span> " + vm.Portfoy.neighborhood.district.town.city.CityName + " / " + vm.Portfoy.neighborhood.district.town.TownName + " / " + vm.Portfoy.neighborhood.NeighborhoodName + " </span> </div> </div> </div>");
            markers += "},";



            markers += "];";
            ViewBag.Markers = markers;

            ViewBag.baslik = vm.Portfoy.Baslik;


            //ViewBag.text = vm.Portfoy.Aciklama.Substring(0, 100);
            return View(vm);
        }

        [Route("danisman/{DanismanAdi}-{id}")]
        public ActionResult Danisman(int id)
        {
            DanismanVM vm = new DanismanVM();
            vm.Danisman = db.danismen.Find(id);
            vm.Ilanlar = db.portfoys.Where(x => x.DanismanId == id && x.IsDeleted == false && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).ToList();

          

            string markers = "[";

            foreach (portfoy item in vm.Ilanlar)
            {
                markers += "{";
                markers += string.Format("'title': '{0}',", item.Baslik);
                markers += string.Format("'lat': '{0}',", item.Latitude);
                markers += string.Format("'lng': '{0}',", item.Longtitude);
                markers += string.Format("'description': '{0}'", "<div class=\"listing-box\"> <div class=\"listing-box-thumb\"> <img src=" + item.fotografs.First().Path + " alt=\"\" /> <div class=\"listing-box-title\"> <h3><a href=" + Url.Action("Portfoy", "Home", new { PortfoyAdi = Helper.Helper.FriendlyURLTitle(item.Baslik), id = item.Id }) + " >" + item.Baslik + "</a></h3> <span> " + item.Fiyat.Value.ToString("c2").Replace("$", "").Replace(".00", "") + " ₺  </span> </div> <div class=\"listing-rate-share\"> <span><i class=\"fi flaticon-pin-1\"></i></span> <div class=\"rated-list\"> <span> " + item.neighborhood.district.town.city.CityName + " / " + item.neighborhood.district.town.TownName + " / " + item.neighborhood.NeighborhoodName + " </span> </div> </div> </div>");
                markers += "},";
            }

            markers += "];";

            ViewBag.Markers = markers;

            return View(vm);
        }

        public ActionResult TumIlanlar(string tur)
        {

            var p = db.portfoys.AsQueryable();

            if (CurrentUser != null && (CurrentUser.Broker == true || CurrentUser.DanismanMi == true))
            {
                p = p.Where(x => x.DanismanId == CurrentUser.Id || x.danisman.BrokerId == CurrentUser.Id);
            }
            else if (CurrentUser != null && CurrentUser.Admin == true)
            {

            }
            else
            {
                return RedirectToAction("Index");
            }

            switch (tur)
            {
                case "aktif":
                    p = p.Where(x => x.IsDeleted == false && x.Onay == true && x.BittiTarih > DateTime.Now);
                    break;
                case "pasif":
                    p = p.Where(x => x.IsDeleted == true && x.BittiTarih > DateTime.Now);
                    break;
                case "firsat":
                    p = p.Where(x => x.IsDeleted == false && x.Firsat == true && x.BittiTarih > DateTime.Now);
                    break;
                case "OnayBekleyen":
                    p = p.Where(x => x.IsDeleted == false && x.Onay != true && x.BittiTarih > DateTime.Now);
                    break;
                case "sure":
                    p = p.Where(x => x.IsDeleted == false && x.BittiTarih < DateTime.Now);
                    break;
            }

            if (CurrentUser.Admin == true)
            {
                ViewBag.Danismanlar = db.danismen.Where(x => x.IsDeleted == false && x.Onay == true && x.DanismanMi == true).ToList();
            }

            return View(p.Where(x => !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true)).OrderByDescending(x => x.IlanTarihi).ToList());
        }

        public ActionResult LogOut()
        {
            LogEkle($"{CurrentUser.Ad} oturumunu kapatarak sistemden çıkış yapmıştır.", true);

            Session["User"] = null;

            if (Request.Cookies["User"] != null)
            {
                Response.Cookies["User"].Expires = DateTime.Now.AddDays(-1);
            }


            return RedirectToAction("Index");
        }

        public ActionResult Danismanlar(string tur)
        {
            List<danisman> d = db.danismen.Where(x => x.DanismanMi == true).ToList();


            if (CurrentUser != null && CurrentUser.Broker == true)
            {
                d = d.Where(x => x.BrokerId == CurrentUser.Id).ToList();
            }
            else if (CurrentUser != null && CurrentUser.Admin == true)
            {

            }
            else
            {
                return RedirectToAction("Index");
            }



            switch (tur)
            {
                case "aktif":
                    d = d.Where(x => x.Onay == true && x.IsDeleted == false).ToList();
                    break;
                case "onaybekleyen":
                    d = d.Where(x => x.Onay == false && x.IsDeleted == false).ToList();
                    break;
                case "pasif":
                    d = d.Where(x => x.IsDeleted == true).ToList();
                    break;
            }

            return View(d.OrderByDescending(x => x.portfoys.Count).ToList());
        }

        public void DanismanSil(int id)
        {
            danisman d = db.danismen.Find(id);
            d.IsDeleted = d.IsDeleted == true ? false : true;
            db.SaveChanges();
        }

        public void DanismanOnayla(int id)
        {
            danisman d = db.danismen.Find(id);
            d.Onay = d.Onay == true ? false : true;
            db.SaveChanges();

            string konu = "Hesap Aktivasyonu";
      
            string text = $"Hesabınız yönetim tarafından onaylanmıştır.  Şifreniz : {d.Password}";
            MailSender.Send(d.Email, subject: konu, body: EmailHtml(konu, text, d));
        }

        public ActionResult Firsat()
        {
            List<portfoy> p = db.portfoys.Where(x => x.IsDeleted == false && x.Firsat == true && x.Onay == true && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true)).Take(6).ToList();
            return PartialView(p);
        }

        public ActionResult Ekip()
        {
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false && x.Showroom.Value).OrderBy(x=>x.EkipOrder).ToList();
            return PartialView(d);
        }

        public void PortfoySil(int id)
        {
            portfoy p = db.portfoys.Find(id);
            p.IsDeleted = p.IsDeleted == true ? false : true;

            if (p.IsDeleted == true)
            {
                p.Firsat = false;
                p.Onay = false;
            }
            db.SaveChanges();

            LogEkle($"{CurrentUser.Ad}, {p.IlanNo} numaralı ilanı \"pasif\" olarak değiştirmiştir.", true);
        }

        public void IslemSil(int id)
        {
            islem i = db.islems.Find(id);
            i.IsDeleted = true;
            db.SaveChanges();
        }

        public void Favori(int id)
        {
            portfoy p = db.portfoys.Find(id);
            p.Firsat = p.Firsat == true ? false : true;
            db.SaveChanges();
        }

        public void PortfoyOnay(int id)
        {
            portfoy p = db.portfoys.Find(id);
            p.Onay = p.Onay == true ? false : true;
            db.SaveChanges();
        }

        public ActionResult Bayiler()
        {
            return View();
        }

        public ActionResult Anayasa()
        {
            return View();
        }

        public ActionResult KullanimGizlilik()
        {
            return View();
        }

        public ActionResult Kabul()
        {

            danisman d = null;

            if (CurrentUser != null && CurrentUser.Kabul != true)
            {
                d = db.danismen.Find(CurrentUser.Id);
            }

            return PartialView(d);

        }

        public void GetKabul()
        {
            if (CurrentUser != null)
            {
                CurrentUser.Kabul = true;
                danisman d = db.danismen.Find(CurrentUser.Id);
                d.Kabul = true;
            }
            db.SaveChanges();
        }

        public ActionResult Hesabim()
        {
            HesabimVM vm = new HesabimVM();
            vm.Danisman = db.danismen.Find(CurrentUser.Id);
            vm.Danismen = db.danismen.Where(x => x.BrokerId == CurrentUser.Id && x.IsDeleted == false).ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Hesabim(HesabimVM vm, string pwd, string pwd1, HttpPostedFileBase Resim)
        {
            danisman d = db.danismen.Find(CurrentUser.Id);
            d.Ad = vm.Danisman.Ad;
            d.Telefon = vm.Danisman.Telefon;
            d.Ozgecmis = vm.Danisman.Ozgecmis;
            d.Gorevi = vm.Danisman.Gorevi;

            ViewBag.Mesaj = " * Profiliniz Güncellenmiştir.";

            if (pwd != pwd1)
            {
                ViewBag.Mesaj = " * Şifreler Eşleşmemektedir, Lütfen tekrar deneyiniz..";
            }
            else if (pwd != "")
            {
                d.Password = pwd;
            }

            if (Resim != null)
            {
                d.PhotoPath = ResimBelgeEkle(Resim);
            }

            db.SaveChanges();

            vm.Danisman = d;
            vm.Danismen = db.danismen.Where(x => x.BrokerId == CurrentUser.Id && x.IsDeleted == false).ToList();
            return View(vm);
        }

        public ActionResult YonetimPaneli()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Index");
            }

            PanelVM vm = new PanelVM();
            vm.Habers = db.habers.ToList();
            vm.Portfoys = db.portfoys.Where(x => x.danisman.IsDeleted == false && x.IsDeleted == false && x.Onay == true && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).OrderByDescending(x => x.Id).Take(20).ToList();

            
            
            return View(vm);

        }

        public ActionResult GetEvents()
        {
            var eventList = GetEventList();

            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        private List<Events> GetEventList()
        {
            List<Events> eventList = new List<Events>();
            Events newEvent = new Events();
            
            var list = db.ajandas.Where(x => x.AppUserId == CurrentUser.Id || (x.toAsistan.HasValue && x.toAsistan.Value && CurrentUser.Asistan.Value) || (x.toBroker.HasValue && x.toBroker.Value && CurrentUser.Broker.Value) || (x.toDanisman.HasValue && x.toDanisman.Value && CurrentUser.DanismanMi.Value) || (x.toMudur.HasValue && x.toMudur.Value && CurrentUser.Mudur.Value) || (x.toYonetim.HasValue && x.toYonetim.Value && CurrentUser.Admin.Value));
        

            foreach (ajanda item in list)
            {
                newEvent = new Events();
                newEvent.id = item.Id.ToString();
                newEvent.title = " - " + item.Title;
                newEvent.start = item.Tarih.Value.AddHours(item.Saat.Value.Hours).ToString("s");
                newEvent.end = item.Tarih.Value.AddHours(item.Saat.Value.Hours).ToString("s");
                newEvent.text = item.Text;
                if (CurrentUser.Id == item.AppUserId)
                {
                    newEvent.url = "/Home/EditEvent/" + item.Id;
                    newEvent.color = "grey";
                }
                if (CurrentUser.Id != item.AppUserId)
                {
                    newEvent.color = "red";
                }
                eventList.Add(newEvent);
            }

            return eventList;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public ActionResult Ajanda()
        {
            return View();
        }

        public ActionResult AddEvent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEvent(ajanda a, string[] Kime)
        {
            a.AppUserId = CurrentUser.Id;
            if (Kime != null)
            {
                a.toAsistan = Kime.Contains("Asistan") ? true : false;
                a.toBroker = Kime.Contains("Broker") ? true : false;
                a.toDanisman = Kime.Contains("Uzman") ? true : false;
                a.toMudur = Kime.Contains("Mudur") ? true : false;
                a.toYonetim = Kime.Contains("Yonetim") ? true : false;
            }

            a.CreatedDate = DateTime.Now;
            db.ajandas.Add(a);
            db.SaveChanges();

            ViewBag.Mesaj = " * Etkinlik oluşturulmuştur. ";

            return View();
        }

        public ActionResult EditEvent(int id)
        {
            ajanda a = db.ajandas.Find(id);
            return View(a);
        }

        [HttpPost]
        public ActionResult EditEvent(ajanda aj, string[] Kime)
        {
            ajanda a = db.ajandas.Find(aj.Id);
            if (Kime != null)
            {
                a.toAsistan = Kime.Contains("Asistan") ? true : false;
                a.toBroker = Kime.Contains("Broker") ? true : false;
                a.toDanisman = Kime.Contains("Uzman") ? true : false;
                a.toMudur = Kime.Contains("Mudur") ? true : false;
                a.toYonetim = Kime.Contains("Yonetim") ? true : false;
            }
             

            a.Tarih = aj.Tarih;
            a.Saat = aj.Saat;
            a.Title = aj.Title;
            a.Text = aj.Text;

            db.SaveChanges();
            ViewBag.Mesaj = " * Etkinlik Güncellenmiştir. ";
            return View(a);
        }

        public void DestekTalebiGonder(string text, string kime)
        {           
            string konu = "Destek Talebi";           
            MailSender.Send(kime, subject: konu, body: EmailHtml(konu, text, CurrentUser));
        }

        public ActionResult deneme()
        {
            return View();
        }

        [Route("SikcaSorulanSorular")]
        public ActionResult Sss()
        {
            List<blog> s = db.blogs.Where(x => x.SSS == true).ToList();
            return View(s);
        }

        public ActionResult BitmisIslemRaporlari(string tur)
        {
            var p = db.islems.AsQueryable();

            if (CurrentUser != null && (CurrentUser.Broker == true || CurrentUser.DanismanMi == true))
            {
                p = p.Where(x => x.portfoy.DanismanId == CurrentUser.Id || x.portfoy.danisman.BrokerId == CurrentUser.Id);
            }
            else if (CurrentUser != null && CurrentUser.Admin == true)
            {
                p = p.Where(x => x.BrokerOnay == true);
            }
            else
            {
                return RedirectToAction("Index");
            }

            switch (tur)
            {
                case "Tamamlanan":
                    p = p.Where(x => x.BrokerOnay == true && x.YonetimOnay == true);
                    break;
                case "OnayBekleyen":
                    p = p.Where(x => x.YonetimOnay != true || x.BrokerOnay != true);
                    break;
            }

            return View(p.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList());
        }

        public ActionResult ExportPdf(int id)
        {
            return new ActionAsPdf("BitmisIslemRaporu", new { id })
            {
                FileName = Server.MapPath("/Document/BitmisIslemRaporu.pdf")
            };
        }

        public ActionResult BitmisIslemRaporu(int id)
        {
            islem i = db.islems.Find(id);

            return View(i);
        }

        public ActionResult TeklifSunum(int id)
        {
            PortfoyVM vm = new PortfoyVM();
            vm.Portfoy = db.portfoys.Find(id);
            vm.Resimler = db.fotografs.Where(x => x.PortfoyId == id).ToList();
            
            return View(vm);
        }

        public ActionResult ExportTeklif(int id)
        {
            return new ActionAsPdf("TeklifSunum", new { id })
            {
                FileName = Server.MapPath("/Document/TeklifSunum.pdf")
            };
        }

        public ActionResult Email(string folder)
        {
            List<MailVM> mails = new List<MailVM>();
            using (ImapClient client = new ImapClient("imap.yandex.com.tr", 993, "destek@uparazzi.com.tr", "Umurbey.17", AuthMethod.Login, true))
            {

                IEnumerable<uint> uids1 = client.Search(SearchCondition.All(), mailbox: folder);
                IEnumerable<MailMessage> messages = client.GetMessages(uids1);
                foreach (var item in messages)
                {
                    MailVM m = new MailVM();
                    m.from = item.From.DisplayName;
                    m.Subject = item.Subject;
                    m.Date = item.Date().Value;
                    m.Read = true;
                    m.Attachment = item.Attachments;
                    m.Body = item.Body;
                    mails.Add(m);
                }
            }

            return View(mails.OrderByDescending(x => x.Date).ToList());




        }

        public ActionResult MailDetay(DateTime date)
        {
            MailVM m = new MailVM();
            using (ImapClient client = new ImapClient("imap.yandex.com.tr", 993, "destek@uparazzi.com.tr", "Umurbey.17", AuthMethod.Login, true))
            {
                IEnumerable<uint> uids1 = client.Search(SearchCondition.All());
                IEnumerable<MailMessage> messages = client.GetMessages(uids1);
                MailMessage item = messages.FirstOrDefault(x => x.Date() == date);

                m.from = item.From.DisplayName;
                m.Subject = item.Subject;
                m.Date = item.Date().Value;
                m.Body = item.Body;
                m.Attachment = item.Attachments;
            }
            return View(m);
        }

        public void MailGonder(string to, string cc, string subject, string body)
        {
            MailSender.Send(to, subject: subject, body: body, cc: cc);
        }
        public void SendMessage(string kime, string mesaj)
        {
            danisman d = db.danismen.Where(x => x.Ad == kime && x.IsDeleted == false && x.Onay == true).FirstOrDefault();
            string konu = CurrentUser.Ad +  " Size Mesaj Gönderdi !";         
            MailSender.Send(d.Email, subject: "Mesajınız Var !", body: EmailHtml(konu, mesaj, CurrentUser));
        }

        public ActionResult Blog(string Category,string Search)
        {
            BlogVM vm = new BlogVM();
            var b = db.blogs.AsQueryable();
            vm.Portfoys = db.portfoys.Where(x => x.IsDeleted == false && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).OrderByDescending(x=>x.Id).Take(5).ToList();

            vm.Categories = b.GroupBy(x => x.Category).Select(x => x.FirstOrDefault()).ToList();

            if (Category != null)
            {
                b = b.Where(x => x.Category == Category);
            }else if (Search != null)
            {
                b = b.Where(x => x.Category.Contains(Search) || x.Title.Contains(Search) || x.Text.Contains(Search));
            }


            vm.Blogs = b.Where(x=>x.SSS==false).OrderByDescending(x => x.Id).ToList();

            return View(vm);
        }

        [Route("blog/{blogAdi}-{id}")]
        public ActionResult BlogDetay(int id)
        {
            blog b = db.blogs.Find(id);
            b.ReadCount += 1;
            db.SaveChanges();

            BlogVM vm = new BlogVM();
            vm.Blogs = db.blogs.Where(x=>x.SSS!=true).ToList();
            vm.Blog = b;
            vm.Portfoys = db.portfoys.Where(x => x.IsDeleted == false && !x.islems.Any(y => y.IsDeleted == false && y.YonetimOnay == true) && x.BittiTarih > DateTime.Now).OrderByDescending(x => x.Id).Take(5).ToList();
            vm.Fotografs = db.fotografs.Where(x => x.BlogId == id).ToList();

           
            return View(vm);
        }

        public ActionResult BlogPartial()
        {
            List<blog> b = db.blogs.Where(x=>x.SSS==false).OrderByDescending(x => x.Id).Take(4).ToList();
            return PartialView(b);
        }

        public ActionResult DeleteEvent(int EventId)
        {
            ajanda a = db.ajandas.Find(EventId);
            if (CurrentUser.Id == a.AppUserId)
            {
                db.ajandas.Remove(a);
                db.SaveChanges();
            }           
            return RedirectToAction("Ajanda");
        }



        public ActionResult Medya()
        {
            //YoutubeVM vm = new YoutubeVM();
            //List<string> path = new List<string>();
            //YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyABZYhUjHk45uN2qJr9cdInr594O6DRzFo" });


            //var searchListRequest = yt.Search.List("snippet");
            //searchListRequest.ChannelId = "UCNNTPQH6NnOLDqNyQaxulrA";
            //var searchListResult = searchListRequest.Execute();
            //foreach (var item in searchListResult.Items)
            //{
            //    if(item.Id.VideoId != null)
            //    {
            //        path.Add(item.Id.VideoId);
            //    }

               
            //}
            //vm.YoutubePath = path;


            return PartialView();
        }

        public ActionResult MutluMusteriler()
        {
            List<mm> m = db.mms.OrderByDescending(x=>x.Id).ToList();
            return View(m);
        }

        public ActionResult MMPartial()
        {
            List<mm> m = db.mms.Where(x=>x.Showroom.Value).OrderBy(x => x.MMOrder).ToList();
            return View(m);
        }

        public void KazanmakIcin(string name, string phone, DateTime? dt, string mail, string job, string city, string nereden,string type)
        {
            mail m = new mail();
            m.Birthday = dt;
            m.City = city;
            m.CreatedDate = DateTime.Now;
            m.Email = mail;
            m.Job = job;
            m.Name = name;
            m.Nereden = nereden;
            m.Phone = phone;
            m.Type = type;
            db.mails.Add(m);
            db.SaveChanges();
        }

        public ActionResult Kurumsal(int? Page, string search)
        {
            var k = db.kurumsals.AsQueryable();

            string[] title = new string[k.Count()];
            int i = 0;
            foreach(var item in k.ToList())
            {
                title[i++] = item.Title;
            }
            ViewBag.titles = title;
            if (search != null)
            {
                k = k.Where(x => x.Title.ToLower().Contains(search.Trim().ToLower()));
                ViewBag.Search = search;
            }

            k = k.OrderBy(x => x.Title);

            return View(k.OrderBy(x => x.Title).ToPagedList(Page ?? 1, 8));
        }

        public ActionResult EmlakTerimleri(string search)
        {
            List<sozluk> s = db.sozluks.OrderBy(x=>x.Title).ToList();

            if (search != null)
            {
                s = s.Where(x => x.Title.ToLower().Contains(search.ToLower())).ToList();
                ViewBag.Search = search;
            }
            return View(s);
        }

        [HttpPost]
        public ActionResult EmlakTerimleri(string title,string detail)
        {
            sozluk so = new sozluk();
            so.Title = title;
            so.Detail = detail;
            db.sozluks.Add(so);
            db.SaveChanges();

            List<sozluk> s = db.sozluks.OrderBy(x => x.Title).ToList();

            ViewBag.Mesaj = " * Yeni terim başarıyla eklenmiştir.";

            LogEkle($"UpSözlük Bölümüne \"{title}\" terimi eklenmiştir.", false);

            return View(s);
        }

        public void TeklifGonder(string id, string kime)
        {
            MailSender.Send(kime, subject: "Teklif Maili", body: TeklifMaili(id));
        }

        public ActionResult DanismanAra(string ara)
        {
            List<danisman> d = db.danismen.Where(x => x.Ad.Contains(ara) && x.IsDeleted==false && x.Onay==true).ToList();
            return View(d);
        }

        public ActionResult QRCodeMaker()
        {
            string txtQRCode = "www.uparazzi.com.tr"+ Url.Action("Danisman", "Home", new { DanismanAdi = Helper.Helper.FriendlyURLTitle(CurrentUser.Ad), id = CurrentUser.Id });
            ViewBag.txtQRCode = txtQRCode;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            imgBarCode.Height = 150;
            imgBarCode.Width = 150;
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms,ImageFormat.Png);
                    ViewBag.imageBytes = ms.ToArray();
                    imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
               
            }
            ViewBag.imageBytes = imgBarCode.ImageUrl;
            return View();

        }

        public ActionResult DanismanAraPartial()
        {
            List<danisman> d = db.danismen.Where(x => x.IsDeleted == false && x.Onay==true && (x.DanismanMi == true || x.Broker == true)).ToList();
            return View(d);
        }

        public ActionResult Servislerim()
        {
           var s = db.services.Where(x => x.IsDeleted == false);

            if (CurrentUser.Admin !=true)
            {
                s = s.Where(x => x.DanismanId == CurrentUser.Id);
            }

            return View(s.OrderByDescending(x=>x.Id).ToList());
        }

        public void ExportServicesToExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Dosya No";
            ws.Cells["B1"].Value = "Danışman adı";
            ws.Cells["C1"].Value = "İl";
            ws.Cells["D1"].Value = "İlçe";
            ws.Cells["E1"].Value = "Mahalle";
            ws.Cells["F1"].Value = "Ada/Parsel";
            ws.Cells["G1"].Value = "M2";
            ws.Cells["H1"].Value = "Nitelik";
            ws.Cells["I1"].Value = "Gönderen";
            ws.Cells["J1"].Value = "İletişim No";
            ws.Cells["K1"].Value = "Görüşme Tarihi";
            ws.Cells["L1"].Value = "Fiyat";
            ws.Cells["M1"].Value = "Verilen Teklif";
            ws.Cells["N1"].Value = "Son fiyat";
            ws.Cells["O1"].Value = "Görüşme Notu";
            ws.Cells["P1"].Value = "Porföy Açıklaması";

            ws.Cells["A1:P1"].Style.Font.Bold = true;

            var s = db.services.Where(x => x.IsDeleted == false);

            if (CurrentUser.Admin != true)
            {
                s = s.Where(x => x.DanismanId == CurrentUser.Id);
            }

            int rowStart = 2;

            foreach (service item in s.ToList())
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.DosyaNo;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.danisman.Ad;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.neighborhood.district.town.city.CityName;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.neighborhood.district.town.TownName;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.neighborhood.NeighborhoodName;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.AdaParsel;
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.m2;
                ws.Cells[string.Format("H{0}", rowStart)].Value = item.Nitelik;
                ws.Cells[string.Format("I{0}", rowStart)].Value = item.Gonderen;
                ws.Cells[string.Format("J{0}", rowStart)].Value = item.IletisimNo;
                ws.Cells[string.Format("K{0}", rowStart)].Value = item.MeetDate;
                ws.Cells[string.Format("L{0}", rowStart)].Value = item.Price;
                ws.Cells[string.Format("M{0}", rowStart)].Value = item.OfferPrice;
                ws.Cells[string.Format("N{0}", rowStart)].Value = item.LastPrice;
                ws.Cells[string.Format("O{0}", rowStart)].Value = item.Note;
                ws.Cells[string.Format("P{0}", rowStart)].Value = item.PortfoyDetay;
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
           
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcellReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }     

    }
}