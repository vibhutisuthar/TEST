using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;
using System.Linq.Expressions;
using System.Data;
using System.Reflection;
using System.Data.Entity;
using System.Data.Linq;
using System.Data.Objects.DataClasses;
using System.Data.Linq.Mapping;
using System.Data.Objects;
using System.Data.Metadata.Edm;


namespace MTP.DAL
{
    public class MailRecordsDAL : IDisposable
    {
        #region Public Declaration and Class constuctor

        //     private string _CurrentSite;
        //public string CurrentSite
        //{
        //    get { return _CurrentSite; }
        //    set { _CurrentSite = value; }
        //}


        string CurrentSite = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentSite"];
        //CurrentSite = strCurrentSite; //System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentSite"];
        qitransactionsEntities qitransactionsData = null;
        wozEntities wozData = null;

        public MailRecordsDAL(string strCurrentSite)
        {
            GetEntities(strCurrentSite);
        }



        #endregion

        #region Helper Methods

        /// <summary>
        /// GetEntities
        /// </summary>
        public void GetEntities(string strCurrentSite)
        {
            CurrentSite = strCurrentSite;
            if (strCurrentSite == "")
            {
                CurrentSite = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentSite"];
            }

            if (CurrentSite == "EU")
            {
                qitransactionsData = Connection.GetEUEntities();
            }
            else if (CurrentSite == "AU")
            {
                wozData = Connection.GetAUEntities();
            }
        }

