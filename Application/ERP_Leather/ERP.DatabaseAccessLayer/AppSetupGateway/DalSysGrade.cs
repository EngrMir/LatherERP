using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysGrade
    {
        private readonly BLC_DEVEntities _context;
        public long GradeID = 0;

        public DalSysGrade()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysGrade objSysGrade, int userID)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysGrade = new Sys_Grade
                {
                    GradeName = objSysGrade.GradeName,
                    Description = objSysGrade.Description,
                    IsActive = objSysGrade.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = userID,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userID,
                    IPAddress = string.Empty
                };

                _context.Sys_Grade.Add(tblSysGrade);
                _context.SaveChanges();
                GradeID = tblSysGrade.GradeID;

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Save Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to save.";
            }

            return vmMsg;
        }

        public ValidationMsg Update(SysGrade objSysGrade, int userID)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var grade = _context.Sys_Grade.FirstOrDefault(s => s.GradeID == objSysGrade.GradeID);

                if (grade != null)
                {
                    grade.GradeName = objSysGrade.GradeName;
                    grade.Description = objSysGrade.Description;
                    grade.IsActive = objSysGrade.IsActive == "Active";
                    
                    grade.ModifiedOn = DateTime.Now;
                    grade.ModifiedBy = userID;

                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to update.";
            }

            return vmMsg;
        }

        public IEnumerable<SysGrade> GetAll()
        {
            IEnumerable<SysGrade> iLstSysGrade = from sg in _context.Sys_Grade
                                                 where !sg.IsDelete
                                                 select new SysGrade
                                                    {
                                                        GradeID = sg.GradeID,
                                                        GradeName = sg.GradeName,
                                                        Description = sg.Description,
                                                        IsActive = sg.IsActive ? "Active" : "Inactive",
                                                        IsDelete = sg.IsDelete,
                                                    };

            return iLstSysGrade;
        }

        public IEnumerable<SysGrade> GetAllActiveGrade()
        {
            IEnumerable<SysGrade> listSysGrade = from ss in _context.Sys_Grade
                                                 where ss.IsActive == true
                                                 select new SysGrade
                                                 {
                                                     GradeID = ss.GradeID,
                                                     GradeName = ss.GradeName,
                                                     Description = ss.Description
                                                 };

            return listSysGrade;
        }
        public ValidationMsg Delete(string GradeID, int userID)
        {
            var vmMsg = new ValidationMsg();
            try
            {

                var grade = (from g in _context.Sys_Grade.AsEnumerable()
                             where g.GradeID == Convert.ToInt16(GradeID)
                                select g).FirstOrDefault();

                if (grade != null)
                {
                    grade.IsDelete = true;
                    grade.ModifiedOn = DateTime.Now;
                    grade.ModifiedBy = userID;
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

        public long GetGradeID()
        {
            return GradeID;
        }
    }
}
