using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using ERP.BusinessLogicLayer.AppSetupManager;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;

namespace ERP_Leather.Controllers
{
    public class HomeController : Controller
    {
        DalMenuItems  dalObj = new DalMenuItems();
        DalUser objUser=new DalUser();
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("SessionOutMsg", "Home");
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel objModellogin)
        {
            var strErrorMsg = string.Empty;
            var strExpiryValMsg = string.Empty;
            
            try
            {
                if (objModellogin.LoginID != null)
                {
                    //strExpiryValMsg = UserAuthenticator.UserAuthenticator.ValidationExpiryMsg(DateTime.Now);
                    if(string.IsNullOrEmpty(strExpiryValMsg))
                    { 
                    var userInfo = objUser.GetUserInformation(null, objModellogin.LoginID).FirstOrDefault();
                    if (userInfo!=null)
                    {
                        if (UserAuthenticator.UserAuthenticator.ValidatePassword(objModellogin.Password.Trim(), userInfo.Password))
                        {
                        Session["UserID"] = Convert.ToInt32(userInfo.UserID);
                        Session["UserName"] = userInfo.FullName;
                        if (!userInfo.IsActive || userInfo.IsLocked && userInfo.IsLocked)
                        {
                            return Json(new { val = 1, result = "User not valid", url = Url.Action("Index", "Home") });
                        }

                        if (userInfo.IsPasswordAccepted != null && (bool)userInfo.IsPasswordAccepted)
                        {
                            Session["IsLogged"] = true;
                            FormsAuthentication.SetAuthCookie(userInfo.UserID.ToString(), false);
                           var  userUrl= dalObj.UserAccessPermissionList(Convert.ToInt32(userInfo.UserID));
                           Session["userUrlPermission"] = userUrl;
                           
                            return RedirectToAction("Index", "Home");
                        }
                        Session["IsLogged"] = true;
                        return RedirectToAction("ChangePassword", "Home");
                    }
                        strErrorMsg = "Incorrect User Password!";
                    }
                    else
                    {
                        strErrorMsg = "Incorrect User Information!";
                    }
                    }
                    else
                    {
                        strErrorMsg = strExpiryValMsg;
                    }
                }
                ViewBag.Msg = strErrorMsg;
                return View();
            }
            catch (Exception exception)
            {
                return RedirectToAction("Error", "Home");
            }
            
        }

        public ActionResult LogOut()
        {
            Session["IsLogged"] = false;
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("login","Home");
        }
        public ActionResult ChangePassword()
        {
            return View();

        }
        [HttpPost]
        public ActionResult ChangePassword(int userID, string password)
        {
            var count = objUser.ChangePassword(userID, UserAuthenticator.UserAuthenticator.GetHashPassword(password));
            return count > 0
                ? (ActionResult) Json(new {result = "Password has been changed", url = Url.Action("Index", "Home")})
                : RedirectToAction("Error", "Home");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LoadMenu(int? id)
        {
            if (Request.Url != null)
            {
                var nodes = dalObj.LoadMenu(id, Convert.ToInt32(Session["UserID"]));

                return Json(nodes, JsonRequestBehavior.AllowGet);
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }


        public ActionResult Error()
        {
            return View();
        }
        public ActionResult SessionOutMsg()
        {
            return View();
        }
    }
}