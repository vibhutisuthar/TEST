using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;
using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace MTP.DAL
{
    public class ItineraryDataDAL
    {
        #region Public Declaration and Class constuctor

        qilivedataEntities qilivedata = null;

        public ItineraryDataDAL()
        {
            qilivedata = new qilivedataEntities();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// SearchItineraryDataBySurnameAndRLOC
        /// </summary>
        /// <param name="Surname"></param>
        /// <param name="RLOC"></param>
        /// <returns></returns>
        public IList<XmlBookingModel> SearchItineraryDataBySurnameAndRLOC(string Surname, string RLOC)
        {
            try
            {
                MySqlParameter[] queryParams = new MySqlParameter[] { 
                                        new MySqlParameter("p_RLOC", RLOC),
                                        new MySqlParameter("p_lastname", Surname),                                      
                                    };

                StringBuilder sb = new StringBuilder();
                sb.Append("CALL GetItineraryDataBySurnameAndRLOC(@p_RLOC, @p_lastname)");

                string commandText = sb.ToString();
                var results = qilivedata.ExecuteStoreQuery<XmlBookingModel>(commandText, queryParams);

                return results.ToList();
            }
            catch
            {
                IList<XmlBookingModel> lst = new List<XmlBookingModel>();
                return lst;
            }
        }

        //public DataTable SearchItineraryDataBySurnameAndRLOC(string Surname, string RLOC)
        //{                    
        //    List<GetItineraryDataBySurnameAndRLOC_Result> lstItinerary = new List<GetItineraryDataBySurnameAndRLOC_Result>();
        //    var lstItineraryModel = from itinData in qilivedata.GetItineraryDataBySurnameAndRLOC(RLOC, Surname) select itinData;
        //    lstItinerary = lstItineraryModel.ToList();

        //    DataTable dtQiLiveItineraryData = new DataTable("QiLiveItineraryData");
        //    dtQiLiveItineraryData = ListToDataTable<GetItineraryDataBySurnameAndRLOC_Result>(lstItinerary);
        //    return dtQiLiveItineraryData;
        //}

        #endregion
    }


    public class BookingDataDAL
    {
        #region Public Declaration and Class constuctor

        qilivedataEntities qilivedata = null;

        public BookingDataDAL()
        {
            qilivedata = new qilivedataEntities();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// GetBookingByEmail
        /// </summary>
        /// <param name="Surname"></param>
        /// <param name="RLOC"></param>
        /// <returns></returns>
        public IList<BookingModel> GetBookingByEmail(string strEmailAddress)
        {
            MySqlParameter[] queryParams = new MySqlParameter[] { 
                                        new MySqlParameter("email", strEmailAddress),                                        
                                    };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL GetBookingByEmail(@email)");

            string commandText = sb.ToString();
            var results = qilivedata.ExecuteStoreQuery<BookingModel>(commandText, queryParams);
      
            return results.ToList();
        }

        //public DataTable SearchItineraryDataBySurnameAndRLOC(string Surname, string RLOC)
        //{                    
        //    List<GetItineraryDataBySurnameAndRLOC_Result> lstItinerary = new List<GetItineraryDataBySurnameAndRLOC_Result>();
        //    var lstItineraryModel = from itinData in qilivedata.GetItineraryDataBySurnameAndRLOC(RLOC, Surname) select itinData;
        //    lstItinerary = lstItineraryModel.ToList();

        //    DataTable dtQiLiveItineraryData = new DataTable("QiLiveItineraryData");
        //    dtQiLiveItineraryData = ListToDataTable<GetItineraryDataBySurnameAndRLOC_Result>(lstItinerary);
        //    return dtQiLiveItineraryData;
        //}

        #endregion

    }
}