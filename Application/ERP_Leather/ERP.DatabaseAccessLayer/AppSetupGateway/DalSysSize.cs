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
    public class DalSysSize
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public byte SizeId = 0;
        public DalSysSize()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg Save(SysSize objSysSize, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var exit = _context.Sys_Size.Where(m => m.SizeName == objSysSize.SizeName && m.SizeCategory == objSysSize.SizeCategory).Any();
                if (!exit)
                {
                    var tblSysSize = new Sys_Size
                    {
                        SizeID = objSysSize.SizeID,
                        SizeName = objSysSize.SizeName,
                        SizeCategory = objSysSize.SizeCategory,
                        IsActive = objSysSize.IsActive == "Active",
                        IsDelete = false,
                        SetOn = DateTime.Now,
                        SetBy = userid,
                        IPAddress = string.Empty,
                    };
                    _context.Sys_Size.Add(tblSysSize);
                    _context.SaveChanges();
                    SizeId = tblSysSize.SizeID;
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Save Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Duplicate Record.";
                }
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Save.";
            }

            return _vmMsg;
        }

        public long GetSizeId()
        {
            return SizeId;
        }

        public ValidationMsg Update(SysSize objSysSize, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var size = _context.Sys_Size.FirstOrDefault(s => s.SizeID == objSysSize.SizeID);
                if (size != null)
                {
                    size.SizeName = objSysSize.SizeName;
                    size.SizeCategory = objSysSize.SizeCategory;
                    size.IsActive = objSysSize.IsActive == "Active";
                    size.ModifiedOn = DateTime.Now;
                    size.ModifiedBy = userid;
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Update Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public IEnumerable<SysSize> GetAll()
        {
            IEnumerable<SysSize> iLstSysSize = from ss in _context.Sys_Size
                                               where !ss.IsDelete
                                               select new SysSize
                                                        {
                                                            SizeID = ss.SizeID,
                                                            SizeName = ss.SizeName,
                                                            SizeCategory = ss.SizeCategory,
                                                            IsActive = ss.IsActive ? "Active" : "Inactive",
                                                            IsDelete = ss.IsDelete,
                                                        };

            return iLstSysSize;
        }

        public IEnumerable<SysSize> GetAllActiveItemSize()
        {
            IEnumerable<SysSize> iLstSysSize = (from ss in _context.Sys_Size.AsEnumerable()
                                                where ss.IsActive && !ss.IsDelete && ss.SizeCategory == "Raw Hide Leather"
                                                select new SysSize
                                                {
                                                    SizeID = ss.SizeID,
                                                    SizeName = ss.SizeName
                                                }).ToList();

            return iLstSysSize;
        }

        public IEnumerable<SysSize> GetAllActiveChemicalItemSize()
        {
            IEnumerable<SysSize> iLstSysSize = (from ss in _context.Sys_Size.AsEnumerable()
                                                where ss.IsActive && !ss.IsDelete && ss.SizeCategory == "Chemical"
                                                select new SysSize
                                                {
                                                    SizeID = ss.SizeID,
                                                    SizeName = ss.SizeName
                                                }).ToList();

            return iLstSysSize;
        }

        public IEnumerable<SysSize> GetAllActiveChemicalPackSize()
        {
            IEnumerable<SysSize> iLstSysSize = (from ss in _context.Sys_Size.AsEnumerable()
                                                where ss.IsActive && !ss.IsDelete && ss.SizeCategory == "ChemicalPack"
                                                select new SysSize
                                                {
                                                    SizeID = ss.SizeID,
                                                    SizeName = ss.SizeName
                                                }).ToList();

            return iLstSysSize;
        }

        public ValidationMsg Delete(string sizeId, int userid)
        {
            var sizeid = string.IsNullOrEmpty(sizeId) ? 0 : Convert.ToInt32(sizeId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysSize = _context.Sys_Size.FirstOrDefault(s => s.SizeID == sizeid);
                _context.Sys_Size.Remove(sysSize);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }

    }
}
