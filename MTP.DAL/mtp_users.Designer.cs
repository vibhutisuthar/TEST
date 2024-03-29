﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]

namespace MTP.DAL
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class mtpUsersEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new mtpUsersEntities object using the connection string found in the 'mtpUsersEntities' section of the application configuration file.
        /// </summary>
        public mtpUsersEntities() : base("name=mtpUsersEntities", "mtpUsersEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new mtpUsersEntities object.
        /// </summary>
        public mtpUsersEntities(string connectionString) : base(connectionString, "mtpUsersEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new mtpUsersEntities object.
        /// </summary>
        public mtpUsersEntities(EntityConnection connection) : base(connection, "mtpUsersEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<LoginUser> LoginUsers
        {
            get
            {
                if ((_LoginUsers == null))
                {
                    _LoginUsers = base.CreateObjectSet<LoginUser>("LoginUsers");
                }
                return _LoginUsers;
            }
        }
        private ObjectSet<LoginUser> _LoginUsers;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the LoginUsers EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToLoginUsers(LoginUser loginUser)
        {
            base.AddObject("LoginUsers", loginUser);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="mtp_usersModel", Name="LoginUser")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class LoginUser : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new LoginUser object.
        /// </summary>
        /// <param name="id">Initial value of the id property.</param>
        /// <param name="emailaddress">Initial value of the emailaddress property.</param>
        /// <param name="password">Initial value of the Password property.</param>
        public static LoginUser CreateLoginUser(global::System.Int32 id, global::System.String emailaddress, global::System.String password)
        {
            LoginUser loginUser = new LoginUser();
            loginUser.id = id;
            loginUser.emailaddress = emailaddress;
            loginUser.Password = password;
            return loginUser;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    OnidChanging(value);
                    ReportPropertyChanging("id");
                    _id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("id");
                    OnidChanged();
                }
            }
        }
        private global::System.Int32 _id;
        partial void OnidChanging(global::System.Int32 value);
        partial void OnidChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String emailaddress
        {
            get
            {
                return _emailaddress;
            }
            set
            {
                OnemailaddressChanging(value);
                ReportPropertyChanging("emailaddress");
                _emailaddress = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("emailaddress");
                OnemailaddressChanged();
            }
        }
        private global::System.String _emailaddress;
        partial void OnemailaddressChanging(global::System.String value);
        partial void OnemailaddressChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Password
        {
            get
            {
                return _Password;
            }
            set
            {
                OnPasswordChanging(value);
                ReportPropertyChanging("Password");
                _Password = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Password");
                OnPasswordChanged();
            }
        }
        private global::System.String _Password;
        partial void OnPasswordChanging(global::System.String value);
        partial void OnPasswordChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> created
        {
            get
            {
                return _created;
            }
            set
            {
                OncreatedChanging(value);
                ReportPropertyChanging("created");
                _created = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("created");
                OncreatedChanged();
            }
        }
        private Nullable<global::System.DateTime> _created;
        partial void OncreatedChanging(Nullable<global::System.DateTime> value);
        partial void OncreatedChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> last_login
        {
            get
            {
                return _last_login;
            }
            set
            {
                Onlast_loginChanging(value);
                ReportPropertyChanging("last_login");
                _last_login = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("last_login");
                Onlast_loginChanged();
            }
        }
        private Nullable<global::System.DateTime> _last_login;
        partial void Onlast_loginChanging(Nullable<global::System.DateTime> value);
        partial void Onlast_loginChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String status
        {
            get
            {
                return _status;
            }
            set
            {
                OnstatusChanging(value);
                ReportPropertyChanging("status");
                _status = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("status");
                OnstatusChanged();
            }
        }
        private global::System.String _status;
        partial void OnstatusChanging(global::System.String value);
        partial void OnstatusChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String encodestring
        {
            get
            {
                return _encodestring;
            }
            set
            {
                OnencodestringChanging(value);
                ReportPropertyChanging("encodestring");
                _encodestring = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("encodestring");
                OnencodestringChanged();
            }
        }
        private global::System.String _encodestring;
        partial void OnencodestringChanging(global::System.String value);
        partial void OnencodestringChanged();

        #endregion
    
    }

    #endregion
    
}
