//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpArazzi2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class neighborhood
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public neighborhood()
        {
            this.portfoys = new HashSet<portfoy>();
            this.services = new HashSet<service>();
        }
    
        public int NeighborhoodID { get; set; }
        public int DistrictID { get; set; }
        public string NeighborhoodName { get; set; }
        public string ZipCode { get; set; }
    
        public virtual district district { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<portfoy> portfoys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<service> services { get; set; }
    }
}
