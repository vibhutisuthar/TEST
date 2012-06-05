using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DAL
{
    public static class Connection
    {
      
        #region Helper Methods

      
        /// <summary>
        /// GetEntities
        /// </summary>
        public static wozEntities GetAUEntities()
        {
            wozEntities wozData = new wozEntities();
            return wozData;
        }

        /// <summary>
        /// GetEntities
        /// </summary>
        public static qitransactionsEntities GetEUEntities()
        {
            qitransactionsEntities qitransactionsData = qitransactionsData = new qitransactionsEntities();
            return qitransactionsData;
        }


        /// <summary>
        /// GetMTPUsersData
        /// </summary>
        public static mtpUsersEntities GetMTPUsersEntities()
        {
            mtpUsersEntities mtp_usersEntitiesData = new mtpUsersEntities();
            return mtp_usersEntitiesData;
        }


        /// <summary>
        /// GetMTPUsersData
        /// </summary>
        public static qilive_onlineEntities Getqilive_onlineEntities()
        {
            qilive_onlineEntities qiliveonlineData = new qilive_onlineEntities();
            return qiliveonlineData;
        }


        #endregion

    }
}
