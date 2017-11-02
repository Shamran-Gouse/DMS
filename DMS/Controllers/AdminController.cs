using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult SignIn()
        {
            return View();
        }

        // GET: Admin/Details/5
        public ActionResult SignUp()
        {
            return View();
        }
    }
}
