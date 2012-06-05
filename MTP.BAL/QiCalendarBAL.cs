using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DAL;
using MTP.DTO;

namespace MTP.BAL
{
    public class QiCalendarBAL
    {

        #region Public Declaration and Class constuctor

        QiCalendarDAL objQiCalendarDAL;

        public QiCalendarBAL()
        {
            objQiCalendarDAL = new QiCalendarDAL();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// SearchDataBaseByRLOC_DownloadToCalendar
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <param name="segmentNumber"></param>
        /// <returns></returns>
        public List<QiCalendarModel> SearchDataBaseByRLOC_DownloadToCalendar(string referenceNumber, string surName)
        {
            return objQiCalendarDAL.SearchDataBaseByRLOC_DownloadToCalendar(referenceNumber, surName).ToList();
        }

        #endregion
    }
}
