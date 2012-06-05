using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTP_JAPA.Controllers
{
    public class HelpController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult HelpView()
        {
            //if (Session["RLOC"] != null)
            //{
            //    ViewBag.TabDisplay = 1;
            //}
            //else
            //{
            //    ViewBag.TabDisplay = 0;
            //}
            ViewBag.TabDisplay = 1;
            ViewBag.theGoClick = 100;
            Session["theGoClick"] = 100;
            ViewBag.bPrint = 0;
            return View();
        }

    }
}
