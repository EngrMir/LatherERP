using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqChemLocalPurcBillEnt
    {
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private int _mode;
        private bool _save;
        private readonly BLC_DEVEntities _context;
        public DalPrqChemLocalPurcBillEnt()
        {
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _mode = 0;
            _context = new BLC_DEVEntities();
        }
        public ValidationMsg Save(PrqChemLocalPurcBill model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var bill = BillConversion(model, userId, url);
                        if (model.BillId == 0)
                        {
                            _context.PRQ_ChemLocalPurcBill.Add(bill);
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
                                var entityRef = BillReferenceConversion(reference, userId, bill.BillID);
                                if (reference.BillRefId == 0)
                                {
                                    _context.PRQ_ChemLocalPurcBillRef.Add(entityRef);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                                if (reference.Items != null)
                                {
                                    foreach (var item in reference.Items)
                                    {
                                        var entityItem = BillItemConversion(item, userId, bill.BillID, entityRef.BillRefID);
                                        if (item.BillItemId == 0)
                                        {
                                            _context.PRQ_ChemLocalPurcBillItem.Add(entityItem);
                                            _context.SaveChanges();
                                        }
                                        else
                                        {
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _validationMsg.ReturnId = bill.BillID;
                            _validationMsg.ReturnCode = bill.BillNo;
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

        public ValidationMsg Check(int billId, int userId, string comment)
        {
            try
            {
                var bill = _unit.ChemicalLocalPurchaseBillRepository.GetByID(billId);
                if (bill != null)
                {
                    bill.CheckedBy = userId;
                    bill.CheckDate = DateTime.Now;
                    bill.RecordStatus = "CHK";
                    bill.CheckComments = comment;
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

        public ValidationMsg Confirm(int billId, string comment)
        {
            try
            {
                var bill = _unit.ChemicalLocalPurchaseBillRepository.GetByID(billId);
                if (bill != null)
                {
                    bill.RecordStatus = "CNF";
                    bill.CheckComments = comment;
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.ReturnId = bill.BillID;
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
        public ValidationMsg DeleteReference(int billRefId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_unit)
                    {
                        var delete = _unit.ChemicalLocalPurchaseBillReferenceRepository.GetByID(billRefId);
                        if (delete != null)
                        {
                            var items =
                                _unit.ChemicalLocalPurchaseBillItemRepository.Get()
                                    .Where(ob => ob.BillRefID == delete.BillRefID)
                                    .ToList();
                            if (items.Count > 0)
                            {
                                foreach (var item in items)
                                {
                                    _unit.ChemicalLocalPurchaseBillItemRepository.Delete(item);
                                }
                            }
                            _unit.ChemicalLocalPurchaseBillReferenceRepository.Delete(delete);
                        }
                        _save = _unit.IsSaved();
                    }
                    tx.Complete();
                }
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

        public ValidationMsg DeleteItem(int billItemId)
        {
            try
            {
                var delete = _unit.ChemicalLocalPurchaseBillItemRepository.GetByID(billItemId);
                if (delete != null)
                {
                    _unit.ChemicalLocalPurchaseBillItemRepository.GetByID(billItemId);
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

        public ValidationMsg DeleteBill(int billId)
        {
            try
            {
                var delete = _unit.ChemicalLocalPurchaseBillRepository.GetByID(billId);
                if (delete != null)
                {
                    var deleteRef =
                        _unit.ChemicalLocalPurchaseBillReferenceRepository.Get().Where(ob => ob.BillID == delete.BillID).ToList();
                    foreach (var delRef in deleteRef)
                    {
                        var deleteItem =
                            _unit.ChemicalLocalPurchaseBillItemRepository.Get()
                                .Where(ob => ob.BillRefID == delRef.BillRefID)
                                .ToList();
                        foreach (var item in deleteItem)
                        {
                            _unit.ChemicalLocalPurchaseBillItemRepository.Delete(item);
                        }
                        _unit.ChemicalLocalPurchaseBillReferenceRepository.Delete(delRef);
                    }
                    _unit.ChemicalLocalPurchaseBillRepository.Delete(delete);
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

        private PRQ_ChemLocalPurcBill BillConversion(PrqChemLocalPurcBill model, int userId, string url)
        {
            var entity = model.BillId == 0
                ? new PRQ_ChemLocalPurcBill()
                : (from b in _context.PRQ_ChemLocalPurcBill.AsEnumerable()
                   where b.BillID == model.BillId
                   select b).FirstOrDefault();

            entity.BillID = model.BillId;
            entity.BillNo = model.BillId == 0 ? DalCommon.GetPreDefineNextCodeByUrl(url) : model.BillNo;
            entity.BillDate = DalCommon.SetDate(model.BillDate);
            entity.PurchaseYear = model.PurchaseYear;
            entity.SupplierID = model.SupplierId;
            entity.SupplierAddressID = model.SupplierAddressId;
            entity.SupplierBillNo = model.SupplierBillNo;
            entity.SupBillDate = DalCommon.SetDate(model.SupBillDate);
            entity.BillCategory = "Real";
            entity.BillType = "Supplier";
            entity.Currency = model.Currency;
            entity.ExchangRate = model.ExchangRate;
            entity.ExchangCurrency = model.ExchangCurrency;
            entity.ExchangValue = model.ExchangValue;
            entity.BillAmt = model.BillAmt;
            entity.VatAmt = model.VatAmt;
            entity.DiscountPercent = model.DiscountPercent;
            entity.DiscountAmt = model.DiscountAmt;
            entity.PayableAmt = model.PayableAmt;
            entity.RecordStatus = model.BillId == 0 ? "NCF" : model.RecordStatus;
            entity.SetBy = model.BillId == 0
                ? userId
                : _unit.ChemicalLocalPurchaseBillRepository.GetByID(model.BillId).SetBy;
            entity.SetOn = model.BillId == 0
                ? DateTime.Now
                : _unit.ChemicalLocalPurchaseBillRepository.GetByID(model.BillId).SetOn;
            entity.ModifiedBy = model.BillId == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.BillId == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        private PRQ_ChemLocalPurcBillRef BillReferenceConversion(PrqChemLocalPurcBillRef model, int userId, long billId)
        {
            var entity = model.BillRefId == 0 ? new PRQ_ChemLocalPurcBillRef() : (from b in _context.PRQ_ChemLocalPurcBillRef.AsEnumerable()
                                                                                  where b.BillRefID == model.BillRefId
                                                                                  select b).FirstOrDefault();

            entity.BillRefID = model.BillRefId;
            entity.BillID = billId;
            entity.ReceiveID = model.ReceiveID;
            entity.OrderID = model.OrderID;
            entity.SetOn = model.BillRefId == 0
                ? DateTime.Now
                : _unit.ChemicalLocalPurchaseBillReferenceRepository.GetByID(model.BillRefId).SetOn;
            entity.SetBy = model.BillRefId == 0
                ? userId
                : _unit.ChemicalLocalPurchaseBillReferenceRepository.GetByID(model.BillRefId).SetBy;
            entity.ModifiedOn = model.BillRefId == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.BillRefId == 0 ? (int?)null : userId;

            return entity;
        }

        private PRQ_ChemLocalPurcBillItem BillItemConversion(PrqChemLocalPurcBillItem model, int userId, long? billId, long billRefId)
        {
            var entity = model.BillItemId == 0 ? new PRQ_ChemLocalPurcBillItem() : (from b in _context.PRQ_ChemLocalPurcBillItem.AsEnumerable()
                                                                                    where b.BillItemID == model.BillItemId
                                                                                    select b).FirstOrDefault();

            entity.BillItemID = model.BillItemId;
            entity.BillRefID = model.BillRefId ?? billRefId;
            entity.ItemID = model.ItemId;
            entity.SupplierID = model.SupplierId;
            entity.PackSize = model.PackSize;
            entity.SizeUnit = model.SizeUnit;
            entity.PackQty = model.PackQty;
            entity.ReceiveQty = model.ReceiveQty;
            entity.UnitID = model.UnitId;
            entity.UnitPrice = model.UnitPrice;
            entity.TotalPrice = model.TotalPrice;
            entity.ManufacturerID = model.ManufacturerId;
            entity.SetOn = model.BillItemId == 0
                ? DateTime.Now
                : _unit.ChemicalLocalPurchaseBillItemRepository.GetByID(model.BillItemId).SetOn;
            entity.SetBy = model.BillItemId == 0
                ? userId
                : _unit.ChemicalLocalPurchaseBillItemRepository.GetByID(model.BillItemId).SetBy;
            entity.ModifiedBy = model.BillItemId == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.BillItemId == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        public IEnumerable<string> GeBillListForSearch(string billNo)
        {
            return _context.PRQ_ChemLocalPurcBill.Select(m => m.SupplierBillNo).ToList();
        }

        public List<PrqChemLocalPurcBill> GetBillList(string billNo)
        {
            var searchList = _context.PRQ_ChemLocalPurcBill.Where(m => m.SupplierBillNo.StartsWith(billNo)).ToList();

            return searchList.Select(SetToBillModel).ToList();
        }

        private PrqChemLocalPurcBill SetToBillModel(PRQ_ChemLocalPurcBill entity)
        {
            var model = new PrqChemLocalPurcBill();
            model.BillId = entity.BillID;
            model.BillNo = entity.BillNo;
            model.BillDate = string.Format("{0:dd/MM/yyyy}",entity.BillDate);
            model.SupplierName = entity.SupplierID == null
                ? ""
                : _unit.SysSupplierRepository.GetByID(entity.SupplierID).SupplierName;
            model.SupplierBillNo = entity.SupplierBillNo;
            model.RecordStatus = DalCommon.ReturnRecordStatus(entity.RecordStatus);
            return model;
        }

        public IEnumerable<string> GeSupplierListForSearch(string supplier)
        {
            return _context.Sys_Supplier.Select(m => m.SupplierName).ToList();
        }

        public List<SupplierDetails> GetSupplierList(string supplier)
        {
            var searchList = _context.Sys_Supplier.Where(m => m.SupplierName.StartsWith(supplier)).ToList();

            return searchList.Select(SetToSupplierModel).ToList();
        }

        private SupplierDetails SetToSupplierModel(Sys_Supplier entity)
        {
            var model = new SupplierDetails();
            model.SupplierID = entity.SupplierID;
            model.SupplierCode = entity.SupplierCode;
            model.SupplierName = entity.SupplierName;
            var address = _context.Sys_SupplierAddress.FirstOrDefault(
                    ob => ob.SupplierID == entity.SupplierID && ob.IsActive && ob.IsDelete == false);
            if (address != null)
            {
                model.SupplierAddressID = address.SupplierAddressID.ToString();
                model.Address = address.Address;
                model.ContactPerson = address.ContactPerson;
                model.ContactNumber = model.ContactNumber;
            }
            return model;
        }
    }

}

