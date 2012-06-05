using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTP_JAPA.Controllers
{
    public class MessageDisplayController : BaseController
    {

        #region  Error Display Action

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Message()
        {
            try
            {
                ViewBag.theHiddenTabIndex = 2;
                ViewBag.TabDisplay = 1;

                //if (Session["RLOC"] != null)
                //{
                //    ViewBag.TabDisplay = 1;
                //}
                //else
                //{
                //    ViewBag.TabDisplay = 0;
                //}
                if (Session["theGoClick"] != null && Convert.ToString(Session["theGoClick"]) != "")
                {
                    int TabId = Convert.ToInt32(Session["theGoClick"]);
                    ViewBag.theGoClick = TabId;
                }
                else
                {
                    ViewBag.theGoClick = 404;
                }

                if (Request.QueryString["E"] != null)
                {
                    ProcessError(int.Parse(Request.QueryString["E"].ToString()));
                }
            }
            catch
            {
                ViewBag.Results = "Could not process Error.Press the back button ans retry";
                //ViewBag.Results = "Could not process Error.Press the back button ans retry";
            }
            return View("Error");
        }

        #endregion

        #region Helper Method

        /// <summary>
        /// ProcessError
        /// </summary>
        /// <param name="ErrorNumber"></param>
        protected void ProcessError(int ErrorNumber)
        {

            switch (ErrorNumber)
            {
                case 0:
                    ViewBag.Results = Request.QueryString["M"];
                    break;
                case 1:
                    ViewBag.Results = "To view your travel plans please enter the booking reference and your surname.Please retry";
                    break;
                case 2:
                    ViewBag.Results = "Unable to locate – Please check details and re-enter";
                    break;
                case 3:
                    ViewBag.Results = "Failed to get a response from the hosting system.Please try again shortly";
                    break;
                case 4:
                    ViewBag.Results = "Sorry no host device configured for this operation.";
                    break;
                case 5:
                    ViewBag.Results = "You have tried " + GetRetryCount().ToString() + " consecutive times to access data from mytravelplans, your account will be locked for 60 minutes. If you have having problems remembering you travel details, please contact your support. ";
                    break;

                case 6:
                    ViewBag.Results = "Your session has timed out. ";
                    break;

                case 7:
                    ViewBag.Results = "We can not locate the reference you have provided." +
                        "The booking may no longer be live in the American Express system or you may have used invalid references." +
                        "Please select BACK and change your search criteria.";
                    break;
                case 10:
                    ViewBag.Results = "We are unable to display your reservation." +
                        "Please either check out our HINTS section for assistance" +
                        "Or contact your Travel Agent/Provider for further reservation details." +
                        "Thank You";
                    break;

                case 11:
                    ViewBag.Results = "We are unable to email your reservation." +
                        "Please either check out our HINTS section for assistance" +
                        "Or contact your Travel Agent/Provider for further reservation details." +
                        "Thank You";
                    break;
                case 12:
                    ViewBag.Results =
                       "We are unable to locate an Itinerary for this PNR right now." +
                       "If your booking was made via an online tool, a ticket must be issued before the final itinerary will appear on this site. Please refer back to your online tool for any changes to your itinerary." +
                        "For all other itineraries, your consultant may still be updating the information." +
                        "Please check again in a few minutes or contact your travel team for further assistance.";
                    break;
                case 13:

                    ViewBag.Results =
                        "More than one invoice has been found for this number, please enter you Record Locator to retrieve the document.";
                    break;
                case 14:

                    ViewBag.Results =
                        "We are unable to locate an Itinerary for this PNR." +
                        "The booking may no longer be available in our system or the itinerary has not been uploaded to this website yet.";
                    break;

                case 15:

                    ViewBag.Results =
                        "We are unable to locate an Invoice for this PNR." +
                        "The booking may no longer be live in the system or you may have used an invalid reference.";
                    break;
                case 16:

                    ViewBag.Results =
                        "There is no itinerary record available to send. Your session may have timed out. Please try again orPlease contact your Travel team to request a current itinerary";
                    break;
                case 20:

                    ViewBag.Results =
                        "There is not enough information provided to process this request.";
                    break;
                case 21:

                    ViewBag.Results =
                        "Sorry, Unable to process calendar request.";
                    break;

                case 22:

                    ViewBag.Results =
                        "Failed to retrieve booking due to some error.";
                    break;

                case 23:

                    ViewBag.Results =
                        "Ticket number must have a numeric value and Surname is mandatory. Please correct and resend.";
                    break;

                case 24:

                    ViewBag.Results =
                       "We are unable to locate an E-ticket for this booking.An E-ticket may not have been issued yet or a paper ticket is required.Please contact your travel team if you require more information.";
                    break;

                case 100:

                    ViewBag.Results =
                       "Under Construction";
                    break;

                case 25:

                    ViewBag.Results =
                       "Invoice not found.";
                    break;

                case 26:

                    ViewBag.Results =
                       "System Timed out processing your request.";
                    break;


                case 27:

                    ViewBag.Results =
                       "No invoices found on disk";
                    break;


                case 28:

                    ViewBag.Results =
                       "Can't process this file";
                    break;

                case 29:

                    ViewBag.Results =
                        "Failed to retrieve eticket due to some error.";
                    break;

                case 30:

                    ViewBag.Results =
                        "Download: No records found for matching criteria";
                    break;


                case 31:

                    ViewBag.Results =
                        "Failed to retrieve calendar file due to some error.";
                    break;

                case 32:

                    ViewBag.Results =
                        "Can’t find that file.";
                    break;

                case 33:

                    ViewBag.Results =
                        "Either invoice not found or failed to retrieve invoice due to some error.Please contact your travel team if you require more information.";
                    break;

                case 34:

                    ViewBag.Results =
                        "Either itinerary not found or failed to retrieve Itinerary due to some error.Please contact your travel team if you require more information.";
                    break;

                case 35:

                    ViewBag.Results =
                        "Either itinerary not found or failed to retrieve itinerary due to some error.";
                    break;

                case 36:
                    ViewBag.Results =
                        "Password and confirm password are not same.Please try again";
                    break;

                case 37:
                    ViewBag.Results =
                        "Either email address is not registered yet or password is not correct.Please enter valid data.";
                    break;

                case 38:
                    ViewBag.Results =
                        "Mail has been succesfully sent with your new password on given email id.";
                    break;

                case 39:
                    ViewBag.Results =
                        "User successfully registered, Please check your email and confirm your registration with MyTravelPlans from American Express Business Travel";
                    break;

                case 40:
                    ViewBag.Results =
                        "Please enter valid email address.";
                    break;

                case 41:
                    ViewBag.Results =
                        "Please enter valid data.";
                    break;

                case 42:
                    ViewBag.Results =
                        "User already exist.";
                    break;


                case 43:
                    ViewBag.Results =
                        "Failed due to some error.";
                    break;

                case 44:
                    ViewBag.Results =
                        "Either email address not registered or old password is incorrect.";
                    break;

                case 45:
                    ViewBag.Results =
                        "Your Password has changed successfully.";
                    break;

                case 46:
                    ViewBag.Results =
                        "password should have minimum 6 characters.";
                    break;

                case 47:
                    ViewBag.Results =
                        "Either current password is not correct or some error has occured.Please try again.";
                    break;
                case 48:
                    ViewBag.Results =
                        "Chaptcha Image Data Provided by you is Invalid. Please try again.";
                    break;
                case 49:
                    ViewBag.Results =
                        "Your Registration Confirmation link is not valid or already registred.";
                    break;
                case 50:
                    ViewBag.Results =
                        "Your Registration is not confirmed with us, Please verify your email address by click on Registration Confirmation link";
                    break;
                case 51:
                    ViewBag.Results =
                        "We sent you an email with the Reset Password link, Please check email and click on the link";
                    break;

                case 52:
                    ViewBag.Results =
                        "Either no calendar is available or some error has occured.Please try again.";
                    break;
                default:
                    break;


            }
        }

        #endregion

    }
}
