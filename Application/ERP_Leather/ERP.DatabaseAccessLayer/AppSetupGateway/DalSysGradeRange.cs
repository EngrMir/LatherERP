using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysGradeRange
    {
        private readonly BLC_DEVEntities _context;

        public DalSysGradeRange()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Create(SysGradeRange objSysGradeRange)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysGradeRange = new Sys_GradeRange
                {
                    GradeRangeID = objSysGradeRange.GradeRangeID,
                    GradeRangeName = objSysGradeRange.GradeRangeName,
                    Description = objSysGradeRange.Description,
                    IsActive = objSysGradeRange.IsActive,
                    IsDelete = objSysGradeRange.IsDelete,
                    SetOn = DateTime.Now,
                    SetBy = 25,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = 25,
                    IPAddress = string.Empty
                };

                _context.Sys_GradeRange.Add(tblSysGradeRange);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Save Successfully.";

            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to update.";
            }

            return vmMsg;
        }

        public ValidationMsg Update(SysGradeRange objSysGradeRange)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysGradeRange = _context.Sys_GradeRange.FirstOrDefault(s => s.GradeRangeID == objSysGradeRange.GradeRangeID);
                if (tblSysGradeRange != null)
                {
                    tblSysGradeRange.GradeRangeName = objSysGradeRange.GradeRangeName;
                    tblSysGradeRange.Description = objSysGradeRange.Description;
                    tblSysGradeRange.IsActive = Convert.ToBoolean(objSysGradeRange.IsActive);
                    tblSysGradeRange.IsDelete = Convert.ToBoolean(objSysGradeRange.IsDelete);
                    tblSysGradeRange.ModifiedOn = DateTime.Now;
                    tblSysGradeRange.ModifiedBy = 27;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to update.";
            }
            return vmMsg;
        }

        public IEnumerable<SysGradeRange> GetAll()
        {
            IEnumerable<SysGradeRange> iLstSysGradeRange = from sgr in _context.Sys_GradeRange
                                                           where !sgr.IsDelete
                                                           select new SysGradeRange
                                                             {
                                                                 GradeRangeID = sgr.GradeRangeID,
                                                                 GradeRangeName = sgr.GradeRangeName,
                                                                 Description = sgr.Description,
                                                                 IsActive = sgr.IsActive,
                                                                 IsDelete = sgr.IsDelete,
                                                             };

            return iLstSysGradeRange;
        }

        public ValidationMsg Delete(SysGradeRange objSysGradeRange)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysGradeRange = _context.Sys_GradeRange.FirstOrDefault(s => s.GradeRangeID == objSysGradeRange.GradeRangeID);
                if (tblSysGradeRange != null)
                {
                    tblSysGradeRange.GradeRangeName = objSysGradeRange.GradeRangeName;
                    tblSysGradeRange.Description = objSysGradeRange.Description;
                    tblSysGradeRange.IsActive = Convert.ToBoolean(objSysGradeRange.IsActive);
                    tblSysGradeRange.IsDelete = true;
                    tblSysGradeRange.ModifiedOn = DateTime.Now;
                    tblSysGradeRange.ModifiedBy = 27;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to update.";
            }
            return vmMsg;
        }

    }
}
