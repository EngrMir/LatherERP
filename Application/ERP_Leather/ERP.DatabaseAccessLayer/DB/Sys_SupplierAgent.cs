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
    
    public partial class Sys_SupplierAgent
    {
        public int SupplierAgentID { get; set; }
        public Nullable<int> AgentID { get; set; }
        public int SupplierID { get; set; }
        public string AgentType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    
        public virtual Sys_Supplier Sys_Supplier { get; set; }
        public virtual Sys_Supplier Sys_Supplier1 { get; set; }
    }
}