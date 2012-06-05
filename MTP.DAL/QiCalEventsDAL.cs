using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;
using System.Linq.Expressions;
using System.Data;
using System.Reflection;

namespace MTP.DAL
{
    public class QiCalEventsDAL
    {
        #region Public Declaration and Class constuctor

        qitransactionsEntities qitransactionsData = null;

        public QiCalEventsDAL()
        {
            qitransactionsData = new qitransactionsEntities();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <returns></returns>
        public IQueryable<QiCalEventsModel> SearchDataBaseByRLOC(string referenceNumber, string surName)
        {
            IQueryable<QiCalEventsModel> lstQiCalEvents = null;
            lstQiCalEvents = (from qical in qitransactionsData.qicalevents
                              where qical.RLOC == referenceNumber
                              && qical.PaxName == surName
                              select new QiCalEventsModel
                       {
                           GDS = qical.GDS,
                           RLOC = qical.RLOC,
                           PaxName = qical.PaxName,
                           SegType = qical.SegType,
                           SegNumber = qical.SegNumber,
                           DTStart = qical.DTStart,
                           DTEnd = qical.DTEnd,
                           DCreated = qical.DCreated,
                           UID = qical.UID,
                           Subject = qical.Subject,
                           Location = qical.Location,
                           Description = qical.Description,
                           Stamp = qical.Stamp,
                           EventText = qical.EventText,
                           AltDescription = qical.AltDescription
                       }).OrderBy(x => x.SegNumber);

            return lstQiCalEvents;
        }

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <param name="segmentNumber"></param>
        /// <returns></returns>
        public IQueryable<QiCalEventsModel> SearchDataBaseByRLOC(string referenceNumber, string surName, int segmentNumber)
        {
            IQueryable<QiCalEventsModel> lstQiCalEvents = null;
            lstQiCalEvents = (from qical in qitransactionsData.qicalevents
                              where qical.RLOC == referenceNumber &&
                              qical.PaxName == surName && qical.SegNumber == segmentNumber
                              select new QiCalEventsModel
                       {
                           //DTStart = qical.DTStart,
                           //DTEnd = qical.DTEnd,
                           //DCreated = qical.DCreated,
                           //UID = qical.UID,
                           //Subject = qical.Subject,
                           //Location = qical.Location,
                           //Description = qical.Description,
                           //AltDescription = qical.AltDescription,
                           GDS = qical.GDS,
                           RLOC = qical.RLOC,
                           PaxName = qical.PaxName,
                           SegType = qical.SegType,
                           SegNumber = qical.SegNumber,
                           DTStart = qical.DTStart,
                           DTEnd = qical.DTEnd,
                           DCreated = qical.DCreated,
                           UID = qical.UID,
                           Subject = qical.Subject,
                           Location = qical.Location,
                           Description = qical.Description,
                           Stamp = qical.Stamp,
                           EventText = qical.EventText,
                           AltDescription = qical.AltDescription
                       });

            return lstQiCalEvents;
        }


        #endregion
    }
}
