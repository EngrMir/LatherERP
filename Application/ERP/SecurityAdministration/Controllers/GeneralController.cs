using System;
using System.Linq;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class GeneralController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        [HttpPost]
        public ActionResult Login(string loginID, string password)
        {

            try
            {
                
                var userInfo = _unitOfWork.UserCredentialInformationRepository.Get(null,loginID).FirstOrDefault();
                   
                if (userInfo != null)
                {
                    int roleId =_unitOfWork.UserInRoleRepository.GetRoleId(Convert.ToInt32(userInfo.UserID));
                  
                    Session["RoleId"] = roleId;
                  
                    if (UserAuthenticator.UserAuthenticator.ValidatePassword(password.Trim(), userInfo.Password))
                        {
                            Session["UserId"] = userInfo.UserID;
                            Session["UserName"] = userInfo.FullName;
                            Session["UserType"] = userInfo.UserType;

                            if (!userInfo.IsActive || userInfo.IsLocked && (bool) userInfo.IsLocked)
                            {
                                return Json(new {val = 1, result = "User not valid", url = Url.Action("Index", "Home")});
                            }

                            if (userInfo.IsPasswordAccepted != null && (bool) userInfo.IsPasswordAccepted)
                            {

                                Session["IsLogged"] = true;
                                if (roleId == 0)
                                {
                                    Session["RoleMsg"] = " : Role not assign";
                                }
                                
                                SetLoginInformation(userInfo);
                                return
                                    Json(
                                        new
                                        {
                                            val = 2,
                                            result = "Welcome " + userInfo.FullName,
                                            url = Url.Action("Index", "Home")
                                        });

                            }
                            Session["IsLogged"] = true;
                            SetLoginInformation(userInfo);
                            return
                                Json(
                                    new
                                    {
                                        val = 3,
                                        result = "Change your password",
                                        url = Url.Action("ChangePassword", "Home")
                                    });
                        }
                    else
                    {
                        Session["LoginError"] = "Incorrect user login information.";
                        return Json(
                        new
                        {
                            val = 4,
                            result = "Incorrect Login password",
                            url = Url.Action("Index", "Home")
                        });
                    }
                    
                }
                else
                {
                    Session["LoginError"] = "Incorrect user login information.";
                    return Json(new {val=1, result = "Login Failed", url = Url.Action("Index", "Home")});
                }
                
            }
            catch (Exception exception)
            {
                return Json(new { result = "Sorry system error", url = Url.Action("Index", "Home") });
            }
        }

        private void SetLoginInformation(UserView userView)
        {
            LoginInformation.UserID = Convert.ToInt32(userView.UserID);
            LoginInformation.FullName = userView.FirstName + " " + userView.MiddleName + " " + userView.LastName;
            LoginInformation.IsActive = userView.IsActive;
            LoginInformation.IsLocked = userView.IsLocked;
            LoginInformation.IsPasswordAccepted = userView.IsPasswordAccepted;
            LoginInformation.LastPasswordChangedDate = Convert.ToDateTime(userView.LastPasswordChangedDate);
            LoginInformation.LoginID = userView.LoginID;
            LoginInformation.Password = userView.Password;
            LoginInformation.SupervisorUserID = Convert.ToInt32(userView.SupervisorUserID);
            LoginInformation.UserCode = userView.UserCode;
            LoginInformation.UserType = userView.UserType;
            LoginInformation.DesignationID = userView.DesignationID;
        }

        public ActionResult LogOut()
        {
            Session["IsLogged"] = false;
            Session.Abandon();
            return Json(new {result = "Redirect", url = Url.Action("Index", "Home")});
        }
    }
}