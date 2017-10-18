using System;

namespace SecurityAdministration.BLL
{
    public class Utility
    {
    }

    public class JData
    {
        public int Check { get; set; }
        public object JsonData { get; set; }
        public string Message { get; set; }
    }

    public enum AuditStatusFlag
    {
        Create = 'C',
        Modify = 'M',
        Delete = 'D'
    }
}