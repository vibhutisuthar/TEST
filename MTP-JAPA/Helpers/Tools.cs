using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

	public sealed class CustomEncryption
	{
		private static string key = "wozwoz" ;

		public CustomEncryption()
		{
		}

		public static string Encrypt(string plainText )
		{
			string encrypted = null;
			try
			{
				byte[] inputBytes = ASCIIEncoding.ASCII.GetBytes(plainText);
				byte[] pwdhash = null;
				MD5CryptoServiceProvider hashmd5;

				//generate an MD5 hash from the password. 
				//a hash is a one way encryption meaning once you generate
				//the hash, you cant derive the password back from it.
				hashmd5 = new MD5CryptoServiceProvider();
				pwdhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
				hashmd5 = null;

				// Create a new TripleDES service provider 
				TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();
				tdesProvider.Key = pwdhash;
				tdesProvider.Mode = CipherMode.ECB;
				byte [] edata = tdesProvider.CreateEncryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length);
				encrypted = Convert.ToBase64String(	edata );
					
			}
			catch(Exception e)
			{
				string str = e.Message;
				throw ;
			}
			return encrypted;
		}

		public static string Decrypt(string encryptedString)
		{
			string decyprted = null;
			byte[] inputBytes = null;

			try
			{
				inputBytes = Convert.FromBase64String(encryptedString);
				byte[] pwdhash = null;
				MD5CryptoServiceProvider hashmd5;

				//generate an MD5 hash from the password. 
				//a hash is a one way encryption meaning once you generate
				//the hash, you cant derive the password back from it.
				hashmd5 = new MD5CryptoServiceProvider();
				pwdhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
				hashmd5 = null;

				// Create a new TripleDES service provider 
				TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();
				tdesProvider.Key = pwdhash;
				tdesProvider.Mode = CipherMode.ECB;

				decyprted = ASCIIEncoding.ASCII.GetString(
					tdesProvider.CreateDecryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));
			}
			catch(Exception e)
			{
				string str = e.Message;
				throw ;
			}
			return decyprted;
		}
	}
