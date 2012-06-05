using System;

	/// <summary>
	/// Summary description for Credentials.
	/// </summary>
	public class Credentials
	{
		string m_PSW = "";
		string m_User = "";
		string m_CRS = "";
		string m_URL = "";
		string m_ItineraryURL = "";
		string m_HAP = "";

		public Credentials()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public string User
		{
			get
			{
				return m_User;
			}
			set
			{
				m_User = value;
			}
		}
		public string CRS
		{
			get
			{
				return m_CRS;
			}
			set
			{
				m_CRS = value;
			}
		}
		public string ItineraryURL
		{
			get
			{
				return m_ItineraryURL;
			}
			set
			{
				m_ItineraryURL = value;
			}
		}
		public string URL
		{
			get
			{
				return m_URL;
			}
			set
			{
				m_URL = value;
			}
		}
		public string PSW
		{
			get
			{
				return m_PSW;
			}
			set
			{
				m_PSW = value;
			}
		}
		public string HAP
		{
			get
			{
				return m_HAP;
			}
			set
			{
				m_HAP = value;
			}
		}

	}
