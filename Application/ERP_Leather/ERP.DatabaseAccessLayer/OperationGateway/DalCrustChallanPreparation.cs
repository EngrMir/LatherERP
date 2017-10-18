using System;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.AppSetupModel;
using System.Transactions;
using System.Linq;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalCrustChallanPreparation
    {
        private readonly BLC_DEVEntities _context;

        public DalCrustChallanPreparation()
        {
            _context = new BLC_DEVEntities();
        }


        public long Save(SysArticleChallan model, int userId)
        {
            long CurrentChallanID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        Sys_ArticleChallan objChallan = new Sys_ArticleChallan();

                        objChallan.ArticleChallanNo = model.ArticleChallanNo;
                        objChallan.ChallanNote = model.ChallanNote;
                        objChallan.PreparationDate = DalCommon.SetDate(model.PreparationDate);
                        objChallan.BuyerID = model.BuyerID;
                        objChallan.ArticleID = model.ArticleID;
                        objChallan.ArticleArea = model.ArticleArea;
                        objChallan.AreaUnit = model.AreaUnit;
                        objChallan.ArticleNote = model.ArticleNote;
                        objChallan.RecordStatus = "NCF";
                        objChallan.SetBy = userId;
                        objChallan.ModifiedOn = DateTime.Now;
                        objChallan.IsActive = true;

                        _context.Sys_ArticleChallan.Add(objChallan);
                        _context.SaveChanges();
                        CurrentChallanID = objChallan.ArticleChallanID;

                        if(model.ArticleDetailList != null)
                        {
                            foreach(var article in model.ArticleDetailList)
                            {
                                Sys_ArticleChallanDetail objArticleDetail = new Sys_ArticleChallanDetail();

                                objArticleDetail.ArticleChallanID = CurrentChallanID;
                                objArticleDetail.SizeRange = article.SizeRange;

                                if (article.SizeRangeUnitName != null && article.SizeRangeUnitName != "")
                                    objArticleDetail.SizeRangeUnit = DalCommon.GetUnitCode(article.SizeRangeUnitName);

                                objArticleDetail.PcsSideDescription = article.PcsSideDescription;
                                objArticleDetail.GradeRange = article.GradeRange;
                                objArticleDetail.ThicknessRange = article.ThicknessRange;
                                objArticleDetail.RequiredPercentage = article.RequiredPercentage;

                                if (article.ThicknessUnitName != null && article.ThicknessUnitName != "")
                                    objArticleDetail.ThicknessUnit = DalCommon.GetUnitCode(article.ThicknessUnitName);

                                objArticleDetail.ThicknessAt = (article.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");
                                objArticleDetail.Remarks = article.Remarks;
                                objArticleDetail.SetBy = userId;
                                objArticleDetail.SetOn = DateTime.Now;

                                _context.Sys_ArticleChallanDetail.Add(objArticleDetail);
                                _context.SaveChanges();
                            }
                        }

                        if(model.ColorList!=null)
                        {
                            foreach (var color in model.ColorList)
                            {
                                Sys_ArticleChallanColor objColor = new Sys_ArticleChallanColor();

                                objColor.ArticleChallanID = CurrentChallanID;
                                objColor.ArticleColorNo = color.ArticleColorNo;
                                objColor.ArticleColor = color.ArticleColor;
                                objColor.ArticleColorArea = color.ArticleColorArea;

                                if (color.ColorAreaUnitName != null && color.ColorAreaUnitName != "")
                                    objColor.ColorAreaUnit = DalCommon.GetUnitCode(color.ColorAreaUnitName);

                                if (color.RemarkDate != null)
                                    objColor.RemarkDate = Convert.ToDateTime(color.RemarkDate);
                                objColor.Remarks = color.Remarks;
                                objColor.QuantityDescription = color.QuantityDescription;
                                
                                objColor.SetBy = userId;
                                objColor.SetOn = DateTime.Now;

                                _context.Sys_ArticleChallanColor.Add(objColor);
                                _context.SaveChanges();
                            }
                        }
                    }
                    transaction.Complete();
                }
                return CurrentChallanID;
            }
            catch (Exception e)
            {
                return 0;
            }

        }


        public int Update(SysArticleChallan model, int userId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var Challan = (from c in _context.Sys_ArticleChallan.AsEnumerable()
                                       where c.ArticleChallanID == model.ArticleChallanID
                                       select c).FirstOrDefault();

                        Challan.ArticleChallanNo = model.ArticleChallanNo;
                        Challan.ChallanNote = model.ChallanNote;
                        Challan.PreparationDate = DalCommon.SetDate(model.PreparationDate);
                        Challan.BuyerID = model.BuyerID;
                        Challan.ArticleID = model.ArticleID;
                        Challan.ArticleArea = model.ArticleArea;
                        Challan.AreaUnit = model.AreaUnit;
                        Challan.ArticleNote = model.ArticleNote;
                        Challan.ModifiedBy = userId;
                        Challan.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();

                        if (model.ArticleDetailList != null)
                        {
                            foreach (var article in model.ArticleDetailList)
                            {
                                if(article.ArticleChallanDtlID!=0)
                                {
                                    var ChallanDetail = (from cd in _context.Sys_ArticleChallanDetail.AsEnumerable()
                                                         where cd.ArticleChallanDtlID == article.ArticleChallanDtlID
                                                         select cd).FirstOrDefault();

                                    ChallanDetail.ArticleChallanID = model.ArticleChallanID;
                                    ChallanDetail.SizeRange = article.SizeRange;

                                    if (article.SizeRangeUnitName != null && article.SizeRangeUnitName != "")
                                        ChallanDetail.SizeRangeUnit = DalCommon.GetUnitCode(article.SizeRangeUnitName);

                                    ChallanDetail.PcsSideDescription = article.PcsSideDescription;
                                    ChallanDetail.GradeRange = article.GradeRange;
                                    ChallanDetail.ThicknessRange = article.ThicknessRange;
                                    ChallanDetail.RequiredPercentage = article.RequiredPercentage;

                                    if (article.ThicknessUnitName != null && article.ThicknessUnitName != "")
                                        ChallanDetail.ThicknessUnit = DalCommon.GetUnitCode(article.ThicknessUnitName);

                                    ChallanDetail.ThicknessAt = (article.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");
                                    ChallanDetail.Remarks = article.Remarks;
                                    ChallanDetail.ModifiedBy = userId;
                                    ChallanDetail.ModifiedOn = DateTime.Now;
                                    _context.SaveChanges();

                                }
                                else
                                {
                                    Sys_ArticleChallanDetail objArticleDetail = new Sys_ArticleChallanDetail();

                                    objArticleDetail.ArticleChallanID = model.ArticleChallanID;
                                    objArticleDetail.SizeRange = article.SizeRange;

                                    if (article.SizeRangeUnitName != null && article.SizeRangeUnitName != "")
                                        objArticleDetail.SizeRangeUnit = DalCommon.GetUnitCode(article.SizeRangeUnitName);

                                    objArticleDetail.PcsSideDescription = article.PcsSideDescription;
                                    objArticleDetail.GradeRange = article.GradeRange;
                                    objArticleDetail.ThicknessRange = article.ThicknessRange;

                                    if (article.ThicknessUnitName != null && article.ThicknessUnitName != "")
                                        objArticleDetail.ThicknessUnit = DalCommon.GetUnitCode(article.ThicknessUnitName);
                                    
                                    objArticleDetail.RequiredPercentage = article.RequiredPercentage;
                                    objArticleDetail.ThicknessAt = (article.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");
                                    objArticleDetail.Remarks = article.Remarks;
                                    objArticleDetail.SetBy = userId;
                                    objArticleDetail.SetOn = DateTime.Now;

                                    _context.Sys_ArticleChallanDetail.Add(objArticleDetail);
                                    _context.SaveChanges();
                                }
                            }
                        }


                        if (model.ColorList != null)
                        {
                            foreach (var color in model.ColorList)
                            {
                                if (color.ArticleChallanIDColor != 0)
                                {
                                    var ColorDetail = (from cd in _context.Sys_ArticleChallanColor.AsEnumerable()
                                                         where cd.ArticleChallanIDColor == color.ArticleChallanIDColor
                                                         select cd).FirstOrDefault();

                                    ColorDetail.ArticleChallanID = model.ArticleChallanID;
                                    ColorDetail.ArticleColorNo = color.ArticleColorNo;
                                    ColorDetail.ArticleColor = color.ArticleColor;
                                    ColorDetail.ArticleColorArea = color.ArticleColorArea;
                                    ColorDetail.QuantityDescription = color.QuantityDescription;

                                    if (color.ColorAreaUnitName != null && color.ColorAreaUnitName != "")
                                        ColorDetail.ColorAreaUnit = DalCommon.GetUnitCode(color.ColorAreaUnitName);

                                    if(color.RemarkDate!=null)
                                    {
                                        try
                                        {
                                            var GridRemarkDate = color.RemarkDate.Contains("/") ? color.RemarkDate : Convert.ToDateTime(color.RemarkDate).ToString("dd/MM/yyyy");
                                            ColorDetail.RemarkDate = DalCommon.SetDate(GridRemarkDate);
                                        }
                                        catch
                                        {
                                            var GridRemarkDate = Convert.ToDateTime(color.RemarkDate).Date.ToString("dd/MM/yyyy");
                                            ColorDetail.RemarkDate = DalCommon.SetDate(GridRemarkDate);
                                        }
                                    }
                                    //else
                                    //{
                                    //    ColorDetail.RemarkDate = null;
                                    //}
                                   





                                    //if (color.RemarkDate != null)
                                    //    ColorDetail.RemarkDate = Convert.ToDateTime(color.RemarkDate);


                                    ColorDetail.Remarks = color.Remarks;

                                    ColorDetail.ModifiedBy = userId;
                                    ColorDetail.ModifiedOn = DateTime.Now;
                                    _context.SaveChanges();

                                }
                                else
                                {
                                    Sys_ArticleChallanColor objColor = new Sys_ArticleChallanColor();

                                    objColor.ArticleChallanID = model.ArticleChallanID;
                                    objColor.ArticleColorNo = color.ArticleColorNo;
                                    objColor.ArticleColor = color.ArticleColor;
                                    objColor.ArticleColorArea = color.ArticleColorArea;
                                    objColor.QuantityDescription = color.QuantityDescription;

                                    if (color.ColorAreaUnitName != null && color.ColorAreaUnitName != "")
                                        objColor.ColorAreaUnit = DalCommon.GetUnitCode(color.ColorAreaUnitName);

                                    if (color.RemarkDate != null)
                                        objColor.RemarkDate = Convert.ToDateTime(color.RemarkDate);
                                    objColor.Remarks = color.Remarks;

                                    objColor.SetBy = userId;
                                    objColor.SetOn = DateTime.Now;

                                    _context.Sys_ArticleChallanColor.Add(objColor);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        

                    }

                    transaction.Complete();


                }
                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public List<SysArticleChallanDetail> GetArticleDetailListAfterSave(long ArticleChallanID)
        {
            using(var context= new BLC_DEVEntities())
            {
                var Data = (from d in context.Sys_ArticleChallanDetail.AsEnumerable()
                            where d.ArticleChallanID == ArticleChallanID

                            join u in _context.Sys_Unit on d.SizeRangeUnit equals u.UnitID into Units
                            from u in Units.DefaultIfEmpty()

                            join tu in _context.Sys_Unit on d.ThicknessUnit equals tu.UnitID into ThicknessUnits
                            from tu in ThicknessUnits.DefaultIfEmpty()

                            select new SysArticleChallanDetail
                            {
                                ArticleChallanDtlID = d.ArticleChallanDtlID,
                                SizeRange = d.SizeRange,
                                PcsSideDescription = d.PcsSideDescription,
                                GradeRange = d.GradeRange,
                                ThicknessRange = d.ThicknessRange,
                                ThicknessAt = (d.ThicknessAt == "AFFN" ? "After Finishing" : "After Shaving"),
                                SizeRangeUnit = d.SizeRangeUnit,
                                SizeRangeUnitName = (u == null ? "SFT" : u.UnitName),
                                RequiredPercentage= d.RequiredPercentage,
                                ThicknessUnit=d.ThicknessUnit,
                                ThicknessUnitName= (tu==null?"SFT":tu.UnitName),
                                Remarks= d.Remarks


                            }).ToList();

                return Data;
            }
        }


        public List<SysArticleChallanColor> GetArticleColorListAfterSave(long ArticleChallanID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from d in context.Sys_ArticleChallanColor.AsEnumerable()
                            where d.ArticleChallanID == ArticleChallanID

                            join c in _context.Sys_Color on d.ArticleColor equals c.ColorID into Colors
                            from c in Colors.DefaultIfEmpty()

                            join u in _context.Sys_Unit on d.ColorAreaUnit equals u.UnitID into Units
                            from u in Units.DefaultIfEmpty()

                            orderby d.ArticleColorNo ascending
                            select new SysArticleChallanColor
                            {
                                ArticleChallanIDColor = d.ArticleChallanIDColor,
                                ArticleColorNo = d.ArticleColorNo,
                                ArticleColor = d.ArticleColor,
                                ArticleColorName = (c == null ? null : c.ColorName),
                                ColorAreaUnit = d.ColorAreaUnit,
                                ColorAreaUnitName = (u == null ? "SFT" : u.UnitName),
                                ArticleColorArea = d.ArticleColorArea,
                                RemarkDate = d.RemarkDate == null ? null : Convert.ToDateTime(d.RemarkDate).ToString("dd'/'MM'/'yyyy"),
                                Remarks = d.Remarks,
                                QuantityDescription = d.QuantityDescription,
                            }).ToList();

                return Data;
            }
        }

        public List<SysArticleChallan> GetSearchInformation()
        {
            using(_context)
            {
                var Data = (from c in _context.Sys_ArticleChallan.AsEnumerable()

                            join b in _context.Sys_Buyer on c.BuyerID equals b.BuyerID into Buyers
                            from b in Buyers.DefaultIfEmpty()

                            join a in _context.Sys_Article on c.ArticleID equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            orderby c.ArticleChallanID descending
                            select new SysArticleChallan
                            {
                                ArticleChallanID = c.ArticleChallanID,
                                ArticleChallanNo = c.ArticleChallanNo,
                                BuyerID = c.BuyerID,
                                BuyerCode= (b==null?null:b.BuyerCode),
                                BuyerName = (b == null ? null : b.BuyerName),
                                ArticleID = c.ArticleID,
                                ArticleName= (a==null?null:a.ArticleName),
                                PreparationDate = Convert.ToDateTime(c.PreparationDate).ToString("dd'/'MM'/'yyyy"),
                                RecordStatus= DalCommon.ReturnRecordStatus(c.RecordStatus),
                                ChallanNote= c.ChallanNote,
                                ArticleNote= c.ArticleNote,
                                ArticleArea= c.ArticleArea
                            }).ToList();

                return Data;
            }
        }

        public bool DeleteColorItem(string _ArticleChallanIDColor)
        {
            try
            {
                var Color = (from c in _context.Sys_ArticleChallanColor.AsEnumerable()
                             where c.ArticleChallanIDColor.ToString() == _ArticleChallanIDColor
                             select c).FirstOrDefault();

                _context.Sys_ArticleChallanColor.Remove(Color);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteArticleDetail(string _ArticleChallanDtlID)
        {
            try
            {
                var ArticleDetail = (from c in _context.Sys_ArticleChallanDetail.AsEnumerable()
                                     where c.ArticleChallanDtlID.ToString() == _ArticleChallanDtlID
                                     select c).FirstOrDefault();

                _context.Sys_ArticleChallanDetail.Remove(ArticleDetail);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteChallan(string _ArticleChallanID)
        {
            try
            {
                var Purchase = (from c in _context.Sys_ArticleChallan.AsEnumerable()
                                where c.ArticleChallanID.ToString() == _ArticleChallanID
                                select c).FirstOrDefault();

                _context.Sys_ArticleChallan.Remove(Purchase);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ConfirmArticleChallan(string _ArticleChallanID, string _ConfirmNote)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var ArticleChallanInfo = (from p in _context.Sys_ArticleChallan.AsEnumerable()
                                                  where (p.ArticleChallanID).ToString() == _ArticleChallanID
                                                  select p).FirstOrDefault();

                        ArticleChallanInfo.RecordStatus = "CNF";
                        _context.SaveChanges();
                    }
                    Transaction.Complete();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
    }
}
