using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpArazzi2.Models
{
    public class IlanlarVM
    {
        public PagedList<portfoy> Ilanlar{ get; set; }
        public List<fotograf> Resimler{ get; set; }
    }
}