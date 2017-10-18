using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalExpBillOfLading
    {
        private readonly BLC_DEVEntities _context;
        private UnitOfWork _unit;
        private ValidationMsg _msg;
        private int _mode;

        public DalExpBillOfLading()
        {
            _context = new BLC_DEVEntities();
            _msg = new ValidationMsg();
            _unit = new UnitOfWork();
            _mode = new int();
        }

        public ValidationMsg Save(ExpBillOfLading model, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var bol = ConvertBol(model, userId);
                        if (model.BLID == 0)
                        {
                            _context.EXP_BillofLading.Add(bol);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.Container != null)
                        {
                            foreach (var cont in model.Container)
                            {
                                var bolc = ConvertBolc(cont, bol.BLID, userId);
                                if (cont.BLCcntID == 0)
                                {
                                    _context.EXP_BillofLadingContainer.Add(bolc);
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
                            _msg.ReturnId = bol.BLID;
                            _msg.ReturnCode = bol.BLNo;
                            _msg.Type = Enums.MessageType.Success;
                            _msg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _msg.Type = Enums.MessageType.Update;
                            _msg.Msg = "Updated successfully.";
                        }
                    }
                }
            }
            catch(Exception)
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to save.";
            }
            return _msg;
        }

        public ValidationMsg Check(long id, string note, int userId)
        {
            try
            {
                var bol = _unit.ExpBillOfLadingRepository.GetByID(id);
                bol.BLNote = note;
                bol.RecordStatus = "CHK";
                bol.CheckedBy = userId;
                bol.CheckDate = DateTime.Now;

                _unit.IsSaved();
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

        public ValidationMsg Confirm(long id, string note, int userId)
        {
            try
            {
                var bol = _unit.ExpBillOfLadingRepository.GetByID(id);
                if (bol != null)
                {
                    bol.BLNote = note;
                    bol.RecordStatus = "CNF";
                    bol.CheckedBy = userId;
                    bol.CheckDate = DateTime.Now;

                    _unit.IsSaved();
                    _msg.Type = Enums.MessageType.Success;
                    _msg.Msg = "Confirmed successfully.";
                }
            }
            catch (Exception)
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to confirm.";
            }
            return _msg;
        }

        public ValidationMsg DeleteCont(long id)
        {
            try
            {
                var cont = _unit.ExpBillOfLadingContainerRepository.GetByID(id);
                if (cont != null)
                {
                    _unit.ExpBillOfLadingContainerRepository.Delete(cont);
                }
                _unit.IsSaved();
                _msg.Type = Enums.MessageType.Delete;
                _msg.Msg = "Deleted successfully.";
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to delete.";
            }
            return _msg;
        }

        public ValidationMsg DeleteAll(long id)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var bol = _context.EXP_BillofLading.FirstOrDefault(ob => ob.BLID == id);
                        if (bol != null)
                        {
                            var bolc =
                                _context.EXP_BillofLadingContainer.Where(ob => ob.BLID == id).ToList();
                            if (bolc.Count > 0)
                            {
                                foreach (var cont in bolc)
                                {
                                    _context.EXP_BillofLadingContainer.Remove(cont);
                                }
                            }
                            _context.EXP_BillofLading.Remove(bol);
                            _context.SaveChanges();
                        }
                        tx.Complete();
                        _msg.Type = Enums.MessageType.Delete;
                        _msg.Msg = "Deleted successfully.";
                    }
                }
            }
            catch (Exception)
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to delete.";
            }
            return _msg;
        }

        private EXP_BillofLading ConvertBol(ExpBillOfLading model, int userId)
        {
            var entity = model.BLID == 0
                ? new EXP_BillofLading()
                : (from b in _context.EXP_BillofLading.AsEnumerable()
                   where b.BLID == model.BLID
                   select b).FirstOrDefault();
            entity.BLID = model.BLID;
            entity.BLNo = model.BLNo ?? BillNoGenerate();
            entity.RefBLNo = model.RefBLNo;
            entity.BLDate = model.BLDate == null ? DateTime.Now : DalCommon.SetDate(model.BLDate);
            entity.CIID = model.CIID;
            //entity.PLID = model.PLID;
            entity.ShippedOnBoardDate = model.ShippedOnBoardDate == null
                ? DateTime.Now
                : DalCommon.SetDate(model.ShippedOnBoardDate);
            entity.ExpectedArrivalTime = model.ExpectedArrivalTime == null
                ? DateTime.Now
                : DalCommon.SetDate(model.ExpectedArrivalTime);
            entity.Shipper = model.Shipper;
            entity.ShipmentMode = model.ShipmentMode;
            entity.VesselName = model.VesselName;
            entity.VoyageNo = model.VoyageNo;
            entity.TransShipmentPort = model.TransShipmentPort;
            entity.ShipmentPort = model.ShipmentPort;
            entity.RecordStatus = model.BLID == 0
                ? "NCF"
                : _context.EXP_BillofLading.FirstOrDefault(ob => ob.BLID == model.BLID).RecordStatus;
            entity.SetBy = model.BLID == 0
                ? userId
                : _context.EXP_BillofLading.FirstOrDefault(ob => ob.BLID == model.BLID).SetBy;
            entity.SetOn = model.BLID == 0
                ? DateTime.Now
                : _context.EXP_BillofLading.FirstOrDefault(ob => ob.BLID == model.BLID).SetOn;
            entity.ModifiedBy = model.BLID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.BLID == 0 ? (DateTime?)null : DateTime.Now;
            return entity;
        }

        private EXP_BillofLadingContainer ConvertBolc(ExpBillOfLadingContainer model, long billId, int userId)
        {
            var entity = model.BLCcntID == 0 ? new EXP_BillofLadingContainer() : (from b in _context.EXP_BillofLadingContainer.AsEnumerable()
                                                                                  where b.BLCcntID == model.BLCcntID
                                                                                  select b).FirstOrDefault();
            entity.BLCcntID = model.BLCcntID;
            entity.BLID = billId;
            entity.ContainerNo = model.ContainerNo;
            entity.ContainerType = model.ContainerType;
            entity.SealNo = model.SealNo;
            entity.PackageQty = model.PackageQty;
            entity.GrossWeight = model.GrossWeight;
            var wUnit = _context.Sys_Unit.FirstOrDefault(ob => ob.UnitName == "KG");
            entity.WeightUnit = wUnit == null ? (byte?)null : wUnit.UnitID ;
            entity.Measurement = model.Measurement;
            var mUnit = _context.Sys_Unit.FirstOrDefault(ob => ob.UnitName == "CBM");
            entity.MeasurementUnit = mUnit == null ? (byte?) null : mUnit.UnitID;
            entity.SetOn = model.BLCcntID == 0
                ? DateTime.Now
                : _unit.ExpBillOfLadingContainerRepository.GetByID(model.BLCcntID).SetOn;
            entity.SetBy = model.BLCcntID == 0
                ? userId
                : _unit.ExpBillOfLadingContainerRepository.GetByID(model.BLCcntID).SetBy;
            return entity;
        }

        private string BillNoGenerate()
        {
            var maxId = _context.EXP_BillofLading.OrderByDescending(ob => ob.BLID).FirstOrDefault();
            if (maxId == null) return "BLNo-1";
            var newId = maxId.BLID + 1;
            var str = "BLNo-" + newId;
            return str;
        }
    }
}
