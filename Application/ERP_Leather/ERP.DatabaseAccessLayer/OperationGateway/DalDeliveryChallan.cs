using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.Entity;
using System.Configuration;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalDeliveryChallan
    {
        private readonly BLC_DEVEntities _context = new BLC_DEVEntities();
        UnitOfWork repository = new UnitOfWork();
        private ValidationMsg _vmMsg = new ValidationMsg();
        private int save;

        public DalDeliveryChallan()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();

        }
        public IEnumerable<EXPDeliveryChallan> GetCIList()
        {
            string sql = @"SELECT	    CIID,
		                                ISNULL(CINo,'')CINo,
		                                CONVERT(VARCHAR(12),CIDate, 106) CIDate,
		                                ISNULL(CIAmount,0)CIAmount 
                            FROM        EXP_CI 
                            WHERE       RecordStatus='CNF'
                            ORDER BY    CIID DESC";
            var result = _context.Database.SqlQuery<EXPDeliveryChallan>(sql);
            return result;
        }
        public IEnumerable<EXPDeliveryChallan> GetPLList()
        {
            string sql = @"SELECT			PLID,
				                            ISNULL(PLNo,'')PLNo,
				                            CONVERT(VARCHAR(12),PLDate, 106) PLDate
                            FROM			EXP_PackingList
                            WHERE			RecordStatus='CNF'
                            ORDER BY		PLID	DESC";
            var result = _context.Database.SqlQuery<EXPDeliveryChallan>(sql);
            return result;
        }

        #region Save Delivery Challan Data
        public ValidationMsg Save(EXPDeliveryChallan model, int _userid)//, string pageUrl
        {


            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        //model.DeliverChallanNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.DeliverChallanNo != null)
                        {
                            EXP_DeliveryChallan tblDeliveryChallan = SetTotblDeliveryChallanModelObject(model, _userid);//, _userid
                            _context.EXP_DeliveryChallan.Add(tblDeliveryChallan);
                            _context.SaveChanges();

                            if (model.expDeliveryChallanCIList.Count > 0)
                            {
                                foreach (var deliveryChallanCI in model.expDeliveryChallanCIList)
                                {

                                    deliveryChallanCI.DeliverChallanID = tblDeliveryChallan.DeliverChallanID;
                                    EXP_DeliveryChallanCI tblDeliveryChallanCI = SetToDeliveryChallanCIModelObject(deliveryChallanCI, _userid);
                                    _context.EXP_DeliveryChallanCI.Add(tblDeliveryChallanCI);
                                    _context.SaveChanges();
                                }
                            }
                            _context.SaveChanges();

                            tx.Complete();


                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                    }
                }
            }


            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return _vmMsg;
        }
        #endregion

        #region UPDATE Delivery Challan DATA
        public ValidationMsg Update(EXPDeliveryChallan model, int _userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Delivery Challan

                            EXP_DeliveryChallan deliveryChallan = SetTotblDeliveryChallanModelObject(model, _userid);
                            var OriginalEntity = _context.EXP_DeliveryChallan.First(m => m.DeliverChallanID == model.DeliverChallanID);

                            OriginalEntity.DeliverChallanNo = deliveryChallan.DeliverChallanNo;
                            OriginalEntity.DeliverChallanDate = deliveryChallan.DeliverChallanDate;
                            OriginalEntity.TruckNo = deliveryChallan.TruckNo;
                            OriginalEntity.DeliveryChallanNote = deliveryChallan.DeliveryChallanNote;
                            OriginalEntity.ModifiedOn = DateTime.Now;
                            OriginalEntity.ModifiedBy = _userid;
                            _context.SaveChanges();
                        #endregion


                        #region Save NEW Data & Update Existing Crust Leather Transfer Data

                        if (model.expDeliveryChallanCIList != null)
                        {
                            foreach (EXPDeliveryChallanCI deliveryChallanCIList in model.expDeliveryChallanCIList)
                            {
                                deliveryChallanCIList.DeliverChallanID = model.DeliverChallanID;


                                if (deliveryChallanCIList.CIID != model.CIID)
                                {
                                    deliveryChallanCIList.DeliverChallanID = model.DeliverChallanID;
                                    EXP_DeliveryChallanCI tblDeliveryChallanCI = SetToDeliveryChallanCIModelObject(deliveryChallanCIList, _userid);
                                    _context.EXP_DeliveryChallanCI.Add(tblDeliveryChallanCI);
                                }
                                else
                                {

                                    EXP_DeliveryChallanCI deliveryChallanCIEntity = SetToDeliveryChallanCIModelObject(deliveryChallanCIList, _userid);
                                    var OriginalIssueItemEntity = _context.EXP_DeliveryChallanCI.First(m => m.DeliverChallanID == deliveryChallanCIList.DeliverChallanID);


                                    // OriginalDeliveryChallanCI.DeliverChallanID = deliveryChallanCIEntity.DeliverChallanID;
                                    //OriginalDeliveryChallanCI.CIID = deliveryChallanCIEntity.CIID;
                                    //OriginalDeliveryChallanCI.PLID = deliveryChallanCIEntity.PLID;
                                    OriginalIssueItemEntity.SetOn = deliveryChallanCIEntity.SetOn;
                                    OriginalIssueItemEntity.SetBy = deliveryChallanCIEntity.SetBy;


                                    OriginalIssueItemEntity.ModifiedBy = _userid;
                                    OriginalIssueItemEntity.ModifiedOn = DateTime.Now;
                                    OriginalIssueItemEntity.IPAddress = GetIPAddress.LocalIPAddress();
                                }


                            }
                        }
                        #endregion

                        _context.SaveChanges();
                        tx.Complete();
                       
                        //CLTransferNo = model.CLTransferNo;
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }
        #endregion

        #region Delete Delivery Challan Data
        //public ValidationMsg Delete(long deliverChallanID)
        //{
        //    try
        //    {
        //        using (var tx = new TransactionScope())
        //        {
        //            var delete = _context.EXP_DeliveryChallan.FirstOrDefault(ob => ob.DeliverChallanID == deliverChallanID);
        //            if (delete != null)
        //            {
        //                var deleteDeliverChallanCI = (from temp in _context.EXP_DeliveryChallan
        //                                              join temp2 in _context.EXP_DeliveryChallanCI on temp.DeliverChallanID equals temp2.DeliverChallanID
        //                                              where temp.DeliverChallanID == deliverChallanID
        //                                              select temp2).ToList(); //repository.EXPDeliveryChallanCIRepository.Get().Where(ob => ob.DeliverChallanID == delete.DeliverChallanID && ob.CIID == deleteCI.CIID).ToList();
        //                foreach (var item in deleteDeliverChallanCI)
        //                {
        //                    _context.EXP_DeliveryChallanCI.Remove(item);
        //                }
        //                _context.EXP_DeliveryChallan.Remove(delete);

        //                save = _context.SaveChanges();
        //                if (save == 1)
        //                {
        //                    _vmMsg.Type = Enums.MessageType.Delete;
        //                    _vmMsg.Msg = "Deleted Successfully";
        //                }
        //                else
        //                {
        //                    _vmMsg.Type = Enums.MessageType.Error;
        //                    _vmMsg.Msg = "Failed to Delete the Record.";
        //                }
        //            }
        //            else
        //            {
        //                _vmMsg.Type = Enums.MessageType.Error;
        //                _vmMsg.Msg = "There has no data to Delete.";
        //            }
        //            tx.Complete();
        //        }      
        //    }
        //    catch
        //    {
        //        _vmMsg.Type = Enums.MessageType.Error;
        //        _vmMsg.Msg = "Error occured when Delete Data";
        //    }
        //    return _vmMsg;
        //}


        //public ValidationMsg Delete(long deliverChallanID)
        //{
        //    try
        //    {
        //        using (var tx = new TransactionScope())
        //        {
        //            EXP_DeliveryChallan  delete = _context.EXP_DeliveryChallan.FirstOrDefault(ob => ob.DeliverChallanID == deliverChallanID);
        //            if (delete != null)
        //            {
        //                var deleteDeliverChallanCI = (from temp in _context.EXP_DeliveryChallan
        //                                              join temp2 in _context.EXP_DeliveryChallanCI on temp.DeliverChallanID equals temp2.DeliverChallanID
        //                                              where temp.DeliverChallanID == deliverChallanID
        //                                              select temp2).ToList(); 
        //                foreach (var item in deleteDeliverChallanCI)
        //                {
        //                    _context.EXP_DeliveryChallanCI.Remove(item);
        //                    save = _context.SaveChanges();
        //                }
        //                _context.EXP_DeliveryChallan.Remove(delete);
        //                save = _context.SaveChanges();
        //            }
        //            tx.Complete();
        //            _vmMsg.Type = Enums.MessageType.Delete;
        //            _vmMsg.Msg = "Deleted Sucessfully";
        //        }
        //    }
        //    catch
        //    {
        //        _vmMsg.Type = Enums.MessageType.Error;
        //        _vmMsg.Msg = "Failed to Delete.";
        //    }
        //    return _vmMsg;
        //}

        public ValidationMsg Delete(long deliverChallanID)
        {
            try
            {
                EXP_DeliveryChallan delete = _context.EXP_DeliveryChallan.FirstOrDefault(ob => ob.DeliverChallanID == deliverChallanID);
                if (delete != null)
                {
                    var deleteDeliverChallanCI = (from temp in _context.EXP_DeliveryChallan
                                                  join temp2 in _context.EXP_DeliveryChallanCI on temp.DeliverChallanID equals temp2.DeliverChallanID
                                                  where temp.DeliverChallanID == deliverChallanID
                                                  select temp2).ToList();
                    foreach (var item in deleteDeliverChallanCI)
                    {
                        _context.EXP_DeliveryChallanCI.Remove(item);
                        save = _context.SaveChanges();
                    }
                    _context.EXP_DeliveryChallan.Remove(delete);
                    save = _context.SaveChanges();
                }

                save = repository.Save();
                if (save == 0)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Deleted Successfully";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete the Record.";
            }
            return _vmMsg;
        }

        #endregion

        //        public List<EXPDeliveryChallanCI> SearchIssueInfo(string DeliverChallanID)
