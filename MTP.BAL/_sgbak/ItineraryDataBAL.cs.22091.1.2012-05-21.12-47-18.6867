using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;
using MTP.DAL;
using System.Data;

namespace MTP.BAL
{
    public class ItineraryDataBAL
    {
        #region Public Declaration and Class constuctor

        ItineraryDataDAL objItineraryDataDAL;

        public ItineraryDataBAL()
        {
            objItineraryDataDAL = new ItineraryDataDAL();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// SearchItineraryDataBySurnameAndRLOC
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <param name="segmentNumber"></param>
        /// <returns></returns>
        public List<XmlBookingModel> SearchItineraryDataBySurnameAndRLOC(string Surname, string RLOC)
        {
            return objItineraryDataDAL.SearchItineraryDataBySurnameAndRLOC(Surname, RLOC).ToList();
        }

        #endregion

    }


    public class BookingDataBAL
    {

        #region Public Declaration and Class constuctor

        BookingDataDAL objBookingDataDAL;

        public BookingDataBAL()
        {
            objBookingDataDAL = new BookingDataDAL();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// SearchItineraryDataBySurnameAndRLOC
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <param name="segmentNumber"></param>
        /// <returns></returns>
        public List<BookingModel> GetBookingByEmail(string strEmailAddress)
        {
            return objBookingDataDAL.GetBookingByEmail(strEmailAddress).ToList();
        }

        #endregion
    }


}
