using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;
using System.Transactions;
using System.Linq;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public static class DalCommon
    {
        public static List<SysStore> GetStoreListForFixedType(string _Type)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from s in context.SYS_Store.AsEnumerable()
                            where s.StoreCategory == _Type && s.StoreType == _Type && s.IsActive && !s.IsDelete
                            select new SysStore
                            {
                                StoreID = s.StoreID,
                                StoreName = s.StoreName
                            }).ToList();

                return Data;
            }

        }

        public static List<SysStore> GetStoreListForFixedCategoryType(string _Category, string _Type)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from s in context.SYS_Store.AsEnumerable()
                            where s.StoreCategory == _Category && s.StoreType == _Type && s.IsActive && !s.IsDelete
                            select new SysStore
                            {
                                StoreID = s.StoreID,
                                StoreName = s.StoreName
                            }).ToList();

                return Data;
            }

        }
        public static byte GetStoreCode(string StoreName)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StoreID = (from s in context.SYS_Store.AsEnumerable()
                               where s.StoreName == StoreName
                               select s.StoreID).FirstOrDefault();

                return StoreID;
            }
        }

        public static string GetStoreName(byte StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StoreName = (from s in context.SYS_Store.AsEnumerable()
                                 where s.StoreID == StoreID
                                 select s.StoreName).FirstOrDefault();

                return StoreName;
            }
        }


        public static byte GetItemTypeCode(string ItemTypeName)
        {
            using (var context = new BLC_DEVEntities())
            {
                var ItemTypeID = (from s in context.Sys_ItemType.AsEnumerable()
                                  where s.ItemTypeName == ItemTypeName
                                  select s.ItemTypeID).FirstOrDefault();

                return ItemTypeID;
            }
        }

        public static string GetItemTypeName(byte ItemTypeID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var ItemTypeName = (from s in context.Sys_ItemType.AsEnumerable()
                                    where s.ItemTypeID == ItemTypeID
                                    select s.ItemTypeName).FirstOrDefault();

                return ItemTypeName;
            }
        }


        public static byte GetSizeCode(string SizeName)
        {
            using (var context = new BLC_DEVEntities())
            {
                var SizeID = (from s in context.Sys_Size.AsEnumerable()
                              where s.SizeName == SizeName
                              select s.SizeID).FirstOrDefault();

                return SizeID;
            }
        }

        public static string GetSizeName(byte SizeID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var SizeName = (from s in context.Sys_Size.AsEnumerable()
                                where s.SizeID == SizeID
                                select s.SizeName).FirstOrDefault();

                return SizeName;
            }
        }


        public static byte GetUnitCode(string UnitName)
        {
            using (var context = new BLC_DEVEntities())
            {
                var UnitID = (from s in context.Sys_Unit.AsEnumerable()
                              where s.UnitName == UnitName
                              select s.UnitID).FirstOrDefault();

                return UnitID;
            }
        }

        public static string GetUnitName(byte UnitID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var UnitName = (from s in context.Sys_Unit.AsEnumerable()
                                where s.UnitID == UnitID
                                select s.UnitName).FirstOrDefault();

                return UnitName;
            }
        }

        public static byte GetLeatherTypeCode(string LeatherType)
        {
            using (var context = new BLC_DEVEntities())
            {
                var LeatherTypeID = (from l in context.Sys_LeatherType.AsEnumerable()
                                     where l.LeatherTypeName == LeatherType
                                     select l.LeatherTypeID).FirstOrDefault();

                return LeatherTypeID;
            }

        }

        public static byte GetLeatherStatusCode(string LeatherType)
        {
            using (var context = new BLC_DEVEntities())
            {
                var LeatherTypeID = (from l in context.Sys_LeatherStatus.AsEnumerable()
                                     where l.LeatherStatusName == LeatherType
                                     select l.LeatherStatusID).FirstOrDefault();

                return LeatherTypeID;
            }

        }

        public static DateTime SetDate(string strDate)
        {
            var newDate = new DateTime();
            if (string.IsNullOrEmpty(strDate)) return newDate;
            var strDatepart = strDate.Substring(0, 10);
            if (strDatepart.Contains('/'))
            {
                var str = strDatepart.Split('/');
                newDate = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]),
                    Convert.ToInt32(str[0]));
            }
            else if (strDatepart.Contains('-'))
            {
                var str = strDatepart.Split('-');
                newDate = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]),
                    Convert.ToInt32(str[0]));
            }
            else
            {
                var str = Regex.Split(strDate, @"\s+");
               newDate = new DateTime(Convert.ToInt32(str[2]), ReturnMonthCode(str[1]),
                  Convert.ToInt32(str[0]));

            }
            return newDate;
        }

        public static string ReturnOrderType(string OrderType)
        {
            if (OrderType == "UR")
                return "Urgent";
            else if (OrderType == "NR")
                return "Normal";
            else if (OrderType == "Urgent")
                return "UR";
            else if (OrderType == "Normal")
                return "NR";
            else
                return "";
        }

        public static string ReturnOrderTo(string OrderTo)
        {
            if (OrderTo == "SP")
                return "Supplier";
            else if (OrderTo == "LA")
                return "Local Agent";
            else if (OrderTo == "FA")
                return "Foreign Agent";
            else if (OrderTo == "Supplier")
                return "SP";
            else if (OrderTo == "Local Agent")
                return "LA";
            else if (OrderTo == "Foreign Agent")
                return "FA";
            else
                return "";

        }

        public static string ReturnRequisitionStatus(string RS)
        {
            if (RS == "RNG")
                return "Running";
            else if (RS == "CNL")
                return "Cancelled";
            else if (RS == "COM")
                return "Completed";
            else if (RS == "Completed")
                return "COM";
            else if (RS == "Cancelled")
                return "CNL";
            else if (RS == "Running")
                return "RNG";
            else
                return "";
        }


        public static string ReturnRequisitionCategory(string RC)
        {
            if (RC == "RPD")
                return "Regular Production";
            else if (RC == "RJO")
                return "Regular Job Order";
            else if (RC == "BOP")
                return "Buyer Order Production";
            else if (RC == "BJO")
                return "Buyer Order Job Order";
            else
                return "";
        }

        public static string ReturnItemSource(string Source)
        {
            if (Source == "Via Requisition")
                return "VR";
            else if (Source == null)
                return "FR";
            else if (Source == "VR")
                return "Via Requisition";
            else if (Source == "FR")
                return "From Order";

            else
                return "";
        }
        public static string ConvertLeatherSizeTextToValue(string sizeQtyRef)
        {

            if (sizeQtyRef == "12-15 sft")
                return "SizeQty1";
            else if (sizeQtyRef == "16-20 sft")
                return "SizeQty2";
            else if (sizeQtyRef == "21-25 sft")
                return "SizeQty3";
            else if (sizeQtyRef == "26-30 sft")
                return "SizeQty4";
            else if (sizeQtyRef == "31-35 sft")
                return "SizeQty5";
            else if (sizeQtyRef == "Side")
                return "SideQty";
            else if (sizeQtyRef == "Area")
                return "AreaQty";
            else
                return "";
        }
        public static string GetPreDefineValue(string groupid, string pageid)
        {
            BLC_DEVEntities _context = new BLC_DEVEntities();
            try
            {
                var preDefineValueForId =
                _context.Sys_PreDefineValueFor.Where(m => m.ConcernPageID == pageid)
                    .FirstOrDefault()
                    .PreDefineValueForID;

                var parameterValue = _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == preDefineValueForId && m.PreDefineValueGroup == groupid && m.IsActive == true).FirstOrDefault() == null ? null : _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == preDefineValueForId && m.PreDefineValueGroup == groupid && m.IsActive == true).FirstOrDefault().MaxValue;

                var syspreDefineValue =
                        _context.Sys_PreDefineValue.FirstOrDefault(s => s.MaxValue == parameterValue);
                if (syspreDefineValue.PreDefineValueIncreaseBy == "FD")
                {
                    var prevalue = parameterValue.Substring(parameterValue.Length - 4);
                    var prestr = parameterValue.Remove(parameterValue.Length - 4);
                    var strlen = (Convert.ToInt64(prevalue) + 1).ToString();
                    if (strlen.Length == 1)
                    {
                        syspreDefineValue.MaxValue = prestr.ToString() + "000" + strlen;
                    }
                    else if (strlen.Length == 2)
                    {
                        syspreDefineValue.MaxValue = prestr.ToString() + "00" + strlen;
                    }
                    else if (strlen.Length == 3)
                    {
                        syspreDefineValue.MaxValue = prestr.ToString() + "0" + strlen;
                    }
                }
                else if (syspreDefineValue.PreDefineValueIncreaseBy == "Y")
                {
                    if (!string.IsNullOrEmpty(syspreDefineValue.PreDefineValueContent))
                    {
                        var prevalue = parameterValue.Substring(parameterValue.Length - 10);

                        if (DateTime.Now.Year.ToString().Substring(2) == prevalue.Remove(2))
                        {
                            var prestr = parameterValue.Remove(parameterValue.Length - 10);
                            syspreDefineValue.MaxValue = prestr.ToString() + (Convert.ToInt64(prevalue) + 1).ToString();
                        }
                        else
                        {
                            syspreDefineValue.MaxValue = syspreDefineValue.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + "00" + "00" + "0001";
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Year.ToString().Substring(2) == parameterValue.Remove(2))
                        {
                            syspreDefineValue.MaxValue = (Convert.ToInt64(parameterValue) + 1).ToString();
                        }
                        else
                        {
                            syspreDefineValue.MaxValue = syspreDefineValue.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + "00" + "00" + "0001";
                        }
                    }
                }
                else if (syspreDefineValue.PreDefineValueIncreaseBy == "M")
                {
                    if (!string.IsNullOrEmpty(syspreDefineValue.PreDefineValueContent))
                    {
                        var prevalue = parameterValue.Substring(parameterValue.Length - 10);

                        if (DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") == prevalue.Remove(4))
                        {
                            var prestr = parameterValue.Remove(parameterValue.Length - 10);
                            syspreDefineValue.MaxValue = prestr.ToString() + (Convert.ToInt64(prevalue) + 1).ToString();
                        }
                        else
                        {
                            syspreDefineValue.MaxValue = syspreDefineValue.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + "00" + "0001";
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") == parameterValue.Remove(4))
                        {
                            syspreDefineValue.MaxValue = (Convert.ToInt64(parameterValue) + 1).ToString();
                        }
                        else
                        {
                            syspreDefineValue.MaxValue = syspreDefineValue.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + "00" + "0001";
                        }
                    }
                    //var prevalue = parameterValue.Substring(parameterValue.Length - 10);
                    //var prestr = parameterValue.Remove(parameterValue.Length - 10);
                    //syspreDefineValue.MaxValue = prestr.ToString() + (Convert.ToInt64(prevalue) + 1).ToString();
                }
                else if (syspreDefineValue.PreDefineValueIncreaseBy == "D")
                {
                    if (!string.IsNullOrEmpty(syspreDefineValue.PreDefineValueContent))
                    {
                        var prevalue = parameterValue.Substring(parameterValue.Length - 10);

                        if (DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") == prevalue.Remove(6))
                        {
                            var prestr = parameterValue.Remove(parameterValue.Length - 10);
                            syspreDefineValue.MaxValue = prestr.ToString() + (Convert.ToInt64(prevalue) + 1).ToString();
                        }
                        else
                        {
                            syspreDefineValue.MaxValue = syspreDefineValue.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + "0001";
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") == parameterValue.Remove(6))
                        {
                            syspreDefineValue.MaxValue = (Convert.ToInt64(parameterValue) + 1).ToString();
                        }
                        else
                        {
                            syspreDefineValue.MaxValue = syspreDefineValue.PreDefineValueContent + DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2) + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + "0001";
                        }
                    }
                }
                else
                {
                    var prevalue = parameterValue.Substring(parameterValue.Length - 10);
                    var prestr = parameterValue.Remove(parameterValue.Length - 10);
                    syspreDefineValue.MaxValue = prestr.ToString() + (Convert.ToInt64(prevalue) + 1).ToString();
                }
                _context.SaveChanges();
                return syspreDefineValue.MaxValue;
            }
            catch
            {
                return null;
            }

        }


        public static string ReturnProductionProcessCategoryName(string _Type)
        {
            if (_Type == "WB")
                return "Wet Blue";
            else if (_Type == "CP")
                return "Crusting";
            else if (_Type == "FN")
                return "Finished";
            else
                return "";
        }

        public static string GetPreDefineNextCodeByUrl(string pageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(pageUrl))
                    return null;

                string scode;
                string pdvGroupId;
                using (var context = new BLC_DEVEntities())
                {
                    scode = context.Screens.FirstOrDefault(s => s.URL.Trim() == pageUrl).ScreenCode.ToString();

                    var pdvId =
                        context.Sys_PreDefineValueFor.FirstOrDefault(s => s.ConcernPageID == scode)
                            .PreDefineValueForID.ToString();

                    var tempPdvId = int.Parse(pdvId);

                    pdvGroupId =
                        context.Sys_PreDefineValue.FirstOrDefault(s => s.PreDefineValueForID == tempPdvId)
                            .PreDefineValueGroup.ToString();
                }
                return GetPreDefineValue(pdvGroupId, scode);
            }
            catch
            {
                return null;
            }

        }

        public static string ReturnRecordStatus(string _Type)
        {
            switch (_Type)
            {
                case "NCF":
                    return "Not Confirmed";
                case "CNF":
                    return "Confirmed";
                case "CHK":
                    return "Checked";
                case "APV":
                    return "Approved";
                case "RCV":
                    return "Received";
                case "ACK":
                    return "Acknowledged";
                default:
                    return "";
            }
        }

        public static string ReturnMonthName(string _MonthName)
        {
            switch (_MonthName)
            {
                case "01":
                    return "January";

                case "02":
                    return "February";

                case "03":
                    return "March";

                case "04":
                    return "April";

                case "05":
                    return "May";

                case "06":
                    return "June";

                case "07":
                    return "July";

                case "08":
                    return "August";

                case "09":
                    return "September";

                case "10":
                    return "October";

                case "11":
                    return "November";

                case "12":
                    return "December";

                default:
                    return "";

            }
        }

        public static int ReturnMonthCode(string name)
        {
            switch (name)
            {
                case "Jan":
                    return 1;

                case "Feb":
                    return 2;

                case "Mar":
                    return 3;

                case "Apr":
                    return 4;

                case "May":
                    return 5;

                case "Jun":
                    return 6;

                case "Jul":
                    return 7;

                case "Aug":
                    return 8;

                case "Sep":
                    return 9;

                case "Oct":
                    return 10;

                case "Nov":
                    return 11;

                case "Dec":
                    return 12;

                default:
                    return 0;

            }
        }

        public static string ReturnChemicalItemCategory(string _Category)
        {
            switch (_Category)
            {
                case "RH":
                    return "Raw Hide";

                case "WB":
                    return "Wet Blue";

                case "FN":
                    return "Finished";

                case "CR":
                    return "Crust";

                case "CM":
                    return "Common";

                default:
                    return "";
            }
        }

        public static string ReturnOrderStatus(string orderStatus)
        {
            switch (orderStatus)
            {
                case "OD":
                    return "Ordered";
                case "CN":
                    return "Canceled";
                case "CM":
                    return "Completed";
                case "OG":
                    return "Ongoing";
                case "RJ":
                    return "Rejected";
                default:
                    return "";
            }
        }

        public static string ReverseOrderStatus(string orderStatus)
        {
            switch (orderStatus)
            {
                case "Ordered":
                    return "OD";
                case "Canceled":
                    return "CN";
                case "Completed":
                    return "CM";
                case "Ongoing":
                    return "OG";
                case "Rejected":
                    return "RJ";
                default:
                    return "";
            }
        }

        public static string ReturnOrderCategory(string orderStatus)
        {
            switch (orderStatus)
            {
                case "FRN":
                    return "Foreign";
                case "LCL":
                    return "Local";
                case "SLF":
                    return "Self";
                default:
                    return "";
            }
        }


        public static string ReturnProductionStatus(string ProductionStatus)
        {
            switch (ProductionStatus)
            {
                case "SCH":
                    return "Schedule";
                case "ONG":
                    return  "On Going";
                case "POS":
                    return  "Postponed";
                case "CMP":
                    return  "Completed";
                default:
                    return  "";
            }
        }

        public static string ReturnBankCategory(string bank)
        {
            switch (bank)
            {
                case "BNK":
                    return "Bank";
                case "INC":
                    return "Insurance";
                default:
                    return "";
            }
        }

        public static string ReturnBankType(string bank)
        {
            switch (bank)
            {
                case "LOC":
                    return "Local";
                case "BNF":
                    return "Benificiary";
                case "ADV":
                    return "Advising";
                default:
                    return "";
            }
        }

        public static string ReturnOrderDeliveryMode(string _DeliveryMode)
        {
            switch (_DeliveryMode)
            {
                case "BA":
                    return "By Air";
                case "BS":
                    return "By Sea";
                case "BR":
                    return "By Road";
                case "By Road":
                    return "BR";
                case "By Sea":
                    return "BS";
                case "By Air":
                    return "BA";
                default:
                    return "";
            }
        }

        public static string ReturnTransHeadType(string _TransHead)
        {
            switch (_TransHead)
            {
                case "LN":
                    return "Loan";
                case "EX":
                    return "Expense";
                case "IN":
                    return "Income";
                case "Income":
                    return "IN";
                case "Expense":
                    return "EX";
                case "Loan":
                    return "LN";
                default:
                    return "";
            }
        }

        public static List<SysProductionProces> GetCategoryWiseProductionProcess(string _Category)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from p in context.Sys_ProductionProces.AsEnumerable()
                            where p.ProcessCategory == _Category
                            select new SysProductionProces
                            {
                                ProcessID = p.ProcessID,
                                ProcessName = p.ProcessName
                            }).ToList();

                return Data;
            }
        }

        public static byte GetCurrencyID(string _CurrencyName)
        {
            using(var context= new BLC_DEVEntities())
            {
                var CurrencyID = (from c in context.Sys_Currency
                                    where c.CurrencyName == _CurrencyName
                                    select c.CurrencyID).FirstOrDefault();

                return CurrencyID;
            }
        }
    }
}
