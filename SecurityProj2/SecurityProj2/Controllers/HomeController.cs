using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


//HomeController.cs manages the Index View (homepage)
namespace SecurityProj2.Controllers
{
    public class HomeController : Controller
    {
        //Returns Index View
        public ActionResult Index()
        {
            return View();
        }
    }
}
