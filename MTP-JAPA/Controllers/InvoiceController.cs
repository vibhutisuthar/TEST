using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Data;
using System.Xml;
using NLog;
using NLog.Config;
using System.IO;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MTP_JAPA.Helpers;
using MTP.BAL;
using MTP.DTO;

namespace MTP_JAPA.Controllers
{
    public class InvoiceController : BaseController
    {
        const string NotFoundError = "No Invoices can be located for this booking.<br>Please contact your Travel Team.";
        static AutoResetEvent Event1 = new AutoResetEvent(false);
        static AutoResetEvent Event2 = new AutoResetEvent(false);
        static AutoResetEvent Event3 = new AutoResetEvent(false);
        static AutoResetEvent Event4 = new AutoResetEvent(false);

        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.RegularExpressionValidator ReferenceValidator;
        protected System.Web.UI.WebControls.TextBox tbSurname;
        protected System.Web.UI.WebControls.TextBox tbReference;
        protected System.Web.UI.HtmlControls.HtmlInputButton Submit1;
        string m_URL = "";
        bool m_bFilesFound = false;
        string m_FileName = "";
        bool m_DirectLink = false;

        // Filed by the processing
        DataRow[] m_InvoiceArray = null;
        //DataRow[] m_ItineraryArray = null;
        protected DataSet m_InvoicesOnDisk;
        DataSet m_ItinerariesOnDisk = null;
        DataRow[] m_DbRecs = null;
        DiskRecord[] m_DiskRecords = null;
        string m_Summary = "";
        string m_User = "";
        protected System.Web.UI.HtmlControls.HtmlForm Form2;
        string m_PSW = "";
        string gRLOC = "";
        string gLocation = "";
        string gInfoName = "";

        #region Load

        /// <summary>
        /// InvoiceView
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceView()
        {
            ViewBag.TabDisplay = 1;
            ViewBag.theGoClick = 2;
            Session["theGoClick"] = 2;
            try
            {
                m_User = GetUser("Diablo").User;
                m_PSW = GetUser("Diablo").PSW;

                XmlDocument doc = new XmlDocument();

                string Path = Server.MapPath("");

                if (!Path.EndsWith("\\"))
                    Path += "\\";

                doc.Load(Path + "Nlog.config");

                LogManager.Configuration = new XmlLoggingConfiguration(doc.DocumentElement, null);
                logger = LogManager.GetLogger("Travelbytes.TravelPlans");
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Logger initialisation failed\r\n" + e.Message);
            }


            // Put user code to initialize the page here
            LoadSessionVars();
            GetQueryVars();
            Event1 = new AutoResetEvent(false);
            Event2 = new AutoResetEvent(false);
            Event3 = new AutoResetEvent(false);
            Event4 = new AutoResetEvent(false);

            ViewBag.Adobe = false;
            //Adobe.Visible = false;

            if (m_DirectLink)
                DisplayFile();

            if (Request.QueryString["F"] != null)
            {
                m_FileName = Convert.ToString(Request.QueryString["F"]);
            }

            if (m_RLOC.Length == 6 &&
                 m_FileName.Length > 0 &&
                 m_Location.Length > 0 &&
                 (m_Type == "V" || m_Type == "I")
                 )
            {


                // build location string
                string Store = "";
                string Extention = "";
                if (m_Type == "V")
                {
                    Store = GetInvoiceStore();
                    Extention = ".pdf";
                }
                else if (m_Type == "I")
                {
                    Store = GetItineraryStore();
                    Extention = ".doc";
                }
                if (m_Location == "NA")
                    m_Location = "";

                string Location = Store;

                if (!Location.EndsWith("\\"))
                    Location += "\\";

                if (m_Location.StartsWith(@"\\"))
                    Location = m_Location;
                else
                    Location += m_Location;

                if (!Location.EndsWith("\\"))
                    Location += "\\";

                Location += m_RLOC[0] +
                    "\\" + m_RLOC[1] +
                    "\\" + m_RLOC[2] +
                    "\\" + m_RLOC[3] +
                    "\\" + m_RLOC[4] +
                    "\\" + m_FileName;


                bool result = DisplayFilePDF(m_RLOC, Location, 0, Extention);
                if (result == false)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 28 });
                }


