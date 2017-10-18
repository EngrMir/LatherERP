using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalExpTransportChallan
    {
        private readonly BLC_DEVEntities _context;
        private UnitOfWork _unit;
        private ValidationMsg _msg;
        private int _mode;

        public DalExpTransportChallan()
        {
            _context = new BLC_DEVEntities();
            _msg = new ValidationMsg();
            _unit = new UnitOfWork();
            _mode = new int();
        }

        public ValidationMsg Save(ExpTransportChallan model, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var transChln = ConvertTransportChallan(model, userId);
                        if (model.TransportChallanID == 0)
                        {
                            _context.EXP_TransportChallan.Add(transChln);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.ChallanCis != null)
                        {
                            foreach (var ci in model.ChallanCis)
                            {
                                var transChlnCi = ConverTransportChallanCi(ci, transChln.TransportChallanID,
                                    userId, transChln.RecordStatus);
                                if (ci.TransportChallanID == 0)
                                {
                                    _context.EXP_TransportChallanCI.Add(transChlnCi);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                            }
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _msg.ReturnId = transChln.TransportChallanID;
                            _msg.ReturnCode = transChln.TransportChallanNo;
                            _msg.Type = Enums.MessageType.Success;
                            _msg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _msg.Type = Enums.MessageType.Update;
                            _msg.Msg = "Updated successfully.";
                        }
                    }
                }
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to save.";
            }

            return _msg;
        }

        public ValidationMsg Confirm(long id, int userId, string note)
        {
            try
            {
                var tranChln = _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == id);
                if (tranChln != null)
                {
                    tranChln.RecordStatus = "CNF";
                    tranChln.ConfirmeDate = DateTime.Now;
                    tranChln.ConfirmedBy = userId;
                    tranChln.ConfirmNote = note;
                    _context.SaveChanges();
                    _msg.Type = Enums.MessageType.Success;
                    _msg.Msg = "Confirmed successfully.";
                }
                else
                {
                    _msg.Type = Enums.MessageType.Error;
                    _msg.Msg = "Failed to confirm.";
                }

            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to confirm.";
            }
            return _msg;
        }

        public ValidationMsg Delete(long id)
        {
            var rem = new EXP_TransportChallanCI();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var transChln = _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == id);
                        if (transChln != null)
                        {
                            var transChlnCi =
                                _context.EXP_TransportChallanCI.Where(ob => ob.TransportChallanID == id).ToList();
                            if (transChlnCi.Count > 0)
                            {
                                foreach (var ci in transChlnCi)
                                {
                                    var expCi = _context.EXP_CI.FirstOrDefault(ob => ob.CIID == ci.CIID);
                                    if (expCi != null)
                                    {
                                        _context.EXP_TransportChallanCI.Remove(ci);
                                    }
                                    else
                                    {
                                        _msg.Type = Enums.MessageType.Error;
                                        _msg.Msg = "Relation exists with commercial invoice.";
                                        return _msg;
                                    }
                                }

                            }
                            _context.EXP_TransportChallan.Remove(transChln);
                        }
                        _context.SaveChanges();
                        tx.Complete();
                        _msg.Type = Enums.MessageType.Delete;
                        _msg.Msg = "Deleted successfully.";
                    }
                }
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to delete.";
            }
            return _msg;
        }

        public ValidationMsg DeleteTrnsChlnCi(long trnsId, long ciId)
        {
            var rem = new EXP_TransportChallanCI();
            try
            {
                using (_context)
                {
                    var transChlnCi =
                        _context.EXP_TransportChallanCI.FirstOrDefault(
                            ob => ob.TransportChallanID == trnsId && ob.CIID == ciId);
                    if (transChlnCi != null)
                    {
                        //var expCi = _context.EXP_CI.FirstOrDefault(ob => ob.CIID == transChlnCi.CIID);
                        //if (expCi != null)
                        //{
                            _context.EXP_TransportChallanCI.Remove(transChlnCi);
                        //}
                        //else
                        //{
                        //    _msg.Type = Enums.MessageType.Error;
                        //    _msg.Msg = "Relation exists with commercial invoice.";
                        //    return _msg;
                        //}
                    }
                    _context.SaveChanges();
                    
                    _msg.Type = Enums.MessageType.Delete;
                    _msg.Msg = "Deleted successfully.";
                }
            }
            catch
            {
                _msg.Type = Enums.MessageType.Error;
                _msg.Msg = "Failed to delete.";
            }
            return _msg;
        }

        private EXP_TransportChallan ConvertTransportChallan(ExpTransportChallan model, int userId)
        {
            var entity = model.TransportChallanID == 0
                ? new EXP_TransportChallan()
                : (from b in _context.EXP_TransportChallan.AsEnumerable()
                   where b.TransportChallanID == model.TransportChallanID
                   select b).FirstOrDefault();

            entity.TransportChallanID = model.TransportChallanID;
            entity.TransportChallanNo = model.TransportChallanNo;
            entity.TransportChallanDate = DalCommon.SetDate(model.TransportChallanDate);
            entity.TransportChallanRef = model.TransportChallanRef;
            entity.TransportChallanNote = model.TransportChallanNote;
            entity.DeliverChallanID = model.DeliverChallanID;
            entity.Sender = model.Sender;
            entity.SenderAddress = model.SenderAddress;
            entity.Receiver = model.Receiver;
            entity.ReceverAddress = model.ReceverAddress;
            entity.DeliveryFrom = model.DeliveryFrom;
            entity.DEliveryTo = model.DEliveryTo;
            entity.RecordStatus = model.RecordStatus ?? "NCF";
            entity.SetBy = model.TransportChallanID == 0
                ? userId
                : _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == model.TransportChallanID).SetBy;
            entity.SetOn = model.TransportChallanID == 0
                ? DateTime.Now
                : _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == model.TransportChallanID).SetOn;
            entity.ModifiedBy = model.TransportChallanID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.TransportChallanID == 0 ? (DateTime?)null : DateTime.Now;
            return entity;
        }

        private EXP_TransportChallanCI ConverTransportChallanCi(ExpTransportChallanCI model, long entityId, int userId
            , string recStat)
        {
            var entity = model.TransportChallanID == 0
                ? new EXP_TransportChallanCI()
                : (from b in _context.EXP_TransportChallanCI.AsEnumerable()
                   where b.TransportChallanID == model.TransportChallanID && b.CIID == model.CIID
                   select b).FirstOrDefault();

            entity.TransportChallanID = entityId;
            entity.CIID = model.CIID;
            entity.PLID = model.PLID;
            entity.DeliverChallanID = model.DeliverChallanID;
            entity.RecordStatus = recStat;
            entity.SetBy = model.TransportChallanID == 0
                ? userId
                : _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == model.TransportChallanID).SetBy;
            entity.SetOn = model.TransportChallanID == 0
                ? DateTime.Now
                : _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == model.TransportChallanID).SetOn;
            entity.ModifiedBy = model.TransportChallanID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.TransportChallanID == 0 ? (DateTime?)null : DateTime.Now;
            return entity;
        }

        public object GetTranportChallan(long id)
        {
            var transChaln = _context.EXP_TransportChallan.FirstOrDefault(ob => ob.TransportChallanID == id);
            var result = new ExpTransportChallan();

            result.TransportChallanID = transChaln.TransportChallanID;
            result.TransportChallanNo = transChaln.TransportChallanNo;
            result.TransportChallanDate = string.Format("{0:dd/MM/yyyy}", transChaln.TransportChallanDate);
            result.TransportChallanRef = transChaln.TransportChallanRef;
            result.TransportChallanNote = transChaln.TransportChallanNote;
            result.DeliverChallanID = transChaln.DeliverChallanID;
            result.DeliverChallanNo = transChaln.DeliverChallanID == null
                ? ""
                : _context.EXP_DeliveryChallan.FirstOrDefault(ob => ob.DeliverChallanID == transChaln.DeliverChallanID)
                    .DeliverChallanNo;
            result.Sender = transChaln.Sender;
            result.SenderAddress = transChaln.SenderAddress;
            result.Receiver = transChaln.Receiver;
            result.ReceverAddress = transChaln.ReceverAddress;
            result.DeliveryFrom = transChaln.DeliveryFrom;
            result.DEliveryTo = transChaln.DEliveryTo;
            result.RecordStatus = transChaln.RecordStatus;
            result.ConfirmNote = transChaln.ConfirmNote;
            result.ChallanCis = new List<ExpTransportChallanCI>();

            var cis =
                _context.EXP_TransportChallanCI.Where(ob => ob.TransportChallanID == transChaln.TransportChallanID)
                    .ToList();

            foreach (var ci in cis)
            {
                var x = new ExpTransportChallanCI();
                x.TransportChallanID = ci.TransportChallanID;
                x.CIID = ci.CIID;
                x.CINo = ci.CIID == null ? "" : _context.EXP_CI.First(ob => ob.CIID == ci.CIID).CINo;
                x.CIDate = ci.CIID == null
                    ? ""
                    : string.Format("{0:dd/MM/yyyy}", _context.EXP_CI.First(ob => ob.CIID == ci.CIID).CIDate);
                x.PLID = ci.PLID;
                x.PLNo = ci.PLID == null ? "" : _context.EXP_PackingList.First(ob => ob.PLID == ci.PLID).PLNo;
                x.PLDate = ci.PLID == null
                    ? ""
                    : string.Format("{0:dd/MM/yyyy}", _context.EXP_PackingList.First(ob => ob.PLID == ci.PLID).PLDate);
                x.DeliverChallanID = ci.DeliverChallanID;
                x.RecordStatus = ci.RecordStatus;
                result.ChallanCis.Add(x);
            }
            return result;
        }
    }
}
