﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using UpArazzi2.Models;
using UpArazzi2.Models.Message;

namespace UpArazzi2.Tasks.Jobs
{
    class HatirlatmaJob : IJob
    {       
        UpArazziDBEntities db = new UpArazziDBEntities();
        void MailGonder()
        {
            DateTime day = DateTime.Now.AddDays(1).Date;
         
            List<ajanda> a = db.ajandas.Where(x => x.Tarih.Value == day).ToList();

            if (day.Hour > 10)
            {
                a = a.Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Hour > 10).ToList();
            }

            List<danisman> dlist = db.danismen.Where(x => x.IsDeleted == false).ToList();

            List<danisman> d = new List<danisman>();

            foreach (ajanda item in a)
            {
                if (item.toBroker == true)
                {
                    d.AddRange(dlist.Where(x => x.Broker == true).ToList());
                }

                if (item.toDanisman == true)
                {
                    d.AddRange(dlist.Where(x => x.DanismanMi == true).ToList());
                }

                if (item.toAsistan == true)
                {
                    d.AddRange(dlist.Where(x => x.Asistan == true).ToList());
                }

                if (item.toMudur == true)
                {
                    d.AddRange(dlist.Where(x => x.Mudur == true).ToList());
                }

                if (item.toYonetim == true)
                {
                    d.AddRange(dlist.Where(x => x.Admin == true).ToList());
                }

                d.Add(item.danisman);
            }

            foreach (danisman item in d.Distinct())
            {                
                string konu = "UpAsist Hatırlatma";
                string resim = "https://uparazzi.com.tr" + item.PhotoPath;
                string text = $"Yarın için yaklaşan planlı faaliyetleriniz gözüküyor. Detaylı bilgi için lütfen UpAsist'inizi kontrol ediniz.";
                MailSender.Send(item.Email, subject: konu, body: EmailHtml(resim, konu, text, item));
            }
        }

