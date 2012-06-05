using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Collections;
using Travelbytes;
using Travelbytes.ClassMap;
using MTP_JAPA.Helpers;
using System.Data;
using System.IO;
using mshtml;
using MySql.Data.MySqlClient;
using MTP.BAL;
using MTP.DTO;
using System.Text.RegularExpressions;
using System.Xml;
using NLog;
using NLog.Config;

namespace MTP_JAPA.Controllers
{
    public class EticketController : BaseController
    {
        //
        // GET: /Eticket/
        string msg = "<span class=\"Error\" >We are unable to locate an E-ticket for this booking.<br/>An E-ticket may not have been issued yet or a paper ticket is required.<br/>Please contact your travel team if you require more information.<br/>";
        static AutoResetEvent Event1 = new AutoResetEvent(false);
        static AutoResetEvent Event2 = new AutoResetEvent(false);
        static AutoResetEvent Event3 = new AutoResetEvent(false);
        static AutoResetEvent Event4 = new AutoResetEvent(false);

        //protected System.Web.UI.WebControls.Label Label1;
        //protected System.Web.UI.WebControls.Label Label2;
        //protected System.Web.UI.WebControls.RegularExpressionValidator ReferenceValidator;
        //protected System.Web.UI.WebControls.TextBox tbSurname;
        //protected System.Web.UI.WebControls.TextBox tbReference;
        string m_URL = "";
        string m_FileName = "";
        string m_TicketNumber = "";
        // Filed by the processing
        string m_User = "";
        string m_PSW = "";
        public string m_Jump = "";
        int m_error_number;
        string m_error = "";
        ArrayList m_Etickets = new ArrayList();
        ArrayList m_URLS = new ArrayList();

        #region Load

        /// <summary>
        /// EticketView
        /// </summary>
        /// <returns></returns>
        public ActionResult EticketView()
        {
            ViewBag.TabDisplay = 1;
            ViewBag.theGoClick = 3;
            Session["output"] = null;
            Session["theGoClick"] = 3;
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

            m_Jump = "";
            // Put user code to initialize the page here
            LoadSessionVars();
            GetQueryVars();

            if (m_RLOC.Length == 6 && m_Type == "T")
            {
                if (FindEticketRecords(m_RLOC))
                {
                    String output = Convert.ToString(Session["output"]);
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + output);
                    return File(output, "application/pdf");
                }
            }
            if (Session["HTMLFileName"] != null)
            {

                if (FindEticketRecords(m_RLOC))
                {
                    if (Session["output"] != null)
                    {
                        String output = Convert.ToString(Session["output"]);
                        if (output != null & output != "")
                        {
                            Response.AppendHeader("Content-Disposition", "inline; filename=" + output);
                            return File(output, "application/pdf");
                        }
                        else
                        {
                            return RedirectToAction("Message", "MessageDisplay", new { E = 29 });
                        }
                    }
                    return View();
                }
            }

            if (Session["HTMLFileName"] == null)
            {
                if (m_RLOC.Length == 6 && m_Type == "E")
                {
                    if (FindEticketRecords(m_RLOC))
                    {
                        if (Session["output"] != null)
                        {
                            String output = Convert.ToString(Session["output"]);
                            if (output != null & output != "")
                            {
                                Response.AppendHeader("Content-Disposition", "inline; filename=" + output);
                                return File(output, "application/pdf");
                            }
                            else
                            {
                                return RedirectToAction("Message", "MessageDisplay", new { E = 29 });
                            }
                        }
                        return View();
                    }
                }
            }

            if (Request.QueryString["ET"] != null)
            {
                if (FindEticketRecords(m_RLOC))
                {
                    String output = Convert.ToString(Session["output"]);
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + output);
                    return File(output, "application/pdf");
                }
            }

            string CountryList = GetLiveCountryList();

