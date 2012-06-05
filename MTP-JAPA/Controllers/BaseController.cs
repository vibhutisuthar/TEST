using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using NLog.Config;
using System.Xml;
using System.Web.Configuration;

namespace MTP_JAPA.Controllers
{
    public class BaseController : Controller
    {
        #region Variables

        public string m_LastError = "";
        public string m_Surname = "";
        public string m_RLOC = "";
        public string m_InvoiceNumber = "";
        public string m_EticketNumber = "";
        public string m_GDS = "";
        public string m_Location = "";
        public string m_Type = "";
        public static Logger logger = LogManager.GetLogger("TravelPlans");
        public bool bPrint = false;

        protected string[] Months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        protected enum eMonths
        {
            Jan,
            Feb,
            Mar,
            Apr,
            May,
            Jun,
            Jul,
            Aug,
            Sep,
            Oct,
            Nov,
            Dec
        }

        private const string UNHANDLED_EXCEPTION = "Unhandled Exception:";
        //
        // Session Key Constants
        //
        private const string KEY_CACHECART = "Cache:ShoppingCart:";
        private const string KEY_CACHECUSTOMER = "Cache:Customer:";
        // 
        // SSL
        //
        private static string pageSecureUrlBase = "https://";
        private static string pageUrlBase = "/QiOnline";
        //private static string urlSuffix = "http://";

        string m_UserFullName = "";
        string m_UserFullNameThread = "";
        bool m_Administrator = false;
        bool m_Etickets = false;
        public string m_Name = "";
        public string m_Reference = "";

        #endregion

        #region Properties

        public bool HasEticket
        {
            get
            {
                return m_Etickets;
            }
            set
            {
                m_Etickets = value;
            }
        }

        public string GetName
        {
            set
            {
                m_Name = value;
            }
            get
            {
                return m_Name;
            }
        }
        public string GetReference
        {
            set
            {
                m_Reference = value;
            }
            get
            {
                return m_Reference;
            }
        }


