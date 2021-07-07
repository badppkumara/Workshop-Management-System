using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;

namespace UILAB.Controllers
{
    [ValidateSAdminSession]
    public class SAdminController : Controller
    {
        [HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.Current = "Dashboard";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["SuperAdmin"]);

            using (var db = new DatabaseContext())
            {
                //var customer = (from data in db.vw_CustomerMaster where data.SegmentID == _Segment orderby data.FullName descending select data).ToList();
                //if (customer.Count > 0)
                //{
                //    TempData["customers"] = customer.Count;
                //}
                //else
                //{
                //    TempData["customers"] = "0";
                //}


                return View();
            }
        }
    }
}