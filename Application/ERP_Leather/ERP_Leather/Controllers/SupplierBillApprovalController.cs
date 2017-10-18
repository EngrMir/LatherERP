using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class SupplierBillApprovalController : Controller
    {
        // GET: SupplierBillApproval

        [CheckUserAccess("SupplierBillApproval/SupplierBillApproval")]
        public ActionResult SupplierBillApproval()
        {
            return View();
        }
    }
}