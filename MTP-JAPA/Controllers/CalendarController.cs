using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using MTP_JAPA.Helpers;
using MTP.BAL;
using MTP.DTO;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Text;
using JQueryDataTables.Models;

namespace MTP_JAPA.Controllers
{
    public class CalendarController : BaseController
    {

        #region Variables

        string SetCurrentSite = "EU";
        private System.Data.DataTable m_DataTable = null;
        private string m_Segment = "";
        const string FileExtension = ".doc";
        const string FileExtensionXML = ".xml";
        protected System.Web.UI.HtmlControls.HtmlGenericControl Pax;
        protected System.Web.UI.HtmlControls.HtmlGenericControl RLOC;
        protected System.Web.UI.HtmlControls.HtmlGenericControl Agent;
        protected System.Web.UI.HtmlControls.HtmlForm Form1;
        protected System.Web.UI.HtmlControls.HtmlGenericControl ClientAddress;
        protected System.Web.UI.HtmlControls.HtmlGenericControl AgencyAddress;
        protected System.Web.UI.HtmlControls.HtmlGenericControl results;
        protected string m_FileName = "";
        protected string m_FormName = "";
        private System.Data.DataTable m_DataTable_DownloadToCalendar = null;
        private string m_Filename = "";

        #endregion

        #region Load

        /// <summary>
        /// Download
        /// </summary>
        /// <returns></returns>
        public ActionResult Download()
        {
            ViewBag.theGoClick = 4;
            Session["theGoClick"] = 4;
            ViewBag.TabDisplay = 1;
            m_DataTable = new DataTable();
            if (null == m_DataTable)
            {
                // Missing reference number
                return RedirectToAction("Message", "MessageDisplay", new { E = 3 });
                //Response.Redirect("Error.aspx?E=3");
            }

            /////////////////////////
            // Get search params

            LoadSessionVars();
            GetQueryVars();


            /////////////////////////
            // Check search params

            if (string.IsNullOrEmpty(m_RLOC))
            {
                // Missing reference number
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
                //Response.Redirect("Error.aspx?E=20");
            }
            if (string.IsNullOrEmpty(m_Surname))
            {
                // Missing last name
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
                //Response.Redirect("Error.aspx?E=20");
            }

            /////////////////////////
            // Load for requested data

            int rValue = LoadData();
            if (rValue == 0)
            {
                return RedirectToAction("QiLiveCalendarView", "QiLiveCalendar", new { R = m_RLOC, S = m_Surname });
                //return RedirectToAction("Message", "MessageDisplay", new { E = 30 });
            }

            /////////////////////////
            // Display results

            m_DataTable.Columns.Add(new DataColumn("url", typeof(string)));
            foreach (DataRow _Row in m_DataTable.Rows)
            {
                _Row["url"] = GetDownloadLink(_Row);
            }

            if (m_DataTable.Rows.Count == 0)
            {
                //ItinContent.Visible = false;
                //nod2c.Visible = true;
            }

            return View(m_DataTable);
        }

        /// <summary>
        /// DownloadSegmentToCalendar
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadSegmentToCalendar()
        {
            m_DataTable = new DataTable();
            if (null == m_DataTable)
            {
                // Missing reference number
                return RedirectToAction("Message", "MessageDisplay", new { E = 3 });
                //Response.Redirect("Error.aspx?E=3");
            }

            /////////////////////////
            // Get search params

            LoadSessionVars();
            GetQueryVars();
            if (Request.QueryString["Segment"] != null)
            {
                m_Segment = Request.QueryString["Segment"].ToString();
            }

            /////////////////////////
            // Check search params

            if (string.IsNullOrEmpty(m_RLOC))
            {
                // Missing reference number
                //Response.Redirect("Error.aspx?E=20");
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
            }
            if (string.IsNullOrEmpty(m_Surname))
            {
                // Missing last name
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
                //Response.Redirect("Error.aspx?E=20");
            }

            if (string.IsNullOrEmpty(m_Segment))
            {
                // Missing segment number
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
                //Response.Redirect("Error.aspx?E=20");
            }

            /////////////////////////
            // Check for requested data

            if (CheckForData() == 0)
            {
                // Record not found
                return RedirectToAction("Message", "MessageDisplay", new { E = 30 });
                //Response.Redirect("Error.aspx?E=22");
            }

            /////////////////////////
            // Stream file to output

            RenderIcsFile();

            return View();
        }


