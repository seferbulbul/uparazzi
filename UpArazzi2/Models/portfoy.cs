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
    
    public partial class portfoy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public portfoy()
        {
            this.fotografs = new HashSet<fotograf>();
            this.islems = new HashSet<islem>();
            this.portfoyozelliks = new HashSet<portfoyozellik>();
        }
    
        public int Id { get; set; }
        public string Baslik { get; set; }
        public Nullable<int> NeighborhoodId { get; set; }
        public Nullable<decimal> Fiyat { get; set; }
        public Nullable<int> M2 { get; set; }
        public Nullable<decimal> M2Fiyat { get; set; }
        public Nullable<System.DateTime> IlanTarihi { get; set; }
        public string TapuDurumu { get; set; }
        public Nullable<bool> TakasMi { get; set; }
        public Nullable<bool> KatKarsiligi { get; set; }
        public string IlanTipi { get; set; }
        public string AdaNo { get; set; }
        public Nullable<bool> KrediyeUygunluk { get; set; }
        public string PaftaTo { get; set; }
        public string ParselNo { get; set; }
        public string Kimden { get; set; }
        public string Emsal { get; set; }
        public Nullable<bool> KadastralYol { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> Onay { get; set; }
        public Nullable<int> DanismanId { get; set; }
        public string Imar { get; set; }
        public string Gabari { get; set; }
        public string TapuResim { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public Nullable<bool> Firsat { get; set; }
        public Nullable<bool> Satilik { get; set; }
        public string IlanNo { get; set; }
        public string Aciklama { get; set; }
        public string Video { get; set; }
        public Nullable<System.DateTime> BittiTarih { get; set; }
    
        public virtual danisman danisman { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<fotograf> fotografs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<islem> islems { get; set; }
        public virtual neighborhood neighborhood { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<portfoyozellik> portfoyozelliks { get; set; }
    }
}
