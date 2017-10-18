using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalEXPPackingList
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        private readonly string _connString = string.Empty;
        public long PLID = 0;
        public long PLPIID = 0;
        public long PLPIItemColorID = 0;
        public string PLNo = string.Empty;

        public DalEXPPackingList()
        {
            _context = new BLC_DEVEntities();
            _connString = StrConnection.GetConnectionString();
        }

        public ValidationMsg Save(EXPPackingList model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Save

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.PLNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.PLNo != null)
                        {
                            #region CI

                            EXP_PackingList tblEXPPackingList = SetToModelObject(model, userid);
                            _context.EXP_PackingList.Add(tblEXPPackingList);
                            _context.SaveChanges();

                            #endregion

                            #region CIPI

                            model.EXPPLPIList[0].PLID = tblEXPPackingList.PLID;
                            EXP_PLPI tblEXPPLPI = SetToModelObject(model.EXPPLPIList[0], userid);
                            _context.EXP_PLPI.Add(tblEXPPLPI);
                            _context.SaveChanges();

                            #endregion

                            #region CIPIItem

                            if (model.EXPPLPIItemColorList != null)
                            {
                                foreach (EXPPLPIItemColor objEXPPLPIItemColor in model.EXPPLPIItemColorList)
                                {
                                    objEXPPLPIItemColor.PLPIID = tblEXPPLPI.PLPIID;

                                    EXP_PLPIItemColor tblEXPCIPIItem = SetToModelObject(objEXPPLPIItemColor, userid);
                                    _context.EXP_PLPIItemColor.Add(tblEXPCIPIItem);
                                    _context.SaveChanges();

                                    #region CIPIItemColor

                                    if (model.EXPPLPIItemColorBaleList != null)
                                    {
                                        if ((objEXPPLPIItemColor.ArticleID == model.EXPPLPIItemColorBaleList[0].ArticleID) && (objEXPPLPIItemColor.ColorID == model.EXPPLPIItemColorBaleList[0].ColorID))
                                        {
                                            foreach (EXPPLPIItemColorBale objEXPCIPIItemColor in model.EXPPLPIItemColorBaleList)
                                            {
                                                objEXPCIPIItemColor.PLPIItemColorID = tblEXPCIPIItem.PLPIItemColorID;
                                                EXP_PLPIItemColorBale tblEXPCIPIItemColor = SetToModelObject(objEXPCIPIItemColor, userid);
                                                _context.EXP_PLPIItemColorBale.Add(tblEXPCIPIItemColor);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();

                            PLID = tblEXPPackingList.PLID;
                            PLNo = model.PLNo;
                            PLPIID = tblEXPPLPI.PLPIID;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "PLNo Predefine Value not Found.";
                        }
                    }
                }

                //#region CIPIItemColor

                //if (model.EXPPLPIItemColorBaleList != null)
                //{
                //    //if ((objEXPPLPIItemColor.ArticleID == model.EXPPLPIItemColorBaleList[0].ArticleID) && (objEXPPLPIItemColor.ColorID == model.EXPPLPIItemColorBaleList[0].ColorID))
                //    //{
                //    foreach (EXPPLPIItemColorBale objEXPCIPIItemColor in model.EXPPLPIItemColorBaleList)
                //    {
                //        objEXPCIPIItemColor.PLPIItemColorID = 5;// tblEXPCIPIItem.PLPIItemColorID;
                //        objEXPCIPIItemColor.Remarks = "";// tblEXPCIPIItem.PLPIItemColorID;
                //        if (string.IsNullOrEmpty(objEXPCIPIItemColor.GradeName))
                //            objEXPCIPIItemColor.GradeID = null;
                //        else
                //            objEXPCIPIItemColor.GradeID = 5;// _context.Sys_Grade.Where(m => m.GradeName == objEXPCIPIItemColor.GradeName).FirstOrDefault().GradeID;

                //        if (string.IsNullOrEmpty(objEXPCIPIItemColor.GrossBaleWeightUnitName))
                //        {
                //            objEXPCIPIItemColor.NetWeightUnit = null;
                //            objEXPCIPIItemColor.GrossBaleWeightUnit = null;
                //        }
                //        else
                //        {
                //            objEXPCIPIItemColor.NetWeightUnit = 5;// _context.Sys_Unit.Where(m => m.UnitName == objEXPCIPIItemColor.GrossBaleWeightUnitName).FirstOrDefault().UnitID;
                //            objEXPCIPIItemColor.GrossBaleWeightUnit = 5;// objEXPCIPIItemColor.NetWeightUnit;// _context.Sys_Unit.Where(m => m.UnitName == objEXPCIPIItemColor.GrossBaleWeightUnitName).FirstOrDefault().UnitID;
                //        }

                //        SqlConnection con = new SqlConnection(_connString);
                //        //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ToString());
                //        con.Open();
                //        string query = "UspInsertPLColorBale";         //Stored Procedure name 
                //        //cmd.Connection = conn;
                //        //cmd.CommandText = "UspConfirmWetBlueIssue";
                //        SqlCommand com = new SqlCommand(query, con);  //creating  SqlCommand  object
                //        com.CommandType = CommandType.StoredProcedure;  //here we declaring command type as stored Procedure

                //        com.Parameters.AddWithValue("@PLPIItemColorBaleNo", objEXPCIPIItemColor.PLPIItemColorBaleNo);
                //        com.Parameters.AddWithValue("@PLPIItemColorID", objEXPCIPIItemColor.PLPIItemColorID);
                //        com.Parameters.AddWithValue("@GradeID", objEXPCIPIItemColor.GradeID);
                //        com.Parameters.AddWithValue("@PcsInBale", objEXPCIPIItemColor.PcsInBale);
                //        com.Parameters.AddWithValue("@SideInBale", objEXPCIPIItemColor.SideInBale);
                //        com.Parameters.AddWithValue("@FootPLPIBaleQty", objEXPCIPIItemColor.FootPLPIBaleQty);
                //        com.Parameters.AddWithValue("@MeterPLPIBaleQty", objEXPCIPIItemColor.MeterPLPIBaleQty);
                //        com.Parameters.AddWithValue("@PLPIBaleNetWeight", objEXPCIPIItemColor.PLPIBaleNetWeight);
                //        com.Parameters.AddWithValue("@PLPIBGrossaleWeight", objEXPCIPIItemColor.PLPIBGrossaleWeight);
                //        com.Parameters.AddWithValue("@NetWeightUnit", objEXPCIPIItemColor.NetWeightUnit);
                //        com.Parameters.AddWithValue("@GrossBaleWeightUnit", objEXPCIPIItemColor.GrossBaleWeightUnit);
                //        com.Parameters.AddWithValue("@Remarks", objEXPCIPIItemColor.Remarks);

                //        com.ExecuteNonQuery();

                //        //EXP_PLPIItemColorBale tblEXPCIPIItemColor = SetToModelObject(objEXPCIPIItemColor, userid);
                //        //_context.EXP_PLPIItemColorBale.Add(tblEXPCIPIItemColor);
                //    }
                //    //}
                //}
                //#endregion

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Packinglist No Already Exit.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(EXPPackingList model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region CI

                        EXP_PackingList CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.EXP_PackingList.First(m => m.PLID == model.PLID);

                        //OriginalEntity.PLNo = CurrentEntity.PLNo;
                        OriginalEntity.PLDate = CurrentEntity.PLDate;
                        OriginalEntity.CIID = CurrentEntity.CIID;
                        OriginalEntity.BalesNo = CurrentEntity.BalesNo;
                        OriginalEntity.BaleQty = CurrentEntity.BaleQty;
                        OriginalEntity.TotalPcs = CurrentEntity.TotalPcs;
                        OriginalEntity.TotalSide = CurrentEntity.TotalSide;
                        OriginalEntity.MeterCIQty = CurrentEntity.MeterCIQty;
                        OriginalEntity.FootCIQty = CurrentEntity.FootCIQty;
                        OriginalEntity.PLNetWeight = CurrentEntity.PLNetWeight;
                        OriginalEntity.NetWeightUnit = CurrentEntity.NetWeightUnit;
                        OriginalEntity.PLGrossWeight = CurrentEntity.PLGrossWeight;
                        OriginalEntity.GrossWeightUnit = CurrentEntity.GrossWeightUnit;
                        OriginalEntity.PLMarks = CurrentEntity.PLMarks;
                        OriginalEntity.PLNote = CurrentEntity.PLNote;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion

                        #region CIPI

                        //if (string.IsNullOrEmpty(model.PLNo))
                        if (model.EXPPLPIList[0].PLPIID == 0)
                        {
                            model.EXPPLPIList[0].PLID = model.PLID;
                            EXP_PLPI tblEXPPLPI = SetToModelObject(model.EXPPLPIList[0], userid);
                            _context.EXP_PLPI.Add(tblEXPPLPI);
                            _context.SaveChanges();
                            PLPIID = tblEXPPLPI.PLPIID;
                        }
                        else
                        {
                            PLPIID = model.EXPPLPIList[0].PLPIID;
                            EXP_PLPI CurrentCIPIEntity = SetToModelObject(model.EXPPLPIList[0], userid);
                            var OriginalCIPIEntity = _context.EXP_PLPI.First(m => m.PLPIID == PLPIID);

                            //OriginalCIPIEntity.PLID = CurrentCIPIEntity.PLID;
                            OriginalCIPIEntity.PIID = CurrentCIPIEntity.PIID;
                            OriginalCIPIEntity.LCID = CurrentCIPIEntity.LCID;
                            OriginalCIPIEntity.BuyerOrderID = CurrentCIPIEntity.BuyerOrderID;
                            OriginalCIPIEntity.PLPIPcs = CurrentCIPIEntity.PLPIPcs;
                            OriginalCIPIEntity.PLPISide = CurrentCIPIEntity.PLPISide;
                            OriginalCIPIEntity.FootPLPIQty = CurrentCIPIEntity.FootPLPIQty;
                            OriginalCIPIEntity.MeterPLPIQty = CurrentCIPIEntity.MeterPLPIQty;
                            OriginalCIPIEntity.NetWeightUnit = CurrentCIPIEntity.NetWeightUnit;
                            OriginalCIPIEntity.GrossWeightUnit = CurrentCIPIEntity.GrossWeightUnit;
                            OriginalCIPIEntity.PLPINetWeight = CurrentCIPIEntity.PLPINetWeight;
                            OriginalCIPIEntity.PLPIGrossWeight = CurrentCIPIEntity.PLPIGrossWeight;
                            OriginalCIPIEntity.ModifiedBy = userid;
                            OriginalCIPIEntity.ModifiedOn = DateTime.Now;
                        }

                        #endregion

                        #region CIPIItem

                        if (model.EXPPLPIItemColorList != null)
                        {
                            foreach (EXPPLPIItemColor objEXPPLPIItemColor in model.EXPPLPIItemColorList)
                            {
                                if (objEXPPLPIItemColor.PLPIItemColorID == 0)
                                {
                                    objEXPPLPIItemColor.PLPIID = PLPIID;
                                    EXP_PLPIItemColor tblYearMonthScheduleDate = SetToModelObject(objEXPPLPIItemColor, userid);
                                    _context.EXP_PLPIItemColor.Add(tblYearMonthScheduleDate);
                                    _context.SaveChanges();
                                    PLPIItemColorID = tblYearMonthScheduleDate.PLPIItemColorID;
                                }
                                else
                                {
                                    PLPIItemColorID = objEXPPLPIItemColor.PLPIItemColorID;
                                    EXP_PLPIItemColor CurrEntity = SetToModelObject(objEXPPLPIItemColor, userid);
                                    var OrgrEntity = _context.EXP_PLPIItemColor.First(m => m.PLPIItemColorID == objEXPPLPIItemColor.PLPIItemColorID);

                                    ////OrgrEntity.PLPIID = CurrEntity.PLPIID;
                                    //OrgrEntity.Commodity = CurrEntity.Commodity;
                                    //OrgrEntity.ArticleID = CurrEntity.ArticleID;
                                    //OrgrEntity.ArticleNo = CurrEntity.ArticleNo;
                                    //OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                    //OrgrEntity.LeatherTypeID = CurrEntity.LeatherTypeID;
                                    //OrgrEntity.LeatherStatusID = CurrEntity.LeatherStatusID;
                                    //OrgrEntity.MaterialNo = CurrEntity.MaterialNo;
                                    //OrgrEntity.AvgSize = CurrEntity.AvgSize;
                                    //OrgrEntity.SideDescription = CurrEntity.SideDescription;
                                    //OrgrEntity.SelectionRange = CurrEntity.SelectionRange;
                                    //OrgrEntity.Thickness = CurrEntity.Thickness;
                                    //OrgrEntity.ThicknessAt = CurrEntity.ThicknessAt;
                                    //OrgrEntity.MeterCIQty = CurrEntity.MeterCIQty;
                                    //OrgrEntity.MeterUnitPrice = CurrEntity.MeterUnitPrice;
                                    //OrgrEntity.MeterTotalPrice = CurrEntity.MeterTotalPrice;
                                    //OrgrEntity.FootCIQty = CurrEntity.FootCIQty;
                                    //OrgrEntity.FootUnitPrice = CurrEntity.FootUnitPrice;
                                    //OrgrEntity.FootTotalPrice = CurrEntity.FootTotalPrice;
                                    //OrgrEntity.PackQty = CurrEntity.PackQty;
                                    //OrgrEntity.Remarks = CurrEntity.Remarks;
                                    OrgrEntity.ModifiedBy = userid;
                                    OrgrEntity.ModifiedOn = DateTime.Now;
                                }
                            }
                        }

                        #endregion

                        #region CIPIItemColor

                        if (model.EXPPLPIItemColorBaleList != null)
                        {
                            //if ((objEXPPLPIItemColor.ArticleID == model.EXPPLPIItemColorBaleList[0].ArticleID) && (objEXPPLPIItemColor.ColorID == model.EXPPLPIItemColorBaleList[0].ColorID))
                            //{
                            foreach (EXPPLPIItemColorBale objEXPCIPIItemColor in model.EXPPLPIItemColorBaleList)
                            {
                                if (objEXPCIPIItemColor.PLPIItemColorBaleID == 0)
                                {
                                    objEXPCIPIItemColor.PLPIItemColorID = PLPIItemColorID;
                                    //objEXPCIPIItemColor.MaterialNo = objEXPPLPIItemColor.MaterialNo;

                                    //objEXPCIPIItemColor.AvgSize = objEXPPLPIItemColor.AvgSize;
                                    //if (string.IsNullOrEmpty(objEXPPLPIItemColor.AvgSizeUnitName))
                                    //    objEXPCIPIItemColor.AvgSizeUnit = null;
                                    //else
                                    //    objEXPCIPIItemColor.AvgSizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == objEXPPLPIItemColor.AvgSizeUnitName).FirstOrDefault().UnitID);
                                    //objEXPCIPIItemColor.SideDescription = objEXPPLPIItemColor.SideDescription;
                                    //objEXPCIPIItemColor.SelectionRange = objEXPPLPIItemColor.SelectionRange;
                                    //objEXPCIPIItemColor.Thickness = objEXPPLPIItemColor.Thickness;
                                    //if (string.IsNullOrEmpty(objEXPPLPIItemColor.ThicknessUnitName))
                                    //    objEXPCIPIItemColor.ThicknessUnit = null;
                                    //else
                                    //    objEXPCIPIItemColor.ThicknessUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == objEXPPLPIItemColor.ThicknessUnitName).FirstOrDefault().UnitID);
                                    //objEXPCIPIItemColor.ThicknessAt = objEXPPLPIItemColor.ThicknessAt;

                                    EXP_PLPIItemColorBale tblEXPCIPIItemColor = SetToModelObject(objEXPCIPIItemColor, userid);
                                    _context.EXP_PLPIItemColorBale.Add(tblEXPCIPIItemColor);
                                }
                                else
                                {
                                    EXP_PLPIItemColorBale CurrEntity = SetToModelObject(objEXPCIPIItemColor, userid);
                                    var OrgrEntity = _context.EXP_PLPIItemColorBale.First(m => m.PLPIItemColorBaleID == objEXPCIPIItemColor.PLPIItemColorBaleID);

                                    OrgrEntity.PLPIItemColorBaleNo = CurrEntity.PLPIItemColorBaleNo;
                                    OrgrEntity.GradeID = CurrEntity.GradeID;
                                    OrgrEntity.PcsInBale = CurrEntity.PcsInBale;
                                    OrgrEntity.SideInBale = CurrEntity.SideInBale;
                                    OrgrEntity.FootPLPIBaleQty = CurrEntity.FootPLPIBaleQty;
                                    OrgrEntity.MeterPLPIBaleQty = CurrEntity.MeterPLPIBaleQty;
                                    OrgrEntity.NetWeightUnit = CurrEntity.NetWeightUnit;
                                    OrgrEntity.GrossBaleWeightUnit = CurrEntity.GrossBaleWeightUnit;
                                    OrgrEntity.PLPIBaleNetWeight = CurrEntity.PLPIBaleNetWeight;
                                    OrgrEntity.PLPIBGrossaleWeight = CurrEntity.PLPIBGrossaleWeight;
                                }
                            }
                            //}
                        }
                        #endregion

                        _context.SaveChanges();
                        tx.Complete();
                        PLID = model.PLID;
                        PLNo = model.PLNo;
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";

                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Packinglist No Already Exit.";
                }
            }
            return _vmMsg;
        }

        public long GetPLID()
        {
            return PLID;
        }

        public string GetPLNo()
        {
            return PLNo;
        }

        public long GetPLPIID()
        {
            return PLPIID;
        }

        public EXP_PackingList SetToModelObject(EXPPackingList model, int userid)
        {
            EXP_PackingList Entity = new EXP_PackingList();

            Entity.PLID = model.PLID;
            Entity.PLNo = model.PLNo;
            Entity.CIID = model.CIID;
            Entity.PLDate = DalCommon.SetDate(model.PLDate);
            Entity.BalesNo = model.BalesNo;
            Entity.BaleQty = model.BaleQty;
            Entity.TotalPcs = model.TotalPcs;
            Entity.TotalSide = model.TotalSide;
            Entity.MeterCIQty = model.MeterCIQty;
            Entity.FootCIQty = model.FootCIQty;
            Entity.PLNetWeight = model.PLNetWeight;
            Entity.NetWeightUnit = model.NetWeightUnit;
            Entity.PLGrossWeight = model.PLGrossWeight;
            Entity.GrossWeightUnit = model.GrossWeightUnit;
            Entity.PLMarks = model.PLMarks;
            Entity.PLNote = model.PLNote;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_PLPI SetToModelObject(EXPPLPI model, int userid)
        {
            EXP_PLPI Entity = new EXP_PLPI();

            Entity.PLPIID = model.PLPIID;
            Entity.PLID = model.PLID;
            Entity.PIID = model.PIID;
            Entity.LCID = model.LCID;
            Entity.BuyerOrderID = model.BuyerOrderID;
            Entity.MeterPLPIQty = model.MeterPLPIQty;
            Entity.FootPLPIQty = model.FootPLPIQty;
            Entity.PLPIPcs = model.PLPIPcs;
            Entity.PLPISide = model.PLPISide;

            Entity.PLPINetWeight = model.PLPINetWeight;
            Entity.NetWeightUnit = model.PLPINetWeightUnit;
            Entity.PLPIGrossWeight = model.PLPIGrossWeight;
            Entity.GrossWeightUnit = model.PLPIGrossWeightUnit;

            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_PLPIItemColor SetToModelObject(EXPPLPIItemColor model, int userid)
        {
            EXP_PLPIItemColor Entity = new EXP_PLPIItemColor();

            Entity.PLPIItemColorID = model.PLPIItemColorID;
            Entity.PLPIID = model.PLPIID;
            Entity.Commodity = model.Commodity;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID;
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.MaterialNo = model.MaterialNo;
            Entity.AvgSize = model.AvgSize;
            Entity.AvgSizeUnit = model.AvgSizeUnit;
            //if (string.IsNullOrEmpty(model.AvgSizeUnitName))
            //    Entity.AvgSizeUnit = null;
            //else
            //    Entity.AvgSizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.AvgSizeUnitName).FirstOrDefault().UnitID);
            Entity.SideDescription = model.SideDescription;
            Entity.SelectionRange = model.SelectionRange;
            Entity.Thickness = model.Thickness;
            Entity.ThicknessUnit = model.ThicknessUnit;
            //if (string.IsNullOrEmpty(model.ThicknessUnitName))
            //    Entity.ThicknessUnit = null;
            //else
            //    Entity.ThicknessUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.ThicknessUnitName).FirstOrDefault().UnitID);
            Entity.ThicknessAt = model.ThicknessAt;
            Entity.ColorID = model.ColorID;
            Entity.MeterPLPIItemQty = model.MeterPLPIItemQty;
            Entity.FootPLPIItemQty = model.FootPLPIItemQty;
            Entity.PLPIItemWeight = model.PLPIItemWeight;
            Entity.ItemWeightUnit = model.ItemWeightUnit;
            //Entity.PackQty = model.PackQty;
            //if (string.IsNullOrEmpty(model.PackUnitName))
            //    Entity.PackUnit = null;
            //else
            //    Entity.PackUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.PackUnitName).FirstOrDefault().UnitID);
            Entity.RecordStatus = "NCF";
            Entity.Remarks = model.Remarks;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_PLPIItemColorBale SetToModelObject(EXPPLPIItemColorBale model, int userid)
        {
            EXP_PLPIItemColorBale Entity = new EXP_PLPIItemColorBale();

            Entity.PLPIItemColorBaleID = model.PLPIItemColorBaleID;
            Entity.PLPIItemColorBaleNo = model.PLPIItemColorBaleNo;
            Entity.PLPIItemColorID = model.PLPIItemColorID;
            Entity.PcsInBale = model.PcsInBale;
            Entity.SideInBale = model.SideInBale;
            Entity.MeterPLPIBaleQty = model.MeterPLPIBaleQty;
            Entity.FootPLPIBaleQty = model.FootPLPIBaleQty;
            Entity.PLPIBaleNetWeight = model.PLPIBaleNetWeight;
            Entity.PLPIBGrossaleWeight = model.PLPIBGrossaleWeight;
            if (string.IsNullOrEmpty(model.GradeName))
                Entity.GradeID = null;
            else
                Entity.GradeID = _context.Sys_Grade.Where(m => m.GradeName == model.GradeName).FirstOrDefault().GradeID;
            //if (string.IsNullOrEmpty(model.GrossBaleWeightUnitName))
            //{
            //    Entity.NetWeightUnit = null;
            //    Entity.GrossBaleWeightUnit = null;
            //}
            //else
            //{
            //Entity.NetWeightUnit = _context.Sys_Unit.Where(m => m.UnitName == model.GrossBaleWeightUnitName).FirstOrDefault().UnitID;
            //Entity.GrossBaleWeightUnit = _context.Sys_Unit.Where(m => m.UnitName == model.GrossBaleWeightUnitName).FirstOrDefault().UnitID;
            //}
            Entity.NetWeightUnit = _context.Sys_Unit.Where(m => m.UnitName == "KG").FirstOrDefault().UnitID;
            Entity.GrossBaleWeightUnit = _context.Sys_Unit.Where(m => m.UnitName == "KG").FirstOrDefault().UnitID;
            Entity.Remarks = model.Remarks;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public List<EXPPLPIItemColor> GetCIPIColorInformation(string CIPIID)
        {
            if (!string.IsNullOrEmpty(CIPIID))
            {
                var query = @"select item.CIPIItemID,item.Commodity,item.ArticleID,(select ArticleName from Sys_Article where ArticleID = item.ArticleID)ArticleName,
                            item.AvgSize,item.AvgSizeUnit,(select UnitName from Sys_Unit where UnitID = item.AvgSizeUnit)AvgSizeUnitName,
                            item.SideDescription,item.SelectionRange,item.Thickness,item.ThicknessUnit,(select UnitName from Sys_Unit where UnitID = item.ThicknessUnit)ThicknessUnitName,
                            item.ThicknessAt,color.ColorID,(select ColorName from Sys_Color where ColorID = color.ColorID)ColorName
                            from EXP_CIPIItem item
                            inner join EXP_CIPIItemColor color
                            on item.CIPIItemID = color.CIPIItemID
                            where item.CIPIID = " + CIPIID + "";
                var allData = _context.Database.SqlQuery<EXPPLPIItemColor>(query).ToList();
                return allData;
            }
            return null;
        }

        public List<EXPPLPIItemColor> GetPIColorInformation(string PIID)
        {
            if (!string.IsNullOrEmpty(PIID))
            {
                var query = @"select item.Commodity,item.ArticleID,(select ArticleName from Sys_Article where ArticleID = item.ArticleID)ArticleName,
                            item.AvgSize,item.AvgSizeUnit,(select UnitName from Sys_Unit where UnitID = item.AvgSizeUnit)AvgSizeUnitName,
                            item.SideDescription,item.SelectionRange,item.Thickness,item.ThicknessUnit,
							(select UnitName from Sys_Unit where UnitID = item.ThicknessUnit)ThicknessUnitName,
                            item.ThicknessAt,color.ColorID,(select ColorName from Sys_Color where ColorID = color.ColorID)ColorName
                            from EXP_PIItem item
                            inner join EXP_PIItemColor color
                            on item.PIItemID = color.PIItemID
                            where item.PIID = " + PIID + "";
                var allData = _context.Database.SqlQuery<EXPPLPIItemColor>(query).ToList();
                return allData;
            }
            return null;
        }

        public List<EXPPackingList> GetPackingListInformation()
        {
            List<EXP_PackingList> searchList = _context.EXP_PackingList.OrderByDescending(m => m.PLID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPPackingList>();
        }

        public EXPPackingList SetToBussinessObject(EXP_PackingList Entity)
        {
            EXPPackingList Model = new EXPPackingList();

            Model.PLID = Entity.PLID;
            Model.PLNo = Entity.PLNo;
            Model.CIID = Entity.CIID;
            Model.CINo = Entity.CIID == null ? "" : _context.EXP_CI.Where(m => m.CIID == Entity.CIID).FirstOrDefault().CIRefNo;
            Model.CIDate = Entity.CIID == null ? "" : Convert.ToDateTime(_context.EXP_CI.Where(m => m.CIID == Entity.CIID).FirstOrDefault().CIDate).ToString("dd/MM/yyyy");
            Model.PLDate = Convert.ToDateTime(Entity.PLDate).ToString("dd/MM/yyyy");
            Model.BalesNo = Entity.BalesNo;
            Model.BaleQty = Entity.BaleQty;
            Model.TotalPcs = Entity.TotalPcs;
            Model.TotalSide = Entity.TotalSide;
            Model.MeterCIQty = Entity.MeterCIQty;
            Model.FootCIQty = Entity.FootCIQty;
            Model.PLNetWeight = Entity.PLNetWeight;
            Model.NetWeightUnit = Entity.NetWeightUnit;
            Model.PLGrossWeight = Entity.PLGrossWeight;
            Model.GrossWeightUnit = Entity.GrossWeightUnit;
            Model.PLMarks = Entity.PLMarks;
            Model.PLNote = Entity.PLNote;
            Model.RecordStatus = Entity.RecordStatus;
            if (Entity.RecordStatus == "NCF")
                Model.RecordStatusName = "Not Confirmed";
            else if (Entity.RecordStatus == "CNF")
                Model.RecordStatusName = "Confirmed";
            else if (Entity.RecordStatus == "CHK")
                Model.RecordStatusName = "Checked";
            else
                Model.RecordStatusName = "";

            return Model;
        }

        public List<EXPPLPI> GetPLPIInformation(string PLID)
        {
            long pLID = Convert.ToInt64(PLID);
            List<EXP_PLPI> searchList = _context.EXP_PLPI.Where(m => m.PLID == pLID).OrderByDescending(m => m.PLPIID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPPLPI>();
        }

        public EXPPLPI SetToBussinessObject(EXP_PLPI Entity)
        {
            EXPPLPI Model = new EXPPLPI();

            Model.PLPIID = Entity.PLPIID;
            Model.PLID = Entity.PLID;

            Model.PIID = Entity.PIID;
            Model.PINo = Entity.PIID == null ? "" : _context.EXP_LeatherPI.Where(m => m.PIID == Entity.PIID).FirstOrDefault().PINo;

            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Model.BuyerOrderID).FirstOrDefault().OrderNo;

            //Model.BuyerOrderID = Entity.PIID == null ? null : _context.EXP_LeatherPI.Where(m => m.PIID == Entity.PIID).FirstOrDefault().BuyerOrderID;
            //Model.BuyerOrderNo = Entity.PIID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Model.BuyerOrderID).FirstOrDefault().BuyerOrderNo;

            Model.BuyerID = Model.BuyerOrderID == null ? null : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Model.BuyerOrderID).FirstOrDefault().BuyerID;
            Model.BuyerName = Model.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Model.BuyerID).FirstOrDefault().BuyerName;

            Model.LCID = Entity.LCID;
            Model.LCNo = Entity.LCID == null ? "" : _context.EXP_LCOpening.Where(m => m.LCID == Entity.LCID).FirstOrDefault().LCNo;


            Model.PLPIPcs = Entity.PLPIPcs;
            Model.PLPISide = Entity.PLPISide;

            Model.FootPLPIQty = Entity.FootPLPIQty;
            Model.MeterPLPIQty = Entity.MeterPLPIQty;

            Model.PLPINetWeight = Entity.PLPINetWeight;
            Model.PLPINetWeightUnit = Entity.NetWeightUnit;

            Model.PLPIGrossWeight = Entity.PLPIGrossWeight;
            Model.PLPIGrossWeightUnit = Entity.GrossWeightUnit;

            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public List<EXPPLPIItemColor> GetExpPLPIItemColorList(string PLPIID)
        {
            long? pLPIID = Convert.ToInt64(PLPIID);
            List<EXP_PLPIItemColor> searchList = _context.EXP_PLPIItemColor.Where(m => m.PLPIID == pLPIID).OrderBy(m => m.PLPIItemColorID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPPLPIItemColor>();
        }

        public EXPPLPIItemColor SetToBussinessObject(EXP_PLPIItemColor Entity)
        {
            EXPPLPIItemColor Model = new EXPPLPIItemColor();

            Model.PLPIItemColorID = Entity.PLPIItemColorID;
            Model.PLPIID = Entity.PLPIID;
            Model.MaterialNo = Entity.MaterialNo;
            Model.Commodity = Entity.Commodity;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.AvgSize = Entity.AvgSize;
            Model.AvgSizeUnit = Entity.AvgSizeUnit;
            Model.AvgSizeUnitName = Entity.AvgSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AvgSizeUnit).FirstOrDefault().UnitName;
            Model.SideDescription = Entity.SideDescription;
            Model.SelectionRange = Entity.SelectionRange;
            Model.Thickness = Entity.Thickness;
            Model.ThicknessUnit = Entity.ThicknessUnit;
            Model.ThicknessUnitName = Entity.ThicknessUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ThicknessUnit).FirstOrDefault().UnitName;
            Model.ThicknessAt = Entity.ThicknessAt;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            //Model.MeterPLPIItemQty = Entity.MeterPLPIItemQty;
            //Model.FootPLPIItemQty = Entity.FootPLPIItemQty;
            //Model.PLPIItemWeight = Entity.PLPIItemWeight;
            //Model.ItemWeightUnit = Entity.ItemWeightUnit;
            //Model.PackQty = Entity.PackQty;
            //Model.PackUnit = Entity.PackUnit;
            //Model.AvgSizeUnitName = Entity.PackUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.PackUnit).FirstOrDefault().UnitName;
            Model.Remarks = Entity.Remarks;


            return Model;
        }

        public List<EXPPLPIItemColorBale> GetExpPLPIItemColorBaleList(string PLPIItemColorID)
        {
            long pLPIItemColorID = Convert.ToInt64(PLPIItemColorID);
            //List<EXP_PLPIItemColorBale> searchList = _context.EXP_PLPIItemColorBale.Where(m => m.PLPIItemColorID == pLPIItemColorID).OrderByDescending(m => m.PLPIItemColorBaleID).ToList();
            List<EXP_PLPIItemColorBale> searchList = _context.EXP_PLPIItemColorBale.Where(m => m.PLPIItemColorID == pLPIItemColorID).OrderBy(m => m.PLPIItemColorBaleID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPPLPIItemColorBale>();
        }

        public EXPPLPIItemColorBale SetToBussinessObject(EXP_PLPIItemColorBale Entity)
        {
            EXPPLPIItemColorBale Model = new EXPPLPIItemColorBale();

            Model.PLPIItemColorBaleID = Entity.PLPIItemColorBaleID;
            Model.PLPIItemColorBaleNo = Entity.PLPIItemColorBaleNo;
            Model.PLPIItemColorID = Entity.PLPIItemColorID;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.PcsInBale = Entity.PcsInBale;
            Model.SideInBale = Entity.SideInBale;
            Model.MeterPLPIBaleQty = Entity.MeterPLPIBaleQty;
            Model.FootPLPIBaleQty = Entity.FootPLPIBaleQty;
            Model.PLPIBaleNetWeight = Entity.PLPIBaleNetWeight;
            //Model.NetWeightUnit = Entity.NetWeightUnit;
            Model.PLPIBGrossaleWeight = Entity.PLPIBGrossaleWeight;
            Model.GrossBaleWeightUnit = Entity.GrossBaleWeightUnit;
            Model.GrossBaleWeightUnitName = Entity.GrossBaleWeightUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.GrossBaleWeightUnit).FirstOrDefault().UnitName;
            Model.Remarks = Entity.Remarks;

            return Model;
        }

        public ValidationMsg DeletedPackingList(long? PLID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.EXP_PLPI.Where(m => m.PLID == PLID).ToList();

                if (itemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteplElement = _context.EXP_PackingList.First(m => m.PLID == PLID);
                    _context.EXP_PackingList.Remove(deleteplElement);
                    _context.SaveChanges();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedEXPPLPIItemColor(long? PLPIItemColorID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.EXP_PLPIItemColorBale.Where(m => m.PLPIItemColorID == PLPIItemColorID).ToList();

                if (itemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.EXP_PLPIItemColor.First(m => m.PLPIItemColorID == PLPIItemColorID);
                    _context.EXP_PLPIItemColor.Remove(deleteElement);
                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedEXPPLPIItemColorBale(long? PLPIItemColorBaleID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.EXP_PLPIItemColorBale.First(m => m.PLPIItemColorBaleID == PLPIItemColorBaleID);
                _context.EXP_PLPIItemColorBale.Remove(deleteElement);

                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }
        public ValidationMsg ConfirmedEXPPackingList(EXPPackingList model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityCI = _context.EXP_PackingList.First(m => m.PLID == model.PLID);
                        originalEntityCI.RecordStatus = "CNF";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;

                        //CIPIID = model.ExpCIPIList[0].CIPIID;
                        //var originalEntityCIPI = _context.EXP_CIPI.First(m => m.CIPIID == CIPIID);
                        //originalEntityCIPI.RecordStatus = "CNF";
                        //originalEntityCIPI.ModifiedBy = userid;
                        //originalEntityCIPI.ModifiedOn = DateTime.Now;

                        //if (model.ExpCIPIItemList.Count > 0)
                        //{
                        //    foreach (EXPCIPIItem objEXPCIPIItem in model.ExpCIPIItemList)
                        //    {
                        //        var originalEntityCIPIItem = _context.EXP_CIPIItem.First(m => m.CIPIItemID == objEXPCIPIItem.CIPIItemID);
                        //        originalEntityCIPIItem.RecordStatus = "CNF";
                        //        originalEntityCIPIItem.ModifiedBy = userid;
                        //        originalEntityCIPIItem.ModifiedOn = DateTime.Now;
                        //    }
                        //}

                        _context.SaveChanges();

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Confirmed Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Confirmed.";
            }
            return _vmMsg;
        }
    }
}
