using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.ActionFilters
{
    public class CheckUserAccess : ActionFilterAttribute
    {

        private readonly string _urlString;

        public CheckUserAccess(string urlString)
        {
            _urlString = urlString;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpContext ctx = HttpContext.Current;

            if (ctx.Session["userUrlPermission"] == null)
            {
                //String redirectTo = string.Format("~/Home/login");
                String redirectTo = string.Format("~/Home/SessionOutMsg");
                filterContext.Result = new RedirectResult(redirectTo);
            }
            else
            {
                var userUrlList = ctx.Session["userUrlPermission"] as IEnumerable<string>;

                if (userUrlList != null)
                {
                    var isExists = Enumerable.Any(userUrlList, v => v.ToUpper().Equals(_urlString.ToUpper()));
                    if (isExists) return;
                    String redirectTo = string.Format("~/Home/login");
                    filterContext.Result = new RedirectResult(redirectTo);
                }
                else
                {
                    String redirectTo = string.Format("~/Home/login");
                    filterContext.Result = new RedirectResult(redirectTo);
                }

                
            }
        }
    }
}