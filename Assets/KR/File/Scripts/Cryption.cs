using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System;
namespace KR
{
	public class Cryptography
	{



		static private byte[] cipherTextBytes, keyBytes, plainTextBytes;
		static private RijndaelManaged symmetricKey;
		static private ICryptoTransform decryptor, encryptor;
		static private MemoryStream memoryStream;
		static private CryptoStream cryptoStream;
		//		static private BinaryFormatter bf = new BinaryFormatter();
		public static string Decrypt(string encryptedText)
		{
			cipherTextBytes = Convert.FromBase64String(encryptedText);
			keyBytes = new Rfc2898DeriveBytes(Device.instance.passwordHash, Encoding.ASCII.GetBytes(Device.instance.saltKey)).GetBytes(256 / 8);
			symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

			decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(Device.instance.viKey));
			memoryStream = new MemoryStream(cipherTextBytes);
			cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			plainTextBytes = new byte[cipherTextBytes.Length];

			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
		}



		public static string Encrypt(string input)
		{
			plainTextBytes = Encoding.UTF8.GetBytes(input);
			keyBytes = new Rfc2898DeriveBytes(Device.instance.passwordHash, Encoding.ASCII.GetBytes(Device.instance.saltKey)).GetBytes(256 / 8);
			symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
			encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(Device.instance.viKey));

			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			return Convert.ToBase64String(cipherTextBytes);
		}
	}
}
