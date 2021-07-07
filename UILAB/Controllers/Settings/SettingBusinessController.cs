using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Settings
{
    [ValidateAdminSession]
    public class SettingBusinessController : Controller
    {
        OrganizationSchemeInfo objorginfo = new OrganizationSchemeInfo();

        ISetting _Setting;

        public SettingBusinessController()
        {
            _Setting = new SettingConcrete();
        }

        [HttpGet]
        public ActionResult Create()
        {
            TempData["Error"] = "";
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Segment";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.OrganizationSchemeInfos.SingleOrDefault(b => b.SegmentID == _Segment);

                if (result == null)
                {
                    return View(Create());
                }
                else
                {
                    //var segment = (from data in db.OrganizationSchemeTRs orderby data.SegmentName ascending select data).ToList();
                    //ViewBag.SegmentList = new SelectList(segment, "SegmentID", "SegmentName");

                    var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                    ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                    return View(db.OrganizationSchemeInfos.Where(x => x.SegmentID == _Segment).FirstOrDefault<OrganizationSchemeInfo>());
                }
            }
        }

        [HttpGet]
        public ActionResult BusinessInfo()
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            TempData["Error"] = "";
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Segment";

            using (var db = new DatabaseContext())
            {
                var result = db.OrganizationSchemeInfos.SingleOrDefault(b => b.SegmentID == _Segment);

                if (result == null)
                {
                    return View(Create());
                }
                else
                {
                    //var segment = (from data in db.OrganizationSchemeTRs orderby data.SegmentName ascending select data).ToList();
                    //ViewBag.SegmentList = new SelectList(segment, "SegmentID", "SegmentName");

                    var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                    ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                    return View(db.OrganizationSchemeInfos.Where(x => x.SegmentID == _Segment).FirstOrDefault<OrganizationSchemeInfo>());
                }
            }
        }

        [HttpPost]
        public ActionResult BusinessInfo(OrganizationSchemeInfo id, HttpPostedFileBase FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.OrganizationSchemeInfos.SingleOrDefault(b => b.SegmentID == _Segment);

                if (result == null)
                {
                    return View(Create());
                }
                else
                {
                    //TempData["username"] = result.UserName;
                    return View(db.OrganizationSchemeInfos.Where(x => x.SegmentID == _Segment).FirstOrDefault<OrganizationSchemeInfo>());
                }
            }
        }
    }
}