using System;

namespace SecurityAdministration.BLL
{
    public static class LoginInformation
    {
        public static int UserID { get; set; }
        public static string UserCode { get; set; }
        public static byte? UserType { get; set; }
        public static string FullName { get; set; }
        public static bool IsActive { get; set; }
        public static byte? DesignationID { get; set; }
        public static int SupervisorUserID { get; set; }
        public static string LoginID { get; set; }
        public static string Password { get; set; }
        public static DateTime LastPasswordChangedDate { get; set; }
        public static bool? IsPasswordAccepted { get; set; }
        public static bool? IsLocked { get; set; }
    }
}