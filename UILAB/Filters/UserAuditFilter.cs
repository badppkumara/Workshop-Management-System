using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Filters
{
    public class UserAuditFilter : ActionFilterAttribute
    {
        IAudit _IAudit;
        public UserAuditFilter()
        {
            _IAudit = new AuditConcrete();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            AuditUserLog objaudit = new AuditUserLog(); 

            string _ActionName = filterContext.ActionDescriptor.ActionName; //Getting Controller Name 
            string _ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var request = filterContext.HttpContext.Request;

            Uri myReferrer = request.UrlReferrer;

            if (myReferrer != null)
            {
                string actual = myReferrer.ToString();

                if (actual != null)
                {
                    objaudit.UrlReferrer = request.UrlReferrer.AbsolutePath;
                }                
            }
            else
            {
                objaudit.UrlReferrer = "";
            }

            if (HttpContext.Current.Session["SuperAdmin"] != null)
            {
                objaudit.UserID = Convert.ToInt32(HttpContext.Current.Session["SuperAdmin"]);
                objaudit.SegmentID = Convert.ToInt32(HttpContext.Current.Session["Segment"]);
                objaudit.UserType = 1;
            }
            else if (HttpContext.Current.Session["AdminUser"] != null)
            {
                objaudit.UserID = Convert.ToInt32(HttpContext.Current.Session["AdminUser"]);
                objaudit.SegmentID = Convert.ToInt32(HttpContext.Current.Session["Segment"]);
                objaudit.UserType = 2;
            }
            else if (HttpContext.Current.Session["Employee"] != null)
            {
                objaudit.UserID = Convert.ToInt32(HttpContext.Current.Session["Employee"]);
                objaudit.SegmentID = Convert.ToInt32(HttpContext.Current.Session["Segment"]);
                objaudit.UserType = 3;
            }
            else if (HttpContext.Current.Session["Customer"] != null)
            {
                objaudit.UserID = Convert.ToInt32(HttpContext.Current.Session["Customer"]);
                objaudit.SegmentID = Convert.ToInt32(HttpContext.Current.Session["Segment"]);
                objaudit.UserType = 4;
            }
            else
            {
                objaudit.UserID = -1;
                objaudit.UserType = -1;
                objaudit.SegmentID = -1;
            }


            objaudit.SessionID = HttpContext.Current.Session.SessionID; // Application SessionID // User IPAddress 
            objaudit.IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            if (string.IsNullOrEmpty(objaudit.IPAddress))
            {
                objaudit.IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            objaudit.PageAccessed = Convert.ToString(filterContext.HttpContext.Request.Url); // URL User Requested 
            objaudit.LoggedInAt = DateTime.Now; // Time User Logged In || And time User Request Method 
            objaudit.LoggedOutAt = DateTime.Parse("1900-01-01");

            if (_ActionName == "Logout")
            {
                objaudit.LoggedOutAt = DateTime.Now; // Time User Logged OUT 
            }
            else
            {
                objaudit.LoggedOutAt = DateTime.Parse("1900-01-01");
            }

            objaudit.LoginStatus = "A";
            objaudit.ControllerName = _ControllerName; // ControllerName 
            objaudit.ActionName = _ActionName;
            objaudit.MachineName = System.Environment.MachineName;


            _IAudit.InsertAuditData(objaudit);

        }
    }
}