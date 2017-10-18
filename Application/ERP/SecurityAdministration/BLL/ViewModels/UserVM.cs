using System.Collections.Generic;
using System.Web.Mvc;
using SecurityAdministration.DAL;
using System.ComponentModel.DataAnnotations;

namespace SecurityAdministration.BLL.ViewModels
{
    public class UserVM
    {
        public IEnumerable<UserView> Users { get; set; }
        public UserView User { get; set; }
        public SelectList DesignationList { get; set; }
        public SelectList UserList { get; set; }
        public SelectList SupervisorList { get; set; }
    }

    public class UserView
    {
        public int? UserID { get; set; }
        public string UserCode { get; set; }
        public byte? UserType { get; set ; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public byte? DesignationID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? SupervisorUserID { get; set; }
        public string Description { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
        public string DesignationTitle { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string LastPasswordChangedDate { get; set; }
        public bool? IsPasswordAccepted { get; set; }
        public bool IsLocked { get; set; }
        public string UCI_SetOn { get; set; }
        public int UCI_SetBy { get; set; }

        public string FullName
        {
            get { return Title + " " + FirstName + " " + MiddleName + " " + LastName; }
        }

        public string Supervisor
        {
            get { return FirstName + " " + MiddleName + " " + LastName + " [ " + DesignationTitle + " ] "; }
        }
    }
}