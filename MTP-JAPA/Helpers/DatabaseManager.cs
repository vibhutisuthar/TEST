using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;

namespace ChwLib
{
    public class DatabaseManager
    {
        /// <summary>
        /// Error members
        /// </summary>
        public string ErrorDescription;

        /// <summary>
        /// Connection objects
        /// </summary>
        private MySql.Data.MySqlClient.MySqlConnection m_Connection;
        private string m_ConnectionString;

        /// <summary>
        /// Responsible for initializing class
        /// </summary>
        public DatabaseManager()
        {
            // Init error members
            ErrorDescription = "";

            // Init connection members
            //PageBase TmpPage = new PageBase();            
            m_ConnectionString = GetDataSourceMySQL();
            m_Connection = new MySql.Data.MySqlClient.MySqlConnection();
            m_Connection.ConnectionString = m_ConnectionString;
        }

        /// <summary>
        /// GetDataSourceMySQL
        /// </summary>
        /// <returns></returns>
        public string GetDataSourceMySQL()
        {
            string DSN = WebConfigurationManager.AppSettings["DataSourceMySQL"];
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
        /// Responsible for cleaning up the class
        /// </summary>
        public void Dispose()
        {
            if (null != m_Connection)
            {
                m_Connection.Close();
                m_Connection.Dispose();
                m_Connection = null;
            }
        }

        /// <summary>
        /// Responsible for inserting a record to the mailrecord table
        /// </summary>
        /// <param name="Rloc">
        /// record locator value
        /// </param>
        /// <param name="Created">
        /// create timestamp
        /// </param>
        /// <param name="IPAdress">
        /// user ip address
        /// </param>
        /// <param name="ProcessorName">
        /// processor name value
        /// </param>
        /// <param name="FormatName">
        /// format name value
        /// </param>
        /// <returns>
        /// true if successful, false otherwise
        /// </returns>
        public bool addMailRecord(string Rloc,
            System.DateTime Created,
            string FromName,
            string ProcessorName,
            string FormName,
            string Subject,
            string Team,
            string Company
            )
        {
            bool ReturnValue = false;
            MySql.Data.MySqlClient.MySqlCommand TmpInsertCommand =
                new MySql.Data.MySqlClient.MySqlCommand("CompositionAdd", m_Connection);
            string TmpInsertSql;

            ///////////////////////
            // Init error description
            ErrorDescription = "";

            ///////////////////////
            // Init parameters
            TmpInsertSql = "INSERT INTO `MAILRECORDS` (`RLOC`,`CREATED`,`FROMNAME`,`PROCESSORNAME`,`FORMNAME`,`SUBJECT`,`TEAM`,`COMPANY`) VALUE (?RLOC,?CREATED,?FROMNAME,?PROCESSORNAME,?FORMNAME,?SUBJECT,?TEAM,?COMPANY); ";

            // RLOC / PNR
            MySql.Data.MySqlClient.MySqlParameter pRLoc =
                new MySql.Data.MySqlClient.MySqlParameter("?RLOC", MySql.Data.MySqlClient.MySqlDbType.VarChar, 7);
            pRLoc.Value = (Rloc != null) ? (object)Rloc : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pRLoc);

            // Created
            MySql.Data.MySqlClient.MySqlParameter pCreated =
                new MySql.Data.MySqlClient.MySqlParameter("?CREATED", MySql.Data.MySqlClient.MySqlDbType.Datetime);
            pCreated.Value = (Created != null) ? (object)Created : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pCreated);

            // FromName / To/From
            MySql.Data.MySqlClient.MySqlParameter pFromName =
                new MySql.Data.MySqlClient.MySqlParameter("?FROMNAME", MySql.Data.MySqlClient.MySqlDbType.VarChar, 150);
            pFromName.Value = (FromName != null) ? (object)FromName : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pFromName);

            // ProcessorName
            MySql.Data.MySqlClient.MySqlParameter pProcessorName =
                new MySql.Data.MySqlClient.MySqlParameter("?PROCESSORNAME", MySql.Data.MySqlClient.MySqlDbType.VarChar, 30);
            pProcessorName.Value = (ProcessorName != null) ? (object)ProcessorName : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pProcessorName);

            // FormName / Format Name
            MySql.Data.MySqlClient.MySqlParameter pFormName =
                new MySql.Data.MySqlClient.MySqlParameter("?FORMNAME", MySql.Data.MySqlClient.MySqlDbType.VarChar, 65);
            pFormName.Value = (FormName != null) ? (object)FormName : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pFormName);

            // Subject
            MySql.Data.MySqlClient.MySqlParameter pSubject =
                new MySql.Data.MySqlClient.MySqlParameter("?SUBJECT", MySql.Data.MySqlClient.MySqlDbType.VarChar, 500);
            pSubject.Value = (Subject != null) ? (object)Subject : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pSubject);

            // Team
            MySql.Data.MySqlClient.MySqlParameter pTeam =
                new MySql.Data.MySqlClient.MySqlParameter("?TEAM", MySql.Data.MySqlClient.MySqlDbType.VarChar, 65);
            pTeam.Value = (Team != null) ? (object)Team : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pTeam);

            // Company
            MySql.Data.MySqlClient.MySqlParameter pCompany =
                new MySql.Data.MySqlClient.MySqlParameter("?COMPANY", MySql.Data.MySqlClient.MySqlDbType.VarChar, 65);
            pCompany.Value = (Company != null) ? (object)Company : (object)DBNull.Value;
            TmpInsertCommand.Parameters.Add(pCompany);


            TmpInsertCommand.CommandType = CommandType.Text;
            TmpInsertCommand.CommandText = TmpInsertSql;
            m_Connection.Open();

            try
            {
                TmpInsertCommand.ExecuteNonQuery();
                ReturnValue = true;
            }
            catch (MySql.Data.MySqlClient.MySqlException exc)
            {
                // Log error
                ErrorDescription = exc.Message;
            }
            finally
            {
                // Clean up
                m_Connection.Close();
                TmpInsertCommand.Dispose();
            }
            return ReturnValue;
        }

        /// <summary>
        /// Responsible for inserting a record to the mailrecord table
        /// </summary>
        /// <param name="Rloc">
        /// record locator value
        /// </param>
        /// <param name="Created">
        /// create timestamp
        /// </param>
        /// <param name="IPAdress">
        /// user ip address
        /// </param>
        /// <param name="ProcessorName">
        /// processor name value
        /// </param>
        /// <param name="FormatName">
        /// format name value
        /// </param>
        /// <returns>
        /// true if successful, false otherwise
        /// </returns>
        static public bool AddMailRecord(string Rloc,
            System.DateTime Created,
            string FromName,
            string ProcessorName,
            string FormName,
            string Subject,
            string Team,
            string Company
        )
        {
            bool ReturnValue = false;
            ChwLib.DatabaseManager TmpDataMan =
                new ChwLib.DatabaseManager();
            if (null != TmpDataMan)
            {
                ReturnValue = TmpDataMan.addMailRecord(Rloc, Created, FromName, ProcessorName, FormName, Subject, Team, Company);
                TmpDataMan.Dispose();
                TmpDataMan = null;
            }
            return ReturnValue;
        }
    }



}
