using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;
using System.Transactions;
using System.Linq;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalExportPI
    {
        private readonly BLC_DEVEntities _context;

        public DalExportPI()
        {
            _context = new BLC_DEVEntities();
        }

        public List<PRQChemFrgnPurcOrdr> GetBuyerListForPI()
        {
            using (_context)
            {
                var Data = (from b in _context.SLS_BuyerOrder.AsEnumerable()

                            join bd in _context.Sys_Buyer on b.BuyerID equals bd.BuyerID into BuyerDetails
                            from bd in BuyerDetails.DefaultIfEmpty()

                            join ba in _context.Sys_BuyerAddress on b.BuyerAddressID equals ba.BuyerAddressID into BuyerAddress
                            from ba in BuyerAddress.DefaultIfEmpty()



                            join la in _context.Sys_Buyer on (b == null ? null : b.BuyerLocalAgentID) equals la.BuyerID into LocalAgents
                            from la in LocalAgents.DefaultIfEmpty()

                            join laa in _context.Sys_BuyerAddress on (b == null ? null : b.BuyerLocalAgentID) equals laa.BuyerID into LocalAgentsAddress
                            from laa in LocalAgentsAddress.DefaultIfEmpty()



                            join fa in _context.Sys_Buyer on (b == null ? null : b.BuyerForeignAgentID) equals fa.BuyerID into ForeignAgents
                            from fa in ForeignAgents.DefaultIfEmpty()

                            join faa in _context.Sys_BuyerAddress on (b == null ? null : b.BuyerForeignAgentID) equals faa.BuyerID into ForeignAgentsAddress
                            from faa in ForeignAgentsAddress.DefaultIfEmpty()

                            orderby b.BuyerOrderID descending

                            select new PRQChemFrgnPurcOrdr
                            {
                                BuyerID = Convert.ToInt16(b.BuyerID),
                                BuyerCode = (bd == null ? null : bd.BuyerCode),
                                BuyerName = (bd == null ? null : bd.BuyerName),
                                //BuyerAddressID = b.BuyerAddressID.ToString(),
                                BuyerAddress = (ba == null ? null : ba.Address),
                                BuyerContactNumber = (ba == null ? null : ba.ContactNumber),

                                LocalAgent = Convert.ToInt16(b.BuyerLocalAgentID),
                                LocalAgentCode = (la == null ? null : la.BuyerCode),
                                LocalAgentName = (la == null ? null : la.BuyerName),
                                LocalAgentAddress = (laa == null ? null : laa.Address),
                                LocalAgentContactNumber = (laa == null ? null : laa.ContactNumber),

                                ForeignAgent = Convert.ToInt16(b.BuyerForeignAgentID),
                                ForeignAgentCode = (fa == null ? null : fa.BuyerCode),
                                ForeignAgentName = (fa == null ? null : fa.BuyerName),
                                ForeignAgentAddress = (faa == null ? null : faa.Address),
                                ForeignAgentContactNumber = (faa == null ? null : faa.ContactNumber),

                            }).ToList();

                return Data;
            }

        }

        public List<PRQChemFrgnPurcOrdr> GetLocalBuyerAgent()
        {
            using(_context)
            {
                var LocalAgents = (from la in _context.Sys_Buyer
                                   where la.BuyerCategory == "Buyer Agent" & la.BuyerType == "Local Agent"

                                   join laa in _context.Sys_BuyerAddress on la.BuyerID equals laa.BuyerID into Address
                                   from laa in Address.DefaultIfEmpty()
                                   select new PRQChemFrgnPurcOrdr
                                   {
                                       LocalAgent = la.BuyerID,
                                       LocalAgentName = la.BuyerName,
                                       LocalAgentCode = la.BuyerCode,
                                       LocalAgentAddress = laa == null ? null : laa.Address,
                                       LocalAgentContactNumber = laa == null ? null : laa.ContactNumber
                                   }).ToList();

                return LocalAgents;
            }
        }

        public List<PRQChemFrgnPurcOrdr> GetForeignBuyerAgent()
        {
            using (_context)
            {
                var ForeignAgents = (from la in _context.Sys_Buyer
                                   where la.BuyerCategory == "Buyer Agent" & la.BuyerType == "Foreign Agent"

                                   join laa in _context.Sys_BuyerAddress on la.BuyerID equals laa.BuyerID into Address
                                   from laa in Address.DefaultIfEmpty()
                                   select new PRQChemFrgnPurcOrdr
                                   {
                                       ForeignAgent = la.BuyerID,
                                       ForeignAgentName = la.BuyerName,
                                       ForeignAgentCode = la.BuyerCode,
                                       ForeignAgentAddress = laa == null ? null : laa.Address,
                                       ForeignAgentContactNumber = laa == null ? null : laa.ContactNumber
                                   }).ToList();

                return ForeignAgents;
            }
        }

        public List<SysBeneficiary> GetBeneficiaryListForPI()
        {
            using (_context)
            {
                var Data = (from b in _context.Sys_Supplier.AsEnumerable()
                            where b.SupplierCategory == "Leather"

                            from ba in _context.Sys_SupplierAddress
                            where ba.SupplierID == b.SupplierID && ba.IsActive == true

                            select new SysBeneficiary
                            {
                                BeneficiaryID = b.SupplierID,
                                BeneficiaryCode = b.SupplierCode,
                                BeneficiaryName = b.SupplierName,
                                BeneficiaryAddressID = (ba.SupplierAddressID),
                                BeneficiaryAddress = ba.Address,
                                BeneficiaryContactNumber = ba.ContactNumber
                            }).ToList();

                return Data;
            }

        }

        public List<SlsBuyerOrderBadhon> GetOrderListForParticularBuyer(string _BuyerID)
        {
            var Data = (from o in _context.SLS_BuyerOrder.AsEnumerable()
                        where o.BuyerID.ToString() == _BuyerID & (o.RecordStatus == "CNF" || o.RecordStatus == "ACK")
                        //where o.BuyerID.ToString() == _BuyerID & o.AcknowledgementStatus == "FLA" & o.BuyerOrderCategory == "FRN"


                        orderby o.BuyerOrderID descending
                        select new SlsBuyerOrderBadhon
                        {
                            BuyerOrderID = o.BuyerOrderID,
                            BuyerOrderNo = o.OrderNo,
                            BuyerOrderDate = Convert.ToDateTime(o.BuyerOrderDate).ToString("dd'/'MM'/'yyyy"),
                            ExpectedShipmentDate = Convert.ToDateTime(o.ExpectedShipmentDate).ToString("dd'/'MM'/'yyyy"),
                            PriceLevel= o.PriceLevel
                        }).ToList();

            return Data;
            
        }

        public List<SlsBuyerOrderDeliveryBadhon> BuyerOrderDeliveryList(long _BuyerOrderID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from o in context.SLS_BuyerOrderDelivery.AsEnumerable()
                            where o.BuyerOrderID == _BuyerOrderID & o.IsActive == true

                            join a in context.Sys_Article on o.ArticleID == null ? null : o.ArticleID equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            join c in context.Sys_Color on o.ColorID == null ? null : o.ColorID equals c.ColorID into Colors
                            from c in Colors.DefaultIfEmpty()

                            orderby Convert.ToDateTime(o.OrdDeliveryDate)

                            select new SlsBuyerOrderDeliveryBadhon
                            {
                                BuyerOrderDeliveryID = o.BuyerOrderDeliveryID,
                                OrdDeliverySL = o.OrdDeliverySL,
                                OrdDeliveryDate = Convert.ToDateTime(o.OrdDeliveryDate).ToString("dd'/'MM'/'yyyy"),
                                OrdDateFootQty = o.OrdDateFootQty,
                                OrdDateMeterQty = o.OrdDateMeterQty,
                                ArticleID = o.ArticleID == null ? null : o.ArticleID,
                                ArticleName = a == null ? null : a.ArticleName,
                                ColorID = o.ColorID == null ? null : o.ColorID,
                                ColorName = c == null ? null : c.ColorName,
                            });

                return Data.ToList();
            }
            

        }


        public List<SlsBuyerOrderItemBadhon> GetOrderItemList(string _BuyerOrderID)
        {
            using(_context)
            {
                var Data = (from o in _context.SLS_BuyerOrderItem.AsEnumerable()
                            where o.BuyerOrderID.ToString() == _BuyerOrderID

                            join a in _context.Sys_Article on o.ArticleID equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            join au in _context.Sys_Unit on o.AvgSizeUnit equals au.UnitID into SizeUnit
                            from au in SizeUnit.DefaultIfEmpty()

                            join tu in _context.Sys_Unit on o.AvgSizeUnit equals tu.UnitID into ThicknessUnit
                            from tu in ThicknessUnit.DefaultIfEmpty()


                            select new SlsBuyerOrderItemBadhon
                            {
                                BuyerOrderItemID = o.BuyerOrderItemID,
                                commodity = (o.commodity == null ? "" : o.commodity),
                                HSCode = o.HSCode,
                                ArticleID = o.ArticleID,
                                ArticleName = (a == null ? null : a.ArticleName),
                                ArticleNo = o.ArticleNo,
                                AvgSize = o.AvgSize,
                                AvgSizeUnit = o.AvgSizeUnit,
                                AvgSizeUnitName = (au == null ? "" : au.UnitName),
                                SideDescription = o.SideDescription,
                                SelectionRange = o.SelectionRange,
                                Thickness = o.Thickness,
                                ThicknessUnit = o.ThicknessUnit,
                                ThicknessUnitName = (tu == null ? "" : tu.UnitName),
                                ThicknessAt = o.ThicknessAt,

                                ArticleFootQty = o.ArticleFootQty,

                                SeaFootUnitPrice = o.SeaFootUnitPrice,
                                SeaFootTotalPrice = o.SeaFootTotalPrice,
                                AirFootUnitPrice = o.AirFootUnitPrice,
                                AirFootTotalPrice = o.AirFootTotalPrice,
                                //RoadFootUnitPrice = o.RoadFootUnitPrice,
                                //RoadFootTotalPrice = o.RoadFootTotalPrice,

                                ArticleMeterQty = o.ArticleMeterQty,

                                SeaMeterUnitPrice = o.SeaMeterUnitPrice,
                                SeaMeterTotalPrice = o.SeaMeterTotalPrice,
                                AirMeterTotalPrice = o.AirMeterTotalPrice,
                                AirMeterUnitPrice = o.AirMeterUnitPrice,
                                //RoadMeterUnitPrice = o.RoadMeterUnitPrice,
                                //RoadMeterTotalPrice = o.RoadMeterTotalPrice,

                            }).ToList();

                return Data;
            }
        }

        public List<SlsBuyerOrderItemColorBadhon> GetOrderItemColorList(string _BuyerOrderID, string _BuyerOrderItemId)
        {
            var Data = (from c in _context.SLS_BuyerOrderItemColor.AsEnumerable()
                        where c.BuyerOrderID.ToString() == _BuyerOrderID & c.BuyerOrderItemID.ToString() == _BuyerOrderItemId

                        join cl in _context.Sys_Color on c.ColorID equals cl.ColorID into Colors
                        from cl in Colors.DefaultIfEmpty()

                        join cu in _context.Sys_Unit on c.ColorUnit equals cu.UnitID into ColorUnits
                        from cu in ColorUnits.DefaultIfEmpty()

                        select new SlsBuyerOrderItemColorBadhon
                        {
                            OrderItemColorId = c.BuyerOrdItemColorID,
                            ColorID = c.ColorID,
                            ColorName = cl == null ? null : cl.ColorName,

                            ColorFootQty = c.ColorFootQty,

                            SeaFootUnitPrice = c.SeaFootUnitPrice,
                            SeaFootTotalPrice = c.SeaFootTotalPrice,
                            AirFootUnitPrice = c.AirFootUnitPrice,
                            AirFootTotalPrice = c.AirFootTotalPrice,
                            //RoadFootUnitPrice = c.RoadFootUnitPrice,
                            //RoadFootTotalPrice = c.RoadFootTotalPrice,

                            ColorMeterQty = c.ColorMeterQty,

                            SeaMeterUnitPrice = c.SeaMeterUnitPrice,
                            SeaMeterTotalPrice = c.SeaMeterTotalPrice,
                            AirMeterTotalPrice = c.AirMeterTotalPrice,
                            AirMeterUnitPrice = c.AirMeterUnitPrice,
                            //RoadMeterUnitPrice = o.RoadMeterUnitPrice,
                            //RoadMeterTotalPrice = o.RoadMeterTotalPrice,
                        }).ToList();

            //foreach(var item in Data)
            //{
            //    if(item.ColorUnitName.EndsWith("t") || item.ColorUnitName.EndsWith("T"))
            //    {
            //        item.FootColorQty = Convert.ToDecimal(item.ColorQty);
            //        item.FootUnitPrice = Convert.ToDecimal(item.UnitPrice);
            //        item.MeterColorQty = Decimal.Round(Convert.ToDecimal(0.0929 * item.ColorQty),2);
            //        //item.MeterUnitPrice = Convert.ToDecimal(item.UnitPrice) / Convert.ToDecimal(0.0929); // Calculation Needed
            //        item.MeterUnitPrice = Convert.ToDecimal(item.TotalPrice / item.MeterColorQty);
            //    }
            //    else if(item.ColorUnitName.EndsWith("r") || item.ColorUnitName.EndsWith("R"))
            //    {
            //        item.MeterColorQty = Convert.ToDecimal(item.ColorQty);
            //        item.MeterUnitPrice = Convert.ToDecimal(item.UnitPrice);
            //        item.FootColorQty = Decimal.Round(Convert.ToDecimal(item.ColorQty * 10.7639), 2);
            //        item.FootUnitPrice = Convert.ToDecimal(item.UnitPrice / Convert.ToDecimal(10.7639)); // Calculation Needed
            //    }
            //}
            return Data;
        }

        public long Save(EXPLeatherPI model, int userId, string pageUrl)
        {
            long CurrentPIID = 0;
            long CurrentPIItemID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        
                        #region New_PI_Insert

                        EXP_LeatherPI objPI = new EXP_LeatherPI();

                        objPI.PINo = model.PINo;
                        objPI.PIDate = DalCommon.SetDate(model.PIDate);
                        objPI.PICategory = model.PICategory;
                        objPI.BuyerOrderID = model.BuyerOrderID;
                        objPI.BuyerID = model.BuyerID;
                        if (model.LocalAgent == 0)
                            objPI.LocalAgent = null;
                        else
                            objPI.LocalAgent = model.LocalAgent;

                        if (model.ForeignAgent == 0)
                            objPI.ForeignAgent = null;
                        else
                            objPI.ForeignAgent = model.ForeignAgent;

                        objPI.LocalComPercent = model.LocalComPercent;
                        objPI.ForeignComPercent = model.ForeignComPercent;

                        if (model.BeneficiaryID == 0)
                            objPI.PIBeneficiary = null;
                        else
                            objPI.PIBeneficiary = model.BeneficiaryID;

                        if (model.BeneficiaryAddressID == 0)
                            objPI.BeneficiaryAddressID = null;
                        else
                            objPI.BeneficiaryAddressID = model.BeneficiaryAddressID;

                        objPI.PINotify = model.PINotify;
                        objPI.PIConsignee = model.PIConsignee;

                        if (model.PICurrency == 0)
                            objPI.PICurrency = null;
                        else
                            objPI.PICurrency = model.PICurrency;

                        objPI.PaymentMode = model.PaymentMode;
                        objPI.PaymentNote = model.PaymentNote;
                        

                        objPI.PriceLevel = model.PriceLevel;
                        if (model.CountryOforigin == 0)
                            objPI.CountryOforigin = null;
                        else
                            objPI.CountryOforigin = model.CountryOforigin;

                        
                        objPI.DeliveryTerm = model.DeliveryTerm;

                        if (model.PortofLoading == 0)
                            objPI.PortofLoading = null;
                        else
                            objPI.PortofLoading = model.PortofLoading;

                        if (model.PortofDischarge == 0)
                            objPI.PortofDischarge = null;
                        else
                            objPI.PortofDischarge = model.PortofDischarge;


                        objPI.Transshipment = model.Transshipment;
                        objPI.PartialShipment = model.PartialShipment;


                        objPI.GoodDelivery = model.GoodDelivery;
                        objPI.ShippingMark = model.ShippingMark;

                        objPI.Packing = model.Packing;
                        objPI.PreShipmentIns = model.PreShipmentIns;

                        if (model.OfferValidityDays == 0)
                            objPI.OfferValidityDays = null;
                        else
                            objPI.OfferValidityDays = model.OfferValidityDays;


                        objPI.OfferValidityNote = model.OfferValidityNote;
                        objPI.OtherTerm = model.OtherTerm;
                       
                        objPI.ShipmentNote = model.ShipmentNote;
                        objPI.InsuranceTerm = model.InsuranceTerm;
                       

                        if (model.BuyerBank == 0)
                            objPI.BuyerBank = null;
                        else
                            objPI.BuyerBank = model.BuyerBank;

                        objPI.BuyerBankAccount = model.BuyerBankAccount;

                        if (model.BeneficiaryBank == 0) //Seller Bank
                            objPI.BeneficiaryBank = null;
                        else
                            objPI.BeneficiaryBank = model.BeneficiaryBank;

                        objPI.BankAccount = model.BankAccount; // Seller Bank Account
                        

                        objPI.RecordStatus = "NCF";
                        objPI.SetBy = userId;
                        objPI.SetOn = DateTime.Now;

                        objPI.PIStatus = "PIPR"; //PI Processed
                        objPI.PIState = "RNG";

                        _context.EXP_LeatherPI.Add(objPI);
                        _context.SaveChanges();
                        CurrentPIID = objPI.PIID;
                        #endregion

                        #region Delivery Date Insert

                        foreach (var item in model.OrderDeliveryDates)
                        {
                            if (item.BuyerOrderDeliveryID != 0)
                            {
                                var objDelivery = (from d in _context.SLS_BuyerOrderDelivery
                                                   where d.BuyerOrderDeliveryID == item.BuyerOrderDeliveryID
                                                   select d).FirstOrDefault();

                                objDelivery.PIDateFootQty = item.OrdDateFootQty;
                                objDelivery.PIDateMeterQty = item.OrdDateMeterQty;

                                try
                                {
                                    var GridChallanDate = item.OrdDeliveryDate.Contains("/") ? item.OrdDeliveryDate : Convert.ToDateTime(item.OrdDeliveryDate).ToString("dd/MM/yyyy");
                                    objDelivery.PIDeliveryDate = DalCommon.SetDate(GridChallanDate);
                                }
                                catch
                                {
                                    var GridChallanDate = Convert.ToDateTime(item.OrdDeliveryDate).Date.ToString("dd/MM/yyyy");
                                    objDelivery.PIDeliveryDate = DalCommon.SetDate(GridChallanDate);
                                }
                                objDelivery.PIDeliverySL = item.OrdDeliverySL;
                                objDelivery.ArticleID = item.ArticleID;
                                objDelivery.ColorID = item.ColorID;
                            }
                            else
                            {
                                var objDelivery = new SLS_BuyerOrderDelivery();

                                objDelivery.BuyerOrderID = model.BuyerOrderID;
                                objDelivery.PIDateFootQty = item.OrdDateFootQty;
                                objDelivery.PIDateMeterQty = item.OrdDateMeterQty;
                                objDelivery.PIDeliveryDate = Convert.ToDateTime(item.OrdDeliveryDate);
                                objDelivery.PIDeliverySL = item.OrdDeliverySL;
                                objDelivery.ArticleID = item.ArticleID;
                                objDelivery.ColorID = item.ColorID;
                                objDelivery.IsActive = true;

                                _context.SLS_BuyerOrderDelivery.Add(objDelivery);
                            }


                        }
                        #endregion

                        #region Item Insert
                        if (model.PIItem != null)
                        {
                            foreach (var item in model.PIItem)
                            {
                                EXP_PIItem objItem = new EXP_PIItem();

                                objItem.PIID = CurrentPIID;
                                objItem.Commodity = item.commodity;
                                objItem.HSCode = item.HSCode;
                                objItem.ArticleID = item.ArticleID;
                                objItem.ArticleNo = item.ArticleNo;
                                objItem.AvgSize = item.AvgSize;
                                objItem.AvgSizeUnit = DalCommon.GetUnitCode(item.AvgSizeUnitName);
                                objItem.SideDescription = item.SideDescription;
                                objItem.SelectionRange = item.SelectionRange;
                                objItem.Thickness = item.Thickness.ToString();
                                objItem.ThicknessUnit = DalCommon.GetUnitCode(item.ThicknessUnitName);
                                objItem.ThicknessAt = (item.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");

                                objItem.ArticleFootQty = item.ArticleFootQty;
                                objItem.SeaFootUnitPrice = item.SeaFootUnitPrice;
                                objItem.SeaFootTotalPrice = item.SeaFootTotalPrice;
                                objItem.AirFootUnitPrice = item.AirFootUnitPrice;
                                objItem.AirFootTotalPrice = item.AirFootTotalPrice;

                                objItem.ArticleMeterQty = item.ArticleMeterQty;
                                objItem.SeaMeterUnitPrice = item.SeaMeterUnitPrice;
                                objItem.SeaMeterTotalPrice = item.SeaMeterTotalPrice;
                                objItem.AirMeterUnitPrice = item.AirMeterUnitPrice;
                                objItem.AirMeterTotalPrice = item.AirMeterTotalPrice;

                                objItem.SetOn = DateTime.Now;
                                objItem.SetBy = userId;

                                _context.EXP_PIItem.Add(objItem);
                                _context.SaveChanges();
                                CurrentPIItemID = objItem.PIItemID;
                            }

                        }
                        #endregion

                        #region Color Insert
                        
                        if (model.PIColor != null)
                        {
                            foreach (var item in model.PIColor)
                            {
                                EXP_PIItemColor objItem = new EXP_PIItemColor();

                                objItem.PIItemID = CurrentPIItemID;
                                objItem.ColorID = item.ColorID;

                                objItem.ColorFootQty = item.ColorFootQty;
                                objItem.SeaFootUnitPrice = item.SeaFootUnitPrice;
                                objItem.SeaFootTotalPrice = item.SeaFootTotalPrice;
                                objItem.AirFootUnitPrice = item.AirFootUnitPrice;
                                objItem.AirFootTotalPrice = item.AirFootTotalPrice;

                                objItem.ColorMeterQty = item.ColorMeterQty;
                                objItem.SeaMeterUnitPrice = item.SeaMeterUnitPrice;
                                objItem.SeaMeterTotalPrice = item.SeaMeterTotalPrice;
                                objItem.AirMeterUnitPrice = item.AirMeterUnitPrice;
                                objItem.AirMeterTotalPrice = item.AirMeterTotalPrice;

                                objItem.SetOn = DateTime.Now;
                                objItem.SetBy = userId;

                                _context.EXP_PIItemColor.Add(objItem);
                                _context.SaveChanges();
                            }

                        }
                        #endregion
                        
                    }
                    transaction.Complete();
                }
                return CurrentPIID;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public int Update(EXPLeatherPI model, int userId)
        {
            long CurrentPIItemID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {

                        #region PI_Informaiton_Update
                        var currentPI = (from p in _context.EXP_LeatherPI.AsEnumerable()
                                         where p.PIID == model.PIID
                                         select p).FirstOrDefault();

                        currentPI.PINo = model.PINo;
                        currentPI.PIDate = DalCommon.SetDate(model.PIDate);
                        currentPI.PICategory = model.PICategory;
                        currentPI.BuyerOrderID = model.BuyerOrderID;
                        //objPI.PIIssueDate = DalCommon.SetDate(model.BuyerOrderDate); // Is it OK?
                        currentPI.BuyerID = model.BuyerID;
                        if (model.LocalAgent == 0)
                            currentPI.LocalAgent = null;
                        else
                            currentPI.LocalAgent = model.LocalAgent;


                        if (model.ForeignAgent == 0)
                            currentPI.ForeignAgent = null;
                        else
                            currentPI.ForeignAgent = model.ForeignAgent;

                        currentPI.LocalComPercent = model.LocalComPercent;
                        currentPI.ForeignComPercent = model.ForeignComPercent;


                        if (model.BeneficiaryID == 0)
                            currentPI.PIBeneficiary = null;
                        else
                            currentPI.PIBeneficiary = model.BeneficiaryID;


                        if (model.BeneficiaryAddressID == 0)
                            currentPI.BeneficiaryAddressID = null;
                        else
                            currentPI.BeneficiaryAddressID = model.BeneficiaryAddressID;


                        currentPI.PINotify = model.PINotify;
                        currentPI.PIConsignee = model.PIConsignee;

                        currentPI.PriceLevel = model.PriceLevel;
                        if (model.PICurrency == 0)
                            currentPI.PICurrency = null;
                        else
                            currentPI.PICurrency = model.PICurrency;

                        currentPI.PaymentMode = model.PaymentMode;
                        currentPI.PaymentNote = model.PaymentNote;
                      
                        if (model.CountryOforigin == 0)
                            currentPI.CountryOforigin = null;
                        else
                            currentPI.CountryOforigin = model.CountryOforigin;

                        if (model.BuyerBank == 0)
                            currentPI.BuyerBank = null;
                        else
                            currentPI.BuyerBank = model.BuyerBank;

                        currentPI.BuyerBankAccount = model.BuyerBankAccount;

                        if (model.BeneficiaryBank == 0) //Seller Bank
                            currentPI.BeneficiaryBank = null;
                        else
                            currentPI.BeneficiaryBank = model.BeneficiaryBank;

                        currentPI.BankAccount = model.BankAccount; // Seller Bank Account




                        currentPI.DeliveryTerm = model.DeliveryTerm;
                        
                        if (model.PortofLoading == 0)
                            currentPI.PortofLoading = null;
                        else
                            currentPI.PortofLoading = model.PortofLoading;


                        if (model.PortofDischarge == 0)
                            currentPI.PortofDischarge = null;
                        else
                            currentPI.PortofDischarge = model.PortofDischarge;
                        currentPI.Transshipment = model.Transshipment;
                        currentPI.PartialShipment = model.PartialShipment;
                        currentPI.GoodDelivery = model.GoodDelivery;
                        currentPI.ShippingMark = model.ShippingMark;

                        currentPI.Packing = model.Packing;
                        currentPI.PreShipmentIns = model.PreShipmentIns;

                        if (model.OfferValidityDays == 0)
                            currentPI.OfferValidityDays = null;
                        else
                            currentPI.OfferValidityDays = model.OfferValidityDays;


                        currentPI.OfferValidityNote = model.OfferValidityNote;
                        currentPI.OtherTerm = model.OtherTerm;
                        currentPI.ShipmentNote = model.ShipmentNote;
                        currentPI.InsuranceTerm = model.InsuranceTerm;
                        
                        currentPI.ModifiedBy = userId;
                        currentPI.ModifiedOn = DateTime.Now;

                        _context.SaveChanges();
                        #endregion

                        #region Delivery Date Insert

                        foreach (var item in model.OrderDeliveryDates)
                        {
                            if(item.BuyerOrderDeliveryID!=0)
                            {
                                var objDelivery = (from d in _context.SLS_BuyerOrderDelivery
                                                   where d.BuyerOrderDeliveryID == item.BuyerOrderDeliveryID
                                                   select d).FirstOrDefault();

                                objDelivery.PIDateFootQty = item.OrdDateFootQty;
                                objDelivery.PIDateMeterQty = item.OrdDateMeterQty;

                                try
                                {
                                    var GridChallanDate = item.OrdDeliveryDate.Contains("/") ? item.OrdDeliveryDate : Convert.ToDateTime(item.OrdDeliveryDate).ToString("dd/MM/yyyy");
                                    objDelivery.PIDeliveryDate = DalCommon.SetDate(GridChallanDate);
                                }
                                catch
                                {
                                    var GridChallanDate = Convert.ToDateTime(item.OrdDeliveryDate).Date.ToString("dd/MM/yyyy");
                                    objDelivery.PIDeliveryDate = DalCommon.SetDate(GridChallanDate);
                                }
                                //objDelivery.PIDeliveryDate = Convert.ToDateTime(item.OrdDeliveryDate);
                                objDelivery.PIDeliverySL = item.OrdDeliverySL;
                                objDelivery.ArticleID = item.ArticleID;
                                objDelivery.ColorID = item.ColorID;
                            }
                            else
                            {
                                var objDelivery = new SLS_BuyerOrderDelivery();

                                objDelivery.BuyerOrderID = model.BuyerOrderID;
                                objDelivery.PIDateFootQty = item.OrdDateFootQty;
                                objDelivery.PIDateMeterQty = item.OrdDateMeterQty;
                                objDelivery.PIDeliveryDate = Convert.ToDateTime(item.OrdDeliveryDate);
                                objDelivery.PIDeliverySL = item.OrdDeliverySL;
                                objDelivery.ArticleID = item.ArticleID;
                                objDelivery.ColorID = item.ColorID;
                                objDelivery.IsActive = true;

                                _context.SLS_BuyerOrderDelivery.Add(objDelivery);
                            }
                            

                        }
                        #endregion



                        #region Update_PIItemList
                        if (model.PIItem != null)
                        {
                            foreach (var item in model.PIItem)
                            {

                                var checkPIItem = (from i in _context.EXP_PIItem
                                                   where i.PIItemID == item.PIItemID
                                                   select i).Any();

                                #region New_Requisition_Insertion
                                if (!checkPIItem)
                                {
                                    EXP_PIItem objItem = new EXP_PIItem();
                                    objItem.PIID = model.PIID;
                                    objItem.Commodity = item.commodity;
                                    objItem.HSCode = item.HSCode;
                                    objItem.ArticleID = item.ArticleID;
                                    objItem.ArticleNo = item.ArticleNo;
                                    objItem.AvgSize = item.AvgSize;
                                    objItem.AvgSizeUnit = DalCommon.GetUnitCode(item.AvgSizeUnitName);
                                    objItem.SideDescription = item.SideDescription;
                                    objItem.SelectionRange = item.SelectionRange;
                                    objItem.Thickness = item.Thickness;
                                    objItem.ThicknessUnit = DalCommon.GetUnitCode(item.ThicknessUnitName);
                                    objItem.ThicknessAt = (item.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");

                                    objItem.ArticleFootQty = item.ArticleFootQty;
                                    objItem.SeaFootUnitPrice = item.SeaFootUnitPrice;
                                    objItem.SeaFootTotalPrice = item.SeaFootTotalPrice;
                                    objItem.AirFootUnitPrice = item.AirFootUnitPrice;
                                    objItem.AirFootTotalPrice = item.AirFootTotalPrice;

                                    objItem.ArticleMeterQty = item.ArticleMeterQty;
                                    objItem.SeaMeterUnitPrice = item.SeaMeterUnitPrice;
                                    objItem.SeaMeterTotalPrice = item.SeaMeterTotalPrice;
                                    objItem.AirMeterUnitPrice = item.AirMeterUnitPrice;
                                    objItem.AirMeterTotalPrice = item.AirMeterTotalPrice;


                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.EXP_PIItem.Add(objItem);
                                    _context.SaveChanges();
                                    CurrentPIItemID = objItem.PIItemID;

                                }
                                #endregion

                                #region Existing_Requisition_Update
                                else
                                {
                                    var FoundPI = (from i in _context.EXP_PIItem.AsEnumerable()
                                                   where i.PIItemID == item.PIItemID
                                                   select i).FirstOrDefault();

                                    FoundPI.PIID = model.PIID;

                                    FoundPI.Commodity = item.commodity;
                                    FoundPI.HSCode = item.HSCode;
                                    FoundPI.ArticleID = item.ArticleID;
                                    FoundPI.ArticleNo = item.ArticleNo;
                                    FoundPI.AvgSize = item.AvgSize;
                                    FoundPI.AvgSizeUnit = DalCommon.GetUnitCode(item.AvgSizeUnitName);
                                    FoundPI.SideDescription = item.SideDescription;
                                    FoundPI.SelectionRange = item.SelectionRange;
                                    FoundPI.Thickness = item.Thickness;
                                    FoundPI.ThicknessUnit = DalCommon.GetUnitCode(item.ThicknessUnitName);
                                    //FoundPI.ThicknessAt = (item.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");

                                    FoundPI.ArticleFootQty = item.ArticleFootQty;
                                    FoundPI.SeaFootUnitPrice = item.SeaFootUnitPrice;
                                    FoundPI.SeaFootTotalPrice = item.SeaFootTotalPrice;
                                    FoundPI.AirFootUnitPrice = item.AirFootUnitPrice;
                                    FoundPI.AirFootTotalPrice = item.AirFootTotalPrice;

                                    FoundPI.ArticleMeterQty = item.ArticleMeterQty;
                                    FoundPI.SeaMeterUnitPrice = item.SeaMeterUnitPrice;
                                    FoundPI.SeaMeterTotalPrice = item.SeaMeterTotalPrice;
                                    FoundPI.AirMeterUnitPrice = item.AirMeterUnitPrice;
                                    FoundPI.AirMeterTotalPrice = item.AirMeterTotalPrice;

                                    FoundPI.ModifiedBy = userId;
                                    FoundPI.ModifiedOn = DateTime.Now;

                                    //_context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion

                        #region To_Find_ItemID_For_ItemColors_If_Any

                        if (model.PIColor != null)
                        {
                            foreach (var Color in model.PIColor)
                            {
                                if (Color.PIItemID != 0)
                                {
                                    CurrentPIItemID = Convert.ToInt64(Color.PIItemID);
                                    break;
                                }
                                else
                                {
                                    if (model.SelectedPIItemID != 0)
                                    {
                                        CurrentPIItemID = model.SelectedPIItemID;
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion

                        #region Update_Item_Color_Information
                        if (model.PIColor != null)
                        {
                            foreach (var Color in model.PIColor)
                            {
                                #region New_Color_Insertion
                                if (Color.PIItemColorID == 0)
                                {
                                    EXP_PIItemColor objColor = new EXP_PIItemColor();

                                    objColor.PIItemID = CurrentPIItemID;
                                    objColor.ColorID = Color.ColorID;

                                    objColor.ColorFootQty = Color.ColorFootQty;
                                    objColor.SeaFootUnitPrice = Color.SeaFootUnitPrice;
                                    objColor.SeaFootTotalPrice = Color.SeaFootTotalPrice;
                                    objColor.AirFootUnitPrice = Color.AirFootUnitPrice;
                                    objColor.AirFootTotalPrice = Color.AirFootTotalPrice;

                                    objColor.ColorMeterQty = Color.ColorMeterQty;
                                    objColor.SeaMeterUnitPrice = Color.SeaMeterUnitPrice;
                                    objColor.SeaMeterTotalPrice = Color.SeaMeterTotalPrice;
                                    objColor.AirMeterUnitPrice = Color.AirMeterUnitPrice;
                                    objColor.AirMeterTotalPrice = Color.AirMeterTotalPrice;
                                    objColor.SetOn = DateTime.Now;
                                    objColor.SetBy = userId;

                                    _context.EXP_PIItemColor.Add(objColor);
                                    //_context.SaveChanges();
                                }
                                #endregion

                                #region Update_Existing_Color
                                else if (Color.PIItemColorID != 0)
                                {
                                    var currentColorItem = (from c in _context.EXP_PIItemColor
                                                            where c.PIItemColorID == Color.PIItemColorID
                                                            select c).FirstOrDefault();

                                    currentColorItem.ColorID = Color.ColorID;


                                    currentColorItem.ColorFootQty = Color.ColorFootQty;
                                    currentColorItem.SeaFootUnitPrice = Color.SeaFootUnitPrice;
                                    currentColorItem.SeaFootTotalPrice = Color.SeaFootTotalPrice;
                                    currentColorItem.AirFootUnitPrice = Color.AirFootUnitPrice;
                                    currentColorItem.AirFootTotalPrice = Color.AirFootTotalPrice;

                                    currentColorItem.ColorMeterQty = Color.ColorMeterQty;
                                    currentColorItem.SeaMeterUnitPrice = Color.SeaMeterUnitPrice;
                                    currentColorItem.SeaMeterTotalPrice = Color.SeaMeterTotalPrice;
                                    currentColorItem.AirMeterUnitPrice = Color.AirMeterUnitPrice;
                                    currentColorItem.AirMeterTotalPrice = Color.AirMeterTotalPrice;

                                    currentColorItem.ModifiedBy = userId;
                                    currentColorItem.ModifiedOn = DateTime.Now;
                                    //_context.SaveChanges();
                                }
                                #endregion

                            }
                        }
                        #endregion

                        _context.SaveChanges();
                    }

                    transaction.Complete();
                }
                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public List<PRQChemicalPI> GetPIInformationForSearch()
        {
            using (_context)
            {
                var Data = (from p in _context.EXP_LeatherPI.AsEnumerable()

                            join s in _context.Sys_Buyer on (p.BuyerID==null? null: p.BuyerID) equals s.BuyerID into Buyers
                            from s in Buyers.DefaultIfEmpty()

                            //from s in _context.Sys_Buyer.Where(x => x.BuyerID == p.BuyerID).DefaultIfEmpty()

                            join lad in _context.Sys_Buyer on (p.LocalAgent==null?null:p.LocalAgent) equals lad.BuyerID into LocalAgent
                            from lad in LocalAgent.DefaultIfEmpty()

                            join fad in _context.Sys_Buyer on (p.ForeignAgent == null ? null : p.LocalAgent) equals fad.BuyerID into ForeignAgent
                            from fad in ForeignAgent.DefaultIfEmpty()

                            orderby p.PIID descending
                            select new PRQChemicalPI
                            {
                                PIID = Convert.ToInt32(p.PIID),
                                PINo = p.PINo,
                                PIDate = (Convert.ToDateTime(p.PIDate)).ToString("dd'/'MM'/'yyyy"),
                                PICategory = (p.PICategory == "PI" ? "Proforma Invoice" : "Indent Order"),
                                SupplierName = (s == null ? null : s.BuyerName),
                                LocalAgentName = (lad == null ? null : lad.BuyerName),
                                ForeignAgentName = (fad == null ? null : fad.BuyerName),
                                RecordStatus = DalCommon.ReturnRecordStatus(p.RecordStatus)
                            }).ToList();

                return Data;
            }
        }


        public EXPLeatherPIBadhon GetPIDetailsAfterSearch(long PIID)
        {
            using (_context)
            {
                //var GrandTotalSFT = GetGrandTotalSFT(PIID);
                var GrandTotalSMT = GetGrandTotalSMT(PIID);

                var model = new EXPLeatherPIBadhon();

                #region PI Information
                var PIInfo = (from p in _context.EXP_LeatherPI.AsEnumerable()
                              where p.PIID == PIID
                              select p).FirstOrDefault();

                model.PIID = Convert.ToInt32(PIInfo.PIID);
                model.PINo = PIInfo.PINo;
                model.PIDate = (Convert.ToDateTime(PIInfo.PIDate)).ToString("dd'/'MM'/'yyyy");
                model.PICategory = PIInfo.PICategory;
                model.BuyerOrderID = Convert.ToInt32(PIInfo.BuyerOrderID);
                model.BuyerOrderNo = (from b in _context.SLS_BuyerOrder
                                      where b.BuyerOrderID == PIInfo.BuyerOrderID
                                      select b.OrderNo).FirstOrDefault();

                model.BuyerID = Convert.ToInt16(PIInfo.BuyerID);
                model.BuyerCode = (from s in _context.Sys_Buyer.AsEnumerable()
                                   where s.BuyerID == PIInfo.BuyerID
                                   select s.BuyerCode).FirstOrDefault();
                
                model.BuyerName = (from s in _context.Sys_Buyer.AsEnumerable()
                                   where s.BuyerID == PIInfo.BuyerID
                                   select s.BuyerName).FirstOrDefault();

                model.BuyerAddress = (from s in _context.Sys_BuyerAddress.AsEnumerable()
                                      where s.BuyerID == PIInfo.BuyerID && s.IsActive == true
                                      select s.Address).FirstOrDefault();

                model.BuyerContactNumber = (from s in _context.Sys_BuyerAddress.AsEnumerable()
                                            where s.BuyerID == PIInfo.BuyerID && s.IsActive == true
                                            select s.ContactNumber).FirstOrDefault();

                model.LocalAgent = Convert.ToInt16(PIInfo.LocalAgent);

                var LocalAgent = (from s in _context.Sys_Buyer.AsEnumerable()
                                  where s.BuyerID == PIInfo.LocalAgent
                                  select s).FirstOrDefault();

                if(LocalAgent!=null)
                {
                    model.LocalAgentCode = LocalAgent.BuyerCode;

                    model.LocalAgentName = LocalAgent.BuyerName;

                    model.LocalAgentAddress = (from s in _context.Sys_BuyerAddress.AsEnumerable()
                                               where s.BuyerID == LocalAgent.BuyerID && s.IsActive == true
                                               select s.Address).FirstOrDefault();

                    model.LocalAgentContactNumber = (from s in _context.Sys_BuyerAddress.AsEnumerable()
                                                     where s.BuyerID == LocalAgent.BuyerID && s.IsActive == true
                                                     select s.ContactNumber).FirstOrDefault();
                }

                

                model.ForeignAgent = Convert.ToInt16(PIInfo.ForeignAgent);


                var ForeignAgent = (from s in _context.Sys_Buyer.AsEnumerable()
                                    where s.BuyerID == PIInfo.ForeignAgent

                                    select s).FirstOrDefault();

                if(ForeignAgent!=null)
                {
                    model.ForeignAgentCode = ForeignAgent.BuyerCode;

                    model.ForeignAgentName = ForeignAgent.BuyerName;

                    model.ForeignAgentAddress = (from s in _context.Sys_BuyerAddress.AsEnumerable()
                                                 where s.BuyerID == ForeignAgent.BuyerID && s.IsActive == true
                                                 select s.Address).FirstOrDefault();

                    model.ForeignAgentContactNumber = (from s in _context.Sys_BuyerAddress.AsEnumerable()
                                                       where s.BuyerID == ForeignAgent.BuyerID && s.IsActive == true
                                                       select s.ContactNumber).FirstOrDefault();
                }

                model.LocalComPercent = PIInfo.LocalComPercent;
                model.ForeignComPercent = PIInfo.ForeignComPercent;
                

                model.BeneficiaryID = Convert.ToInt16(PIInfo.PIBeneficiary);

                model.BeneficiaryCode = (from s in _context.Sys_Supplier.AsEnumerable()
                                         where s.SupplierID == PIInfo.PIBeneficiary
                                         select s.SupplierCode).FirstOrDefault();
                model.BeneficiaryName = (from s in _context.Sys_Supplier.AsEnumerable()
                                         where s.SupplierID == PIInfo.PIBeneficiary
                                         select s.SupplierName).FirstOrDefault();

                model.BeneficiaryAddressID = Convert.ToInt16(PIInfo.BeneficiaryAddressID);
                model.BeneficiaryAddress = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                            where s.SupplierID == PIInfo.PIBeneficiary && s.IsActive && !s.IsDelete
                                            select s.Address).FirstOrDefault();

                model.BeneficiaryContactNumber = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                                  where s.SupplierID == PIInfo.PIBeneficiary && s.IsActive && !s.IsDelete
                                                  select s.ContactNumber).FirstOrDefault();

                
                model.PICurrency = Convert.ToByte(PIInfo.PICurrency);

                model.PriceLevel = PIInfo.PriceLevel;
                model.BuyerBankAccount = PIInfo.BuyerBankAccount;
               

                
                model.GrandTotalSFT = GetGrandTotalSFT(PIID);
                model.GrandTotalSMT = GetGrandTotalSMT(PIID);
                
                model.PaymentMode = PIInfo.PaymentMode;
                model.PaymentNote = PIInfo.PaymentNote;
                
                model.CountryOforigin = Convert.ToInt16(PIInfo.CountryOforigin);
                
                model.DeliveryTerm = PIInfo.DeliveryTerm;
                
                model.PortofLoading = Convert.ToInt16(PIInfo.PortofLoading);
                model.PortofDischarge = Convert.ToInt16(PIInfo.PortofDischarge);
                model.Transshipment = PIInfo.Transshipment;
                model.PartialShipment = PIInfo.PartialShipment;
                model.GoodDelivery = PIInfo.GoodDelivery;
                model.ShippingMark = PIInfo.ShippingMark;
                
                model.Packing = PIInfo.Packing;
                model.PreShipmentIns = PIInfo.PreShipmentIns;
                model.OfferValidityDays = Convert.ToInt16(PIInfo.OfferValidityDays);
                model.OfferValidityNote = PIInfo.OfferValidityNote;
                
                model.RecordStatus = PIInfo.RecordStatus;
                model.ApprovalAdvice = PIInfo.ApprovalAdvice;
                model.BankAccount = PIInfo.BankAccount;


                model.BuyerBank = PIInfo.BuyerBank;
                model.BeneficiaryBank = PIInfo.BeneficiaryBank;


                model.InsuranceTerm = PIInfo.InsuranceTerm;
                model.ShipmentNote = PIInfo.ShipmentNote;
                model.OtherTerm = PIInfo.OtherTerm;
                model.PINotify = PIInfo.PINotify;
                model.PIConsignee = PIInfo.PIConsignee;

                #endregion

                var PIItemList = GetPIItemList(PIID); 
                model.PIItem = PIItemList;

                model.OrderDeliveryDates = BuyerOrderDeliveryListAfterSave(model.BuyerOrderID);
                
                return model;
            }

        }

        public List<EXPPIItem> GetPIItemList(long PIID)
        {
            using (var Context =new BLC_DEVEntities())
            {
                var PIItemList = (from pi in Context.EXP_PIItem.AsEnumerable()
                                  where pi.PIID == PIID

                                  join su in Context.Sys_Unit on (pi.AvgSizeUnit == null ? null : pi.AvgSizeUnit) equals su.UnitID into SizeUnits
                                  from su in SizeUnits.DefaultIfEmpty()

                                  join tu in Context.Sys_Unit on (pi.ThicknessUnit == null ? null : pi.ThicknessUnit) equals tu.UnitID into ThicknessUnits
                                  from tu in ThicknessUnits.DefaultIfEmpty()

                                  join ar in Context.Sys_Article on (pi.ArticleID == null ? null : pi.ArticleID) equals ar.ArticleID into Articles
                                  from ar in Articles.DefaultIfEmpty()

                                  select new EXPPIItem
                                  {
                                      PIID = Convert.ToInt16(pi.PIID),
                                      PIItemID = pi.PIItemID,
                                      HSCode = pi.HSCode,
                                      commodity = pi.Commodity == null ? "" : pi.Commodity,
                                      ArticleID = Convert.ToInt32(pi.ArticleID),
                                      ArticleNo = pi.ArticleNo,
                                      ArticleName = (ar == null ? null : ar.ArticleName),
                                      AvgSize = pi.AvgSize,
                                      AvgSizeUnit = Convert.ToByte(pi.AvgSizeUnit),
                                      AvgSizeUnitName = (su == null ? null : su.UnitName),
                                      SideDescription = pi.SideDescription,
                                      SelectionRange = pi.SelectionRange,
                                      Thickness = pi.Thickness,
                                      //ThicknessAt = (pi.ThicknessAt == "AFSV" ? "After Shaving" : "After Finishing"),
                                      ThicknessUnit = Convert.ToByte(pi.ThicknessUnit),
                                      ThicknessUnitName = (tu == null ? null : tu.UnitName),

                                      ArticleFootQty = pi.ArticleFootQty,
                                      SeaFootUnitPrice = pi.SeaFootUnitPrice,
                                      SeaFootTotalPrice = pi.SeaFootTotalPrice,
                                      AirFootUnitPrice = pi.AirFootUnitPrice,
                                      AirFootTotalPrice = pi.AirMeterTotalPrice,

                                      ArticleMeterQty = pi.ArticleMeterQty,
                                      AirMeterUnitPrice= pi.AirMeterUnitPrice,
                                      AirMeterTotalPrice=pi.AirMeterTotalPrice,
                                      SeaMeterUnitPrice= pi.SeaMeterUnitPrice,
                                      SeaMeterTotalPrice= pi.SeaMeterTotalPrice,
                                  }).ToList();
                return PIItemList;
            }
            

            
        }

        public List<EXPPIItemColor> GetColorListForPIItem(long PIItemID)
        {
            using(var context= new BLC_DEVEntities())
            {
                var Data = (from c in context.EXP_PIItemColor.AsEnumerable()
                            where c.PIItemID == PIItemID

                            join cl in context.Sys_Color on c.ColorID equals cl.ColorID into Colors
                            from cl in Colors.DefaultIfEmpty()
                            select new EXPPIItemColor
                            {
                                PIItemColorID = c.PIItemColorID,
                                ColorID = Convert.ToInt32(c.ColorID),
                                ColorName = (cl == null ? "" : cl.ColorName),
                                PIItemID = PIItemID,

                                ColorMeterQty = (c.ColorMeterQty),
                                SeaMeterUnitPrice = c.SeaMeterUnitPrice,
                                SeaMeterTotalPrice = c.SeaMeterTotalPrice,
                                AirMeterUnitPrice = c.AirMeterUnitPrice,
                                AirMeterTotalPrice = c.AirMeterTotalPrice,

                                ColorFootQty = c.ColorFootQty,
                                SeaFootUnitPrice = c.SeaFootUnitPrice,
                                SeaFootTotalPrice = c.SeaFootTotalPrice,
                                AirFootUnitPrice = c.AirFootUnitPrice,
                                AirFootTotalPrice = c.AirFootTotalPrice
                            }).ToList();

                return Data;
            }
            
        }

        public List<SlsBuyerOrderDeliveryBadhon> BuyerOrderDeliveryListAfterSave(long? _BuyerOrderID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from o in context.SLS_BuyerOrderDelivery.AsEnumerable()
                            where o.BuyerOrderID == _BuyerOrderID & o.IsActive == true

                            join a in context.Sys_Article on o.ArticleID == null ? null : o.ArticleID equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            join c in context.Sys_Color on o.ColorID == null ? null : o.ColorID equals c.ColorID into Colors
                            from c in Colors.DefaultIfEmpty()

                            orderby Convert.ToDateTime(o.PIDeliveryDate)

                            select new SlsBuyerOrderDeliveryBadhon
                            {
                                BuyerOrderDeliveryID = o.BuyerOrderDeliveryID,
                                OrdDeliverySL = o.PIDeliverySL,
                                OrdDeliveryDate = Convert.ToDateTime(o.PIDeliveryDate).ToString("dd'/'MM'/'yyyy"),
                                ArticleID = o.ArticleID == null ? null : o.ArticleID,
                                ArticleName = a == null ? null : a.ArticleName,
                                ColorID = o.ColorID == null ? null : o.ColorID,
                                ColorName = c == null ? null : c.ColorName,
                                OrdDateFootQty = o.PIDateFootQty,
                                OrdDateMeterQty = o.PIDateMeterQty

                            });

                return Data.ToList();
            }


        }

        

        public bool DeletePIItem(string _PIItemID)
        {
            try
            {
                var PIItem = (from c in _context.EXP_PIItem.AsEnumerable()
                                       where (c.PIItemID).ToString() == _PIItemID
                                       select c).FirstOrDefault();

                _context.EXP_PIItem.Remove(PIItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool DeletePIItemColor(string _PIItemColorID)
        {
            try
            {
                var Color = (from c in _context.EXP_PIItemColor.AsEnumerable()
                             where c.PIItemColorID.ToString() == _PIItemColorID
                             select c).FirstOrDefault();

                _context.EXP_PIItemColor.Remove(Color);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteDelivery(string _BuyerOrderDeliveryID)
        {
            try
            {
                var DeliveryDate = (from c in _context.SLS_BuyerOrderDelivery.AsEnumerable()
                                    where c.BuyerOrderDeliveryID.ToString() == _BuyerOrderDeliveryID
                                    select c).FirstOrDefault();

                DeliveryDate.IsActive = false;
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeletePrice(string _BuyerOrderPriceID)
        {
            try
            {
                var DeliveryPrice = (from c in _context.SLS_BuyerOrderPrice.AsEnumerable()
                                     where c.BuyerOrderPriceID.ToString() == _BuyerOrderPriceID
                                     select c).FirstOrDefault();

                DeliveryPrice.IsActive = false;
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeletePI(string _PIID)
        {
            try
            {
                var PI = (from c in _context.EXP_LeatherPI.AsEnumerable()
                             where c.PIID.ToString() == _PIID
                             select c).FirstOrDefault();

                _context.EXP_LeatherPI.Remove(PI);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ConfirmPI(string _PIID, string confirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var DateInfo = (from p in _context.EXP_LeatherPI.AsEnumerable()
                                        where (p.PIID).ToString() == _PIID
                                        select p).FirstOrDefault();
                        DateInfo.ApprovalAdvice = confirmComment;

                        DateInfo.RecordStatus = "CNF";

                        _context.SaveChanges();

                    }
                    Transaction.Complete();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public decimal GetGrandTotalSFT(long  _PIID)
        {
            decimal GrandTotalSFT = 0;
            using(var context=new BLC_DEVEntities())
            {
                var PIItemList = (from i in context.EXP_PIItem.AsEnumerable()
                                  where i.PIID == _PIID
                                  select i).ToList();

                foreach(var item in PIItemList)
                {
                    var PIColorList = (from c in context.EXP_PIItemColor.AsEnumerable()
                                       where c.PIItemID == item.PIItemID
                                       select c).ToList();

                    foreach(var color in PIColorList)
                    {
                        //GrandTotalSFT = Convert.ToDecimal(GrandTotalSFT + color.ColorFootTotalPrice);
                    }
                }
                return GrandTotalSFT;
            }
        }

        public decimal GetGrandTotalSMT(long _PIID)
        {
            decimal GrandTotalSMT = 0;
            using (var context = new BLC_DEVEntities())
            {
                var PIItemList = (from i in context.EXP_PIItem
                                  where i.PIID == _PIID
                                  select i).ToList();

                foreach (var item in PIItemList)
                {
                    var PIColorList = (from c in context.EXP_PIItemColor
                                       where c.PIItemID == item.PIItemID
                                       select c).ToList();

                    foreach (var color in PIColorList)
                    {
                        //GrandTotalSMT = Convert.ToDecimal(GrandTotalSMT + color.ColorMeterTotalPrice);
                    }
                }
                return GrandTotalSMT;
            }
        }


        public List<EXPLeatherPIBadhon> GetAllExportPi()
        {
            using (_context)
            {
                var data = (from o in _context.EXP_LeatherPI.AsEnumerable()
                            //where o.RecordStatus =="CNF"
                            orderby o.PIID descending
                            select new EXPLeatherPIBadhon
                            {
                                PIID = o.PIID,
                                PINo = o.PINo,
                                PIDate = Convert.ToDateTime(o.PIDate).ToString("dd'/'MM'/'yyyy"),
                            });
                return data.ToList();
            }
           
        }
    }
}
