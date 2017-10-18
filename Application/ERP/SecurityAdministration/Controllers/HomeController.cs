using System.Web.Mvc;


namespace SecurityAdministration.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public ActionResult RenderMenu()
        //{
        //    List<MenuItem> menuItems = GeneralController.GetMenuItems();
        //    return View(menuItems);
        //}
        public ActionResult ChangePassword()
        {
            return View();

        }
    }
}