           // if ((Session["Country"] == null && GetLiveCountryList().IndexOf(Session["Country"].ToString()) != -1))
            //{
                // Goto error
                //int rValue = LoadItinerary();  //Need TO work-Discussed with Phil
                //if (rValue == 1)
                //{
                //    return RedirectToAction("Message", "MessageDisplay", new { E = m_error_number });
                //}
                //else if (rValue == 2)
                //{
                //    return RedirectToAction("Message", "MessageDisplay", new { E = 7 });
                //}
                //else if (rValue == 3)
                //{
                //    return RedirectToAction("Message", "MessageDisplay", new { E = 0, M = m_error });
                //}
                //else if (rValue == 4)
                //{
                //    return RedirectToAction("Message", "MessageDisplay", new { E = 5 });
                //}
                //else if (rValue == 5)
                //{
                //    return RedirectToAction("Message", "MessageDisplay", new { E = 2 });
                //}
                //if (rValue != 0)
                //{
                //    Response.End();
                //}
            //}


            if (Session["Etickets"] != null)
            {
                //m_Etickets = (ArrayList)Session["Etickets"]; //Need TO work-Discussed with Phil

                //ProcessEtickets();

                //if (m_URL.Length > 0)
                //{
                //    // 
                //    Tranx Trans1 = new Tranx();
                //    Trans1.EticketRetrieve(this.m_RLOC, m_Surname, m_TicketNumber);

                //    m_Jump = m_URL;
                //    //Response.Redirect( m_URL, true );
                //}

                // outError.InnerHtml = msg;
                // return View(); ;
            }

            //DropDownList1.Visible = false;  //Need TO ask
            //Submit1.Visible=false;
            //etLabel.Visible=false;
            //outError.InnerHtml = msg;
            //outError.Visible = true;            
            return RedirectToAction("Message", "MessageDisplay", new { E = 24 });

