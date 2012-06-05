using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;


namespace MTP.DAL
{
    public class emailDAL
    {


           #region Public Declaration and Class constuctor

        //string CurrentSite = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentSite"];
        mtpUsersEntities mtpusersData = null;
        qilive_onlineEntities qiliveonlineData = null;
        public emailDAL()
        {
            GetEntities();
        }

        /// <summary>
        /// GetEntities
        /// </summary>
        public void GetEntities()
        {
            qiliveonlineData = Connection.Getqilive_onlineEntities();
        }

        #endregion

        #region Helper Method

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public IQueryable<emailModel> FindEmail(string strEmailCode)
        {
            IQueryable<emailModel> lstemailModel = null;
            lstemailModel = (from email in qiliveonlineData.emails
                             where email.email_code== strEmailCode
                             select new emailModel
                             {
                                 ID = email.id,
                                 subject = email.subject,
                                 email_code = email.email_code,
                                 email_type = email.email_type,
                                 seq = email.seq,
                                 from_address = email.from_address,
                                 from_name = email.from_name,
                                 cc = email.cc,
                                 bcc = email.bcc,
                                 stamp = email.stamp,
                                 access_criteria = email.access_criteria,
                                 owner_market = email.owner_market,
                                 owner_region = email.owner_region,
                                 deployment = email.deployment,
                                 txt_xslt_file = email.txt_xslt_file,
                                 html_xslt_file = email.html_xslt_file
                             });

            return lstemailModel;
        }

        
        #endregion


    }
}
