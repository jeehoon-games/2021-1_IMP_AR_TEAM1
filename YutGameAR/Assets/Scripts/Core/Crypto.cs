using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Core;
using UnityEngine;

public class Crypto
{
    #region hash methods
    
    public static string MD5hash(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();

        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(str));
        byte[] hash = md5.Hash;

        StringBuilder sBuilder = new StringBuilder();
        foreach (byte bt in hash)
        {
            sBuilder.Append(bt.ToString("x2"));
        }

        return sBuilder.ToString();
    }
    
    
    public static string SHAhash(string str, string type)
    {
        byte[] hash = null;
        StringBuilder result = new StringBuilder();
        
        switch (type)
        {
            case "sha256":
                SHA256 sha256 = new SHA256Managed();
                hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(str));
                break;
            case "sha512":
                SHA512 sha512 = new SHA512Managed();
                hash = sha512.ComputeHash(Encoding.ASCII.GetBytes(str));
                break;
            default:
                break;
        }

        foreach (byte bt in hash)
        {
            result.AppendFormat("{0:x2}", bt);
        }
        return result.ToString();
    }
    
    #endregion
}