            // should no be here unless etickets are present
            // if (Request.Form["Submit1"] != null)    //Need To ask
            //return RedirectToAction("Index", "User", new { E = 1 });
            //Response.Redirect("Default.aspx?E=1");
        }


        #endregion

        #region Events

        /// <summary>
        /// DoGo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>   
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EticketView(FormCollection formCollection)
        {
            string URL = ""; // DropDownList1.SelectedValue;            
            foreach (string key in formCollection.Keys)
            {
                if (key == "ddlETicket")
                {
                    URL = formCollection[key];
                }
            }

            m_Jump = URL;
            if (m_Jump != null & m_Jump != "")
            {
                Response.AppendHeader("Content-Disposition", "inline; filename=" + m_Jump);
                return File(m_Jump, "application/pdf");
            }
            else
            {
                return RedirectToAction("Message", "MessageDisplay", new { E = 29 });
            }
        }

        #endregion

        #region Property

        public string GetJump
        {
            get
            {
                //ViewBag.m_Jump = m_Jump;
                return ViewBag.m_Jump;
            }
            set
            {
                //m_Jump = value;
                ViewBag.m_Jump = value;
            }
        }

        #endregion

        #region Helper Methods

        void ProcessEtickets()
        {
            Travelbytes.WebService.Itinerary.Decoder Decoder = new Travelbytes.WebService.Itinerary.Decoder();
            Decoder.Reload = false;
            Decoder.Config = Server.MapPath("");
            Decoder.Templates = Server.MapPath("Templates/");
            Decoder.AddressDSN = GetAddressDataSourceMySQL();
            Credentials Cred = GetUser("Galileo");
            Decoder.User = Cred.User;
            Decoder.PSW = Cred.PSW;
            Decoder.HAP = Cred.HAP;
            Decoder.URL = Cred.URL;
            Decoder.RLOC = m_RLOC;
            Decoder.Offset = GetOffSet();
            String GetTick = "";
            if (Request.QueryString["ET"] != null)
            {
                GetTick = Request.QueryString["ET"].ToString();
            }

            Decoder.ItineraryURL = Cred.ItineraryURL;
            int i = 0;
            string Location = Server.MapPath("temp");
            if (!Location.EndsWith(@"\")) Location += @"\";

            if (m_Etickets.Count > 1)
            {
                ArrayList MyTickets = Decoder.FormatMultipleEticketRecovery(m_Etickets, Location, Session.SessionID, m_Surname);

                if (MyTickets == null || MyTickets.Count == 0)
                {
                    //DropDownList1.Visible = false;  //Need to Work
                    //Submit1.Visible = false;
                    //etLabel.Visible = false;
                    //outError.InnerHtml = msg;
                    return;
                }
                else if (MyTickets.Count == 1)
                {
                    // go direct
                    Travelbytes.WebService.Itinerary.LocalEticketRetrieval str =
                        (Travelbytes.WebService.Itinerary.LocalEticketRetrieval)MyTickets[0];
                    Travelbytes.WebService.Itinerary.LocalEticketRetrieval mytkt = (Travelbytes.WebService.Itinerary.LocalEticketRetrieval)MyTickets[0];
                    Tranx Trans1 = new Tranx();
                    Trans1.EticketRetrieve(this.m_RLOC, m_Surname, mytkt.Eticket.AirV + " " + mytkt.Eticket.FirstTkNum);

                    Response.Redirect("temp/" + str.FileName, true);
                }

                int TKTCount = 0;
                foreach (Travelbytes.WebService.Itinerary.LocalEticketRetrieval str in MyTickets)
                {
                    if (GetTick.Length > 10)
                    {
                        if (str.FileName.IndexOf(GetTick) != -1)
                        {
                            Response.Redirect("temp/" + str.FileName, true);
                        }
                    }
                    {
                    }
                    TKTCount++;
                    if (str.Eticket != null && str.Status.Length == 0)
                    {
                        //ListItem item = new ListItem(str.Eticket.AirV + " " + str.Eticket.FirstTkNum, "temp/" + str.FileName);
                        //DropDownList1.Items.Add(item);
                    }
                    else
                    {
                        //ListItem item = new ListItem(string.Format("Ticket {0}", TKTCount), "temp/" + str.FileName);
                        //DropDownList1.Items.Add(item);
                    }
                }
            }
            else
            {

                // for a single eticket
                foreach (Travelbytes.ClassMap.ETicketNum T in m_Etickets)
                {

                    string Name = Session.SessionID + i.ToString() + ".xml";
                    if (Decoder.GetTicket(T.AirV + T.FirstTkNum, Name, Server.MapPath("temp")))
                    {
                        this.m_TicketNumber = T.AirV + T.FirstTkNum;
                        m_URLS.Add("temp/" + Name);
                    }
                }

                //Decoder.EndSession();

                if (m_URLS.Count == 1)
                {
                    Tranx Trans1 = new Tranx();
                    Trans1.EticketRetrieve(this.m_RLOC, m_Surname, m_TicketNumber);
                    Response.Redirect(m_URLS[0].ToString(), true);
                }
            }


        }
        public int LoadItinerary()
        {
            try
            {
                Travelbytes.WebService.Itinerary.Decoder Decoder = null;
                int Retry = 0;

                Decoder = new Travelbytes.WebService.Itinerary.Decoder();
                Decoder.Reload = false;
                Decoder.Print = false;
                Decoder.TempLocation = Server.MapPath("Temp");
                Decoder.Style = Server.MapPath("styles/");
                Decoder.Config = Server.MapPath("");
                Decoder.Templates = Server.MapPath("Templates/");
                Decoder.AddressDSN = GetAddressDataSourceMySQL();
                Decoder.PassedCountry = m_GDS;
                Credentials Cred = GetUser("Galileo");
                Decoder.User = Cred.User;
                Decoder.PSW = Cred.PSW;
                Decoder.HAP = Cred.HAP;
                Decoder.URL = Cred.URL;
                Decoder.ItineraryURL = Cred.ItineraryURL;
                Decoder.Offset = GetOffSet();

                Tranx Tran = new Tranx();
                Tran.ItineraryRetrieve(m_RLOC, m_Surname, "");

                string str = Decoder.GetItinerary(m_RLOC);

                Session["SMSSummary"] = Decoder.SMSSummary;

                if ((base.HasEticket = Decoder.HasTickets) == true)
                {
                    Session["Etickets"] = Decoder.TicketNumbers;
                }

                if (str.Length == 0)
                {
                    if (Decoder.LastErrorNumber != 0)
                    {
                        //Response.Redirect("Error.aspx?E=" + Decoder.LastErrorNumber);
                        m_error_number = Decoder.LastErrorNumber;
                        return 1;
                    }
                    else if (str.IndexOf("Invalid reference") != -1)
                        //Response.Redirect("Error.aspx?E=7");                        
                        return 2;
                    else
                        //Response.Redirect("Error.aspx?E=0&M=" + Server.UrlEncode(Decoder.LastError));
                        m_error = Server.UrlEncode(Decoder.LastError);
                    return 3;
                }

                // check the surname matches the one provided
                if (Decoder.Surname.ToUpper().IndexOf(m_Surname.ToUpper()) == -1)
                {

                    if (Session["RetryCount"] == null)
                        Retry = 0;
                    else
                        Retry = ((int)Session["RetryCount"]) + 1;

                    if (Retry >= GetRetryCount())
                    {
                        Session["RetryCount"] = GetRetryCount();
                        m_RLOC = "";
                        //Response.Redirect("error.aspx?E=5");
                        //Response.End();
                        return 4;
                    }
                    Session["RetryCount"] = Retry;
                    // failed to match
                    //Response.Redirect("Error.aspx?E=2");
                    //Response.End();
                    return 5;

                }

                Session["RetryCount"] = 0;
                GetReference = m_RLOC;
                GetName = Decoder.Pax;
                Session["RLOC"] = m_RLOC;
                Session["Name"] = Decoder.PaxName;
                Session["Decoder"] = Decoder;
                return 0;
            }
            catch { }

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="RLOC"></param>
        /// <returns></returns>
        public bool FindEticketRecords(string RLOC)
        {
            bool rv = false;
            String GetTick = "";
            if (Request.QueryString["ET"] != null)
            {
                GetTick = Request.QueryString["ET"].ToString();
            }
            try
            {
                string strFormName = "";
                if (m_GDS == "AU")
                {
                    strFormName = "AXQIETICKETAUD";
                }
                else if (m_GDS == "SG")
                {
                    strFormName = "AXQIETICKET";
                }
                else if (m_GDS == "IN")
                {
                    strFormName = "AXQIETICKETINR";
                }

                DataSet m_ProcessPNRsDS = new DataSet();
                MailRecordsBAL objMailRecordsBAL = new MailRecordsBAL("");
                List<MailRecordsModel> lstMailRecordsModel = objMailRecordsBAL.FindEticketRecords(RLOC, GetTick, strFormName);
                DataTable dt = Common.ListToDataTable(lstMailRecordsModel);
                m_ProcessPNRsDS.Tables.Add(dt);

                // check if the record exist already
                //if (m_ProcessPNRsDS.Tables != null && m_ProcessPNRsDS.Tables.Count != 0 && m_ProcessPNRsDS.Tables[0].Rows != null)
                //    return rv;

                string Created = "";
                string Subject = "";
                string LastSubject = "LastSubject";

                foreach (DataRow myRow in m_ProcessPNRsDS.Tables[0].Rows)
                {
                    string FormName = myRow["FormName"].ToString();

                    if (myRow["Created"].ToString() == Created && LastSubject == Subject)
                    {
                        // must be a copy record
                        continue;
                    }

                    Subject = myRow["Subject"].ToString();
                    if (FormName.ToUpper().IndexOf("QIETICKET") != -1)
                    {
                        Created = myRow["Created"].ToString();
                        LastSubject = Subject;

                        // look for matching surname
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

                        string newName = Names[0].Trim();

                        if (newName == m_Surname)
                        {
                            if (myRow["FileName"] == null || myRow["FileName"].ToString().Length == 0)
                                continue;

                            string FileName = myRow["FileName"].ToString().ToUpper();
                            // now retrieve the doc and place it in the temp folder.
                            if (GetTransaction(ref FileName, myRow["TicketNumber"].ToString()))
                            {
                                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                dictionary.Add("c:/Temp/" + Path.GetFileName(FileName), myRow["Subject"].ToString());

                                var selectList = new SelectList(dictionary, "Key", "Value");
                                ViewData["ETicket"] = selectList;

                                //ListItem item = new ListItem(myRow["Subject"].ToString() + " ", "temp/" + Path.GetFileName(FileName));  //Need To Work
                                //DropDownList1.Items.Add(item);                                
                                if (GetTick.Length > 0)
                                {
                                    Session["output"] = "c:/Temp/" + Path.GetFileName(FileName);
                                    return true;
                                    //Response.Redirect("temp/" + Path.GetFileName(FileName), true);
                                }
                            }
                            rv = true;
                        }
                    }
                }

                m_ProcessPNRsDS.Dispose();
                GC.Collect();
                return rv;

            }
            catch (InvalidOperationException ioExp)
            {
                m_LastError += "InvalidOperationException in FindMailrecords " + m_RLOC + " : " + ioExp.Message + "\r\n";
                System.Diagnostics.Trace.WriteLine(ioExp.Message);
            }
            catch (MySqlException oleExp)
            {
                m_LastError += "MySqlException in FindMailrecords " + m_RLOC + " : " + oleExp.Message + "\r\n";
                System.Diagnostics.Trace.WriteLine(oleExp.Message);
            }
            catch (Exception e)
            {
                m_LastError += "Exception in FindMailrecords " + m_RLOC + " : " + e.Message + "\r\n";
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return rv;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="TicketNumber"></param>
        /// <returns></returns>
        protected bool GetTransaction(ref string FileName, string TicketNumber)
        {
            bool rv = false;
            // Get the file
            string myLocation = "c:\\Temp" + "\\" + TicketNumber.Replace(" ", "") + ".htm"; //Server.MapPath("Temp") + "\\" + TicketNumber.Replace(" ", "") + ".htm";
            logger.Debug("Retrieving {0}", FileName);
            rv = CreateDisplayFile(FileName, ref myLocation);

            if (rv)
                FileName = myLocation;

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="storeFile"></param>
        /// <returns></returns>
        public bool CreateDisplayFile(string FileName, ref string storeFile)
        {
            try
            {
                if (GetDebug() && GetAltDebug())
                {
                    FileName = FileName.ToLower().Replace(@"203.19.215.74\itineraries\", @"203.19.215.74\cdrive\travelbytes\itineraries\");
                }

                string buffer = ReadFileFromDisk(FileName);
                if (buffer.StartsWith("%PDF"))
                {
                    try
                    {
                        // we have a pdf file to display
                        storeFile = storeFile.Replace(Path.GetExtension(storeFile), ".pdf");
                        //using (new Impersonator("woz", "", "2rachelle"))
                        //{
                            System.IO.File.Copy(FileName, storeFile);
                        
                        //}
                        
                    }
                    catch { }
                }
                else
                {
                    logger.Debug("Fixing HTML");
                    string bufferfixed = FixHTML(buffer);
                    WriteFileTo(storeFile, bufferfixed);
                }
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
                //{
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        // use the BCL to load the part in a byte[] buffer
                        byte[] bufferWorkbookPart = new BinaryReader(fs).ReadBytes((int)fs.Length);

                        // use our BIFF12 reader
                        // ByteConverter con = new ByteConverter();
                        Data = System.Text.Encoding.GetEncoding("windows-1252").GetString(bufferWorkbookPart);
                    }
                //}
            }
            catch (FileNotFoundException e)
            {
                logger.Error("ReadFileFromDisk FNF Exception: " + e.Message);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                logger.Error("ReadFileFromDisk Exception: " + e.Message);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            return Data;
        }

        /// <summary>
        /// WriteFileTo
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool WriteFileTo(string FileName, string Data)
        {
            bool rv = false;
            StreamWriter SwFromFile;
            try
            {
                SwFromFile = new StreamWriter(FileName);
                SwFromFile.Write(Data);
                SwFromFile.Close();
                rv = true;
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return rv;
        }

        /// <summary>
        /// FixHTML
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
                IHTMLElementCollection allInputs = (IHTMLElementCollection)allElements.tags("img");
                IHTMLElementCollection allStyles = (IHTMLElementCollection)htmlDocument.all.tags("style");
                IHTMLElementCollection allhead = (IHTMLElementCollection)htmlDocument.all.tags("head");
                string MyDoc = "<html>";

                foreach (IHTMLElement HeadElement in allhead)
                {
                    MyDoc += HeadElement.outerHTML;
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
                        else
                        {
                            URL += u;
                        }

                        URL += GetWebDirectory() + src.Substring(Pos + 1);
                        element.setAttribute("src", URL, 0);
                    }
                }

                //::......... Return the parent element content ( BODY > HTML )
                MyDoc += htmlDocument.body.outerHTML;

                MyDoc += "</html>";
                return MyDoc;
            }
            catch (Exception e)
            {
                logger.Error("Exception in HTML fix: " + e.Message);
            }
            return htmlToParse;
        }

        #endregion

    }
}
