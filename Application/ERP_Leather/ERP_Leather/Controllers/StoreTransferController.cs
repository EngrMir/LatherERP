using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class StoreTransferController : Controller
    {
        [CheckUserAccess("StoreTransfer/StoreTransfer")]
        public ActionResult StoreTransfer()
        {
            return View();
        }
	}
}