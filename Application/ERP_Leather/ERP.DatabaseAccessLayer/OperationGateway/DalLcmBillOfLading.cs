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
    public class DalLcmBillOfLading
    {
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private int _save;
        private int _mode;
        private BLC_DEVEntities _context;

        public DalLcmBillOfLading()
        {
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _validationMsg = new ValidationMsg();
            _save = 0;
        }

        public ValidationMsg DeleteBillOfLadingContainer(int blcCntId)
        {
            try
            {
                _unit.BillOfLadingContainerRepository.Delete(blcCntId);
                _save = _unit.Save();
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Successfully deleted container.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete container.";
            }
            return _validationMsg;
        }

        public ValidationMsg DeleteBillOfLading(int blid)
        {
            try
            {
                var containers = _unit.BillOfLadingContainerRepository.Get().Where(ob => ob.BLID == blid).ToList();
                if (containers.Count > 0)
                {
                    foreach (var container in containers)
                    {
                        _unit.BillOfLadingContainerRepository.Delete(container);
                    }
                }
                _unit.BillOfLadingRepository.Delete(blid);
                _unit.Save();
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Deleted successfully";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Failed to delete.";
            }
            return _validationMsg;
        }

        public ValidationMsg Save(LcmBillOfLadingVM model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var billOfLading = BillOfLadingModelConverter(model, userId, url);
                        if (model.Blid == 0)
                        {
                            _context.LCM_BillofLading.Add(billOfLading);
                            _context.SaveChanges();
                               _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.Containers != null)
                        {
                            foreach (var container in model.Containers)
                            {
                                var billOfLadingContainer = BillOfLadingContainerModelConverter(container, userId,
                                    billOfLading.BLID);
                                if (container.BlcCntId == 0)
                                {
                                    _context.LCM_BillofLadingContainer.Add(billOfLadingContainer);
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
                            _validationMsg.ReturnId = billOfLading.BLID;
                            _validationMsg.ReturnCode = billOfLading.BLNo;
                            _validationMsg.Type = Enums.MessageType.Success;
                            _validationMsg.Msg = "Saved successfully";
                        }
                        if (_mode == 2)
                        {
                            _validationMsg.Type = Enums.MessageType.Update;
                            _validationMsg.Msg = "Updated successfully";
                        }
                    }
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to save";
            }
            return _validationMsg;
        }

        public ValidationMsg Check(int blid, int userId, string checkNote)
        {
            try
            {
                var bol = _unit.BillOfLadingRepository.GetByID(blid);
                bol.CheckedBy = userId;
                bol.CheckDate = DateTime.Now;
                bol.RecordStatus = "CHK";
                bol.BLNote = checkNote;
                _unit.BillOfLadingRepository.Update(bol);
                _unit.Save();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Check confirmed.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Failed to confirm check.";
            }
            return _validationMsg;
        }

        public ValidationMsg Confirm(int blid, int userId, string confirmNote)
        {
            try
            {
                var bol = _unit.BillOfLadingRepository.GetByID(blid);
                bol.RecordStatus = "CNF";
                bol.BLNote = confirmNote;
                _unit.BillOfLadingRepository.Update(bol);
                _unit.Save();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Record confirmed successfully.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm record.";
            }
            return _validationMsg;
        }

        private LCM_BillofLading BillOfLadingModelConverter(LcmBillOfLadingVM model, int userId, string url)
        {
            var entity = model.Blid == 0 ? new LCM_BillofLading() : (from b in _context.LCM_BillofLading.AsEnumerable()
                                                                     where b.BLID == model.Blid select b).FirstOrDefault();

            entity.BLID = model.Blid;
            entity.BLNo = model.BlNo;
            entity.BLDate = DalCommon.SetDate(model.BlDate);
            entity.CIID = model.CIID;
            entity.CINo = model.CINo;
            entity.LCID = model.Lcid;
            entity.LCNo = model.LcNo;
            entity.ShippedOnBoardDate = DalCommon.SetDate(model.ShippedOnBoardDate);
            entity.ExpectedArrivalTime = DalCommon.SetDate(model.ExpectedArrivalTime);
            entity.Shipper = model.Shipper;
            entity.ShipmentMode = model.ShipmentMode;
            entity.VesselName = model.VesselName;
            entity.VoyageNo = model.VoyageNo;
            entity.TransShipmentPort = model.TransShipmentPort;
            entity.ShipmentPort = model.ShipmentPort;
            entity.BLNote = model.BlNote;
            if (model.Blid == 0)
            {
                entity.RecordStatus = "NCF";
                entity.SetBy = userId;
                entity.SetOn = DateTime.Now;
            }
            else
            {
                entity.ModifiedBy = userId;
                entity.ModifiedOn = DateTime.Now;
            }
            return entity;
        }


        private LCM_BillofLadingContainer BillOfLadingContainerModelConverter(LcmBillOfLadingContainerVM model, int userId, int blid)
        {
            var entity = model.BlcCntId == 0 ? new LCM_BillofLadingContainer() : (from b in _context.LCM_BillofLadingContainer.AsEnumerable()
                                                                                  where b.BLCcntID == model.BlcCntId select b).FirstOrDefault();

            entity.BLCcntID = model.BlcCntId;
            entity.BLID = blid;
            entity.ContainerNo = model.ContainerNo;
            entity.ContainerType = model.ContainerType;
            entity.SealNo = model.SealNo;
            entity.PackageQty = model.PackageQty;
            entity.GrossWeight = model.GrossWeight;
            entity.WeightUnit = model.WeightUnit;
            if (model.BlcCntId == 0)
            {
                entity.SetBy = userId;
                entity.SetOn = DateTime.Now;
            }
            else
            {
                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedBy = userId;
            }
            return entity;
        }
    }
}
