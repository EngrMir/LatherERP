using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ExpExportForm
    {
        public long ExportFormID { get; set; }
        public string ExportFormNo { get; set; }
        public string ExportFormRef { get; set; }
        public string ExportFormRef1 { get; set; }
        public string ExportFormRef2 { get; set; }
        public string ExportFormRef3 { get; set; }
        public string ExportFormDate4 { get; set; }
        public long? LCID { get; set; }
        public long? CIID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public long? PIID { get; set; }
        public long? PLID { get; set; }
        public string PLNo { get; set; }
        public string PLDate { get; set; }
        public string Comodity { get; set; }
        public string ComodityCode { get; set; }
        public int? DestinationCountry { get; set; }
        public string DestinationCountryName { get; set; }
        public int? CountryCode { get; set; }
        public int? DestinationPort { get; set; }
        public string DestinationPortName { get; set; }
        public string PortCode { get; set; }
        public decimal? PackQTy { get; set; }
        public byte? PackUnit { get; set; }
        public string PackUnitName { get; set; }
        public string PackUnitCode { get; set; }
        public decimal? VolumeQty { get; set; }
        public byte? VolumeUnit { get; set; }
        public string VolumeUnitName { get; set; }
        public byte? DeclaredCurrency { get; set; }
        public string DeclaredCurrencyName { get; set; }
        public string DeclaredCurrencyCode { get; set; }
        public decimal? InvoiceValue { get; set; }
        public byte? InvoiceCurrency { get; set; }
        public string InvoiceCurrencyName { get; set; }
        public string DeliveryTerm { get; set; }
        public string TermOfSaleNo { get; set; }
        public string TermSaleDate { get; set; }
        public int? Importer { get; set; }
        public string ImporterName { get; set; }
        public string CarryingVassal { get; set; }
        public string ExportBillDtl { get; set; }
        public int? PortOfShipment { get; set; }
        public string PortOfShipmentName { get; set; }
        public int? LandCustomPort { get; set; }
        public string LandCustomPortName { get; set; }
        public string ShipmentDate { get; set; }
        public int? Exporter { get; set; }
        public string ExporterName { get; set; }
        public string CCIESNo { get; set; }
        public string CCIESDate { get; set; }
        public string Sector { get; set; }
        public string SectorCode { get; set; }
        public string RecordStatus { get; set; }
        public int? DealerID { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public int? DealerAddressID { get; set; }
        public string DealerAddress { get; set; }
    }
}
