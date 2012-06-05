using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTP.BAL;
using MTP.DTO;
using System.Xml;
using System.Data;
using System.IO;
using System.Collections;
using System.Text;

namespace MTP_JAPA.Controllers
{
    //Flight Information
    //-----------------------------------
    //Date                Mon 21May 
    //Airline             Klm Royal Dutch Airli
    //Flight/Class        KL835 - I Business
    //Origin              Amsterdam  Schiphol Airport
    //Destination         Singapore Changi
    //Departing           2100
    //Arriving            1520 22 May 
    //Arrival Terminal    Terminal 1
    //Estimated Time/Stops12.20 hours - Non Stop
    //Confirmed

    public class QiLiveCalendarController : BaseController
    {
        string strSegNumberCol = "SegNumber";
        string strSubjectCol = "Subject";
        string strDateCol = "Date";
        string strAirlineCol = "Airline";
        string strFlightClassCol = "FlightClass";
        string strOriginCol = "Origin";
        string strDestinationCol = "Destination";
        string strDepartingCol = "Departing";
        string strArrivingCol = "Arriving";
        string strArrivalTerminalCol = "ArrivalTerminal";
        string strEstimatedTimeAndStopsCol = "EstimatedTimeAndStops";
        string strConfirmedCol = "Confirmed";
        string strStartDepartDateCol = "StartDepartDate";
        string strEndArrivalDateCol = "EndArrivalDate";

        #region Load

