using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.DatabaseAccessLayer.Utility;


namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysArticle
    {
        private readonly BLC_DEVEntities _context;


        public int ArticleId = 0;

        public DalSysArticle()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Create(SysArticle objSysArticle, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysArticle = new Sys_Article
                {
                    ArticleID = objSysArticle.ArticleID,
                    ArticleName = objSysArticle.ArticleName,
                    ArticleNo = objSysArticle.ArticleNo,
                    ArticleColor = objSysArticle.ArticleColor,
                    ArticleChallanNo = objSysArticle.ArticleChallanNo,
                    IsActive = objSysArticle.IsActive == "Active",
                    ArticleCategory = objSysArticle.ArticleCategory,
                    Remarks = objSysArticle.Remarks,
                    SetOn = DateTime.Now,
                    SetBy = userid,
                    IPAddress = GetIPAddress.LocalIPAddress()
                };

                _context.Sys_Article.Add(tblSysArticle);
                _context.SaveChanges();
                ArticleId = tblSysArticle.ArticleID;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Save.";
            }
            return vmMsg;
        }

        public long GetArticleID()
        {
            return ArticleId;
        }

        public ValidationMsg Update(SysArticle objSysArticle, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysArticle = _context.Sys_Article.FirstOrDefault(s => s.ArticleID == objSysArticle.ArticleID);
                if (sysArticle != null)
                {
                    sysArticle.ArticleName = objSysArticle.ArticleName;
                    sysArticle.ArticleNo = objSysArticle.ArticleNo;
                    sysArticle.ArticleColor = objSysArticle.ArticleColor;
                    sysArticle.ArticleChallanNo = objSysArticle.ArticleChallanNo;
                    sysArticle.ArticleCategory = objSysArticle.ArticleCategory;
                    sysArticle.Remarks = objSysArticle.Remarks;
                    sysArticle.IsActive = objSysArticle.IsActive == "Active";
                    sysArticle.ModifiedOn = DateTime.Now;
                    sysArticle.ModifiedBy = userid;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Update.";
            }
            return vmMsg;
        }

        public IEnumerable<SysArticle> GetAll()
        {
            IEnumerable<SysArticle> iLstSysArticle = from ss in _context.Sys_Article
                                                     select new SysArticle
                                                            {
                                                                ArticleID = ss.ArticleID,
                                                                ArticleName = ss.ArticleName,
                                                                ArticleNo = ss.ArticleNo,
                                                                ArticleColor = ss.ArticleColor,
                                                                ColorName = ss.ArticleColor == null ? "" : _context.Sys_Color.Where(m => m.ColorID == ss.ArticleColor).FirstOrDefault().ColorName,
                                                                ArticleChallanNo = ss.ArticleChallanNo,
                                                                ArticleCategory = ss.ArticleCategory,
                                                                Remarks = ss.Remarks,
                                                                IsActive = ss.IsActive == true ? "Active" : "Inactive"
                                                            };

            return iLstSysArticle;
        }

        public IEnumerable<SysArticle> GetAllActiveArticle()
        {
            IEnumerable<SysArticle> iLstSysArticle = from ss in _context.Sys_Article
                                                     where ss.IsActive == true
                                                     select new SysArticle
                                                     {
                                                         ArticleID = ss.ArticleID,
                                                         ArticleNo = ss.ArticleNo,
                                                         ArticleName = ss.ArticleName,
                                                         ArticleChallanNo = ss.ArticleChallanNo,
                                                         ArticleColor = ss.ArticleColor,
                                                         ColorName = ss.ArticleColor == null ? "" : _context.Sys_Color.Where(m => m.ColorID == ss.ArticleColor).FirstOrDefault().ColorName
                                                     };

            return iLstSysArticle.OrderBy(o=>o.ArticleName);
        }

        public ValidationMsg Delete(string sourceId, int userid)
        {
            var sourceid = string.IsNullOrEmpty(sourceId) ? 0 : Convert.ToInt32(sourceId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysArticle = _context.Sys_Article.FirstOrDefault(s => s.ArticleID == sourceid);
                if (sysArticle != null)
                {
                    _context.Sys_Article.Remove(sysArticle);
                    _context.SaveChanges();
                    vmMsg.Type = Enums.MessageType.Success;
                    vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }
    }
}
