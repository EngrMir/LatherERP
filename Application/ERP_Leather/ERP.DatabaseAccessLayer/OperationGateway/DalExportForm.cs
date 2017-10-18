using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using IronRuby.Compiler.Ast;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalExportForm
    {
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private int _save;
        private int _mode;
        private BLC_DEVEntities _context;

        public DalExportForm()
        {
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _validationMsg = new ValidationMsg();
            _save = 0;
        }

        public ValidationMsg Save(ExpExportForm model, int userId, string url)
        {
            try
            {
                var expFrm = ConvertModel(model, userId, url);
                if (model.ExportFormID == 0)
                {
                    _context.EXP_ExportForm.Add(expFrm);
                    _context.SaveChanges();
                    _mode = 1;
                }
                else
                {
                    _context.SaveChanges();
                    _mode = 2;
                }
                if (_mode == 1)
                {
                    _validationMsg.ReturnId = expFrm.ExportFormID;
                    _validationMsg.ReturnCode = expFrm.ExportFormNo;
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Saved successfully";
                }
                if (_mode == 2)
                {
                    _validationMsg.Type = Enums.MessageType.Update;
                    _validationMsg.Msg = "Updated successfully";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to save";
            }
            return _validationMsg;
        }

        private EXP_ExportForm ConvertModel(ExpExportForm model, int userId, string url)
        {

            var entity = model.ExportFormID == 0 ? new EXP_ExportForm() : (from b in _context.EXP_ExportForm.AsEnumerable()
                                                                          where b.ExportFormID == model.ExportFormID
                                                                          select b).FirstOrDefault();
            entity.DealerID = model.DealerID;
            entity.ExportFormID = model.ExportFormID;
            entity.ExportFormNo = model.ExportFormNo;
            entity.ExportFormDate4 = DalCommon.SetDate(model.ExportFormDate4);
            entity.ExportFormRef = model.ExportFormRef;
            entity.ExportFormRef1 = model.ExportFormRef1;
            entity.ExportFormRef2 = model.ExportFormRef2;
            entity.ExportFormRef3 = model.ExportFormRef3;
            entity.CIID = model.CIID;
            entity.PLID = model.PLID;
            entity.Comodity = model.Comodity;
            entity.ComodityCode = model.ComodityCode;
            entity.DestinationCountry = model.DestinationCountry;
            entity.CountryCode = model.CountryCode;
            entity.DestinationPort = model.DestinationPort;
            entity.PortCode = model.PortCode;
            entity.PackQTy = model.PackQTy;
            entity.PackUnit = model.PackUnit;
            entity.PackUnitCode = model.PackUnitCode;
            entity.VolumeQty = model.VolumeQty;
            entity.VolumeUnit = model.VolumeUnit;
            entity.DeclaredCurrency = model.DeclaredCurrency;
            entity.InvoiceValue = model.InvoiceValue;
            entity.InvoiceCurrency = model.InvoiceCurrency;
            entity.DeliveryTerm = model.DeliveryTerm;
            entity.TermOfSaleNo = model.TermOfSaleNo;
            entity.TermSaleDate = DalCommon.SetDate(model.TermSaleDate);
            entity.Importer = model.Importer;
            entity.CarryingVassal = model.CarryingVassal;
            entity.ExportBillDtl = model.ExportBillDtl;
            entity.PortOfShipment = model.PortOfShipment;
            entity.LandCustomPort = model.LandCustomPort;
            entity.ShipmentDate = DalCommon.SetDate(model.ShipmentDate);
            entity.Exporter = model.Exporter;
            entity.CCIESNo = model.CCIESNo;
            entity.CCIESDate = DalCommon.SetDate(model.CCIESDate);
            entity.Sector = model.Sector;
            entity.SectorCode = model.SectorCode;
            entity.SetOn = model.ExportFormID == 0
                ? DateTime.Now
                : _unit.ExportFormRepository.GetByID(model.ExportFormID).SetOn;
            entity.SetBy = model.ExportFormID == 0
                ? userId
                : _unit.ExportFormRepository.GetByID(model.ExportFormID).SetBy;
            entity.ModifiedOn = model.ExportFormID == 0 ? (DateTime?) null : DateTime.Now;
            entity.ModifiedBy = model.ExportFormID == 0 ? (int?) null : userId;
            entity.RecordStatus = model.RecordStatus ?? "NCF";
            return entity;
        }

        public ValidationMsg Check(long id, int userId)
        {
            try
            {
                var expFrm = _context.EXP_ExportForm.FirstOrDefault(ob => ob.ExportFormID == id);
                if (expFrm != null)
                {
                    expFrm.RecordStatus = "CHK";
                    expFrm.ModifiedBy = userId;
                    expFrm.ModifiedOn = DateTime.Now;
                }

                _context.SaveChanges();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Checked successfully.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to check.";
            }
            return _validationMsg;
        }

        public ValidationMsg Confirm(long id, int userId)
        {
            try
            {
                var expFrm = _context.EXP_ExportForm.FirstOrDefault(ob => ob.ExportFormID == id);
                if (expFrm != null)
                {
                    expFrm.RecordStatus = "CNF";
                    expFrm.ModifiedBy = userId;
                    expFrm.ModifiedOn = DateTime.Now;
                }

                _context.SaveChanges();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Confirmed successfully.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to check.";
            }
            return _validationMsg;
        }

        public ValidationMsg Delete(long id)
        {
            try
            {
                var expFrm = _context.EXP_ExportForm.FirstOrDefault(ob => ob.ExportFormID == id);
                if (expFrm != null)
                {
                    _context.EXP_ExportForm.Remove(expFrm);
                }
                _context.SaveChanges();
                _validationMsg.Type = Enums.MessageType.Success;
                _validationMsg.Msg = "Deleted successfully.";
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete.";
            }
            return _validationMsg;
        }


        public ExpExportForm GetExpFrm(long id)
        {
            var expFrm = _context.EXP_ExportForm.FirstOrDefault(ob => ob.ExportFormID == id);
            var result = new ExpExportForm();
            result.DealerID = expFrm.DealerID;
            result.DealerCode = expFrm.DealerID == null
                ? ""
                : _context.Sys_Branch.FirstOrDefault(ob => ob.BranchID == expFrm.DealerID).BanchCode;
            result.DealerName = expFrm.DealerID == null
                ? ""
                : _context.Sys_Branch.FirstOrDefault(ob => ob.BranchID == expFrm.DealerID).BranchName;
            result.DealerAddress = expFrm.DealerID == null
                ? ""
                : _context.Sys_Branch.FirstOrDefault(ob => ob.BranchID == expFrm.DealerID).Address1;
            result.ExportFormID = expFrm.ExportFormID;
            result.ExportFormNo = expFrm.ExportFormNo;
            result.ExportFormRef = expFrm.ExportFormRef;
            result.ExportFormRef1 = expFrm.ExportFormRef1;
            result.ExportFormRef2 = expFrm.ExportFormRef2;
            result.ExportFormRef3 = expFrm.ExportFormRef3;
            result.ExportFormDate4 = string.Format("{0:dd/MM/yyyy}",expFrm.ExportFormDate4);
            result.CIID = expFrm.CIID;
            result.CINo = expFrm.CIID == null ? "" : _context.EXP_CI.FirstOrDefault(ob => ob.CIID == expFrm.CIID).CINo;
            result.CIDate = expFrm.CIID == null
                ? ""
                : string.Format("{0:dd/MM/yyyy}", _context.EXP_CI.FirstOrDefault(ob => ob.CIID == expFrm.CIID).CIDate);
            result.LCID = expFrm.LCID;
            result.PIID = expFrm.PIID;
            result.PLID = expFrm.PLID;
            result.PLNo = expFrm.PLID == null ? "" : _context.EXP_PackingList.FirstOrDefault(ob => ob.PLID == expFrm.PLID).PLNo;
            result.PLDate = expFrm.PLID == null
                ? ""
                : string.Format("{0:dd/MM/yyyy}",
                    _context.EXP_PackingList.FirstOrDefault(ob => ob.PLID == expFrm.PLID).PLDate);
            result.Comodity = expFrm.Comodity;
            result.ComodityCode = expFrm.ComodityCode;
            result.DestinationCountry = expFrm.DestinationCountry;
            result.DestinationCountryName = expFrm.DestinationCountry == null
                ? ""
                : _context.Sys_Country.FirstOrDefault(ob => ob.CountryID == expFrm.DestinationCountry).CountryName;
            result.CountryCode = expFrm.CountryCode;
            result.DestinationPort = expFrm.DestinationPort;
            result.DestinationPortName = expFrm.DestinationPort == null
                ? ""
                : _context.Sys_Port.FirstOrDefault(ob => ob.PortID == expFrm.DestinationPort).PortName;
            result.PortCode = expFrm.PortCode;
            result.PackQTy = expFrm.PackQTy;
            result.PackUnit = expFrm.PackUnit;
            result.PackUnitName = expFrm.PackUnit == null
                ? ""
                : _context.Sys_Unit.FirstOrDefault(ob => ob.UnitID == expFrm.PackUnit).UnitName;
            result.PackUnitCode = expFrm.PackUnitCode;
            result.VolumeQty = expFrm.VolumeQty;
            result.VolumeUnit = expFrm.VolumeUnit;
            result.VolumeUnitName = expFrm.VolumeUnit == null
                ? ""
                : _context.Sys_Unit.FirstOrDefault(ob => ob.UnitID == expFrm.VolumeUnit).UnitName;
            result.DeclaredCurrency = expFrm.DeclaredCurrency;
            result.DeclaredCurrencyName = expFrm.DeclaredCurrency == null
                ? ""
                : _context.Sys_Currency.FirstOrDefault(ob => ob.CurrencyID == expFrm.DeclaredCurrency).CurrencyName;
            result.InvoiceValue = expFrm.InvoiceValue;
            result.InvoiceCurrency = expFrm.InvoiceCurrency;
            result.InvoiceCurrencyName = expFrm.InvoiceCurrency == null
                ? ""
                : _context.Sys_Currency.FirstOrDefault(ob => ob.CurrencyID == expFrm.InvoiceCurrency).CurrencyName;
            result.DeliveryTerm = expFrm.DeliveryTerm;
            result.TermOfSaleNo = expFrm.TermOfSaleNo;
            result.TermSaleDate = string.Format("{0:dd/MM/yyyy}",expFrm.TermSaleDate);
            result.Importer = expFrm.Importer;
            result.ImporterName = expFrm.Importer == null
                ? ""
                : _context.Sys_Buyer.FirstOrDefault(ob => ob.BuyerID == expFrm.Importer).BuyerName;
            result.CarryingVassal = expFrm.CarryingVassal;
            result.ExportBillDtl = expFrm.ExportBillDtl;
            result.PortOfShipment = expFrm.PortOfShipment;
            result.PortOfShipmentName = expFrm.PortOfShipment == null
                ? ""
                : _context.Sys_Port.FirstOrDefault(ob => ob.PortID == expFrm.PortOfShipment).PortName;
            result.LandCustomPort = expFrm.LandCustomPort;
            result.LandCustomPortName = expFrm.LandCustomPort == null
                ? ""
                : _context.Sys_Port.FirstOrDefault(ob => ob.PortID == expFrm.LandCustomPort).PortName;
            result.ShipmentDate = string.Format("{0:dd/MM/yyyy}", expFrm.ShipmentDate);
            result.Exporter = expFrm.Exporter;
            result.ExporterName = expFrm.Exporter == null
                ? ""
                : _context.Sys_Buyer.FirstOrDefault(ob => ob.BuyerID == expFrm.Exporter).BuyerName;
            result.CCIESNo = expFrm.CCIESNo;
            result.CCIESDate = string.Format("{0:dd/MM/yyyy}",expFrm.CCIESDate);
            result.Sector = expFrm.Sector;
            result.SectorCode = expFrm.SectorCode;
            result.RecordStatus = expFrm.RecordStatus;
            return result;
        }
    }
}
