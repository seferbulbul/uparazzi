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
    
    public partial class ajanda
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public Nullable<System.TimeSpan> Saat { get; set; }
        public Nullable<int> AppUserId { get; set; }
        public Nullable<bool> toDanisman { get; set; }
        public Nullable<bool> toBroker { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> toYonetim { get; set; }
        public Nullable<bool> toMudur { get; set; }
        public Nullable<bool> toAsistan { get; set; }
    
        public virtual danisman danisman { get; set; }
    }
}