                if (m_URL.Length > 0)
                {
                    Tranx Tran = new Tranx();
                    Tran.InvoiceRetrieve(m_RLOC, m_Surname, m_FileName);
                    String output = m_URL;
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + m_URL);
                    return File(output, "application/pdf");
                    //Response.Redirect(m_URL, true);
                }

                return View();  //Need to work 
            }

            if (m_InvoiceNumber.Length > 0)
            {
                int rValue = PullInvoice();
                if (rValue == 0)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 13 });
                }
                else if (rValue == 1)
                {
                    return RedirectToAction("InvoiceView", "Invoice", new { T = "V", R = gRLOC, M = gLocation, F = gInfoName });
                }
                else if (rValue == 25)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 25 });
                }
                else if (rValue == 26)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 26 });
                }
                else if (rValue == 27)
                {
                    return RedirectToAction("Message", "MessageDisplay", new { E = 27 });
                }
            }


            if ((m_RLOC == null || m_RLOC.Length == 0) || (m_Surname == null || m_Surname.Length == 0))
            {
                logger.Debug("Enter : debug point 5");
                return RedirectToAction("Message", "MessageDisplay", new { E = 1 });
                //return RedirectToAction("Index", "User", new { E = 1 });
                //Response.Redirect("Default.aspx?E=1");             
            }


            bool bReturnValue = DoGo();
            if (bReturnValue == false)
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 33 });
            }
            logger.Debug("m_RLOC : " + m_RLOC);
            return View();
        }

        #endregion

        #region Helper Methods


        protected override void GetQueryVars()
        {
            base.GetQueryVars();

            if (Request.QueryString["F"] != null)
            {
                m_FileName = Request.QueryString["F"].ToString();
            }
            if (Request.QueryString["D"] != null)
            {
                if (Request.QueryString["D"].ToString().ToUpper() == "T" &&
                    m_Surname.Trim().Length > 0 &&
                    m_RLOC.Trim().Length > 0 &&
                    m_InvoiceNumber.Trim().Length > 0)
                    m_DirectLink = true;
            }

            if (m_Location.Length == 0)
                m_Location = "NA";
        }

        /// <summary>
        /// DoGo
        /// </summary>
        /// <param name="objSender"></param>
        /// <param name="objArgs"></param>
        protected bool DoGo()
        {

            logger.Debug("Enter : debug point 1");
            m_Surname = m_Surname.ToUpper();
            //outError.InnerHtml = "";
            //Results.InnerHtml = "";

            string results = "";
            string OutError = "";
            ViewBag.OutError = "";
            ViewBag.Results = "";

            //outError.InnerHtml = "<span class='error'>" + m_Surname + "</span>";

            bool bDataDrawn = false;
            m_InvoiceArray = null;
            m_Summary = "";

            AutoResetEvent[] evs = new AutoResetEvent[4];
            evs[0] = Event1;    // Event for t1
            evs[1] = Event2;    // Event for t2
            evs[2] = Event3;    // Event for t3
            evs[3] = Event4;    // Event for t4


            try
            {
                logger.Debug("Enter : debug point 2");
                Thread t1 = new Thread(new ThreadStart(ThreadDiskInvoice));
                Thread t2 = new Thread(new ThreadStart(ThreadDiskItinerary));
                Thread t3 = new Thread(new ThreadStart(ThreadSearchDataBase));
                Thread t4 = new Thread(new ThreadStart(ThreadCheckSummary));
                logger.Debug("Starting Search Threads");
                t1.Start();   // 
                t2.Start();   // 
                t3.Start();   // 
                t4.Start();   // 
                logger.Debug("m_RLOC : " + m_RLOC);
                logger.Debug("ending Search Threads");
                // wait for all threads to finish before we continue
                if (!WaitHandle.WaitAll(evs, 60000, false))
                {

                    logger.Debug("Thread wait timed out returning failed");
                    logger.Error("Thread wait timed out returning failed");
                    //outError.InnerHtml += "<br><span class='error'>System Timed out processing your request.</span>";                
                    OutError = OutError + "<br><span class='error'>System Timed out processing your request.</span>";
                    return false;
                }

            }
            catch (Exception ex)
            {
                logger.Debug("error :" + ex.Message);
                return false;
            }


            DataSet DS = m_InvoicesOnDisk;
            DataSet DSItin = m_ItinerariesOnDisk;
            DiskRecord[] DbRecs = m_DiskRecords;
            //			DrawTripSummary(m_RLOC);

            // if it is not stored no use matching to database
            //  if ((m_InvoicesOnDisk != null && m_InvoicesOnDisk.Tables != null && m_InvoicesOnDisk.Tables[0].Rows.Count > 0))
            {

                logger.Debug("Enter : debug point 3");
                // now check we have the same number on the database
                if ((DbRecs != null) && DbRecs.Length > 0)//== m_InvoicesOnDisk.Tables[0].Rows.Count)
                {
                    // now use a one to one match to display them
                    logger.Debug("DbRecs : " + DbRecs.Length.ToString());


                    //DataRow[] drarrayInvoiceOnDisk = m_InvoicesOnDisk.Tables[0].Select("", "TransactionDate");
                    results = "<table class=\"InvoiceSummary\" border=\"1\" >" + DrawLiveItinerary(m_RLOC, m_Surname) +
                        "<tr><td class=\"InvoiceSummaryHeader\">Invoice</td><td class=\"InvoiceSummaryHeader\">Ticket Number</td><td class=\"InvoiceSummaryHeader\">Invoice Date</td><td class=\"InvoiceSummaryHeader\">&nbsp;</td></tr>";
                    string LastTKTNumber = "";
                    string LastDocNumber = "";
                    //for (int i = 0; i < DbRecs.Length; i++)
                    for (int i = DbRecs.Length - 1; i >= 0; i--)
                    {

                        DataRow Table; //, File;

                        Table = DbRecs[i].Row;

                        if (DbRecs[i].NameFound)
                        {

                            if (LastDocNumber == Table["DocumentNumber"].ToString() &&
                                    (LastTKTNumber == Table["TicketNumber"].ToString()))
                                continue;

                            // File = drarrayInvoiceOnDisk[i];

                            bDataDrawn = true;

                            DateTime FileDate = DateTime.Parse(Table["Created"].ToString());

                            string Filename = Table["Filename"].ToString();

                            string[] MyPath = Path.GetFullPath(Table["Filename"].ToString()).Split('\\');
                            string month = "";
                            string Year = "JANFEBMARAPRMAYJUNJULAUGSEPOCTNOVDEC";
                            foreach (string str in MyPath)
                            {
                                if (str.Length == 3 && Year.IndexOf(str.ToUpper()) != -1)
                                {
                                    month = str;
                                    break;
                                }
                            }

                            //results = results + "<tr>" +
                            //    "<td class=\"InvoiceSummary\">&nbsp;" + Table["DocumentNumber"].ToString() + "&nbsp;</td>" +
                            //    "<td class=\"InvoiceSummary\">&nbsp;" + Table["TicketNumber"] + "&nbsp;</td>" +
                            //    "<td class=\"InvoiceSummary\">" + FileDate.ToString("dd MMMM yyyy") + "</td>" +
                            //    "<td class=\"InvoiceSummary\"><A target='_blank' class=\"ViewInvoice\" href='Invoice.aspx?T=V&R=" + m_RLOC + "&M=" + month +
                            //    "&F=" + Path.GetFileName(Filename) + "'><img src='images/pdf.gif' border=0 TITLE='View Invoice'><span class=\"ViewInvoice\">&nbsp;View/Download&nbsp;</span></a></td>" +
                            //    "</tr>";


                            string root = "/";
                            string redirectTo = "Invoice/InvoiceView?T=V&R=" + m_RLOC + "&M=" + month + "&F=" + Path.GetFileName(Filename);
                            string redirectPath = root + redirectTo;

                            results = results + "<tr>" +
                                "<td class=\"InvoiceSummary\">&nbsp;" + Table["DocumentNumber"].ToString() + "&nbsp;</td>" +
                                "<td class=\"InvoiceSummary\">&nbsp;" + Table["TicketNumber"] + "&nbsp;</td>" +
                                "<td class=\"InvoiceSummary\">" + FileDate.ToString("dd MMMM yyyy") + "</td>" +
                                "<td class=\"InvoiceSummary\"><A target='_blank' class=\"ViewInvoice\" href='" + redirectPath + "'><img src='images/pdf.gif' border=0 TITLE='View Invoice'><span class=\"ViewInvoice\">&nbsp;View/Download&nbsp;</span></a></td>" +
                                "</tr>";

                            LastDocNumber = Table["DocumentNumber"].ToString();
                            LastTKTNumber = Table["TicketNumber"].ToString();
                        }
                    }

                    results += "</table>";
                    ViewBag.Results = results;
                    logger.Debug("Enter : debug point 4");
                    return true;
                }
                else
                {
                    string Text = "";

                    if (DbRecs == null)
                        Text = " Database records DS was null";
                    else
                    {
                        Text = "length dbRecs = " + DbRecs.Length.ToString() + " Invoice on disk = " + m_InvoicesOnDisk.Tables[0].Rows.Count;
                    }

                    logger.Debug("Invoices in database / found on disk" + Text);
                    return false;
                }

            }
            if (m_URL.Length > 0)
            {
                // Response.Redirect(m_URL, true);
                return true;
            }
            else
            {
                if (!bDataDrawn)
                {
                    if (m_bFilesFound)
                        OutError += "<br><span class='error'>Name doesn't match the reference provided.</span>";
                    else
                    {
                        ViewBag.Results = "<table>" + DrawLiveItinerary(m_RLOC, m_Surname) + "</table>";
                        OutError += "<br><span class='error'>" + NotFoundError + "</span>";
                    }
                    return false;
                }
                else
                    //Adobe.Visible = true;
                    ViewBag.Adobe = true;
                return true;

            }
        }

        /// <summary>
        /// PullInvoice
        /// </summary>
        protected int PullInvoice()
        {
            string results = "";
            string OutError = "";
            ViewBag.OutError = "";
            ViewBag.Results = "";

            //outError.InnerHtml = "";
            //Results.InnerHtml = "";
            m_InvoiceArray = null;
            m_Summary = "";

            m_Surname = m_Surname.ToUpper();

            DataRow[] Invoices = SearchDataBaseForInvoice(m_Surname);

            // if it is not stored no use matching to database
            if (Invoices != null && Invoices.Length > 0)
            {

                // now make sure we have a matching record on disk
                foreach (DataRow Row in Invoices)
                {

                    string ID = Row["ID"].ToString();

                    if (Row["FileName"] != null && Row["FileName"].ToString().Length > 0)
                    {
                        System.IO.FileInfo Info = null;

                        // we can redirect directly
                        // need to check it is still on the disk
                        try
                        {
                            Info = new System.IO.FileInfo(Row["FileName"].ToString());
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Trace.WriteLine(e.Message);
                            logger.Error("Direct access to file: File was not found");
                            OutError += "<br><span class='error'>Invoice not found.</span>";
                            return 25;  //ErrorNumber in ErrorDisplayController
                        }
                        if (Info != null)
                        {

                            string InvoiceStore = GetInvoiceStore();

                            string Location = Info.FullName.Replace(GetInvoiceStore().ToLower(), "").ToLower();


                            // Take out the name
                            Location = Location.Replace(Info.Name.ToLower(), "");

                            string RLOC = Row["RLOC"].ToString();

                            string RLOCLOcation = RLOC[0] +
                                "\\" + RLOC[1] +
                                "\\" + RLOC[2] +
                                "\\" + RLOC[3] +
                                "\\" + RLOC[4] +
                                "\\";

                            Location = Location.Replace(RLOCLOcation.ToLower(), "");

                            // finished now lets send on
                            //return 1;                           
                            //string URL = "Invoice.aspx?T=V&R=" + RLOC + "&M=" + Location + "&F=" + Info.Name;
                            //Response.Redirect(URL, true);
                            gRLOC = RLOC;
                            gLocation = Location;
                            gInfoName = Info.Name;
                            return 1;
                        }
                    }

                    // check I have m_Rloc set
                    m_RLOC = Row["RLOC"].ToString();

                    m_InvoiceArray = null;

                    AutoResetEvent[] evs = new AutoResetEvent[2];
                    evs[0] = Event1;    // Event for t1
                    evs[1] = Event3;    // Event for t2

                    Thread t1 = new Thread(new ThreadStart(ThreadDiskInvoice));
                    Thread t2 = new Thread(new ThreadStart(ThreadSearchDataBase));
                    //Thread t2 = new Thread(new ThreadStart(ThreadSearchDataBasePerRLOC));
                    logger.Debug("Starting Search Threads");
                    t1.Start();   // 
                    t2.Start();   // 

                    // wait for all threads to finish before we continue
                    if (!WaitHandle.WaitAll(evs, 20000, false))
                    {
                        logger.Error("Thread wait timed out returning failed");
                        OutError += "<br><span class='error'>System Timed out processing your request.</span>";
                        return 26; //ErrorNumber in ErrorDisplayController
                    }


                    DataRow[] drarrayInvoiceOnDisk = m_InvoicesOnDisk.Tables[0].Select("", "TransactionDate");
                    //					DataRow[]  DbRecs = m_DbRecs;
                    DiskRecord[] DbRecs = m_DiskRecords;

                    if (DbRecs != null)
                    {
                        for (int i = 0; i < DbRecs.Length; i++)
                        {
                            DataRow Table, File;

                            Table = DbRecs[i].Row;

                            string RowID = Table["ID"].ToString();
                            string RowIDInvoiceNumber = Row["ID"].ToString();

                            if (Row["ID"].ToString() == Table["ID"].ToString() && DbRecs[i].NameFound)
                            {
                                // found the record and matching file
                                File = drarrayInvoiceOnDisk[i];

                                DateTime FileDate = (DateTime)File["TransactionDate"];

                                ViewBag.OutError = OutError;

                                // finished now lets send on
                                string URL = "Invoice.aspx?T=V&R=" + m_RLOC + "&M=" + File["Month"].ToString() + "&F=" + File["Name"].ToString();

                                Response.Redirect(URL, true);

                                return 1;

                            }

                        }
                    }

                }


            }
            else
            {

                string Text = "";

                if (m_InvoicesOnDisk == null)
                    Text = " Invoices on disk DS was null";
                else if (m_InvoicesOnDisk.Tables == null)
                    Text = " Invoices on disk DS Tables was null";
                else if (m_InvoicesOnDisk.Tables[0].Rows.Count == 0)
                    Text = " Invoices on no rows found";

                logger.Debug("No invoices found on disk" + Text);

                return 27; //ErrorNumber in ErrorDisplayController
            }

            OutError += "<br><span class='error'>" + NotFoundError + "</span>";
            ViewBag.OutError = OutError;
            return 1;
        }

        /// <summary>
        /// DisplayFile
        /// </summary>
        void DisplayFile()
        {
            DataRow[] DbRecs = SearchDataBaseForInvoice(m_Surname.Trim());

            if (DbRecs == null)
                Response.Redirect("error.aspx?e=15");

            foreach (DataRow myRow in DbRecs)
            {
                string DocNumber = myRow["DocumentNumber"].ToString().ToUpper();
                string FileName = myRow["FileName"].ToString().ToUpper();

                if (DocNumber != null &&
                    DocNumber.Length > 0 && DocNumber == m_InvoiceNumber)
                {

                    if (FileName != null && FileName.Length > 0)
                    {

                        // found the record

                        if (DisplayFilePDF(m_RLOC, FileName, 1, ".pdf"))
                        {
                            Response.Redirect(m_URL);
                        }
                    }
                }
            }
            Response.Redirect("error.aspx?e=15");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RLOC"></param>
        /// <param name="Source"></param>
        /// <param name="Count"></param>
        /// <param name="Extention"></param>
        /// <returns></returns>
        public bool DisplayFilePDF(string RLOC, string Source, int Count, string Extention)
        {

            string FileName, OutFileName;
            int Status;

            string results = "";
            string OutError = "";
            ViewBag.OutError = "";
            ViewBag.Results = "";

            FileName = Session.SessionID + Extention;
            m_FileName = FileName;


            string InFileName;
            InFileName = Source;

            string TempDir = Server.MapPath("/Temp");
            TempDir = @"c:\temp";
            if (!TempDir.EndsWith("\\"))
                TempDir += "\\";

            string WebTempDir = Server.MapPath("/Temp");
            if (!WebTempDir.EndsWith("\\"))
                WebTempDir += "\\";

            string szSource = TempDir + FileName + ".txt";


            if (GetDebug() && GetAltDebug())
            {
                //Source = Source.ToLower().Replace(@"203.19.215.74\itineraries\", @"203.19.215.74\cdrive\travelbytes\itineraries\");
            }

            // copy infile to local drive
            try
            {
                //using (new Impersonator("woz", "", "2rachelle"))
                //{
                    System.IO.File.Copy(Source, szSource, true);
                //}
            }
            catch (Exception e)
            {
                logger.Error("Error file, " + e.Message);
                OutError = "<span class='error'>Can't process this file" + "</span>";
                ViewBag.OutError = OutError;
                return false;
            }

            OutFileName = TempDir + FileName;

            logger.Debug("Decrypting " + szSource + " to " + OutFileName);

            //using ( new Impersonator( m_User, ".", m_PSW ) )
            //{
            logger.Debug("should be Processing as " + m_User);

            Status = FileDecrypt(szSource, OutFileName);
            //}
            System.IO.File.Delete(szSource);

            if (Status != 1)
            {
                logger.Error("Error decrypting file, " + m_LastError);
                OutError = "<span class='error'>Can't process this file: " + m_LastError + "</span>";
                ViewBag.OutError = OutError;
                return false;
            }

            string TestBuff = ReadFileFromDisk(OutFileName);

            if (TestBuff.StartsWith("%PDF"))
            {
                FileName = m_FileName + ".pdf";

            }
            else if (TestBuff.StartsWith("{\\rtf"))
            {
                FileName = m_FileName + ".rtf";
            }
            else if (TestBuff.StartsWith("<htm"))
            {
                FileName = m_FileName + ".htm";
            }
            else if (TestBuff.StartsWith("<?xml"))
            {
                FileName = m_FileName + ".xml";
            }
            else
                FileName = m_FileName + ".doc";


            System.IO.File.Copy(OutFileName, WebTempDir + FileName, true);
            System.IO.File.Delete(OutFileName);

            string path = Server.MapPath("/Temp/");
            m_URL = path + FileName;

            logger.Debug("redirecting to " + m_URL);

            return true;

        }


        /// <summary>
        /// ReadFileFromDisk
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public string ReadFileFromDisk(string FileName)
        {
            string Data = "";
            try
            {
                //using (new Impersonator("woz", "", "2rachelle"))
               // {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        // use the BCL to load the part in a byte[] buffer
                        byte[] bufferWorkbookPart = new BinaryReader(fs).ReadBytes((int)fs.Length);

                        // use our BIFF12 reader
                        // ByteConverter con = new ByteConverter();
                        Data = System.Text.Encoding.GetEncoding("windows-1252").GetString(bufferWorkbookPart);
                    }
               // }
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            return Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Surname"></param>
        /// <returns></returns>
        DataRow[] SearchDataBaseForInvoice(string Surname)
        {
            string outError = "";
            ViewBag.OutError = "";

            outError = "";
            DataRow[] rv = null;

            try
            {
                DataSet DS = new DataSet();
                MailRecordsBAL objMailRecordsBAL = new MailRecordsBAL("");
                List<MailRecordsModel> lstMailRecordsModel = objMailRecordsBAL.SearchDataBaseForInvoice(m_InvoiceNumber, m_InvoiceNumber);
                DataTable dt = Common.ListToDataTable(lstMailRecordsModel);
                DS.Tables.Add(dt);

                if (DS == null || DS.Tables == null || DS.Tables[0].Rows.Count == 0)
                {
                    logger.Warn("No records found in mailrecords table matching Invoice/Ticket Numbers provided");
                    return rv;
                }

                // need to replace all angle brackets with &lt;&gt;
                bool bFound = false;

                string Created = "";
                string RLOC = "";

                logger.Debug("Invoice/Ticket found : " + DS.Tables[0].Rows.Count.ToString());

                foreach (DataRow myRow in DS.Tables[0].Rows)
                {
                    string Subject = myRow["Subject"].ToString().ToUpper();
                    string FormName = myRow["FormName"].ToString().ToUpper();
                    string CurrentRLOC = myRow["RLOC"].ToString().ToUpper();

                    if (FormName == null || !(FormName.IndexOf("INV") != -1 || FormName.IndexOf("CRD") != -1))
                    //if (FormName == null || FormName.IndexOf("INV") == -1)
                    {
                        // must be a copy record
                        continue;
                    }

                    if (Created == myRow["Created"].ToString())
                        continue;

                    Created = myRow["Created"].ToString();


                    Match Ma = Regex.Match(Subject, GetNameCommand());
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

                    string NewName = Names[0].Trim();


                    if (NewName.IndexOf(Surname) != -1)
                    {
                        if (CurrentRLOC != RLOC && RLOC.Length != 0)
                        {
                            logger.Error("Multiple rlocs with same name and invoice number");
                            // nned to throw some error here
                            //return RedirectToAction("Message", "MessageDisplay", new { E = 13 });
                            return null;
                            //Response.Redirect("error.aspx?E=13", true);                            
                        }

                        RLOC = CurrentRLOC;
                        bFound = true;

                        // must be an Invoice
                        if (m_InvoiceArray == null)
                        {
                            m_InvoiceArray = new DataRow[1];
                            m_InvoiceArray[0] = myRow;
                        }
                        else
                        {
                            DataRow[] Invoice = new DataRow[m_InvoiceArray.Length + 1];
                            m_InvoiceArray.CopyTo(Invoice, 0);
                            Invoice[Invoice.Length - 1] = myRow;
                            m_InvoiceArray = Invoice;
                        }

                    }

                }

                if (m_InvoiceArray != null)
                    logger.Debug("Records found for TKT/Invoice in DB: " + m_InvoiceArray.Length.ToString() + "");
                else
                    logger.Debug("Records found for TKT/Invoice in DB: zero");


                if (bFound)
                    return m_InvoiceArray;

            }
            catch (MySqlException oleExp)
            {
                return rv;
                ////display error details
                //outError = "<b>* Error while accessing data</b>.<br />"
                //    + oleExp.Message + "<br />" + oleExp.Source + "<p />";
                //logger.Error("Exception in Check Surname : " + oleExp.ToString());
                //ViewBag.OutError = outError;
                //return rv;  // and stop execution

            }
            catch (Exception objError)
            {

                //display error details
                outError = "<b>* Error while accessing data</b>.<br />"
                    + objError.Message + "<br />" + objError.Source + "<p />";
                logger.Error("Exception in Check Surname : " + objError.ToString());
                ViewBag.OutError = outError;
                return rv;  // and stop execution

            }
            return rv;

        }

        /// <summary>
        /// 
        /// </summary>
        void ThreadDiskInvoice()
        {
            logger.Debug("Disk Search Invoice started");
            //using (new Impersonator(m_User, ".", m_PSW))
            //{ }
            logger.Debug("Disk Search Invoice in way");
            // m_InvoicesOnDisk = FindInvoiceRecordsOnDisk(m_RLOC);
            Event1.Set();     // AutoResetEvent.Set() flagging method is done
            System.Diagnostics.Trace.WriteLine("ThreadDiskInvoice Completed");
            logger.Debug("Disk Search Invoice finished");

        }

        /// <summary>
        /// 
        /// </summary>
        void ThreadDiskItinerary()
        {
            //m_ItinerariesOnDisk = FindITineraryRecordsOnDisk( m_RLOC);
            Event2.Set();     // AutoResetEvent.Set() flagging method is done
            System.Diagnostics.Trace.WriteLine("ThreadDiskItinerary Completed");
            logger.Debug("Disk Search Itinerary finished");
        }

        /// <summary>
        /// ThreadSearchDataBase
        /// </summary>
        void ThreadSearchDataBase()
        {
            logger.Debug("Surname Check started");
            logger.Debug("m_RLOC : " + m_RLOC);
            m_DiskRecords = SearchDataBase(m_RLOC, m_Surname);
            Event3.Set();     // AutoResetEvent.Set() flagging method is done
            System.Diagnostics.Trace.WriteLine("ThreadSearchDataBase Completed");
            logger.Debug("Surname Check Finished");
        }

        /// <summary>
        /// SearchDataBase
        /// </summary>
        /// <param name="RLOC"></param>
        /// <param name="Surname"></param>
        /// <returns></returns>
        DiskRecord[] SearchDataBase(string RLOC, string Surname)
        {
            string outError = "";
            ViewBag.OutError = "";

            ViewBag.OutError = "";
            DiskRecord[] rv = null;
            Surname = Surname.ToUpper();

            try
            {
                // now calculate the MD5 connection string
                //MySqlConnection Conn = new MySqlConnection(GetDataSourceMySQL());
                //Conn.Open();
                //string SQL = "";

                DateTime tm = DateTime.Now.AddMonths(GetSeachMonths());
                DateTime ThreeMonths = new DateTime(tm.Year, tm.Month, 1, 0, 0, 0, 0);

                DataSet DS = new DataSet();
                MailRecordsBAL objMailRecordsBAL = new MailRecordsBAL("");
                List<MailRecordsModel> lstMailRecordsModel = objMailRecordsBAL.SearchDataBase(RLOC, ThreeMonths);  //.ToString("yyyy-MM-dd HH:mm")
                DataTable dt = Common.ListToDataTable(lstMailRecordsModel);
                DS.Tables.Add(dt);


                //SQL = "SELECT ID, RLOC, Subject, TicketNumber, Created, Method, DocumentNumber, FormName, FileName FROM MailRecords where RLOC='{0}' and Created > '{1}' and PreprocStatus > 0 order by created";               
                //SQL = string.Format(SQL, RLOC, ThreeMonths.ToString("yyyy-MM-dd HH:mm"));
                //MySqlCommand selectCMD = new MySqlCommand(SQL, Conn);

                //MySqlDataAdapter custDA = new MySqlDataAdapter(selectCMD);
                //DataSet DS = new DataSet();

                //custDA.Fill(DS);
                //Conn.Close();

                if (DS == null || DS.Tables == null || DS.Tables[0].Rows.Count == 0)
                {
                    logger.Debug("No records found in mailrecords table matching criteria");
                    // logger.Debug(SQL);
                    return rv;
                }

                // need to replace all angle brackets with &lt;&gt;

                string Created = "";
                string LastSubject = "";

                logger.Debug("Mailrecords found : " + DS.Tables[0].Rows.Count.ToString());
                foreach (DataRow myRow in DS.Tables[0].Rows)
                {
                    string Subject = myRow["Subject"].ToString().ToUpper();
                    string FormName = myRow["FormName"].ToString().ToUpper();
                    string CreatedTime = myRow["Created"].ToString();

                    if (!(FormName.IndexOf("INV") != -1 || FormName.IndexOf("CRD") != -1))
                        continue;

                    if (myRow["Created"].ToString() == Created && LastSubject == Subject)
                    {
                        // must be a copy record
                        continue;
                    }

                    // fill with latest record
                    Created = myRow["Created"].ToString();
                    LastSubject = Subject;

                    DiskRecord rec = new DiskRecord();
                    rec.Row = myRow;

                    Match Ma = Regex.Match(Subject, GetNameCommand());
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


                    string NewName = Names[0].Trim();



                    //  if (NewName == Surname)
                    if (LastSubject.IndexOf(Surname) != -1)
                    {
                        //bFound = true;
                        rec.NameFound = true;
                        if (m_InvoiceArray == null)
                        {
                            m_InvoiceArray = new DataRow[1];
                            m_InvoiceArray[0] = myRow;
                        }
                        else
                        {
                            DataRow[] Invoice = new DataRow[m_InvoiceArray.Length + 1];
                            m_InvoiceArray.CopyTo(Invoice, 0);
                            Invoice[Invoice.Length - 1] = myRow;
                            m_InvoiceArray = Invoice;
                        }

                    }

                    if (rv == null)
                    {
                        rv = new DiskRecord[1];
                        rv[0] = rec;
                    }
                    else
                    {
                        DiskRecord[] Invoice = new DiskRecord[rv.Length + 1];
                        rv.CopyTo(Invoice, 0);
                        Invoice[Invoice.Length - 1] = rec;
                        rv = Invoice;
                    }



                }

                if (m_InvoiceArray != null)
                    logger.Debug("Records found for Invoice in DB: " + m_InvoiceArray.Length.ToString() + "");
                else
                    logger.Debug("Records found for Invoice in DB: zero");

            }
            catch (MySqlException oleExp)
            {

                //display error details
                ViewBag.OutError = "<b>* Error while accessing data</b>.<br />"
                    + oleExp.Message + "<br />" + oleExp.Source + "<p />";
                logger.Error("Exception in Check Surname : " + oleExp.ToString());
                return rv;  // and stop execution

            }
            catch (Exception objError)
            {

                //display error details
                ViewBag.OutError = "<b>* Error while accessing data</b>.<br />"
                    + objError.Message + "<br />" + objError.Source + "<p />";
                logger.Error("Exception in Check Surname : " + objError.ToString());
                return rv;  // and stop execution

            }
            return rv;

        }

        /// <summary>
        /// DrawLiveItinerary
        /// </summary>
        /// <param name="Ref"></param>
        /// <param name="Surname"></param>
        /// <returns></returns>
        string DrawLiveItinerary(string Ref, string Surname)
        {
            string rv = ""; //"<tr><td colspan=3 align=center><A href='itinerary.aspx?R=" + Ref + "&N=" + Surname + "'>View live Itinerary</a></td></tr>";
            if (m_Summary.Length > 0)
                rv += "<tr><td colspan=3 align=center>" + m_Summary + "</td></tr>";

            return rv;
        }

        /// <summary>
        /// ThreadCheckSummary
        /// </summary>
        void ThreadCheckSummary()
        {
            m_Summary = "";//DrawTripSummary(m_RLOC);
            Event4.Set();     // AutoResetEvent.Set() flagging method is done
            System.Diagnostics.Trace.WriteLine("ThreadCheckSummary Completed");
            logger.Debug("Trip Summary Finished");
        }

        #endregion
    }

    class DiskRecord
    {
        public DataRow Row = null;
        public bool NameFound = false;
    }

}
