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
    public class DalExpFreightBillDoc
    {
        private readonly BLC_DEVEntities _context;
        private readonly UnitOfWork _unit;
        private readonly ValidationMsg _msg;
        private int _mode;

        public DalExpFreightBillDoc()
        {
            _context = new BLC_DEVEntities();
            _msg = new ValidationMsg();
            _unit = new UnitOfWork();
            _mode = new int();
        }

        public ValidationMsg Save(ExpFreightBill model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var freightBill = CovertModel(model, userId, url);
                        if (model.FreightBillID == 0)
                        {
                            _context.EXP_FreightBill.Add(freightBill);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _msg.ReturnId = freightBill.FreightBillID;
                            _msg.ReturnCode = freightBill.FreightBillNo;
                            _msg.Type = Enums.MessageType.Success;
                            _msg.Msg = "Saved successfully";
                        }
                        if (_mode == 2)
                        {
                            _msg.ReturnId = freightBill.FreightBillID;
                            _msg.ReturnCode = freightBill.FreightBillNo;
                            _msg.Type = Enums.MessageType.Update;
                            _msg.Msg = "Updated successfully";
                        }
                    }
                }
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to save";
            }
            return _msg;
        }

        private EXP_FreightBill CovertModel(ExpFreightBill model, int userId, string url)
        {
            var entity = model.FreightBillID == 0
                ? new EXP_FreightBill()
                : (from b in _context.EXP_FreightBill.AsEnumerable()
                   where b.FreightBillID == model.FreightBillID
                   select b).FirstOrDefault();

            entity.FreightBillID = model.FreightBillID;
            entity.FreightBillNo = model.FreightBillNo ?? DalCommon.GetPreDefineNextCodeByUrl(url);
            entity.FreightBillRef = model.FreightBillRef;
            entity.FreightBillDate = model.FreightBillDate == null ? (DateTime?)null : DalCommon.SetDate(model.FreightBillDate);
            entity.LCID = model.LCID;
            entity.CIID = model.CIID;
            entity.PIID = model.PIID;
            entity.BLID = model.BLID;
            entity.FreightAgentID = model.FreightAgentID;
            //entity.NotifyTo = model.NotifyTo;
            entity.MSNo = model.MSNo;
            entity.MSDate = model.MSDate == null ? (DateTime?)null : DalCommon.SetDate(model.MSDate);
            entity.ShipmentOf = model.ShipmentOf;
            entity.ShipmentFrom = model.ShipmentFrom;
            entity.ShipmentTo = model.ShipmentTo;
            entity.WBNo = model.WBNo;
            entity.WBDate = model.WBDate == null ? (DateTime?)null : DalCommon.SetDate(model.WBDate);
            entity.FreightWeight = model.FreightWeight;
            entity.FreightRate = model.FreightRate;
            entity.FreightValue = model.FreightValue;
            entity.FreightBillCurrency = model.FreightBillCurrency;
            entity.ExchangeCurrency = model.ExchangeCurrency;
            entity.ExchangeRate = model.ExchangeRate;
            entity.ExchangeValue = model.ExchangeValue;
            //entity.THCharge = model.THCharge;
            entity.AirWayBill = model.AirWayBill;
            entity.FCSMYCCharge = model.FCSMYCCharge;
            entity.SSCMCCCharge = model.SSCMCCCharge;
            entity.OtherCharge = model.OtherCharge;
            entity.LocalCarringCharge = model.LocalCarringCharge;
            entity.CustomCharge = model.CustomCharge;
            entity.VATAsReceipt = model.VATAsReceipt;
            entity.LoadUnloadCharge = model.LoadUnloadCharge;
            //entity.GSPEXpence = model.GSPEXpence;
            entity.AgencyCommision = model.AgencyCommision;
            entity.SpecialDeliveryCharge = model.SpecialDeliveryCharge;
            entity.TotalAmt = model.TotalAmt;
            //entity.AdvanceAmt = model.AdvanceAmt;
            //entity.NetFreightAmt = model.NetFreightAmt;
            entity.TrminalCharge = model.TrminalCharge;
            entity.ExamineCharge = model.ExamineCharge;
            entity.AmendmentCharge = model.AmendmentCharge;
            entity.RecordStatus = model.RecordStatus ?? "NCF";
            entity.SetOn = model.FreightBillID == 0
                ? DateTime.Now
                : _unit.FreightBillRepository.GetByID(model.FreightBillID).SetOn;
            entity.SetBy = model.FreightBillID == 0
                ? userId
                : _unit.FreightBillRepository.GetByID(model.FreightBillID).SetBy;
            entity.ModifiedBy = model.FreightBillID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.FreightBillID == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        public object GetFrghtBillDocs()
        {
            var billDocs = _context.EXP_FreightBill.ToList();

            var results = billDocs.Select(
                bill => new
                {
                    bill.FreightBillID,
                    bill.FreightBillNo,
                    FreightBillDate = string.Format("{0:dd/MM/yyyy}", bill.FreightBillDate),
                    bill.FreightBillRef,
                    //bill.LCID,
                    //LCNo = bill.LCID == null ? "" : _context.EXP_LCOpening.FirstOrDefault(ob => ob.LCID == bill.LCID).LCNo,
                    //bill.PIID,
                    //PINo = bill.PIID == null ? "" : _context.EXP_LeatherPI.FirstOrDefault(ob => ob.PIID == bill.PIID).PINo,
                    bill.CIID,
                    CINo = bill.CIID == null ? "" : _context.EXP_CI.FirstOrDefault(ob => ob.CIID == bill.CIID).CINo,
                    CIRefNo = bill.CIID == null ? "" : _context.EXP_CI.FirstOrDefault(ob => ob.CIID == bill.CIID).CIRefNo,
                    bill.BLID,
                    BLNo = bill.BLID == null ? "" : _context.EXP_BillofLading.FirstOrDefault(ob => ob.BLID == bill.BLID).BLNo,
                    bill.FreightAgentID,
                    FreightAgentName = bill.FreightAgentID == null ? ""
                    : _context.Sys_Buyer.FirstOrDefault(ob => ob.BuyerID == bill.FreightAgentID).BuyerName,
                    RecordStatus = DalCommon.ReturnRecordStatus(bill.RecordStatus)
                }).OrderByDescending(ob => ob.FreightBillID).ToList();
            return results;
        }


        public object GetFrghtBillDoc(long fbdId)
        {
            var frghtBillDoc = _unit.FreightBillRepository.GetByID(fbdId);
            var result = new ExpFreightBill();
            result.FreightBillID = frghtBillDoc.FreightBillID;
            result.FreightBillNo = frghtBillDoc.FreightBillNo;
            result.FreightBillRef = frghtBillDoc.FreightBillRef;
            //result.FreightBillRef = frghtBillDoc.FreightBillRef;
            result.FreightBillDate = frghtBillDoc.FreightBillDate == null ? "" :
                string.Format("{0:dd/MM/yyyy}", frghtBillDoc.FreightBillDate);
            //result.LCID = frghtBillDoc.LCID;
            //result.PIID = frghtBillDoc.PIID;
            //result.PINo = frghtBillDoc.PIID == null ? "" : _unit.ExpLeatherPI.GetByID(frghtBillDoc.PIID).PINo;
            //result.PIDate = frghtBillDoc.PIID == null
            //    ? ""
            //    : string.Format("{0:dd/MM/yyyy}", _unit.ExpLeatherPI.GetByID(frghtBillDoc.PIID).PIDate);
            result.CIID = frghtBillDoc.CIID;
            result.CINo = frghtBillDoc.CIID == null
                ? ""
                : _unit.ExpCommercialInvoiceRepository.GetByID(frghtBillDoc.CIID).CINo;
            result.CIRefNo = frghtBillDoc.CIID == null
                ? ""
                : _unit.ExpCommercialInvoiceRepository.GetByID(frghtBillDoc.CIID).CIRefNo;
            result.CIDate = frghtBillDoc.CIID == null
               ? ""
               : string.Format("{0:dd/MM/yyyy}", _unit.ExpCommercialInvoiceRepository.GetByID(frghtBillDoc.CIID).CIDate);
            result.OrdDeliveryMode = frghtBillDoc.CIID == null
                ? ""
                : DalCommon.ReturnOrderDeliveryMode(
                    _unit.ExpCommercialInvoiceRepository.GetByID(frghtBillDoc.CIID).OrdDeliveryMode);
            result.BLID = frghtBillDoc.BLID;
            result.BLNo = frghtBillDoc.BLID == null
                ? ""
                : _unit.ExpBillOfLadingRepository.GetByID(frghtBillDoc.BLID).BLNo;
            result.BLDate = frghtBillDoc.BLID == null
                ? ""
                : string.Format("{0:dd/MM/yyyy}", _unit.ExpBillOfLadingRepository.GetByID(frghtBillDoc.BLID).BLDate);
            result.FreightAgentID = frghtBillDoc.FreightAgentID;
            result.FreightAgentCode = frghtBillDoc.FreightAgentID == null
                ? ""
                : _unit.SysBuyerRepository.GetByID(frghtBillDoc.FreightAgentID).BuyerCode;
            result.FreightAgentName = frghtBillDoc.FreightAgentID == null
                ? ""
                : _unit.SysBuyerRepository.GetByID(frghtBillDoc.FreightAgentID).BuyerName;
            //result.NotifyTo = frghtBillDoc.NotifyTo;
            result.MSNo = frghtBillDoc.MSNo;
            result.MSDate = string.Format("{0:dd/MM/yyyy}", frghtBillDoc.MSDate);
            result.ShipmentOf = frghtBillDoc.ShipmentOf;
            result.ShipmentFrom = frghtBillDoc.ShipmentFrom;
            result.ShipmentTo = frghtBillDoc.ShipmentTo;
            result.WBNo = frghtBillDoc.WBNo;
            result.WBDate = string.Format("{0:dd/MM/yyyy}", frghtBillDoc.WBDate);
            result.FreightWeight = frghtBillDoc.FreightWeight;
            result.FreightRate = frghtBillDoc.FreightRate;
            result.FreightValue = frghtBillDoc.FreightValue;
            result.FreightBillCurrency = frghtBillDoc.FreightBillCurrency;
            result.ExchangeCurrency = frghtBillDoc.ExchangeCurrency;
            result.ExchangeRate = frghtBillDoc.ExchangeRate;
            result.ExchangeValue = frghtBillDoc.ExchangeValue;
            //result.THCharge = frghtBillDoc.THCharge;
            result.AirWayBill = frghtBillDoc.AirWayBill;
            result.FCSMYCCharge = frghtBillDoc.FCSMYCCharge;
            result.SSCMCCCharge = frghtBillDoc.SSCMCCCharge;
            result.OtherCharge = frghtBillDoc.OtherCharge;
            result.LocalCarringCharge = frghtBillDoc.LocalCarringCharge;
            result.CustomCharge = frghtBillDoc.CustomCharge;
            result.VATAsReceipt = frghtBillDoc.VATAsReceipt;
            result.LoadUnloadCharge = frghtBillDoc.LoadUnloadCharge;
            //result.GSPEXpence = frghtBillDoc.GSPEXpence;
            result.AgencyCommision = frghtBillDoc.AgencyCommision;
            result.SpecialDeliveryCharge = frghtBillDoc.SpecialDeliveryCharge;
            result.TotalAmt = frghtBillDoc.TotalAmt;
            //result.AdvanceAmt = frghtBillDoc.AdvanceAmt;
            //result.NetFreightAmt = frghtBillDoc.NetFreightAmt;
            result.TrminalCharge = frghtBillDoc.TrminalCharge;
            result.ExamineCharge = frghtBillDoc.ExamineCharge;
            result.AmendmentCharge = frghtBillDoc.AmendmentCharge;
            result.FreightBillNote = frghtBillDoc.FreightBillNote;
            result.RecordStatus = frghtBillDoc.RecordStatus;

            return result;
        }

        public ValidationMsg Delete(long id)
        {
            try
            {
                using (_context)
                {
                    var delItm = _context.EXP_FreightBill.FirstOrDefault(ob => ob.FreightBillID == id);
                    if (delItm != null)
                    {
                        _context.EXP_FreightBill.Remove(delItm);
                        _context.SaveChanges();
                    }
                    _msg.Type = Enums.MessageType.Delete;
                    _msg.Msg = "Successfully deleted.";
                }
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to delete.";
            }
            return _msg;
        }

        public ValidationMsg Confirm(long id, int userId, string note)
        {
            try
            {
                var cnfData = _context.EXP_FreightBill.FirstOrDefault(ob => ob.FreightBillID == id);
                if (cnfData != null)
                {
                    cnfData.RecordStatus = "CNF";
                    cnfData.ModifiedBy = userId;
                    cnfData.ModifiedOn = DateTime.Now;
                    cnfData.FreightBillNote = note;
                }
                _context.SaveChanges();
                _msg.Type = Enums.MessageType.Success;
                _msg.Msg = "Successfully Confirmed";
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to confirm.";
            }

            return _msg;
        }

        public ValidationMsg Check(long id, int userId, string note)
        {
            try
            {
                var chkData = _context.EXP_FreightBill.FirstOrDefault(ob => ob.FreightBillID == id);
                if (chkData != null)
                {
                    chkData.RecordStatus = "CHK";
                    chkData.ModifiedBy = userId;
                    chkData.ModifiedOn = DateTime.Now;
                    chkData.FreightBillNote = note;
                }
                _context.SaveChanges();
                _msg.Type = Enums.MessageType.Success;
                _msg.Msg = "Checked successfully.";
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to check.";
            }

            return _msg;
        }


    }
}
