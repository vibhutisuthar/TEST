using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;

namespace MTP.DAL
{
    public class QiCalendarDAL
    {

        #region Public Declaration and Class constuctor

        qitransactionsEntities qitransactionsData = null;

        public QiCalendarDAL()
        {
            qitransactionsData = new qitransactionsEntities();
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
        public IQueryable<QiCalendarModel> SearchDataBaseByRLOC_DownloadToCalendar(string referenceNumber, string surName)
        {
            IQueryable<QiCalendarModel> lstQiCalendar = null;
            lstQiCalendar = (from qical in qitransactionsData.qicalendars
                             where qical.RLoc == referenceNumber &&
                             qical.PaxName == surName
                             select new QiCalendarModel
                             {
                                 GDS = qical.GDS,
                                 RLOC = qical.RLoc,
                                 PaxName = qical.PaxName,
                                 Language = qical.Language,
                                 FileLocation = qical.FileLocation,
                                 Stamp = qical.Stamp
                             });

            return lstQiCalendar;
        }


        #endregion
    }
}
