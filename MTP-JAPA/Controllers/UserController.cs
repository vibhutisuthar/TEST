using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTP_JAPA.Helpers;
using NLog;
using NLog.Config;
using System.Xml;
using MTP.DTO;
using MTP.BAL;
using System.Text;
using System.Data;
using System.Configuration;
using System.IO;
using System.Xml.Xsl;

namespace MTP_JAPA.Controllers
{
    public class UserController : BaseController
    {

        #region Variables

        string strBookingReference = "Booking Reference";
        string strInvoiceNumber = "Invoice Number";
        string strTicketNumber = "Ticket Number";


        #endregion

        #region Load

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Session["RLOC"] = null;
            Session["Surname"] = null;
            Session["GDS"] = null;
            Session["InvoiceNumber"] = null;
            Session["QiLiveItineraryData"] = null;
            Session["LoginEmailAddress"] = null;
            ViewBag.theHiddenTabIndex = 0;
            ViewBag.TabDisplay = 1;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult EmailLogin()
        {
            ViewBag.theHiddenTabIndex = 1;
            //ViewBag.form_footer_detail = 0;
            ViewBag.TabDisplay = 1;
            return View();
        }

        /// <summary>
        /// EmailLogin
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailLogin(FormCollection formCollection)
        {

            string strEMailAddress = "";
            string strPassword = "";
            foreach (string key in formCollection.Keys)
            {
                if (key == "email")
                {
                    strEMailAddress = formCollection[key].Trim();
                }
                else if (key == "password")
                {
                    strPassword = formCollection[key].Trim();
                }
            }
            Session["LoginEmailAddress"] = null;
            if (strEMailAddress != "" && strPassword != "" && strEMailAddress.ToLower() != "email address")
            {
                UsersBAL objUserBAL = new UsersBAL();
                List<UsersModel> lstUsersModel = objUserBAL.FindUser(strEMailAddress, strPassword);
                if (lstUsersModel != null && lstUsersModel.Count > 0)
                {
                    if (lstUsersModel[0].Status == "confirm")
                    {
                        Session["LoginEmailAddress"] = strEMailAddress;
                        return RedirectToAction("BookingList", "Bookings");
                    }
                    else
                    {
                        return RedirectToAction("Message", "MessageDisplay", new { E = 50 });
                    }
                }
                else
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 37 });
                }
            }
            else
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 2 });
            }
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            if (Request.Params["p"] == null)
            {
                ViewBag.theHiddenTabIndex = 1;
                //ViewBag.form_footer_detail = 0;
                ViewBag.TabDisplay = 1;
                return View();
            }
            else
            {
                if (ConfirmeRegistration(Server.UrlDecode(Convert.ToString(Request.Params["p"]))) == 1)
                {
                    return RedirectToAction("RegistrationConfirmation");
                }
                else
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 49 });
                }
            }
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        public ActionResult Reset()
        {
            if (Request.Params["p"] != null)
            {
                UsersModel usr = ResetEncodeStringExist(Server.UrlDecode(Convert.ToString(Request.Params["p"])));
                if (usr != null)
                {
                    Session["LoginEmailAddress"] = usr.EmailAddress;
                    Session["LoginCurrentPassword"] = usr.Password;
                    return RedirectToAction("EnterNewPassword");
                }
                else
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 49 });
                }
            }
            return null;
        }


        public ActionResult EnterNewPassword()
        {
            ViewBag.theHiddenTabIndex = 1;
            ViewBag.TabDisplay = 1;
            return View();
        }


        public ActionResult Privacy()
        {
            ViewBag.theHiddenTabIndex = 5;
            ViewBag.TabDisplay = 1;
            return View();
        }


        [HttpPost]
        public ActionResult EnterNewPassword(FormCollection formCollection)
        {
            string EmailAddress = "", CurrentPassword = "", password = "", confirmPassword = "";

            if (Session["LoginEmailAddress"] == null)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 2 });
            }
            EmailAddress = Convert.ToString(Session["LoginEmailAddress"]);
            CurrentPassword = Convert.ToString(Session["LoginCurrentPassword"]);

            foreach (string key in formCollection.Keys)
            {
                if (key == "password")
                {
                    password = formCollection[key].Trim();
                }
                else if (key == "confirmpassword")
                {
                    confirmPassword = formCollection[key].Trim();
                }
            }

            logger.Debug("Validations check");
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 41 });
            }

            if (password.Length < 6)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 46 });
            }

            if (password != confirmPassword)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 36 });
            }

            int retVal = ChangePasswordMethod(EmailAddress, CurrentPassword, password);
            if (retVal == 1)
            {
                return RedirectToAction("NewPasswordConfirmed");
            }
            else
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 47 });
            }
        }


        /// <summary>
        /// Change Password
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            ViewBag.theHiddenTabIndex = 1;
            //ViewBag.form_footer_detail = 0;
            ViewBag.TabDisplay = 1;
            return View();
        }

        /// <summary>
        /// ChangePassword
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(FormCollection formCollection)
        {
            string emailAddress = "";
            if (Session["LoginEmailAddress"] != null)
            {
                emailAddress = Convert.ToString(Session["LoginEmailAddress"]);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }

            UsersBAL objUserBAL = new UsersBAL();
            logger.Debug("Change Password");
            string currentpassword = "", password = "", confirmPassword = "";
            foreach (string key in formCollection.Keys)
            {
                if (key == "currentpassword")
                {
                    currentpassword = formCollection[key].Trim();
                }
                else if (key == "password")
                {
                    password = formCollection[key].Trim();
                }
                else if (key == "confirmpassword")
                {
                    confirmPassword = formCollection[key].Trim();
                }
            }

            logger.Debug("Validations check");
            if (string.IsNullOrEmpty(currentpassword) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(emailAddress))
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 41 });
            }
            if (password.Length < 6)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 46 });
            }
            if (password != confirmPassword)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 36 });
            }

            logger.Debug("Call Change password Method");
            int retVal = ChangePasswordMethod(emailAddress, currentpassword, password);
            if (retVal == 1)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 45 });
            }
            else
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 47 });
            }
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        [CaptchaValidator]
        [HttpPost]
        public ActionResult Register(FormCollection formCollection, bool captchaValid)
        {

            if (ModelState.IsValid && captchaValid)
            {
                UsersBAL objUserBAL = new UsersBAL();
                logger.Debug("Register New User");
                string userName = "", emailAddress = "", password = "", confirmPassword = "";
                foreach (string key in formCollection.Keys)
                {
                    if (key == "username")
                    {
                        userName = formCollection[key].Trim();
                    }
                    else if (key == "email")
                    {
                        emailAddress = formCollection[key].Trim();
                    }
                    else if (key == "password")
                    {
                        password = formCollection[key].Trim();
                    }
                    else if (key == "confirmpassword")
                    {
                        confirmPassword = formCollection[key].Trim();
                    }
                }

                logger.Debug("Validations check");
                if (string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 41 });
                }

                if (password.Length < 6)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 46 });
                }

                if (Common.ValidateEmailAddress(emailAddress) == false)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 40 });
                }

                if (password != confirmPassword)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 36 });
                }

                logger.Debug("check user already exist or not");
                int retValue = UserAlreadyExist(emailAddress);
                if (retValue == 1)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 42 });
                }
                else if (retValue == 2)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 43 });
                }

                logger.Debug("If user not already exist then register new user");
                if (retValue == 0)
                {
                    int NewUserRegistered = RegisterNewUser(emailAddress, password, Common.GetSHA1HashData(emailAddress));


                    if (NewUserRegistered == 1)
                    {
                        return RedirectToAction("Message", "MessageDisplay", new { E = 39 });
                    }
                    else
                    {
                        return RedirectToAction("Message", "MessageDisplay", new { E = 43 });
                    }
                }
            }

            if (!captchaValid)
                return RedirectToAction("Message", "MessageDisplay", new { E = 48 });


            return View();
        }

        /// <summary>
        /// ResetPassword
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            ViewBag.theHiddenTabIndex = 1;
            //ViewBag.form_footer_detail = 0;
            ViewBag.TabDisplay = 1;
            return View();
        }

        /// <summary>
        /// ReNewPassword
        /// </summary>
        /// <returns></returns>
        [CaptchaValidator]
        [HttpPost]
        public ActionResult ResetPassword(FormCollection formCollection, bool captchaValid)
        {

            if (ModelState.IsValid && captchaValid)
            {
                string email = "";
                foreach (string key in formCollection.Keys)
                {
                    if (key == "email")
                    {
                        email = formCollection[key].Trim();
                    }
                }

                if (Common.ValidateEmailAddress(email) == false)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 40 });
                }

                if (!string.IsNullOrEmpty(email))
                {

                    int retValue = SendMailForRenewPassword(email);

                    if (retValue == 1)
                    {
                        return RedirectToAction("Message", "MessageDisplay", new { E = 38 });
                    }

                    if (retValue == 0)
                    {
                        return RedirectToAction("Message", "MessageDisplay", new { E = 51 });
                    }
                }
                else
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 41 });
                }



            }

            if (!captchaValid)
                return RedirectToAction("Message", "MessageDisplay", new { E = 48 });

            return View();

        }

        /// <summary>
        /// RegistrationConfirmation
        /// </summary>
        /// <returns></returns>
        public ActionResult RegistrationConfirmation()
        {
            ViewBag.theHiddenTabIndex = 1;
            ViewBag.TabDisplay = 1;


            ViewBag.EmailAddress = Session["LoginEmailAddress"];

            //1) First Decrypt QueryString
            //2) Update user Status to confirmed
            //UsersBAL objUsersBAL = new UsersBAL();
            ///objUsersBAL.UpdateUserStatus(1, "confirmed");
            return View();
        }

        /// <summary>
        /// ResetPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPasswordConfirmation()
        {
            ViewBag.theHiddenTabIndex = 1;
            ViewBag.TabDisplay = 1;
            return View();
        }

        public ActionResult NewPasswordConfirmed()
        {
            ViewBag.theHiddenTabIndex = 1;
            ViewBag.TabDisplay = 1;
            return View();
        }


        #endregion

        #region Events

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FormCollection formCollection, string radioValue)
        {
            ViewBag.theGoClick = 0;
            Session["theGoClick"] = 0;

            string strValue = "";
            string strSearchOn = "";
            foreach (string key in formCollection.Keys)
            {
                if (key == "surname")
                {
                    m_Surname = formCollection[key].Trim();

                }
                else if (key == "reference_number")
                {
                    strValue = formCollection[key].Trim();

                }
                else if (key == "country")
                {
                    //m_GDS = formCollection[key].Trim();
                }
            }
            if (Session["Country"] != null)
            {
                m_GDS = Convert.ToString(Session["Country"]);  // formCollection[key];
            }
            else
            {
                m_GDS = "AU";
            }
            if (m_Surname == "" || m_Surname.ToLower() == "surname" || strValue == "" || m_GDS == "" || strValue == strBookingReference || strValue == strInvoiceNumber || strValue == strTicketNumber)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
            }

            if (radioValue == strBookingReference) //Booking reference search
            {
                strSearchOn = Common.RetrieveType.BookingReference;
                m_RLOC = strValue;

                int ReturnValue = DoGo(strSearchOn);
                if (ReturnValue == 1)
                {
                    //return RedirectToAction("ItineraryView", "Itinerary");
                }
                if (ReturnValue == 2)
                {
                    //return RedirectToAction("ItineraryView", "Itinerary");
                }
                if (ReturnValue == 3)
                {
                    return RedirectToAction("ItineraryView", "Itinerary", new { R = m_RLOC, S = Server.UrlEncode(m_Surname), G = m_GDS });
                }
                if (ReturnValue == 0)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 2 });
                }
            }
            else if (radioValue == strInvoiceNumber) //Invoice number search
            {
                strSearchOn = Common.RetrieveType.InvoiceNumber;
                m_InvoiceNumber = strValue;

                int ReturnValueInvoice = 0;
                ReturnValueInvoice = DoGoInvoice(strSearchOn);  //Under Development
                if (ReturnValueInvoice == 0)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 2 });
                    //return RedirectToAction("Message", "MessageDisplay", new { E = 100 });
                }
                if (ReturnValueInvoice == 1)
                {
                    //return RedirectToAction("Message", "MessageDisplay", new { E = 100 });
                    return RedirectToAction("InvoiceView", "Invoice", new { I = m_InvoiceNumber }); //Under Development
                }
            }
            else if (radioValue == strTicketNumber) //TIcket number search
            {
                strSearchOn = Common.RetrieveType.TicketNumber;
                m_EticketNumber = strValue;

                int ReturnValueTicket = DoGoTicket(strSearchOn);
                if (ReturnValueTicket == 0)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 23 });
                }
                else if (ReturnValueTicket == 1)
                {
                    return RedirectToAction("EticketView", "Eticket", new { T = "T", ET = m_InvoiceNumber });
                }
            }
            return View();
        }


        #endregion

        #region Helper Methods

        /// <summary>
        /// ChangePassword
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strOldPassword"></param>
        /// <param name="strNewPassword"></param>
        /// <returns></returns>
        public int ChangePasswordMethod(string strEmailAddress, string strOldPassword, string strNewPassword)
        {
            UsersBAL objUsersBAL = new UsersBAL();
            int successfullyUpdated = 0;
            try
            {
                List<UsersModel> lstUsersModel = objUsersBAL.GetPasswordByEmail(strEmailAddress);
                if (lstUsersModel != null && lstUsersModel.Count > 0)
                {
                    foreach (UsersModel obj in lstUsersModel)
                    {
                        if (obj.Password == strOldPassword)
                        {
                            obj.Password = strNewPassword;
                            obj.encodestring = "";
                            obj.Status = "confirm";
                            successfullyUpdated = objUsersBAL.SaveUser(obj);
                            if (successfullyUpdated == 1)
                            {
                                return 1;
                            }
                        }
                        else
                        {
                            return 0; // Either email address not registered or old password is incorrect.  
                            //  return RedirectToAction("Message", "MessageDisplay", new { E = 44 });
                        }
                    }
                }
                else
                {
                    return 0; // Either email address not registered or old password is incorrect.
                    //  return RedirectToAction("Message", "MessageDisplay", new { E = 44 });
                }
                return 0;
            }
            catch (Exception)
            {
                return 2;
            }
            finally
            {
                objUsersBAL = null;
            }
        }

        /// <summary>
        /// UserAlreadyExist
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int UserAlreadyExist(string strEmailAddress)
        {
            UsersBAL objUser = new UsersBAL();
            try
            {
                List<UsersModel> lstUsersModel = objUser.GetPasswordByEmail(strEmailAddress);
                if (lstUsersModel != null && lstUsersModel.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 2;
            }
            finally
            {
                objUser = null;
            }
        }

        /// <summary>
        /// UserAlreadyExist
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int ConfirmeRegistration(string strEncodeString)
        {
            UsersBAL objUser = new UsersBAL();
            try
            {
                List<UsersModel> lstUsersModel = objUser.GetConfirmeRegistration(strEncodeString);
                if (lstUsersModel != null && lstUsersModel.Count > 0)
                {
                    UpdateUserStatus(lstUsersModel[0].ID, "confirm", "");
                    Session["LoginEmailAddress"] = lstUsersModel[0].EmailAddress;
                    Session["LoginCurrentPassword"] = lstUsersModel[0].Password;
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 2;
            }
            finally
            {
                objUser = null;
            }
        }

        /// <summary>
        /// UserAlreadyExist
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UsersModel ResetEncodeStringExist(string strEncodeString)
        {
            UsersBAL objUser = new UsersBAL();
            try
            {
                UsersModel objUsersModel = objUser.ResetEncodeStringExist(strEncodeString);
                if (objUsersModel != null)
                {
                    return objUsersModel;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                objUser = null;
            }
        }



        /// <summary>
        /// RegisterNewUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int RegisterNewUser(string strEmailAddress, string password, string encodestring)
        {
            UsersBAL objUser = new UsersBAL();
            try
            {
                UsersModel objUsersModel = new UsersModel();
                objUsersModel.EmailAddress = strEmailAddress;
                objUsersModel.Password = password;
                objUsersModel.ID = 0;
                objUsersModel.Status = "pending";
                objUsersModel.encodestring = encodestring;
                int intNewUserRegistered = objUser.RegisterNewUSer(objUsersModel);
                //Send E-mail
                //SendEmail(Common.MailType.ConfirmRegistration, strEmailAddress);

                emailModel objEmailmodel = new emailModel();
                emailBAL objEmailBAL = new emailBAL();
                List<emailModel> lstemailmodel = objEmailBAL.FindEmail("MyTP-Confirm-Register-JAPA");

                if (lstemailmodel.Count > 0)
                {
                    DataTable DT = Common.ListToDataTable(lstemailmodel);
                    if (DT != null)
                    {

                        DataRow DR = DT.Rows[0];
                        string FileName = ConfigurationManager.AppSettings["EmailTemplatePath"] + Convert.ToString(DR["html_xslt_file"]);

                        //string buffer = Common.ReadFileFromDisk(FileName, ref logger);

                        XmlDocument xd = new XmlDocument();

                        xd.LoadXml("<tbdoc><encodestring>" + Server.UrlEncode(encodestring) + "</encodestring></tbdoc>");

                        string body = RunXSLTransform(FileName, xd).ToHtmlString();

                        string FromAddress = Convert.ToString(DR["from_address"]);
                        string FromName = Convert.ToString(DR["from_name"]);
                        string subject = Convert.ToString(DR["subject"]);
                        string cc = Convert.ToString(DR["cc"]);
                        string bcc = Convert.ToString(DR["bcc"]);

                        Common.SendEmail(FromAddress, FromName, strEmailAddress, cc, bcc, subject, body, true);
                    }
                }
                return intNewUserRegistered;
            }
            catch
            {
                return 2;
            }
            finally
            {
                objUser = null;
            }
        }


        private MvcHtmlString RunXSLTransform(string xslFile, XmlDocument xd)
        {
            //using (new Impersonator("woz", "", "2rachelle"))
           // {
                XslTransform xsl = new XslTransform();
                try
                {
                    xsl.Load(xslFile);
                }
                catch (XmlException e)
                {
                    throw (e);
                }
                catch (XsltException e)
                {
                    throw (e);
                }
                try
                {
                    StringWriter sw = new StringWriter();
                    xsl.Transform(xd, new System.Xml.Xsl.XsltArgumentList(), sw);
                    MvcHtmlString mvcHtml = new MvcHtmlString(sw.ToString());
                    return mvcHtml;
                }
                catch (XmlException e)
                {
                    throw (e);
                }
           // }
        }

        /// <summary>
        /// RegisterNewUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int UpdateUserStatus(Int32 UsersID, string strStatus, string encodestring)
        {
            UsersBAL objUser = new UsersBAL();
            try
            {
                int iUpdateSuccessfully = objUser.UpdateUserStatus(UsersID, strStatus, encodestring);
                return iUpdateSuccessfully;
            }
            catch
            {
                return 2;
            }
            finally
            {
                objUser = null;
            }
        }

        public int SendMailForRenewPassword(string strEmail)
        {

            try
            {
                UsersBAL objUser = new UsersBAL();
                UsersModel objUsersModel = objUser.FindUser(strEmail);

                if (objUsersModel == null)
                {
                    return 1;
                }


                //Update encode string in database
                objUser.UpdateUserStatus(objUsersModel.ID, objUsersModel.Status, Common.GetSHA1HashData(objUsersModel.EmailAddress));


                //Send email to user, from XSTL
                emailModel objEmailmodel = new emailModel();
                emailBAL objEmailBAL = new emailBAL();
                List<emailModel> lstemailmodel = objEmailBAL.FindEmail("MyTP-Confirm-Reset-JAPA");

                if (lstemailmodel.Count > 0)
                {
                    DataTable DT = Common.ListToDataTable(lstemailmodel);
                    if (DT != null)
                    {

                        DataRow DR = DT.Rows[0];
                        string FileName = ConfigurationManager.AppSettings["EmailTemplatePath"] + Convert.ToString(DR["html_xslt_file"]);
                        XmlDocument xd = new XmlDocument();
                        xd.LoadXml("<tbdoc><encodestring>" + Server.UrlEncode(Common.GetSHA1HashData(objUsersModel.EmailAddress)) + "</encodestring></tbdoc>");
                        string body = RunXSLTransform(FileName, xd).ToHtmlString();

                        string FromAddress = Convert.ToString(DR["from_address"]);
                        string FromName = Convert.ToString(DR["from_name"]);
                        string subject = Convert.ToString(DR["subject"]);
                        string cc = Convert.ToString(DR["cc"]);
                        string bcc = Convert.ToString(DR["bcc"]);

                        Common.SendEmail(FromAddress, FromName, objUsersModel.EmailAddress, cc, bcc, subject, body, true);
                        return 0;
                    }
                }

                return 2;
            }
            catch
            {
                return 2;
            }
        }


        /// <summary>
        /// MoveToSessionVars
        /// </summary>
        void MoveToSessionVars()
        {
            Session["RLOC"] = m_RLOC;
            Session["Surname"] = m_Surname;
            Session["GDS"] = m_GDS;
            Session["InvoiceNumber"] = m_InvoiceNumber;
        }

        /// <summary>
        /// DoGo
        /// </summary>
        /// <param name="objSender"></param>
        /// <param name="objArgs"></param>
        protected int DoGo(string strSearchOn)
        {
            Session["HTMLFileName"] = null;
            Session["Etickets"] = null;
            Session["Decoder"] = null;

            bool InvSummary = false;
            bool TKTSummary = false;

            if (strSearchOn == "INV")
                InvSummary = true;

            if (strSearchOn == "TKT")
                TKTSummary = true;

            if (m_RLOC.Length == 6 && m_Surname.Length > 0 && m_GDS.Length == 2)
            {
                this.MoveToSessionVars();
                string URL = "";

                // forward to Intinerary page
                if (InvSummary)
                {
                    //URL = "Invoice.aspx?T=V";
                }
                else if (TKTSummary)
                {
                    //URL = "Eticket.aspx?T=T";
                }
                else
                {
                    if (m_Type == "I")
                        return 1;
                    else if (m_Type == "V")
                        //URL = string.Format("Invoice.aspx?R={0}&S={1}&G={2}", Common.m_RLOC, Server.UrlEncode(Common.m_Surname), Common.m_GDS);
                        return 2;
                    else
                        return 3;
                }
            }
            return 0;
            //if (m_InvoiceNumber.Length > 0 && m_Surname.Length > 0)  //Need To work
            //    DoGoInvoice(objSender, objArgs);
            //// not all fields are correct
            //outError.InnerHtml = "Not all mandatory fields completed. Please correct and re-enter";

        }

        /// <summary>
        /// DoGoInvoice
        /// </summary>
        /// <param name="strSearchOn"></param>
        /// <returns></returns>
        protected int DoGoInvoice(string strSearchOn)
        {
            Session["HTMLFileName"] = null;
            Session["Etickets"] = null;

            if (m_InvoiceNumber != null)
            {
                if (m_InvoiceNumber.Substring(3, 1) == " ")
                {
                    // remove airline code
                    m_InvoiceNumber = m_InvoiceNumber.Replace(" ", "").Substring(3);
                }
                else
                    m_InvoiceNumber = m_InvoiceNumber.Replace(" ", "");

                this.MoveToSessionVars();
                return 1;
            }
            return 0;

            //string URL = "";
            // forward to Intinerary page
            //URL = string.Format("Invoice.aspx?I={0}", m_InvoiceNumber);
            //Response.Redirect(URL); //Need to work                        
            // not all fields are correct
            //   outError.InnerHtml = "Invoice number must have a numeric value and Surname is mandatory. Please correct and resend.";
        }

        /// <summary>
        /// DoGoTicket
        /// </summary>
        /// <param name="strSearchOn"></param>
        /// <returns></returns>
        protected int DoGoTicket(string strSearchOn)
        {
            Session["HTMLFileName"] = null;
            Session["Etickets"] = null;

            if (m_EticketNumber.Length > 0 && m_Surname.Length > 0)
            {
                m_InvoiceNumber = m_EticketNumber; //.Replace(" ", "");

                //if (m_InvoiceNumber.Length > 10) //Need To Work-Discussed With Phil
                //{
                //    // take the last 10 chars
                //    m_InvoiceNumber = m_InvoiceNumber.Substring(m_EticketNumber.Length - 10);
                //}

                this.MoveToSessionVars();
                return 1;

                //string URL = "";
                // forward to Intinerary page
                //URL = string.Format("Invoice.aspx?I={0}", m_InvoiceNumber);
                //URL = "Eticket.aspx?T=T&ET=" + m_InvoiceNumber;                
                //Response.Redirect(URL);
            }
            return 0;
        }

        #endregion

    }
}
