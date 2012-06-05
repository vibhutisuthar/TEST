using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;
using MTP.DAL;

namespace MTP.BAL
{
    public class QiCalEventsBAL
    {

        #region Public Declaration and Class constuctor

        QiCalEventsDAL objQiCalEventsDAL;

        public QiCalEventsBAL()
        {
            objQiCalEventsDAL = new QiCalEventsDAL();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <returns></returns>
        public List<QiCalEventsModel> SearchDataBaseByRLOC(string referenceNumber, string surName, int segmentNumber)
        {
            return objQiCalEventsDAL.SearchDataBaseByRLOC(referenceNumber, surName, segmentNumber).ToList();
        }

         /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="surName"></param>
        /// <param name="segmentNumber"></param>
        /// <returns></returns>
        public List<QiCalEventsModel> SearchDataBaseByRLOC(string referenceNumber, string surName)
        {
            return objQiCalEventsDAL.SearchDataBaseByRLOC(referenceNumber, surName).ToList();
        }

        #endregion


    }
}