//        {
//            string sql = @"  SELECT		
//				                        ISNULL(dci.CIID,0)CIID,
//				                        ISNULL(dci.PLID,0)PLID,
//				                        ISNULL(c.CINo,'')CINo,
//				                        CONVERT(VARCHAR(12),c.CIDate, 106) CIDate,
//				                        ISNULL(p.PLNo,'')PLNo,
//				                        CONVERT(VARCHAR(12),p.PLDate, 106) PLDate
//				
//                                  FROM			EXP_DeliveryChallan dc
//                                  INNER JOIN    EXP_DeliveryChallanCI dci ON dc.DeliverChallanID='" + DeliverChallanID + "' LEFT JOIN	EXP_CI c ON dci.CIID=c.CIID  LEFT JOIN		EXP_PackingList p ON dci.PLID=p.PLID ORDER BY		dc.DeliverChallanNo ";
//            var result = _context.Database.SqlQuery<EXPDeliveryChallanCI>(sql);
//            return result;

//        }
        public IEnumerable<EXPDeliveryChallanCI> SearchDeliveryChallanCI(string DeliverChallanID)
        {

            string sql = @"  SELECT		
            				                        ISNULL(dci.CIID,0)CIID,
            				                        ISNULL(dci.PLID,0)PLID,
            				                        ISNULL(c.CINo,'')CINo,
            				                        CONVERT(VARCHAR(12),c.CIDate, 106) CIDate,
            				                        ISNULL(p.PLNo,'')PLNo,
            				                        CONVERT(VARCHAR(12),p.PLDate, 106) PLDate
            				
                                              FROM			EXP_DeliveryChallanCI dci 
											  LEFT JOIN		EXP_CI c ON dci.CIID=c.CIID  
											  LEFT JOIN		EXP_PackingList p ON dci.PLID=p.PLID 
											  WHERE			dci.DeliverChallanID='" + DeliverChallanID + "'  ORDER BY		dci.DeliverChallanID ";
            var result = _context.Database.SqlQuery<EXPDeliveryChallanCI>(sql);
            return result;
        }
        public IEnumerable<EXPDeliveryChallan> GetCIList(string DeliverChallanID)
        {
            string sql = @"SELECT		
//				                        ISNULL(dci.CIID,0)CIID,
//				                        ISNULL(dci.PLID,0)PLID,
//				                        ISNULL(c.CINo,'')CINo,
//				                        CONVERT(VARCHAR(12),c.CIDate, 106) CIDate,
//				                        ISNULL(p.PLNo,'')PLNo,
//				                        CONVERT(VARCHAR(12),p.PLDate, 106) PLDate
//				
//                                  FROM			EXP_DeliveryChallan dc
//                                  INNER JOIN    EXP_DeliveryChallanCI dci ON dc.DeliverChallanID='" + DeliverChallanID + "' LEFT JOIN	EXP_CI c ON dci.CIID=c.CIID  LEFT JOIN		EXP_PackingList p ON dci.PLID=p.PLID ORDER BY		dc.DeliverChallanNo ";
            var result = _context.Database.SqlQuery<EXPDeliveryChallan>(sql);
            return result;
        }
        public EXP_DeliveryChallan SetTotblDeliveryChallanModelObject(EXPDeliveryChallan model, int _userid)//int _userid
        {
            EXP_DeliveryChallan entity = new EXP_DeliveryChallan();
            entity.DeliverChallanID = model.DeliverChallanID;
            entity.DeliverChallanNo = model.DeliverChallanNo == null ? "" : model.DeliverChallanNo;
            entity.DeliverChallanRef = model.DeliverChallanRef == null ? "" : model.DeliverChallanRef;
            entity.DeliveryChallanNote = model.DeliveryChallanNote == null ? "" : model.DeliveryChallanNote;
            entity.DeliverChallanDate = model.DeliverChallanDate;
            entity.TruckNo = model.TruckNo == null ? "" : model.TruckNo;
            entity.RecordStatus = "NCF";
            entity.SetOn = DateTime.Now;
            entity.SetBy = _userid;
            entity.IPAddress = GetIPAddress.LocalIPAddress();

            return entity;
        }
        public  EXP_DeliveryChallanCI SetToDeliveryChallanCIModelObject(EXPDeliveryChallanCI model, int _userid)
        {
            EXP_DeliveryChallanCI entity = new EXP_DeliveryChallanCI();

            entity.DeliverChallanID = model.DeliverChallanID;
            entity.CIID = model.CIID;
            entity.PLID = model.PLID;
            entity.RecordStatus = DalCommon.ReturnRecordStatus(model.RecordStatus);
            entity.SetOn = DateTime.Now;
            entity.SetBy = _userid;
            entity.RecordStatus = "NCF";
            entity.IPAddress = GetIPAddress.LocalIPAddress();
            return entity;
        }
        public ValidationMsg ConfirmDeliveryChallan(int userid, string deliverChallanID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    if (deliverChallanID != "")
                    {
                        EXP_DeliveryChallan obDeliveryChallan = repository.EXPDeliveryChallanRepository.GetByID(Convert.ToInt32(deliverChallanID));
                        if (obDeliveryChallan.RecordStatus == "NCF")
                        {


                            obDeliveryChallan.RecordStatus = "CNF";
                            obDeliveryChallan.SetBy = userid;
                            obDeliveryChallan.SetOn = DateTime.Now;
                            repository.EXPDeliveryChallanRepository.Update(obDeliveryChallan);

                            //EXP_DeliveryChallanCI obDeliveryChallanCI = repository.EXPDeliveryChallanCIRepository.GetByID(Convert.ToInt32(deliverChallanID));
                            //obDeliveryChallanCI.RecordStatus = "CNF";
                            //repository.EXPDeliveryChallanCIRepository.Update(obDeliveryChallanCI);

                            int flag = repository.Save();
                            if (flag == 1)
                            {
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Confirmed Successfully.";
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "  Confirmed Faild.";
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Data has Already Confirmed.";
                        }
                    }

                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Save First.";
                    }

                    tx.Complete();
                }

            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return _vmMsg;
        }
    }
}