        /// <summary>
        /// QiLiveCalendarView
        /// </summary>
        /// <returns></returns>
        public ActionResult QiLiveCalendarView()
        {
            ViewBag.theGoClick = 4;
            Session["theGoClick"] = 4;
            ViewBag.TabDisplay = 1;
            GetQueryVars();
            LoadSessionVars();
            logger.Debug("QI Live Calendar view");
            DataTable dtCalendarDisplay = GetCalendarList();
            if (dtCalendarDisplay == null || dtCalendarDisplay.Rows.Count <= 0)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 52 });
            }
            Session["dtCalendarDisplay"] = dtCalendarDisplay;
            return View(dtCalendarDisplay);
        }

        /// <summary>
        /// QiLiveCalendarView
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadCalendar()
        {
            ViewBag.theGoClick = 4;
            Session["theGoClick"] = 4;
            ViewBag.TabDisplay = 1;
            string m_Segment = "";

            logger.Debug("download calendar");
            if (Request.QueryString["Segment"] != null)
            {
                m_Segment = Request.QueryString["Segment"].ToString();
            }

            if (Request.QueryString["R"] != null)
            {
                m_RLOC = Request.QueryString["R"].ToString();
            }

            if (Request.QueryString["S"] != null)
            {
                m_Surname = Request.QueryString["S"].ToString();
            }

            if (string.IsNullOrEmpty(m_RLOC))
            {
                // Missing reference number
                return RedirectToAction("Error", "ErrorDisplay", new { E = 20 });
            }
            if (string.IsNullOrEmpty(m_Surname))
            {
                // Missing last name
                //Response.Redirect("Error.aspx?E=20");
                return RedirectToAction("Error", "ErrorDisplay", new { E = 20 });
            }
            //if (string.IsNullOrEmpty(m_Segment))
            //{
            //    // Missing segment number
            //    //Response.Redirect("Error.aspx?E=20");
            //    return RedirectToAction("Error", "ErrorDisplay", new { E = 20 });
            //}
            DataTable dtCalendarDisplay = null;
            if (Session["dtCalendarDisplay"] != null)
            {
                dtCalendarDisplay = Session["dtCalendarDisplay"] as DataTable;
            }
            else
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 52 });
            }
            logger.Debug("render calendar");
            return RenderIcsFile(m_Segment, dtCalendarDisplay);
        }


        #endregion

        #region Helper Methods

        /// <summary>
        /// GetCalendarList
        /// </summary>
        /// <returns></returns>
        public DataTable GetCalendarList()
        {
            DataTable dtCalendarDetail = CreateCalendarDetailDataTable();

            string utid_id = "";
            string strXmlString = "";
            ItineraryDataBAL objItineraryDataBAL = new ItineraryDataBAL();
            logger.Debug("Fetch data from databse");
            List<XmlBookingModel> lstQiLiveItineraryData = objItineraryDataBAL.SearchItineraryDataBySurnameAndRLOC(m_Surname, m_RLOC);
            if (lstQiLiveItineraryData != null && lstQiLiveItineraryData.Count > 0)
            {
                logger.Debug("fetched rows from database");
                foreach (XmlBookingModel xmlBooking in lstQiLiveItineraryData)
                {
                    //id,utid_id,phase,entry_date,stamp,xml
                    utid_id = Convert.ToString(xmlBooking.utid_id);
                    strXmlString = Convert.ToString(xmlBooking.xml);
                    dtCalendarDetail = GetCalendarData(strXmlString, m_RLOC);
                }
            }
            else
            {
                return dtCalendarDetail;
                //return RedirectToAction("Error", "ErrorDisplay", new { E = 52 });                
            }
            if (dtCalendarDetail != null)
            {
                dtCalendarDetail.Columns.Add(new DataColumn("url", typeof(string)));
                foreach (DataRow _Row in dtCalendarDetail.Rows)
                {
                    _Row["url"] = GetDownloadLink(_Row);
                }
            }
            else
            {
                return dtCalendarDetail;//return to error page
            }
            return dtCalendarDetail;
        }

        /// <summary>
        /// Responsible for returning url text
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetDownloadLink(DataRow row)
        {
            System.Text.StringBuilder ReturnValue = new System.Text.StringBuilder();
            //ReturnValue.Append("@Url.Action(\"DownloadCalendar,QiLiveCalendar,new { R = '" + m_RLOC + "',S='" + m_Surname + "',Segment='" + row["SegNumber"].ToString() + "' })");

            string IndividualCalendarRootPath = System.Web.Configuration.WebConfigurationManager.AppSettings["IndividualCalendarRootPath"];
            ReturnValue.Append(IndividualCalendarRootPath + "QiLiveCalendar/DownloadCalendar?R=");
            ReturnValue.Append(m_RLOC);
            ReturnValue.Append("&S=");
            ReturnValue.Append(m_Surname);
            ReturnValue.Append("&Segment=");
            ReturnValue.Append(row["SegNumber"].ToString());

            return ReturnValue.ToString();
        }

        /// <summary>
        /// CreateDataTable
        /// </summary>
        /// <returns></returns>
        public DataTable CreateCalendarDetailDataTable()
        {
            DataTable dtBind = new DataTable();
            dtBind.Columns.Add(strSegNumberCol, typeof(string));
            dtBind.Columns.Add(strSubjectCol, typeof(string));
            dtBind.Columns.Add(strDateCol, typeof(string));
            dtBind.Columns.Add(strAirlineCol, typeof(string));
            dtBind.Columns.Add(strFlightClassCol, typeof(string));
            dtBind.Columns.Add(strOriginCol, typeof(string));
            dtBind.Columns.Add(strDestinationCol, typeof(string));
            dtBind.Columns.Add(strDepartingCol, typeof(string));
            dtBind.Columns.Add(strArrivingCol, typeof(string));
            dtBind.Columns.Add(strArrivalTerminalCol, typeof(string));
            dtBind.Columns.Add(strEstimatedTimeAndStopsCol, typeof(string));
            dtBind.Columns.Add(strConfirmedCol, typeof(string));
            dtBind.Columns.Add(strStartDepartDateCol, typeof(string));
            dtBind.Columns.Add(strEndArrivalDateCol, typeof(string));
            return dtBind;
        }

        /// <summary>
        /// GetCalendarData
        /// </summary>
        /// <param name="ResponseString"></param>
        /// <returns></returns>
        public DataTable GetCalendarData(string inputXMLString, string m_RLOC)
        {
            List<Dictionary<string, string>> calDatalist = null;
            DataTable dtCalendarDetail = null;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(inputXMLString);
                string query = "tbstruct/tbdoc/TravelDetails/Date";

                XmlNodeList nodeList;
                if (doc.DocumentElement.Attributes["xmlns"] != null)
                {
                    string xmlns = doc.DocumentElement.Attributes["xmlns"].Value;
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("MsBuild", xmlns);
                    nodeList = doc.SelectNodes("/MsBuild:Travel/MsBuild:GeneralInformation/MsBuild:Date", nsmgr);
                    calDatalist = ExtractCalendarData(nodeList);
                }
                else
                {
                    nodeList = doc.SelectNodes(query);
                    calDatalist = ExtractCalendarData(nodeList);
                }
                if (calDatalist != null)
                {
                    dtCalendarDetail = FillDatatable(calDatalist, m_RLOC);
                }
                else
                {
                    return dtCalendarDetail;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Get data : error occured :" + ex.ToString());
                dtCalendarDetail = null;
            }
            finally
            {

            }
            return dtCalendarDetail;
        }

        /// <summary>
        /// FillDatatable
        /// </summary>
        /// <param name="strCalendarData"></param>
        /// <returns></returns>
        public DataTable FillDatatable(List<Dictionary<string, string>> lstCalendarData, string m_RLOC)
        {
            string RLOC = m_RLOC;
            string strFlight = "";
            string start = "";
            string destination = "";
            string strDepartDate = "";
            string strArrivalDate = "";
            string strDepartTime = "";
            string strArrivalTime = "";
            string ArrivalTimeOnly = "";
            string strArrivalDatePart = "";
            DataTable dtCalendarDetail = CreateCalendarDetailDataTable();
            logger.Debug("Fill table");
            try
            {
                if (lstCalendarData != null)
                {
                    logger.Debug("create rows");
                    foreach (Dictionary<string, string> item in lstCalendarData)
                    {
                        DataRow newRow = dtCalendarDetail.NewRow();
                        foreach (KeyValuePair<string, string> pair in item)
                        {
                            //SegmentNumber="1" Airline="British Airways" FlightNo="BA16" Date="Saturday, 10 November 2012" DepartureCity="Sydney" 
                            //DepartureTime="4:25 PM" DepartureDate="10/11/2012" ArrivalCity="London Heathrow" 
                            //ArrivalDate="11/11/2012" ArrivalTime="5:25 AM / 11Nov" Status="PN Pending confirmation" 
                            //Class="World Traveller" ClassCode="Y" Operator="British Airways" SupplierStops="One stop" 
                            //EstimatedTime="14Hr 5Min" SupplierService="Meal" 
                            //DepartureTerminal="Terminal 1" ArrivalTerminal="Terminal 3" AirCraft="Boeing 747-400"

                            string pairKeyName = "";
                            string[] pairKey = pair.Key.Split('|');
                            if (pairKey != null)
                            {
                                pairKeyName = pairKey[0].ToLower();
                            }
                            if (pairKeyName != "")
                            {
                                if (pairKeyName == "segmentnumber")
                                {
                                    newRow[strSegNumberCol] = pair.Value.Trim();
                                }

                                if (pairKeyName == "date")
                                {
                                    newRow[strDateCol] = pair.Value.Trim();
                                }
                                if (pairKeyName == "airline")
                                {
                                    newRow[strAirlineCol] = pair.Value.Trim();
                                }
                                if (pairKeyName == "flightno") //strFlightClassCol.ToLower()
                                {
                                    newRow[strFlightClassCol] = pair.Value.Trim();
                                    strFlight = "Flight " + pair.Value.Trim();
                                }
                                if (pairKeyName == "class") //strFlightClassCol.ToLower()
                                {
                                    if (string.IsNullOrEmpty(Convert.ToString(newRow[strFlightClassCol])))
                                    {
                                        newRow[strFlightClassCol] = newRow[strFlightClassCol];
                                    }
                                    else
                                    {
                                        newRow[strFlightClassCol] = newRow[strFlightClassCol] + " " + pair.Value.Trim();
                                    }
                                }
                                if (pairKeyName == "departurecity") //strOriginCol.ToLower()
                                {
                                    newRow[strOriginCol] = pair.Value.Trim();
                                    start = pair.Value;
                                }
                                if (pairKeyName == "arrivalcity")
                                {
                                    newRow[strDestinationCol] = pair.Value.Trim();
                                    destination = pair.Value.Trim();
                                }
                                if (pairKeyName == "departuredate")
                                {
                                    //DateTime newDepartureDateTime=null;
                                    string[] strDepartureDate = pair.Value.Trim().Split('/');
                                    if (strDepartureDate != null)
                                    {
                                        int day = Convert.ToInt32(strDepartureDate[0]);
                                        int month = Convert.ToInt32(strDepartureDate[1]);
                                        int year = Convert.ToInt32(strDepartureDate[2]);
                                        DateTime newDepartureDateTime = new DateTime(year, month, day);
                                        string strTemp = newDepartureDateTime.ToString("dd/MM/yyyy");
                                        strDepartDate = strTemp.Trim();
                                    }
                                    //DateTime dtTemp = DateTime.ParseExact(pair.Value.Trim(), "dd/MM/yyyy", null);

                                }
                                if (pairKeyName == "departuretime")
                                {
                                    strDepartTime = pair.Value.Replace("/", " ");
                                    //if (pair.Value.IndexOf("/") > 0)
                                    //{
                                    //    string[] DepartTime = pair.Value.Split('/');
                                    //    strDepartTime = DepartTime[0];
                                    //}                               
                                }
                                if (pairKeyName == "arrivaldate")
                                {
                                    string[] strArrivalDt = pair.Value.Trim().Split('/');
                                    if (strArrivalDt != null)
                                    {
                                        int day = Convert.ToInt32(strArrivalDt[0]);
                                        int month = Convert.ToInt32(strArrivalDt[1]);
                                        int year = Convert.ToInt32(strArrivalDt[2]);
                                        DateTime newArrivalDate = new DateTime(year, month, day);
                                        string strTemp = newArrivalDate.ToString("dd/MM/yyyy");
                                        strArrivalDate = strTemp.Trim();
                                    }
                                    //DateTime dtTemp = DateTime.ParseExact(pair.Value.Trim(), "dd/MM/yyyy", null);
                                    ////DateTime dtTemp = Convert.ToDateTime(pair.Value);
                                    //string strTemp = dtTemp.ToString("dd/MM/yyyy");
                                    //strArrivalDate = strTemp.Trim();
                                }
                                if (pairKeyName == "arrivaltime")
                                {
                                    if (pair.Value.IndexOf("/") > 0)
                                    {
                                        string[] ArrivalTimeWithDate = pair.Value.Trim().Split('/');
                                        if (ArrivalTimeWithDate != null && ArrivalTimeWithDate.Length > 0)
                                        {
                                            ArrivalTimeOnly = ArrivalTimeWithDate[0].Trim();
                                            strArrivalDatePart = ArrivalTimeWithDate[1].Trim();
                                        }
                                    }
                                    else
                                    {
                                        ArrivalTimeOnly = pair.Value.Trim();
                                        strArrivalDatePart = "";
                                    }
                                    strArrivalTime = pair.Value.Trim().Replace("/", " ");
                                }
                                if (pairKeyName == "arrivalterminal")
                                {
                                    newRow[strArrivalTerminalCol] = pair.Value.Trim();
                                }
                                if (pairKeyName == "estimatedtime")
                                {
                                    newRow[strEstimatedTimeAndStopsCol] = pair.Value.Trim();
                                }
                                if (pairKeyName == "status")
                                {
                                    newRow[strConfirmedCol] = pair.Value.Trim();
                                }
                            }
                        }

                        logger.Debug("depart and arrival date");
                        //DateTime.ParseExact(strDepartDate + " " + strDepartTime, "dd/MM/yyyy h:mm tt", null)
                        DateTime dtDepartDate = DateTime.ParseExact(strDepartDate + " " + strDepartTime, "dd/MM/yyyy h:mm tt", null);
                        //DateTime dtDepartDate = Convert.ToDateTime(strDepartDate + " " + strDepartTime);
                        //DateTime dtArrivalDate = Convert.ToDateTime(strArrivalDate + " " + ArrivalTimeOnly);
                        DateTime dtArrivalDate = DateTime.ParseExact(strArrivalDate + " " + ArrivalTimeOnly, "dd/MM/yyyy h:mm tt", null);
                        logger.Debug("depart and arrival date done");

                        string strSubjectDepart = "";
                        string strSubjectArrial = "";
                        if (dtDepartDate != null)
                        {
                            newRow[strDepartingCol] = dtDepartDate.ToString("H:mm").Replace(":", "");//strDepartTime;                    
                            newRow[strStartDepartDateCol] = Convert.ToString(dtDepartDate);
                            strSubjectDepart = dtDepartDate.ToString("ddd d MMM") + " " + dtDepartDate.ToString("H:mm").Replace(":", "");
                        }
                        else
                        {
                            newRow[strDepartingCol] = "";
                            newRow[strStartDepartDateCol] = DateTime.Now;
                            strSubjectDepart = "";
                        }

                        if (dtArrivalDate != null)
                        {
                            newRow[strArrivingCol] = dtArrivalDate.ToString("H:mm").Replace(":", "") + " " + strArrivalDatePart;
                            newRow[strEndArrivalDateCol] = dtArrivalDate;
                            strSubjectArrial = dtArrivalDate.ToString("H:mm").Replace(":", "") + " " + strArrivalDatePart;
                        }
                        else
                        {
                            newRow[strArrivingCol] = "";
                            newRow[strEndArrivalDateCol] = DateTime.Now;
                            strSubjectArrial = "";
                        }

                        string strSubject = RLOC + "-" + strFlight + " " + start + " " + "to" + " " + destination + " " + strSubjectDepart + "/" + strSubjectArrial;
                        newRow[strSubjectCol] = strSubject;
                        //newRow[strConfirmedCol] = "Confirmed";
                        dtCalendarDetail.Rows.Add(newRow);
                    }
                    logger.Debug("rows added to table");
                }
                else
                {
                    return dtCalendarDetail;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Fill table : error occured :" + ex.ToString());
                return dtCalendarDetail;
            }
            finally
            {

            }
            return dtCalendarDetail;
        }

        /// <summary>
        /// GetResultString
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> ExtractCalendarData(XmlNodeList nodeList)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> calDataFields = null;
            logger.Debug("extract data from xml");
            try
            {
                for (int i = 0; i < nodeList.Count; i++) //Date
                {
                    //XmlAttribute dateAttribute = nodeList[i].Attributes["Date"];
                    for (int j = 0; j < nodeList[i].ChildNodes.Count; j++) //1
                    {
                        if (nodeList[i].ChildNodes[j].Name.ToLower() == "flightinformation")
                        {
                            if (nodeList[i].ChildNodes[j].Attributes.Count > 0)
                            {
                                calDataFields = new Dictionary<string, string>();
                                XmlAttributeCollection collection = nodeList[i].ChildNodes[j].Attributes;
                                if (collection != null)
                                {
                                    foreach (XmlAttribute item in collection)
                                    {
                                        calDataFields.Add(item.Name + "|" + nodeList[i].ChildNodes[j].Attributes[0].Value, item.Value);
                                    }
                                }

                            }
                            if (calDataFields != null && calDataFields.Count > 0)
                            {
                                list.Add(calDataFields);
                            }
                            logger.Debug("extracted data from xml");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Debug("Extract data : error occured :" + ex.ToString());
                return list;
            }
            return list;
        }

        /// <summary>
        /// GetQueryVars
        /// </summary>
        protected virtual void GetQueryVars()
        {
            if (Request.QueryString["R"] != null)
            {
                m_RLOC = Request.QueryString["R"].ToString().ToUpper();
                ViewBag.RLOC = m_RLOC;
            }
            if (Request.QueryString["S"] != null)
            {
                m_Surname = Request.QueryString["S"].ToString().ToUpper();
                ViewBag.Surname = m_Surname;
            }
            if (Request.QueryString["G"] != null)
            {
                m_GDS = Request.QueryString["G"].ToString().ToUpper();
            }
            if (Request.QueryString["E"] != null)
            {
                try
                {
                    int ErrorNumber = int.Parse(Request.QueryString["E"].ToString());
                    //ProcessError(ErrorNumber);
                }
                catch
                {
                    m_LastError = "Could not process Error.<br>Press the back button ans retry";
                }
            }


        }

        /// <summary>
        /// LoadSessionVars
        /// </summary>
        protected void LoadSessionVars()
        {
            if (Session["RLOC"] != null)
            {
                m_RLOC = Session["RLOC"].ToString().ToUpper();
                m_Reference = m_RLOC;
            }
            if (Session["Surname"] != null)
            {
                m_Surname = Session["Surname"].ToString().ToUpper();
            }
            if (Session["GDS"] != null)
            {
                m_GDS = Session["GDS"].ToString().ToUpper();
            }
            if (Session["Location"] != null)
            {
                m_Location = Session["Location"].ToString();
            }
            if (Session["Name"] != null)
            {
                m_Name = Session["Name"].ToString().ToUpper();
            }
        }

        /// <summary>
        /// RenderIcsFile
        /// </summary>
        /// <param name="m_DataTable"></param>
        public ActionResult RenderIcsFile(string m_Segment, DataTable m_DataTable)
        {
            try
            {
                MemoryStream _Stream = new MemoryStream();
                StreamWriter _Writer = new StreamWriter(_Stream);
                DataRow _Row = null;
                string dtCreated = "";
                string dtStart = "";
                string dtEnd = "";
                string UID = "";

                if (m_DataTable != null && m_DataTable.Rows.Count > 0)
                {
                    _Writer.AutoFlush = true;
                    _Writer.WriteLine("BEGIN:VCALENDAR");
                    _Writer.WriteLine("VERSION:2.0");
                    _Writer.WriteLine("PRODID:-//TravelBytes/MyTp D2Cv7//EN");
                    _Writer.WriteLine("METHOD:PUBLISH");

                    if (m_Segment != "")
                    {
                        logger.Debug("Segment calendar");
                        DataRow[] result = m_DataTable.Select("SegNumber = " + m_Segment);
                        _Row = result[0];
                        dtCreated = DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");

                        if (Convert.ToString(_Row[strStartDepartDateCol]) != null && Convert.ToString(_Row[strStartDepartDateCol]) != "")
                        {
                            dtStart = Convert.ToDateTime(_Row[strStartDepartDateCol]).ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                        }
                        else
                        {
                            dtStart = DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                        }

                        if (Convert.ToString(_Row[strEndArrivalDateCol]) != null && Convert.ToString(_Row[strEndArrivalDateCol]) != "")
                        {
                            dtEnd = Convert.ToDateTime(_Row[strEndArrivalDateCol]).ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                        }
                        else
                        {
                            dtEnd = DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                        }

                        UID = CreateUniqueID();

                        _Writer.WriteLine("BEGIN:VEVENT");
                        _Writer.WriteLine("DCREATED:" + dtCreated);
                        _Writer.WriteLine("DTSTART:" + dtStart);
                        _Writer.WriteLine("DTEND:" + dtEnd);
                        _Writer.WriteLine("UID:" + UID);
                        _Writer.WriteLine("SUMMARY:" + _Row["Subject"].ToString());

                        string strDesc = GetCalendarFormatData(_Row);
                        if (string.IsNullOrEmpty(strDesc) == false)
                        {
                            _Writer.WriteLine(strDesc);
                        }

                        string strLocation = GetLocationData(_Row);
                        if (strLocation != null && strLocation != "")
                        {
                            _Writer.WriteLine("LOCATION:" + strLocation);
                        }

                        //_Writer.WriteLine(RFC2445TextField(AltDescription));
                        _Writer.WriteLine("CLASS:PUBLIC");
                        _Writer.WriteLine("END:VEVENT");
                        _Writer.WriteLine("END:VCALENDAR");
                        logger.Debug("segment calendar done");
                    }
                    else
                    {
                        logger.Debug("complete calendar");
                        foreach (DataRow row in m_DataTable.Rows)
                        {
                            _Row = row;
                            dtCreated = DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");

                            if (Convert.ToString(_Row[strStartDepartDateCol]) != null && Convert.ToString(_Row[strStartDepartDateCol]) != "")
                            {
                                dtStart = Convert.ToDateTime(_Row[strStartDepartDateCol]).ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                            }
                            else
                            {
                                dtStart = DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                            }

                            if (Convert.ToString(_Row[strEndArrivalDateCol]) != null && Convert.ToString(_Row[strEndArrivalDateCol]) != "")
                            {
                                dtEnd = Convert.ToDateTime(_Row[strEndArrivalDateCol]).ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                            }
                            else
                            {
                                dtEnd = DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                            }

                            UID = CreateUniqueID();
                            _Writer.WriteLine("BEGIN:VEVENT");
                            _Writer.WriteLine("DCREATED:" + dtCreated);
                            _Writer.WriteLine("DTSTART:" + dtStart);
                            _Writer.WriteLine("DTEND:" + dtEnd);
                            _Writer.WriteLine("UID:" + UID);
                            _Writer.WriteLine("SUMMARY:" + _Row["Subject"].ToString());
                            //string strDescription = "DESCRIPTION:Flight Information\\n-----------------------------------\\nDate                Mon 21May \\nAirline             Klm Royal Dutch Airli\\nFlight/Class        KL1348 - J Business\\nOrigin              Billund  Billund\\nDestination         Amsterdam Schiphol Airport\\nDeparting           1825\\nArriving            1935 \\nEstimated Time/Stops1.10 hours - Non Stop\\nConfirmed\\n\\n ";//_Row["Description"].ToString().Replace("=\r\n", "").Replace("\\n", "");

                            string strDesc = GetCalendarFormatData(_Row);
                            if (string.IsNullOrEmpty(strDesc) == false)
                            {
                                _Writer.WriteLine(strDesc);
                            }

                            //Working//string strDescription = "DESCRIPTION:Flight Information\\n-----------------------------------\\nDate          " + _Row[strDateCol] + "\\nAirline          " + _Row[strAirlineCol] + "\\nFlight/Class          " + _Row[strFlightClassCol] + "\\nOrigin          " + _Row[strOriginCol] + "\\nDestination          " + _Row[strDestinationCol] + " \\nDeparting          " + _Row[strDepartingCol] + "\\nArriving        " + _Row[strArrivingCol] + "\\nEstimated Time/Stops          " + _Row[strEstimatedTimeAndStopsCol] + "\\n" + _Row[strConfirmedCol] + "\\n\\n ";
                            //string strDescription = "DESCRIPTION:Flight Information\\n\\r-----------------------------------\\n\\rDate         =Mon 21May \\n\\rAirline             Klm Royal Dutch Airli\\n\\rFlight/C=lass        KL1348 - J Business\\n\\rOrigin              Billund  Billund\\n\\=rDestination         Amsterdam Schiphol Airport\\n\\rDeparting           182=5\\n\\rArriving            1935 \\n\\rEstimated Time/Stops1.10 hours - Non Sto=p\\n\\rConfirmed\\n\\r\\n\\r";                                                            
                            string strLocation = GetLocationData(_Row);
                            if (strLocation != null && strLocation != "")
                            {
                                _Writer.WriteLine("LOCATION:" + strLocation);
                            }
                            //_Writer.WriteLine(RFC2445TextField(AltDescription));
                            _Writer.WriteLine("CLASS:PUBLIC");
                            _Writer.WriteLine("END:VEVENT");

                        }
                    }

                    _Writer.WriteLine("END:VCALENDAR");
                    Response.Clear();
                    Response.ContentType = "text/v-calendar";
                    if (m_Segment != "")
                    {
                        Response.AddHeader("content-disposition", "attachment; filename=" + MakeCalendarFilename(_Row, m_Segment) + ".ics");
                    }
                    else
                    {
                        string TmpFileTitle = "download_";
                        TmpFileTitle += m_RLOC + "_";
                        TmpFileTitle += m_Surname;
                        Response.AddHeader("content-disposition", "attachment; filename=" + TmpFileTitle + ".ics");

                    }
                    Response.AppendHeader("Content-Length", _Stream.Length.ToString());
                    logger.Debug("calendar done");
                    //Response.ContentType = "application/download";
                    //Response.BinaryWrite(_Stream.ToArray());
                    return File(_Stream.ToArray(), "application/octet-stream");
                    //return File(_Stream.ToArray(), "application/download");

                    //Response.End();            
                }
                return View();
            }
            catch (Exception ex)
            {
                logger.Debug("render calendar : error occured :" + ex.ToString());
                return RedirectToAction("Message", "MessageDisplay", new { E = 52 });
            }
            finally
            {
            }
            //return View();
        }

        /// <summary>
        /// GetCalendarFormatData
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public string GetCalendarFormatData(DataRow row)
        {
            if (row == null)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("DESCRIPTION:Flight Information\\n-----------------------------------");
            if (string.IsNullOrEmpty(Convert.ToString(row[strDateCol])) == false)
            {
                sb.Append("\\nDate          ");
                sb.Append(Convert.ToString(row[strDateCol]));
            }

            if (string.IsNullOrEmpty(Convert.ToString(row[strAirlineCol])) == false)
            {
                sb.Append("\\nAirline          ");
                sb.Append(Convert.ToString(row[strAirlineCol]));
            }

            if (string.IsNullOrEmpty(Convert.ToString(row[strFlightClassCol])) == false)
            {
                sb.Append("\\nFlight/Class          ");
                sb.Append(Convert.ToString(row[strFlightClassCol]));
            }

            if (string.IsNullOrEmpty(Convert.ToString(row[strOriginCol])) == false)
            {
                sb.Append("\\nOrigin          ");
                sb.Append(Convert.ToString(row[strOriginCol]));
            }
            if (string.IsNullOrEmpty(Convert.ToString(row[strDestinationCol])) == false)
            {
                sb.Append("\\nDestination          ");
                sb.Append(Convert.ToString(row[strDestinationCol]));
            }
            if (string.IsNullOrEmpty(Convert.ToString(row[strDepartingCol])) == false)
            {
                sb.Append("\\nDeparting          ");
                sb.Append(Convert.ToString(row[strDepartingCol]));
            }

            if (string.IsNullOrEmpty(Convert.ToString(row[strArrivingCol])) == false)
            {
                sb.Append("\\nArriving        ");
                sb.Append(Convert.ToString(row[strArrivingCol]));
            }

            if (string.IsNullOrEmpty(Convert.ToString(row[strEstimatedTimeAndStopsCol])) == false)
            {
                sb.Append("\\nEstimated Time/Stops          ");
                sb.Append(Convert.ToString(row[strEstimatedTimeAndStopsCol]));
            }

            if (string.IsNullOrEmpty(Convert.ToString(row[strConfirmedCol])) == false)
            {
                sb.Append("\\n");
                sb.Append(Convert.ToString(row[strConfirmedCol]));
            }
            sb.Append("\\n\\n");
            return sb.ToString();

        }

        /// <summary>
        /// GetLocationData
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public string GetLocationData(DataRow row)
        {
            if (row == null)
            {
                return "";
            }
            StringBuilder sbLocation = new StringBuilder();
            if (string.IsNullOrEmpty(Convert.ToString(row[strAirlineCol])) == false)
            {
                sbLocation.Append(Convert.ToString(row[strAirlineCol]) + " - ");
            }
            if (string.IsNullOrEmpty(Convert.ToString(row[strOriginCol])) == false)
            {
                sbLocation.Append(Convert.ToString(row[strOriginCol]) + " / ");
            }
            if (string.IsNullOrEmpty(Convert.ToString(row[strDestinationCol])) == false)
            {
                sbLocation.Append(Convert.ToString(row[strDestinationCol]) + " ");
            }
            return sbLocation.ToString();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CreateUniqueID()
        {
            string guid = System.Guid.NewGuid().ToString();
            return guid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public string BreakandFormat(string Text)
        {
            string Out = "";
            string Buff = "";
            StringReader r = new StringReader(Text);
            int nChr;
            while ((nChr = r.Read()) != -1)
            {
                char ch = (char)nChr;
                //if (ch == '\t')
                //    ch = ' ';

                if (ch != '\t')
                    Buff += ch;

                if (Buff.Length >= 70)
                {
                    //Origional
                    Out += "\t" + Buff + "\r\n";
                    Buff = "";
                }
            }

            if (Buff.Length > 0)
                Out += "\t" + Buff + "\r\n";
            //Out += "\t" + Buff + "\n\n";            

            return Out;
        }

        /// <summary>
        /// RFC2445TextField
        /// </summary>
        /// <param name="LongText"></param>
        /// <returns></returns>
        private string RFC2445TextField(string LongText)
        {
            int i;

            //LongText = LongText.Replace("\\", "\\\\");
            //LongText = LongText.Replace("\n", "");
            //LongText = LongText.Replace("&nbsp;", " ");

            LongText = LongText.Replace(@"\", @"\\")
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace("\t", "")
            .Replace("<br/>", "<br/><br/>")
            .Replace(",", @"\,");

            //LongText = LongText.Replace("&nbsp;", " ");
            //LongText = LongText.Replace("\", "\\")
            //LongText = LongText.Replace(";", @"\;");

            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
            char[] charArray = LongText.ToCharArray();

            for (i = 1; i <= charArray.Length; i++)
            {
                sBuilder.Append(charArray[i - 1]);
                /*if (i % 71 == 0)
                    sBuilder.Append(Environment.NewLine + "\t");*/
                if (i % 74 == 0)
                    sBuilder.Append(Environment.NewLine + " ");
            }

            return sBuilder.ToString();

        }

        /// <summary>
        /// Responsible for making the download calendar filename 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string MakeCalendarFilename(DataRow row, string m_Segment)
        {
            string ReturnValue = row["Subject"].ToString();
            if (string.IsNullOrEmpty(ReturnValue))
            {
                ReturnValue = "itinerary_";
                ReturnValue += m_RLOC + "_";
                ReturnValue += m_Surname + "_";
                ReturnValue += m_Segment;
            }
            else
            {
                ReturnValue = RemoveRestrictedChars(ReturnValue);
            }
            ReturnValue = ReturnValue.Trim();
            ReturnValue = ReturnValue.Replace(" ", "_");

            return ReturnValue;
        }

        /// <summary>
        /// Responsible for removing restricted characters from file title
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string RemoveRestrictedChars(string value)
        {
            System.Text.StringBuilder _ReturnValue = new System.Text.StringBuilder(value);
            int _CurrIndex = 0;
            int _CopyIndex = -1;
            int _Count = value.Length;
            char _CurrChar;
            int _NumberOfExtras = 0;

            Hashtable _RestrictedChars = new Hashtable();
            _RestrictedChars.Add('\\', '\\');
            _RestrictedChars.Add('/', '/');
            _RestrictedChars.Add(':', ':');
            _RestrictedChars.Add('*', '*');
            _RestrictedChars.Add('?', '?');
            _RestrictedChars.Add('\'', '\'');
            _RestrictedChars.Add('<', '<');
            _RestrictedChars.Add('>', '>');
            _RestrictedChars.Add('|', '|');

            while ((_CurrIndex < _Count))
            {
                _CurrChar = _ReturnValue[_CurrIndex];

                if (_RestrictedChars.ContainsKey(_CurrChar))
                {
                    // There is at least one instance of a double
                    // comma, from here on, start copying
                    // using the copy index
                    if ((_CopyIndex == -1))
                    {
                        // Toggle copy index once
                        _CopyIndex = _CurrIndex;
                    }
                    _NumberOfExtras = _NumberOfExtras + 1;
                }
                else
                {
                    if ((_CopyIndex != -1))
                    {
                        // Copy char to location pointed by
                        // copy index
                        _ReturnValue[_CopyIndex] = _CurrChar;
                        // Increment copy index
                        _CopyIndex = _CopyIndex + 1;
                    }
                }

                // Increment current index and loop to
                // evaluate next char
                _CurrIndex = _CurrIndex + 1;
            }
            return _ReturnValue.ToString().Substring(0, _Count - _NumberOfExtras);
        }

        #endregion
    }
}

//string html = "<html><title></title><head></head><body>This is test</body><html>";
//string AltDescription = "X-ALT-DESC;FMTTYPE=text/html:" + html.Replace("\n", "");
//--------------------------------------------
//"FMTTYPE=text/html:<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3
//.2//EN"><html><title></title><head></head><body  style="font-family: '
//Courier New', Courier, monospace"><span  font="font-family: 'Courier N
//ew', Courier, monospace">Flight Information&nbsp;&nbsp;&nbsp;&nbsp;&nb
//sp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><br/><span  font="
//font-family: 'Courier New', Courier, monospace">----------------------
//-------------&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nb
//sp;&nbsp;&nbsp;</span><br/><span  font="font-family: 'Courier New', Co
//urier, monospace">Date&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
//&nbsp;&nbsp;&nbsp;&nbsp;Mon 21May</span><br/><span  font="font-family:
// 'Courier New', Courier, monospace">Airline&nbsp;&nbsp;&nbsp;&nbsp;&nb
//sp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Klm Royal Dutch Airli</sp
//an><br/><span  font="font-family: 'Courier New', Courier, monospace">F
//light/Class&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
//;&nbsp;&nbsp;KL1348 - J Business</span><br/><span  font="font-family: 
//'Courier New', Courier, monospace">Origin&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
//;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Billund  Billund</span><br/
//><span  font="font-family: 'Courier New', Courier, monospace">Destinat
//ion&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&
//nbsp;Amsterdam Schiphol Airport</span><br/><span  font="font-family: '
//Courier New', Courier, monospace">Departing&nbsp;&nbsp;&nbsp;&nbsp;&nb
//sp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1825</span><br/><span  fo
//nt="font-family: 'Courier New', Courier, monospace">Arriving&nbsp;&nbs
//p;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1935</sp
//an><br/><span  font="font-family: 'Courier New', Courier, monospace">E
//stimated Time/Stops1.10 hours - Non Stop&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><br/><span  font="fon
//t-family: 'Courier New', Courier, monospace">Confirmed&nbsp;&nbsp;&nbs
//p;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><br/><s
//pan  font="font-family: 'Courier New', Courier, monospace">&nbsp;&nbsp
//;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><b
//r/></body></html>
