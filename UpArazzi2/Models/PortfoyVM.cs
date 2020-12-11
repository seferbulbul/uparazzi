using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpArazzi2.Models
{
    public class PortfoyVM
    {
        public portfoy Portfoy { get; set; }
        public List<fotograf> Resimler { get; set; }
        public List<portfoy> Portfoyler { get; set; }
        public List<ozellik> Ozellikler { get; set; }
        public List<portfoyozellik> Portfoyozelliks { get; set; }
    }
}