using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Web.Mvc;
using System.Linq;
using ERP_Leather.ActionFilters;


namespace ERP_Leather.Controllers
{
    public class CountryController : Controller
    {
        private DalSysCountry _dalSysCountry;
        private ValidationMsg _vmMsg;

        public CountryController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysCountry = new DalSysCountry();
        }

        [CheckUserAccess("Country/Country")]
        public ActionResult Country()
        {
            ViewBag.formTiltle = "Country Form";
            return View();
        }

        [HttpPost]
        public ActionResult Country(SysCountry model)
        {
            if (model != null && model.CountryID != 0)
            {
                _vmMsg = _dalSysCountry.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysCountry.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { CountryID = _dalSysCountry.GetCountryID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Delete(string CountryID)
        {
            _vmMsg = _dalSysCountry.Delete(CountryID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCountryList()
        {
            var sysCountry = _dalSysCountry.GetAll().OrderByDescending(s => s.CountryID);
            return Json(sysCountry, JsonRequestBehavior.AllowGet);
        }
	}
}