using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PackingListVM
    {
        public int Plid { get; set; }
        public string PlNo { get; set; }
        public string PlDate { get; set; }
        public int? Lcid { get; set; }
        public string LcNo { get; set; }
        public string LcDate { get; set; }
        public int? Ciid { get; set; }
        public string CiNo { get; set; }
        public string CiDate { get; set; }
        public decimal? NetWeight { get; set; }
        public byte? NetWeightUnit { get; set; }
        public decimal? GrossWeight { get; set; }
        public byte? GrossWeightUnit { get; set; }
        public string PlNote { get; set; }
        public string RecordStatus { get; set; }
        public List<PackingListItemVM> packingListItem { get; set; }
        public List<PackingListPacksVM> packingListPacks { get; set; }
    }

    public class PackingListItemVM
    {
        public long PlItemID { get; set; }
        public int? Plid { get; set; }
        public int? ItemID { get; set; } 
        public string ItemName { get; set; } // Item name from table Sys_ChemicalItem
        public string HsCode { get; set; } // HSCode from table Sys_ChemicalItem
        public byte? PackSize { get; set; }
        public string PackSizeName { get; set; } // PackSizeName from table Sys_Size
        public byte? SizeUnit { get; set; }
        public string SizeUnitName { get; set; } // SizeUnitName from table Sys_Unit
        public int? PackQty { get; set; }
        public decimal? PlQty { get; set; }
        public byte? PlUnit { get; set; }
        public string PlUnitName { get; set; } // PlUnitName from table Sys_Unit
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; } // SupplierName from table Sys_Supplier
        public int? ManufacturerID { get; set; }
        public string ManufacturerName { get; set; } // ManufacturerName from table Sys_Supplier
    }

    public class PackingListPacksVM
    {
        public int PlPackID { get; set; }
        public int? Plid { get; set; }
        public byte? PackUnit { get; set; }
        public string PackUnitName { get; set; }
        public int? PackQty { get; set; }
    }
}
