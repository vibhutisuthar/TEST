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
    public partial class qiEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new qiEntities object using the connection string found in the 'qiEntities' section of the application configuration file.
        /// </summary>
        public qiEntities() : base("name=qiEntities", "qiEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new qiEntities object.
        /// </summary>
        public qiEntities(string connectionString) : base(connectionString, "qiEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new qiEntities object.
        /// </summary>
        public qiEntities(EntityConnection connection) : base(connection, "qiEntities")
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
        public ObjectSet<qi_company> qi_company
        {
            get
            {
                if ((_qi_company == null))
                {
                    _qi_company = base.CreateObjectSet<qi_company>("qi_company");
                }
                return _qi_company;
            }
        }
        private ObjectSet<qi_company> _qi_company;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the qi_company EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToqi_company(qi_company qi_company)
        {
            base.AddObject("qi_company", qi_company);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="qiModel", Name="qi_company")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class qi_company : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new qi_company object.
        /// </summary>
        /// <param name="primaryCode">Initial value of the PrimaryCode property.</param>
        /// <param name="pseudo">Initial value of the Pseudo property.</param>
        /// <param name="secondaryCode">Initial value of the SecondaryCode property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="addressCode">Initial value of the AddressCode property.</param>
        /// <param name="phoneCode">Initial value of the PhoneCode property.</param>
        /// <param name="macroCode">Initial value of the MacroCode property.</param>
        /// <param name="mailCode">Initial value of the MailCode property.</param>
        /// <param name="language">Initial value of the Language property.</param>
        /// <param name="region">Initial value of the Region property.</param>
        /// <param name="pageCode">Initial value of the PageCode property.</param>
        /// <param name="stamp">Initial value of the Stamp property.</param>
        /// <param name="headerFooterCode">Initial value of the HeaderFooterCode property.</param>
        /// <param name="costCentreCode">Initial value of the CostCentreCode property.</param>
        /// <param name="countryCode">Initial value of the CountryCode property.</param>
        /// <param name="accessCode">Initial value of the AccessCode property.</param>
        /// <param name="name2">Initial value of the Name2 property.</param>
        /// <param name="eticket">Initial value of the Eticket property.</param>
        /// <param name="creditNote">Initial value of the CreditNote property.</param>
        /// <param name="itinerary">Initial value of the Itinerary property.</param>
        /// <param name="d2C">Initial value of the D2C property.</param>
        public static qi_company Createqi_company(global::System.String primaryCode, global::System.String pseudo, global::System.String secondaryCode, global::System.String name, global::System.String addressCode, global::System.String phoneCode, global::System.String macroCode, global::System.String mailCode, global::System.String language, global::System.String region, global::System.String pageCode, global::System.DateTime stamp, global::System.String headerFooterCode, global::System.String costCentreCode, global::System.String countryCode, global::System.String accessCode, global::System.String name2, global::System.Int64 eticket, global::System.Int64 creditNote, global::System.Int64 itinerary, global::System.Int64 d2C)
        {
            qi_company qi_company = new qi_company();
            qi_company.PrimaryCode = primaryCode;
            qi_company.Pseudo = pseudo;
            qi_company.SecondaryCode = secondaryCode;
            qi_company.Name = name;
            qi_company.AddressCode = addressCode;
            qi_company.PhoneCode = phoneCode;
            qi_company.MacroCode = macroCode;
            qi_company.MailCode = mailCode;
            qi_company.Language = language;
            qi_company.Region = region;
            qi_company.PageCode = pageCode;
            qi_company.Stamp = stamp;
            qi_company.HeaderFooterCode = headerFooterCode;
            qi_company.CostCentreCode = costCentreCode;
            qi_company.CountryCode = countryCode;
            qi_company.AccessCode = accessCode;
            qi_company.Name2 = name2;
            qi_company.Eticket = eticket;
            qi_company.CreditNote = creditNote;
            qi_company.Itinerary = itinerary;
            qi_company.D2C = d2C;
            return qi_company;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String PrimaryCode
        {
            get
            {
                return _PrimaryCode;
            }
            set
            {
                if (_PrimaryCode != value)
                {
                    OnPrimaryCodeChanging(value);
                    ReportPropertyChanging("PrimaryCode");
                    _PrimaryCode = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("PrimaryCode");
                    OnPrimaryCodeChanged();
                }
            }
        }
        private global::System.String _PrimaryCode;
        partial void OnPrimaryCodeChanging(global::System.String value);
        partial void OnPrimaryCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Pseudo
        {
            get
            {
                return _Pseudo;
            }
            set
            {
                if (_Pseudo != value)
                {
                    OnPseudoChanging(value);
                    ReportPropertyChanging("Pseudo");
                    _Pseudo = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("Pseudo");
                    OnPseudoChanged();
                }
            }
        }
        private global::System.String _Pseudo;
        partial void OnPseudoChanging(global::System.String value);
        partial void OnPseudoChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String SecondaryCode
        {
            get
            {
                return _SecondaryCode;
            }
            set
            {
                if (_SecondaryCode != value)
                {
                    OnSecondaryCodeChanging(value);
                    ReportPropertyChanging("SecondaryCode");
                    _SecondaryCode = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("SecondaryCode");
                    OnSecondaryCodeChanged();
                }
            }
        }
        private global::System.String _SecondaryCode;
        partial void OnSecondaryCodeChanging(global::System.String value);
        partial void OnSecondaryCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String AddressCode
        {
            get
            {
                return _AddressCode;
            }
            set
            {
                OnAddressCodeChanging(value);
                ReportPropertyChanging("AddressCode");
                _AddressCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("AddressCode");
                OnAddressCodeChanged();
            }
        }
        private global::System.String _AddressCode;
        partial void OnAddressCodeChanging(global::System.String value);
        partial void OnAddressCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String PhoneCode
        {
            get
            {
                return _PhoneCode;
            }
            set
            {
                OnPhoneCodeChanging(value);
                ReportPropertyChanging("PhoneCode");
                _PhoneCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("PhoneCode");
                OnPhoneCodeChanged();
            }
        }
        private global::System.String _PhoneCode;
        partial void OnPhoneCodeChanging(global::System.String value);
        partial void OnPhoneCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String MacroCode
        {
            get
            {
                return _MacroCode;
            }
            set
            {
                OnMacroCodeChanging(value);
                ReportPropertyChanging("MacroCode");
                _MacroCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("MacroCode");
                OnMacroCodeChanged();
            }
        }
        private global::System.String _MacroCode;
        partial void OnMacroCodeChanging(global::System.String value);
        partial void OnMacroCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String MailCode
        {
            get
            {
                return _MailCode;
            }
            set
            {
                OnMailCodeChanging(value);
                ReportPropertyChanging("MailCode");
                _MailCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("MailCode");
                OnMailCodeChanged();
            }
        }
        private global::System.String _MailCode;
        partial void OnMailCodeChanging(global::System.String value);
        partial void OnMailCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Language
        {
            get
            {
                return _Language;
            }
            set
            {
                OnLanguageChanging(value);
                ReportPropertyChanging("Language");
                _Language = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Language");
                OnLanguageChanged();
            }
        }
        private global::System.String _Language;
        partial void OnLanguageChanging(global::System.String value);
        partial void OnLanguageChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Region
        {
            get
            {
                return _Region;
            }
            set
            {
                OnRegionChanging(value);
                ReportPropertyChanging("Region");
                _Region = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Region");
                OnRegionChanged();
            }
        }
        private global::System.String _Region;
        partial void OnRegionChanging(global::System.String value);
        partial void OnRegionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String PageCode
        {
            get
            {
                return _PageCode;
            }
            set
            {
                OnPageCodeChanging(value);
                ReportPropertyChanging("PageCode");
                _PageCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("PageCode");
                OnPageCodeChanged();
            }
        }
        private global::System.String _PageCode;
        partial void OnPageCodeChanging(global::System.String value);
        partial void OnPageCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime Stamp
        {
            get
            {
                return _Stamp;
            }
            set
            {
                OnStampChanging(value);
                ReportPropertyChanging("Stamp");
                _Stamp = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Stamp");
                OnStampChanged();
            }
        }
        private global::System.DateTime _Stamp;
        partial void OnStampChanging(global::System.DateTime value);
        partial void OnStampChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String HeaderFooterCode
        {
            get
            {
                return _HeaderFooterCode;
            }
            set
            {
                OnHeaderFooterCodeChanging(value);
                ReportPropertyChanging("HeaderFooterCode");
                _HeaderFooterCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("HeaderFooterCode");
                OnHeaderFooterCodeChanged();
            }
        }
        private global::System.String _HeaderFooterCode;
        partial void OnHeaderFooterCodeChanging(global::System.String value);
        partial void OnHeaderFooterCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String CostCentreCode
        {
            get
            {
                return _CostCentreCode;
            }
            set
            {
                OnCostCentreCodeChanging(value);
                ReportPropertyChanging("CostCentreCode");
                _CostCentreCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("CostCentreCode");
                OnCostCentreCodeChanged();
            }
        }
        private global::System.String _CostCentreCode;
        partial void OnCostCentreCodeChanging(global::System.String value);
        partial void OnCostCentreCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String CountryCode
        {
            get
            {
                return _CountryCode;
            }
            set
            {
                OnCountryCodeChanging(value);
                ReportPropertyChanging("CountryCode");
                _CountryCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("CountryCode");
                OnCountryCodeChanged();
            }
        }
        private global::System.String _CountryCode;
        partial void OnCountryCodeChanging(global::System.String value);
        partial void OnCountryCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String AccessCode
        {
            get
            {
                return _AccessCode;
            }
            set
            {
                OnAccessCodeChanging(value);
                ReportPropertyChanging("AccessCode");
                _AccessCode = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("AccessCode");
                OnAccessCodeChanged();
            }
        }
        private global::System.String _AccessCode;
        partial void OnAccessCodeChanging(global::System.String value);
        partial void OnAccessCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name2
        {
            get
            {
                return _Name2;
            }
            set
            {
                OnName2Changing(value);
                ReportPropertyChanging("Name2");
                _Name2 = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name2");
                OnName2Changed();
            }
        }
        private global::System.String _Name2;
        partial void OnName2Changing(global::System.String value);
        partial void OnName2Changed();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 Eticket
        {
            get
            {
                return _Eticket;
            }
            set
            {
                OnEticketChanging(value);
                ReportPropertyChanging("Eticket");
                _Eticket = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Eticket");
                OnEticketChanged();
            }
        }
        private global::System.Int64 _Eticket;
        partial void OnEticketChanging(global::System.Int64 value);
        partial void OnEticketChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Email
        {
            get
            {
                return _Email;
            }
            set
            {
                OnEmailChanging(value);
                ReportPropertyChanging("Email");
                _Email = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Email");
                OnEmailChanged();
            }
        }
        private global::System.String _Email;
        partial void OnEmailChanging(global::System.String value);
        partial void OnEmailChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> Invoice
        {
            get
            {
                return _Invoice;
            }
            set
            {
                OnInvoiceChanging(value);
                ReportPropertyChanging("Invoice");
                _Invoice = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Invoice");
                OnInvoiceChanged();
            }
        }
        private Nullable<global::System.Int64> _Invoice;
        partial void OnInvoiceChanging(Nullable<global::System.Int64> value);
        partial void OnInvoiceChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 CreditNote
        {
            get
            {
                return _CreditNote;
            }
            set
            {
                OnCreditNoteChanging(value);
                ReportPropertyChanging("CreditNote");
                _CreditNote = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreditNote");
                OnCreditNoteChanged();
            }
        }
        private global::System.Int64 _CreditNote;
        partial void OnCreditNoteChanging(global::System.Int64 value);
        partial void OnCreditNoteChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                OnUserNameChanging(value);
                ReportPropertyChanging("UserName");
                _UserName = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("UserName");
                OnUserNameChanged();
            }
        }
        private global::System.String _UserName;
        partial void OnUserNameChanging(global::System.String value);
        partial void OnUserNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 Itinerary
        {
            get
            {
                return _Itinerary;
            }
            set
            {
                OnItineraryChanging(value);
                ReportPropertyChanging("Itinerary");
                _Itinerary = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Itinerary");
                OnItineraryChanged();
            }
        }
        private global::System.Int64 _Itinerary;
        partial void OnItineraryChanging(global::System.Int64 value);
        partial void OnItineraryChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 D2C
        {
            get
            {
                return _D2C;
            }
            set
            {
                OnD2CChanging(value);
                ReportPropertyChanging("D2C");
                _D2C = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("D2C");
                OnD2CChanged();
            }
        }
        private global::System.Int64 _D2C;
        partial void OnD2CChanging(global::System.Int64 value);
        partial void OnD2CChanged();

        #endregion
    
    }

    #endregion
    
}
