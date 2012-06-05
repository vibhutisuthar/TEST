using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTP.BAL;
using MTP.DTO;

namespace MTP_JAPA.Controllers
{
    public class BookingsController : Controller
    {
        //
        // GET: /Bookings/

        public ActionResult BookingList()
        {
            ViewBag.theHiddenTabIndex = 1;
            //ViewBag.form_footer_detail = 0;
            ViewBag.TabDisplay = 1;
            ViewBag.LoginEmailAddress=Convert.ToString(Session["LoginEmailAddress"]);
            //if (Session["LoginEmailAddress"] != null)
            //{
            //    string strEmailAddress = Convert.ToString(Session["LoginEmailAddress"]);
            //    BookingDataBAL objBookingDataBAL = new BookingDataBAL();
            //    List<BookingModel> lstBookingModel = objBookingDataBAL.GetBookingByEmail(strEmailAddress);
            //    ViewBag.BookingModelList = lstBookingModel;

            //}

            return View();
        }

    }
}
