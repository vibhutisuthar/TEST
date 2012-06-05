using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;

namespace MTP.DAL
{
    public class QiCompanyDAL
    {
        #region Public Declaration and Class constuctor

        //string CurrentSite = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentSite"];
        qiEntities qiEntitiesData = null;

        public QiCompanyDAL()
        {
            qiEntitiesData = new qiEntities();
        }


        #endregion


        #region Helper Methods

        /// <summary>
        /// GetCompanyData
        /// </summary>
        /// <returns></returns>
        public IQueryable<QiCompanyModel> GetCompanyData(string strCompany, string strFormName, string strCountryCode)
        {
            IQueryable<QiCompanyModel> lstQiCompany = null;
            lstQiCompany = (from qicompany in qiEntitiesData.qi_company
                            where qicompany.PrimaryCode == strCompany && qicompany.SecondaryCode == strFormName && qicompany.CountryCode == strCountryCode
                            select new QiCompanyModel
                            {
                                PrimaryCode = qicompany.PrimaryCode,
                                Pseudo = qicompany.Pseudo,
                                SecondaryCode = qicompany.SecondaryCode,
                                Name = qicompany.Name,
                                AddressCode = qicompany.AddressCode,
                                PhoneCode = qicompany.PhoneCode,
                                MacroCode = qicompany.MacroCode,
                                MailCode = qicompany.MailCode,
                                Language = qicompany.Language,
                                Region = qicompany.Region,
                                PageCode = qicompany.PageCode,
                                Stamp = qicompany.Stamp,
                                HeaderFooterCode = qicompany.HeaderFooterCode,
                                CostCentreCode = qicompany.CostCentreCode,
                                CountryCode = qicompany.CountryCode,
                                AccessCode = qicompany.AccessCode,
                                Name2 = qicompany.Name2,
                                Eticket = qicompany.Eticket,
                                Email = qicompany.Email,
                                Invoice = qicompany.Invoice,
                                CreditNote = qicompany.CreditNote,
                                UserName = qicompany.UserName,
                                Itinerary = qicompany.Itinerary,
                                D2C = qicompany.D2C
                            });


            return lstQiCompany;
        }

        #endregion


    }
}
