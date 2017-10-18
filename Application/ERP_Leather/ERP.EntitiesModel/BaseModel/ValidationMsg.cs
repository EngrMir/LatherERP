using System;
using System.Collections.Generic;

namespace ERP.EntitiesModel.BaseModel
{
    public class ValidationMsg
    {
        public Enums.MessageType Type { get; set; }
        public string Msg { get; set; }
        public long ReturnId { get; set; }
        public string ReturnCode { get; set; }
        public List<string> LstErrors { get; set; }
        public ValidationMsg()
        {
            LstErrors = new List<string>();
            Type = Enums.MessageType.None;
        }
    }
}
