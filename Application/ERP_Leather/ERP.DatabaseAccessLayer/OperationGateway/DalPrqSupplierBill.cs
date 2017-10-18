using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Data.SqlClient;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqSupplierBill
    {
        private readonly BLC_DEVEntities _context;
        private UnitOfWork _unit;
        private ValidationMsg _vmMsg;
        private int _mode;
        private bool _save;

        public DalPrqSupplierBill()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            _unit = new UnitOfWork();
            _mode = 0;
        }

        public ValidationMsg SaveSupplierBill(PrqSupplierBill billModel, List<PrqSupplierBillItem> billItemModel,
            List<PrqSupplierBillChallan> billChallanModel, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var billStatus = billModel.SupplierBillID == 0 ? "new" : "old";
                        var supplierBill = ConvertBill(billModel, userId);
                        if (billModel.SupplierBillID == 0)
                        {
                            _context.Prq_SupplierBill.Add(supplierBill);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }

                        if (billChallanModel != null)
                        {
                            foreach (var challan in billChallanModel)
                            {
                                var supplierChallan = ConvertChallan(challan, supplierBill.SupplierBillID,
                                    supplierBill.PurchaseID, billStatus);
                                if (billStatus == "new")
                                {
                                    _context.Prq_SupplierBillChallan.Add(supplierChallan);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                            }
                        }

                        if (billItemModel != null)
                        {
                            foreach (var item in billItemModel)
                            {

                                var supplierBillItem = ConvertItem(item, supplierBill.SupplierBillID, userId);
                                if (item.BillItemID == 0)
                                {
                                    _context.Prq_SupplierBillItem.Add(supplierBillItem);
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
                            _vmMsg.ReturnId = supplierBill.SupplierBillID;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated successfully.";
                        }
                    }
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg Delete(long id, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var bill = _unit.SupplierBillRepository.GetByID(id);
                        if (bill != null)
                        {
                            var challans =
                                _unit.SupplierBillChallanRepository.Get()
                                    .Where(ob => ob.SupplierBillID == id && ob.IsDelete == false)
                                    .ToList();
                            if (challans.Count > 0)
                            {
                                foreach (var challan in challans)
                                {
                                    challan.IsDelete = true;
                                    _unit.SupplierBillChallanRepository.Update(challan);
                                }
                            }
                            var items =
                                _unit.SupplierBillItemRepository.Get()
                                    .Where(ob => ob.SupplierBillID == bill.SupplierBillID && ob.IsDelete == false)
                                    .ToList();
                            if (items.Count > 0)
                            {
                                foreach (var item in items)
                                {
                                    item.IsDelete = true;
                                    item.ModifiedOn = DateTime.Now;
                                    item.ModifiedBy = userId;
                                    _unit.SupplierBillItemRepository.Update(item);
                                }
                            }

                            bill.IsDelete = true;
                            bill.ModifiedOn = DateTime.Now;
                            bill.ModifiedBy = userId;
                            _unit.SupplierBillRepository.Update(bill);
                        }
                        _save = _unit.IsSaved();
                        if (_save)
                        {
                            _vmMsg.Type = Enums.MessageType.Delete;
                            _vmMsg.Msg = "Deleted successfully";
                        }

                        tx.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeleteChallan(long id, long billId)
        {
            try
            {
                var challan =
                    _unit.SupplierBillChallanRepository.Get()
                        .FirstOrDefault(ob => ob.ChallanID == id && ob.SupplierBillID == billId);
                if (challan != null)
                {
                    challan.IsDelete = true;
                    _unit.SupplierBillChallanRepository.Update(challan);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Challan successfully deleted.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeleteItem(long id, int userId)
        {
            try
            {
                var item = _unit.SupplierBillItemRepository.GetByID(id);
                if (item != null)
                {
                    item.IsDelete = true;
                    item.ModifiedBy = userId;
                    item.ModifiedOn = DateTime.Now;
                    _unit.SupplierBillItemRepository.Update(item);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Item successfully deleted.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg ConfirmBill(long billId, string comment, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_unit)
                    {
                        var billConfirm = _unit.SupplierBillRepository.GetByID(billId);

                        if (billConfirm != null)
                        {
                            billConfirm.RecordStatus = "CNF";
                            billConfirm.CheckedBy = userId;
                            billConfirm.CheckDate = DateTime.Now;
                            billConfirm.CheckComment = comment;
                            var billItemConfirm =
                                _unit.SupplierBillItemRepository.Get()
                                    .Where(ob => ob.SupplierBillID == billId && ob.IsDelete == false)
                                    .ToList();
                            if (billItemConfirm.Count > 0)
                            {
                                foreach (var item in billItemConfirm)
                                {
                                    item.RecordStatus = "CNF";
                                    item.ModifiedBy = userId;
                                    item.ModifiedOn = DateTime.Now;
                                    _unit.SupplierBillItemRepository.Update(item);
                                }
                            }
                            _unit.SupplierBillRepository.Update(billConfirm);
                            _save = _unit.IsSaved();
                            if (_save)
                            {
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Bill confirmed successfully.";
                            }

                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please save the bill before you confirm it.";
                        }
                        tx.Complete();
                    }
                }

            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Confirmation failed.";
            }
            return _vmMsg;
        }

        public ValidationMsg ApproveBill(PrqSupplierBill billModel, IEnumerable<PrqSupplierBillItem> billItemModel,
            int userId)
        {
            try
            {
                var billApprove =
                    _context.Prq_SupplierBill.FirstOrDefault(w => w.SupplierBillID == billModel.SupplierBillID);
                if (billApprove != null)
                {
                    billApprove.RecordStatus = "APV";
                    billApprove.ApprovedPrice = billModel.ApprovedPrice;
                    billApprove.ApprovedAmt = billModel.ApprovedAmt;
                    billApprove.BillCategory = billModel.BillCategory;
                    billApprove.ApprovedBy = userId;
                    billApprove.ApproveDate = DateTime.Now;
                    billApprove.ApproveComment = billModel.ApproveComment;
                    if (billItemModel != null)
                    {
                        foreach (var item in billItemModel)
                        {
                            var billItemApprove =
                                _context.Prq_SupplierBillItem.SingleOrDefault(w => w.BillItemID == item.BillItemID);
                            if (billItemApprove != null)
                            {
                                billItemApprove.ApproveRate = item.ApproveRate;
                                billItemApprove.RecordStatus = "APV";
                                billItemApprove.ModifiedBy = userId;
                                billItemApprove.ModifiedOn = DateTime.Now;
                            }
                            _context.SaveChanges();
                        }
                    }
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Bill approved successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Please save the bill before approval.";
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Approval failed.";
            }
            return _vmMsg;
        }

        public string GetChallanCategory(long challanId)
        {
            var dalPrqPurchaseChallan = new DalPrqPurchaseChallan();

            string challanCategory = dalPrqPurchaseChallan.GetChallanCategoryByChallanId(challanId);
            return challanCategory;
        }


        public bool CheckExistingChallan(long challanId)
        {
            var challan = _context.Prq_SupplierBillChallan.FirstOrDefault(w => w.ChallanID == challanId);
            if (challan != null)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<PrqPurchaseChallan> GetChallanListInfo(string supplierId)
        {
            var query =
                " select PC.ChallanID, PC.ChallanNo,PC.ChallanCategory from Prq_Purchase P " +
                " inner join Prq_PurchaseChallan PC on PC.PurchaseID=P.PurchaseID " +
                " left join Prq_SupplierBillChallan SBC on SBC.ChallanID=PC.ChallanID where SBC.ChallanID is null AND P.SupplierID=" +
                supplierId;
            var purchaseChallan = _context.Database.SqlQuery<PrqPurchaseChallan>(query).ToList();

            return purchaseChallan;
        }

        public IEnumerable<PrqPurchase> GetPurchaseListBySupplier(int supplierId)
        {
            var sb = new StringBuilder();
            sb.Append(" SELECT P.PurchaseID,P.PurchaseNo,CONVERT(NVARCHAR(12), P.PurchaseDate,106)PurchaseDate,");
            sb.Append(" RecordStatus=case P.RecordStatus when 'CNF' THEN 'Confirmed' END FROM Prq_Purchase P");
            sb.Append(
                " LEFT JOIN Prq_SupplierBill SB ON SB.PurchaseID=P.PurchaseID where P.RecordStatus = 'CNF' AND SB.PurchaseID is null ");
            sb.Append(" AND P.SupplierID='" + supplierId + "' ORDER BY P.PurchaseID DESC");

            return _context.Database.SqlQuery<PrqPurchase>(sb.ToString()).ToList();
        }

        public Prq_SupplierBill ConvertBill(PrqSupplierBill model, int userId)
        {
            var entity = model.SupplierBillID == 0
                ? new Prq_SupplierBill()
                : (from b in _context.Prq_SupplierBill.AsEnumerable()
                   where b.SupplierBillID == model.SupplierBillID
                   select b).FirstOrDefault();
            entity.SupplierBillID = model.SupplierBillID;
            entity.SupplierBillNo = model.SupplierBillID == 0 ? (Convert.ToInt64(_unit.SupplierBillRepository.Get().LastOrDefault().SupplierBillNo) + 1).ToString() : model.SupplierBillNo ;//DalCommon.GetPreDefineNextCodeByUrl(model.SupplierBillNo);
            entity.SupplierBillRefNo = model.SupplierBillRefNo;
            entity.SupplierID = model.SupplierID;
            entity.SupplierAddressID = model.SupplierAddressID;
            entity.PurchaseID = model.PurchaseID;
            entity.BillCategory = model.BillCategory;
            entity.BillType = model.BillType;
            entity.BillDate = DalCommon.SetDate(model.BillDate);
            entity.PurchaseYear = model.PurchaseYear ?? "";
            entity.Remarks = model.Remarks;
            entity.TotalQty = model.TotalQty;
            entity.TotalRejectQty = model.TotalRejectQty;
            entity.AvgPrice = model.AvgPrice;
            entity.ApprovedPrice = model.ApprovedPrice;
            entity.OtherCost = model.OtherCost ?? 0;
            entity.DiscountAmt = model.DiscountAmt ?? 0;
            entity.DiscountPercent = model.DiscountPercent ?? 0;
            entity.ApprovedAmt = model.ApprovedAmt ?? 0;
            entity.TotalAmt = model.TotalAmt;
            entity.PayableAmt = model.PayableAmt;
            entity.RecordStatus = "NCF";
            entity.SetBy = model.SupplierBillID == 0
                ? userId
                : _unit.SupplierBillRepository.GetByID(model.SupplierBillID).SetBy;
            entity.SetOn = model.SupplierBillID == 0
                ? DateTime.Now
                : _unit.SupplierBillRepository.GetByID(model.SupplierBillID).SetOn;
            entity.ModifiedBy = model.SupplierBillID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.SupplierBillID == 0 ? (DateTime?)null : DateTime.Now;
            entity.IsDelete = false;
            return entity;
        }

        public Prq_SupplierBillChallan ConvertChallan(PrqSupplierBillChallan model, long billId, long? purchaseId,
            string billStatus)
        {
            var entity = billStatus == "new"
                ? new Prq_SupplierBillChallan()
                : (from b in _context.Prq_SupplierBillChallan.AsEnumerable()
                   where b.SupplierBillID == billId && b.ChallanID == model.ChallanID
                   select b).FirstOrDefault();
            entity.ChallanID = model.ChallanID;
            entity.SupplierBillID = billId;
            entity.S_ChallanRef = model.ChallanNo;
            entity.ChallanCategory = _unit.PrqPurchaseChallanRepo.GetByID(model.ChallanID).ChallanCategory;
            entity.PurchaseID = purchaseId;
            entity.IsDelete = false;
            return entity;
        }

        public Prq_SupplierBillItem ConvertItem(PrqSupplierBillItem model, long billId, int userId)
        {
            var entity = model.BillItemID == 0
                ? new Prq_SupplierBillItem()
                : (from b in _context.Prq_SupplierBillItem.AsEnumerable()
                   where b.BillItemID == model.BillItemID
                   select b).FirstOrDefault();
            entity.SupplierBillID = billId;
            entity.BillItemID = model.BillItemID;
            entity.ItemType = model.ItemTypeID;
            entity.ItemSize = model.ItemSizeID;
            entity.ItemQty = model.ItemQty;
            entity.RejectQty = model.RejectQty ?? 0;
            entity.ItemRate = model.ItemRate;
            entity.ApproveRate = model.ApproveRate;
            entity.Amount = model.Amount;
            entity.AmtUnit = model.AmtUnit;
            entity.AvgArea = model.AvgArea;
            entity.AreaUnit = model.AreaUnit;
            entity.Remarks = model.Remarks;
            entity.SetOn = model.BillItemID == 0
                ? DateTime.Now
                : _unit.SupplierBillItemRepository.GetByID(model.BillItemID).SetOn;
            entity.SetBy = model.BillItemID == 0
                ? userId
                : _unit.SupplierBillItemRepository.GetByID(model.BillItemID).SetBy;
            entity.RecordStatus = "NCF";
            entity.IsDelete = false;

            return entity;
        }

        public List<SupplierBill> GetBills(int supplierId)
        {
            var result = _context.Database.SqlQuery<SupplierBill>("EXEC FUspGetRawHideSupplierBill @SupplierID ={0}", supplierId).ToList();
            return result;
        }

        //public List<SupplierBill> GetBillsForSearch()
        //{

        //}

    }
}
