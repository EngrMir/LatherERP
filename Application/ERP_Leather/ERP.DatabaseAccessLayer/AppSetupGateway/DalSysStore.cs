using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysStore
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public long StoreID = 0;

        public DalSysStore()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysStore objSysStore, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var tblSysStore = new SYS_Store
                {
                    StoreID = objSysStore.StoreID,
                    StoreCode = objSysStore.StoreCode,
                    StoreName = objSysStore.StoreName,
                    StoreCategory = objSysStore.StoreCategory,
                    StoreType = objSysStore.StoreType,
                    IsActive = objSysStore.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = userid,
                    IPAddress = string.Empty
                };
                _context.SYS_Store.Add(tblSysStore);
                _context.SaveChanges();
                StoreID = tblSysStore.StoreID;
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Store Code Already Exit..";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Save.";
                }
            }

            return _vmMsg;
        }

        public long GetStoreId()
        {
            return StoreID;
        }

        public ValidationMsg Update(SysStore objSysStore, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var store = _context.SYS_Store.FirstOrDefault(s => s.StoreID == objSysStore.StoreID);
                if (store != null)
                {
                    store.StoreCode = objSysStore.StoreCode;
                    store.StoreName = objSysStore.StoreName;
                    store.StoreCategory = objSysStore.StoreCategory;
                    store.StoreType = objSysStore.StoreType;
                    store.IsActive = objSysStore.IsActive == "Active";
                    store.ModifiedOn = DateTime.Now;
                    store.ModifiedBy = userid;
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Store Code Already Exit..";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Update.";
                }
            }
            return _vmMsg;
        }

        public IEnumerable<SysStore> GetAll()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where !ss.IsDelete
                                                 select new SysStore
                                                         {
                                                             StoreID = ss.StoreID,
                                                             StoreCode = ss.StoreCode,
                                                             StoreName = ss.StoreName,
                                                             StoreCategory = ss.StoreCategory,
                                                             StoreType = ss.StoreType,
                                                             IsActive = ss.IsActive ? "Active" : "Inactive",

                                                         };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveConcernStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive == true
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };
            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive == true && !ss.IsDelete && ss.StoreType == "Raw Hide" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveChemicalStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Chemical" && ss.StoreCategory == "Chemical"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveProductionStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "WB Production" && ss.StoreCategory == "Production"
                                                 //where ss.IsActive && !ss.IsDelete && ss.StoreType == "WB Production" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveWetBlueProductionStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "CR Production" && ss.StoreCategory == "Production"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveFinishProductionStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "FN Production" && ss.StoreCategory == "Production"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveCrustQCStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Crust QC" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveFinishOwnQCStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Own QC" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveFinishBuyerQCStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Buyer QC" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveCrustStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Crust" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }


        public IEnumerable<SysStore> GetAllActiveWetBlueStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "WB Production" && ss.StoreCategory == "Production"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }


        public IEnumerable<SysStore> GetAllActiveWetBlueLeatherStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive == true && !ss.IsDelete && ss.StoreType == "Wet Blue" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveFinishedStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive == true && !ss.IsDelete && ss.StoreType == "FN Production" && ss.StoreCategory == "Production"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveStoreTransferReceiveStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive == true && !ss.IsDelete && ss.StoreType == "Finish" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActivePackingStores()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive == true && !ss.IsDelete && ss.StoreType == "Packing" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }


        public List<string> GetStoreListForSearch()
        {
            return _context.SYS_Store.Where(m => !m.IsDelete).Select(m => m.StoreName).ToList();
        }

        public IEnumerable<SysStore> GetAllActiveLoanStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Loan" && ss.StoreCategory == "Chemical"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllRawHideStore()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Raw Hide" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public ValidationMsg Delete(string storeId, int userid)
        {
            var storeid = string.IsNullOrEmpty(storeId) ? 0 : Convert.ToInt32(storeId);
            _vmMsg = new ValidationMsg();
            try
            {
                var store = _context.SYS_Store.FirstOrDefault(s => s.StoreID == storeid);
                _context.SYS_Store.Remove(store);
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public List<SYS_Store> StoreGetByID(int id)
        {
            return _context.SYS_Store.Where(ob => ob.StoreID == id).ToList();
        }

        public IEnumerable<SysStore> GetAllActiveTransactionStoreforCrusting()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Crust Fail" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }

        public IEnumerable<SysStore> GetAllActiveIssuedStoreforCrusting()
        {
            IEnumerable<SysStore> iLstSysStore = from ss in _context.SYS_Store
                                                 where ss.IsActive && !ss.IsDelete && ss.StoreType == "Crust Pass" && ss.StoreCategory == "Leather"
                                                 select new SysStore
                                                 {
                                                     StoreID = ss.StoreID,
                                                     StoreName = ss.StoreName,
                                                 };

            return iLstSysStore;
        }





    }
}
