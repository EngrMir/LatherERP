//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DatabaseAccessLayer.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sys_GradeRange
    {
        public Sys_GradeRange()
        {
            this.Sys_GradeRangeFormat = new HashSet<Sys_GradeRangeFormat>();
        }
    
        public short GradeRangeID { get; set; }
        public string GradeRangeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    
        public virtual ICollection<Sys_GradeRangeFormat> Sys_GradeRangeFormat { get; set; }
    }
}
