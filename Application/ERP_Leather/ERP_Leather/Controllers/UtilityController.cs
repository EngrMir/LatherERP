using ERP.DatabaseAccessLayer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;


namespace ERP_Leather.Controllers
{
    public class UtilityController : Controller
    {
      
        //
        // GET: /Utility/
        private UnitOfWork _repository;
        private SysCommonUtility _utility;
        public UtilityController()
        {
            _utility = new SysCommonUtility();
            _repository = new UnitOfWork();
           
        }
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public bool IsAlreadyExist(string table, string fld, string val, string datatype)
        {
            return _utility.IsValid(table, fld, val, datatype);
        }

        #region Chemical Item Popup Grid
     
        [HttpGet]
        public ActionResult GetItemInfo()
        {
            var result = from t in _repository.SysChemicalItemRepository.Get(filter: ob => ob.IsActive == true).OrderBy(ob => ob.ItemName)
                         select new
                         {
                             ItemID = t.ItemID,
                             ItemCategory = t.ItemCategory,
                             ItemName = t.ItemName,
                             HSCode = t.HSCode
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult GetItemInfo(int itemID)
        {
            var result = from t in _repository.SysChemicalItemRepository.Get(filter: ob => ob.IsActive == true).OrderBy(ob => ob.ItemName)
                         select new
                         {
                             ItemID = t.ItemID,
                             ItemCategory = t.ItemCategory,
                             ItemName = t.ItemName,
                             HSCode = t.HSCode
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

         
        public ActionResult SearchItemByFirstName(string search)
        {
            if (search == null)
            {
                var result = from t in _repository.SysChemicalItemRepository.Get(filter: ob => ob.IsActive == true).OrderBy(ob => ob.ItemName)
                             select new
                             {
                                 ItemID = t.ItemID,
                                 ItemCategory = t.ItemCategory,
                                 ItemName = t.ItemName,
                                 HSCode = t.HSCode
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                search = search.ToUpper();
                var result = from t in _repository.SysChemicalItemRepository.Get(filter: ob => ob.IsActive == true).OrderBy(ob => ob.ItemName)
                             where ((t.ItemName.ToUpper().StartsWith(search) || t.ItemName.ToUpper() == search))
                             select new
                             {
                                 ItemID = t.ItemID,
                                 ItemCategory = t.ItemCategory,
                                 ItemName = t.ItemName,
                                 HSCode = t.HSCode
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

          
        public ActionResult GetItemAutocompleteData()
        {
            var search = from t in _repository.SysChemicalItemRepository.Get(filter: ob => ob.IsActive == true)
                         select new { 
                         ItemName =t.ItemName
                         };

            return Json(search, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult CrustBuyerList()
        {
            var res = from t in _repository.SysBuyerRepository.Get(filter:ob=>ob.BuyerCategory =="Buyer" && ob.IsActive==true) select new { t.BuyerID,t.BuyerName,t.BuyerCode};
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CrustChallanList() 
        {
            var res = from t in _repository.Prd_YearMonthCrustReqItemRepo.Get() select new { t.ArticleChallanID, t.ArticleChallanNo};
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CrustArticleList()
        {
            var res = from t in _repository.Prd_YearMonthCrustReqItemRepo.Get() select new { t.ArticleID, t.ArticleNo };
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPIList()
        {
            var res = from t in _repository.PRQ_ChemicalPIRepository.Get() select new { 
                  PIID= t.PIID,
                    PINo=t.PINo ,
                    PIDate=Convert.ToDateTime(t.PIDate).ToString("dd/MM/yyyy"),
                    RecordStatus=t.RecordStatus=="CNF"?"Confirmed":t.RecordStatus=="CHK"?"Checked":t.RecordStatus=="NCF"?"Not Confirmed":t.RecordStatus,
                    PICategory=t.PICategory=="PI"?"Proforma Invoice":t.PICategory=="IO"? "Indent Order":t.PICategory
            };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPLList()
        {
            var data = from t in _repository.PackingListRepository.Get()
                       select new {
                           PLID = t.PLID,
                           PLNo= t.PLNo,
                           PLDate =Convert.ToDateTime(t.PLDate).ToString("dd/MM/yyyy"),
                           RecordStatus =t.RecordStatus
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBLList()
        {
            var data = from t in _repository.BillOfLadingRepository.Get()
                       select new
                       {
                           BLID = t.BLID,
                           BLNo = t.BLNo,
                           BLDate = Convert.ToDateTime(t.BLDate).ToString("dd/MM/yyyy"),
                           RecordStatus = t.RecordStatus
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCnfBillList()
        {
            var data = from t in _repository.LcmCnFBillRpository.Get()
                       select new
                       {
                           CnfBillID = t.CnfBillID,
                           CnfBillNo = t.CnfBillNo,
                           CnfBillDate = Convert.ToDateTime(t.CnfBillDate).ToString("dd/MM/yyyy"),
                           RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)//t.RecordStatus
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLimList()
        {
            var data = from t in _repository.LimInfoRepository.Get()
                       select new
                       {
                           LimID = t.LimID,
                           LimNo = t.LimNo,
                           LimDate = Convert.ToDateTime(t.LimDate).ToString("dd/MM/yyyy"),
                           RecordStatus = t.RecordStatus
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLCRetirementList()
        {
            var data = from t in _repository.LcmRetirementRpository.Get()
                       select new
                       {
                           LCRetirementID = t.LCRetirementID,
                           LCRetirementNo = t.LCRetirementNo,
                           LCNo = t.LCNo,
                           LCRetirementDate = Convert.ToDateTime(t.LCRetirementDate).ToString("dd/MM/yyyy"),
                           RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetExportCnfBillList()
        {
            var data = from t in _repository.ExpCnfBill.Get()
                       select new
                       {
                           CnfBillID = t.CnfBillID,
                           CnfBillNo = t.CnfBillNo,
                           CnfBillDate = Convert.ToDateTime(t.CnfBillDate).ToString("dd/MM/yyyy"),
                           RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetExportLCRetirementList()
        {
            var data = from t in _repository.EXPLCRetirementRepository.Get()
                       select new
                       {
                           LCRetirementID = t.LCRetirementID,
                           LCRetirementNo = t.LCRetirementNo,
                           LCRetirementDate = Convert.ToDateTime(t.LCRetirementDate).ToString("dd/MM/yyyy"),
                           RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetExportDeliveryChallanList()
        {
            var data = from t in _repository.EXPDeliveryChallanRepository.Get()
                       select new
                       {
                           DeliverChallanID = t.DeliverChallanID,
                           DeliverChallanNo = t.DeliverChallanNo,
                           DeliverChallanDate = Convert.ToDateTime(t.DeliverChallanDate).ToString("dd/MM/yyyy"),
                           RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpPLList()
        {
            var data = from t in _repository.ExpPackingListRepository.Get()
                       select new
                       {
                           PLID = t.PLID,
                           PLNo = t.PLNo,
                           PLDate = Convert.ToDateTime(t.PLDate).ToString("dd/MM/yyyy"),
                           RecordStatus = t.RecordStatus
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetExpCIList()
        {
            var data = from t in _repository.ExpCommercialInvoiceRepository.Get()
                       select new
                       {
                           CIID = t.CIID,
                           CINo = t.CINo,
                           CIDate = Convert.ToDateTime(t.CIDate).ToString("dd/MM/yyyy"),
                           RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CrustLeatherIssueArticle()
        {
            var res = from t in _repository.CrustLeatherIssueItem.Get()
                      join t2 in _repository.ArticleRepository.Get() on t.ArticleID equals t2.ArticleID
                      select new
                      {
                          ArticleID = t.ArticleID,
                          ArticleNo = t2.ArticleNo,
                         ArticleName = t2.ArticleName
                      };
          var data= res.GroupBy(test => test.ArticleID)
                    .Select(grp => grp.First());
          return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrustLeatherChallanList()
        {
            var res = (from t in _repository.CrustLeatherIssueItem.Get() select new { t.ArticleChallanID, t.ArticleChallanNo })
                .GroupBy(ob=>ob.ArticleChallanID).Select(temp=>temp.First());
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinishedLeatherIssueArticle()
        {
            var res = from t in _repository.FinishLeatherIssueItem.Get()
                      join t2 in _repository.ArticleRepository.Get() on t.ArticleID equals t2.ArticleID
                      select new
                      {
                          ArticleID = t.ArticleID,
                          ArticleNo = t2.ArticleNo,
                          ArticleName = t2.ArticleName
                      };
            var data = res.GroupBy(test => test.ArticleID)
                      .Select(grp => grp.First());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinishedLeatherChallanList()
        {
            var res = (from t in _repository.FinishLeatherIssueItem.Get() select new { t.ArticleChallanID, t.ArticleChallanNo })
                .GroupBy(ob => ob.ArticleChallanID).Select(temp => temp.First());
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CrustStoreList()
        {
            var res = (from t in _repository.StoreRepository.Get() where (t.StoreCategory == "Leather" && t.IsDelete==false) select new { t.StoreID, t.StoreCode, t.StoreName });                
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradingSize() {
            var res = (from t in _repository.SysSizeRepository.Get() where (t.SizeCategory == "GradeSize" && t.IsDelete == false) select new { SizeID = t.SizeID, SizeName = t.SizeName }).OrderBy(ob=>ob.SizeID).AsQueryable();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrade()
        {
            var res = (from t in _repository.SysGrade.Get() where (t.IsActive == true && t.IsDelete == false) select new { GradeID = t.GradeID, GradeName = t.GradeName }).OrderBy(ob => ob.GradeID).AsQueryable();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        
        
	}
}