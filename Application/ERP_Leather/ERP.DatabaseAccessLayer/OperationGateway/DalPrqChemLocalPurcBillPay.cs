using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqChemLocalPurcBillPay
    {
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private int _mode;
        private bool _save;
        private readonly BLC_DEVEntities _context;
        public DalPrqChemLocalPurcBillPay()
        {
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg DeleteRef(long paymentId)
        {
            try
            {
                var delete = _unit.BillPaymentReferenceRepository.GetByID(paymentId);
                if (delete != null)
                {
                    _unit.BillPaymentReferenceRepository.Delete(delete);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Delete;
                    _validationMsg.Msg = "Successfully deleted.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete.";
            }
            return _validationMsg;
        }

        public ValidationMsg Delete(long paymentId)
        {
            try
            {
                var delete = _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(paymentId);
                if (delete != null)
                {
                    var deleteRef =
                        _unit.ChemicalBillPaymentReference.Get().Where(ob => ob.PaymentID == delete.PaymentID).ToList();
                    if (deleteRef != null)
                    {
                        foreach (var reff in deleteRef)
                        {
                            _unit.ChemicalBillPaymentReference.Delete(reff);
                        }
                    }
                    _unit.ChemicalLocalPurchaseBillPaymentRepository.Delete(delete);
                    _save = _unit.IsSaved();
                    if (_save)
                    {
                        _validationMsg.Type = Enums.MessageType.Success;
                        _validationMsg.Msg = "Bill deleted successfully.";
                    }
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete.";
            }
            return _validationMsg;
        }


        public ValidationMsg Save(PrqChemLocalPurcBillPay model, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var bill = ConvertBillPayment(model, userId);
                        if (bill.PaymentID == 0)
                        {
                            _context.PRQ_ChemLocalPurcBillPayment.Add(bill);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.References != null)
                        {
                            foreach (var reference in model.References)
                            {
                                var entityRef = ConvertBillPaymentReference(reference, bill.PaymentID, userId);
                                if (entityRef.BillPaymtRefID == 0)
                                {
                                    _context.PRQ_ChemBillPymtReference.Add(entityRef);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                            }
                        }

                        tx.Complete();
                        if (_mode == 1)
                        {
                            _validationMsg.ReturnId = bill.PaymentID;
                            _validationMsg.ReturnCode = bill.PaymentNo;
                            _validationMsg.Type = Enums.MessageType.Success;
                            _validationMsg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _validationMsg.Type = Enums.MessageType.Update;
                            _validationMsg.Msg = "Updated successfully.";
                        }
                    }
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to save.";
            }
            return _validationMsg;
        }

        public ValidationMsg Check(long paymentId, int userId, string comment)
        {
            try
            {
                var bill = _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(paymentId);
                if (bill != null)
                {
                    bill.CheckedBy = userId;
                    bill.CheckDate = DateTime.Now;
                    bill.RecordStatus = "CHK";
                    bill.CheckComments = comment;
                }
                var billRefs = _unit.ChemicalBillPaymentReference.Get().Where(ob => ob.PaymentID == paymentId).ToList();
                foreach (var billRef in billRefs)
                {
                    billRef.RecordStatus = "CHK";
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Checked successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to check.";
            }
            return _validationMsg;
        }

        public ValidationMsg BillConfirm(long paymentId)
        {
            try
            {
                var billPay = _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(paymentId);
                if (billPay != null)
                {

                    billPay.PaymentStatus = true;
                    billPay.RecordStatus = "CNF";
                }
                var billRefs = _unit.ChemicalBillPaymentReference.Get().Where(ob => ob.PaymentID == paymentId).ToList();
                foreach (var billRef in billRefs)
                {
                    billRef.RecordStatus = "CNF";
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Confirmed successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm.";
            }

            return _validationMsg;
        }

        public ValidationMsg Approve(long paymentId, int userId, string comment)
        {
            try
            {
                var bill = _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(paymentId);
                if (bill != null)
                {
                    if (bill.RecordStatus == "CNF")
                    {
                        bill.ApprovedBy = userId;
                        bill.ApproveDate = DateTime.Now;
                        bill.RecordStatus = "APV";
                        bill.ApprovalAdvice = comment;
                        var billRefs = _unit.ChemicalBillPaymentReference.Get().Where(ob => ob.PaymentID == paymentId).ToList();
                        foreach (var billRef in billRefs)
                        {
                            billRef.RecordStatus = "APV";
                        }
                    }
                    else
                    {
                        _validationMsg.Type = Enums.MessageType.Error;
                        _validationMsg.Msg = "Please confirm the bill before approving.";
                    }
                }

                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Approved successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to approve.";
            }
            return _validationMsg;
        }

        private PRQ_ChemLocalPurcBillPayment ConvertBillPayment(PrqChemLocalPurcBillPay model, int userId)
        {
            var entity = model.PaymentId == 0 ? new PRQ_ChemLocalPurcBillPayment() : (from b in _context.PRQ_ChemLocalPurcBillPayment.AsEnumerable()
                                                                                      where b.PaymentID == model.PaymentId
                                                                                      select b).FirstOrDefault();
            entity.PaymentID = model.PaymentId;
            entity.PaymentNo = model.PaymentNo;
            entity.PaymentDate = DalCommon.SetDate(model.PaymentDate);
            entity.SupplierID = model.SupplierId;
            entity.SupplierAddressID = model.SupplierAddressId;
            entity.PurchaseYear = model.PurchaseYear;
            entity.PaymentType = model.PaymentType;
            entity.PaymentMethod = model.PaymentMethod;
            entity.Currency = model.Currency;
            entity.PaymentDoc = model.PaymentDoc;
            entity.BillAmount = model.BillAmount;
            entity.VatAmount = model.VatAmount;
            entity.DeductAmount = model.DeductAmount;
            entity.PaymentAmount = model.PaymentAmount;
            entity.PaymentStatus = model.PaymentStatus;
            entity.Remarks = model.Remarks;
            entity.RecordStatus = model.PaymentId == 0
                ? "NCF"
                : _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(model.PaymentId).RecordStatus;
            entity.SetOn = model.PaymentId == 0
                ? DateTime.Now
                : _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(model.PaymentId).SetOn;
            entity.SetBy = model.PaymentId == 0
                ? userId
                : _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(model.PaymentId).SetBy;
            entity.ModifiedBy = model.PaymentId == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.PaymentId == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        private PRQ_ChemBillPymtReference ConvertBillPaymentReference(PrqChemBillPymtRef model, long paymentId, int userId)
        {
            var entity = model.BillPaymtRefID == 0 ? new PRQ_ChemBillPymtReference() : (from b in _context.PRQ_ChemBillPymtReference.AsEnumerable()
                                                                                        where b.BillPaymtRefID == model.BillPaymtRefID
                                                                                        select b).FirstOrDefault();
            entity.BillPaymtRefID = model.BillPaymtRefID;
            entity.PaymentID = model.PaymentID ?? paymentId;
            entity.BillID = model.BillID;
            entity.SupplierBillNo = model.SupplierBillNo;
            entity.RecordStatus = model.BillPaymtRefID == 0
                ? "NCF"
                : _unit.ChemicalBillPaymentReference.GetByID(model.BillPaymtRefID).RecordStatus;
            entity.SetOn = model.BillPaymtRefID == 0
                ? DateTime.Now
                : _unit.ChemicalBillPaymentReference.GetByID(model.BillPaymtRefID).SetOn;
            entity.SetBy = model.BillPaymtRefID == 0
                ? userId
                : _unit.ChemicalBillPaymentReference.GetByID(model.BillPaymtRefID).SetBy;
            entity.ModifiedOn = model.BillPaymtRefID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.BillPaymtRefID == 0 ? (int?)null : userId;
            return entity;
        }


    }
}
