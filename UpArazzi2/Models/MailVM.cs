using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpArazzi2.Models
{
    public class MailVM
    {
        public int Uid { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public System.Net.Mail.AttachmentCollection Attachment { get; set; }
        public DateTime Date { get; set; }
        public bool Read { get; set; }
    }
}