        /// <summary>
        /// DownloadToCalendar
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadToCalendar()
        {
            bPrint = false;
            if (Request.QueryString["L"] != null)
            {
                bPrint = true;
            }
            ViewBag.bPrint = bPrint;

            m_DataTable_DownloadToCalendar = new DataTable();
            if (null == m_DataTable_DownloadToCalendar)
            {
                // Missing reference number
                //Response.Redirect("Error.aspx?E=3");
                return RedirectToAction("Message", "MessageDisplay", new { E = 3 });
            }

            /////////////////////////
            // Get search params

            LoadSessionVars();
            GetQueryVars();


            /////////////////////////
            // Check search params

            if (string.IsNullOrEmpty(m_RLOC))
            {
                // Missing reference number
                //Response.Redirect("Error.aspx?E=20");
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
            }
            if (string.IsNullOrEmpty(m_Surname))
            {
                // Missing last name
                //Response.Redirect("Error.aspx?E=20");
                return RedirectToAction("Message", "MessageDisplay", new { E = 20 });
            }

            /////////////////////////
            // Check for requested data

            if (CheckForData_DownloadToCalendar() == 0)
            {
                // Record not found
                //Response.Redirect("Error.aspx?E=22");
                return RedirectToAction("Message", "MessageDisplay", new { E = 31 });
            }
            LogToQiOnline();

            if (CheckForFile() == 0)
            {
                // File not found
                //Response.Redirect("Error.aspx?E=23");
                return RedirectToAction("Message", "MessageDisplay", new { E = 31 });
            }

            /////////////////////////
            // Stream file to output
            //using (new Impersonator("woz", "", "2rachelle"))
            //{
                System.IO.FileInfo file = new System.IO.FileInfo(m_Filename);
                if (!file.Exists)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 32 });
                    //throw new FileNotFoundException("Can’t find that file.");
                }
                else
                {
                    string TmpFileTitle = "download_";
                    TmpFileTitle += m_RLOC + "_";
                    TmpFileTitle += m_Surname + "_";
                    TmpFileTitle += System.IO.Path.GetFileName(m_Filename);

                    // clear the current output content from the buffer  
                    Response.Clear();

                    // add the header that specifies the default filename for the Download/SaveAs dialog  
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + TmpFileTitle);

                    // add the header that specifies the file size, so that the browser can show the download progress  
                    Response.AddHeader("Content-Length", file.Length.ToString());

                    // specify that the response is a stream that cannot be read by the client and must be downloaded  
                    Response.ContentType = "application/octet-stream";

                    // send the file stream to the client  
                    Response.WriteFile(file.FullName);
               // }
            }

            return View();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int LoadData()
        {
            return SearchDataBaseByRLOC(m_RLOC, m_Surname.ToUpper());
        }

        /// <summary>
        /// Responsible for checking the database for the specified data
        /// </summary>
        /// <returns>
        /// true if data was found, false otherwise
        /// </returns>
        int SearchDataBaseByRLOC(string referenceNumber, string surName)
        {
            int ReturnValue = 0;
            try
            {
                //DataSet DS = new DataSet();
                QiCalEventsBAL objQiCalEventsBAL = new QiCalEventsBAL();
                List<QiCalEventsModel> lstQiCalEventsModel = objQiCalEventsBAL.SearchDataBaseByRLOC(referenceNumber, surName);
                if (lstQiCalEventsModel != null)
                {
                    DataTable dt = Common.ListToDataTable(lstQiCalEventsModel);
                    m_DataTable = dt;
                    ReturnValue = 1;
                }
                else
                {
                    return ReturnValue;
                }
                if (m_DataTable.Rows.Count != 0)
                {
                    // Records found
                    logger.Debug("Download: qicalendar found : " + m_DataTable.Rows.Count.ToString());
                }
                else
                {
                    // Records not found
                    logger.Debug("Download: No records found in qicalevents table matching criteria");
                    ReturnValue = 0;
                    //logger.Debug(TmpSQL);
                }
            }
            catch (MySqlException oleExp)
            {
                // Log error
                logger.Error("Exception in SearchDataBaseByRLOC: " + oleExp.ToString());
            }
            catch (Exception objError)
            {
                //display error details
                logger.Error("Exception in SearchDataBaseByRLOC: " + objError.ToString());
            }
            finally
            {
            }
            return ReturnValue;
        }

        /// <summary>
        /// Responsible for returning url text
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetDownloadLink(DataRow row)
        {
            System.Text.StringBuilder ReturnValue = new System.Text.StringBuilder();

            ReturnValue.Append("DownloadSegmentToCalendar/Calendar?R=");
            ReturnValue.Append(m_RLOC);
            ReturnValue.Append("&S=");
            ReturnValue.Append(m_Surname);
            ReturnValue.Append("&Segment=");
            ReturnValue.Append(row["SegNumber"].ToString());

            return ReturnValue.ToString();
        }


        /// <summary>
        /// Responsible for checking the database for the specified data
        /// </summary>
        /// <returns>
        /// true if data was found, false otherwise
        /// </returns>
        int CheckForData()
        {
            return SearchDataBaseByRLOC(m_RLOC, m_Surname.ToUpper(), m_Segment);
        }

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <param name="segmentNumber"></param>
        /// <returns></returns>
        int SearchDataBaseByRLOC(string referenceNumber, string surName, string segmentNumber)
        {
            int ReturnValue = 0;
            try
            {
                int iSegmentNumber = 0;
                if (segmentNumber != "")
                {
                    iSegmentNumber = Convert.ToInt32(segmentNumber);
                }

                QiCalEventsBAL objQiCalEventsBAL = new QiCalEventsBAL();
                List<QiCalEventsModel> lstQiCalEventsModel = objQiCalEventsBAL.SearchDataBaseByRLOC(referenceNumber, surName, iSegmentNumber);
                if (lstQiCalEventsModel != null)
                {
                    DataTable dt = Common.ListToDataTable(lstQiCalEventsModel);
                    m_DataTable = dt;
                    ReturnValue = 1;
                }
                else
                {
                    return ReturnValue;
                }

                if (m_DataTable.Rows.Count != 0)
                {
                    // Records found
                    logger.Debug("Download: qicalendar found : " + m_DataTable.Rows.Count.ToString());
                }
                else
                {
                    // Records not found
                    logger.Debug("Download: No records found in qicalevents table matching criteria");
                    ReturnValue = 0;
                    // logger.Debug(TmpSQL);
                }
            }
            catch (MySqlException oleExp)
            {
                // Log error
                logger.Error("Exception in SearchDataBaseByRLOC: " + oleExp.ToString());
            }
            catch (Exception objError)
            {
                //display error details
                logger.Error("Exception in SearchDataBaseByRLOC: " + objError.ToString());
            }
            finally
            {
            }
            return ReturnValue;
        }


        private void RenderIcsFile()
        {
            MemoryStream _Stream = new MemoryStream();
            StreamWriter _Writer = new StreamWriter(_Stream);
            DataRow _Row = m_DataTable.Rows[0];

            _Writer.AutoFlush = true;
            _Writer.WriteLine("BEGIN:VCALENDAR");
            _Writer.WriteLine("VERSION:2.0");
            _Writer.WriteLine("PRODID:-//TravelBytes/QI D2Cv5.02//EN");
            _Writer.WriteLine("METHOD:PUBLISH");
            _Writer.WriteLine("BEGIN:VEVENT");
            _Writer.WriteLine("DCREATED:" + _Row["DCreated"].ToString());
            _Writer.WriteLine("DTSTART:" + _Row["DTSTART"].ToString());
            _Writer.WriteLine("DTEND:" + _Row["DTEND"].ToString());
            _Writer.WriteLine("UID:" + _Row["UID"].ToString());
            _Writer.WriteLine("SUMMARY:" + _Row["Subject"].ToString());

            string strDescription = _Row["Description"].ToString().Replace("=\r\n", "").Replace("\\n", "");
            //string strDecodedString = DecodeQuotedPrintables(strDescription, "utf-8");
            _Writer.WriteLine(strDescription);

            //_Writer.WriteLine(_Row["Description"].ToString() + "\n");        
            _Writer.WriteLine("LOCATION:" + _Row["Location"].ToString());

            //_Writer.WriteLine(_Row["AltDescription"].ToString());
            _Writer.WriteLine(RFC2445TextField(_Row["AltDescription"].ToString()));

            _Writer.WriteLine("CLASS:PUBLIC");
            //_Writer.WriteLine("END:VALARM" + "\n");
            _Writer.WriteLine("END:VEVENT");
            _Writer.WriteLine("END:VCALENDAR");

            Response.Clear();
            //Response.ContentType = "text/calendar";
            Response.ContentType = "text/v-calendar";
            Response.AddHeader("content-disposition", "attachment; filename=" + MakeCalendarFilename(_Row) + ".ics");
            Response.AppendHeader("Content-Length", _Stream.Length.ToString());
            Response.ContentType = "application/download";
            Response.BinaryWrite(_Stream.ToArray());
            Response.End();
        }

        ///// <summary>
        ///// RenderIcsFile
        ///// </summary>
        //private void RenderIcsFile()
        //{
        //    MemoryStream _Stream = new MemoryStream();
        //    StreamWriter _Writer = new StreamWriter(_Stream);
        //    DataRow _Row = m_DataTable.Rows[0];

        //    _Writer.AutoFlush = true;
        //    _Writer.WriteLine("BEGIN:VCALENDAR");
        //    _Writer.WriteLine("VERSION:2.0");
        //    _Writer.WriteLine("PRODID:-//TravelBytes/QI D2Cv5.02//EN");
        //    _Writer.WriteLine("METHOD:PUBLISH");
        //    _Writer.WriteLine("BEGIN:VEVENT");
        //    _Writer.WriteLine("DCREATED:" + _Row["DCreated"].ToString());
        //    _Writer.WriteLine("DTSTART:" + _Row["DTSTART"].ToString());
        //    _Writer.WriteLine("DTEND:" + _Row["DTEND"].ToString());
        //    _Writer.WriteLine("UID:" + _Row["UID"].ToString());
        //    _Writer.WriteLine("SUMMARY:" + _Row["Subject"].ToString());

        //    string strDescription = _Row["Description"].ToString();
        //    string strDecodedString = DecodeQuotedPrintables(strDescription, "utf-8");
        //    _Writer.WriteLine(strDecodedString);

        //    //_Writer.WriteLine(_Row["Description"].ToString() + "\n");        
        //    _Writer.WriteLine("LOCATION:" + _Row["Location"].ToString());

        //    //_Writer.WriteLine(_Row["AltDescription"].ToString());
        //    _Writer.WriteLine(RFC2445TextField(_Row["AltDescription"].ToString()));

        //    _Writer.WriteLine("CLASS:PUBLIC");
        //    //_Writer.WriteLine("END:VALARM" + "\n");
        //    _Writer.WriteLine("END:VEVENT");
        //    _Writer.WriteLine("END:VCALENDAR");

        //    Response.Clear();
        //    Response.ContentType = "text/v-calendar";
        //    Response.AddHeader("content-disposition", "attachment; filename=" + MakeCalendarFilename(_Row) + ".ics");
        //    Response.AppendHeader("Content-Length", _Stream.Length.ToString());
        //    Response.ContentType = "application/download";
        //    Response.BinaryWrite(_Stream.ToArray());
        //    Response.End();
        //}

        /// <summary>
        /// Responsible for making the download calendar filename 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string MakeCalendarFilename(DataRow row)
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

        /// <summary>
        /// This function is used to decode quoted printables chars in description field of ical file.
        /// As in lotus notes,If we not decode quoted printables chars,it gives parsing error.
        /// In order to solve parsing error ,we have to decode quoted printables chars.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charSet"></param>
        /// <returns></returns>
        private static string DecodeQuotedPrintables(string input, string charSet)
        {
            if (string.IsNullOrEmpty(charSet))
            {
                var charSetOccurences = new Regex(@"=\?.*\?Q\?", RegexOptions.IgnoreCase);
                var charSetMatches = charSetOccurences.Matches(input);
                foreach (Match match in charSetMatches)
                {
                    charSet = match.Groups[0].Value.Replace("=?", "").Replace("?Q?", "");
                    input = input.Replace(match.Groups[0].Value, "").Replace("?=", "");
                }
            }

            Encoding enc = new ASCIIEncoding();
            if (!string.IsNullOrEmpty(charSet))
            {
                try
                {
                    enc = Encoding.GetEncoding(charSet);
                }
                catch
                {
                    enc = new ASCIIEncoding();
                }
            }

            //decode iso-8859-[0-9]
            var occurences = new Regex(@"=[0-9A-Z]{2}", RegexOptions.Multiline);
            var matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                try
                {
                    byte[] b = new byte[] { byte.Parse(match.Groups[0].Value.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier) };
                    char[] hexChar = enc.GetChars(b);
                    input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
                }
                catch
                { ;}
            }

            //decode base64String (utf-8?B?)
            occurences = new Regex(@"\?utf-8\?B\?.*\?", RegexOptions.IgnoreCase);
            matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                byte[] b = Convert.FromBase64String(match.Groups[0].Value.Replace("?utf-8?B?", "").Replace("?UTF-8?B?", "").Replace("?", ""));
                string temp = Encoding.UTF8.GetString(b);
                input = input.Replace(match.Groups[0].Value, temp);
            }

            input = input.Replace("=\r\n", "");
            input = input.Replace("\r\r\n", "\\n\\n");
            input = input.Replace(";ENCODING=QUOTED-PRINTABLE", "");

            return input;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LongText"></param>
        /// <returns></returns>
        private string RFC2445TextField(string LongText)
        {
            int i;

            LongText = LongText.Replace(@"\", @"\\");
            LongText = LongText.Replace("\n", "");
            LongText = LongText.Replace("\r", "");
            LongText = LongText.Replace("\t", "");
            LongText = LongText.Replace("<br/>", "<br/><br/>");
            LongText = LongText.Replace(",", @"\,");

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

        #region Complete File Download

        /// <summary>
        /// Responsible for checking the database for the specified data
        /// </summary>
        /// <returns>
        /// true if data was found, false otherwise
        /// </returns>
        int CheckForData_DownloadToCalendar()
        {
            return SearchDataBaseByRLOC_DownloadToCalendar(m_RLOC, m_Surname.ToUpper());
        }

        /// <summary>
        /// SearchDataBaseByRLOC_DownloadToCalendar
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <returns></returns>
        int SearchDataBaseByRLOC_DownloadToCalendar(string referenceNumber, string surName)
        {
            int ReturnValue = 0;
            try
            {
                //TmpSQL = "SELECT `Language`, `FileLocation` FROM `qicalendar` WHERE `RLoc`=?ReferenceLocatore AND `PaxName`=?PaxName ORDER BY Stamp DESC; ";

                QiCalendarBAL objQiCalendarBAL = new QiCalendarBAL();
                List<QiCalendarModel> lstQiCalendarModel = objQiCalendarBAL.SearchDataBaseByRLOC_DownloadToCalendar(referenceNumber, surName);
                if (lstQiCalendarModel != null)
                {
                    DataTable dt = Common.ListToDataTable(lstQiCalendarModel);
                    m_DataTable_DownloadToCalendar = dt;
                    ReturnValue = 1;
                }
                else
                {
                    return ReturnValue;
                }

                if (m_DataTable_DownloadToCalendar.Rows.Count != 0)
                {
                    // Records found
                    logger.Debug("Download-to-calendar: qicalendar found : " + m_DataTable_DownloadToCalendar.Rows.Count.ToString());
                    ReturnValue = 1;
                }
                else
                {
                    // Records not found
                    logger.Debug("Download-to-calendar: No records found in qicalendar table matching criteria");
                    //logger.Debug(TmpSQL);
                }
            }
            catch (MySqlException oleExp)
            {
                // Log error
                logger.Error("Exception in SearchDataBaseByRLOC: " + oleExp.ToString());
            }
            catch (Exception objError)
            {
                //display error details
                logger.Error("Exception in SearchDataBaseByRLOC: " + objError.ToString());
            }
            finally
            {
            }
            return ReturnValue;
        }

        /// <summary>
        /// CheckForFile
        /// </summary>
        /// <returns></returns>
        int CheckForFile()
        {
            int ReturnValue = 0;

            foreach (System.Data.DataRow TmpRow in m_DataTable_DownloadToCalendar.Rows)
            {
                string TmpFilename;
                TmpFilename = TmpRow["FileLocation"].ToString();

                if (string.IsNullOrEmpty(TmpFilename))
                {
                    // Filename is blank, next row
                    continue;
                }
                //using (new Impersonator("woz", "", "2rachelle"))
                //{
                    if (!System.IO.File.Exists(TmpFilename))
                    {
                        // File does not exist, next row
                        continue;
                    }
                //}

                // Found file, exit loop
                m_Filename = TmpFilename;
                ReturnValue = 1;
                break;
            }
            return ReturnValue;
        }


        /// <summary>
        /// LogToQiOnline
        /// </summary>
        protected void LogToQiOnline()
        {
            try
            {
                string TmpRloc;
                string TmpIPAddress;
                string TmpProcessorName;
                string TmpFormName;
                string TmpSubject;
                string TmpTeam;
                string TmpCompany;
                TmpRloc = GetReference;
                TmpIPAddress = GetIPAddress();
                TmpProcessorName = "DOWNLOAD_TO_CALENDAR";
                TmpFormName = "";
                TmpSubject = "Download to calendar - " + m_Surname;
                TmpTeam = "";
                TmpCompany = "";

                /////////////////////////////
                // Insert record to mailrecord before redirecting to generated document
                logger.Debug("Insert record to mailrecord");

                MailRecordsModel objMailRecordsDTO = new MailRecordsModel();
                objMailRecordsDTO.RLOC = TmpRloc;
                objMailRecordsDTO.Created = System.DateTime.UtcNow;
                objMailRecordsDTO.FromName = TmpIPAddress;
                objMailRecordsDTO.ProcessorName = TmpProcessorName;
                objMailRecordsDTO.FormName = TmpFormName;
                objMailRecordsDTO.Subject = TmpSubject;
                objMailRecordsDTO.Team = TmpTeam;
                objMailRecordsDTO.Company = TmpCompany;

                MailRecordsBAL objMailRecordsBAL = new MailRecordsBAL(SetCurrentSite);
                objMailRecordsBAL.SaveMailRecord(objMailRecordsDTO);

                //ChwLib.DatabaseManager.AddMailRecord(
                //    // RLOC / PRN
                //    TmpRloc,
                //    // Created
                //    System.DateTime.UtcNow,
                //    // FromName / To-From
                //    TmpIPAddress,
                //    // ProcessorName
                //    TmpProcessorName,
                //    // FormName / Format-Name
                //    TmpFormName,
                //    // Subject
                //    TmpSubject,
                //    // Team
                //    TmpTeam,
                //    // Company
                //    TmpCompany);

            }
            catch
            {
            }
        }
        #endregion

        #endregion
    }
}
