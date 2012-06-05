﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTP.BAL;
using MTP.DTO;
using MTP_JAPA.Helpers;
using NLog;
using NLog.Config;
using System.Text.RegularExpressions;
using System.IO;
using mshtml;
using System.Xml;
using Travelbytes;

namespace MTP_JAPA.Controllers
{
    public class ItineraryController : BaseController
    {
        #region Variables

        const string FileExtension = ".doc";
        const string FileExtensionXML = ".xml";
        protected string m_FileName = "";
        protected string m_FormName = "";
        bool AllowLiveRetrieve = false;
        string CreditSuissesgCompany = "CREDITSUISSESG";


        #endregion

        #region Load

        /// <summary>
        /// ItineraryView
        /// </summary>
        /// <returns></returns>
        public ActionResult ItineraryView()
        {
            ViewBag.TabDisplay = 1;
            ViewBag.theGoClick = 1;
            Session["theGoClick"] = 1;
            string TemplatesPath = Server.MapPath(@"\Content\Templates\").Replace("\\", "/");
            string CSSPath = Server.MapPath(@"\Content\static\css\").Replace("\\", "/");
            string TempPath = Server.MapPath(@"\Temp\").Replace("\\", "/");

            string strCompany = "";
            if (Request.QueryString["R"] != null)
            {
                Session["RLOC"] = Request.QueryString["R"];
            }
            if (Request.QueryString["S"] != null)
            {
                Session["Surname"] = Request.QueryString["S"];
            }
            if (Request.QueryString["G"] != null)
            {
                Session["GDS"] = Request.QueryString["G"];
            }

            //Decoder-----------------------

            int Retry = 0;
            if (Session["RetryCount"] != null)
            {
                Retry = ((int)Session["RetryCount"]);

                if (Retry >= GetRetryCount())
                {
                    Session["RetryCount"] = GetRetryCount();
                    return RedirectToAction("Message", "MessageDisplay", new { E = 5 });
                    //Response.Redirect("error.aspx?E=5");
                    //Response.End();
                }
            }
            //Decoder--------------------------

            bPrint = false;
            if (Request.QueryString["L"] != null)
            {
                bPrint = true;
            }
            ViewBag.bPrint = bPrint;

            //Decoder-----------------------------------------------------------------------------------------------------            
            string str = "";
            Travelbytes.WebService.Itinerary.Decoder Decoder = null;

            if (Request.QueryString["L"] != null)
            {
                if (Request.QueryString["L"] == "R" && Session["Decoder"] != null)
                {
                    Decoder = (Travelbytes.WebService.Itinerary.Decoder)Session["Decoder"];
                    Decoder.Reload = true;
                    Decoder.TempLocation = TempPath;//Server.MapPath("Temp");
                    Decoder.Config = Server.MapPath("");
                    Decoder.PassedCountry = m_GDS;
                    Decoder.Offset = GetOffSet();
                    Decoder.Style = CSSPath; //Server.MapPath("styles/");

                    str = Decoder.GetItinerary(m_RLOC);
                    ViewBag.pax = Decoder.Pax;
                    //Pax.InnerHtml = Decoder.Pax;
                    //RLOC.InnerText = Decoder.RLOC;
                    //Agent.InnerText = Decoder.Agent.Replace("(", "");
                    //ClientAddress.InnerHtml = Decoder.ClientAddress;
                    //AgencyAddress.InnerHtml = Decoder.AgencyAddress;
                    ViewBag.RLOC = Decoder.RLOC;
                    ViewBag.Agent = Decoder.Agent.Replace("(", "");
                    ViewBag.Agent = ViewBag.Agent.Replace(")", "");
                    ViewBag.ClientAddress = Decoder.ClientAddress;
                    ViewBag.AgencyAddress = Decoder.AgencyAddress;

                    //---------------------------------------------------------------------------------------------------------------------------------------//

                    if (Decoder.m_bkg != null)
                    {
                        foreach (Travelbytes.Common.REMARKITEM itm in Decoder.m_bkg.Remarks)
                        {
                            if (itm.m_REMARKITEM.Trim().ToLower().IndexOf("company") > 0)
                            {
                                string Remarks2 = itm.m_REMARKITEM.Trim();
                                string[] GetCompany = Remarks2.Split(':');
                                if (GetCompany.Length > 0)
                                {
                                    if (GetCompany[0].Trim().ToLower().IndexOf("company") > 0)
                                    {
                                        if (GetCompany[0].Trim().ToLower() == "qi 8 company" || GetCompany[0].Trim().ToLower() == "qi 9 company")
                                        {
                                            strCompany = GetCompany[1].Trim();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string strUpdatedHTMLString = "";
                    if (strCompany == CreditSuissesgCompany)
                    {
                        strUpdatedHTMLString = GetUpdatedHTMLString(Decoder.m_Response, str);
                        strUpdatedHTMLString = GetComplianceDataAttached(strUpdatedHTMLString);
                        strUpdatedHTMLString = ReplaceLinks(strUpdatedHTMLString);
                    }
                    else
                    {
                        strUpdatedHTMLString = str;
                        strUpdatedHTMLString = ReplaceLinks(strUpdatedHTMLString);
                    }


                    if (!string.IsNullOrEmpty(strUpdatedHTMLString))
                    {
                        //results.InnerHtml = strUpdatedHTMLString;
                        ViewBag.results = strUpdatedHTMLString;
                    }
                    else
                    {
                        //results.InnerHtml = str;
                        ViewBag.results = strUpdatedHTMLString;
                    }
                    //---------------------------------------------------------------------------------------------------------------------------------------//

                    return View();
                }
                if (Request.QueryString["L"] == "P" && Session["Decoder"] != null)
                {
                    try
                    {
                        bPrint = true;
                        ViewBag.bPrint = bPrint;
                        // requested print Itin
                        if (Session["Decoder"] == null)
                        {
                            //results.InnerHtml = "Failed to retrieve booking. No previous Itinerary retrieved";
                            ViewBag.results = "Failed to retrieve booking. No previous Itinerary retrieved";
                            return View();
                        }

                        Decoder = (Travelbytes.WebService.Itinerary.Decoder)Session["Decoder"];
                        Decoder.Reload = true;
                        Decoder.Print = bPrint;
                        Decoder.Config = Server.MapPath("");
                        Decoder.PassedCountry = m_GDS;
                        Decoder.TempLocation = TempPath; //Server.MapPath("Temp");
                        Decoder.Offset = GetOffSet();
                        Decoder.Style = CSSPath;//Server.MapPath("styles/");

                        str = Decoder.GetItinerary(m_RLOC);
                        ViewBag.pax = Decoder.Pax;
                        //Pax.InnerHtml = Decoder.Pax;
                        //RLOC.InnerText = Decoder.RLOC;
                        //Agent.InnerText = Decoder.Agent.Replace("(", "");
                        //ClientAddress.InnerHtml = Decoder.ClientAddress;
                        //AgencyAddress.InnerHtml = Decoder.AgencyAddress;
                        ViewBag.RLOC = Decoder.RLOC;
                        ViewBag.Agent = Decoder.Agent.Replace("(", "");
                        ViewBag.Agent = ViewBag.Agent.Replace(")", "");
                        ViewBag.ClientAddress = Decoder.ClientAddress;
                        ViewBag.AgencyAddress = Decoder.AgencyAddress;
                        //Pax.InnerHtml = Decoder.Pax;
                        //RLOC.InnerText = Decoder.RLOC;
                        //Agent.InnerText = Decoder.Agent;
                        //ClientAddress.InnerHtml = Decoder.ClientAddress;
                        //AgencyAddress.InnerHtml = Decoder.AgencyAddress;
                        //results.InnerHtml = Decoder.Itinerary;
                        //results.InnerHtml = str;
                        //---------------------------------------------------------------------------------------------------------------------------------------//
                        if (Decoder.m_bkg != null)
                        {
                            foreach (Travelbytes.Common.REMARKITEM itm in Decoder.m_bkg.Remarks)
                            {
                                if (itm.m_REMARKITEM.Trim().ToLower().IndexOf("company") > 0)
                                {
                                    string Remarks2 = itm.m_REMARKITEM.Trim();
                                    string[] GetCompany = Remarks2.Split(':');
                                    if (GetCompany.Length > 0)
                                    {
                                        if (GetCompany[0].Trim().ToLower().IndexOf("company") > 0)
                                        {
                                            if (GetCompany[0].Trim().ToLower() == "qi 8 company" || GetCompany[0].Trim().ToLower() == "qi 9 company")
                                            {
                                                strCompany = GetCompany[1].Trim();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        string strUpdatedHTMLSession = "";
                        if (strCompany == CreditSuissesgCompany)
                        {
                            strUpdatedHTMLSession = GetUpdatedHTMLString(Decoder.m_Response, str);
                            strUpdatedHTMLSession = GetComplianceDataAttached(strUpdatedHTMLSession);
                            strUpdatedHTMLSession = ReplaceLinks(strUpdatedHTMLSession);
                        }
                        else
                        {
                            strUpdatedHTMLSession = str;
                            strUpdatedHTMLSession = ReplaceLinks(strUpdatedHTMLSession);
                        }

                        if (!string.IsNullOrEmpty(strUpdatedHTMLSession))
                        {
                            // results.InnerHtml = strUpdatedHTMLSession;
                            ViewBag.results = strUpdatedHTMLSession;
                        }
                        else
                        {
                            //results.InnerHtml = str;
                            ViewBag.results = str;
                        }
                        //---------------------------------------------------------------------------------------------------------------------------------------//

                        //PrintLayer.Visible = true;
                    }
                    catch (Exception p)
                    {
                        //results.InnerHtml = "Failed to retrieve booking. Error is<br>" + p.ToString() +
                        //    "<br>" + p.StackTrace;
                        ViewBag.results = "Failed to retrieve booking. Error is<br>" + p.ToString() +
                           "<br>" + p.StackTrace;
                    }
                    return View();
                }

            }
            //Decoder-----------------------------------------------------------------------------------------------------            


            LoadSessionVars();
            GetQueryVars();

            //--First Find from new Itinerary-------------------------

            ItineraryDataBAL objItineraryDataBAL = new ItineraryDataBAL();
            List<XmlBookingModel> lstQiLiveItineraryData = objItineraryDataBAL.SearchItineraryDataBySurnameAndRLOC(m_Surname, m_RLOC);
            if (lstQiLiveItineraryData != null && lstQiLiveItineraryData.Count > 0)
            {
                Session["QiLiveItineraryData"] = lstQiLiveItineraryData;
                return RedirectToAction("QiLiveItineraryView", "QiLiveItinerary", new { R = m_RLOC, S = m_Surname });
            }
            //else
            //{
            //    Session["QiLiveItineraryData"] = null;
            //    return RedirectToAction("QiLiveItineraryView", "QiLiveItinerary", new { R = m_RLOC, S = m_Surname });
            //}


            //--If Not Found in new Itinerary then search in old system-------------------------


            Session["QiLiveItineraryData"] = null;
            if (CheckForStorage())
            {
                if (!AllowLiveRetrieve)
                {
                    // decrypt and load redirect out
                    if (m_FileName.Length == 0)
                        return RedirectToAction("Message", "MessageDisplay", new { E = 14 }); //Response.Redirect("Error.aspx?E=14");

                    int rValue = GetTransaction(m_FileName);
                    if (rValue == 0)
                    {
                        // only get here if there was an error
                        return RedirectToAction("Message", "MessageDisplay", new { E = 14 });
                    }
                    else if (rValue == 1)
                    {
                        if (bPrint == true)
                        {
                            return RedirectToAction("Itin", "Itinerary", new { R = "ITIN", FN = m_FormName, L = "P" }); //Response.Redirect("Itin.aspx?R=ITIN&FN=" + m_FormName, true);                        
                        }
                        else
                        {
                            return RedirectToAction("Itin", "Itinerary", new { R = "ITIN", FN = m_FormName }); //Response.Redirect("Itin.aspx?R=ITIN&FN=" + m_FormName, true);                                                
                        }

                    }
                    else if (rValue == 2)
                    {
                        if (bPrint == true)
                        {
                            return RedirectToAction("Itin", "Itinerary", new { R = "ITIN", L = "P" }); //Response.Redirect("Itin.aspx?R=ITIN", true);                        
                        }
                        else
                        {
                            return RedirectToAction("Itin", "Itinerary", new { R = "ITIN" }); //Response.Redirect("Itin.aspx?R=ITIN", true);                        

                        }
                    }
                    else if (rValue == 3)
                    {

                    }
                }
            }
            else
            {
                string Country = "";
                if (Session["Country"] != null)
                    Country = Session["Country"].ToString();

                string CountryList = GetLiveCountryList();

                if ((Session["Country"] == null || GetLiveCountryList().IndexOf(Country) == -1))
                {
                    // Goto error                    
                    return RedirectToAction("Message", "MessageDisplay", new { E = 14 });
                }
            }
            //Decoder-----------------------------------------------------------------------------------------------------------------------------

            if (m_RLOC == null || m_RLOC.Length == 0)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 1 });
                //Response.Redirect("Error.aspx?E=1");
                //return view;
            }


            if (Session["Decoder"] != null)
            {
                Decoder = (Travelbytes.WebService.Itinerary.Decoder)Session["Decoder"];
                Decoder.Reload = true;
                Decoder.TempLocation = TempPath; //Server.MapPath("Temp");
                Decoder.Config = Server.MapPath("");
                Decoder.PassedCountry = m_GDS;
                Decoder.Offset = GetOffSet();
                Decoder.Style = CSSPath; //Server.MapPath("styles/");

                str = Decoder.GetItinerary(m_RLOC);
                str = str.Replace("Download to calendar", "");
                ViewBag.RLOC = Decoder.RLOC;
                ViewBag.Agent = Decoder.Agent.Replace("(", "");
                ViewBag.Agent = ViewBag.Agent.Replace(")", "");
                ViewBag.ClientAddress = Decoder.ClientAddress;
                ViewBag.AgencyAddress = Decoder.AgencyAddress;
                //Pax.InnerHtml = Decoder.Pax;
                //RLOC.InnerText = Decoder.RLOC;
                //Agent.InnerText = Decoder.Agent.Replace("(", "");
                //Agent.InnerText = Agent.InnerText.Replace(")", "");
                //ClientAddress.InnerHtml = Decoder.ClientAddress;
                //AgencyAddress.InnerHtml = Decoder.AgencyAddress;

                //---------------------------------------------------------------------------------------------------------------------------------------//
                if (Decoder.m_bkg != null)
                {
                    foreach (Travelbytes.Common.REMARKITEM itm in Decoder.m_bkg.Remarks)
                    {
                        if (itm.m_REMARKITEM.Trim().ToLower().IndexOf("company") > 0)
                        {
                            string Remarks2 = itm.m_REMARKITEM.Trim();
                            string[] GetCompany = Remarks2.Split(':');
                            if (GetCompany.Length > 0)
                            {
                                if (GetCompany[0].Trim().ToLower().IndexOf("company") > 0)
                                {
                                    if (GetCompany[0].Trim().ToLower() == "qi 8 company" || GetCompany[0].Trim().ToLower() == "qi 9 company")
                                    {
                                        strCompany = GetCompany[1].Trim();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                string strUpdatedHTMLFromSession = "";
                if (strCompany == CreditSuissesgCompany)
                {
                    strUpdatedHTMLFromSession = GetUpdatedHTMLString(Decoder.m_Response, str);
                    strUpdatedHTMLFromSession = GetComplianceDataAttached(strUpdatedHTMLFromSession);
                    strUpdatedHTMLFromSession = ReplaceLinks(strUpdatedHTMLFromSession);
                }
                else
                {
                    strUpdatedHTMLFromSession = str;
                    strUpdatedHTMLFromSession = ReplaceLinks(strUpdatedHTMLFromSession);
                }


                if (!string.IsNullOrEmpty(strUpdatedHTMLFromSession))
                {
                    //results.InnerHtml = strUpdatedHTMLFromSession;
                    ViewBag.results = strUpdatedHTMLFromSession;
                }
                else
                {
                    //results.InnerHtml = str;
                    ViewBag.results = str;
                }
                //---------------------------------------------------------------------------------------------------------------------------------------//

                //results.InnerHtml = Decoder.Itinerary;
                //results.InnerHtml = str;
                return View();
            }


            logger.Debug("Live Retrievel Start - Debug Point 1");
            Decoder = new Travelbytes.WebService.Itinerary.Decoder();

            Decoder.Reload = false;
            Decoder.Print = bPrint;
            Decoder.TempLocation = TempPath; //Server.MapPath("Temp");
            Decoder.Config = Server.MapPath("");
            Decoder.Templates = TemplatesPath;
            Decoder.AddressDSN = GetAddressDataSourceMySQL();
            Decoder.PassedCountry = m_GDS;
            Decoder.Style = CSSPath; //Server.MapPath("styles/");

            Credentials Cred = GetUser("Galileo");
            Decoder.User = Cred.User;
            Decoder.PSW = Cred.PSW;
            Decoder.HAP = Cred.HAP;
            Decoder.URL = Cred.URL;
            Decoder.ItineraryURL = Cred.ItineraryURL;
            Decoder.Offset = GetOffSet();

            Tranx Tran = new Tranx();
            Tran.ItineraryRetrieve(m_RLOC, m_Surname, "");

            logger.Debug("Debug Point 2");

            //Decoder.RLOC = m_RLOC;
            str = "";
            str = Decoder.GetItinerary(m_RLOC);

            logger.Debug("Debug Point 3");

            if (str.Length == 0)
            {
                logger.Debug("Debug Point 4 : " + Decoder.LastError);
                if (Decoder.LastErrorNumber != 0)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 34 });
                    //Response.Redirect("Error.aspx?E=" + Decoder.LastErrorNumber);
                }
                else if (str.IndexOf("Invalid reference") != -1)
                    return RedirectToAction("Message", "MessageDisplay", new { E = 7 });
                //Response.Redirect("Error.aspx?E=7");
                else
                {
                    if (Decoder.LastError.Length == 0)
                        return RedirectToAction("Message", "MessageDisplay", new { E = 14 });
                    //Response.Redirect("Error.aspx?E=14");
                    else
                        return RedirectToAction("Message", "MessageDisplay", new { E = 35 });
                    //return RedirectToAction("Message", "MessageDisplay", new { E = 22 });                    
                    //Response.Redirect("Error.aspx?E=0&M=" + Server.UrlEncode(Decoder.LastError));
                }
            }

            logger.Debug("Debug Point 5");

            str = str.Replace("Download to calendar", "");
            Session["SMSSummary"] = Decoder.SMSSummary;
            if ((base.HasEticket = Decoder.HasTickets) == true)
            {
                Session["Etickets"] = Decoder.TicketNumbers;
            }


            // check the surname matches the one provided
            if (m_Surname.Length == 0 || Decoder.Surname.ToUpper().IndexOf(m_Surname.ToUpper()) == -1)
            {

                if (Session["RetryCount"] == null)
                    Retry = 0;
                else
                    Retry = ((int)Session["RetryCount"]) + 1;

                if (Retry >= GetRetryCount())
                {
                    Session["RetryCount"] = GetRetryCount();
                    m_RLOC = "";
                    return RedirectToAction("Message", "MessageDisplay", new { E = 5 });
                    //Response.Redirect("error.aspx?E=5");
                    //Response.End();
                }
                Session["RetryCount"] = Retry;
                // failed to match
                return RedirectToAction("Message", "MessageDisplay", new { E = 2 });
                //Response.Redirect("Error.aspx?E=2");
                //Response.End();
            }

            Session["RetryCount"] = 0;
            GetReference = m_RLOC;
            GetName = Decoder.Pax;

            Session["RLOC"] = m_RLOC;
            Session["Name"] = Decoder.PaxName;

            ViewBag.RLOC = Decoder.RLOC;
            ViewBag.Agent = Decoder.Agent;
            ViewBag.ClientAddress = Decoder.ClientAddress;
            ViewBag.AgencyAddress = Decoder.AgencyAddress;
            //ClientAddress.InnerHtml = Decoder.ClientAddress;
            //AgencyAddress.InnerHtml = Decoder.AgencyAddress;
            //Pax.InnerHtml = Decoder.Pax;
            //RLOC.InnerText = Decoder.RLOC;
            //Agent.InnerText = Decoder.Agent;

            //---------------------------------------------------------------------------------------------------------------------------------------//

            if (Decoder.m_bkg != null)
            {
                foreach (Travelbytes.Common.REMARKITEM itm in Decoder.m_bkg.Remarks)
                {
                    if (itm.m_REMARKITEM.ToLower().IndexOf("company") > 0)
                    {
                        string Remarks2 = itm.m_REMARKITEM;
                        string[] GetCompany = Remarks2.Split(':');
                        if (GetCompany.Length > 0)
                        {
                            if (GetCompany[0].ToLower().IndexOf("company") > 0)
                            {
                                if (GetCompany[0].Trim().ToLower() == "qi 8 company" || GetCompany[0].Trim().ToLower() == "qi 9 company")
                                {
                                    strCompany = GetCompany[1].Trim();
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            string strUpdatedHTML = "";
            if (strCompany == CreditSuissesgCompany)
            {
                strUpdatedHTML = GetUpdatedHTMLString(Decoder.m_Response, str);
                strUpdatedHTML = GetComplianceDataAttached(strUpdatedHTML);
                strUpdatedHTML = ReplaceLinks(strUpdatedHTML);
            }
            else
            {
                strUpdatedHTML = str;
                strUpdatedHTML = ReplaceLinks(strUpdatedHTML);
            }
            //---------------------------------------------------------------------------------------------------------------------------------------//

            if (!string.IsNullOrEmpty(strUpdatedHTML))
            {
                //results.InnerHtml = strUpdatedHTML;
                ViewBag.results = strUpdatedHTML;
            }
            else
            {
                //results.InnerHtml = str;
                ViewBag.results = str;
            }

            //results.InnerHtml = str;
            Session["Decoder"] = Decoder;


            //Decoder-----------------------------------------------------------------------------------------------------------------------------


            return View();
        }

        /// <summary>
        /// Itin
        /// </summary>
        /// <returns></returns>
        public ActionResult Itin()
        {
            // Put user code to initialize the page here
            ViewBag.TabDisplay = 1;
            try
            {
                bPrint = false;
                if (Request.QueryString["L"] != null)
                {
                    bPrint = true;
                }
                ViewBag.bPrint = bPrint;

                ViewBag.theGoClick = 1;
                Session["theGoClick"] = 1;

                if (Request.QueryString["R"] == null)
                {
                    logger.Error("Itin: No query string");
                    return RedirectToAction("Message", "MessageDisplay", new { E = 14 }); //Response.Redirect("Error.aspx?E=14");                                         
                }
                if (Request.QueryString["FN"] != null)
                {
                    m_FormName = Request.QueryString["FN"].ToString();
                }

                if (Session["HTMLFileName"] == null)
                {
                    logger.Error("Itin: No FileName provided");
                    return RedirectToAction("Message", "MessageDisplay", new { E = 14 });
                }

                m_FileName = Session["HTMLFileName"].ToString();
                GetTransactionItin(m_FileName);
                return View("Itin");
            }
            catch (Exception e1)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 22 });
                //results.InnerHtml = "Failed to retrieve booking. Error is<br>" + e1.Message; //Need to work
                //return;
            }
            return View();
        }

        #endregion

        #region Helper Methods

        #region Itinerary View

        /// <summary>
        /// CheckForStorage
        /// </summary>
        /// <returns></returns>
        bool CheckForStorage()
        {
            return SearchDataBaseByRLOC(m_RLOC, m_Surname.ToUpper());
        }

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <param name="RLOC"></param>
        /// <param name="Surname"></param>
        /// <returns></returns>
        bool SearchDataBaseByRLOC(string RLOC, string strSurname)
        {
            try
            {
                bool rv = false;
                DateTime tm = DateTime.Now.AddMonths(GetSeachMonths());
                DateTime ThreeMonths = new DateTime(tm.Year, tm.Month, 1, 0, 0, 0, 0);

                MailRecordsBAL objMailRecordsBAL = new MailRecordsBAL("");
                List<MailRecordsModel> lstMailRecordsModel = objMailRecordsBAL.SearchDataBaseByRLOC(RLOC, ThreeMonths, strSurname);
                if (lstMailRecordsModel == null)
                {
                    logger.Debug("Itinerary: No records found in mailrecords table matching criteria");
                    return rv;
                }

                DataSet DS = new DataSet();
                DataTable dt = Common.ListToDataTable(lstMailRecordsModel);
                DS.Tables.Add(dt);

                if (DS == null || DS.Tables == null || DS.Tables[0].Rows.Count == 0)
                {
                    logger.Debug("Itinerary: No records found in mailrecords table matching criteria");
                    return rv;
                }

                string Created = "";
                string LastSubject = "";

                logger.Debug("Itinerary: Mailrecords found : " + DS.Tables[0].Rows.Count.ToString());
                string[] ItinerarySearch = GetItinerarySearch().Split(',');
                string[] ItineraryProcessorNames = GetItineraryProcessorNames().Split(',');
                AllowLiveRetrieve = false;

                foreach (DataRow myRow in DS.Tables[0].Rows)
                {
                    string Subject = myRow["Subject"].ToString().ToUpper();
                    string FormName = myRow["FormName"].ToString().ToUpper();
                    string ProcessorName = myRow["ProcessorName"].ToString().ToUpper();

                    foreach (string str in ItineraryProcessorNames)
                    {
                        if (ProcessorName == str)
                        {
                            AllowLiveRetrieve = true;
                            return true;
                        }
                    }

                    bool Found = false;
                    foreach (string str in ItinerarySearch)
                    {
                        if (Subject.IndexOf(str) != -1)
                        {
                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                        continue;

                    Created = myRow["Created"].ToString();
                    LastSubject = Subject;

                    // look for matching surname
                    Match Ma = Regex.Match(Subject.ToUpper(), GetNameCommand());
                    if (!Ma.Success)
                        continue;

                    string[] Names;

                    if (Ma.Groups["detail"].ToString().IndexOf("_") != -1)
                    {
                        Names = Ma.Groups["detail"].ToString().Split('_');
                    }
                    else
                    {
                        Names = Ma.Groups["detail"].ToString().Split('/');
                    }

                    string newName = Names[0].Trim();

                    //  if (newName == Surname)
                    if (LastSubject.IndexOf(strSurname) != -1)
                    {

                        string ID = myRow["ID"].ToString();

                        if (myRow["FileName"] == null || myRow["FileName"].ToString().Length == 0)
                            continue;
                        m_FileName = myRow["FileName"].ToString().ToUpper();
                        m_FormName = myRow["FormName"].ToString().ToUpper();

                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            finally
            {

            }
            return false;
        }

        /// <summary>
        /// GetTransaction
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        protected int GetTransaction(string FileName)
        {
            //bool rv = false;

            int intReturnValue = 0;
            // Get the file
            string myLocation = Server.MapPath("/Temp");
            logger.Debug("Retrieving {0}", FileName);
            intReturnValue = CreateDisplayFile(FileName.Replace(" ", ""), myLocation + "\\" + Session.SessionID + FileExtension);
            return intReturnValue;
        }

        /// <summary>
        /// CreateDisplayFile
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Destination"></param>
        /// <returns></returns>
        public int CreateDisplayFile(string Source, string Destination)
        {
            logger.Debug("CreateDisplayFile debug point 01");

            int Status = 0;
            string InFileName;
            InFileName = Source;

            Source = Source.ToLower();
            string TempDir = Server.MapPath("/Temp");

            TempDir = @"c:\temp";
            if (!TempDir.EndsWith("\\"))
                TempDir += "\\";

            string WebTempDir = Server.MapPath("/Temp");
            if (!WebTempDir.EndsWith("\\"))
                WebTempDir += "\\";

            string szSource = TempDir + Session.SessionID + ".txt";
            string szSource1 = TempDir + Session.SessionID + FileExtension;

            if (GetDebug() && GetAltDebug())
            {
                Source = Source.ToLower().Replace(@"203.19.215.74\itineraries\", @"203.19.215.74\cdrive\travelbytes\itineraries\");
            }

            // copy infile to local drive            
            try
            {
                //using (new Impersonator("woz", "", "2rachelle"))
               // {
                    logger.Debug("Copying " + Source + " to " + szSource + "");
                    System.IO.File.Copy(Source, szSource, true);
                    logger.Debug("File copied");
                //}
            }
            catch (System.ComponentModel.Win32Exception ex32)
            {
                logger.Error("Error impersonating user : " + ex32.Message);
                m_LastError = "Error impersonating user";
                return 0;
            }
            catch (Exception e)
            {
                logger.Error("File not found : " + e.Message);
                m_LastError = "File not found";
                return 0;
            }
            int Type = Common.CheckFileType(szSource);
            if (Source.IndexOf(".htm") == -1 && Type != 6)
            {
                logger.Debug("Decrypting file");
                Status = FileDecrypt(szSource, ref szSource1);
                logger.Debug("Decrypted file");
                // System.IO.File.Delete(szSource);
                logger.Debug("Deleted " + szSource);

                if (szSource1.ToLower().IndexOf(".htm") != -1)
                {
                    Session["HTMLFileName"] = szSource1;
                    logger.Debug("Returning");
                    return 1;
                    //Response.Redirect("Itin.aspx?R=ITIN&FN=" + m_FormName, true);
                }
            }
            else
            {
                try
                {
                    szSource1 = szSource1.Replace(FileExtension, ".htm");
                    Destination = Destination.Replace(FileExtension, ".htm");
                    System.IO.File.Copy(Source, szSource1, true);
                    Session["HTMLFileName"] = szSource1;
                    return 2;
                    //Response.Redirect("Itin.aspx?R=ITIN", true);
                }
                catch
                {
                    logger.Error("File not found");
                    m_LastError = "File not found";
                    return 0;
                }
            }

            if (Status > 0)
            {
                try
                {
                    if (szSource1.EndsWith(".pdf"))
                    {
                        Destination = Destination.Replace(FileExtension, ".pdf");
                    }
                    else if (szSource1.EndsWith(".xml"))
                    {
                        Destination = Destination.Replace(FileExtension, ".xml");
                    }
                    else if (szSource1.EndsWith(".htm"))
                    {
                        Destination = Destination.Replace(FileExtension, ".htm");
                        Session["HTMLFileName"] = Destination;
                    }
                    else if (szSource1.EndsWith(".doc"))
                    {
                        Destination = Destination.Replace(FileExtension, ".doc");
                    }
                    else if (szSource1.EndsWith(".rtf"))
                    {
                        Destination = Destination.Replace(FileExtension, ".rtf");
                    }
                    System.IO.File.Copy(szSource1, Destination, true);
                    System.IO.File.Delete(szSource1);

                }
                catch
                {
                    logger.Error("Failed to copy file to destination ");
                    m_LastError = "File not found";
                    return 0;
                }
                logger.Debug("Decrypt Success redirecting to : " + Destination);
                //Response.Redirect(Destination.Replace(Server.MapPath("") + "\\", ""), true);  //Need To Work 
            }

            if (Status != 1)
            {
                logger.Error("Error decrypting file, " + m_LastError);
                return 0;
            }
            return 5;  //just to return interger value,5 not mean anything 
        }

        /// <summary>
        /// FileDecrypt
        /// </summary>
        /// <param name="InFile"></param>
        /// <param name="OutFile"></param>
        /// <returns></returns>
        protected int FileDecrypt(string InFile, ref string OutFile)
        {
            int Status = 0;
            try
            {
                logger.Debug("FileDecrypt: Debug point 01");
                m_LastError = "";
                WOZCRYPTOLib.IFile Crypto = new WOZCRYPTOLib.FileClass();
                logger.Debug("FileDecrypt: Debug point 02");

                Crypto.InFileName = InFile;
                Crypto.OutFileName = OutFile;

                logger.Debug("Decrypt : " + InFile + " to " + OutFile);

                Status = Crypto.Decrypt();
                m_LastError = Crypto.LastError;
            }
            catch (Exception ex)
            {
                logger.Debug("Error in File Decrypt : " + ex.Message);
            }



            if (Status > 0)
            {
                int FileType = Common.CheckFileType(OutFile);
                string newOutputFile = OutFile;
                switch (FileType)
                {
                    case 4:
                        newOutputFile = OutFile.Replace(FileExtension, ".rtf");

                        if (System.IO.File.Exists(newOutputFile))
                            System.IO.File.Delete(newOutputFile);

                        System.IO.File.Move(OutFile, newOutputFile);
                        OutFile = newOutputFile;
                        break;
                    case 0x20:
                        newOutputFile = OutFile.Replace(FileExtension, ".pdf");
                        if (System.IO.File.Exists(newOutputFile))
                            System.IO.File.Delete(newOutputFile);

                        System.IO.File.Move(OutFile, newOutputFile);
                        OutFile = newOutputFile;
                        break;
                    case 6:
                        newOutputFile = OutFile.Replace(FileExtension, ".htm");
                        if (System.IO.File.Exists(newOutputFile))
                            System.IO.File.Delete(newOutputFile);

                        System.IO.File.Move(OutFile, newOutputFile);
                        OutFile = newOutputFile;
                        break;
                    default:
                        break;


                }

                logger.Debug("Decrypted : " + m_LastError);

            }
            else
                logger.Debug("Decrypt failed : " + m_LastError);

            return Status;
        }

        #endregion

        #region Itin

        /// <summary>
        /// GetTransactionItin
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        protected bool GetTransactionItin(string FileName)
        {
            bool rv = false;
            // Get the file
            string myLocation = Server.MapPath("/Temp");
            logger.Debug("Retrieving {0}", FileName);
            rv = CreateDisplayFile(FileName);
            return rv;
        }

        /// <summary>
        /// CreateDisplayFile
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool CreateDisplayFile(string FileName)
        {
            try
            {
                string buffer = Common.ReadFileFromDisk(FileName, ref logger);

                logger.Debug("Fixing HTML");
                string bufferfixed = FixHTML(buffer);

                ViewBag.ItinContent = bufferfixed;
                //ItinContent.InnerHtml = bufferfixed;
                return true;
            }
            catch (Exception e)
            {
                logger.Error("File not found :" + e.Message);
                m_LastError = "File not found";
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlToParse"></param>
        /// <returns></returns>
        string FixHTML(string htmlToParse)
        {
            try
            {
                //::......... Declare a new HTML document to use, and write our normal HTML
                IHTMLDocument2 htmlDocument = new HTMLDocumentClass();

                htmlDocument.write(htmlToParse);
                htmlDocument.close();

                //::......... With this we retrieve all of the HTML elements collection existing on out HTML block
                IHTMLElementCollection allElements = (IHTMLElementCollection)htmlDocument.body.all;

                //IHTMLImgElement Logo = (IHTMLImgElement)allElements.item((object)"myTextBox", (object)0);

                //::......... Find by name out INPUT element on the group, and set a new value
                //object Number = 0;
                //IHTMLInputElement myTextBox = (IHTMLInputElement)allElements.item((object)"myTextBox", (object)Number);
                //myTextBox.value = "This is my text box!";

                //::......... Our button, but now its a "IHTMLElement", the generic object, that gives us more properties
                //::......... And set a new attribute to our element

                //IHTMLElement myButton = (IHTMLElement)allElements.item("myButton", 0);
                //myButton.setAttribute("onClick", "javascript:alert(//This is the button!//)", 0);

                //::......... As a input, we set its value
                //IHTMLInputElement myButton2 = (IHTMLInputElement)allElements.item("myButton", Number);
                //myButton2.value = "Click me!";

                //::......... Get the INPUT group of elements
                IHTMLElementCollection allInputs = (IHTMLElementCollection)allElements.tags("img");
                IHTMLElementCollection allStyles = (IHTMLElementCollection)htmlDocument.all.tags("style");


                foreach (IHTMLElement element in allStyles)
                {
                    //MasterPageStoredItin Mastr = (MasterPageStoredItin)Page.Master;
                    string killLink = element.innerHTML.Replace("a:link,", "");
                    killLink = killLink.Replace("span.MsoHyperlink", "Killed");
                    //Mastr.SetStyle = killLink.Replace("a:visited,", "");  //Need To Work
                    break;
                }
                //::......... Change some properties
                foreach (IHTMLElement element in allInputs)
                {

                    string src = (string)element.getAttribute("src", 0);
                    int Pos = -1;
                    if ((Pos = src.IndexOf("_files/")) != -1)
                    {
                        string URL = GetSecurePDF();
                        string u = Request.Url.Authority;

                        if (u.ToLower() == "mytravelplans.com.sg")
                        {
                            URL += u;
                        }
                        else if (u.ToLower() == "mytravelplans.com.au")
                        {
                            URL += u;
                        }
                        else if (u.ToLower() == "mytravelplans.co.in")
                        {
                            URL += u;
                        }
                        else if (u.ToLower() == "mytravelplans.eu")
                        {
                            URL += u;
                        }
                        else
                        {
                            URL += u;
                        }

                        string ImgName = GetImageName(m_FormName);

                        if (ImgName.Length > 0)
                        {
                            src = src.ToUpper().Replace("IMAGE001.JPG", ImgName);
                        }

                        URL += GetWebDirectory() + src.Substring(Pos + 1);
                        element.setAttribute("src", URL, 0);
                    }
                }

                //::......... Return the parent element content ( BODY > HTML )
                string MyoutString = htmlDocument.body.innerHTML;
                MyoutString = MyoutString.Replace("Download to calendar", "");
                MyoutString = MyoutString.Replace("https", "http");

                if (MyoutString.IndexOf("image002.jpg") != -1)
                {

                    MyoutString = MyoutString.Replace("image001.jpg", "ErnstYoung.JPG");

                    MyoutString = MyoutString.Replace("image002.jpg", "image001.jpg");

                    //MyoutString = "<img id='Image-Maps_1201201201026058' src='http://www.mytptest.com/files/ernstyoung.jpg' usemap='#Image-Maps_1201201201026058' border='0' /><map id='_Image-Maps_1201201201026058' name='Image-Maps_1201201201026058'><area shape='rect' coords='386,292,767,342' href='https://help.telstra.com/app/answers/detail/a_id/17272/c/1986%2c1511%2c1640%2c2423/r_id/130958/sno/0' alt='' title=''    /><area shape='rect' coords='347,734,728,784' href='mailto:support@techhead.com.au' alt='' title=''    /></map>" + MyoutString;


                    //  htmlToParse = "test";
                }

                return MyoutString;
            }
            catch (Exception e)
            {
                logger.Error("Exception in HTML fix: " + e.Message);
            }
            return "";
        }

        #endregion


        #region "Reason Code Mngt"

        /// <summary>
        /// GetEmergencyAssistance
        /// </summary>
        /// <returns></returns>
        public string GetEmergencyAssistanceText()
        {
            string strEmergencyAssistance = "";
            strEmergencyAssistance = "<table class=\"SegmentTable\" ID=\"Table2\" border=\"0\" xmlns=\"http://www.w3.org/1999/xhtml\"><tr><td class=\"Remark\"><p><span class=\"RemarkHeader\">Emergency Assistance</span><br />Travellers who need after hours emergency assistance can contact American Express Travel at<strong>1-800-294-3113 (dialed from Singapore only) or at 65-6-294-3113 when dialling fromoutside of Singapore.</strong> Additional service fees may apply. <br /></p></td></tr></table>";
            return strEmergencyAssistance;
        }

        /// <summary>
        /// GetCustomerFeedback
        /// </summary>
        /// <returns></returns>
        public string GetCustomerFeedbackText()
        {
            string strCustomerFeedback = "";
            strCustomerFeedback = "<table class=\"SegmentTable\" ID=\"Table2\" border=\"0\" xmlns=\"http://www.w3.org/1999/xhtml\"><tr><td class=\"Remark\"><p><span class=\"RemarkHeader\">Customer Feedback</span><br />To submit feedback to our Business Travel Operations and Quality teams 24 hours a day/7days a week. visit AIMS Online at: http://www.americanexpress.com.au/aimsonline <br /></p></td></tr></table>";
            return strCustomerFeedback;
        }

        /// <summary>
        /// GetInternationalTravelInformationText
        /// </summary>
        /// <returns></returns>
        public string GetInternationalTravelInformationText()
        {
            string strInternationalTravelInformation = "";
            strInternationalTravelInformation = "<table class=\"SegmentTable\" ID=\"Table1\" border=\"0\" xmlns=\"http://www.w3.org/1999/xhtml\"><tr><td class=\"Remark\"><p><span class=\"RemarkHeader\">International Travel Information</span>&nbsp;&nbsp;<br />Please take the time to read prior to your flight, the following containshelpful information<br /><span class=\"RemarkSubHeader\">Check In:</span>&nbsp;&nbsp; We recommend travellers to check in at least 2.5 hours prior to departure. We suggest onward reservations and flight times are reconfirmed 72 hrs prior to departure.Due to heightened security, the airlines check in time and reconfirmation policies may differ. Please check with your airline for the latest information.<br /><span class=\"RemarkSubHeader\">Electronic Tickets:</span>&nbsp;&nbsp; When travelling on an electronic ticket it is a requirement that you carry your passport for international journey at all times. It is essential that the name on your ticket matches exactly the name as shown on your passport.<br /><span class=\"RemarkSubHeader\">Seating and Meals:</span>&nbsp;&nbsp; Pre-assigned seating and special meal requests as shown on your itinerary are subject to confirmation upon check in and the airlines reserve the right to change this without notice at any time.<br /><span class=\"RemarkSubHeader\">Baggage and Quarantine:</span>&nbsp;&nbsp; There exists an international safety measure in place to protect travellers. Please refer to respective airlines/countries policy for further information on restricted baggage and quarantine items or baggage allowance.<br /><span class=\"RemarkSubHeader\">Pricing and Taxes:</span>&nbsp;&nbsp; All pricing and costs quoted are subject to change without notice due to airline/operator increases and/or currency fluctuations. Prices can only be guaranteed when paid in full and tickets and documents have been issued. Many countries have a departure tax which is additional to the price of your ticket. Please ensure you have sufficient local currency to pay for this at the airport.<br /><span class=\"RemarkSubHeader\">Travel Insurance:</span>&nbsp;&nbsp; It is strongly recommended to take out a Travel Insurance policy for all destinations. Please contact your American Express Travel Consultant on the range of policies that we have negotiated with specialist insurance underwriters especially for you.<br /><span class=\"RemarkSubHeader\">No Show / Cancellation Penalties:</span>&nbsp;&nbsp; You may incur an airline no-show fee if you fail to notify the airline or American Express travel that you are unable to travel on the flights you have booked. Additionally failure to cancel the flight reservation may deem your ticket unusable and non refundable. Certain categories of fares have penalties applicable if cancellation within stipulated time. If not cancelled within these time-frames, the ticket is deemed No Show. No Show / Cancellation Penalties apply to hotel and car rental bookings as well. Generally,hotel / car reservations need to be cancelled at least 48 hours prior to check in.Please check with hotels / car rental for more information.<br /><span class=\"RemarkSubHeader\">Itinerary:</span>&nbsp;&nbsp; To view your itinerary on the internet, log on to www.viewtrip.com - to access your booking, you will need your Booking Reference which is detailed on your American Express itinerary. <br /><span class=\"RemarkSubHeader\">Frequent Flyer:</span>&nbsp;&nbsp; Please retain your boarding passes to assist in the reconciliation of your frequent flyer statements. At time of check in, the airline can reconfirm that your frequent flyer number is entered.<br /><span class=\"RemarkSubHeader\">Passport:</span>&nbsp;&nbsp; If you are travelling internationally, please ensure you are holding a passport that has at least 6 months validity beyond the last day of the ensuing trip. Please advise American Express Travel if you are not travelling on a Singapore passport as you may require a re-entry permit.<br /><span class=\"RemarkSubHeader\">Visa:</span>&nbsp;&nbsp; Please ensure you have any applicable visas for the countries you are visiting or transiting. It is your responsibility to obtain correct visa documentation and American Express Travel is happy to provide information and assistance. Helpful visa information for Singaporean citizens is available online at www.mfa.gov.sg.<br /><span class=\"RemarkSubHeader\">Vaccinations:</span>&nbsp;&nbsp; Some countries have health and vaccination requirements. Travellers who do not have appropriate documentation for the required vaccinations would be subjected to medical check-up, isolation, or quarantine or a combination of all these for up to 6 days or more. We recommend you contact your doctor or a local vaccination clinic who are experts in this area. Alternatively, you can go on-line to the WHO web-site for further information: http://www.who.int/en/ <br /><span class=\"RemarkHeader\">Important Information</span>&nbsp;&nbsp;<br /> Please take the time to read, the following contains important information <br /><span class=\"SubRemarkHeader\">All Non-US Passport Holders Travelling to US:</span>&nbsp;&nbsp; As from 04 October 2005, the US Customs and Border Protection Agency will require additional details from all Non-US Passport Holders travelling to the United States. Travellers must provide passport and details of where they are staying in the US prior to departure at  airport check in. Please ensure you allow additional time at check in to avoid anticipated delays. All travellers to the US: Please note that your airlines may be required by laws in the USA and other countries to give border control agencies access to passenger data. Accordingly any information they hold about you and your travel arrangement may be disclosed to the customs and immigration authorities of any country in your itinerary. <br /> US Border Requirement: Due to US borders changes each foreign visitor between 14 and 79 years of age will be required to have their fingerprints and photographs taken digitally upon arrival at a US port by the immigration authorities. For Further Information Please Refer To The United States Consular Website: http://www.dhs.gov/dhspublic/ <br /><span class=\"RemarkSubHeader\">Travel Intermediary Disclosure: </span>&nbsp;&nbsp; American Express (AMEX) helps manage your company’s travel expenses and assists you in finding travel suppliers and making arrangements that meet your individual needs. We consider various factors in identifying travel suppliers and recommending itineraries. In this role, we are acting as an independent third party and not as a fiduciary. We want you to be aware that certain suppliers pay us commissions as well as incentives for reaching sales targets or other goals, and from time to time may also provide incentives to our travel counsellors. Certain suppliers may also provide compensation to us for various marketing and administrative services that we perform for them, such as granting them access to our marketing channels, participating in marketing programs and supporting technology initiatives. In addition, we receive compensation from suppliers when customers use the American Express® Card or other American Express products to pay for supplier products and services. From time to time we may enter into other business relationships with suppliers and these arrangements, including levels and types of compensation and incentives we receive, are subject to change. In identifying suppliers and recommending itineraries, we may consider a number of factors; including supplier availability, your preferences, and any agreements we have to book travel in accordance with your company’s travel policy. The relationships we have with suppliers may also influence the suppliers we identify and the itineraries we recommend. <br /><span class=\"RemarkSubHeader\">Liability Statement:</span>&nbsp;&nbsp; American Express (AMEX) acts only as agent for the airlines hotels and other contractors providing services (suppliers). By using the services on this itinerary the client agrees that neither AMEX nor its related companies employees or representatives shall be liable for any loss costs expense injury accident or damage to person or property resulting directly or indirectly from (a) the acts or omissions of such suppliers (b) acts of god or (c) any other cause beyond AMEX control.<br /></p></td></tr></table>";
            return strInternationalTravelInformation;
        }

        /// <summary>
        /// ReplaceLinks
        /// </summary>
        /// <returns></returns>
        public string ReplaceLinks(string strInternationalTravelInformation)
        {
            //  System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (strInternationalTravelInformation.IndexOf("www.viewtrip.com") > 0)
            {
                //sb.Append(strInternationalTravelInformation.Replace("www.viewtrip.com", "<a href=\"http://www.viewtrip.com\" title=\"www.viewtrip.com\" target=\"_blank\" alt=\"www.viewtrip.com\">www.viewtrip.com</a>"));
                //text-decoration:underline;
                strInternationalTravelInformation = strInternationalTravelInformation.Replace("www.viewtrip.com", "<a href=\"http://www.viewtrip.com\" style=\"color:#0066CC;\" target=\"_blank\" alt=\"www.viewtrip.com\">www.viewtrip.com</a>");
            }
            if (strInternationalTravelInformation.IndexOf("www.mfa.gov.sg") > 0)
            {
                strInternationalTravelInformation = strInternationalTravelInformation.Replace("www.mfa.gov.sg", "<a href=\"http://www.mfa.gov.sg\" style=\"color:#0066CC;\" target=\"_blank\" alt=\"www.mfa.gov.sg\">www.mfa.gov.sg</a>");
            }
            string TempStr = "http://" + "www.who.int/en/";
            if (strInternationalTravelInformation.IndexOf(TempStr) > 0)
            {
                strInternationalTravelInformation = strInternationalTravelInformation.Replace(TempStr, "<a href=\"http://www.who.int/en/\" style=\"color:#0066CC;\" target=\"_blank\" alt=\"www.who.int/en/\">www.who.int/en/</a>");
            }
            TempStr = "http://" + "www.dhs.gov/dhspublic";
            if (strInternationalTravelInformation.IndexOf("www.dhs.gov/dhspublic") > 0)
            {
                strInternationalTravelInformation = strInternationalTravelInformation.Replace(TempStr, "<a href=\"http://www.dhs.gov/dhspublic\" style=\"color:#0066CC;\" target=\"_blank\" alt=\"www.dhs.gov/dhspublic\">www.dhs.gov/dhspublic</a>");
            }

            TempStr = "http://" + "www.americanexpress.com.au/aimsonline";
            if (strInternationalTravelInformation.IndexOf("www.americanexpress.com.au/aimsonline") > 0)
            {
                strInternationalTravelInformation = strInternationalTravelInformation.Replace(TempStr, "<a href=\"http://www.americanexpress.com.au/aimsonline\" style=\"color:#0066CC;\" target=\"_blank\" alt=\"www.americanexpress.com.au/aimsonline\">www.americanexpress.com.au/aimsonline</a>");
            }
            return strInternationalTravelInformation;
        }

        /// <summary>
        /// GetComplianceDataAttached
        /// </summary>
        /// <param name="HTMLString"></param>
        /// <returns></returns>
        public string GetComplianceDataAttached(string HTMLString)
        {
            if (!string.IsNullOrEmpty(HTMLString))
            {
                int indexEmergencyAssistance = HTMLString.IndexOf("<span class=\"RemarkHeader\">Emergency Assistance</span>");
                if (indexEmergencyAssistance == -1)
                {
                    int i = HTMLString.Length;
                    HTMLString = HTMLString.Substring(0, i) + GetEmergencyAssistanceText();
                }
                //----------------------------------------------------------
                int indexCustomerFeedback = HTMLString.IndexOf("<span class=\"RemarkHeader\">Customer Feedback</span>");
                if (indexCustomerFeedback == -1)
                {
                    int i = HTMLString.Length;
                    HTMLString = HTMLString.Substring(0, i) + GetCustomerFeedbackText();
                }
                //----------------------------------------------------------
                int indexInternationalTravelInformation = HTMLString.IndexOf("<span class=\"RemarkHeader\">International Travel Information</span>");
                if (indexInternationalTravelInformation == -1)
                {
                    int i = HTMLString.Length;
                    HTMLString = HTMLString.Substring(0, i) + GetInternationalTravelInformationText();
                }
            }
            return HTMLString;

        }

        /// <summary>
        /// GetUpdatedHTMLString
        /// </summary>
        /// <returns></returns>
        public string GetUpdatedHTMLString(string strResponse, string strOrigionalHTML)
        {
            string strReasonString = "";
            //string strGTMTString = "";
            //string strProjectAndClientCodeString = "";
            string strResultString = "";
            string strAddReasonCodeDetail = "";
            string AddReasonTag = "";
            string AddGTMTTag = "";
            string AddProjectAndClientCode = "";
            string strUpdatedHTML = "";

            try
            {
                strResultString = GetReasonCodeData(strResponse);
                string[] getValues = strResultString.Split('|');
                if (getValues.Length > 0)
                {
                    if (!string.IsNullOrEmpty(getValues[0]))
                    {
                        strReasonString = GetReasonCodeText("CS" + getValues[0]);
                        if (!string.IsNullOrEmpty(strReasonString))
                        {
                            //<tr><td class=\"headerCol1\" colspan=\"2\"><p><span class=\"AmexSegment\"  style=\"font-size: 10pt;Font-weight:normal\">Priority Remark Test</span></p></td></tr>
                            AddReasonTag = "<tr><td colspan=\"2\"><span class=\"Remarks\" style=\"color: #0066CC\"> " + strReasonString + "</span></td></tr><tr><td colspan=\"2\"><span class=\"Remarks\">&nbsp;</span></td></tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(getValues[1]))
                    {
                        AddGTMTTag = "<tr><td class=\"headerCol1\" colspan=\"2\"><p style=\"margin:0px\"><span class=\"AmexSegment\" style=\"font-size: 9pt;Font-weight:normal\">GTMT : " + getValues[1] + "</span></p></td></tr>";
                    }
                    if (!string.IsNullOrEmpty(getValues[2]))
                    {
                        AddProjectAndClientCode = "<tr><td class=\"headerCol1\" colspan=\"2\"><p style=\"margin:0px\"><span class=\"AmexSegment\" style=\"font-size: 9pt;Font-weight:normal\">PROJECT CODE/CLIENT CODE : " + getValues[2] + "</span></p></td></tr><tr><td colspan=\"2\"><span class=\"Remarks\">&nbsp;</span></td></tr>";
                    }
                }

                strAddReasonCodeDetail = AddReasonTag + AddGTMTTag + AddProjectAndClientCode;
                if (!string.IsNullOrEmpty(strOrigionalHTML))
                {
                    int index = strOrigionalHTML.IndexOf("<span id=\"DateLine\">");
                    if (index > 0)
                    {
                        strUpdatedHTML = strOrigionalHTML.Substring(0, index) + strAddReasonCodeDetail + strOrigionalHTML.Substring(index);
                    }
                }
            }
            finally
            { }
            return strUpdatedHTML;
        }

        /// <summary>
        /// GetReasonCodeData
        /// </summary>
        /// <param name="ResponseString"></param>
        /// <returns></returns>
        public string GetReasonCodeData(string ResponseXMLString)
        {
            string strResultString = "";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ResponseXMLString);
            string query = "Travel/GeneralInformation/InvoiceRemarks";

            XmlNodeList nodeList;
            if (doc.DocumentElement.Attributes["xmlns"] != null)
            {
                string xmlns = doc.DocumentElement.Attributes["xmlns"].Value;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("MsBuild", xmlns);
                nodeList = doc.SelectNodes("/MsBuild:Travel/MsBuild:GeneralInformation/MsBuild:InvoiceRemarks", nsmgr);
                strResultString = GetResultString(nodeList);
            }
            else
            {
                nodeList = doc.SelectNodes(query);
                strResultString = GetResultString(nodeList);
            }

            return strResultString;
        }

        /// <summary>
        /// GetResultString
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public string GetResultString(XmlNodeList nodeList)
        {
            string strReturnValue = "";
            string ReasonCode = "";
            string GTMT = "";
            string ClientCode = "";
            bool GotReasonCodeAlready = false;

            for (int i = 0; i < nodeList.Count; i++)
            {
                for (int j = 0; j < nodeList[i].ChildNodes.Count; j++)
                {
                    Match matchUDID = Regex.Match(nodeList[i].ChildNodes[j].InnerText, @"^U2-(?<header>.*)/(?<data>.*)", RegexOptions.IgnoreCase);
                    if (matchUDID.Success)
                    {
                        string[] getUDID;
                        string[] getGTMT;
                        string strMatchedUDID = matchUDID.Value;

                        if (strMatchedUDID.Length > 0)
                        {
                            getUDID = strMatchedUDID.Split('-');
                            if (getUDID.Length > 0)
                            {
                                getGTMT = getUDID[1].Split('/');
                                if (getGTMT.Length > 0 && getGTMT.Length >= 1)
                                {
                                    GTMT = getGTMT[0];
                                    ClientCode = getGTMT[1];
                                }
                                else
                                {
                                    GTMT = getGTMT[0];
                                }
                            }
                        }
                    }

                    Match matchReasonCode = Regex.Match(nodeList[i].ChildNodes[j].InnerText, @"^L-[0-9]{1,4}\@*[0-9]*-(?<data>[0-9A-Z]{1,2})", RegexOptions.IgnoreCase);
                    if (matchReasonCode.Success)
                    {
                        if (GotReasonCodeAlready == false)
                        {
                            string strMatchedValue = matchReasonCode.Value;
                            if (strMatchedValue.Length > 0)
                            {
                                string[] getReasonCode = strMatchedValue.Split('-');
                                if (getReasonCode.Length > 0)
                                {
                                    ReasonCode = getReasonCode[2];
                                    GotReasonCodeAlready = true;
                                }
                            }
                        }
                    }
                }
            }

            strReturnValue = ReasonCode + "|" + GTMT + "|" + ClientCode;
            return strReturnValue;
        }

        /// <summary>
        /// GetReasonCodeText
        /// </summary>
        /// <param name="reasonCode"></param>
        public string GetReasonCodeText(string reasonCode)
        {
            string ReasonCodeText = "";
            Dictionary<string, string> reasonCodes = new Dictionary<string, string>();
            reasonCodes.Add("CSG", "The reason code you have selected is within policy no exception code required.");
            reasonCodes.Add("CSGA", "The reason code you have selected is within policy no exception code required.");
            reasonCodes.Add("CSB", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code <b>*Travelling with client*</b> <br/> All out of policy bookings will be reported.");
            reasonCodes.Add("CSBA", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code <b>*Travelling with client*</b> <br/> All out of policy bookings will be reported.");
            reasonCodes.Add("CSJ", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking. <br/>You have selected reason code <b>*In policy carrier or class of service is not available*</b> <br/> All out of policy bookings will be reported.");
            reasonCodes.Add("CSJA", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking. <br/>You have selected reason code <b>*In policy carrier or class of service is not available*</b> <br/> All out of policy bookings will be reported.");
            reasonCodes.Add("CSF", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/> You have selected reason code <b>*Booked higher class due to traveller exception or paying for an upgrade or Business/first class less expensive than economy*</b> <br/> All out of policy bookings will be reported.");
            reasonCodes.Add("CSFA", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/> You have selected reason code <b>*Booked higher class due to traveller exception or paying for an upgrade or Business/first class less expensive than economy*</b> <br/> All out of policy bookings will be reported.");
            reasonCodes.Add("CSL", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code <b>*Fare chosen is lower than CS corporate deal fare*</b><br/>All out of policy bookings will be reported.");
            reasonCodes.Add("CSLA", "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code <b>*Fare chosen is lower than CS corporate deal fare*</b><br/>All out of policy bookings will be reported.");
            reasonCodes.Add("CSP", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to fare restrictions*</b><br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSPA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to fare restrictions*</b><br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSR", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to airport preference*</b> <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSRA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to airport preference*</b> <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSC", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to carrier preference*</b> <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSCA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to carrier preference*</b> <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            //reasonCodes.Add("CSCA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to carrier preference* <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            //reasonCodes.Add("CSCA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to carrier preference* <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSQ", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to traveller class of service preference*</b><br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSQA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to traveller class of service preference*</b><br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CST", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to time preference*</b><br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSTA", "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code <b>*Refused fare due to time preference*</b><br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.");
            reasonCodes.Add("CSK", "Your reservation has been made within 7 days of your travel date.<br/>If your trip is non-client travel you require out of policy approval for submission with your expenses.<br/> Any Shared Services booking also requires out of policy approval for client travel.");
            reasonCodes.Add("CSKA", "Your reservation has been made within 7 days of your travel date.<br/>If your trip is non-client travel you require out of policy approval for submission with your expenses.<br/> Any Shared Services booking also requires out of policy approval for client travel.");
            reasonCodes.Add("CSD", "You are booked with this airline in business class as the fare is cheaper than economy class.<br/>If you need to amend your reservation to travel in economy class please contact your local travel team.");
            reasonCodes.Add("CSDA", "You are booked with this airline in business class as the fare is cheaper than economy class.<br/>If you need to amend your reservation to travel in economy class please contact your local travel team.");

            foreach (string str in reasonCodes.Keys)
            {
                if (str == reasonCode)
                {
                    ReasonCodeText = reasonCodes[str];
                    break;
                }

            }
            return ReasonCodeText;

            //switch (reasonCode)
            //{
            //    case "CSG":
            //        ReasonCodeText = "The reason code you have selected is within policy no exception code required.";
            //        break;

            //    case "CSGA":
            //        ReasonCodeText = "The reason code you have selected is within policy no exception code required.";
            //        break;

            //    case "CSB":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code *Travelling with client* <br/> All out of policy bookings will be reported.";
            //        break;

            //    case "CSBA":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.You have selected reason code *Travelling with client* All out of policy bookings will be reported.";
            //        break;

            //    case "CSJ":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking. <br/>You have selected reason code *In policy carrier or class of service is not available*  <br/> All out of policy bookings will be reported.";
            //        break;

            //    case "CSJA":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking. <br/> You have selected reason code *In policy carrier or class of service is not available*  <br/> All out of policy bookings will be reported.";
            //        break;

            //    case "CSF":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/> You have selected reason code *Booked higher class due to traveller exception or paying for an upgrade or Business/first class less expensive than economy* <br/> All out of policy bookings will be reported.";
            //        break;

            //    case "CSFA":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.You have selected reason code *Booked higher class due to traveller exception or paying for an upgrade or Business/first class less expensive than economy* All out of policy bookings will be reported.";
            //        break;

            //    case "CSL":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code *Fare chosen is lower than CS corporate deal fare*<br/>All out of policy bookings will be reported.";
            //        break;

            //    case "CSLA":
            //        ReasonCodeText = "You selected a valid exception code for making an out of policy booking therefore no out of policy approval is required for this booking.<br/>You have selected reason code *Fare chosen is lower than CS corporate deal fare*<br/>All out of policy bookings will be reported.";
            //        break;

            //    case "CSP":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to fare restrictions*<br/>You will require out of policy approval for submission with your expenses..<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria..<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSPA":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to fare restrictions*<br/>You will require out of policy approval for submission with your expenses..<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria..<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSR":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to airport preference* <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSRA":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to airport preference* <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSC":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to carrier preference* <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSCA":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to carrier preference* <br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSQ":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to traveller class of service preference*<br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSQA":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to traveller class of service preference*<br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CST":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to time preference*<br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSTA":
            //        ReasonCodeText = "Your booking is out of policy as you have not taken the lowest flexible fare.<br/>You have selected reason code *Refused fare due to time preference*<br/>You will require out of policy approval for submission with your expenses.<br/>The alternative fare shown below is the lowest fare you could have taken given your search criteria.<br/>If you require further fare comparisons to support your expense claim please contact your local travel team.";
            //        break;

            //    case "CSK":
            //        ReasonCodeText = "Your reservation has been made within 7 days of your travel date.<br/>If your trip is non-client travel you require out of policy approval for submission with your expenses.<br/> Any Shared Services booking also requires out of policy approval for client travel.";
            //        break;

            //    case "CSKA":
            //        ReasonCodeText = "Your reservation has been made within 7 days of your travel date. <br/>If your trip is non-client travel you require out of policy approval for submission with your expenses.<br/> Any Shared Services booking also requires out of policy approval for client travel.";
            //        break;

            //    case "CSD":
            //        ReasonCodeText = "You are booked with this airline in business class as the fare is cheaper than economy class.<br/>If you need to amend your reservation to travel in economy class please contact your local travel team.";
            //        break;

            //    case "CSDA":
            //        ReasonCodeText = "You are booked with this airline in business class as the fare is cheaper than economy class.<br/>If you need to amend your reservation to travel in economy class please contact your local travel team.";
            //        break;
            //}
        }

        #endregion

        #endregion
    }
}
