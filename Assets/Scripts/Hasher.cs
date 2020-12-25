using System;
using System.Text;
using System.Security.Cryptography;

public static class Hasher
{
    private const uint FNV_offset_basis32 = 2166136261;
    private const uint FNV_prime32 = 16777619;
    private const ulong FNV_offset_basis64 = 14695981039346656037;
    private const ulong FNV_prime64 = 1099511628211;

    public static byte[] GenerateSalt()
    {
        byte[] data = new byte[6];
        new RNGCryptoServiceProvider().GetBytes(data);
        return data;
    }
    public static byte[] HashPassword(string password, byte[] salt)
    {
        string tmp_password = null;

        tmp_password = password + Convert.ToBase64String(salt);
        UTF8Encoding converter = new UTF8Encoding();
        byte[] b_password = converter.GetBytes(tmp_password);
        return new SHA256Managed().ComputeHash(b_password);
    }
    public static string Encode(byte[] data)
    {
        return Convert.ToBase64String(data);
    }
    public static byte[] Decode(string data)
    {
        return Convert.FromBase64String(data);
 
    }
    public static uint GetStableHash32(this string txt)
    {
        unchecked
        {
            uint hash = FNV_offset_basis32;
            for (int i = 0; i < txt.Length; i++)
            {
                uint ch = txt[i];
                hash = hash * FNV_prime32;
                hash = hash ^ ch;
            }
            return hash;
        }
    }
    public static ulong GetStableHash64(this string txt)
    {
        unchecked
        {
            ulong hash = FNV_offset_basis64;
            for (int i = 0; i < txt.Length; i++)
            {
                ulong ch = txt[i];
                hash = hash * FNV_prime64;
                hash = hash ^ ch;
            }
            return hash;
        }
    }
}