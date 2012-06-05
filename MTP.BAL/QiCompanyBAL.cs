using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;
using MTP.DAL;

namespace MTP.BAL
{
    public class QiCompanyBAL
    {
        #region Public Declaration and Class constuctor

        QiCompanyBAL objQiCompanyDAL;

        public QiCompanyBAL()
        {
            objQiCompanyDAL = new QiCompanyBAL();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// GetCompanyData
        /// </summary>
        /// <returns></returns>
        public List<QiCompanyModel> GetCompanyData(string strCompany, string strFormName, string strCountryCode)
        {
            return objQiCompanyDAL.GetCompanyData(strCompany, strFormName, strCountryCode).ToList();
        }

        #endregion


    }
}
