using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UILAB.Models;
using UILAB.Concrete;
using System.Diagnostics;

namespace UILAB.Filters
{
    public class ValidateSAdminSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["SuperAdmin"])))
                {
                    filterContext.Controller.TempData["ErrorMessage"] = "Session has been expired please Login";
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login" }));
                }
                else
                {
                    int _UserID = Convert.ToInt32(filterContext.HttpContext.Session["SuperAdmin"]);
                    int _SegmentID = Convert.ToInt32(filterContext.HttpContext.Session["Segment"]);
                    int _EmployeeNo = Convert.ToInt32(filterContext.HttpContext.Session["Employee"]);
                    int _LogInstance = Convert.ToInt32(filterContext.HttpContext.Session["LoginInstance"]);

                    // -------------------------- Get Build Version ------------------------------
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    filterContext.HttpContext.Session["BuildVersion"] = fvi.FileVersion;
                    // ---------------------------------------------------------------------------

                    using (var db = new DatabaseContext())
                    {
                        var active = db.UserSecurities.SingleOrDefault(b => b.IsActive == false && b.UserID == _UserID);
                        if (active != null)
                        {
                            var instance = db.SecurityActiveUser.SingleOrDefault(b => b.LoginInstance == _LogInstance && b.ConnectionAlive == true);

                            instance.LogOutDate = DateTime.Now;
                            instance.ConnectionAlive = false;
                            db.SaveChanges();

                            filterContext.Controller.TempData["errormessage"] = "Account Deactivated";
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login" }));
                        }
                        else
                        {

                            // -------------------------- Get Segment Name ------------------------------                        
                            var data = db.OrganizationSchemeTRs.SingleOrDefault(b => b.SegmentID == _SegmentID);

                            if (data != null)
                            {
                                filterContext.HttpContext.Session["SegmentName"] = data.SegmentName;
                            }
                            else
                            {
                                filterContext.HttpContext.Session["SegmentName"] = "UILAB";
                            }
                            // ---------------------------------------------------------------------------

                            // -------------------------- Get User Name ------------------------------
                            var result = db.UserSecurities.SingleOrDefault(b => b.UserID == _UserID);

                            if (result != null)
                            {
                                var employeedata = db.EmployeeMasters.SingleOrDefault(b => b.SegmentID == _SegmentID && b.EmployeeNo == _EmployeeNo);

                                if (employeedata != null)
                                {
                                    filterContext.HttpContext.Session["UserName"] = employeedata.FirstName + " " + employeedata.LastName;
                                }
                                else
                                {
                                    filterContext.HttpContext.Session["UserName"] = "Super Admin";
                                }
                            }
                            else
                            {
                                filterContext.HttpContext.Session["UserName"] = "Super Admin";
                            }
                            // ---------------------------------------------------------------------------

                            // -------------------------- Get User Image ------------------------------
                            var image = db.FileUsers.SingleOrDefault(b => b.EntryID == _EmployeeNo && b.FileTypeID == 1 && b.IsPrimaryPicture == true);

                            if (image != null)
                            {
                                //byte[] imageByte = (byte[])image.FileBitStreem;

                                //System.Drawing.Image imageToBeResized = System.Drawing.Image.FromStream(BytearrayToStream(imageByte));
                                //int imageHeight = imageToBeResized.Height;
                                //int imageWidth = imageToBeResized.Width;
                                //int maxHeight = 350;
                                //int maxWidth = 400;
                                //imageHeight = (imageHeight * maxWidth) / imageWidth;
                                //imageWidth = maxWidth;

                                //if (imageHeight > maxHeight)
                                //{
                                //    imageWidth = (imageWidth * maxHeight) / imageHeight;
                                //    imageHeight = maxHeight;
                                //}

                                //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imageToBeResized, imageWidth, imageHeight);
                                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                                //bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                stream.Position = 0;
                                byte[] imasave = (byte[])image.FileBitStreem;
                                stream.Read(imasave, 0, imasave.Length);
                                filterContext.HttpContext.Session["ImageAdmin"] = "data:image/png;base64," + Convert.ToBase64String(imasave);
                            }
                            else
                            {
                                filterContext.HttpContext.Session["ImageAdmin"] = "/Content/img/user.jpg";
                            }
                            // ---------------------------------------------------------------------------
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private System.IO.MemoryStream BytearrayToStream(byte[] arr)
        {
            return new System.IO.MemoryStream(arr, 0, arr.Length);
        }
    }
}