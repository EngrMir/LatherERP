using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalPreDefineValue
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public int PreDefineValueForID = 0;

        private int exitidentuty = 0;
        private int sameidentuty = 0;

        private string group = string.Empty;
        public DalPreDefineValue()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysPreDefineValueFor model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region PreDefineValueFor

                        Sys_PreDefineValueFor tblSysPreDefineValueFor = SetToModelObject(model, userid);
                        _context.Sys_PreDefineValueFor.Add(tblSysPreDefineValueFor);
                        _context.SaveChanges();
                        PreDefineValueForID = tblSysPreDefineValueFor.PreDefineValueForID;

                        #endregion

                        #region Save PreDefineValue

                        if (model.PreDefineValueList != null)
                        {
                            foreach (SysPreDefineValue objSysPreDefineValue in model.PreDefineValueList)
                            {
                                objSysPreDefineValue.PreDefineValueForID = PreDefineValueForID;
                                objSysPreDefineValue.IsActive = string.IsNullOrEmpty(objSysPreDefineValue.IsActive)
                                    ? "Active"
                                    : objSysPreDefineValue.IsActive;

                                @group = objSysPreDefineValue.PreDefineValueGroup == "Identity No" ? "1" : "0";

                                if (group == "1")
                                {
                                    var exitData = _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == PreDefineValueForID && m.PreDefineValueGroup == "1" && m.IsActive == true).ToList();
                                    if (exitData.Count > 0)
                                    {
                                        exitidentuty = 1;
                                    }
                                    else
                                    {
                                        Sys_PreDefineValue tblPreDefineValue = SetToModelObject(objSysPreDefineValue, userid);
                                        var sameData = _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == PreDefineValueForID && m.PreDefineValueGroup == "1" && m.PreDefineValueContent == objSysPreDefineValue.PreDefineValueContent && m.PreDefineValueIncreaseBy == tblPreDefineValue.PreDefineValueIncreaseBy).ToList();
                                        if (sameData.Count > 0)
                                        {
                                            sameidentuty = 1;
                                        }
                                        else
                                        {
                                            _context.Sys_PreDefineValue.Add(tblPreDefineValue);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    Sys_PreDefineValue tblPreDefineValue = SetToModelObject(objSysPreDefineValue, userid);
                                    _context.Sys_PreDefineValue.Add(tblPreDefineValue);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        #endregion

                        if (exitidentuty == 0)
                        {
                            if (sameidentuty == 0)
                            {
                                tx.Complete();
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Saved Successfully.";
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Same Combination Found.";
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "An Active Data Found Same Group.";
                        }

                        //if ((exitidentuty == 0) && (sameidentuty == 0))
                        //{
                        //    //_context.SaveChanges();
                        //    tx.Complete();
                        //    _vmMsg.Type = Enums.MessageType.Success;
                        //    _vmMsg.Msg = "Saved Successfully.";
                        //}
                        //else
                        //{
                        //    _vmMsg.Type = Enums.MessageType.Error;
                        //    _vmMsg.Msg = "Please Enter Only One Identity Group.";
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {

                    if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Concern Page Code Already Exit..";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Failed to save.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to save.";
                }
            }
            return _vmMsg;
        }

        public Sys_PreDefineValueFor SetToModelObject(SysPreDefineValueFor model, int userid)
        {
            Sys_PreDefineValueFor Entity = new Sys_PreDefineValueFor();

            Entity.PreDefineValueForID = model.PreDefineValueForID;
            Entity.PreDefineValueFor = model.PreDefineValueFor;
            Entity.ConcernPageID = model.ConcernPageID;
            Entity.Remarks = model.Remarks;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public Sys_PreDefineValue SetToModelObject(SysPreDefineValue model, int userid)
        {
            Sys_PreDefineValue Entity = new Sys_PreDefineValue();

            Entity.PreDefineValueID = model.PreDefineValueID;
            Entity.PreDefineValueForID = model.PreDefineValueForID;
            Entity.PreDefineValueTitle = model.PreDefineValueTitle;
            Entity.PreDefineValueContent = model.PreDefineValueContent;
            switch (model.PreDefineValueGroup)
            {
                case "Identity No":
                    Entity.PreDefineValueGroup = "1";
                    if (model.PreDefineValueIncreaseBy == "Yearly")
                    {
                        Entity.PreDefineValueIncreaseBy = "Y";
                        Entity.MaxValue = model.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + "00" + "00" + "0000";
                    }
                    else if (model.PreDefineValueIncreaseBy == "Monthly")
                    {
                        Entity.PreDefineValueIncreaseBy = "M";
                        Entity.MaxValue = model.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + "00" + "0000";
                    }
                    else if (model.PreDefineValueIncreaseBy == "Daily")
                    {
                        Entity.PreDefineValueIncreaseBy = "D";
                        Entity.MaxValue = model.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + "0000";
                    }
                    else if (model.PreDefineValueIncreaseBy == "Respectively")
                    {
                        Entity.PreDefineValueIncreaseBy = "R";
                        Entity.MaxValue = model.PreDefineValueContent + "R0000";
                    }
                    else if (model.PreDefineValueIncreaseBy == "Four Digits")
                    {
                        Entity.PreDefineValueIncreaseBy = "FD";
                        Entity.MaxValue = model.PreDefineValueContent + "0000";
                    }
                    else
                    {
                        Entity.MaxValue = string.Empty;
                        Entity.PreDefineValueIncreaseBy = string.Empty;
                    }
                    break;
                case "CC":
                    Entity.PreDefineValueGroup = "2";
                    Entity.MaxValue = string.Empty;
                    Entity.PreDefineValueIncreaseBy = string.Empty;
                    break;
                case "Letter Signature":
                    Entity.PreDefineValueGroup = "3";
                    Entity.MaxValue = string.Empty;
                    Entity.PreDefineValueIncreaseBy = string.Empty;
                    break;
                default:
                    Entity.PreDefineValueGroup = string.Empty;
                    break;
            }

            Entity.InternalMailAddress = string.IsNullOrEmpty(model.InternalMailAddress) ? string.Empty : model.InternalMailAddress;
            Entity.InternalMailAutoSend = string.IsNullOrEmpty(model.InternalMailAutoSend) ? string.Empty : model.InternalMailAutoSend;
            Entity.ExternalMailAddress = string.IsNullOrEmpty(model.ExternalMailAddress) ? string.Empty : model.ExternalMailAddress;
            Entity.ExternalMailAutoSend = string.IsNullOrEmpty(model.ExternalMailAutoSend) ? string.Empty : model.ExternalMailAutoSend;
            Entity.Remarks = string.IsNullOrEmpty(model.Remarks) ? string.Empty : model.Remarks;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public int GetPreDefineValueForId()
        {
            return PreDefineValueForID;
        }

        public ValidationMsg Update(SysPreDefineValueFor model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        Sys_PreDefineValueFor CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.Sys_PreDefineValueFor.First(m => m.PreDefineValueForID == model.PreDefineValueForID);

                        OriginalEntity.PreDefineValueFor = CurrentEntity.PreDefineValueFor;
                        OriginalEntity.ConcernPageID = CurrentEntity.ConcernPageID;
                        OriginalEntity.Remarks = CurrentEntity.Remarks;
                        OriginalEntity.IsActive = CurrentEntity.IsActive;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #region Save PreDefineValueFor

                        if (model.PreDefineValueList != null)
                        {
                            foreach (SysPreDefineValue objSysPreDefineValue in model.PreDefineValueList)
                            {
                                if (objSysPreDefineValue.PreDefineValueID == 0)
                                {
                                    objSysPreDefineValue.PreDefineValueForID = model.PreDefineValueForID;
                                    objSysPreDefineValue.IsActive = string.IsNullOrEmpty(objSysPreDefineValue.IsActive)
                                        ? "Active"
                                        : objSysPreDefineValue.IsActive;

                                    @group = objSysPreDefineValue.PreDefineValueGroup == "Identity No" ? "1" : "0";

                                    if (group == "1")
                                    {
                                        var exitData =
                                            _context.Sys_PreDefineValue.Where(
                                                m =>
                                                    m.PreDefineValueForID == model.PreDefineValueForID &&
                                                    m.PreDefineValueGroup == "1" && m.IsActive == true).ToList();
                                        if (exitData.Count > 0)
                                        {
                                            exitidentuty = 1;
                                        }
                                        else
                                        {
                                            Sys_PreDefineValue tblPreDefineValue = SetToModelObject(objSysPreDefineValue, userid);
                                            var sameData = _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == model.PreDefineValueForID && m.PreDefineValueGroup == "1" && m.PreDefineValueContent == objSysPreDefineValue.PreDefineValueContent && m.PreDefineValueIncreaseBy == tblPreDefineValue.PreDefineValueIncreaseBy).ToList();
                                            if (sameData.Count > 0)
                                            {
                                                sameidentuty = 1;
                                            }
                                            else
                                            {
                                                _context.Sys_PreDefineValue.Add(tblPreDefineValue);
                                                _context.SaveChanges();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Sys_PreDefineValue tblPreDefineValue = SetToModelObject(objSysPreDefineValue,
                                            userid);
                                        _context.Sys_PreDefineValue.Add(tblPreDefineValue);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if ((objSysPreDefineValue.PreDefineValueGroup == "Identity No") && (objSysPreDefineValue.IsActive == "Active"))
                                    {
                                        group = "1";
                                    }
                                    else if ((objSysPreDefineValue.PreDefineValueGroup == "Identity No") && (objSysPreDefineValue.IsActive == null))
                                    {
                                        group = "1";
                                    }
                                    else
                                    {
                                        group = "0";
                                    }

                                    if (group == "1")
                                    {
                                        var exitData =
                                            _context.Sys_PreDefineValue.Where(
                                                m =>
                                                    m.PreDefineValueForID == model.PreDefineValueForID &&
                                                    m.PreDefineValueGroup == "1" && m.IsActive == true).ToList();
                                        if (exitData.Count > 0)
                                        {
                                            exitidentuty = 1;
                                        }
                                        else
                                        {
                                            objSysPreDefineValue.IsActive = string.IsNullOrEmpty(objSysPreDefineValue.IsActive) ? "Active" : objSysPreDefineValue.IsActive;
                                            Sys_PreDefineValue CurEntity = SetToModelObject(objSysPreDefineValue, userid);
                                            var OrgEntity =
                                                _context.Sys_PreDefineValue.First(
                                                    m => m.PreDefineValueID == objSysPreDefineValue.PreDefineValueID);

                                            //OrgEntity.PreDefineValueTitle = CurEntity.PreDefineValueTitle;
                                            //OrgEntity.PreDefineValueContent = CurEntity.PreDefineValueContent;
                                            //OrgEntity.PreDefineValueGroup = CurEntity.PreDefineValueGroup;
                                            //OrgEntity.PreDefineValueIncreaseBy = CurEntity.PreDefineValueIncreaseBy;
                                            //OrgEntity.MaxValue = CurEntity.MaxValue;
                                            //OrgEntity.InternalMailAddress = CurEntity.InternalMailAddress;
                                            //OrgEntity.InternalMailAutoSend = CurEntity.InternalMailAutoSend;
                                            //OrgEntity.ExternalMailAddress = CurEntity.ExternalMailAddress;
                                            //OrgEntity.ExternalMailAutoSend = CurEntity.ExternalMailAutoSend;
                                            //OrgEntity.Remarks = CurEntity.Remarks;
                                            OrgEntity.IsActive = CurEntity.IsActive;
                                            OrgEntity.ModifiedBy = userid;
                                            OrgEntity.ModifiedOn = DateTime.Now;
                                        }
                                    }
                                    else
                                    {
                                        objSysPreDefineValue.IsActive = string.IsNullOrEmpty(objSysPreDefineValue.IsActive) ? "Active" : objSysPreDefineValue.IsActive;
                                        Sys_PreDefineValue CurEntity = SetToModelObject(objSysPreDefineValue, userid);
                                        var OrgEntity =
                                            _context.Sys_PreDefineValue.First(
                                                m => m.PreDefineValueID == objSysPreDefineValue.PreDefineValueID);

                                        //OrgEntity.PreDefineValueTitle = CurEntity.PreDefineValueTitle;
                                        //OrgEntity.PreDefineValueContent = CurEntity.PreDefineValueContent;
                                        //OrgEntity.PreDefineValueGroup = CurEntity.PreDefineValueGroup;
                                        //OrgEntity.PreDefineValueIncreaseBy = CurEntity.PreDefineValueIncreaseBy;
                                        //OrgEntity.MaxValue = CurEntity.MaxValue;
                                        //OrgEntity.InternalMailAddress = CurEntity.InternalMailAddress;
                                        //OrgEntity.InternalMailAutoSend = CurEntity.InternalMailAutoSend;
                                        //OrgEntity.ExternalMailAddress = CurEntity.ExternalMailAddress;
                                        //OrgEntity.ExternalMailAutoSend = CurEntity.ExternalMailAutoSend;
                                        //OrgEntity.Remarks = CurEntity.Remarks;
                                        OrgEntity.IsActive = CurEntity.IsActive;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (exitidentuty == 0)
                        {
                            if (sameidentuty == 0)
                            {
                                _context.SaveChanges();
                                tx.Complete();
                                _vmMsg.Type = Enums.MessageType.Update;
                                _vmMsg.Msg = "Updated Successfully.";
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Same Combination Found.";
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "An Active Data Found Same Group.";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Concern Page Code Already Exit.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Failed to Update.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Update.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Delete(string PreDefineValueForID)
        {
            var buyerid = string.IsNullOrEmpty(PreDefineValueForID) ? 0 : Convert.ToInt32(PreDefineValueForID);
            var vmMsg = new ValidationMsg();
            try
            {
                var PreDefineValueList = _context.Sys_PreDefineValue.Where(s => s.PreDefineValueForID == buyerid && s.IsActive == true).ToList();

                if (PreDefineValueList.Count > 0)
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var sysPreDefineValueFor = _context.Sys_PreDefineValueFor.FirstOrDefault(s => s.PreDefineValueForID == buyerid);
                    _context.Sys_PreDefineValueFor.Remove(sysPreDefineValueFor);
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

        public ValidationMsg DeletedPreDefineValue(string PreDefineValueId)
        {
            var preDefineValueId = string.IsNullOrEmpty(PreDefineValueId) ? 0 : Convert.ToInt32(PreDefineValueId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysPreDefineValue = _context.Sys_PreDefineValue.FirstOrDefault(s => s.PreDefineValueID == preDefineValueId);
                _context.Sys_PreDefineValue.Remove(sysPreDefineValue);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }

        public IEnumerable<SysPreDefineValueFor> GetAll()
        {
            IEnumerable<SysPreDefineValueFor> iLstSysPreDefineValueFor = from slt in _context.Sys_PreDefineValueFor
                                                                         where slt.IsActive == true
                                                                         select new SysPreDefineValueFor
                                                                         {
                                                                             PreDefineValueForID = slt.PreDefineValueForID,
                                                                             PreDefineValueFor = slt.PreDefineValueFor,
                                                                             ConcernPageID = slt.ConcernPageID,
                                                                         };

            return iLstSysPreDefineValueFor;
        }

        public string GetPreDefineValueForName(int buyerID)
        {
            var query = "SELECT ConcernPageID FROM [dbo].[Sys_PreDefineValueFor]" +
                                "WHERE PreDefineValueForID =" + buyerID;

            var buyerName = _context.Database.SqlQuery<string>(query).ToString();
            return buyerName;
        }

        //public List<SysPreDefineValueFor> GetConcernPageList()
        //{
        //    //List<Screen> searchList = _context.Screens.Where(m => m.IsActive).ToList();
        //    List<MenuItem> searchList = _context.MenuItems.Where(m => m.IsActive && !m.HasChild).ToList();
        //    return searchList.Select(c => SetToBussinessObject(c)).ToList<SysPreDefineValueFor>();
        //}

        public List<SysPreDefineValueFor> GetConcernPageList()
        {
            var query = "Select s.ScreenCode ConcernPageID,S.Title ConcernPage FROM security.Screens S left join security.MenuItems M ON M.ScreenCode=S.ScreenCode " +
                        "Where M.HasChild=0 or M.HasChild  is null";
            var allData = _context.Database.SqlQuery<SysPreDefineValueFor>(query).ToList();
            return allData;
        }

        public List<SysPreDefineValueFor> GetPreDefineValueForForSearchList()
        {
            List<Sys_PreDefineValueFor> searchList = _context.Sys_PreDefineValueFor.ToList();
            return searchList.Select(c => SetToBussinessObjectPreDefineValueFor(c)).ToList<SysPreDefineValueFor>();
        }

        public List<SysPreDefineValueFor> GetPreDefineValueForList(string buyer)
        {
            List<Sys_PreDefineValueFor> searchList = _context.Sys_PreDefineValueFor.Where(m => m.ConcernPageID.StartsWith(buyer)).ToList();
            return searchList.Select(c => SetToBussinessObjectPreDefineValueFor(c)).ToList<SysPreDefineValueFor>();
        }

        public List<string> GetPreDefineValueForListForSearch()
        {
            return _context.Sys_PreDefineValueFor.Select(m => m.ConcernPageID).ToList();
        }

        public SysPreDefineValueFor SetToBussinessObjectPreDefineValueFor(Sys_PreDefineValueFor Entity)
        {
            SysPreDefineValueFor Model = new SysPreDefineValueFor();

            Model.PreDefineValueForID = Entity.PreDefineValueForID;
            Model.PreDefineValueFor = Entity.PreDefineValueFor;
            Model.ConcernPageID = Entity.ConcernPageID;
            Model.ConcernPage = _context.Screens.Where(m => m.ScreenCode == Entity.ConcernPageID).FirstOrDefault().Title;
            Model.Remarks = Entity.Remarks;

            return Model;
        }

        public SysPreDefineValueFor SetToBussinessObjectPreDefineValueForForSearch(Sys_PreDefineValueFor Entity)
        {
            SysPreDefineValueFor Model = new SysPreDefineValueFor();

            Model.PreDefineValueForID = Entity.PreDefineValueForID;
            Model.PreDefineValueFor = Entity.PreDefineValueFor;
            Model.ConcernPageID = Entity.ConcernPageID;
            //Model.PreDefineValueForCategory = Entity.PreDefineValueForCategory;
            //Model.PreDefineValueForType = Entity.PreDefineValueForType;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public SysPreDefineValue SetToBussinessObject(Sys_PreDefineValue Entity)
        {
            SysPreDefineValue Model = new SysPreDefineValue();

            Model.PreDefineValueID = Entity.PreDefineValueID;
            Model.PreDefineValueForID = Entity.PreDefineValueForID;
            Model.PreDefineValueTitle = Entity.PreDefineValueTitle;
            Model.PreDefineValueContent = Entity.PreDefineValueContent;
            if (Entity.PreDefineValueGroup == "1")
            {
                Model.PreDefineValueGroup = "Identity No";
            }
            else if (Entity.PreDefineValueGroup == "2")
            {
                Model.PreDefineValueGroup = "CC";
            }
            else if (Entity.PreDefineValueGroup == "3")
            {
                Model.PreDefineValueGroup = "Letter Signature";
            }
            else
            {
                Model.PreDefineValueGroup = "";
            }
            if (Entity.PreDefineValueIncreaseBy == "Y")
            {
                Model.PreDefineValueIncreaseBy = "Yearly";
            }
            else if (Entity.PreDefineValueIncreaseBy == "M")
            {
                Model.PreDefineValueIncreaseBy = "Monthly";
            }
            else if (Entity.PreDefineValueIncreaseBy == "D")
            {
                Model.PreDefineValueIncreaseBy = "Daily";
            }
            else if (Entity.PreDefineValueIncreaseBy == "R")
            {
                Model.PreDefineValueIncreaseBy = "Respectively";
            }
            else if (Entity.PreDefineValueIncreaseBy == "Y")
            {
                Model.PreDefineValueIncreaseBy = "Yearly";
            }
            else if (Entity.PreDefineValueIncreaseBy == "FD")
            {
                Model.PreDefineValueIncreaseBy = "Yearly";
            }
            else
            {
                Model.PreDefineValueIncreaseBy = "";
            }
            Model.MaxValue = Entity.MaxValue;
            Model.InternalMailAddress = Entity.InternalMailAddress;
            Model.InternalMailAutoSend = string.IsNullOrEmpty(Entity.InternalMailAutoSend) ? "" : Entity.InternalMailAutoSend;
            Model.ExternalMailAddress = Entity.ExternalMailAddress;
            Model.ExternalMailAutoSend = string.IsNullOrEmpty(Entity.ExternalMailAutoSend) ? "" : Entity.ExternalMailAutoSend;
            Model.Remarks = Entity.Remarks;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public SysPreDefineValue SetToBussinessObjectPreDefineValue(Sys_PreDefineValue Entity)
        {
            SysPreDefineValue Model = new SysPreDefineValue();

            Model.PreDefineValueID = Entity.PreDefineValueID;
            //Model.PreDefineValueForID = Entity.PreDefineValueForID;
            //Model.Address = Entity.Address;
            //Model.ContactPerson = Entity.ContactPerson;
            //Model.ContactNumber = Entity.ContactNumber;
            //Model.EmailAddress = Entity.EmailAddress;
            //Model.FaxNo = Entity.FaxNo;
            //Model.PhoneNo = Entity.PhoneNo;
            //Model.SkypeID = Entity.SkypeID;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public SysPreDefineValueFor SetToBussinessObject(MenuItem Entity)
        {
            SysPreDefineValueFor Model = new SysPreDefineValueFor();

            Model.ConcernPageID = Entity.ScreenCode;
            Model.ConcernPage = _context.Screens.Where(m => m.IsActive && m.ScreenCode == Entity.ScreenCode).FirstOrDefault().Title;

            return Model;
        }

        public List<SysPreDefineValue> GetPreDefineValueList(string PreDefineValueForID)
        {
            var preDefineValueForId = string.IsNullOrEmpty(PreDefineValueForID) ? 0 : Convert.ToInt32(PreDefineValueForID);
            List<Sys_PreDefineValue> searchList = _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == preDefineValueForId).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysPreDefineValue>();
        }
    }
}
