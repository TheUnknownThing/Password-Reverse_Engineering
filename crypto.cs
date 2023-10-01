using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public static class EncryptionUtility
{
    // 对应源代码中的 jfglzs-Class6-smethod_0-4 的实现
    public static void Main()
    {
        // select which mode: Encrypt or Decrypt
        Console.WriteLine("Please select mode: 1 for HashInput and Encrypt, 2 for Decrypt, 3 for Encrypt");
        string mode = Console.ReadLine();
        if (mode == "1")
        {
            Console.WriteLine("Please input the string you want to hash:");
            string input = Console.ReadLine();
            string encrypted = Encrypt(input);
            Console.WriteLine("The encrypted string is: " + encrypted);
            string hashed = HashInput(encrypted);
            Console.WriteLine("The hashed string is: " + hashed);
        }
        else if (mode == "2")
        {
            Console.WriteLine("Please input the string you want to decrypt:");
            string input = Console.ReadLine();
            string decrypted = Decrypt(input);
            Console.WriteLine("The decrypted string is: " + decrypted);
        }
        else if (mode == "3")
        {
            Console.WriteLine("Please input the string you want to encrypt:");
            string input = Console.ReadLine();
            string encrypted = Encrypt(input);
            Console.WriteLine("The encrypted string is: " + encrypted);
        }
        else
        {
            Console.WriteLine("Invalid input, please try again.");
        }
    }
    public static string Encrypt(string plainText)
    {
        string key = GetWindowsDirectoryKey();
        string iv = GetWindowsDirectoryIV();

        DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
        {
            Key = Encoding.UTF8.GetBytes(key),
            IV = Encoding.UTF8.GetBytes(iv)
        };

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, desProvider.CreateEncryptor(), CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter(cryptoStream);

        streamWriter.Write(plainText);
        streamWriter.Flush();
        cryptoStream.FlushFinalBlock();
        memoryStream.Flush();

        string encryptedBase64 = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        return EncodeString(encryptedBase64);
    }

    public static string Decrypt(string encryptedText)
    {
        string key = GetWindowsDirectoryKey();
        string iv = GetWindowsDirectoryIV();
        string processedEncryptedText = DecodeString(encryptedText);

        DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
        {
            Key = Encoding.UTF8.GetBytes(key),
            IV = Encoding.UTF8.GetBytes(iv)
        };

        Console.WriteLine("The key is: " + key);
        Console.WriteLine("The iv is: " + iv);

        byte[] encryptedBytes = Convert.FromBase64String(processedEncryptedText);
        MemoryStream memoryStream = new MemoryStream(encryptedBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, desProvider.CreateDecryptor(), CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }

    private static string GetWindowsDirectoryKey()
    {
        return "C:\\WINDOWS".Substring(0, 8);
    }

    private static string GetWindowsDirectoryIV()
    {
        return "C:\\WINDOWS".Substring(1, 8);
    }

    public static string DecodeString(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in input)
        {
            char decodedChar = (char)(c + 10);
            result.Append(decodedChar);
        }
        Console.WriteLine(result.ToString());
        return result.ToString();
    }

    public static string EncodeString(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in input)
        {
            char encodedChar = (char)(c - 10);
            result.Append(encodedChar);
        }

        return result.ToString();
    }

    public static string HashInput(string input)
    {
        // Compute the MD5 hash of the input string
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));

        // Convert the hash to a hexadecimal string
        StringBuilder hashString = new StringBuilder();
        foreach (byte b in hash)
        {
            hashString.Append(b.ToString("x2"));
        }
        Console.WriteLine(hashString.ToString());
        // Extract a substring from the hash string
        string result = hashString.ToString().Substring(10, 20);

        return result;
    }

}