        #endregion

        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void GetQueryVars()
        {
            if (Request.QueryString["T"] != null)
            {                
                m_Type = Request.QueryString["T"].ToString().ToUpper();
            }
            if (Request.QueryString["R"] != null)
            {
                m_RLOC = Request.QueryString["R"].ToString().ToUpper();
            }
            if (Request.QueryString["S"] != null)
            {
                m_Surname = Request.QueryString["S"].ToString().ToUpper();
            }
            if (Request.QueryString["G"] != null)
            {
                m_GDS = Request.QueryString["G"].ToString().ToUpper();
            }
            if (Request.QueryString["M"] != null)
            {
                m_Location = Request.QueryString["M"].ToString();
            }
            if (Request.QueryString["I"] != null)
            {
                m_InvoiceNumber = Request.QueryString["I"].ToString().ToUpper();
            }
            if (Request.QueryString["ET"] != null)
            {
                m_EticketNumber = Request.QueryString["Et"].ToString().ToUpper();
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
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetLiveCountryList()
        {
            string rv = "";
            try
            {
                string Search = WebConfigurationManager.AppSettings["LiveCountryList"];

                if (Search == null)
                    return rv;

                return Search.ToUpper();
            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int GetSeachMonths()
        {
            int rv = -6;
            try
            {
                string Months = WebConfigurationManager.AppSettings["SearchMonths"];

                if (Months == null)
                    return rv;

                return int.Parse(Months);
            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetItinerarySearch()
        {
            string rv = "DOCUMENTS";
            try
            {
                string Search = WebConfigurationManager.AppSettings["ItinerarySearch"];

                if (Search == null)
                    return rv;

                return Search.ToUpper();
            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetItineraryProcessorNames()
        {
            string rv = "";
            try
            {
                string Search = WebConfigurationManager.AppSettings["LiveQIProcessorNames"];

                if (Search == null)
                    return rv;

                return Search.ToUpper();
            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetNameCommand()
        {
            string NameHeader = WebConfigurationManager.AppSettings["NameHeader"];
            string NameCommand = WebConfigurationManager.AppSettings["NameDetail"];
            string NameTrash = WebConfigurationManager.AppSettings["NameTrash"];

            if (NameHeader == null)
                NameHeader = @"((ITINERARY FOR )|(STORED )|(ITINÉRAIRE DE ))*";

            if (NameCommand == null)
                NameCommand = @"[A-Z ]*[/_][A-Z]*";

            if (NameTrash == null)
                NameTrash = @"[A-Z0-9 /]*";
            //return @"(?<header>((ITINERARY FOR )|(STORED ))*)(?<detail> [A-Z]*[/_][A-Z]*)(?<trash>[A-Z0-9 /]*)";

            string NameRegex = string.Format("(?<header>{0})(?<detail>{1})(?<trash>{2})", NameHeader, NameCommand, NameTrash);

            return NameRegex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool GetDebug()
        {
            bool rv = false;
            try
            {
                string Search = WebConfigurationManager.AppSettings["Debug"];

                if (Search == null)
                    return rv;

                if (Search.ToUpper() == "YES")
                    return true;

            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool GetAltDebug()
        {
            bool rv = false;
            try
            {
                string Search = WebConfigurationManager.AppSettings["AltDebug"];

                if (Search == null)
                    return rv;

                if (Search.ToUpper() == "YES")
                    return true;

            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int GetRetryCount()
        {
            int rv = 5;
            try
            {
                string TimeOut = WebConfigurationManager.AppSettings["RetryCount"];

                if (TimeOut == null)
                    return rv;

                return int.Parse(TimeOut);
            }
            catch
            { }

            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetWebDirectory()
        {
            string CustomRoles = WebConfigurationManager.AppSettings["WebDirectory"];

            if (CustomRoles == null)
                return "";

            if (!CustomRoles.EndsWith("/"))
                CustomRoles += "/";


            return CustomRoles;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetSecurePDF()
        {
            string SecurePDF = WebConfigurationManager.AppSettings["SecurePDF"];

            if (SecurePDF == null)
                return "http://";

            return SecurePDF;

        }

        /// <summary>
        /// GetImageName
        /// </summary>
        /// <param name="FormName"></param>
        /// <returns></returns>
        protected string GetImageName(string FormName)
        {
            string FileName = WebConfigurationManager.AppSettings[FormName];

            if (FileName == null)
                return "";

            return FileName;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAddressDataSourceMySQL()
        {
            string DSN = WebConfigurationManager.AppSettings["AddressDataSourceMySQL"];
            string dbusername = WebConfigurationManager.AppSettings["dbusername"];
            string edbpassword = WebConfigurationManager.AppSettings["dbpassword"];

            try
            {

                if (DSN.IndexOf("{0}") != -1)
                {
                    DSN = string.Format(DSN, dbusername, CustomEncryption.Decrypt(edbpassword));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return "";
            }

            return DSN;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CRS"></param>
        /// <returns></returns>
        public Credentials GetUser(string CRS)
        {
            if (CRS.ToUpper() == "GALILEO")
            {
                Credentials Cred = new Credentials();

                try
                {
                    string GalileoUserName = WebConfigurationManager.AppSettings["galileousername"];

                    string Test = "Amext80";
                    string en = CustomEncryption.Encrypt(Test);
                    string edbpassword = WebConfigurationManager.AppSettings["galileopassword"];


                    Cred.PSW = CustomEncryption.Decrypt(edbpassword);
                    Cred.User = GalileoUserName;
                    Cred.CRS = CRS;
                    Cred.URL = WebConfigurationManager.AppSettings["galileoURL"];
                    Cred.ItineraryURL = WebConfigurationManager.AppSettings["galileoItineraryURL"];
                    Cred.HAP = WebConfigurationManager.AppSettings["galileoHostAccessProfile"];
                    return Cred;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }
            else if (CRS.ToUpper() == "DIABLO")
            {
                Credentials Cred = new Credentials();

                try
                {
                    string GalileoUserName = WebConfigurationManager.AppSettings["diablousername"];
                    string edbpassword = WebConfigurationManager.AppSettings["diablopassword"];

                    Cred.PSW = CustomEncryption.Decrypt(edbpassword);
                    Cred.User = GalileoUserName;
                    Cred.CRS = CRS;
                    Cred.URL = ".";
                    Cred.ItineraryURL = "";
                    Cred.HAP = "";
                    return Cred;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetOffSet()
        {
            string rv = "0";

            try
            {
                HttpCookie GDS = HttpContext.Request.Cookies["hoursDiffGMTTime"];
                if (GDS != null)
                    rv = GDS.Value.ToString();
            }
            catch { }

            return rv;

        }

        /// <summary>
        /// FileDecrypt
        /// </summary>
        /// <param name="InFile"></param>
        /// <param name="OutFile"></param>
        /// <returns></returns>
        protected int FileDecrypt(string InFile, string OutFile)
        {
            m_LastError = "";
            WOZCRYPTOLib.IFile Crypto = new WOZCRYPTOLib.FileClass();

            Crypto.InFileName = InFile;
            Crypto.OutFileName = OutFile;

            int Status = Crypto.Decrypt();

            m_LastError = Crypto.LastError;

            if (Status != 1)
                m_LastError = "Error decrypting file";

            return Status;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetInvoiceStore()
        {

            string Store = WebConfigurationManager.AppSettings["InvoiceStore"];

            if (!Store.EndsWith("\\"))
                Store += "\\";

            return Store;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetItineraryStore()
        {

            string Store = WebConfigurationManager.AppSettings["ItineraryStore"];

            if (!Store.EndsWith("\\"))
                Store += "\\";

            return Store;
        }

        /// <summary>
        /// Responsible for gettting the user IP address (including proxy address list)
        /// </summary>
        /// <returns>
        /// IP address string
        /// </returns>
        public string GetIPAddress()
        {
            string ReturnValue = "";
            string TmpUserIP = Request.UserHostAddress;
            string TmpProxies = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? "" : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();

            ReturnValue = TmpUserIP;
            if (TmpProxies.Length > 0)
            {
                ReturnValue += ",";
                ReturnValue += TmpProxies;
            }
            return ReturnValue;
        }


        #endregion
    }
}
