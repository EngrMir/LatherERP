using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.DatabaseAccessLayer.OperationGateway;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalTransHead
    {
        private readonly BLC_DEVEntities _context;
        public long HeadID = 0;

        public DalTransHead()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysTransHead objSysHead, int userID)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysHead = new Sys_TransHead
                {
                    HeadName = objSysHead.HeadName,
                    HeadCode = objSysHead.HeadCode,
                    HeadCategory = objSysHead.HeadCategory == "Addition" ? "AD" : "DD",
                    HeadType = DalCommon.ReturnTransHeadType(objSysHead.HeadType),
                    IsActive = objSysHead.IsActive == "Active",
                    SetOn = DateTime.Now,
                    SetBy = userID,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userID,
                    IPAddress = string.Empty
                };

                _context.Sys_TransHead.Add(tblSysHead);
                _context.SaveChanges();
                HeadID = tblSysHead.HeadID;

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Save Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to save.";
            }

            return vmMsg;
        }

        public ValidationMsg Update(SysTransHead objSysHead, int userID)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var Head = _context.Sys_TransHead.FirstOrDefault(s => s.HeadID == objSysHead.HeadID);

                if (Head != null)
                {
                    Head.HeadName = objSysHead.HeadName;
                    Head.HeadCode= objSysHead.HeadCode;
                    Head.HeadCategory = objSysHead.HeadCategory == "Addition" ? "AD" : "DD";
                    Head.HeadType = DalCommon.ReturnTransHeadType(objSysHead.HeadType);
                    Head.IsActive = objSysHead.IsActive == "Active";
                    
                    Head.ModifiedOn = DateTime.Now;
                    Head.ModifiedBy = userID;

                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to update.";
            }

            return vmMsg;
        }

        public IEnumerable<SysTransHead> GetAll()
        {
            IEnumerable<SysTransHead> iLstSysHead = (from sg in _context.Sys_TransHead.AsEnumerable()
                                                    //where sg.IsActive == true
                                                    select new SysTransHead
                                                       {
                                                           HeadID = sg.HeadID,
                                                           HeadName = sg.HeadName,
                                                           HeadCode = sg.HeadCode,
                                                           HeadCategory = sg.HeadCategory == "AD" ? "Addition" : "Deduction",
                                                           HeadType = DalCommon.ReturnTransHeadType(sg.HeadType),
                                                           IsActive = sg.IsActive==true? "Active" : "Inactive",
                                                       }).ToList();

            return iLstSysHead;
        }

        public IEnumerable<SysTransHead> GetAllActiveHead()
        {
            IEnumerable<SysTransHead> listSysHead = (from sg in _context.Sys_TransHead.AsEnumerable()
                                                    where sg.IsActive == true
                                                    select new SysTransHead
                                                    {
                                                        HeadID = sg.HeadID,
                                                        HeadName = sg.HeadName,
                                                        HeadCode = sg.HeadCode,
                                                        HeadCategory = sg.HeadCategory == "AD" ? "Addition" : "Deduction",
                                                        HeadType = DalCommon.ReturnTransHeadType(sg.HeadType),
                                                        IsActive = sg.IsActive==true? "Active" : "Inactive",
                                                    }).ToList();

            return listSysHead;
        }

        //public IEnumerable<SysTransHead> GetCategoryWiseTransHead(string _HeadCategory)
        //{
        //    IEnumerable<SysTransHead> listSysHead = (from sg in _context.Sys_TransHead.AsEnumerable()
        //                                             where sg.IsActive == true & sg.HeadCategory== _HeadCategory
        //                                             select new SysTransHead
        //                                             {
        //                                                 HeadID = sg.HeadID,
        //                                                 HeadName = sg.HeadName,
        //                                             }).ToList();

        //    return listSysHead;
        //}
        public ValidationMsg Delete(string HeadID, int userID)
        {
            var vmMsg = new ValidationMsg();
            try
            {

                var Head = (from g in _context.Sys_TransHead.AsEnumerable()
                            where g.HeadID == Convert.ToInt16(HeadID)
                            select g).FirstOrDefault();

                _context.Sys_TransHead.Remove(Head);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to update.";
            }
            return vmMsg;
        }

        public long GetHeadID()
        {
            return HeadID;
        }


    }
}
