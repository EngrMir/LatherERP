//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SecurityAdministration.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.AccessLogs = new HashSet<AccessLog>();
            this.AdditionalOperationPermissions = new HashSet<AdditionalOperationPermission>();
            this.AdditionalScreenPermissions = new HashSet<AdditionalScreenPermission>();
            this.UserInRoles = new HashSet<UserInRole>();
        }
    
        public int UserID { get; set; }
        public string UserCode { get; set; }
        public Nullable<byte> UserType { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<byte> DesignationID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int SupervisorUserID { get; set; }
        public string Description { get; set; }
        public System.DateTime SetOn { get; set; }
        public int SetBy { get; set; }
    
        public virtual ICollection<AccessLog> AccessLogs { get; set; }
        public virtual ICollection<AdditionalOperationPermission> AdditionalOperationPermissions { get; set; }
        public virtual ICollection<AdditionalScreenPermission> AdditionalScreenPermissions { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual UserCredentialInformation UserCredentialInformation { get; set; }
        public virtual ICollection<UserInRole> UserInRoles { get; set; }
    }
}