        /// <summary>
        /// SearchDataBaseByRLOC
        /// </summary>
        /// <returns></returns>
        public IQueryable<MailRecordsModel> SearchDataBaseByRLOC(string strRLOC, DateTime tm, string strSurname)
        {

            //var query = wozData.GetType().GetProperty("mailrecords").GetValue(wozData, null);
            //var query2 = ((IQueryable)query).Where(“it.Id > 1”);

            IQueryable<MailRecordsModel> lstMailRecords = null;
            if (CurrentSite == "EU")
            {
                lstMailRecords = (from mails in qitransactionsData.qimailrecords
                                  where mails.RLOC == strRLOC &&
                                  mails.Created > tm &&
                                      //  mails.FormName.StartsWith("AXQIITN") &&
                                  mails.PreprocStatus > 0
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      Subject = mails.Subject,
                                      TicketNumber = mails.TicketNumber,
                                      Created = mails.Created,
                                      Method = mails.Method,
                                      DocumentNumber = mails.DocumentNumber,
                                      FormName = mails.FormName,
                                      FileName = mails.FileName,
                                      ProcessorName = mails.ProcessorName
                                  }).OrderByDescending(x => x.Created);
            }
            else if (CurrentSite == "AU")
            {
                lstMailRecords = (from mails in wozData.mailrecords
                                  where mails.RLOC == strRLOC &&
                                  mails.Created > tm &&
                                  mails.FormName.StartsWith("AXQIITN") &&
                                  mails.PreprocStatus > 0
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      Subject = mails.Subject,
                                      TicketNumber = mails.TicketNumber,
                                      Created = mails.Created,
                                      Method = mails.Method,
                                      DocumentNumber = mails.DocumentNumber,
                                      FormName = mails.FormName,
                                      FileName = mails.FileName,
                                      ProcessorName = mails.ProcessorName
                                  }).OrderByDescending(x => x.Created);

            }
            return lstMailRecords;
        }

        /// <summary>
        /// FindEticketRecords
        /// </summary>
        /// <param name="strRLOC"></param>
        /// <param name="TicketNumber"></param>
        /// <param name="formName"></param>
        /// <returns></returns>
        public IQueryable<MailRecordsModel> FindEticketRecords(string strRLOC, string TicketNumber, string formName)
        {

            //SQL = "SELECT RLOC, ToName, FromName, Subject, FormName, Created, TicketNumber, Company, Pseudo, FileName from mailrecords
            //where formname='AXQIETICKET' AND SUBSTRING(TicketNumber,5)='" + GetTick + "' LIMIT 1;";
            IQueryable<MailRecordsModel> lstMailRecords = null;
            if (CurrentSite == "EU")
            {
                lstMailRecords = (from mails in qitransactionsData.qimailrecords
                                  where mails.RLOC == strRLOC
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      ToName = mails.ToName,
                                      FormName = mails.FormName,
                                      Subject = mails.Subject,
                                      Created = mails.Created,
                                      TicketNumber = mails.TicketNumber,
                                      Company = mails.Company,
                                      Pseudo = mails.Pseudo,
                                      FileName = mails.FileName
                                  });
            }
            else if (CurrentSite == "AU")
            {
                if (TicketNumber.Length > 0)
                {
                    lstMailRecords = (from mails in wozData.mailrecords
                                      where mails.FormName == formName && mails.TicketNumber == TicketNumber //mails.TicketNumber.Substring(5, TicketNumber.Length) == TicketNumber
                                      select new MailRecordsModel
                                      {
                                          ID = mails.ID,
                                          RLOC = mails.RLOC,
                                          ToName = mails.ToName,
                                          FormName = mails.FormName,
                                          Subject = mails.Subject,
                                          Created = mails.Created,
                                          TicketNumber = mails.TicketNumber,
                                          Company = mails.Company,
                                          Pseudo = mails.Pseudo,
                                          FileName = mails.FileName
                                      });
                }
                else
                {
                    //SQL = "SELECT RLOC, ToName, FromName, Subject, FormName, Created, TicketNumber, Company, Pseudo, FileName from mailrecords where RLOC='" + m_RLOC + "'  order by created desc";
                    lstMailRecords = (from mails in wozData.mailrecords
                                      where mails.RLOC == strRLOC
                                      select new MailRecordsModel
                                      {
                                          ID = mails.ID,
                                          RLOC = mails.RLOC,
                                          ToName = mails.ToName,
                                          FormName = mails.FormName,
                                          Subject = mails.Subject,
                                          Created = mails.Created,
                                          TicketNumber = mails.TicketNumber,
                                          Company = mails.Company,
                                          Pseudo = mails.Pseudo,
                                          FileName = mails.FileName
                                      }).OrderByDescending(x => x.Created);

                }

            }


            return lstMailRecords;
        }

        /// <summary>
        /// SearchDataBaseForInvoice
        /// </summary>
        /// <param name="strRLOC"></param>
        /// <param name="TicketNumber"></param>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        public IQueryable<MailRecordsModel> SearchDataBaseForInvoice(string TicketNumber, string DocumentNumber)
        {
            IQueryable<MailRecordsModel> lstMailRecords = null;
            if (CurrentSite == "EU")
            {
                lstMailRecords = (from mails in qitransactionsData.qimailrecords
                                  where
                                      mails.PreprocStatus > 0 && mails.TicketNumber == TicketNumber || mails.DocumentNumber == DocumentNumber
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      Subject = mails.Subject,
                                      TicketNumber = mails.TicketNumber,
                                      Created = mails.Created,
                                      Method = mails.Method,
                                      DocumentNumber = mails.DocumentNumber,
                                      FormName = mails.FormName,
                                      FileName = mails.FileName,
                                  }).OrderByDescending(x => x.Created);

            }
            else if (CurrentSite == "AU")
            {
                lstMailRecords = (from mails in wozData.mailrecords
                                  where
                                      mails.PreprocStatus > 0 && mails.TicketNumber == TicketNumber || mails.DocumentNumber == DocumentNumber
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      Subject = mails.Subject,
                                      TicketNumber = mails.TicketNumber,
                                      Created = mails.Created,
                                      Method = mails.Method,
                                      DocumentNumber = mails.DocumentNumber,
                                      FormName = mails.FormName,
                                      FileName = mails.FileName,
                                  }).OrderByDescending(x => x.Created);
            }

            return lstMailRecords;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RLOC"></param>
        /// <returns></returns>
        public IQueryable<MailRecordsModel> SearchDataBase(string RLOC, DateTime tm)
        {
            //SQL = "SELECT ID, RLOC, Subject, TicketNumber, Created, Method, DocumentNumber, FormName, FileName FROM MailRecords where RLOC='{0}' and Created > '{1}' and PreprocStatus > 0 order by created";
            IQueryable<MailRecordsModel> lstMailRecords = null;
            if (CurrentSite == "EU")
            {
                lstMailRecords = (from mails in qitransactionsData.qimailrecords
                                  where mails.RLOC == RLOC && mails.Created > tm && mails.PreprocStatus > 0
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      Subject = mails.Subject,
                                      TicketNumber = mails.TicketNumber,
                                      Created = mails.Created,
                                      Method = mails.Method,
                                      DocumentNumber = mails.DocumentNumber,
                                      FormName = mails.FormName,
                                      FileName = mails.FileName,
                                  }).OrderByDescending(x => x.Created);
            }
            else if (CurrentSite == "AU")
            {
                lstMailRecords = (from mails in wozData.mailrecords
                                  where mails.RLOC == RLOC && mails.Created > tm && mails.PreprocStatus > 0
                                  select new MailRecordsModel
                                  {
                                      ID = mails.ID,
                                      RLOC = mails.RLOC,
                                      Subject = mails.Subject,
                                      TicketNumber = mails.TicketNumber,
                                      Created = mails.Created,
                                      Method = mails.Method,
                                      DocumentNumber = mails.DocumentNumber,
                                      FormName = mails.FormName,
                                      FileName = mails.FileName,
                                  }).OrderByDescending(x => x.Created);
            }

            return lstMailRecords;
        }


        /// <summary>
        /// SaveAccountActiveStatus
        /// </summary>
        /// <param name="AccountModelDTO"></param>
        /// <returns></returns>
        public bool SaveMailRecord(MailRecordsModel MailRecordsDTO)
        {
            //Rloc, Created, FromName, ProcessorName, FormName, Subject, Team, Company
            try
            {
                mailrecord objmailrecord = new mailrecord();
                objmailrecord.RLOC = MailRecordsDTO.RLOC;
                objmailrecord.Created = MailRecordsDTO.Created;
                objmailrecord.FromName = MailRecordsDTO.FromName;
                objmailrecord.ProcessorName = MailRecordsDTO.ProcessorName;
                objmailrecord.FormName = MailRecordsDTO.FormName;
                objmailrecord.Subject = MailRecordsDTO.Subject;
                objmailrecord.Team = MailRecordsDTO.Team;
                objmailrecord.Company = MailRecordsDTO.Company;
                wozData.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        //        private  bool IsKindOf(this EntityType entityType, EntityType otherEntityType)  
        //{  
        //    if (entityType == otherEntityType)  
        //        return true;  
        //    if (entityType.BaseType != null)  
        //        return IsKindOf((EntityType)entityType.BaseType, otherEntityType);  
        //    else 
        //        return false;  
        //}  


        //private EntitySet GetEntitySet(Type t, MetadataWorkspace workspace, string defaultContainerName)  
        //{  
        //    EdmEntityTypeAttribute eeta = (EdmEntityTypeAttribute)Attribute.GetCustomAttribute(t, typeof(EdmEntityTypeAttribute));  

        //    string qualifiedName = eeta.NamespaceName + "." + eeta.Name;  
        //    EntityType et = workspace.GetItem<EntityType>(qualifiedName, DataSpace.CSpace);  

        //    EntityContainer ec = workspace.GetEntityContainer(defaultContainerName, DataSpace.CSpace);  
        //    EntitySet foundSet = null;  

        //    foreach (EntitySet set in ec.BaseEntitySets.OfType<EntitySet>())  
        //    {  
        //        if (et.IsKindOf(set.ElementType))  
        //        {  
        //            if (foundSet == null)  
        //            {  
        //                foundSet = set;  
        //            }  
        //            else 
        //            {  
        //                return null;  
        //            }  
        //        }  
        //    }  

        //    return foundSet;  
        //}


        /// <summary>
        /// GetCompanyDeatils
        /// </summary>
        /// <param name="strRLOC"></param>
        /// <param name="strSurname"></param>
        /// <returns></returns>
        public IQueryable<MailRecordsModel> GetCompanyDeatils(string strRLOC, string strFormName)
        {
            IQueryable<MailRecordsModel> lstCompanyDeatils = null;
            lstCompanyDeatils = (from mails in wozData.mailrecords
                                 where mails.RLOC == strRLOC && mails.FormName.Contains(strFormName)
                                 select new MailRecordsModel
                                 {
                                     ID = mails.ID,
                                     RLOC = mails.RLOC,
                                     Company = mails.Company,
                                     FormName = mails.FormName
                                 });

            return lstCompanyDeatils;
        }


        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            wozData.Dispose();
        }

        #endregion
    }



    //static class DataContextExtensions
    //{
    //    public static ITable GetTableByName(this wozEntities context, string tableName)
    //    {
    //        if (context == null)
    //        {
    //            throw new ArgumentNullException("context");
    //        }
    //        if (tableName == null)
    //        {
    //            throw new ArgumentNullException("tableName");
    //        }
    //        return (ITable)context.GetType().GetProperty(tableName).GetValue(context, null);
    //    }
    //}


}
