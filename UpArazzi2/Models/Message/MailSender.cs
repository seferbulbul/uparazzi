using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace UpArazzi2.Models.Message
{
    public static class MailSender
    {
        public static void Send(string receiver,string password="Umurbey.17",string body="İçerik",string subject="Web Site İletişim",string sender="info@uparazzi.com.tr",string cc="")
        {
            MailAddress senderMail = new MailAddress(sender);
            //MailAddress receiverMail = new MailAddress(receiver);
       
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.yandex.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sender, password)
            };

            //using (MailMessage mesaj = new MailMessage(senderMail, receiverMail) {              
            //    Subject=subject,
            //    Body = body
            //})

            MailMessage mesaj = new MailMessage();
            mesaj.From = senderMail;
            mesaj.To.Add(receiver);
            if(cc != "")
            {
                mesaj.CC.Add(cc);
            }
            
            mesaj.Subject = subject;
            mesaj.Body = body;


            {
                mesaj.IsBodyHtml = true;
                smtp.Send(mesaj);
            }
        }
    }
}