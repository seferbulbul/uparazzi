using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpArazzi2.Models
{
    public class BlogVM
    {
        public List<blog> Blogs { get; set; }
        public blog Blog { get; set; }
        public List<portfoy> Portfoys  { get; set; }
        public List<fotograf> Fotografs  { get; set; }
        public List<blog> Categories  { get; set; }
    }
}