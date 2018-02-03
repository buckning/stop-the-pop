using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Security.Cryptography;

/***
 * Code referenced from
 * http://www.fizixstudios.com/labs/do/view/id/unity-file-management-and-xml-p5
 */
public static class StringCipher
{
	private static string defaultKey = "11111111111111111111111111111111";

	public static string encryptData(string toEncrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes(defaultKey);

		// 256-AES key
		byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
		RijndaelManaged rDel = new RijndaelManaged();

		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;

		rDel.Padding = PaddingMode.PKCS7;

		ICryptoTransform cTransform = rDel.CreateEncryptor();
		byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);

		return Convert.ToBase64String (resultArray, 0, resultArray.Length);
	}

	public static string decryptData(string toDecrypt) {
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes (defaultKey);

		// AES-256 key
		byte[] toEncryptArray = Convert.FromBase64String (toDecrypt);
		RijndaelManaged rDel = new RijndaelManaged ();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;


		rDel.Padding = PaddingMode.PKCS7;

		// better lang support
		ICryptoTransform cTransform = rDel.CreateDecryptor ();

		byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);

		return UTF8Encoding.UTF8.GetString (resultArray);
	}
}
