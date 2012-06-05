using System;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Web.Configuration;
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for Log.
/// </summary>
public class Tranx
{
    string _Tranx, _Reference, _Surname, _Detail;
    public ManualResetEvent allDone = new ManualResetEvent(false);

    public Tranx()
    {
    }
    public void ItineraryRetrieve(string Reference, string Surname, string Detail)
    {
        WriteRecordToDatabase("ItineraryRetrieve", Reference, Surname, Detail);
    }
    public void InvoiceRetrieve(string Reference, string Surname, string Detail)
    {
        WriteRecordToDatabase("InvoiceRetrieve", Reference, Surname, Detail);
    }
    public void EticketRetrieve(string Reference, string Surname, string Detail)
    {
        WriteRecordToDatabase("EticketRetrieve", Reference, Surname, Detail);
    }
    public void Email(string Reference, string Surname, string Detail)
    {
        WriteRecordToDatabase("ItineraryEmail", Reference, Surname, Detail);
    }
    public void SMS(string Reference, string Surname, string PhoneNumber, string Detail)
    {
        WriteRecordToDatabase("SMS", Reference, Surname, Detail);
    }
    public void SMS(string PhoneNumber, string Detail, string Status, int StatusCode)
    {
        WriteRecordToSMSDatabase(PhoneNumber, Detail, Status, StatusCode, "");
    }
    public void SMS(string PhoneNumber, string Detail, string Status, int StatusCode, string ErrorResponse)
    {
        WriteRecordToSMSDatabase(PhoneNumber, Detail, Status, StatusCode, ErrorResponse);
    }
    protected string GetDSN()
    {

        string DSN = WebConfigurationManager.AppSettings["BillingDSN"];
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
    void WriteRecordToSMSDatabase(string PhoneNumber, string Detail, string Status, int StatusCode, string ErrorResponse)
    {
        DateTime Now = DateTime.Now;

        //create a suitable SQL statement and execute it
        string strSQL = string.Format(
            "Insert into SMS (ToNumber, Message, Status, StatusCode, ErrorMessage) values ('{0}','{1}','{2}', {3}, '{4}' )",
            PhoneNumber, Detail, Status, StatusCode, ErrorResponse
            );

        string strConnect;

        //create a variable to hold an instance of a DataReader object
        MySql.Data.MySqlClient.MySqlDataReader objDataReader;
        //OleDbDataReader objDataReader;

        try
        {

            strConnect = GetDSN();

            if (strConnect.Length == 0)
                return;
            //string.Format("server={0};user id={1}; password={2}; database=nlog; pooling=false",
            //	"localhost", "woz", "2rachelle" );



            //strConnect = GetDataSourceMySQL();
            //create a new Connection object using the connection string
            //OleDbConnection objConnect = new OleDbConnection(strConnect);
            MySql.Data.MySqlClient.MySqlConnection objConnect = new MySql.Data.MySqlClient.MySqlConnection(strConnect);

            //open the connection to the database
            objConnect.Open();

            //create a new Command using the connection object and select statement
            //OleDbCommand objCommand = new OleDbCommand(strSQL, objConnect);
            MySql.Data.MySqlClient.MySqlCommand objCommand = new MySql.Data.MySqlClient.MySqlCommand(strSQL, objConnect);

            //execute the SQL statement against the command to get the DataReader
            objDataReader = objCommand.ExecuteReader();

            objConnect.Close();
        }
        catch (Exception objError)
        {

            //display error details
            System.Diagnostics.Trace.WriteLine("<b>* Error while accessing data</b>.<br />"
                + objError.Message + "<br />" + objError.Source + "<p />");
            return;  // and stop execution

        }
        System.Diagnostics.Trace.WriteLine("Thread WriteRecordToDatabase Completed");
    }
    void WriteRecordToDatabase(string Tranx, string Reference, string Surname, string Detail)
    {
        _Tranx = Tranx;
        _Reference = Reference;
        _Surname = Surname;
        _Detail = Detail;

        Thread t1 = new Thread(new ThreadStart(WriteRecordToDatabaseInThread));
        t1.Start();
        allDone.WaitOne(1000, false);

        System.Diagnostics.Trace.WriteLine("Thread WriteRecordToDatabase Completed");
    }
    void WriteRecordToDatabaseInThread()
    {
        DateTime Now = DateTime.Now;

        //create a suitable SQL statement and execute it
        string strSQL = string.Format(
            "Insert into transactions (TranxType,Reference,Surname,Detail) values ('{0}','{1}','{2}','{3}')",
            _Tranx, _Reference, _Surname, _Detail
            );

        string strConnect;

        //create a variable to hold an instance of a DataReader object
        MySql.Data.MySqlClient.MySqlDataReader objDataReader;
        //OleDbDataReader objDataReader;

        try
        {

            strConnect = GetDSN();

            if (strConnect.Length == 0)
                return;
            //string.Format("server={0};user id={1}; password={2}; database=nlog; pooling=false",
            //	"localhost", "woz", "2rachelle" );



            //strConnect = GetDataSourceMySQL();
            //create a new Connection object using the connection string
            //OleDbConnection objConnect = new OleDbConnection(strConnect);
            MySql.Data.MySqlClient.MySqlConnection objConnect = new MySql.Data.MySqlClient.MySqlConnection(strConnect);

            //open the connection to the database
            objConnect.Open();

            //create a new Command using the connection object and select statement
            //OleDbCommand objCommand = new OleDbCommand(strSQL, objConnect);
            MySql.Data.MySqlClient.MySqlCommand objCommand = new MySql.Data.MySqlClient.MySqlCommand(strSQL, objConnect);

            //execute the SQL statement against the command to get the DataReader
            objDataReader = objCommand.ExecuteReader();

            objConnect.Close();
        }
        catch (Exception objError)
        {

            //display error details
            System.Diagnostics.Trace.WriteLine("<b>* Error while accessing data</b>.<br />"
                + objError.Message + "<br />" + objError.Source + "<p />");
            allDone.Set();
            return;  // and stop execution

        }

        allDone.Set();


    }
}
