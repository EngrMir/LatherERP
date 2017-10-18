using System;
using System.Globalization;
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
    public class DalProformaInvoice
    {
        private readonly BLC_DEVEntities _context;

        public DalProformaInvoice()
        {
            _context = new BLC_DEVEntities();
        }

        public List<SysBuyer> GetBuyerListForPI()
        {
            using(_context)
            {
                var Data = (from b in _context.Sys_Supplier.AsEnumerable()
                            where b.SupplierCategory == "Chemical"

                            from ba in _context.Sys_SupplierAddress
                            where ba.SupplierID == b.SupplierID && ba.IsActive == true

                            orderby b.SupplierName
                            select new SysBuyer
                            {
                                BuyerID = b.SupplierID,
                                BuyerCode= b.SupplierCode,
                                BuyerName = b.SupplierName,
                                BuyerAddressID= (ba.SupplierAddressID).ToString(),
                                Address= ba.Address,
                                BuyerContactNumber= ba.ContactNumber
                            }).ToList();

                return Data;
            }
           
        }

        public List<PRQChemFrgnPurcOrdr> GetOrderInformationForLOV()
        {
            var Orders = (from o in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                          where o.RecordStatus == "CNF" & o.OrderState=="RNG" & o.OrderCategory=="FPO"

                          from s1 in _context.Sys_Supplier.Where(x => x.SupplierID == o.SupplierID).DefaultIfEmpty()

                          join sa in _context.Sys_SupplierAddress.Where(x=>x.IsActive && !x.IsDelete) on o.SupplierID equals sa.SupplierID into SupplierAddress
                          from sa2 in SupplierAddress.DefaultIfEmpty()

                          from s2 in _context.Sys_Supplier.Where(x => x.SupplierID == o.LocalAgent).DefaultIfEmpty()

                          join la in _context.Sys_SupplierAddress.Where(x=>x.IsActive && !x.IsDelete) on o.LocalAgent equals la.SupplierID into LocalAgentAddress
                          from la2 in LocalAgentAddress.DefaultIfEmpty()

                          from s3 in _context.Sys_Supplier.Where(x => x.SupplierID == o.ForeignAgent).DefaultIfEmpty()

                          join fa in _context.Sys_SupplierAddress.Where(x=>x.IsActive && !x.IsDelete) on o.ForeignAgent equals fa.SupplierID into ForeignAgentAddress
                          from fa2 in ForeignAgentAddress.DefaultIfEmpty()
                          orderby o.OrderID descending
                          select new PRQChemFrgnPurcOrdr
                          {
                              OrderID = o.OrderID,
                              OrderNo = o.OrderNo,
                              OrderTo = DalCommon.ReturnOrderTo(o.OrderTo),
                              OrderDate = (Convert.ToDateTime(o.OrderDate)).ToString("dd'/'MM'/'yyyy"),
                              OrderType = DalCommon.ReturnOrderType(o.OrderType),

                              SupplierID = Convert.ToInt32((s1 == null ? null : (s1.SupplierID).ToString())),
                              SupplierName = (s1 == null ? null : s1.SupplierName),
                              SupplierCode = (s1 == null ? null : s1.SupplierCode),
                              SupplierAddress= (sa2==null? null: sa2.Address),
                              SupplierContactNumber = (sa2 == null ? null : sa2.ContactNumber),


                              LocalAgent = Convert.ToInt32((la2 == null ? null : (la2.SupplierID).ToString())),
                              LocalAgentName = (s2 == null ? null : s2.SupplierName),
                              LocalAgentCode = (s2 == null ? null : s2.SupplierCode),
                              LocalAgentAddress = (la2 == null ? null : la2.Address),
                              LocalAgentContactNumber = (la2 == null ? null : la2.ContactNumber),

                              ForeignAgent = Convert.ToInt32((s3 == null ? null : (s3.SupplierID).ToString())),
                              ForeignAgentName = (s3 == null ? null : s3.SupplierName),
                              ForeignAgentCode = (s3 == null ? null : s3.SupplierCode),
                              ForeignAgentAddress = (fa2 == null ? null : fa2.Address),
                              ForeignAgentContactNumber = (fa2 == null ? null : fa2.ContactNumber),

                              OrderNote = o.OrderNote

                          }).ToList();

            return Orders;
        }

        public List<PRQChemicalPIItem> GetOrderItemListForLOV(string OrderID)
        {
            using(_context)
            {
                var Data = (from i in _context.PRQ_ChemFrgnPurcOrdrItem.AsEnumerable()
                            where (i.OrderID).ToString() == OrderID

                            from ii in _context.Sys_ChemicalItem
                            where ii.ItemID == i.ItemID

                            from u in _context.Sys_Unit.Where(x => x.UnitID == i.OrderUnit).DefaultIfEmpty()
                            from s in _context.Sys_Size.Where(x => x.SizeID == i.PackSize).DefaultIfEmpty()
                            from su in _context.Sys_Unit.Where(x => x.UnitID == i.SizeUnit).DefaultIfEmpty()

                            select new PRQChemicalPIItem
                            {
                                ItemID = i.ItemID,
                                ItemName = ii.ItemName,
                                HSCode = ii.HSCode,
                                OrderQty = Convert.ToDecimal(i.OrderQty),
                                OrderUnitID = i.OrderUnit,
                                OrderUnitName = (u == null ? null : u.UnitName),

                                PackSizeID = i.PackSize,
                                PackSizeName = (s == null ? null : s.SizeName),

                                SizeUnitID = i.SizeUnit,
                                SizeUnitName = (su == null ? null : su.UnitName),

                                PackQty = Convert.ToInt32(i.PackQty),
                                PIQty = Convert.ToDecimal(i.OrderQty),

                                PIUnitID = Convert.ToByte(i.OrderUnit),
                                PIUnitName = (u == null ? null : u.UnitName),
                                SupplierID = Convert.ToInt32(i.SupplierID),
                                //ManufacturerID = Convert.ToInt32(i.ManufacturerID),
                                ItemSource= i.ItemSource

                            }).ToList();

                return Data;
            }
        }

        public int Save(PRQChemicalPI model, int userId, string pageUrl)
        {
            int CurrentPIID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                   
                    using (_context)
                    {
                        
                            #region New_PI_Insert

                            PRQ_ChemicalPI objPI = new PRQ_ChemicalPI();

                            objPI.PINo = model.PINo;
                            objPI.PIDate = DalCommon.SetDate(model.PIDate);
                            objPI.PICategory = model.PICategory;
                            objPI.OrderID = model.OrderID;
                            objPI.OrderNo = model.OrderNo;

                            objPI.PIReceiveDate = DateTime.Now;

                            if (model.SupplierID == 0)
                                objPI.SupplierID = null;
                            else
                                objPI.SupplierID = model.SupplierID;

                            if (model.LocalAgent == 0)
                                objPI.LocalAgent = null;
                            else
                                objPI.LocalAgent = model.LocalAgent;

                            if (model.ForeignAgent == 0)
                                objPI.ForeignAgent = null;
                            else
                                objPI.ForeignAgent = model.ForeignAgent;

                            if (model.BuyerID == 0)
                                objPI.PIBeneficiary = null;
                            else
                                objPI.PIBeneficiary = model.BuyerID;

                            if (model.BuyerAddressID == 0)
                                objPI.BeneficiaryAddressID = null;
                            else
                                objPI.BeneficiaryAddressID = model.BuyerAddressID;

                            if (model.PICurrency == 0)
                                objPI.PICurrency = null;
                            else
                                objPI.PICurrency = model.PICurrency;

                            if (model.ExchangeCurrency == 0)
                                objPI.ExchangeCurrency = null;
                            else
                                objPI.ExchangeCurrency = model.ExchangeCurrency;

                            if (model.ExchangeRate == 0)
                                objPI.ExchangeRate = null;
                            else
                                objPI.ExchangeRate = model.ExchangeRate;

                            if (model.ExchangeValue == 0)
                                objPI.ExchangeValue = null;
                            else
                                objPI.ExchangeValue = model.ExchangeValue;


                            objPI.PaymentTerm = model.PaymentTerm;
                            objPI.PaymentMode = model.PaymentMode;
                            objPI.DeferredDays = model.DeferredDays;


                            if (model.CountryOforigin == 0)
                                objPI.CountryOforigin = null;
                            else
                                objPI.CountryOforigin = model.CountryOforigin;


                            if (model.BeneficiaryBank == 0)
                                objPI.BeneficiaryBank = null;
                            else
                                objPI.BeneficiaryBank = model.BeneficiaryBank;

                            //if (model.AdvisingBank == 0)
                            //    objPI.AdvisingBank = null;
                            //else
                            //    objPI.AdvisingBank = model.AdvisingBank;

                            objPI.FreightCharge = model.FreightCharge;
                            objPI.FreightChargeExtra = model.FreightChargeExtra;

                            objPI.DeliveryTerm = model.DeliveryTerm;
                            objPI.DeliveryMode = model.DeliveryMode;


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

                            if (model.ExpectedShipmentTime == 0)
                                objPI.ExpectedShipmentTime = null;
                            else
                                objPI.ExpectedShipmentTime = model.ExpectedShipmentTime;


                            objPI.Packing = model.Packing;
                            objPI.PreShipmentIns = model.PreShipmentIns;

                            if (model.OfferValidityDays == 0)
                                objPI.OfferValidityDays = null;
                            else
                                objPI.OfferValidityDays = model.OfferValidityDays;


                            objPI.OfferValidityNote = model.OfferValidityNote;
                            objPI.RecordStatus = "NCF";
                            objPI.SetBy = userId;
                            objPI.SetOn = DateTime.Now;

                            objPI.PIStatus = "ODDN";
                            objPI.PIState = "RNG";
                            //objPI.PIReceiveDate=??
                            //objPI.PINote=??
                            //objPI.RunningStatus= ??
                            //objPI.CostIndicator= ??

                            _context.PRQ_ChemicalPI.Add(objPI);
                            _context.SaveChanges();
                            CurrentPIID = objPI.PIID;
                            #endregion

                            #region Item Insert
                            if (model.PIItemList != null)
                            {
                                foreach (var item in model.PIItemList)
                                {
                                    PRQ_ChemicalPIItem objItem = new PRQ_ChemicalPIItem();
                                    objItem.PIID = CurrentPIID;
                                    objItem.ItemID = item.ItemID;
                                    objItem.OrderQty = item.OrderQty;
                                    objItem.OrderUnit = DalCommon.GetUnitCode(item.OrderUnitName);
                                    objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);
                                    objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                    objItem.PackQty = item.PackQty;
                                    objItem.PIQty = Convert.ToDecimal(item.PackSizeName) * item.PackQty;
                                    objItem.PIUnit = DalCommon.GetUnitCode(item.PIUnitName);
                                    objItem.PIUnitPrice = item.PIUnitPrice;
                                    objItem.PITotalPrice = Convert.ToDecimal(item.PackSizeName) * item.PackQty * item.PIUnitPrice;
                                    objItem.SupplierID = item.SupplierID;
                                    //objItem.ManufacturerID = item.ManufacturerID;
                                    objItem.ItemSource = item.ItemSource;
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.PRQ_ChemicalPIItem.Add(objItem);
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
                return CurrentPIID;
            }
        }

        public List<SysBranch> GetAllBranchNameWithBank()
        {
            using (_context)
            {
                var Data = (from b in _context.Sys_Bank.AsEnumerable()
                            where b.BankCategory == "BNK" && b.BankType == "BNF"

                            join br in _context.Sys_Branch on b.BankID equals br.BankID
                            select new SysBranch
                            {
                                BranchID = Convert.ToInt32((br == null ? null : (br.BranchID).ToString())),
                                BranchName = (b.BankName + "," + (br == null ? null : br.BranchName))
                            }).ToList();

                return Data;
            }

        }
        public List<SysBranch> GetAllBranchNameWithBank2()
        {
            using (_context)
            {
                var Data = (from b in _context.Sys_Bank.AsEnumerable()
                            where b.BankCategory == "BNK" && (b.BankType == "BNF" || b.BankType == "ADV")

                            join br in _context.Sys_Branch on b.BankID equals br.BankID
                            select new SysBranch
                            {
                                BranchID = Convert.ToInt32((br == null ? null : (br.BranchID).ToString())),
                                BranchName = (b.BankName + "," + (br == null ? null : br.BranchName))
                            }).ToList();

                return Data;
            }

        }
        

      public List<PRQChemicalPIItem> GetPIItemList(int PIID)
      {
          
          using(var context= new BLC_DEVEntities())
          {
              var PIItemList = (from pi in context.PRQ_ChemicalPIItem.AsEnumerable()
                                where pi.PIID == PIID

                                from i in context.Sys_ChemicalItem
                                where i.ItemID == pi.ItemID

                                from ou in context.Sys_Unit
                                where ou.UnitID == pi.OrderUnit

                                from ps in context.Sys_Size
                                where ps.SizeID == pi.PackSize

                                from su in context.Sys_Unit
                                where su.UnitID == pi.SizeUnit

                                from piu in context.Sys_Unit
                                where piu.UnitID == pi.PIUnit

                                select new PRQChemicalPIItem
                                {
                                    PIID = Convert.ToInt32(pi.PIID),
                                    PIItemID = pi.PIItemID,
                                    ItemID = pi.ItemID,
                                    ItemName = i.ItemName,
                                    HSCode = i.HSCode,
                                    OrderQty = Convert.ToDecimal(pi.OrderQty),
                                    OrderUnitID = pi.OrderUnit,
                                    OrderUnitName = ou.UnitName,
                                    PackSizeID = pi.PackSize,
                                    PackSizeName = ps.SizeName,
                                    SizeUnitID = pi.SizeUnit,
                                    SizeUnitName = su.UnitName,
                                    PackQty = Convert.ToInt32(pi.PackQty),
                                    PIQty = pi.PIQty,
                                    PIUnitID = pi.PIUnit,
                                    PIUnitName = piu.UnitName,
                                    PIUnitPrice = Convert.ToDecimal(pi.PIUnitPrice),
                                    PITotalPrice = Convert.ToDecimal(pi.PITotalPrice),
                                    SupplierID= Convert.ToInt32(pi.SupplierID)
                                }).ToList();

              return PIItemList;
          }
          
      }


        public List<PRQChemicalPI> GetPIInformationForSearch()
        {
            using(_context)
            {
                var Data = (from p in _context.PRQ_ChemicalPI.AsEnumerable()

                            //from s in _context.Sys_Supplier.Where(x => x.SupplierID == p.SupplierID).DefaultIfEmpty()

                            join s in _context.Sys_Supplier on (p.SupplierID==null? null: p.SupplierID) equals s.SupplierID into Suppliers
                            from s in Suppliers.DefaultIfEmpty()

                            //from la in _context.Sys_Supplier.Where(x => x.SupplierID == p.LocalAgent).DefaultIfEmpty()

                            join la in _context.Sys_Supplier on (p.LocalAgent == null ? null : p.SupplierID) equals la.SupplierID into LocalAgents
                            from la in LocalAgents.DefaultIfEmpty()

                            //from fa in _context.Sys_Supplier.Where(x => x.SupplierID == p.ForeignAgent).DefaultIfEmpty()

                            join fa in _context.Sys_Supplier on (p.ForeignAgent == null ? null : p.SupplierID) equals fa.SupplierID into ForeignAgents
                            from fa in ForeignAgents.DefaultIfEmpty()

                            orderby p.PIID descending
                            select new PRQChemicalPI
                            {
                                PIID = p.PIID,
                                PINo = p.PINo,
                                PIDate = (Convert.ToDateTime(p.PIDate)).ToString("dd'/'MM'/'yyyy"),
                                PICategory = (p.PICategory == "PI" ? "Proforma Invoice" : "Indent Order"),
                                SupplierName = (s == null ? null : s.SupplierName),
                                LocalAgentName = (la == null ? null : la.SupplierName),
                                ForeignAgentName = (fa == null ? null : fa.SupplierName),
                                RecordStatus= DalCommon.ReturnRecordStatus(p.RecordStatus)
                            }).ToList();

                return Data;
            }
        }


        public PRQChemicalPI GetPIDetailsAfterSearch(long PIID)
        {
            using(_context)
            {
                var model = new PRQChemicalPI();

                #region PI Information
                var PIInfo = (from p in _context.PRQ_ChemicalPI.AsEnumerable()
                              where p.PIID == PIID
                              select p).FirstOrDefault();

                model.PIID = PIInfo.PIID;
                model.PINo = PIInfo.PINo;
                model.PIDate = (Convert.ToDateTime(PIInfo.PIDate)).ToString("dd'/'MM'/'yyyy");
                model.PICategory = PIInfo.PICategory;
                model.OrderID = PIInfo.OrderID;
                model.OrderNo = PIInfo.OrderNo;


                model.SupplierID = Convert.ToInt32(PIInfo.SupplierID);
                model.SupplierCode = (from s in _context.Sys_Supplier.AsEnumerable()
                                      where s.SupplierID == PIInfo.SupplierID
                                      select s.SupplierCode).FirstOrDefault();

                model.SupplierName = (from s in _context.Sys_Supplier.AsEnumerable()
                                      where s.SupplierID == PIInfo.SupplierID
                                      select s.SupplierName).FirstOrDefault();
                model.SupplierAddress = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                         where s.SupplierID == PIInfo.SupplierID && s.IsActive && !s.IsDelete
                                         select s.Address).FirstOrDefault();
                model.SupplierContactNumber = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                               where s.SupplierID == PIInfo.SupplierID && s.IsActive && !s.IsDelete
                                               select s.ContactNumber).FirstOrDefault();


                model.LocalAgent = Convert.ToInt32(PIInfo.LocalAgent);
                model.LocalAgentCode = (from s in _context.Sys_Supplier.AsEnumerable()
                                        where s.SupplierID == PIInfo.LocalAgent
                                        select s.SupplierCode).FirstOrDefault();

                model.LocalAgentName = (from s in _context.Sys_Supplier.AsEnumerable()
                                        where s.SupplierID == PIInfo.LocalAgent
                                        select s.SupplierName).FirstOrDefault();
                model.LocalAgentAddress = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                           where s.SupplierID == PIInfo.LocalAgent && s.IsActive && !s.IsDelete
                                           select s.Address).FirstOrDefault();
                model.LocalAgentContactNumber = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                                 where s.SupplierID == PIInfo.LocalAgent && s.IsActive && !s.IsDelete
                                                 select s.ContactNumber).FirstOrDefault();


                model.ForeignAgent = Convert.ToInt32(PIInfo.ForeignAgent);
                model.ForeignAgentCode = (from s in _context.Sys_Supplier.AsEnumerable()
                                          where s.SupplierID == PIInfo.ForeignAgent
                                          select s.SupplierCode).FirstOrDefault();

                model.ForeignAgentName = (from s in _context.Sys_Supplier.AsEnumerable()
                                          where s.SupplierID == PIInfo.ForeignAgent
                                          select s.SupplierName).FirstOrDefault();
                model.ForeignAgentAddress = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                             where s.SupplierID == PIInfo.ForeignAgent && s.IsActive && !s.IsDelete
                                             select s.Address).FirstOrDefault();
                model.ForeignAgentContactNumber = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                                   where s.SupplierID == PIInfo.ForeignAgent && s.IsActive && !s.IsDelete
                                                   select s.ContactNumber).FirstOrDefault();

                model.BuyerID = Convert.ToInt32(PIInfo.PIBeneficiary);
                model.BuyerCode = (from s in _context.Sys_Supplier.AsEnumerable()
                                   where s.SupplierID == PIInfo.PIBeneficiary
                                   select s.SupplierCode).FirstOrDefault();
                model.BuyerName = (from s in _context.Sys_Supplier.AsEnumerable()
                                   where s.SupplierID == PIInfo.PIBeneficiary
                                   select s.SupplierName).FirstOrDefault();

                model.BuyerAddressID = Convert.ToInt32(PIInfo.BeneficiaryAddressID);
                model.Address = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                 where s.SupplierID == PIInfo.PIBeneficiary && s.IsActive && !s.IsDelete
                                 select s.Address).FirstOrDefault();

                model.BuyerContactNumber = (from s in _context.Sys_SupplierAddress.AsEnumerable()
                                            where s.SupplierID == PIInfo.PIBeneficiary && s.IsActive && !s.IsDelete
                                            select s.ContactNumber).FirstOrDefault();



                model.ExchangeCurrency = Convert.ToByte(PIInfo.ExchangeCurrency);
                model.PICurrency = Convert.ToByte(PIInfo.PICurrency);
                model.ExchangeRate = Convert.ToDecimal(PIInfo.ExchangeRate);
                model.ExchangeValue = Convert.ToDecimal(PIInfo.ExchangeValue);
                model.PaymentTerm = PIInfo.PaymentTerm;
                model.PaymentMode = PIInfo.PaymentMode;
                model.DeferredDays = PIInfo.DeferredDays;
                model.CountryOforigin = Convert.ToInt32(PIInfo.CountryOforigin);
                model.CountryOforiginName = (from c in _context.Sys_Country
                                             where c.CountryID == PIInfo.CountryOforigin
                                             select c.CountryName).FirstOrDefault();
                model.BeneficiaryBank = Convert.ToInt32(PIInfo.BeneficiaryBank);
                model.BeneficiaryBankName = (from b in _context.Sys_Branch
                                             where b.BranchID == PIInfo.BeneficiaryBank

                                             join ba in _context.Sys_Bank on b.BankID equals ba.BankID into Banks
                                             from ba in Banks.DefaultIfEmpty()
                                             select b.BranchName + "," + ba.BankName).FirstOrDefault();
                model.FreightCharge = PIInfo.FreightCharge;
                model.FreightChargeExtra = PIInfo.FreightChargeExtra;
                model.DeliveryTerm = PIInfo.DeliveryTerm;
                model.DeliveryMode = PIInfo.DeliveryMode;
                model.PortofLoading = Convert.ToInt32(PIInfo.PortofLoading);
                model.PortofLoadingName = (from p in _context.Sys_Port
                                           where p.PortID == PIInfo.PortofLoading
                                           select p.PortName).FirstOrDefault();

                model.PortofDischarge = Convert.ToInt32(PIInfo.PortofDischarge);
                model.PortofDischargeName = (from p in _context.Sys_Port
                                             where p.PortID == PIInfo.PortofDischarge
                                             select p.PortName).FirstOrDefault();
                model.Transshipment = PIInfo.Transshipment;
                model.PartialShipment = PIInfo.PartialShipment;
                model.GoodDelivery = PIInfo.GoodDelivery;
                model.ShippingMark = PIInfo.ShippingMark;
                model.ExpectedShipmentTime = Convert.ToInt32(PIInfo.ExpectedShipmentTime);
                model.Packing = PIInfo.Packing;
                model.PreShipmentIns = PIInfo.PreShipmentIns;
                model.OfferValidityDays = Convert.ToInt32(PIInfo.OfferValidityDays);
                model.OfferValidityNote = PIInfo.OfferValidityNote;
                model.RecordStatus = PIInfo.RecordStatus;
                model.ApprovalAdvice = PIInfo.ApprovalAdvice;
                #endregion



                var PIItemList = (from pi in _context.PRQ_ChemicalPIItem.AsEnumerable()
                                  where pi.PIID == PIID

                                  from i in _context.Sys_ChemicalItem
                                  where i.ItemID== pi.ItemID

                                  from ou in _context.Sys_Unit
                                  where ou.UnitID== pi.OrderUnit

                                  from ps in _context.Sys_Size
                                  where ps.SizeID== pi.PackSize

                                  from su in _context.Sys_Unit
                                  where su.UnitID== pi.SizeUnit

                                  from piu in _context.Sys_Unit
                                  where piu.UnitID== pi.PIUnit

                                  select new PRQChemicalPIItem
                                  {
                                      PIID = Convert.ToInt32(pi.PIID),
                                      PIItemID = pi.PIItemID,
                                      ItemID = pi.ItemID,
                                      ItemName= i.ItemName,
                                      HSCode= i.HSCode,
                                      OrderQty = Convert.ToDecimal(pi.OrderQty),
                                      OrderUnitID= pi.OrderUnit,
                                      OrderUnitName= ou.UnitName,
                                      PackSizeID= pi.PackSize,
                                      PackSizeName= ps.SizeName,
                                      SizeUnitID= pi.SizeUnit,
                                      SizeUnitName= su.UnitName,
                                      PackQty= Convert.ToInt32(pi.PackQty),
                                      PIQty=pi.PIQty,
                                      PIUnitID= pi.PIUnit,
                                      PIUnitName= piu.UnitName,
                                      PIUnitPrice= Convert.ToDecimal(pi.PIUnitPrice),
                                      PITotalPrice = Convert.ToDecimal(pi.PITotalPrice),
                                      SupplierID= Convert.ToInt32(pi.SupplierID)
                                  }).ToList();
                model.PIItemList = PIItemList;


                return model;
            }

        }


        public int Update(PRQChemicalPI model, int userId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {

                        #region PI_Informaiton_Update
                        var currentPI = (from p in _context.PRQ_ChemicalPI.AsEnumerable()
                                         where p.PIID == model.PIID
                                         select p).FirstOrDefault();
                        currentPI.PINo = model.PINo;
                        currentPI.PIDate = DalCommon.SetDate(model.PIDate);
                        currentPI.PICategory = model.PICategory;
                        currentPI.OrderID = model.OrderID;
                        currentPI.OrderNo = model.OrderNo;

                        currentPI.PIReceiveDate = DateTime.Now;

                        if (model.SupplierID == 0)
                            currentPI.SupplierID = null;
                        else
                            currentPI.SupplierID = model.SupplierID;

                        if (model.LocalAgent == 0)
                            currentPI.LocalAgent = null;
                        else
                            currentPI.LocalAgent = model.LocalAgent;

                        if (model.ForeignAgent == 0)
                            currentPI.ForeignAgent = null;
                        else
                            currentPI.ForeignAgent = model.ForeignAgent;

                        if (model.BuyerID == 0)
                            currentPI.PIBeneficiary = null;
                        else
                            currentPI.PIBeneficiary = model.BuyerID;

                        if (model.BuyerAddressID == 0)
                            currentPI.BeneficiaryAddressID = null;
                        else
                            currentPI.BeneficiaryAddressID = model.BuyerAddressID;

                        if (model.PICurrency == 0)
                            currentPI.PICurrency = null;
                        else
                            currentPI.PICurrency = model.PICurrency;

                        if (model.ExchangeCurrency == 0)
                            currentPI.ExchangeCurrency = null;
                        else
                            currentPI.ExchangeCurrency = model.ExchangeCurrency;

                        if (model.ExchangeRate == 0)
                            currentPI.ExchangeRate = null;
                        else
                            currentPI.ExchangeRate = model.ExchangeRate;

                        if (model.ExchangeValue == 0)
                            currentPI.ExchangeValue = null;
                        else
                            currentPI.ExchangeValue = model.ExchangeValue;


                        currentPI.PaymentTerm = model.PaymentTerm;
                        currentPI.PaymentMode = model.PaymentMode;
                        currentPI.DeferredDays = model.DeferredDays;


                        if (model.CountryOforigin == 0)
                            currentPI.CountryOforigin = null;
                        else
                            currentPI.CountryOforigin = model.CountryOforigin;


                        if (model.BeneficiaryBank == 0)
                            currentPI.BeneficiaryBank = null;
                        else
                            currentPI.BeneficiaryBank = model.BeneficiaryBank;

                        currentPI.FreightCharge = model.FreightCharge;
                        currentPI.FreightChargeExtra = model.FreightChargeExtra;

                        //if (model.AdvisingBank == 0)
                        //    currentPI.AdvisingBank = null;
                        //else
                        //    currentPI.AdvisingBank = model.AdvisingBank;


                        currentPI.DeliveryTerm = model.DeliveryTerm;
                        currentPI.DeliveryMode = model.DeliveryMode;


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

                        if (model.ExpectedShipmentTime == 0)
                            currentPI.ExpectedShipmentTime = null;
                        else
                            currentPI.ExpectedShipmentTime = model.ExpectedShipmentTime;


                        currentPI.Packing = model.Packing;
                        currentPI.PreShipmentIns = model.PreShipmentIns;

                        if (model.OfferValidityDays == 0)
                            currentPI.OfferValidityDays = null;
                        else
                            currentPI.OfferValidityDays = model.OfferValidityDays;


                        currentPI.OfferValidityNote = model.OfferValidityNote;
                        currentPI.RecordStatus = "NCF";
                        currentPI.ModifiedBy = userId;
                        currentPI.ModifiedOn = DateTime.Now;

                        _context.SaveChanges();
                        #endregion

                        #region Update_PIItemList
                        if (model.PIItemList != null)
                        {
                            foreach (var PIItem in model.PIItemList)
                            {

                                var checkPIItem = (from i in _context.PRQ_ChemicalPIItem.AsEnumerable()
                                                   where i.PIItemID == PIItem.PIItemID
                                                   select i).Any();

                                #region New_Requisition_Insertion
                                if (!checkPIItem)
                                {
                                    PRQ_ChemicalPIItem objItem = new PRQ_ChemicalPIItem();
                                    objItem.PIID = model.PIID;
                                    objItem.ItemID = PIItem.ItemID;
                                    objItem.OrderQty = PIItem.OrderQty;
                                    objItem.OrderUnit = DalCommon.GetUnitCode(PIItem.OrderUnitName);
                                    objItem.PackSize = DalCommon.GetSizeCode(PIItem.PackSizeName);
                                    objItem.SizeUnit = DalCommon.GetUnitCode(PIItem.SizeUnitName);
                                    objItem.PackQty = PIItem.PackQty;
                                    objItem.PIQty = Convert.ToDecimal(PIItem.PackSizeName) * PIItem.PackQty;
                                    objItem.PIUnit = DalCommon.GetUnitCode(PIItem.PIUnitName);
                                    objItem.PIUnitPrice = PIItem.PIUnitPrice;
                                    objItem.PITotalPrice = Convert.ToDecimal(PIItem.PackSizeName) * PIItem.PackQty * PIItem.PIUnitPrice;
                                    objItem.SupplierID = PIItem.SupplierID;
                                    //objItem.ManufacturerID = item.ManufacturerID;
                                    objItem.ItemSource = PIItem.ItemSource;
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.PRQ_ChemicalPIItem.Add(objItem);
                                    _context.SaveChanges();

                                }
                                #endregion

                                #region Existing_Requisition_Update
                                else
                                {
                                    var FoundPI = (from i in _context.PRQ_ChemicalPIItem.AsEnumerable()
                                                   where i.PIItemID == PIItem.PIItemID
                                                   select i).FirstOrDefault();

                                    FoundPI.PIID = model.PIID;
                                    FoundPI.ItemID = PIItem.ItemID;
                                    FoundPI.OrderQty = PIItem.OrderQty;
                                    FoundPI.OrderUnit = DalCommon.GetUnitCode(PIItem.OrderUnitName);
                                    FoundPI.PackSize = DalCommon.GetSizeCode(PIItem.PackSizeName);
                                    FoundPI.SizeUnit = DalCommon.GetUnitCode(PIItem.SizeUnitName);
                                    FoundPI.PackQty = PIItem.PackQty;

                                    FoundPI.PIQty = Convert.ToDecimal(PIItem.PackSizeName) * PIItem.PackQty;
                                    FoundPI.PIUnit = DalCommon.GetUnitCode(PIItem.PIUnitName);
                                    FoundPI.PIUnitPrice = PIItem.PIUnitPrice;
                                    FoundPI.PITotalPrice = Convert.ToDecimal(PIItem.PackSizeName) * PIItem.PackQty * PIItem.PIUnitPrice;

                                    _context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion
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

        public bool ConfirmPI(string PIID, string confirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var PIInfo = (from p in _context.PRQ_ChemicalPI.AsEnumerable()
                                         where (p.PIID).ToString() == PIID
                                         select p).FirstOrDefault();
                        PIInfo.ApprovalAdvice = confirmComment;

                        PIInfo.RecordStatus = "CNF";

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


        public bool DeletePIItem(string _PIItemID)
        {
            try
            {
                var PIItem = (from c in _context.PRQ_ChemicalPIItem.AsEnumerable()
                                       where c.PIItemID == Convert.ToInt64(_PIItemID)
                                       select c).FirstOrDefault();

                _context.PRQ_ChemicalPIItem.Remove(PIItem);
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
                var PI = (from c in _context.PRQ_ChemicalPI.AsEnumerable()
                             where c.PIID == Convert.ToInt64(_PIID)
                             select c).FirstOrDefault();

                _context.PRQ_ChemicalPI.Remove(PI);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}