        public string EmailHtml(string resim, string konu, string text, danisman u)
        {
            string icerik = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\"><head>    <meta charset=\"utf-8\"> <!-- utf-8 works for most cases -->    <meta name=\"viewport\" content=\"width=device-width\"> <!-- Forcing initial-scale shouldn't be necessary -->    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"> <!-- Use the latest (edge) version of IE rendering engine -->    <meta name=\"x-apple-disable-message-reformatting\">  <!-- Disable auto-scale in iOS 10 Mail entirely -->    <title></title> <!-- The title tag shows in email notifications, like Android 4.4. -->    <link href=\"https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700\" rel=\"stylesheet\">    <!-- CSS Reset : BEGIN -->    <style>        html,body {    margin: 0 auto !important;    padding: 0 !important;    height: 100% !important;    width: 100% !important;    background: #f1f1f1;}* {    -ms-text-size-adjust: 100%;    -webkit-text-size-adjust: 100%;}div[style*=\"margin: 16px 0\"] {    margin: 0 !important;}table,td {    mso-table-lspace: 0pt !important;    mso-table-rspace: 0pt !important;}table {    border-spacing: 0 !important;    border-collapse: collapse !important;    table-layout: fixed !important;    margin: 0 auto !important;}img {    -ms-interpolation-mode:bicubic;}a {    text-decoration: none;}*[x-apple-data-detectors],  .unstyle-auto-detected-links *,.aBn {    border-bottom: 0 !important;    cursor: default !important;    color: inherit !important;    text-decoration: none !important;    font-size: inherit !important;    font-family: inherit !important;    font-weight: inherit !important;    line-height: inherit !important;}.a6S {    display: none !important;    opacity: 0.01 !important;}.im {    color: inherit !important;}img.g-img + div {    display: none !important;}@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {    u ~ div .email-container {        min-width: 320px !important;    }}@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {    u ~ div .email-container {        min-width: 375px !important;  }}@media only screen and (min-device-width: 414px) {    u ~ div .email-container {        min-width: 414px !important;    }}    </style>    <style>	    .primary{	background: #17bebb;}.bg_white{	background: #ffffff;}.bg_light{	background: #f7fafa;}.bg_black{	background: #000000;}.bg_dark{	background: rgba(0,0,0,.8);}.email-section{	padding:2.5em;}.btn{	padding: 10px 15px;	display: inline-block;}.btn.btn-primary{	border-radius: 5px;	background: #17bebb;	color: #ffffff;}.btn.btn-white{	border-radius: 5px;	background: #ffffff;	color: #000000;}.btn.btn-white-outline{	border-radius: 5px;	background: transparent;	border: 1px solid #fff;	color: #fff;}.btn.btn-black-outline{	border-radius: 0px;	background: transparent;	border: 2px solid #000;	color: #000;	font-weight: 700;}.btn-custom{	color: rgba(0,0,0,.3);	text-decoration: underline;}h1,h2,h3,h4,h5,h6{	font-family: 'Poppins', sans-serif;	color: #000000;	margin-top: 0;	font-weight: 400;}body{	font-family: 'Poppins', sans-serif;	font-weight: 400;	font-size: 15px;	line-height: 1.8;	color: rgba(0,0,0,.4);}a{	color: #17bebb;}table{}/*LOGO*/.logo h1{	margin: 0;}.logo h1 a{	color: #17bebb;	font-size: 24px;	font-weight: 700;	font-family: 'Poppins', sans-serif;}/*HERO*/.hero{	position: relative;	z-index: 0;}.hero .text{	color: rgba(0,0,0,.3);}.hero .text h2{	color: #000;	font-size: 34px;	margin-bottom: 0;	font-weight: 200;	line-height: 1.4;}.hero .text h3{	font-size: 24px;	font-weight: 300;}.hero .text h2 span{	font-weight: 600;	color: #000;}.text-author{	bordeR: 1px solid rgba(0,0,0,.05);	max-width: 80%;	margin: 0 auto;	padding: 2em;}.text-author img{	border-radius: 50%;	padding-bottom: 20px;}.text-author h3{	margin-bottom: 0;}ul.social{	padding: 0;}ul.social li{	display: inline-block;	margin-right: 10px;}/*FOOTER*/.footer{	border-top: 1px solid rgba(0,0,0,.05);	color: rgba(0,0,0,.5);}.footer .heading{	color: #000;	font-size: 20px;}.footer ul{	margin: 0;	padding: 0;}.footer ul li{	list-style: none;	margin-bottom: 10px;}.footer ul li a{	color: rgba(0,0,0,1);}@media screen and (max-width: 500px) {}    </style></head><body width=\"100%\" style=\"margin: 0;padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;\">	<center style=\"width: 100%; background-color: #f1f1f1;\">    <div style=\"display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;\">   &zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;    </div>    <div style=\"max-width: 600px; margin: 0 auto;\" class=\"email-container\">    	<!-- BEGIN BODY -->      <table align=\"center\" role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" style=\"margin: auto;\">      	<tr>          <td valign=\"top\" class=\"bg_white\" style=\"padding: 1em 2.5em 0 2.5em;\">          	<table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">          		<tr>          			<td class=\"logo\" style=\"text-align: center;\">			              <img src=\"https://uparazzi.com.tr/Theme/images/logo.png\" width=\"100%\"/>			          </td>          		</tr>          	</table>          </td>	      </tr><!-- end tr -->				<tr>          <td valign=\"middle\" class=\"hero bg_white\" style=\"padding: 2em 0 2em 0;\">            <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">            	<tr>            		<td style=\"padding: 0 1em; text-align: center; padding-bottom: 2em;\">            			<div class=\"text\">            				<h2>" + konu + "</h2>            			</div>            		</td>            	</tr>            	<tr>			          <td style=\"text-align: center;\">			          	<div class=\"text-author\">				          	<img src=" + resim + " alt=\"\" style=\"width: 100px; max-width: 600px; height: auto; margin: auto; display: block;\">				          	<h3 class=\"name\">" + u.Ad + "</h3>				          	<span class=\"position\">" + u.Gorevi + "</span>				           	<p></p>				           	<p style='text-align: left;'>" + text + " </p>			           	</div>			          </td>			        </tr>            </table>          </td>	      </tr><!-- end tr -->      <!-- 1 Column Text + Button : END -->      </table>      <table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'>                <tr>                    <td valign='middle' class='bg_light footer email-section'>                        <table>                            <tr>                                                                <td valign='top' width='50%' style='padding-top: 20px;'>                                    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>                                        <tr>                                            <td style='text-align: left; padding-left: 5px; padding-right: 5px;'>                                                <h3 class='heading'>İletişim</h3>                                                <ul>                                                    <li><span class='text'>Soğanlık Yeni Mahalle, AC Moment İstanbul Residence B Blok No:87 , Kartal/İstanbul</span></li>                                                    <li><span class='text'>0850 227 27 79</span></a></li>                                                </ul>                                            </td>                                        </tr>                                    </table>                                </td>                                <td valign='top' width='50%' style='padding-top: 20px;'>                                    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>                                        <tr>                                            <td style='text-align: left; padding-left: 10px;'>                                                <h3 class='heading'>Bağlantılar</h3>                                                <ul>                                                    <li><a href='https://www.uparazzi.com.tr/'>Anasayfa</a></li>                                                    <li><a href='https://www.uparazzi.com.tr/Home/Hakkimizda'>Kurumsal</a></li>                                                    <li><a href='https://www.uparazzi.com.tr/Home/Ilanlar'>İlanlar</a></li>                                                    <li><a href='https://www.uparazzi.com.tr/SikcaSorulanSorular'>SSS</a></li>                                                    <li><a href='https://www.uparazzi.com.tr/Home/Iletisim'>İletişim</a></li>                                                </ul>                                            </td>                                        </tr>                                    </table>                                </td>                            </tr>                        </table>                    </td>                </tr><!-- end: tr -->                <tr>                    <td class='bg_light' style='text-align: center;'>                        <p> © Tüm Hakları Saklıdır.  <a href='https://www.uparazzi.com.tr' style='color: rgba(0,0,0,.8);'>uparazzi.com.tr</a></p>                    </td>                </tr>            </table>    </div>  </center></body></html>";
            return icerik;
        }

      
        public void Execute(IJobExecutionContext context)
        {
            
            try
            {
                MailGonder();
              
            }
            catch (Exception)
            {
               
            }
        }
    }
}   