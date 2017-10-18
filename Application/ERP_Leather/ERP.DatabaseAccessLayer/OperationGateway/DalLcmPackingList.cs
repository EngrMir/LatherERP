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
    public class DalLcmPackingList
    {
        private readonly BLC_DEVEntities _context;
        UnitOfWork _unit;
        ValidationMsg _validationMsg;
        private bool save;
        private int _mode;

        public DalLcmPackingList()
        {
            _context = new BLC_DEVEntities();
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            save = false;
        }

        public ValidationMsg DeletePackingListPacks(int plPackID)
        {
            _unit.PackingListPacksRepository.Delete(plPackID);
            save = _unit.IsSaved();
            if (save)
            {
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Items is successfully deleted";
            }
            else
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete item";
            }
            return _validationMsg;
        }

        public ValidationMsg DeletePackingListItem(int plItemID)
        {
            _unit.PackingListItemRepository.Delete(plItemID);
            save = _unit.IsSaved();
            if (save)
            {
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Items is successfully deleted";
            }
            else
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete item";
            }
            return _validationMsg;
        }

        public ValidationMsg Save(PackingListVM model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var packingList = ModelConversionPackingList(model, userId, url);
                        if (model.Plid == 0)
                        {
                            //_unit.PackingListRepository.Insert(packingList);
                            _context.LCM_PackingList.Add(packingList);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            //_unit.PackingListRepository.Update(packingList);
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.packingListItem != null)
                        {
                            foreach (var item in model.packingListItem)
                            {
                                var packingItemList = ModelConversionPackingListItem(item, userId, packingList.PLID);
                                if (item.PlItemID == 0)
                                {
                                    //_unit.PackingListItemRepository.Insert(packingItemList);
                                    _context.LCM_PackingListItem.Add(packingItemList);
                                    
                                }
                                else
                                {
                                    //_unit.PackingListItemRepository.Update(packingItemList);
                                    //_context.SaveChanges();
                                }
                            }
                            _context.SaveChanges();
                        }
                        if (model.packingListPacks != null)
                        {
                            foreach (var pack in model.packingListPacks)
                            {
                                var packList = ModelConversionPackingListPacks(pack, userId, packingList.PLID);
                                if (pack.PlPackID == 0)
                                {
                                    //_unit.PackingListPacksRepository.Insert(packList);
                                    _context.LCM_PackingListPacks.Add(packList);
                                    //_context.SaveChanges();
                                }
                                else
                                {
                                    //_unit.PackingListPacksRepository.Update(packList);
                                    //_context.SaveChanges();
                                }
                            }
                            _context.SaveChanges();
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _validationMsg.ReturnId = packingList.PLID;
                            _validationMsg.ReturnCode = packingList.PLNo;
                            _validationMsg.Type = Enums.MessageType.Success;
                            _validationMsg.Msg = "Saved successfully";
                        }
                        if (_mode == 2)
                        {
                            _validationMsg.ReturnId = packingList.PLID;
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

        private LCM_PackingList ModelConversionPackingList(PackingListVM model, int userId, string url)
        {
            var entity = model.Plid == 0 ? new LCM_PackingList() : (from b in _context.LCM_PackingList.AsEnumerable()
                                                                    where b.PLID == model.Plid
                                                                    select b).FirstOrDefault();

            entity.PLID = model.Plid;
            entity.PLNo = model.Plid == 0 ? DalCommon.GetPreDefineNextCodeByUrl(url) : model.PlNo;
            entity.PLDate = DalCommon.SetDate(model.PlDate);
            entity.LCID = model.Lcid;
            entity.LCNo = model.LcNo;
            entity.CIID = model.Ciid;
            entity.CINo = model.CiNo;
            entity.PLNetWeight = model.NetWeight;
            entity.NetWeightUnit = model.NetWeightUnit;
            entity.PLGrossWeight = model.GrossWeight;
            entity.GrossWeightUnit = model.GrossWeightUnit;
            entity.PLNote = model.PlNote;
            entity.RecordStatus = model.Plid == 0 ? "NCF" : model.RecordStatus;
            entity.SetOn = model.Plid == 0 ? DateTime.Now : _unit.PackingListRepository.GetByID(model.Plid).SetOn;
            entity.SetBy = model.Plid == 0 ? userId : _unit.PackingListRepository.GetByID(model.Plid).SetBy;
            entity.ModifiedBy = model.Plid == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.Plid == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        private LCM_PackingListItem ModelConversionPackingListItem(PackingListItemVM model, int userId, int plid)
        {
            var entity = model.PlItemID == 0 ? new LCM_PackingListItem() : (from b in _context.LCM_PackingListItem.AsEnumerable()
                                                                            where b.PLItemID == model.PlItemID
                                                                            select b).FirstOrDefault();

            entity.PLItemID = model.PlItemID;
            entity.PLID = plid;
            entity.ItemID = model.ItemID;
            entity.PackSize = model.PackSize;
            entity.SizeUnit = model.SizeUnit;
            entity.PackQty = model.PackQty;
            entity.PLQty = model.PlQty;
            entity.PLUnit = model.PlUnit;
            entity.SupplierID = model.SupplierID;
            entity.ManufacturerID = model.ManufacturerID;
            entity.SetOn = model.PlItemID == 0 ? DateTime.Now : _unit.PackingListItemRepository.GetByID(model.PlItemID).SetOn;
            entity.SetBy = model.PlItemID == 0 ? userId : _unit.PackingListItemRepository.GetByID(model.PlItemID).SetBy;
            entity.ModifiedOn = model.PlItemID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.PlItemID == 0 ? (int?)null : userId;

            return entity;
        }

        private LCM_PackingListPacks ModelConversionPackingListPacks(PackingListPacksVM model, int userId, int plid)
        {
            var entity = model.PlPackID == 0 ? new LCM_PackingListPacks() : (from b in _context.LCM_PackingListPacks.AsEnumerable()
                                                                             where b.PLPackID == model.PlPackID
                                                                             select b).FirstOrDefault();

            entity.PLPackID = model.PlPackID;
            entity.PLID = plid;
            entity.PackUnit = model.PackUnit;
            entity.PackQty = model.PackQty;
            entity.SetOn = model.PlPackID == 0 ? DateTime.Now : _unit.PackingListPacksRepository.GetByID(model.PlPackID).SetOn;
            entity.SetBy = model.PlPackID == 0 ? userId : _unit.PackingListPacksRepository.GetByID(model.PlPackID).SetBy;
            entity.ModifiedOn = model.PlPackID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.PlPackID == 0 ? (int?)null : userId;

            return entity;
        }

        public ValidationMsg Check(int plid, int userId, string checkNote)
        {
            try
            {
                var packingList = _unit.PackingListRepository.GetByID(plid);
                packingList.CheckedBy = userId;
                packingList.CheckDate = DateTime.Now;
                packingList.RecordStatus = "CHK";
                packingList.PLNote = checkNote;
                _unit.PackingListRepository.Update(packingList);
                save = _unit.IsSaved();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Check confirmed.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm check.";
            }
            return _validationMsg;
        }

        public ValidationMsg Confirm(int plid, int userId, string confirmNote)
        {
            try
            {
                var packingList = _unit.PackingListRepository.GetByID(plid);
                packingList.RecordStatus = "CNF";
                packingList.PLNote = confirmNote;
                _unit.PackingListRepository.Update(packingList);
                save = _unit.IsSaved();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Packing list confirmed.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm.";
            }
            return _validationMsg;
        }

        public ValidationMsg DeletePackingList(int plid)
        {
            try
            {
                var packingListPacks = _unit.PackingListPacksRepository.Get().Where(ob => ob.PLID == plid).ToList();
                if (packingListPacks.Count > 0)
                {
                    foreach (var pack in packingListPacks)
                    {
                        _unit.PackingListPacksRepository.Delete(pack);
                    }
                }

                var packingListItems = _unit.PackingListItemRepository.Get().Where(ob => ob.PLID == plid).ToList();
                if (packingListItems.Count > 0)
                {
                    foreach (var item in packingListItems)
                    {
                        _unit.PackingListItemRepository.Delete(item);
                    }
                }
                _unit.PackingListRepository.Delete(plid);

                save = _unit.IsSaved();
                if (save)
                {
                    _validationMsg.Type = Enums.MessageType.Delete;
                    _validationMsg.Msg = "Successfully deleted the record.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete the record.";
            }
            return _validationMsg;
        }

        public ValidationMsg DelPckLstCont(long plid)
        {
            try
            {
                var pckLstConts = _unit.PackingListItemRepository.Get().Where(ob => ob.PLID == plid).ToList();
                if (pckLstConts.Count > 0)
                {
                    foreach (var item in pckLstConts)
                    {
                        _unit.PackingListItemRepository.Delete(item);
                    }
                }
                var saves = _unit.IsSaved();
                if (saves)
                {
                    _validationMsg.Type = Enums.MessageType.Success;

                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
            }
            return _validationMsg;
        }
    }
}
