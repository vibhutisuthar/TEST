using System;
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
using System.Data;
using System.Xml.XPath;
using System.Text;
using System.Xml.Xsl;
using System.Web.Configuration;
using System.Xml;
using MySql.Data.MySqlClient;


namespace MTP_JAPA.Controllers
{
    public class QiLiveItineraryController : BaseController
    {
        #region Variables

        string RLOC = "";
        string Surname = "";
        string strErrorMsg = "Failed to retrieve booking due to some error.";
        string strUnableToLocate = "Unable to locate – Please check details and re-enter";

        #endregion

        #region Load

        /// <summary>
        /// ItineraryView
        /// </summary>
        /// <returns></returns>
        public ActionResult QiLiveItineraryView()
        {
            //bPrint = false;            
            //ViewBag.bPrint = bPrint;
            ViewBag.TabDisplay = 1;
            ViewBag.theGoClick = 1;
            Session["theGoClick"] = 1;

            if (Request.QueryString["R"] != null)
            {
                RLOC = Request.QueryString["R"].ToString().ToUpper();
            }
            if (Request.QueryString["S"] != null)
            {
                Surname = Request.QueryString["S"].ToString().ToUpper();
            }
            if (Surname != "" && RLOC != "")
            {
                DynamicHeaderElements();
            }
            else
            {
                ViewBag.msgDisplay = strUnableToLocate;
            }
            //TEmp-------------------------
            m_RLOC = RLOC;
            m_Surname = Surname;

            //TEmp-------------------------

            return View();
        }

