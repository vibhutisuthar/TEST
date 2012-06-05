using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DAL;
using MTP.DTO;

namespace MTP.BAL
{
    public class MailRecordsBAL
    {

        #region Public Declaration and Class constuctor

        MailRecordsDAL objMailRecordsDAL;

        public MailRecordsBAL(string strCurrentSite)
        {
            objMailRecordsDAL = new MailRecordsDAL(strCurrentSite);
        }

        #endregion

        #region [Helper Methods]

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNumber"></param>
        /// <param name="AccountName"></param>
        /// <param name="SortExpression"></param>
        /// <param name="TotalRecords"></param>
        /// <returns></returns>
        public List<MailRecordsModel> SearchDataBaseByRLOC(string strRLOC, DateTime tm, string strSurname)
        {
            return objMailRecordsDAL.SearchDataBaseByRLOC(strRLOC, tm, strSurname).ToList();
        }

        /// <summary>
        /// FindEticketRecords
        /// </summary>
        /// <param name="strRLOC"></param>
        /// <param name="TicketNumber"></param>
        /// <param name="formName"></param>
        /// <returns></returns>
        public List<MailRecordsModel> FindEticketRecords(string strRLOC, string TicketNumber, string formName)
        {
            return objMailRecordsDAL.FindEticketRecords(strRLOC, TicketNumber, formName).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRLOC"></param>
        /// <param name="TicketNumber"></param>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        public List<MailRecordsModel> SearchDataBaseForInvoice(string TicketNumber, string DocumentNumber)
        {
            return objMailRecordsDAL.SearchDataBaseForInvoice(TicketNumber, DocumentNumber).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RLOC"></param>
        /// <param name="tm"></param>
        /// <returns></returns>
        public List<MailRecordsModel> SearchDataBase(string RLOC, DateTime tm)
        {
            return objMailRecordsDAL.SearchDataBase(RLOC, tm).ToList();
        }


        /// <summary>
        /// SaveAccountActiveStatus
        /// </summary>
        /// <param name="AccountModelDTO"></param>
        /// <returns></returns>
        public bool SaveMailRecord(MailRecordsModel MailRecordsDTO)
        {
            return objMailRecordsDAL.SaveMailRecord(MailRecordsDTO);
        }

        /// <summary>
        /// GetCompanyDeatils
        /// </summary>
        /// <param name="strRLOC"></param>
        /// <param name="strSurname"></param>
        /// <returns></returns>
        public List<MailRecordsModel> GetCompanyDeatils(string strRLOC, string strFormName)
        {
            return objMailRecordsDAL.GetCompanyDeatils(strRLOC, strFormName).ToList();
        }

        #endregion

    }
}
