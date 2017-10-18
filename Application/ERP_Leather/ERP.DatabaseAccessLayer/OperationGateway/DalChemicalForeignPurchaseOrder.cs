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

    public class DalChemicalForeignPurchaseOrder
    {
        private readonly BLC_DEVEntities _context;

        public DalChemicalForeignPurchaseOrder()
        {
            _context = new BLC_DEVEntities();
        }

        public List<SupplierAgentInformation> GetSupplierListForChemicalForeignPurchaseOrder(string _SupplierType)
        {
            var supplier = (from s in _context.Sys_Supplier.AsEnumerable()
                            where s.SupplierCategory == "Chemical" && s.SupplierType == _SupplierType 
                            && s.IsActive && !s.IsDelete

                            join la in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Local Agent" && x.IsActive && !x.IsDelete)
                            on s.SupplierID equals la.SupplierID into LocalAgents
                            from la2 in LocalAgents.DefaultIfEmpty()

                            join lan in _context.Sys_Supplier on (la2 == null ? 0 : la2.AgentID) equals lan.SupplierID into LocalAgentsName
                            from lan2 in LocalAgentsName.DefaultIfEmpty()

                            join fa in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Foreign Agent" && x.IsActive && !x.IsDelete)
                            on s.SupplierID equals fa.SupplierID into ForeignAgents
                            from fa2 in ForeignAgents.DefaultIfEmpty()

                            join fan in _context.Sys_Supplier on (fa2 == null ? 0 : fa2.AgentID) equals fan.SupplierID into ForeignAgentsName
                            from fan2 in ForeignAgentsName.DefaultIfEmpty()


                            orderby s.SupplierName
                            select new SupplierAgentInformation
                            {
                                SupplierID = s.SupplierID.ToString(),
                                SupplierName = s.SupplierName,
                                SupplierCode = s.SupplierCode,

                                LocalAgentID = (la2 == null ? null : la2.AgentID.ToString()),
                                LocalAgentCode = (lan2 == null ? null : lan2.SupplierCode),
                                LocalAgentName = (lan2 == null ? null : lan2.SupplierName),


                                ForeignAgentID = (fa2 == null ? null : fa2.AgentID.ToString()),
                                ForeignAgentCode = (fan2 == null ? null : fan2.SupplierCode),
                                ForeignAgentName = (fan2 == null ? null : fan2.SupplierName)
                            }).ToList();

            return supplier;
        }


        public List<SupplierAgentInformation> GetLocalAgentListForChemicalForeignPurchaseOrder(string _SupplierType)
        {
            var LocalAgent = (from l in _context.Sys_Supplier.AsEnumerable()
                where l.SupplierCategory == "Chemical" && l.SupplierType == "Local Agent" && l.IsActive && !l.IsDelete

                join SuppliersID in _context.Sys_SupplierAgent on l.SupplierID equals SuppliersID.AgentID into Suppliers
                from SuppliersID in Suppliers.DefaultIfEmpty()

                join suppliersDetails in _context.Sys_Supplier.Where(x=>x.SupplierType== _SupplierType) on SuppliersID == null ? 0 : SuppliersID.SupplierID
                    equals suppliersDetails.SupplierID into SupplierDetails
                from SuppliersDetails in SupplierDetails.DefaultIfEmpty()

                join foreignAgent in
                    _context.Sys_SupplierAgent.Where(x => x.AgentType == "Foreign Agent" && x.IsActive && !x.IsDelete)
                    on SuppliersID == null ? 0 : SuppliersID.SupplierID equals foreignAgent.SupplierID into
                    ForeignAgents
                from foreignAgent in ForeignAgents.DefaultIfEmpty()

                join ForeignAgentName in _context.Sys_Supplier on (foreignAgent == null ? 0 : foreignAgent.AgentID)
                    equals ForeignAgentName.SupplierID into
                    ForeignAgentsName
                from ForeignAgentName in ForeignAgentsName.DefaultIfEmpty()

                orderby l.SupplierName
                select new SupplierAgentInformation
                {
                    LocalAgentID = l.SupplierID.ToString(),
                    LocalAgentCode = l.SupplierCode,
                    LocalAgentName = l.SupplierName,

                    SupplierID = SuppliersDetails == null ? null : SuppliersDetails.SupplierID.ToString(),
                    SupplierName = SuppliersDetails == null ? null : SuppliersDetails.SupplierName,
                    SupplierCode = SuppliersDetails == null ? null : SuppliersDetails.SupplierCode,

                    ForeignAgentID = (foreignAgent == null ? null : foreignAgent.AgentID.ToString()),
                    ForeignAgentCode = (ForeignAgentName == null ? null : ForeignAgentName.SupplierCode),
                    ForeignAgentName = (ForeignAgentName == null ? null : ForeignAgentName.SupplierName)
                }).ToList();

            return LocalAgent;
        }

        public List<SupplierAgentInformation> GetForeignAgentListForChemicalForeignPurchaseOrder(string _SupplierType)
        {

            var ForeignAgent = (from l in _context.Sys_Supplier.AsEnumerable()
                where l.SupplierCategory == "Chemical" && l.SupplierType == "Foreign Agent" && l.IsActive && !l.IsDelete

                join SuppliersID in _context.Sys_SupplierAgent on l.SupplierID equals SuppliersID.AgentID into Suppliers
                from SuppliersID in Suppliers.DefaultIfEmpty()

                                join suppliersDetails in _context.Sys_Supplier.Where(x => x.SupplierType == _SupplierType) on SuppliersID == null ? 0 : SuppliersID.SupplierID
                    equals suppliersDetails.SupplierID into SupplierDetails
                from SuppliersDetails in SupplierDetails.DefaultIfEmpty()

                join localAgent in
                    _context.Sys_SupplierAgent.Where(x => x.AgentType == "Local Agent" && x.IsActive && !x.IsDelete)
                    on SuppliersID == null ? 0 : SuppliersID.SupplierID equals localAgent.SupplierID into
                    LocalAgents
                from localAgent in LocalAgents.DefaultIfEmpty()

                join LocalAgentName in _context.Sys_Supplier on (localAgent == null ? 0 : localAgent.AgentID)
                    equals LocalAgentName.SupplierID into
                    LocalAgentsName
                from LocalAgentName in LocalAgentsName.DefaultIfEmpty()

                orderby l.SupplierName
                select new SupplierAgentInformation
                {
                    ForeignAgentID = l.SupplierID.ToString(),
                    ForeignAgentCode = l.SupplierCode,
                    ForeignAgentName = l.SupplierName,

                    SupplierID = SuppliersDetails == null ? null : SuppliersDetails.SupplierID.ToString(),
                    SupplierName = SuppliersDetails == null ? null : SuppliersDetails.SupplierName,
                    SupplierCode = SuppliersDetails == null ? null : SuppliersDetails.SupplierCode,

                    LocalAgentID = (localAgent == null ? null : localAgent.AgentID.ToString()),
                    LocalAgentCode = (LocalAgentName == null ? null : LocalAgentName.SupplierCode),
                    LocalAgentName = (LocalAgentName == null ? null : LocalAgentName.SupplierName)
                }).ToList();

            return ForeignAgent;
        }

        public List<PRQChemFrgnPurcOrdr> GetOrderInformation(string _OrderCategory)
        {
            var Orders = (from o in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                          where o.OrderCategory == _OrderCategory

                          from s1 in _context.Sys_Supplier.Where(x => x.SupplierID == o.SupplierID).DefaultIfEmpty()

                          from s2 in _context.Sys_Supplier.Where(x => x.SupplierID == o.LocalAgent).DefaultIfEmpty()


                          from s3 in _context.Sys_Supplier.Where(x => x.SupplierID == o.ForeignAgent).DefaultIfEmpty()

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

                              LocalAgent = Convert.ToInt32((s2 == null ? null : (s2.SupplierID).ToString())),
                              LocalAgentName = (s2 == null ? null : s2.SupplierName),
                              LocalAgentCode = (s2 == null ? null : s2.SupplierCode),

                              ForeignAgent = Convert.ToInt32((s3 == null ? null : (s3.SupplierID).ToString())),
                              ForeignAgentName = (s3 == null ? null : s3.SupplierName),
                              ForeignAgentCode = (s3 == null ? null : s3.SupplierCode),

                              OrderNote = o.OrderNote,
                              RecordStatus= DalCommon.ReturnRecordStatus(o.RecordStatus)

                          }).ToList();

            return Orders;
        }


        //After Search For Detail Data
        public ChemicalForeignPurchaseOrder GetDetailOrderInformation(string OrderID)
        {
            ChemicalForeignPurchaseOrder model = new ChemicalForeignPurchaseOrder();

            var OrderInformation = (from o in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                                    where (o.OrderID).ToString() == OrderID

                                    from s1 in _context.Sys_Supplier.Where(x => x.SupplierID == o.SupplierID).DefaultIfEmpty()

                                    from s2 in _context.Sys_Supplier.Where(x => x.SupplierID == o.LocalAgent).DefaultIfEmpty()


                                    from s3 in _context.Sys_Supplier.Where(x => x.SupplierID == o.ForeignAgent).DefaultIfEmpty()

                                    select new PRQChemFrgnPurcOrdr
                                    {
                                        OrderID = o.OrderID,
                                        OrderNo = o.OrderNo,
                                        OrderTo = o.OrderTo,
                                        OrderDate = (Convert.ToDateTime(o.OrderDate)).ToString("dd'/'MM'/'yyyy"),
                                        OrderType = o.OrderType,
                                        SupplierID = Convert.ToInt32((s1 == null ? null : (s1.SupplierID).ToString())),
                                        SupplierName = (s1 == null ? null : s1.SupplierName),
                                        SupplierCode = (s1 == null ? null : s1.SupplierCode),
                                        LocalAgent = Convert.ToInt32((s2 == null ? null : (s2.SupplierID).ToString())),
                                        LocalAgentName = (s2 == null ? null : s2.SupplierName),
                                        LocalAgentCode = (s2 == null ? null : s2.SupplierCode),
                                        ForeignAgent = Convert.ToInt32((s3 == null ? null : (s3.SupplierID).ToString())),
                                        ForeignAgentName = (s3 == null ? null : s3.SupplierName),
                                        ForeignAgentCode = (s3 == null ? null : s3.SupplierCode),
                                        OrderNote = o.OrderNote,
                                        RecordStatus= DalCommon.ReturnRecordStatus(o.RecordStatus),
                                        ConfirmComment= o.ApprovalAdvice

                                    }).FirstOrDefault();

            model.OrderInformation = OrderInformation;



            var RequisitionInfo = (from r in _context.PRQ_ChemFrgnPurcOrdrRqsn.AsEnumerable()
                                   where (r.OrderID).ToString() == OrderID

                                   from or in _context.PRQ_ChemPurcReq
                                   where or.RequisitionID == r.RequisitionID

                                   join u in _context.Users on (or.ReqRaisedBy == null ? null : or.ReqRaisedBy) equals u.UserID into Users
                                   from u in Users.DefaultIfEmpty()

                                   select new PRQChemFrgnPurcOrdrRqsn
                                   {
                                       OrederID = r.OrderID,
                                       RequisitionID = r.RequisitionID,
                                       RequisitionNo = or.RequisitionNo,
                                       ReqRaisedOn = (Convert.ToDateTime(or.ReqRaisedOn)).ToString("dd'/'MM'/'yyyy"),
                                       RequisitionType = DalCommon.ReturnOrderType(or.RequisitionType),
                                       ReqRaisedByID = (or.ReqRaisedBy == null ? null : or.ReqRaisedBy).ToString(),
                                       ReqRaisedByName = (u == null ? null : (u.FirstName + " " + u.MiddleName + " " + u.LastName)),
                                       RequisitionStatus = DalCommon.ReturnRequisitionStatus(or.RequisitionStatus)
                                   }).ToList();

            model.RequisitionList = RequisitionInfo;


            if (RequisitionInfo.Count > 0)
            {
                if ((RequisitionInfo.FirstOrDefault().RequisitionID).ToString() != null)
                {
                    var RequisitionItemInfo = (from c in _context.PRQ_ChemFrgnPurcOrdrItem.AsEnumerable().Where(x => (x.OrderID).ToString() == OrderID &&
                                                    x.RequisitionID == RequisitionInfo.FirstOrDefault().RequisitionID)

                                               from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == c.ItemID).DefaultIfEmpty()


                                               join co in _context.PRQ_ChemPurcReqItem on new { c.ItemID, c.RequisitionID } equals new { co.ItemID, co.RequisitionID } into badhon
                                               from item in badhon.DefaultIfEmpty()

                                               join s in _context.Sys_Size on (item == null ? 0 : item.PackSize) equals s.SizeID into badhon2
                                               from finalitem in badhon2.DefaultIfEmpty()

                                               join us in _context.Sys_Unit on (item == null ? 0 : item.SizeUnit) equals us.UnitID into badhon3
                                               from finalitem2 in badhon3.DefaultIfEmpty()

                                               join u in _context.Sys_Unit on (item == null ? 0 : item.SizeUnit) equals u.UnitID into badhon4
                                               from finalitem3 in badhon4.DefaultIfEmpty()

                                               join au in _context.Sys_Unit on (item == null ? 0 : item.SizeUnit) equals au.UnitID into badhon5
                                               from finalitem4 in badhon5.DefaultIfEmpty()

                                               from os in _context.Sys_Size.Where(x => x.SizeID == c.PackSize).DefaultIfEmpty()
                                               from ou in _context.Sys_Unit.Where(x => x.UnitID == c.SizeUnit).DefaultIfEmpty()
                                               from oou in _context.Sys_Unit.Where(x => x.UnitID == c.OrderUnit).DefaultIfEmpty()

                                               select new PRQChemFrgnPurcOrdrItem
                                               {
                                                   OrderItemID = (c == null ? null : c.OrderItemID.ToString()),

                                                   RequisitionID = (c == null ? null : c.RequisitionID.ToString()),
                                                   ItemID = (c == null ? null : c.ItemID.ToString()),
                                                   ItemName = (i == null ? null : i.ItemName),
                                                   RequisitionPackSizeID = (item == null ? null : (item.PackSize).ToString()),
                                                   RequisitionPackSizeName = (finalitem == null ? null : finalitem.SizeName),
                                                   RequisitionPackSizeUnitID = (item == null ? null : (item.SizeUnit).ToString()),
                                                   RequisitionPackSizeUnitName = (finalitem2 == null ? null : finalitem2.UnitName),
                                                   RequisitionPackQty = (item == null ? null : (item.PackQty).ToString()),
                                                   RequsitionQty = (item == null ? null : decimal.Round(item.RequsitionQty,2).ToString()),
                                                   RequisitionUnitID = (item == null ? null : (item.RequisitionUnit).ToString()),

                                                   RequisitionUnitName = (finalitem3 == null ? null : finalitem3.UnitName),

                                                   ApproveQty = (item == null ? null : decimal.Round(Convert.ToDecimal(item.ApproveQty),2).ToString()),
                                                   ApproveUnitID = (item == null ? null : (item.ApproveUnit).ToString()),
                                                   ApproveUnitName = (finalitem4 == null ? null : finalitem4.UnitName),


                                                   OrderPackSizeID = (c == null ? null : c.PackSize.ToString()),
                                                   OrderPackSizeName = (os == null ? null : os.SizeName.ToString()),

                                                   OrderPackSizeUnitID = (c == null ? null : c.SizeUnit.ToString()),
                                                   OrderPackSizeUnitName = (ou == null ? null : ou.UnitName),
                                                   OrderPackQty = (c == null ? null : (c.PackQty).ToString()),

                                                   OrderQty = (c == null ? null : decimal.Round(Convert.ToDecimal(c.OrderQty),2).ToString()),
                                                   OrderUnitID = (c == null ? null : (c.OrderUnit).ToString()),
                                                   OrderUnitName = (oou == null ? null : oou.UnitName),
                                                   ItemSource = (c == null ? null : DalCommon.ReturnItemSource(c.ItemSource))
                                               }).ToList();

                   
                    model.RequisitionItemList = RequisitionItemInfo;
                }
            }

            return model;
        }

        public SupplierAgentInformation GetSupplierAgentInformation(int SupplierID, int LocalAgentID, int ForeignAgentID)
        {
            SupplierAgentInformation model = new SupplierAgentInformation();


            if (SupplierID != 0)
            {
                var Data = (from s in _context.Sys_Supplier.AsEnumerable()
                            where s.SupplierID == SupplierID

                            join la in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Local Agent" && x.IsActive && !x.IsDelete)
                            on s.SupplierID equals la.SupplierID into LocalAgents
                            from la2 in LocalAgents.DefaultIfEmpty()

                            join lan in _context.Sys_Supplier on (la2 == null ? 0 : la2.AgentID) equals lan.SupplierID into LocalAgentsName
                            from lan2 in LocalAgentsName.DefaultIfEmpty()

                            join fa in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Foreign Agent" && x.IsActive && !x.IsDelete)
                            on s.SupplierID equals fa.SupplierID into ForeignAgents
                            from fa2 in ForeignAgents.DefaultIfEmpty()

                            join fan in _context.Sys_Supplier on (fa2 == null ? 0 : fa2.AgentID) equals fan.SupplierID into ForeignAgentsName
                            from fan2 in ForeignAgentsName.DefaultIfEmpty()

                            select new SupplierAgentInformation
                            {
                                SupplierID = s.SupplierID.ToString(),
                                SupplierCode = s.SupplierCode,
                                SupplierName = s.SupplierName,

                                LocalAgentID = (la2 == null ? null : la2.AgentID.ToString()),
                                LocalAgentCode = (lan2 == null ? null : lan2.SupplierCode),
                                LocalAgentName = (lan2 == null ? null : lan2.SupplierName),


                                ForeignAgentID = (fa2 == null ? null : fa2.AgentID.ToString()),
                                ForeignAgentCode = (fan2 == null ? null : fan2.SupplierCode),
                                ForeignAgentName = (fan2 == null ? null : fan2.SupplierName)
                            }).FirstOrDefault();

                model = Data;
                return model;

            }
            else if (LocalAgentID != 0)
            {
                var Data = (from l in _context.Sys_Supplier.AsEnumerable()
                            where l.SupplierID == LocalAgentID

                            join s in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Local Agent" && x.IsActive && !x.IsDelete)
                            on l.SupplierID equals s.AgentID into LocalAgents
                            from s2 in LocalAgents.DefaultIfEmpty()

                            join sup in _context.Sys_Supplier on (s2 == null ? 0 : s2.SupplierID) equals sup.SupplierID into Suppliers
                            from sup2 in Suppliers.DefaultIfEmpty()

                            join fa in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Foreign Agent" && x.IsActive && !x.IsDelete)
                            on (sup2 == null ? 0 : sup2.SupplierID) equals fa.SupplierID into ForeignAgents
                            from fa2 in ForeignAgents.DefaultIfEmpty()

                            join fan in _context.Sys_Supplier on (fa2 == null ? 0 : fa2.AgentID) equals fan.SupplierID into ForeignAgentsName
                            from fan2 in ForeignAgentsName.DefaultIfEmpty()


                            select new SupplierAgentInformation
                            {
                                LocalAgentID = (l.SupplierID).ToString(),
                                LocalAgentCode = l.SupplierCode,
                                LocalAgentName = l.SupplierName,

                                SupplierID = (sup2 == null ? null : (sup2.SupplierID).ToString()),
                                SupplierCode = (sup2 == null ? null : sup2.SupplierCode),
                                SupplierName = (sup2 == null ? null : sup2.SupplierName),

                                ForeignAgentID = (fa2 == null ? null : fa2.AgentID.ToString()),
                                ForeignAgentCode = (fan2 == null ? null : fan2.SupplierCode),
                                ForeignAgentName = (fan2 == null ? null : fan2.SupplierName)
                            }).FirstOrDefault();

                model = Data;
                return model;
            }
            else if (ForeignAgentID != 0)
            {

                var Data = (from l in _context.Sys_Supplier.AsEnumerable()
                            where l.SupplierID == ForeignAgentID

                            join s in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Foreign Agent" && x.IsActive && !x.IsDelete)
                            on l.SupplierID equals s.AgentID into LocalAgents
                            from s2 in LocalAgents.DefaultIfEmpty()

                            join sup in _context.Sys_Supplier on (s2 == null ? 0 : s2.SupplierID) equals sup.SupplierID into Suppliers
                            from sup2 in Suppliers.DefaultIfEmpty()

                            join fa in _context.Sys_SupplierAgent.Where(x => x.AgentType == "Local Agent" && x.IsActive && !x.IsDelete)
                            on (sup2 == null ? 0 : sup2.SupplierID) equals fa.SupplierID into ForeignAgents
                            from fa2 in ForeignAgents.DefaultIfEmpty()

                            join fan in _context.Sys_Supplier on (fa2 == null ? 0 : fa2.AgentID) equals fan.SupplierID into ForeignAgentsName
                            from fan2 in ForeignAgentsName.DefaultIfEmpty()


                            select new SupplierAgentInformation
                            {
                                ForeignAgentID = (l.SupplierID).ToString(),
                                ForeignAgentCode = l.SupplierCode,
                                ForeignAgentName = l.SupplierName,

                                SupplierID = (sup2 == null ? null : (sup2.SupplierID).ToString()),
                                SupplierCode = (sup2 == null ? null : sup2.SupplierCode),
                                SupplierName = (sup2 == null ? null : sup2.SupplierName),

                                LocalAgentID = (fa2 == null ? null : fa2.AgentID.ToString()),
                                LocalAgentCode = (fan2 == null ? null : fan2.SupplierCode),
                                LocalAgentName = (fan2 == null ? null : fan2.SupplierName)
                            }).FirstOrDefault();

                model = Data;
                return model;
            }
            else
            {
                return model;
            }

        }

        public List<PrqChemPurcReq> GetChemicalRequisition(string _RequisitionCategory, string _SupplierID)
        {
            var Data = (from c in _context.PRQ_ChemPurcReq.AsEnumerable()
                        
                        where c.RequisitionState == "RNG" && c.RequisitionCategory == _RequisitionCategory && c.RecordStatus == "APV"

                        join s in _context.Sys_Supplier on c.RequisitionTo equals s.SupplierID into Suppliers
                        from s in Suppliers.DefaultIfEmpty()

                        join u in _context.Users on c.ReqRaisedBy == null ? null : c.ReqRaisedBy equals u.UserID into Users
                        from u in Users.DefaultIfEmpty()

                        
                        orderby c.RequisitionID descending
                        select new PrqChemPurcReq
                        {
                            RequisitionID = c.RequisitionID,
                            RequisitionNo = c.RequisitionNo,
                            ReqRaisedOn = (Convert.ToDateTime(c.ReqRaisedOn)).ToString("dd'/'MM'/'yyyy"),
                            ReqRaisedBy = (c.ReqRaisedBy == null ? null : c.ReqRaisedBy),
                            ReqRaisedByName = (u == null ? null : (u.FirstName + " " + u.MiddleName + " " + u.LastName)),
                            RequisitionType = DalCommon.ReturnOrderType(c.RequisitionType),
                            RecordStatus = DalCommon.ReturnRecordStatus(c.RecordStatus),
                            RequisitionTo = c.RequisitionTo,
                            SupplierName = (s == null ? null : s.SupplierName)
                        }).ToList();

            return Data;
        }

        public List<PRQChemFrgnPurcOrdrItem> GetChemicalRequisitionItemList(string _RequisitionID, string OrderID)
        {
            var RequisitionItemInfo = (from c in _context.PRQ_ChemFrgnPurcOrdrItem.AsEnumerable().Where(x => (x.OrderID).ToString() == OrderID &&
                                                    (x.RequisitionID).ToString() == _RequisitionID)

                                       from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == c.ItemID).DefaultIfEmpty()


                                       join co in _context.PRQ_ChemPurcReqItem on new { c.ItemID, c.RequisitionID } equals new { co.ItemID, co.RequisitionID } into badhon
                                       from item in badhon.DefaultIfEmpty()

                                       join s in _context.Sys_Size on (item == null ? 0 : item.PackSize) equals s.SizeID into badhon2
                                       from finalitem in badhon2.DefaultIfEmpty()

                                       join us in _context.Sys_Unit on (item == null ? 0 : item.SizeUnit) equals us.UnitID into badhon3
                                       from finalitem2 in badhon3.DefaultIfEmpty()

                                       join u in _context.Sys_Unit on (item == null ? 0 : item.SizeUnit) equals u.UnitID into badhon4
                                       from finalitem3 in badhon4.DefaultIfEmpty()

                                       join au in _context.Sys_Unit on (item == null ? 0 : item.SizeUnit) equals au.UnitID into badhon5
                                       from finalitem4 in badhon5.DefaultIfEmpty()

                                       from os in _context.Sys_Size.Where(x => x.SizeID == c.PackSize).DefaultIfEmpty()
                                       from ou in _context.Sys_Unit.Where(x => x.UnitID == c.SizeUnit).DefaultIfEmpty()
                                       from oou in _context.Sys_Unit.Where(x => x.UnitID == c.OrderUnit).DefaultIfEmpty()

                                       select new PRQChemFrgnPurcOrdrItem
                                       {
                                           OrderItemID = (c == null ? null : c.OrderItemID.ToString()),

                                           RequisitionID = (c == null ? null : c.RequisitionID.ToString()),
                                           ItemID = (c == null ? null : c.ItemID.ToString()),
                                           ItemName = (i == null ? null : i.ItemName),
                                           RequisitionPackSizeID = (item == null ? null : (item.PackSize).ToString()),
                                           RequisitionPackSizeName = (finalitem == null ? null : finalitem.SizeName),
                                           RequisitionPackSizeUnitID = (item == null ? null : (item.SizeUnit).ToString()),
                                           RequisitionPackSizeUnitName = (finalitem2 == null ? null : finalitem2.UnitName),
                                           RequisitionPackQty = (item == null ? null : (item.PackQty).ToString()),
                                           RequsitionQty = (item == null ? null : (item.RequsitionQty).ToString()),
                                           RequisitionUnitID = (item == null ? null : (item.RequisitionUnit).ToString()),

                                           RequisitionUnitName = (finalitem3 == null ? null : finalitem3.UnitName),

                                           ApproveQty = (item == null ? null : (item.ApproveQty).ToString()),
                                           ApproveUnitID = (item == null ? null : (item.ApproveUnit).ToString()),
                                           ApproveUnitName = (finalitem4 == null ? null : finalitem4.UnitName),


                                           OrderPackSizeID = (c == null ? null : c.PackSize.ToString()),
                                           OrderPackSizeName = (os == null ? null : os.SizeName.ToString()),

                                           OrderPackSizeUnitID = (c == null ? null : c.SizeUnit.ToString()),
                                           OrderPackSizeUnitName = (ou == null ? null : ou.UnitName),
                                           OrderPackQty = (c == null ? null : (c.PackQty).ToString()),

                                           OrderQty = (c == null ? null : (c.OrderQty).ToString()),
                                           OrderUnitID = (c == null ? null : (c.OrderUnit).ToString()),
                                           OrderUnitName = (oou == null ? null : oou.UnitName),
                                           ItemSource = (c == null ? null : DalCommon.ReturnItemSource(c.ItemSource))
                                       }).ToList();

            return RequisitionItemInfo;
        }

        public List<PRQChemFrgnPurcOrdrItem> GetChemicalRequisitionItemList(string _RequisitionID)
        {
            var Data = (from i in _context.PRQ_ChemPurcReqItem.AsEnumerable()
                        where (i.RequisitionID).ToString() == _RequisitionID

                        from Item in _context.Sys_ChemicalItem
                        where Item.ItemID == i.ItemID

                        join s in _context.Sys_Size on (i.PackSize == null ? null : i.PackSize) equals s.SizeID into PackSize
                        from s in PackSize.DefaultIfEmpty()

                        join us in _context.Sys_Unit on (i.SizeUnit == null ? null : i.SizeUnit) equals us.UnitID into SizeUnit
                        from us in SizeUnit.DefaultIfEmpty()

                        join u in _context.Sys_Unit on (i.RequisitionUnit == 0 ? 0 : i.RequisitionUnit) equals u.UnitID into RequisitionUnit
                        from u in RequisitionUnit.DefaultIfEmpty()

                        join au in _context.Sys_Unit on (i.ApproveUnit == null ? null : i.ApproveUnit) equals au.UnitID into ApproveUnit
                        from au in ApproveUnit.DefaultIfEmpty()

                        select new PRQChemFrgnPurcOrdrItem
                        {
                            RequisitionID = (i.RequisitionID).ToString(),
                            ItemID = (i.ItemID).ToString(),
                            ItemName = Item.ItemName,
                            RequisitionPackSizeID = (s == null ? null : (s.SizeID).ToString()),
                            RequisitionPackSizeName = (s == null ? "" : s.SizeName),
                            RequisitionPackSizeUnitID = (us == null ? null : (us.UnitID).ToString()),
                            RequisitionPackSizeUnitName = (us == null ? "" : us.UnitName),
                            RequisitionPackQty = (i.PackQty).ToString(),
                            RequsitionQty = (Math.Round(i.RequsitionQty, 2)).ToString(),
                            RequisitionUnitID = (u == null ? null : (u.UnitID).ToString()),
                            RequisitionUnitName = (u == null ? "" : u.UnitName),
                            ApproveQty = (decimal.Round(Convert.ToDecimal(i.ApproveQty), 2)).ToString(),
                            ApproveUnitID = (au == null ? null : (au.UnitID).ToString()),
                            //ApproveUnitName = (au == null ? "" : au.UnitName), Approve Unit not present in purchase requisition form

                            //ApproveUnitName = (u == null ? "" : u.UnitName),

                            ItemSource = "Via Requisition",
                            OrderPackSizeID = (s == null ? null : (s.SizeID).ToString()),
                            OrderPackSizeName = (s == null ? "" : s.SizeName),
                            OrderPackSizeUnitID = (us == null ? null : (us.UnitID).ToString()),
                            OrderPackSizeUnitName = (us == null ? "" : us.UnitName),
                            OrderPackQty = (i.PackQty).ToString(),
                            OrderQty = (decimal.Round(Convert.ToDecimal(i.ApproveQty), 2)).ToString(),
                            OrderUnitID = (u == null ? null : (u.UnitID).ToString()),
                            OrderUnitName = (u == null ? "" : u.UnitName),
                        }).ToList();


            return Data;
        }

        public List<SysChemicalItem> GetAllChemicalItem()
        {
            var AllData = (from c in _context.Sys_ChemicalItem.AsEnumerable()
                           where c.IsActive == true

                           join it in _context.Sys_ItemType on c.ItemTypeID equals it.ItemTypeID into Items
                           from it2 in Items.DefaultIfEmpty()

                           orderby c.ItemName
                           select new SysChemicalItem
                           {
                               ItemID = c.ItemID,
                               ItemName = c.ItemName,
                               ItemCategory = DalCommon.ReturnChemicalItemCategory(c.ItemCategory),
                               ItemTypeID = c.ItemTypeID,
                               ItemTypeName = (it2 == null ? null : it2.ItemTypeName),


                           }).ToList();

            return AllData;
        }

        public long Save(ChemicalForeignPurchaseOrder model, int userId, string pageUrl)
        {
            int CurrentOrderID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    string checkOrderNo;
                    if(model.OrderCategory=="FPO")
                    {
                        checkOrderNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                    }
                    else
                    {
                        checkOrderNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalForeignPurchaseOrder/ChemicalLocalPurchaseOrder");
                        //checkOrderNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalForeignPurchaseOrder/ChemicalLocalPurchaseOrder");
                    }
                    
                    if(checkOrderNo != null)
                    {
                        using (_context)
                        {
                            #region New_Order_Insert

                            PRQ_ChemFrgnPurcOrdr objOrder = new PRQ_ChemFrgnPurcOrdr();

                            objOrder.OrderNo = checkOrderNo;
                            objOrder.OrderDate = DalCommon.SetDate(model.OrderDate);
                            objOrder.OrderCategory = model.OrderCategory;
                            objOrder.OrderType = model.OrderType;

                            objOrder.OrderTo = model.OrderTo;

                            objOrder.SupplierID = model.SupplierID;
                            objOrder.LocalAgent = model.LocalAgentID;
                            objOrder.ForeignAgent = model.ForeignAgentID;
                                
                            objOrder.OrderNote = model.OrderNote;
                            objOrder.OrderStatus = "ORIN";
                            objOrder.RecordStatus = "NCF";
                            objOrder.OrderState = "RNG";
                            objOrder.SetOn = DateTime.Now;
                            objOrder.SetBy = userId;

                            _context.PRQ_ChemFrgnPurcOrdr.Add(objOrder);
                            _context.SaveChanges();

                            CurrentOrderID = objOrder.OrderID;
                            #endregion

                            #region Requisition Insert
                            if (model.RequisitionList != null)
                            {
                                PRQ_ChemFrgnPurcOrdrRqsn objRequisition = new PRQ_ChemFrgnPurcOrdrRqsn();

                                objRequisition.OrderID = CurrentOrderID;
                                objRequisition.RequisitionID = model.RequisitionList.FirstOrDefault().RequisitionID;
                                objRequisition.SetOn = DateTime.Now;
                                objRequisition.SetBy = userId;

                                _context.PRQ_ChemFrgnPurcOrdrRqsn.Add(objRequisition);
                                _context.SaveChanges();
                           


                                #region Item Insert
                                if (model.RequisitionItemList != null)
                                {
                                    foreach (var RequisitionItem in model.RequisitionItemList)
                                    {

                                        PRQ_ChemFrgnPurcOrdrItem objRequisitionItem = new PRQ_ChemFrgnPurcOrdrItem();

                                        objRequisitionItem.OrderID = CurrentOrderID;
                                        objRequisitionItem.RequisitionID = Convert.ToInt32(RequisitionItem.RequisitionID);
                                        objRequisitionItem.ItemID = Convert.ToInt32(RequisitionItem.ItemID);

                                        objRequisitionItem.RequsitionQty = Convert.ToDecimal(RequisitionItem.RequsitionQty);

                                        if (RequisitionItem.RequisitionUnitID != null)
                                            objRequisitionItem.RequisitionUnit = Convert.ToByte(RequisitionItem.RequisitionUnitID);
                                        else
                                            objRequisitionItem.RequisitionUnit = null;

                                        objRequisitionItem.PackSize = DalCommon.GetSizeCode(RequisitionItem.OrderPackSizeID);

                                        objRequisitionItem.SizeUnit = DalCommon.GetUnitCode(RequisitionItem.OrderPackSizeUnitID);

                                        objRequisitionItem.PackQty = Convert.ToInt32(RequisitionItem.OrderPackQty);

                                        objRequisitionItem.OrderQty = Convert.ToDecimal(RequisitionItem.OrderQty);
                                        objRequisitionItem.OrderUnit = DalCommon.GetUnitCode(RequisitionItem.OrderUnitID);

                                        objRequisitionItem.ApproveQty = Convert.ToDecimal(RequisitionItem.ApproveQty);

                                        if (RequisitionItem.ApproveUnitID != null)
                                            objRequisitionItem.ApproveUnit = Convert.ToByte(RequisitionItem.ApproveUnitID);
                                        else
                                            objRequisitionItem.ApproveUnit = null;

                                        objRequisitionItem.SupplierID = RequisitionItem.SupplierID;
                                        objRequisitionItem.ItemSource = DalCommon.ReturnItemSource(RequisitionItem.ItemSource);
                                        objRequisitionItem.SetBy = userId;
                                        objRequisitionItem.SetOn = DateTime.Now;

                                        _context.PRQ_ChemFrgnPurcOrdrItem.Add(objRequisitionItem);
                                        _context.SaveChanges();
                                    }
                                }
                                #endregion
                            }
                            #endregion

                        }
                        transaction.Complete();
                    }
                    
                }
                return CurrentOrderID;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        public List<PRQChemFrgnPurcOrdrRqsn> GetRequisitionInformationForSingleOrder(string OrderID)
        {
            var RequisitionList = (from r in _context.PRQ_ChemFrgnPurcOrdrRqsn.AsEnumerable()
                                   where (r.OrderID).ToString() == OrderID

                                   from or in _context.PRQ_ChemPurcReq
                                   where or.RequisitionID == r.RequisitionID

                                   //from u in _context.Users.Where(x => x.UserID == or.ReqRaisedBy).DefaultIfEmpty()

                                   join u in _context.Users on (or.ReqRaisedBy == null ? null : or.ReqRaisedBy) equals u.UserID into Users
                                   from u in Users.DefaultIfEmpty()

                                   select new PRQChemFrgnPurcOrdrRqsn
                                   {
                                       OrederID= r.OrderID,
                                       RequisitionID = r.RequisitionID,
                                       RequisitionNo = or.RequisitionNo,
                                       ReqRaisedOn = (Convert.ToDateTime(or.ReqRaisedOn)).ToString("dd'/'MM'/'yyyy"),
                                       RequisitionType = DalCommon.ReturnOrderType(or.RequisitionType),
                                       ReqRaisedByID = (or.ReqRaisedBy == null ? null : or.ReqRaisedBy).ToString(),
                                       ReqRaisedByName = (u == null ? null : (u.FirstName + " " + u.MiddleName + " " + u.LastName)),
                                       RequisitionStatus = DalCommon.ReturnRequisitionStatus(or.RequisitionStatus)


                                   }).ToList();

            return RequisitionList;
        }


        public int UpdateChemicalPurchaseOrderInformation(ChemicalForeignPurchaseOrder model, int userId)
        {
            //var CurrentRequisitionID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {

                        #region Order_Informaiton_Update
                        var currentOrder = (from p in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                                            where p.OrderID == Convert.ToInt64(model.OrderID)
                                            select p).FirstOrDefault();

                        currentOrder.OrderDate = DalCommon.SetDate(model.OrderDate);
                        //currentOrder.OrderCategory = "FPO";
                        currentOrder.OrderType = model.OrderType;
                        //objOrder.OrderFrom = ??;
                        currentOrder.OrderTo = model.OrderTo;

                        currentOrder.SupplierID = model.SupplierID;
                        currentOrder.LocalAgent = model.LocalAgentID;
                        currentOrder.ForeignAgent = model.ForeignAgentID;

                       
                        currentOrder.OrderNote = model.OrderNote;
                        currentOrder.ModifiedOn = DateTime.Now;
                        currentOrder.ModifiedBy = userId;

                        _context.SaveChanges();
                        #endregion

                        #region Update_Requisition_Information
                        if (model.RequisitionList != null)
                        {
                            foreach (var Requisition in model.RequisitionList)
                            {
                                #region New_Requisition_Insertion
                                var checkRequisition = (from rq in _context.PRQ_ChemFrgnPurcOrdrRqsn.AsEnumerable()
                                                        where (rq.OrderID).ToString() == model.OrderID && rq.RequisitionID == Requisition.RequisitionID
                                                        select rq).Any();


                                if (!checkRequisition)
                                {
                                    PRQ_ChemFrgnPurcOrdrRqsn objRequisition = new PRQ_ChemFrgnPurcOrdrRqsn();

                                    objRequisition.OrderID = Convert.ToInt32(model.OrderID);
                                    objRequisition.RequisitionID = Requisition.RequisitionID;
                                    //objRequisition.RequisitionState = ??
                                    objRequisition.SetOn = DateTime.Now;
                                    objRequisition.SetBy = userId;

                                    _context.PRQ_ChemFrgnPurcOrdrRqsn.Add(objRequisition);
                                    _context.SaveChanges();

                                    //CurrentRequisitionID = Requisition.RequisitionID;
                                }
                                #endregion

                                #region Existing_Requisition_Update
                                else
                                {
                                    var LatestRequisition = (from rq in _context.PRQ_ChemFrgnPurcOrdrRqsn.AsEnumerable()
                                                             where (rq.OrderID).ToString() == model.OrderID && rq.RequisitionID == Requisition.RequisitionID
                                                             select rq).FirstOrDefault();

                                    //currentRequisition.OrderID = Convert.ToInt32(model.OrderID);
                                    //currentRequisition.RequisitionID = model.RequisitionList.FirstOrDefault().RequisitionID;
                                    //objRequisition.RequisitionState = ??
                                    LatestRequisition.ModifiedOn = DateTime.Now;
                                    LatestRequisition.ModifiedBy = userId;

                                    _context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion


                        #region Update_Requisition_Item_Information
                        if (model.RequisitionItemList != null)
                        {
                            foreach (var RequisitionItem in model.RequisitionItemList)
                            {
                                #region New_Requisition_Item_Insertion
                                if (RequisitionItem.OrderItemID == null)
                                {
                                    PRQ_ChemFrgnPurcOrdrItem objRequisitionItem = new PRQ_ChemFrgnPurcOrdrItem();

                                    objRequisitionItem.OrderID = Convert.ToInt32(model.OrderID);
                                    objRequisitionItem.RequisitionID = Convert.ToInt32(RequisitionItem.RequisitionID);
                                    objRequisitionItem.ItemID = Convert.ToInt32(RequisitionItem.ItemID);

                                    objRequisitionItem.RequsitionQty = Convert.ToDecimal(RequisitionItem.RequsitionQty);

                                    if (RequisitionItem.RequisitionUnitID != null)
                                        objRequisitionItem.RequisitionUnit = Convert.ToByte(RequisitionItem.RequisitionUnitID);
                                    else
                                        objRequisitionItem.RequisitionUnit = null;
                                    //objRequisitionItem.RequisitionUnit = Convert.ToByte(RequisitionItem.RequisitionUnitID);

                                    objRequisitionItem.PackSize = DalCommon.GetSizeCode(RequisitionItem.OrderPackSizeID);

                                    objRequisitionItem.SizeUnit = DalCommon.GetUnitCode(RequisitionItem.OrderPackSizeUnitID);

                                    objRequisitionItem.PackQty = Convert.ToInt32(RequisitionItem.OrderPackQty);

                                    objRequisitionItem.OrderQty = Convert.ToDecimal(RequisitionItem.OrderQty);
                                    objRequisitionItem.OrderUnit = DalCommon.GetUnitCode(RequisitionItem.OrderUnitID);

                                    objRequisitionItem.ApproveQty = Convert.ToDecimal(RequisitionItem.ApproveQty);

                                    if (RequisitionItem.ApproveUnitID != null)
                                        objRequisitionItem.ApproveUnit = Convert.ToByte(RequisitionItem.ApproveUnitID);
                                    else
                                        objRequisitionItem.ApproveUnit = null;

                                    objRequisitionItem.SupplierID = RequisitionItem.SupplierID;
                                    objRequisitionItem.ItemSource = DalCommon.ReturnItemSource(RequisitionItem.ItemSource);




                                    objRequisitionItem.ModifiedBy = userId;
                                    objRequisitionItem.ModifiedOn = DateTime.Now;

                                    _context.PRQ_ChemFrgnPurcOrdrItem.Add(objRequisitionItem);
                                    _context.SaveChanges();
                                }
                                #endregion

                                #region Update_Existing_Requisition_Item
                                else
                                {
                                    var CurrentRequisitionItem = (from ri in _context.PRQ_ChemFrgnPurcOrdrItem.AsEnumerable()
                                                                  where ri.OrderItemID == Convert.ToInt32(RequisitionItem.OrderItemID)
                                                                  select ri).FirstOrDefault();

                                    //CurrentRequisitionItem.OrderID = Convert.ToInt32(model.OrderID);
                                    //CurrentRequisitionItem.RequisitionID = Convert.ToInt32(RequisitionItem.RequisitionID);
                                    CurrentRequisitionItem.ItemID = Convert.ToInt32(RequisitionItem.ItemID);

                                    CurrentRequisitionItem.RequsitionQty = Convert.ToDecimal(RequisitionItem.RequsitionQty);

                                    if (RequisitionItem.RequisitionUnitID != null)
                                        CurrentRequisitionItem.RequisitionUnit = Convert.ToByte(RequisitionItem.RequisitionUnitID);
                                    else
                                        CurrentRequisitionItem.RequisitionUnit = null;

                                    CurrentRequisitionItem.PackSize = DalCommon.GetSizeCode(RequisitionItem.OrderPackSizeID);
                                    CurrentRequisitionItem.SizeUnit = DalCommon.GetUnitCode(RequisitionItem.OrderPackSizeUnitID);
                                    CurrentRequisitionItem.PackQty = Convert.ToInt32(RequisitionItem.OrderPackQty);
                                    CurrentRequisitionItem.OrderQty = Convert.ToDecimal(RequisitionItem.OrderQty);
                                    CurrentRequisitionItem.OrderUnit = DalCommon.GetUnitCode(RequisitionItem.OrderUnitID);
                                    CurrentRequisitionItem.ApproveQty = Convert.ToDecimal(RequisitionItem.ApproveQty);
                                    if (RequisitionItem.ApproveUnitID != null)
                                        CurrentRequisitionItem.ApproveUnit = Convert.ToByte(RequisitionItem.ApproveUnitID);
                                    else
                                        CurrentRequisitionItem.ApproveUnit = null;
                                    CurrentRequisitionItem.SupplierID = RequisitionItem.SupplierID;
                                    CurrentRequisitionItem.ItemSource = DalCommon.ReturnItemSource(RequisitionItem.ItemSource);
                                    CurrentRequisitionItem.ModifiedBy = userId;
                                    CurrentRequisitionItem.ModifiedOn = DateTime.Now;

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

        public string GetOrderNo(long OrderID)
        {
            var OrderNo = (from o in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                           where o.OrderID == OrderID
                           select o.OrderNo).FirstOrDefault();

            return OrderNo;
        }

        public List<SysSize> GetAllPackSizeForChemical()
        {
            var Data = (from s in _context.Sys_Size.AsEnumerable()
                        where s.SizeCategory == "ChemicalPack" && s.IsActive  && !s.IsDelete
                        orderby Convert.ToInt32(s.SizeName) 
                        select new SysSize
                        {
                            SizeID = s.SizeID,
                            SizeName = s.SizeName
                        }).ToList();

            return Data;
        }

        public List<SysUnit> GetAllUnitForChemicalPackSize()
        {
            var Data = (from s in _context.Sys_Unit
                        where s.UnitCategory == "ChemicalPack"
                        orderby s.UnitName
                        select new SysUnit
                        {
                            UnitID = s.UnitID,
                            UnitName = s.UnitName
                        }).ToList();

            return Data;
        }

        public List<SysUnit> GetAllUnitForChemical()
        {
            var Data = (from s in _context.Sys_Unit
                        where s.UnitCategory == "Chemical" || s.UnitCategory == "Common"
                        orderby s.UnitName
                        select new SysUnit
                        {
                            UnitID = s.UnitID,
                            UnitName = s.UnitName
                        }).ToList();

            return Data;
        }

        public bool ConfirmOrder(string OrderID, string confirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var OrderInfo = (from p in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                                            where (p.OrderID).ToString() == OrderID
                                            select p).FirstOrDefault();
                        OrderInfo.ApprovalAdvice = confirmComment;

                        OrderInfo.RecordStatus = "CNF";

                        _context.SaveChanges();


                        var RequisitionList = (from r in _context.PRQ_ChemFrgnPurcOrdrRqsn.AsEnumerable()
                                               where r.OrderID.ToString() == OrderID
                                               select r).ToList();

                        foreach(var rec in RequisitionList)
                        {
                            var Requisition = (from r in _context.PRQ_ChemPurcReq.AsEnumerable()
                                               where r.RequisitionID == rec.RequisitionID
                                               select r).FirstOrDefault();

                            Requisition.RequisitionState = "CMP";
                            Requisition.RequisitionStatus = "RFO";
                            _context.SaveChanges();
                        }

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

        public bool DeleteRequisitionItem(string OrderItemID)
        {
            try
            {
                var RequisitionItem = (from c in _context.PRQ_ChemFrgnPurcOrdrItem.AsEnumerable()
                                       where c.OrderItemID == Convert.ToInt64(OrderItemID)
                                   select c).FirstOrDefault();

                _context.PRQ_ChemFrgnPurcOrdrItem.Remove(RequisitionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteRequisition(string OrderID, string RequisitionID)
        {
            try
            {
                var Requisition = (from c in _context.PRQ_ChemFrgnPurcOrdrRqsn.AsEnumerable()
                               where c.OrderID == Convert.ToInt64(OrderID) && (c.RequisitionID).ToString() == RequisitionID
                               select c).FirstOrDefault();

                _context.PRQ_ChemFrgnPurcOrdrRqsn.Remove(Requisition);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteOrder(string OrderID)
        {
            try
            {
                var Order = (from c in _context.PRQ_ChemFrgnPurcOrdr.AsEnumerable()
                             where c.OrderID == Convert.ToInt64(OrderID)
                             select c).FirstOrDefault();

                _context.PRQ_ChemFrgnPurcOrdr.Remove(Order);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ConsealRequisition(string _RequisitionID)
        {
            try
            {
                var Requisition = (from c in _context.PRQ_ChemPurcReq.AsEnumerable()
                             where c.RequisitionID == Convert.ToInt64(_RequisitionID)
                             select c).FirstOrDefault();

                Requisition.RequisitionState = "COM";
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
