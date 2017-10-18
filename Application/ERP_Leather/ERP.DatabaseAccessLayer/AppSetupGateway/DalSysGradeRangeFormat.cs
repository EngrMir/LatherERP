using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysGradeRangeFormat
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public DalSysGradeRangeFormat()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        //public ValidationMsg Create(SysGradeRangeFormat objGradeRangeFormat)
        //{
        //    var vmMsg = new ValidationMsg();
        //    try
        //    {
        //        var tblSysGradeRange = new Sys_GradeRangeFormat
        //        {
        //            GradeRangeID = objGradeRangeFormat.GradeRangeID,
        //            GradeIDTo = objGradeRangeFormat.GradeIDTo,
        //            GradeIDFrom = objGradeRangeFormat.GradeIDFrom,
        //            Description = objGradeRangeFormat.Description,
        //            IsActive = objGradeRangeFormat.IsActive,
        //            IsDelete = false,
        //            SetOn = DateTime.Now,
        //            SetBy = Convert.ToInt32(25),
        //            ModifiedOn = DateTime.Now,
        //            ModifiedBy = Convert.ToInt32(25),
        //            IPAddress = string.Empty
        //        };

        //        _context.Sys_GradeRangeFormat.Add(tblSysGradeRange);
        //        _context.SaveChanges();

        //        vmMsg.Type = Enums.MessageType.Success;
        //        vmMsg.Msg = "Save Successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        vmMsg.Type = Enums.MessageType.Error;
        //        vmMsg.Msg = "Failed to save.";
        //    }

        //    return vmMsg;
        //}

        public ValidationMsg Update(SysGradeRangeFormat objGradeRangeFormat, int usrId)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var gradeRangeFormat = _context.Sys_GradeRangeFormat.FirstOrDefault(s => s.FormatID == objGradeRangeFormat.FormatID);

                if (gradeRangeFormat != null)
                {
                    gradeRangeFormat.FormatID = objGradeRangeFormat.FormatID;
                    gradeRangeFormat.GradeRangeID = objGradeRangeFormat.GradeRangeID;
                    gradeRangeFormat.GradeIDTo = objGradeRangeFormat.GradeIDTo;
                    gradeRangeFormat.GradeIDFrom = objGradeRangeFormat.GradeIDFrom;
                    gradeRangeFormat.Description = objGradeRangeFormat.Description;
                    gradeRangeFormat.IsActive = Convert.ToBoolean(objGradeRangeFormat.IsActive);

                    gradeRangeFormat.ModifiedOn = DateTime.Now;
                    gradeRangeFormat.ModifiedBy = usrId;

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

        public IEnumerable<SysGradeRangeFormat> GetAll()
        {
            IEnumerable<SysGradeRangeFormat> iLstSysCurrency = from sgrf in _context.Sys_GradeRangeFormat
                                                               where !sgrf.IsDelete
                                                               select new SysGradeRangeFormat
                                                               {
                                                                   FormatID = sgrf.FormatID,
                                                                   GradeRangeID = sgrf.GradeRangeID,
                                                                   GradeIDFrom = sgrf.GradeIDFrom,
                                                                   GradeIDTo = sgrf.GradeIDTo,
                                                                   Description = sgrf.Description,
                                                                   IsActive = sgrf.IsActive,
                                                                   IsDelete = sgrf.IsDelete,
                                                               };

            return iLstSysCurrency;
        }

        public List<SysGrade> GetGradeList()
        {
            List<SysGrade> iLstGradeListType = (from sg in _context.Sys_Grade
                                                where !sg.IsDelete
                                                select new SysGrade
                                                {
                                                    GradeID = sg.GradeID,
                                                    GradeName = sg.GradeName,
                                                }).ToList();
            iLstGradeListType.Insert(0, new SysGrade
            {
                GradeName = "---Select---"
            });

            return iLstGradeListType;
        }

        public List<SysGradeRange> GetGradeRangeList()
        {
            List<SysGradeRange> iLstGradeListType = (from gr in _context.Sys_GradeRange
                                                     where !gr.IsDelete
                                                     select new SysGradeRange
                                                     {
                                                         GradeRangeID = gr.GradeRangeID,
                                                         GradeRangeName = gr.GradeRangeName,
                                                     }).ToList();

            iLstGradeListType.Insert(0, new SysGradeRange
            {
                GradeRangeName = "---Select---"
            });

            return iLstGradeListType;
        }

        public ValidationMsg Delete(SysGradeRangeFormat objGradeRangeFormat, int usrId)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var gradeRangeFormat = _context.Sys_GradeRangeFormat.FirstOrDefault(s => s.FormatID == objGradeRangeFormat.FormatID);

                if (gradeRangeFormat != null)
                {
                    gradeRangeFormat.FormatID = objGradeRangeFormat.FormatID;
                    gradeRangeFormat.GradeRangeID = objGradeRangeFormat.GradeRangeID;
                    gradeRangeFormat.GradeIDTo = objGradeRangeFormat.GradeIDTo;
                    gradeRangeFormat.GradeIDFrom = objGradeRangeFormat.GradeIDFrom;
                    gradeRangeFormat.Description = objGradeRangeFormat.Description;
                    gradeRangeFormat.IsActive = Convert.ToBoolean(objGradeRangeFormat.IsActive);
                    gradeRangeFormat.IsDelete = true;

                    gradeRangeFormat.ModifiedOn = DateTime.Now;
                    gradeRangeFormat.ModifiedBy = usrId;

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

        public List<Sys_GradeRange> GetSysGradeRangeList()
        {
            return _context.Sys_GradeRange.ToList();
        }

        public ValidationMsg SaveGradeRange(GradeRangeTitle model, int usrId)
        {
            try
            {
                if (model.GradeRangeId == 0)
                {
                    Sys_GradeRange obGradeRange = new Sys_GradeRange();
                    obGradeRange.GradeRangeName = model.GradeRangeName;
                    obGradeRange.Description = model.GradeDescription;
                    obGradeRange.IsActive = Convert.ToBoolean(model.IsActiveGradeRange);
                    obGradeRange.IsDelete = false;
                    obGradeRange.SetOn = DateTime.Now;
                    obGradeRange.SetBy = usrId;
                    obGradeRange.ModifiedOn = DateTime.Now;
                    obGradeRange.ModifiedBy = usrId;
                    _context.Sys_GradeRange.Add(obGradeRange);

                    int id = obGradeRange.GradeRangeID;
                    foreach (var item in model.gradeData)
                    {
                        Sys_GradeRangeFormat obRangeFormat = new Sys_GradeRangeFormat();
                        obRangeFormat.GradeRangeID = Convert.ToInt16(id);
                        obRangeFormat.GradeIDFrom = Convert.ToInt16(item.GradeIDFrom);
                        obRangeFormat.GradeIDTo = Convert.ToInt16(item.GradeIDTo);
                        obRangeFormat.Description = item.Description;
                        obRangeFormat.IsActive = Convert.ToBoolean(item.IsActive);
                        obRangeFormat.IsDelete = false;
                        obRangeFormat.SetOn = DateTime.Now;
                        obRangeFormat.ModifiedOn = DateTime.Now;
                        obRangeFormat.ModifiedBy = usrId;
                        _context.Sys_GradeRangeFormat.Add(obRangeFormat);
                    }
                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                }

                else
                {
                    Sys_GradeRange isGradeRangeExist = (from temp in _context.Sys_GradeRange where temp.GradeRangeID == model.GradeRangeId select temp).FirstOrDefault();
                    isGradeRangeExist.GradeRangeName = model.GradeRangeName;
                    isGradeRangeExist.Description = model.GradeDescription;
                    isGradeRangeExist.IsActive = model.IsActiveGradeRange;
                    isGradeRangeExist.IsDelete = false;
                    isGradeRangeExist.ModifiedOn = DateTime.Now;
                    isGradeRangeExist.ModifiedBy = usrId;
                    _context.SaveChanges();

                    var result = (from temp in _context.Sys_GradeRangeFormat where temp.GradeRangeID == isGradeRangeExist.GradeRangeID select temp).ToList();
                    foreach (var item in result)
                    {
                        _context.Sys_GradeRangeFormat.Remove(item);
                    }


                    foreach (var item in model.gradeData)
                    {
                        Sys_GradeRangeFormat obRangeFormat = new Sys_GradeRangeFormat();
                        obRangeFormat.GradeRangeID = Convert.ToInt16(isGradeRangeExist.GradeRangeID);
                        obRangeFormat.GradeIDFrom = Convert.ToInt16(item.GradeIDFrom);
                        obRangeFormat.GradeIDTo = Convert.ToInt16(item.GradeIDTo);
                        obRangeFormat.Description = item.Description;
                        obRangeFormat.IsActive = item.IsActive;
                        obRangeFormat.IsDelete = false;
                        obRangeFormat.SetOn = DateTime.Now;
                        obRangeFormat.SetBy = usrId;
                        obRangeFormat.ModifiedOn = DateTime.Now;
                        obRangeFormat.ModifiedBy = usrId;
                        _context.Sys_GradeRangeFormat.Add(obRangeFormat);
                    }
                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Updated Successfully.";
                }
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Save Faild.";
               
            }

           

            return _vmMsg;
        }

        public List<SysGradeRange> GetGradeformation()
        {
            var result = _context.Sys_GradeRange.Where(ob => ob.IsDelete == false).ToList();
            List<SysGradeRange> lst = new List<SysGradeRange>();
            foreach (var item in result)
            {
                SysGradeRange ob = new SysGradeRange();
                ob.GradeRangeID = item.GradeRangeID;
                ob.GradeRangeName = item.GradeRangeName;
                ob.Description = item.Description;
                lst.Add(ob);
            }
            return lst;
        }

        public object GetDetailGradeRangeInformation(string GradeRange)
        {
            int GradeRangeId = Convert.ToInt16(GradeRange);

            //var result = from temp in _context.Sys_GradeRange
            //             join temp2 in _context.Sys_GradeRangeFormat on temp.GradeRangeID equals temp2.GradeRangeID
            //             where temp.GradeRangeID == GradeRangeId && temp.IsDelete == false && temp2.IsDelete==false
            //             select new
            //             {
            //                 FormatID = temp2.FormatID,
            //                 GradeRangeID = temp.GradeRangeID,
            //                 GradeRangeName = temp.GradeRangeName,
            //                 GradeDescription = temp.Description,
            //                 IsActiveGradeRange = temp.IsActive,
            //                 GradeIDFrom =  temp2.GradeIDFrom,
            //                 GradeIDTo = temp2.GradeIDTo,

            //                 GradeNameFrom = _context.Sys_Grade.Where(m => m.GradeID == temp2.GradeIDFrom).FirstOrDefault().GradeName,
            //                 GradeNameTo = _context.Sys_Grade.Where(m => m.GradeID == temp2.GradeIDTo).FirstOrDefault().GradeName,
            //                 Description = temp2.Description,
            //                 IsActive = temp2.IsActive
            //             };

            var queryString = @"select ISNULL(f.FormatID,0) as FormatID ,s.GradeRangeID,s.GradeRangeName,s.[Description] as GradeDescription,
                                s.IsActive as IsActiveGradeRange,
                                ISNULL(f.GradeIDFrom,0) as GradeIDFrom,ISNULL(f.GradeIDTo,0) as GradeIDTo,g.GradeName as GradeNameFrom, 
                                g2.GradeName as GradeNameTo, f.[Description],ISNULL(f.IsActive,0) as IsActive
                                from Sys_GradeRange s 
                                LEFT JOIN Sys_GradeRangeFormat f ON s.GradeRangeID = f.GradeRangeID AND s.IsDelete = 'false' AND f.IsDelete='false'
                                LEFT JOIN Sys_Grade g ON f.GradeIDFrom = g.GradeID AND g.IsDelete ='false'
                                LEFT JOIN Sys_Grade g2 ON f.GradeIDTo = g2.GradeID AND g2.IsDelete ='false'
                                WHERE s.GradeRangeID ='" + GradeRangeId + "'";

            var data = _context.Database.SqlQuery<SysGradeRangeFormat>(queryString);
            return data.ToList();

        }
        public ValidationMsg DeleteGradeRangeInfo(string GradeRange, int userId)
        {
            try
            {
                int GradeRangeId = Convert.ToInt16(GradeRange);
                var result = (from temp in _context.Sys_GradeRangeFormat where temp.GradeRangeID == GradeRangeId select temp).ToList();
                foreach (var item in result)
                {
                    Sys_GradeRangeFormat objRangeFormat = _context.Sys_GradeRangeFormat.Where(ob =>ob.FormatID == item.FormatID).FirstOrDefault();
                    objRangeFormat.IsDelete = true;
                    objRangeFormat.ModifiedBy = userId;
                }

                Sys_GradeRange objGradRange = _context.Sys_GradeRange.Where(ob =>ob.GradeRangeID == GradeRangeId).FirstOrDefault();
                objGradRange.IsDelete = true;
                objGradRange.ModifiedBy = userId;
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Delete Faild.";
                
            }
            return _vmMsg;

        }
        public ValidationMsg DeleteGradeRangeFormatById(string GradeRange,int userId)
        {
            try
            {
                int FormatId = Convert.ToInt16(GradeRange);
                Sys_GradeRangeFormat objRangeFormat = (from temp in _context.Sys_GradeRangeFormat where temp.FormatID == FormatId select temp).FirstOrDefault();               
                objRangeFormat.IsDelete = true;
                objRangeFormat.ModifiedBy = userId;
                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Delete Faild.";
                return _vmMsg;
            }
            return _vmMsg;

        }
        

    }
}