        /// <summary>
        /// DynamicHeaderElements
        /// </summary>
        /// <returns></returns>
        public ActionResult DynamicHeaderElements()
        {
            try
            {
                string utid_id = "";
                if (Session["QiLiveItineraryData"] != null)
                {
                    List<XmlBookingModel> lstQiLiveItineraryData = (List<XmlBookingModel>)(Session["QiLiveItineraryData"]);  //objItineraryDataBAL.SearchItineraryDataBySurnameAndRLOC(Surname, RLOC);
                    if (lstQiLiveItineraryData != null)
                    {
                        DataTable dtQiLiveItineraryData = Common.ListToDataTable(lstQiLiveItineraryData);
                        if (dtQiLiveItineraryData != null)
                        {
                            if (dtQiLiveItineraryData.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtQiLiveItineraryData.Rows) // Loop over the rows.
                                {
                                    utid_id = Convert.ToString(row["utid_id"]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.msgDisplay = strUnableToLocate;
                    return View();
                }

                XmlDocument xd = GetUtidXML(utid_id);

                string xslFile = WebConfigurationManager.AppSettings["XslFilePath"];
                //get the Transcode element (xslt file)
                if (xd.GetElementsByTagName("transcode")[0] != null)
                {
                    XmlNodeList transcodeitems = xd.GetElementsByTagName("transcode");
                    xslFile = transcodeitems[0].Attributes["xslt_file"].Value;
                }
                else
                    Response.Write("Warning XML Transcode element not found! (defaulting to qitest.xsl)");


                //Set Header Elements (from XML)
                string headerTest = "";
                XmlNodeList xl = xd.GetElementsByTagName("headerItem");
                foreach (XmlNode xn in xl)
                {
                    //if (!headerTest.Contains(xn.Attributes["href"].Value))
                    //{
                    //    if (xn.Attributes["type"].Value == "text/javascript")
                    //        headerTest += string.Format("   <script type=\"{0}\" src=\"{1}\"></script> \r\n", xn.Attributes["type"].Value, xn.Attributes["href"].Value);
                    //    else
                    //        headerTest += string.Format("   <link type=\"{0}\" href=\"{1}\" rel=\"{2}\" media=\"{3}\"></link> \r\n", xn.Attributes["type"].Value, xn.Attributes["href"].Value, xn.Attributes["rel"].Value, xn.Attributes["media"].Value);
                    //}
                    if (xn.Attributes["type"].Value == "text/javascript")
                        headerTest += string.Format("   <script type=\"{0}\" src=\"{1}\"></script> \r\n", xn.Attributes["type"].Value, xn.Attributes["href"].Value);
                    else
                        headerTest += string.Format("   <link type=\"{0}\" href=\"{1}\" rel=\"{2}\" media=\"{3}\"></link> \r\n", xn.Attributes["type"].Value, xn.Attributes["href"].Value, xn.Attributes["rel"].Value, xn.Attributes["media"].Value);
                }
                //headerTest = headerTest.Replace("text/javascript	", "text/javascript").Replace("></link>", "/>")
                //    .Replace("<link type=\"text/javascript\" href=\"http://qionlinetest1.travelbytes.com.au/Content/static/js/libs/jquery-1.6.2.min.js\" rel=\"\" media=\"\"/>", "<script type=\"text/javascript\" src=\"http://qionlinetest1.travelbytes.com.au/Content/static/js/libs/jquery-1.6.2.min.js\"></script>");

                //headerTest = headerTest.Replace("text/javascript	", "text/javascript").Replace("></link>", "/>")
                //    .Replace("<link type=\"text/javascript\" href=\"http://qionlinetest1.travelbytes.com.au/Content/static/js/libs/jquery-1.6.2.min.js\" rel=\"\" media=\"\"/>", "<script type=\"text/javascript\" src=\"http://qionlinetest1.travelbytes.com.au/Content/static/js/libs/jquery-1.6.2.min.js\"></script>");


                ViewData["HeaderData"] = new MvcHtmlString(headerTest);
                ViewData["ItinerayHTML"] = RunXSLTransform(xslFile, xd);

            }
            catch (Exception ex)
            {
                ViewBag.msgDisplay = strErrorMsg;
                return View();
            }
            return View();
        }

        #endregion

        #region "Helper Methods"

        static string MySQLConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["qilive"].ConnectionString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xslFile"></param>
        /// <param name="xd"></param>
        /// <returns></returns>
        private MvcHtmlString RunXSLTransform(string xslFile, XmlDocument xd)
        {
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utid_id"></param>
        /// <returns></returns>
        private XmlDocument GetUtidXML(string utid_id)
        {
            XmlDocument xd = new XmlDocument();
            MySqlConnection mycon = new MySqlConnection(MySQLConnectionString);
            string cmdText = string.Format(@"SELECT 
                                                b.`id`,
                                                b.`utid_id`,
                                                b.`entry_date`,
                                                b.`stamp`,
                                                b.`phase`,
                                                b.`xml`,
                                                b.`notes`,
                                                u.`status`
                                            FROM `xml_booking` b
                                            inner join `utid` u
                                                on u.id = b.utid_id
                                            WHERE b.utid_id='{0}';", utid_id);
            try
            {
                mycon.Open();
                MySqlCommand cmd = new MySqlCommand(cmdText, mycon);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    xd.LoadXml(reader["xml"].ToString());
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                mycon.Close();
            }
            return xd;
        }

        #endregion

        #region "Class"

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <param name="RLOC"></param>
        /// <param name="Surname"></param>
        /// <returns></returns>
        //bool SearchItineraryDataBySurnameAndRLOC(string Surname, string RLOC, string strXML)
        //{
        //    try
        //    {
        //        //string strXML = "";
        //        //ItineraryDataBAL objItineraryDataBAL = new ItineraryDataBAL();
        //        //if (Session["QiLiveItineraryData"] != null)
        //        //{
        //        //    List<XmlBookingModel> lstQiLiveItineraryData = (List<XmlBookingModel>)(Session["QiLiveItineraryData"]);  //objItineraryDataBAL.SearchItineraryDataBySurnameAndRLOC(Surname, RLOC);
        //        //    if (lstQiLiveItineraryData != null)
        //        //    {
        //        //        DataTable dtQiLiveItineraryData = Common.ListToDataTable(lstQiLiveItineraryData);
        //        //        if (dtQiLiveItineraryData != null)
        //        //        {
        //        //            if (dtQiLiveItineraryData.Rows.Count > 0)
        //        //            {
        //        //                foreach (DataRow row in dtQiLiveItineraryData.Rows) // Loop over the rows.
        //        //                {
        //        //                    strXML = Convert.ToString(row["xml"]);
        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}

        //        if (strXML != "")
        //        {
        //            //string xslFilePath = @"\Content\XMLtoHTML5\QItest.xsl";
        //            string xslFilePath = WebConfigurationManager.AppSettings["XslFilePath"];
        //            string xsltPath = xslFilePath; // Server.MapPath(xslFilePath); 
        //            string strHTML = Common.GetHtml(xsltPath, strXML);
        //            if (strHTML != "")
        //            {
        //                ViewBag.QiLiveItinContent = strHTML;
        //                ViewData["ItinerayHTML"] = strHTML;
        //            }
        //            else
        //            {
        //                ViewBag.msgDisplay = strErrorMsg;
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.msgDisplay = strUnableToLocate;
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.msgDisplay = strErrorMsg;
        //        return false;
        //    }
        //}

        //public class BinaryContentResult : ActionResult
        //{
        //    private string ContentType;
        //    private byte[] ContentBytes;
        //    public BinaryContentResult(byte[] contentBytes, string contentType)
        //    {
        //        this.ContentBytes = contentBytes;
        //        this.ContentType = contentType;
        //    }
        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var response = context.HttpContext.Response;
        //        response.Clear();
        //        response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        response.ContentType = this.ContentType;

        //        var stream = new MemoryStream(this.ContentBytes);
        //        stream.WriteTo(response.OutputStream);
        //        stream.Dispose();
        //    }
        //}

        #endregion

        #region #DanCode

        //public ActionResult DynamicXSLT()
        //{
        //    string xslFile = Server.MapPath(string.Format("/Content/xmltohtml5/{0}.xsl", Request.QueryString["xslt"].ToString())); //Server.MapPath("/Content/xmltohtml5/QItest.xsl");
        //    XmlDocument xd = GetUtidXML(Request.QueryString.ToString());
        //    MvcHtmlString mvcHtml = RunXSLTransform(xslFile, xd);
        //    ViewData["ItinerayHTML"] = mvcHtml;
        //    return View();
        //}

        //public ActionResult DynamicHeaderElements()
        //{

        //    bool debugMode = false;
        //    string utid_id = Request.QueryString.ToString();
        //    if (utid_id.Contains("&"))
        //    {
        //        if (utid_id.ToLower().Contains("debugmode=true"))
        //            debugMode = true;
        //        utid_id = utid_id.Remove(Request.QueryString.ToString().IndexOf("&"));

        //    }

        //    XmlDocument xd = GetUtidXML(utid_id);

        //    string xslFile = Server.MapPath("/Content/xmltohtml5/qitest.xsl");

        //    //get the Transcode element (xslt file)
        //    if (xd.GetElementsByTagName("transcode")[0] != null)
        //    {
        //        XmlNodeList transcodeitems = xd.GetElementsByTagName("transcode");
        //        xslFile = transcodeitems[0].Attributes["xslt_file"].Value;
        //    }
        //    else
        //        Response.Write("Warning XML Transcode element not found! (defaulting to qitest.xsl)");

        //    //Set Header Elements (from XML)
        //    string headerTest = "";
        //    XmlNodeList xl = xd.GetElementsByTagName("headerItem");
        //    foreach (XmlNode xn in xl)
        //    {
        //        if (xn.Attributes["type"].Value == "text/javascript")
        //            headerTest += string.Format("   <script type=\"{0}\" src=\"{1}\"></script> \r\n", xn.Attributes["type"].Value, xn.Attributes["href"].Value);
        //        else
        //            headerTest += string.Format("   <link type=\"{0}\" href=\"{1}\" rel=\"{2}\" media=\"{3}\"></link> \r\n", xn.Attributes["type"].Value, xn.Attributes["href"].Value, xn.Attributes["rel"].Value, xn.Attributes["media"].Value);
        //    }
        //    ViewData["HeaderData"] = new MvcHtmlString(headerTest);

        //    //Set Body (from XSLT)
        //    ViewData["ItinerayHTML"] = RunXSLTransform(xslFile, xd);
        //    return View();

        //}
        #endregion

    }
}
