using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;

namespace UILAB.Controllers
{
    [ValidateAdminSession]
    public class ErrorPageController : Controller
    {
        [HttpGet]
        public ActionResult LinkError()
        {
            return View();
        }
    }
}