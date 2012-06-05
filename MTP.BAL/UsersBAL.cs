using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTP.DAL;
using MTP.DTO;

namespace MTP.BAL
{
    public class UsersBAL
    {

        #region Public Declaration and Class constuctor

        UsersDAL objUsersDAL;
        public UsersBAL()
        {
            objUsersDAL = new UsersDAL();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public List<UsersModel> FindUser(string strEmailAddress, string strPassword)
        {
            return objUsersDAL.FindUser(strEmailAddress, strPassword).ToList();
        }

        /// <summary>
        /// FindUser
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public UsersModel FindUser(string strEmailAddress)
        {
            return objUsersDAL.FindUser(strEmailAddress);
        }


        public UsersModel FindUser(Int32 ID)
        {
            return objUsersDAL.FindUser(ID);
        }

        /// <summary>
        /// RegisterNewUSer
        /// </summary>
        /// <returns></returns>
        public int RegisterNewUSer(UsersModel UsersDTO)
        {
            return objUsersDAL.RegisterNewUSer(UsersDTO);
        }

        /// <summary>
        /// SaveUser
        /// </summary>
        /// <param name="AccountModelDTO"></param>
        /// <returns></returns>
        public int SaveUser(UsersModel UsersDTO)
        {
            return objUsersDAL.SaveUser(UsersDTO);
        }

        /// <summary>
        /// GetPasswordByEmail
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <returns></returns>
        public List<UsersModel> GetPasswordByEmail(string strEmailAddress)
        {
            return objUsersDAL.GetPasswordByEmail(strEmailAddress).ToList();
        }

        /// <summary>
        /// GetPasswordByEmail
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <returns></returns>
        public List<UsersModel> GetConfirmeRegistration(string strEncodestring)
        {
            return objUsersDAL.GetConfiremRegistration(strEncodestring).ToList();
        }

        public UsersModel ResetEncodeStringExist(string strEncodestring)
        {
            return objUsersDAL.ResetEncodeStringExist(strEncodestring);
        }

        /// <summary>
        /// UpdateUserStatus
        /// </summary>
        /// <param name="AccountModelDTO"></param>
        /// <returns></returns>
        public int UpdateUserStatus(Int32 UsersID, string strStatus,string encodestring)
        {
            return objUsersDAL.UpdateUserStatus(UsersID, strStatus, encodestring);
        }

        #endregion




    }
}
