using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;
using System.Data.OleDb;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ERP_Leather.Controllers
{
    public class EXPPackingListController : Controller
    {
        private DalEXPPackingList Dalobject;
        private ValidationMsg _vmMsg;
        DalSysUnit objUnit = new DalSysUnit();
        public EXPPackingListController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalEXPPackingList();
        }
        [CheckUserAccess("EXPPackingList/EXPPackingList")]
        public ActionResult EXPPackingList()
        {
            ViewBag.formTiltle = "Packing List";
            ViewBag.ddlUnitList = objUnit.GetAllActiveUnit();
            return View();
        }

        [HttpPost]
        public ActionResult EXPPackingList(EXPPackingList model)
        {
            _vmMsg = model.PLID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "EXPPackingList/EXPPackingList") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { PLID = Dalobject.GetPLID(), PLNo = Dalobject.GetPLNo(), PLPIID = Dalobject.GetPLPIID(), msg = _vmMsg });
        }

        public ActionResult GetCIPIColorInformation(string CIPIID)
        {
            return Json(Dalobject.GetCIPIColorInformation(CIPIID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPIColorInformation(string PIID)
        {
            return Json(Dalobject.GetPIColorInformation(PIID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingListInformation()
        {
            return Json(Dalobject.GetPackingListInformation(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPLPIInformation(string PLID)
        {
            return Json(Dalobject.GetPLPIInformation(PLID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpPLPIItemColorBaleList(string PLPIItemColorID)
        {
            return Json(Dalobject.GetExpPLPIItemColorBaleList(PLPIItemColorID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllGridList(string PLPIID)
        {
            EXPPackingList model = new EXPPackingList();
            if (string.IsNullOrEmpty(PLPIID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.EXPPLPIItemColorList = Dalobject.GetExpPLPIItemColorList(PLPIID);
            if (model.EXPPLPIItemColorList.Count > 0)
                model.EXPPLPIItemColorBaleList = Dalobject.GetExpPLPIItemColorBaleList(model.EXPPLPIItemColorList[0].PLPIItemColorID.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadExcelFile(string fileName)
        {
            //return Json(Dalobject.LoadExcelFile(fileName), JsonRequestBehavior.AllowGet);
            try
            {
                if (string.IsNullOrEmpty(TempData["file"].ToString()))
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                var results = new List<EXPPLPIItemColorBale>();

                string connectionString = "";
                string filename = Path.Combine(Server.MapPath("~/Content"), TempData["file"].ToString());
                //string filename = @"C:\Test\" + TempData["file"].ToString();
                string[] d = filename.Split('.');
                string fileExtension = "." + d[d.Length - 1].ToString();
                if (d.Length > 0)
                {
                    if (fileExtension == ".xls")
                    {
                        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    }

                    //  connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", filename);
                }

                string query = String.Format("SELECT * from [{0}$]", "Sheet1");
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, connectionString);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                DataTable myDataTable = dataSet.Tables[0];

                foreach (DataRow dr in myDataTable.Rows)
                {
                    EXPPLPIItemColorBale objColorBale = new EXPPLPIItemColorBale();

                    objColorBale.PLPIItemColorBaleNo = dr["BaleNo"].ToString();
                    objColorBale.GradeName = dr["Grade"].ToString();

                    if (string.IsNullOrEmpty(dr["Pcs"].ToString()))
                        objColorBale.PcsInBale = null;
                    else
                        objColorBale.PcsInBale = Convert.ToDecimal(dr["Pcs"].ToString());

                    if (string.IsNullOrEmpty(dr["Side"].ToString()))
                        objColorBale.SideInBale = null;
                    else
                        objColorBale.SideInBale = Convert.ToDecimal(dr["Side"].ToString());

                    if (string.IsNullOrEmpty(dr["SFT"].ToString()))
                        objColorBale.FootPLPIBaleQty = null;
                    else
                        objColorBale.FootPLPIBaleQty = Convert.ToDecimal(dr["SFT"].ToString());


                    if (string.IsNullOrEmpty(dr["SMT"].ToString()))
                        objColorBale.MeterPLPIBaleQty = null;
                    else
                        objColorBale.MeterPLPIBaleQty = Convert.ToDecimal(dr["SMT"].ToString());


                    if (string.IsNullOrEmpty(dr["NetWeight"].ToString()))
                        objColorBale.PLPIBaleNetWeight = null;
                    else
                        objColorBale.PLPIBaleNetWeight = Convert.ToDecimal(dr["NetWeight"].ToString());


                    if (string.IsNullOrEmpty(dr["GrossWeight"].ToString()))
                        objColorBale.PLPIBGrossaleWeight = null;
                    else
                        objColorBale.PLPIBGrossaleWeight = Convert.ToDecimal(dr["GrossWeight"].ToString());

                    //objColorBale.GrossBaleWeightUnitName = dr["WeightUnit"].ToString();

                    results.Add(objColorBale);
                }
                Remove(TempData["file"].ToString());
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vmMsg.Msg = ex.Message.ToString();
                //return _vmMsg.Msg;
            }

            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile(IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.FileName = "";
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/Content"), fileName);
                    file.SaveAs(physicalPath);
                    TempData["file"] = fileName;
                    break;
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (!string.IsNullOrEmpty(fileNames))
            {
                var fileName = Path.GetFileName(fileNames);
                var physicalPath = Path.Combine(Server.MapPath("~/Content"), fileName);
                // TODO: Verify user permissions
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedPackingList(string PLID)
        {
            long? pLID = Convert.ToInt64(PLID);
            _vmMsg = Dalobject.DeletedPackingList(pLID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedEXPPLPIItemColor(string PLPIItemColorID)
        {
            long? pLPIItemColorID = Convert.ToInt64(PLPIItemColorID);
            _vmMsg = Dalobject.DeletedEXPPLPIItemColor(pLPIItemColorID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedEXPPLPIItemColorBale(string PLPIItemColorBaleID)
        {
            long? pLPIItemColorBaleID = Convert.ToInt64(PLPIItemColorBaleID);
            _vmMsg = Dalobject.DeletedEXPPLPIItemColorBale(pLPIItemColorBaleID);
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult ConfirmedEXPPackingList(EXPPackingList model)
        {
            _vmMsg = Dalobject.ConfirmedEXPPackingList(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
    }
}