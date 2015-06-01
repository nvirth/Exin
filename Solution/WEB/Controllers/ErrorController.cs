using System.Web.Mvc;

namespace WEB.Controllers
{
	public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index(int? httpErrorCode)
        {
            return View("Error");
        }
    }
}
