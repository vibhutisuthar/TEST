using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DTO;

namespace MTP.DAL
{
    public class UsersDAL
    {

        #region Public Declaration and Class constuctor

        //string CurrentSite = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentSite"];
        mtpUsersEntities mtpusersData = null;

        public UsersDAL()
        {
            GetEntities();
        }

        /// <summary>
        /// GetEntities
        /// </summary>
        public void GetEntities()
        {
            mtpusersData = Connection.GetMTPUsersEntities();
        }

        #endregion

        #region Helper Method

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public IQueryable<UsersModel> FindUser(string strEmailAddress, string strPassword)
        {
            IQueryable<UsersModel> lstUsersModel = null;
            lstUsersModel = (from users in mtpusersData.LoginUsers
                             where users.emailaddress == strEmailAddress && users.Password == strPassword
                             select new UsersModel
                             {
                                 ID = users.id,
                                 EmailAddress = users.emailaddress,
                                 Password = users.Password,
                                 Created = users.created,
                                 LastLogin = users.last_login,
                                 Status = users.status,
                                 encodestring = users.encodestring,
                             });

            return lstUsersModel;
        }

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public UsersModel FindUser(string strEmail)
        {
            return (from users in mtpusersData.LoginUsers
                    where users.emailaddress == strEmail
                    select new UsersModel
                    {
                        ID = users.id,
                        EmailAddress = users.emailaddress,
                        Password = users.Password,
                        Created = users.created,
                        LastLogin = users.last_login,
                        Status = users.status,
                        encodestring = users.encodestring,
                    }).SingleOrDefault();
        }

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public UsersModel FindUser(Int32 ID)
        {
            return (from users in mtpusersData.LoginUsers
                             where users.id == ID
                             select new UsersModel
                             {
                                 ID = users.id,
                                 EmailAddress = users.emailaddress,
                                 Password = users.Password,
                                 Created = users.created,
                                 LastLogin = users.last_login,
                                 Status = users.status,
                                 encodestring = users.encodestring,
                             }).SingleOrDefault();
        }

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public IQueryable<UsersModel> GetPasswordByEmail(string strEmailAddress)
        {
            IQueryable<UsersModel> lstUsersModel = null;
            lstUsersModel = (from users in mtpusersData.LoginUsers
                             where users.emailaddress == strEmailAddress
                             select new UsersModel
                             {
                                 ID = users.id,
                                 EmailAddress = users.emailaddress,
                                 Password = users.Password,
                                 Created = users.created,
                                 LastLogin = users.last_login,
                             });

            return lstUsersModel;
        }

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public IQueryable<UsersModel> GetConfiremRegistration(string strEncodestring)
        {
            IQueryable<UsersModel> lstUsersModel = null;
            lstUsersModel = (from users in mtpusersData.LoginUsers
                             where users.encodestring == strEncodestring && users.status == "pending"
                             select new UsersModel
                             {
                                 ID = users.id,
                                 EmailAddress = users.emailaddress,
                                 Password = users.Password,
                                 Created = users.created,
                                 LastLogin = users.last_login,
                             });

            return lstUsersModel;
        }

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public UsersModel ResetEncodeStringExist(string strEncodestring)
        {
            return (from users in mtpusersData.LoginUsers
                             where users.encodestring == strEncodestring 
                             select new UsersModel
                             {
                                 ID = users.id,
                                 EmailAddress = users.emailaddress,
                                 Password = users.Password,
                                 Created = users.created,
                                 LastLogin = users.last_login,
                             }).SingleOrDefault();
        }



        /// <summary>
        /// RegisterNewUSer
        /// </summary>
        /// <returns></returns>
        public int RegisterNewUSer(UsersModel UsersDTO)
        {
            int retVal = 0;
            retVal = SaveUser(UsersDTO);
            return retVal;
        }


        /// <summary>
        /// SaveUser
        /// </summary>
        /// <param name="AccountModelDTO"></param>
        /// <returns></returns>
        public int SaveUser(UsersModel UsersDTO)
        {
            try
            {
                LoginUser objUser = null;
                if (UsersDTO != null && UsersDTO.ID == 0)
                {
                    objUser = new LoginUser();
                    //objUser.id = UsersDTO.ID;
                    objUser.emailaddress = UsersDTO.EmailAddress;
                    objUser.Password = UsersDTO.Password;
                    objUser.created = DateTime.Now;
                    objUser.last_login = DateTime.Now;
                    objUser.status = UsersDTO.Status;
                    objUser.encodestring = UsersDTO.encodestring;
                    mtpusersData.LoginUsers.AddObject(objUser);
                    mtpusersData.SaveChanges();
                    return 1;
                }
                else if (UsersDTO != null && UsersDTO.ID != 0)
                {
                    objUser = mtpusersData.LoginUsers.FirstOrDefault(t => t.id == UsersDTO.ID);
                    //objUser.id = UsersDTO.ID;
                    //objUser.emailaddress = UsersDTO.EmailAddress;
                    objUser.Password = UsersDTO.Password;
                    objUser.status = UsersDTO.Status;
                    objUser.encodestring = UsersDTO.encodestring;
                    //objUser.last_login = DateTime.Now;
                    mtpusersData.SaveChanges();
                    return 1;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }



        /// <summary>
        /// UpdateUserStatus
        /// </summary>
        /// <param name="AccountModelDTO"></param>
        /// <returns></returns>
        public int UpdateUserStatus(Int32 UsersID, string strStatus, string encodestring)
        {
            try
            {
                LoginUser objUser = null;
                if (UsersID != 0)
                {
                    objUser = mtpusersData.LoginUsers.FirstOrDefault(t => t.id == UsersID);
                    objUser.status = strStatus;
                    objUser.encodestring = encodestring;
                    mtpusersData.SaveChanges();
                    return 1;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }

        #endregion



    }
}
