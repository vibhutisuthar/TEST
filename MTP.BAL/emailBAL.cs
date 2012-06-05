using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DAL;
using MTP.DTO;

namespace MTP.BAL
{
    public class emailBAL
    {
         #region Public Declaration and Class constuctor

        emailDAL objemailDAL;
        public emailBAL()
        {
            objemailDAL = new emailDAL();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public List<emailModel> FindEmail(string strEmailCode)
        {
            return objemailDAL.FindEmail(strEmailCode).ToList();
        }

        
        #endregion




    }
}
