using System;
using System.Activities.Statements;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;
using Telerik.Web.Mvc.Extensions;
using Telerik.Web.Mvc.Infrastructure;

namespace SecurityAdministration.Controllers
{
    public class UserController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            try
            {
                var userViewModel = new UserVM
                {
                    Users = _unitOfWork.UserRepository.GetUserInfo(0,string.Empty).Where(s => s.IsDelete == false).OrderByDescending(s => s.UserID),
                    DesignationList = new SelectList(_unitOfWork.DesignationRepository.Get(), "DesignationID", "Name"),
                    SupervisorList = new SelectList(_unitOfWork.UserRepository.GetAllSupervisor(), "UserID", "Supervisor"),
                };

                ViewBag.UserList = userViewModel.Users.ToList();
                var lastUserCode = _unitOfWork.UserRepository.GetLastUserCodeID();
                ViewBag.UserCode = GetUserCodeID(lastUserCode);

                return View(userViewModel);
            }
            catch (Exception exception)
            {
                throw exception.InnerException;
            }
        }

        public JsonResult Save([Bind(Include = "UserID, SupervisorUserID,UserCode,UserType,Title,FirstName,MiddleName,LastName,DesignationID,Email,Phone,Mobile,IsActive,Description,LoginID,Password,SetOn,SetBy,IsLocked")] UserView userView, bool isInsert, bool isResetPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new User
                    {
                        UserID = Convert.ToInt32(userView.UserID),
                        UserCode = userView.UserCode,
                        UserType = userView.UserType ?? 2,
                        Title = userView.Title,
                        FirstName = userView.FirstName,
                        MiddleName = userView.MiddleName,
                        LastName = userView.LastName,
                        DesignationID = userView.DesignationID,
                        Email = userView.Email,
                        Phone = userView.Phone,
                        Mobile = userView.Mobile,
                        IsActive = userView.IsActive,
                        SupervisorUserID = Convert.ToInt32(userView.SupervisorUserID),
                        Description = userView.Description,
                        SetBy = LoginInformation.UserID,
                        SetOn = DateTime.Now
                    };

                    var userCredential = new UserCredentialInformation
                    {
                        SetBy = LoginInformation.UserID,
                        SetOn = DateTime.Now,
                        LastPasswordChangedDate = DateTime.Now,
                        IsPasswordAccepted = false,
                        IsLocked = userView.IsLocked,
                        Password = UserAuthenticator.UserAuthenticator.GetHashPassword(userView.Password),
                        LoginID = userView.LoginID,
                        UserID = Convert.ToInt32(userView.UserID),
                    };


                    if (isInsert)
                    {
                        var isTrue = _unitOfWork.UserCredentialInformationRepository.CheckUniqueLoginId(userCredential.LoginID);
                        if (isTrue == false)
                        {
                            _unitOfWork.UserRepository.Insert(user);
                            _unitOfWork.Save();
                            var lastInsertID = _unitOfWork.UserRepository.GetLastInsertID();
                            userCredential.UserID = lastInsertID;
                            _unitOfWork.UserCredentialInformationRepository.Insert(userCredential);
                            _unitOfWork.Save();
                            _unitOfWork.UserCredentialInformationRepository.PerformAudit_CredentialInfo(userCredential, Convert.ToChar(AuditStatusFlag.Create)); ;
                        }
                    }
                    else
                    {
                        if (isResetPassword)
                        {
                            _unitOfWork.UserRepository.Update(user);
                            _unitOfWork.UserCredentialInformationRepository.UpdatePassword(userCredential);
                        }
                        else
                        {
                            _unitOfWork.UserRepository.Update(user);
                            _unitOfWork.UserCredentialInformationRepository.UpdateIsLocked(userCredential);
                        }
                        _unitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new JsonResult { Data = _unitOfWork.UserRepository.GetUserInfo(Convert.ToInt32(userView.UserID)).FirstOrDefault() };
        }

        public JsonResult GetUser(int id)
        {
            return new JsonResult { Data = _unitOfWork.UserRepository.GetUserInfo(id).FirstOrDefault() };
        }

        public JsonResult CheckUniqueLoginId(string loginId)
        {
            bool isTrue = _unitOfWork.UserCredentialInformationRepository.CheckUniqueLoginId(loginId);
            return new JsonResult { Data = isTrue };
        }

        public JsonResult UserCodeGenerate()
        {
            var lastUserCode = _unitOfWork.UserRepository.GetLastUserCodeID();
            return new JsonResult { Data = GetUserCodeID(lastUserCode) };
        }

        public void Delete(int id)
        {
            _unitOfWork.UserRepository.InActiveUserInformation(id);
            _unitOfWork.Save();
        }

        public ActionResult ChangePassword(int userID, string password)
        {

            _unitOfWork.UserRepository.PasswordChange(userID, UserAuthenticator.UserAuthenticator.GetHashPassword(password));
            _unitOfWork.Save();

            return Json(new { result = "Password has been changed", url = Url.Action("Index", "Home") });
        }

        public static string GetUserCodeID(string lastUserCode)
        {
            var userCodePrefix = System.Configuration.ConfigurationManager.AppSettings["UserCodePrefix"];

            lastUserCode = lastUserCode ?? userCodePrefix + "-" + "0000";
            int digit = Convert.ToInt32(lastUserCode.Split('-').Last());
            var userCode = "";
            digit++;
            var numberOfUser = System.Configuration.ConfigurationManager.AppSettings["NumberOfUser"];
            var strExpiryValMsg = string.Empty;
            strExpiryValMsg = UserAuthenticator.UserAuthenticator.ValidationExpiryMsg(DateTime.Now);

            if (!string.IsNullOrEmpty(strExpiryValMsg))
            {
                return  "VAL_EXPIRE";
            }
            if (digit > Convert.ToInt32(numberOfUser))
            {
                userCode = "CROSS_LIMIT";
            }
            else
            {
                userCode += userCodePrefix + "-" + Convert.ToString(digit).PadLeft(4, '0');
            }
            return userCode;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
