using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
   public  class LoginModel
    {
        [Required(ErrorMessage = "Enter Login ID")]
        [StringLength(20)]
       public string LoginID { get; set; }


        [Required(ErrorMessage = "Enter Password")]
        [StringLength(20)]
        public string Password { get; set; }
    }
}
