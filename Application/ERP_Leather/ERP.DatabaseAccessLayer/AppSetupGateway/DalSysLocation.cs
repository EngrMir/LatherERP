using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysLocation
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long LocationId = 0;

        public DalSysLocation()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysLocation objSysLocation, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var tblSysLocation = new Sys_Location
                {
                    LocationID = objSysLocation.LocationID,
                    LocationName = objSysLocation.LocationName,
                    LocationAddress = objSysLocation.LocationAddress,
                    ContactPerson = objSysLocation.ContactPerson,
                    ContactNumber = objSysLocation.ContactNumber,
                    IsActive = objSysLocation.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = userid,
                    IPAddress = string.Empty
                };

                _context.Sys_Location.Add(tblSysLocation);
                _context.SaveChanges();
                LocationId = tblSysLocation.LocationID;

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Save.";
            }

            return _vmMsg;
        }

        public long GetLocationId()
        {
            return LocationId;
        }

        public ValidationMsg Update(SysLocation objSysLocation, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var location = _context.Sys_Location.FirstOrDefault(s => s.LocationID == objSysLocation.LocationID);
                if (location != null)
                {
                    location.LocationName = objSysLocation.LocationName;
                    location.LocationAddress = objSysLocation.LocationAddress;
                    location.ContactPerson = objSysLocation.ContactPerson;
                    location.ContactNumber = objSysLocation.ContactNumber;
                    location.IsActive = objSysLocation.IsActive == "Active";
                    location.ModifiedOn = DateTime.Now;
                    location.ModifiedBy = userid;
                }

                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public IEnumerable<SysLocation> GetAll()
        {
            IEnumerable<SysLocation> iLstSysLocation = from l in _context.Sys_Location
                                                       where !l.IsDelete
                                                       select new SysLocation
                                                            {
                                                                LocationID = l.LocationID,
                                                                LocationName = l.LocationName,
                                                                LocationAddress = l.LocationAddress,
                                                                ContactPerson = l.ContactPerson,
                                                                ContactNumber = l.ContactNumber,
                                                                IsActive = l.IsActive ? "Active" : "Inactive",
                                                                IsDelete = l.IsDelete,

                                                            };

            return iLstSysLocation;
        }

        public IEnumerable<SysLocation> GetAllActiveLocation()
        {
            IEnumerable<SysLocation> iLstSysLocation = from l in _context.Sys_Location
                                                       where l.IsActive && !l.IsDelete
                                                       select new SysLocation
                                                            {
                                                                LocationID = l.LocationID,
                                                                LocationName = l.LocationName

                                                            };

            return iLstSysLocation;
        }

        public ValidationMsg Delete(string locationId, int userid)
        {
            var locationid = string.IsNullOrEmpty(locationId) ? 0 : Convert.ToInt32(locationId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysLocation = _context.Sys_Location.FirstOrDefault(s => s.LocationID == locationid);
                _context.Sys_Location.Remove(sysLocation);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }
    }
}
