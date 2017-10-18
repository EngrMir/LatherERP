using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Transactions;
using System.Collections.Generic;
using ERP.EntitiesModel.AppSetupModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqBillPayment
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long PaymentID = 0;
        public DalPrqBillPayment()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PrqBillPayment model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.SetBy = userid;
                        Prq_BillPayment tblBillPayment = SetToModelObject(model);
                        _context.Prq_BillPayment.Add(tblBillPayment);
                        _context.SaveChanges();

                        #region Save Detail Records

                        if (model.BillPaymentReferenceList != null)
                        {
                            foreach (
                                PrqBillPaymentReference objPrqBillPaymentReference in
                                    model.BillPaymentReferenceList)
                            {
                                objPrqBillPaymentReference.PaymentID = tblBillPayment.PaymentID;
                                objPrqBillPaymentReference.RecordStatus = "NCF";
                                Prq_BillPaymentReference tblPurchaseYearPeriod =
                                    SetToModelObject(objPrqBillPaymentReference);
                                _context.Prq_BillPaymentReference.Add(tblPurchaseYearPeriod);
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        PaymentID = tblBillPayment.PaymentID;
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Saved Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public long GetPaymentID()
        {
            return PaymentID;
        }

        public ValidationMsg Update(PrqBillPayment model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.ModifiedBy = userid;
                        Prq_BillPayment CurrentEntity = SetToModelObject(model);
                        var OriginalEntity = _context.Prq_BillPayment.First(m => m.PaymentID == model.PaymentID);

                        OriginalEntity.PaymentDate = CurrentEntity.PaymentDate;// Convert.ToDateTime(Convert.ToDateTime(CurrentEntity.PaymentDate).ToString("dd/MM/yyyy"));
                        OriginalEntity.SupplierID = CurrentEntity.SupplierID;
                        OriginalEntity.SupplierAddressID = CurrentEntity.SupplierAddressID;
                        OriginalEntity.PaymentType = CurrentEntity.PaymentType;
                        OriginalEntity.PaymentMethod = CurrentEntity.PaymentMethod;
                        OriginalEntity.Currency = CurrentEntity.Currency;
                        OriginalEntity.PaymentDoc = CurrentEntity.PaymentDoc;
                        OriginalEntity.BillAmount = CurrentEntity.BillAmount;
                        OriginalEntity.VatPercentage = CurrentEntity.VatPercentage;
                        OriginalEntity.VatAmount = CurrentEntity.VatAmount;
                        OriginalEntity.DeductAmount = CurrentEntity.DeductAmount;
                        OriginalEntity.PaymentAmount = CurrentEntity.PaymentAmount;
                        OriginalEntity.Remarks = CurrentEntity.Remarks;

                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #region Save Detail Records

                        if (model.BillPaymentReferenceList != null)
                        {
                            foreach (PrqBillPaymentReference objPrqBillPaymentReference in model.BillPaymentReferenceList)
                            {
                                if (objPrqBillPaymentReference.PaymentID == 0)
                                {
                                    objPrqBillPaymentReference.PaymentID = model.PaymentID;
                                    objPrqBillPaymentReference.RecordStatus = "NCF";
                                    Prq_BillPaymentReference tblPurchaseYearPeriod = SetToModelObject(objPrqBillPaymentReference);
                                    _context.Prq_BillPaymentReference.Add(tblPurchaseYearPeriod);
                                }
                                else
                                {
                                    Prq_BillPaymentReference CurEntity = SetToModelObject(objPrqBillPaymentReference);
                                    var OrgEntity = _context.Prq_BillPaymentReference.First(m => m.PaymentID == objPrqBillPaymentReference.PaymentID && m.SupplierBillID == objPrqBillPaymentReference.SupplierBillID);

                                    OrgEntity.SupplierBillRef = CurEntity.SupplierBillRef;
                                }
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public ValidationMsg BillPaymentChecked(string paymentId, string checkComment, string recordStatus, int checkedby)
        {
            long PaymentID = string.IsNullOrEmpty(paymentId) ? 0 : Convert.ToInt64(paymentId);
            _vmMsg = new ValidationMsg();
            try
            {
                var OriginalEntity = _context.Prq_BillPayment.First(m => m.PaymentID == PaymentID);

                OriginalEntity.CheckedBy = checkedby;
                OriginalEntity.CheckComment = checkComment;
                OriginalEntity.RecordStatus = recordStatus;

                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Checked.";
            }
            return _vmMsg;
        }

        public ValidationMsg BillPaymentRecommended(string paymentId, string recommendedComment, string recordStatus, int recommendedBy)
        {
            long PaymentID = string.IsNullOrEmpty(paymentId) ? 0 : Convert.ToInt64(paymentId);
            _vmMsg = new ValidationMsg();
            try
            {
                var OriginalEntity = _context.Prq_BillPayment.First(m => m.PaymentID == PaymentID);

                OriginalEntity.RecommendedBy = recommendedBy;
                OriginalEntity.RecommendedComment = recommendedComment;
                OriginalEntity.RecordStatus = recordStatus;

                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Recommended Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Recommended.";
            }
            return _vmMsg;
        }

        public ValidationMsg BillPaymentApproved(string paymentId, string approveComment, string recordStatus, int approvedBy)
        {
            long PaymentID = string.IsNullOrEmpty(paymentId) ? 0 : Convert.ToInt64(paymentId);
            _vmMsg = new ValidationMsg();
            try
            {
                var OriginalEntity = _context.Prq_BillPayment.First(m => m.PaymentID == PaymentID);

                OriginalEntity.ApprovedBy = approvedBy;
                OriginalEntity.ApproveComment = approveComment;
                OriginalEntity.RecordStatus = recordStatus;

                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Approved Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Approved.";
            }
            return _vmMsg;
        }

        public Prq_BillPayment SetToModelObject(PrqBillPayment model)
        {
            Prq_BillPayment Entity = new Prq_BillPayment();

            Entity.PaymentDate = DalCommon.SetDate(model.PaymentDate);// Convert.ToDateTime(Convert.ToDateTime(model.PaymentDate).ToString("dd/MM/yyyy"));
            Entity.SupplierID = model.SupplierID;
            Entity.SupplierAddressID = model.SupplierAddressID;
            Entity.PaymentType = model.PaymentType;
            Entity.PaymentMethod = model.PaymentMethod;
            Entity.Currency = model.Currency;
            Entity.PaymentDoc = model.PaymentDoc;
            Entity.BillAmount = model.BillAmount;
            Entity.VatPercentage = model.VatPercentage;
            Entity.VatAmount = model.VatAmount;
            Entity.DeductAmount = model.DeductAmount;
            Entity.PaymentAmount = model.PaymentAmount;
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";
            Entity.PurchaseYear = model.PurchaseYear;
            Entity.PaymentStatus = true;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public Prq_BillPaymentReference SetToModelObject(PrqBillPaymentReference model)
        {
            Prq_BillPaymentReference Entity = new Prq_BillPaymentReference();

            Entity.PaymentID = model.PaymentID;// Convert.ToInt16(_context.Prq_BillPayment.DefaultIfEmpty().Max(m => m.PaymentID == null ? 0 : m.PaymentID));
            Entity.SupplierBillID = model.SupplierBillID;
            Entity.SupplierBillRef = model.SupplierBillNo;
            Entity.RecordStatus = model.RecordStatus;

            return Entity;
        }

        public List<PrqBillPayment> GetBillPaymentList(int supplierId)
        {
            List<Prq_BillPayment> searchList = _context.Prq_BillPayment.Where(m => m.SupplierID == supplierId && m.RecordStatus == "NCF").ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqBillPayment>();
        }

        public List<PrqBillPayment> GetBillPaymentList()
        {
            List<Prq_BillPayment> searchList = _context.Prq_BillPayment.OrderByDescending(m => m.PaymentID).ToList(); //using table
            //List<Prq_BillPayment> searchList = _context.Prq_BillPayment.Where(m => m.RecordStatus == "NCF").OrderByDescending(m => m.PaymentID).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqBillPayment>();
        }

        //public List<PrqBillPayment> GetBillPaymentListForSearch(string supplier)
        //{
        //    List<Prq_BillPayment> searchList = _context.Prq_BillPayment.Where(m => m.RecordStatus == "NCF").ToList(); //using table
        //    return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqBillPayment>();
        //}

        public PrqBillPayment SetToBussinessObject(Prq_BillPayment Entity)
        {
            PrqBillPayment model = new PrqBillPayment();

            model.PaymentID = Entity.PaymentID;
            model.PaymentDate = Entity.PaymentDate.ToString("dd/MM/yyyy");
            model.SupplierID = Entity.SupplierID;
            model.SupplierName = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            model.SupplierCode = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierCode;
            model.SupplierAddressID = Entity.SupplierAddressID;
            var abc =
                _context.Sys_SupplierAddress.Where(
                    q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                    .FirstOrDefault();
            if (abc != null)
            {
                model.Address = _context.Sys_SupplierAddress.Where(q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false)).FirstOrDefault().Address;
            }
            model.PaymentType = Entity.PaymentType;
            model.PaymentTypeName = _context.Sys_PaymentType.Where(m => m.ID == Entity.PaymentType).FirstOrDefault().Name;
            model.PaymentMethod = Entity.PaymentMethod;
            model.Currency = Entity.Currency;
            model.PaymentDoc = Entity.PaymentDoc;
            model.BillAmount = Entity.BillAmount;
            model.VatPercentage = Entity.VatPercentage;
            model.VatAmount = Entity.VatAmount;
            model.DeductAmount = Entity.DeductAmount;
            model.PaymentAmount = Entity.PaymentAmount;
            model.Remarks = Entity.Remarks;
            model.PurchaseYear = Entity.PurchaseYear;
            model.PaymentStatus = Entity.PaymentStatus;
            model.Remarks = Entity.Remarks;
            model.RecordStatus = Entity.RecordStatus;
            switch (Entity.RecordStatus)
            {
                case "NCF":
                    model.RecordStatusName = "Not Confirmed";
                    break;
                case "CNF":
                    model.RecordStatusName = "Confirmed";
                    break;
                case "APV":
                    model.RecordStatusName = "Approved";
                    break;
                //case "CMP":
                //    model.RecordStatusName = "Completed";
                //    break;
                default:
                    model.RecordStatusName = "";
                    break;
            }

            return model;
        }

        public List<PrqBillPaymentReference> GetPayBillRefInfo(int paymentID)
        {
            List<Prq_BillPaymentReference> searchList = _context.Prq_BillPaymentReference.Where(m => m.PaymentID == paymentID && m.RecordStatus == "NCF").ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqBillPaymentReference>();
        }

        public PrqBillPaymentReference SetToBussinessObject(Prq_BillPaymentReference Entity)
        {
            PrqBillPaymentReference model = new PrqBillPaymentReference();

            model.PaymentID = Entity.PaymentID;
            model.SupplierBillID = Entity.SupplierBillID;
            model.SupplierBillNo = Entity.SupplierBillRef;

            return model;
        }

        public ValidationMsg DeletedBillRefGridData(int supplierBillId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.Prq_BillPaymentReference.First(m => m.SupplierBillID == supplierBillId);
                _context.Prq_BillPaymentReference.Remove(deleteElement);

                //var OrgEntity = _context.Prq_SupplierBill.First(m => m.SupplierBillID == supplierBillId);
                //OrgEntity.RecordStatus = "NCF";

                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public List<PrqBillPaymentReference> GetBillRefList(int supplierId)
        {
            //var AllData = (from s in _context.Prq_SupplierBill.AsEnumerable()
            //               where s.SupplierID == supplierId && (s.RecordStatus == "APV" || s.RecordStatus == "CNF")
            //               select new PrqBillPaymentReference
            //               {
            //                   SupplierBillID = s.SupplierBillID,
            //                   SupplierBillNo = s.SupplierBillNo
            //               }).OrderByDescending(m => m.SupplierBillID).ToList();
            //return AllData;
            var query = @"SELECT SupplierBillID,SupplierBillNo
                        FROM dbo.Prq_SupplierBill
                        WHERE (RecordStatus ='CNF' OR RecordStatus ='APV')
                        AND BillCategory='Real' AND SupplierID=" + supplierId + " AND SupplierBillNo NOT IN (select SupplierBillRef from dbo.Prq_BillPaymentReference)";
            var allData = _context.Database.SqlQuery<PrqBillPaymentReference>(query).ToList();
            return allData;
        }

        public PrqBillPayment GetSupplierBillPaymentAmount(PrqBillPayment model)
        {
            PrqBillPayment modeldata = new PrqBillPayment();
            if (model.BillPaymentReferenceList != null)
            {
                foreach (var BillPaymentReference in model.BillPaymentReferenceList)
                {
                    //modeldata.BillAmount += Convert.ToDecimal(_context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID)
                    //    .FirstOrDefault().TotalAmt == null ? 0 : _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID)
                    //    .FirstOrDefault().TotalAmt);
                    //modeldata.DeductAmount = _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID).FirstOrDefault().DiscountAmt != null ? _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID).FirstOrDefault().DiscountAmt : 0;

                    //modeldata.DeductAmount += modeldata.DeductAmount;
                    //modeldata.PaymentAmount = modeldata.BillAmount + modeldata.DeductAmount;

                    //var approvedAmt =
                    //    _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID)
                    //        .FirstOrDefault().ApprovedAmt == null ? 0 : _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID)
                    //    .FirstOrDefault().ApprovedAmt;
                    var approvedAmt =
                        _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID)
                            .FirstOrDefault().ApprovedAmt == null ? 0 : _context.Prq_SupplierBill.Where(m => m.SupplierBillID == BillPaymentReference.SupplierBillID)
                        .FirstOrDefault().PayableAmt;
                    modeldata.BillAmount += Convert.ToDecimal(approvedAmt);
                    modeldata.PaymentAmount = modeldata.BillAmount;
                    //modeldata.PaymentAmount = modeldata.BillAmount + modeldata.DeductAmount;
                }
            }
            return modeldata;
        }

        //public List<SupplierDetails> SupplierList = new List<SupplierDetails>();
        //public List<SupplierDetails> GetSupplierList()
        //{
        //    var SupplierIDList = _context.Prq_SupplierBill.Where(m => m.RecordStatus == "APV").Select(m => m.SupplierID.ToString()).Distinct().ToList();

        //    foreach (var supplierID in SupplierIDList)
        //    {
        //        int supid = string.IsNullOrEmpty(supplierID) ? 0 : Convert.ToInt16(supplierID);
        //        var supplier = SetToBussinessObject(_context.Sys_Supplier.Where(m => m.SupplierID == supid).SingleOrDefault());
        //        SupplierList.Add(supplier);
        //    }
        //    return SupplierList;
        //}

        public List<SupplierDetails> GetSupplierList()
        {
            List<Sys_Supplier> searchList = _context.Sys_Supplier.Where(m => m.SupplierCategory == "Leather" && (m.SupplierType == "Local" || m.SupplierType == "Local Agent")).OrderBy(m => m.SupplierName).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SupplierDetails>();
        }

        public SupplierDetails SetToBussinessObject(Sys_Supplier Entity)
        {
            SupplierDetails Model = new SupplierDetails();

            Model.SupplierID = Entity.SupplierID;
            Model.SupplierCode = Entity.SupplierCode;
            if (_context.Sys_SupplierAddress.Where(q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false)).FirstOrDefault() != null)
            {
                Model.SupplierAddressID = _context.Sys_SupplierAddress.Where(q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false)).FirstOrDefault().SupplierAddressID.ToString();
                Model.Address = _context.Sys_SupplierAddress.Where(q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false)).FirstOrDefault().Address;
            }
            Model.SupplierName = Entity.SupplierName;

            return Model;
        }

        public ValidationMsg DeletedBillPayment(int paymentId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var issueItemList = _context.Prq_BillPaymentReference.Where(m => m.PaymentID == paymentId).ToList();

                if (issueItemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.Prq_BillPayment.First(m => m.PaymentID == paymentId);
                    _context.Prq_BillPayment.Remove(deleteElement);

                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public List<string> GetSupplierListForSearch()
        {
            var supplierList = new List<string>();
            foreach (var billPaymentSupplier in _context.Prq_BillPayment.Select(m => m.SupplierID).ToList())
            {
                supplierList.Add(_context.Sys_Supplier.Where(m => m.SupplierID == billPaymentSupplier).FirstOrDefault().SupplierName);
            }
            return supplierList;
        }
    }
}
