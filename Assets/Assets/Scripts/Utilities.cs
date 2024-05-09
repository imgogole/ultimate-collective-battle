using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Web;
using System.Linq;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
//using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;  

public static class Utilities
{
    private static readonly string KEY = "TilesKEY";
/*
    public static string ObjectToCryptedString(object obj)
    {
        string objStr = JsonConvert.SerializeObject(obj);
        Debug.Log($"Object {obj} to JSON : {objStr}");
        string crypted = Encrypt(objStr);
        return crypted;
    }

    public static T CryptedStringToObject<T>(string id)
    {
        try
        {
            string decrypted = Decrypt(id);
            return JsonConvert.DeserializeObject<T>(decrypted);
        }
        catch
        {
            return default;
        }
    }*/

    public static string Encrypt(string clearText)
    {
        string EncryptionKey = KEY;
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string Decrypt(string cipherText)
    {
        string EncryptionKey = KEY;
        cipherText = cipherText.Replace(" ", "+");
        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return cipherText;
    }

    /// <summary>
    /// Return the remainder of the euclidian division between x and m.
    /// The result is always between 0 and m excluded.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static int Modulo(int x, int m)
    {
        return (x % m + m) % m;
    }

    /// <summary>
    /// Convert a Unity color to his hexadecimal representation.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHexCode(Color color, bool addHash = false)
    {
        return (addHash ? "#" : "") + ColorUtility.ToHtmlStringRGB(color);
    }

    /// <summary>
    /// Returns a colored representation of the ping.
    /// </summary>
    /// <param name="ping"></param>
    /// <returns></returns>
    public static string ToColorPing(int ping)
    {
        if (ping < 20) return $"<color=#42f56c>{ping}</color>";
        if (ping < 40) return $"<color=#1aff00>{ping}</color>";
        if (ping < 60) return $"<color=#14c900>{ping}</color>";
        if (ping < 80) return $"<color=#a6ff00>{ping}</color>";
        if (ping < 100) return $"<color=#d4ff00>{ping}</color>";
        if (ping < 120) return $"<color=#fff700>{ping}</color>";
        if (ping < 150) return $"<color=#ffb700>{ping}</color>";
        if (ping < 300) return $"<color=#ff6f00>{ping}</color>";
        return $"<color=#eb0000>{ping}</color>";
    }


    /// <summary>
    /// Returns true with a probability of Weigth / Prob. Else returns false.
    /// </summary>
    /// <param name="Probability"></param>
    /// <param name="Weigth"></param>
    /// <returns></returns>
    public static bool CheckProb(int Prob, int Weigth = 1)
    {
        if (Prob <= 0) return false;
        Weigth = Mathf.Abs(Weigth);

        return UnityEngine.Random.Range(0, Prob) < Weigth;
    }

    /// <summary>
    /// Returns a logarithmic random number between 0 and 1. That meens this is more probable to get 0 than 1. All depends of the precision, highest is the precision, accuratest is the result. The default base is E. 
    /// </summary>
    /// <param name="Precision"></param>
    /// <param name="Base"></param>
    /// <returns></returns>
    public static double LogarithmicRandomUnit(int Precision = 1000)
    {
        int Unit = UnityEngine.Random.Range(0, Precision) + 1;
        double LogUnit = Math.Abs(Math.Log(Unit, Precision));
        return 1 - LogUnit;
    }

    /// <summary>
    /// Get the current coordinate of an index corresponding on a table with Base.
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="Base"></param>
    /// <returns></returns>
    public static Vector2 IndexToCoords(int Index, Vector2 Base)
    {
        Vector2 coords = new Vector2(Index / Base.y, Index % Base.y);
        return coords;
    }

    public static int CoordsToIndex(Vector2 Coords, Vector2 Base)
    {
        return (int)(Coords.x * Base.x + Coords.y);
    }
}
