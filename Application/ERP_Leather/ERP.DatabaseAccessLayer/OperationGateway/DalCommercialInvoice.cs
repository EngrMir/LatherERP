using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalCommercialInvoice
    {
        private BLC_DEVEntities _context;
        UnitOfWork _unit;
        ValidationMsg _validationMsg;
        private int save;
        private int _mode;
        public DalCommercialInvoice()
        {
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            save = 0;
            _context = new BLC_DEVEntities();
        }

        public int DeleteCommercialInvoiceItem(int CIItemID)
        {
            _unit.CommercialInvoiceItemRepository.Delete(CIItemID);
            var delete = _unit.Save();

            return delete;
        }

        public ValidationMsg Save(CommercialInvoiceVM model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope()) 
                {
                    using (_context)
                    {
                        var commercialInvoiceData = ModelConversionToLcmCommercialInvoice(model, userId, url);
                        if (model.CIID == 0)
                        {
                            _context.LCM_CommercialInvoice.Add(commercialInvoiceData);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.Items.Count > 0)
                        {
                            foreach (var items in model.Items)
                            {
                                var commercialInvoiceItemData = ModelConversionToLcmCommercialInvoiceItem(items, userId,
                                    commercialInvoiceData.CIID);
                                if (items.CIItemID == 0)
                                {
                                    _context.LCM_CommercialInvoiceItem.Add(commercialInvoiceItemData);
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
                            _validationMsg.ReturnId = commercialInvoiceData.CIID;
                            //_validationMsg.ReturnCode = _unit.CommercialInvoiceRepository.GetByID(_validationMsg.ReturnId).CINo;
                            _validationMsg.Type = Enums.MessageType.Success;
                            _validationMsg.Msg = "Saved successfully";
                        }
                        if (_mode == 2)
                        {
                            _validationMsg.ReturnId = commercialInvoiceData.CIID;
                            _validationMsg.Type = Enums.MessageType.Update;
                            _validationMsg.Msg = "Updated successfully";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to save";
            }
            return _validationMsg;
        }

        public LCM_CommercialInvoice ModelConversionToLcmCommercialInvoice(CommercialInvoiceVM model, int userId, string url)
        {
            var entity = model.CIID == 0
                ? new LCM_CommercialInvoice()
                : (from b in _context.LCM_CommercialInvoice.AsEnumerable()
                   where b.CIID == model.CIID
                   select b).FirstOrDefault();

            entity.CIID = model.CIID;
            entity.CINo = model.CINo;
            entity.CIDate = DalCommon.SetDate(model.CIDate);
            entity.CICurrency = model.CICurrency;
            entity.LCID = model.LCID;
            entity.LCNo = model.LCNo;
            entity.CIStatus = model.CIStatus;
            entity.ExchangeCurrency = model.ExchangeCurrency;
            entity.ExchangeRate = model.ExchangeRate;
            entity.ExchangeValue = model.ExchangeValue;
            entity.CINote = model.CINote;
            entity.RecordStatus = model.CIID == 0 ? "NCF" : _unit.CommercialInvoiceRepository.GetByID(model.CIID).RecordStatus;
            entity.SetBy = model.CIID == 0 ? userId : _unit.CommercialInvoiceRepository.GetByID(model.CIID).SetBy;
            entity.SetOn = model.CIID == 0 ? DateTime.Now : _unit.CommercialInvoiceRepository.GetByID(model.CIID).SetOn;
            entity.ModifiedBy = model.CIID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.CIID == 0 ? (DateTime?)null : DateTime.Now;
            return entity;
        }

        public LCM_CommercialInvoiceItem ModelConversionToLcmCommercialInvoiceItem(CommercialInvoiceItemVM model,
            int userId, int CIID)
        {
            var entity = model.CIItemID == 0
                ? new LCM_CommercialInvoiceItem()
                : (from b in _context.LCM_CommercialInvoiceItem.AsEnumerable()
                   where b.CIItemID == model.CIItemID
                   select b).FirstOrDefault();


            entity.CIItemID = model.CIItemID;
            entity.CIID = CIID;
            entity.ItemID = model.ItemID;
            entity.PackSize = model.PackSize;
            entity.SizeUnit = model.SizeUnit;
            entity.PackQty = model.PackQty;
            entity.CIQty = model.CIQty;
            entity.CIUnit = model.CIUnit;
            entity.CIUnitPrice = model.CIUnitPrice;
            entity.CITotalPrice = model.CITotalPrice;
            entity.SupplierID = model.SupplierID;
            entity.ManufacturerID = model.ManufacturerID;
            entity.SetOn = model.CIItemID == 0 ? DateTime.Now : _unit.CommercialInvoiceItemRepository.GetByID(model.CIItemID).SetOn;
            entity.SetBy = model.CIItemID == 0 ? userId : _unit.CommercialInvoiceItemRepository.GetByID(model.CIItemID).SetBy;
            entity.ModifiedBy = model.CIItemID == 0 ? (int?)null: userId;
            entity.ModifiedOn = model.CIItemID == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        public ValidationMsg Check(int ciid, int userId, string chkNote)
        {
            var commercialInvoice = _unit.CommercialInvoiceRepository.GetByID(ciid);
            commercialInvoice.CheckedBy = userId;
            commercialInvoice.CheckDate = DateTime.Now;
            commercialInvoice.CINote = chkNote;
            commercialInvoice.RecordStatus = "CHK";
            _unit.CommercialInvoiceRepository.Update(commercialInvoice);
            save = _unit.Save();
            if (save == 1)
            {
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Check confirmed.";
            }
            else
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm check.";
            }
            return _validationMsg;
        }

        public ValidationMsg Confirm(int ciid, int userId, string apvNote)
        {
            var commercialInvoice = _unit.CommercialInvoiceRepository.GetByID(ciid);
            commercialInvoice.ApprovedBy = userId;
            commercialInvoice.ApprovalAdvice = apvNote;
            commercialInvoice.ApproveDate = DateTime.Now;
            commercialInvoice.RecordStatus = "CNF";
            _unit.CommercialInvoiceRepository.Update(commercialInvoice);
            save = _unit.Save();
            if (save == 1)
            {
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Invoice confirmed successfully.";
            }
            else
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm invoice.";
            }
            return _validationMsg;
        }

        public ValidationMsg DeleteCommercialInvoice(int ciid)
        {
            try
            {
                var invoiceItems = _unit.CommercialInvoiceItemRepository.Get().Where(ob => ob.CIID == ciid).ToList();
                if (invoiceItems.Count > 0)
                {
                    foreach (var item in invoiceItems)
                    {
                        _unit.CommercialInvoiceItemRepository.Delete(item);
                    }
                }
                _unit.CommercialInvoiceRepository.Delete(ciid);
                save = _unit.Save();
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Invoice successfully deleted.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Reference of this invoice found in other record, system failed to delete invoice.";
            }
            return _validationMsg;
        }
    }
}
