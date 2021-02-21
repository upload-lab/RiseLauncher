using SevenZip;
using SevenZip.Compression.LZMA;
using System;
using System.IO;
using System.Security.Cryptography;

namespace RiseLauncher
{
  public static class UtilFile
  {
    public static string getHashFile(string pathName)
    {
      try
      {
        SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider();
        FileStream fileStream = UtilFile.GetFileStream(pathName);
        byte[] hash = cryptoServiceProvider.ComputeHash((Stream) fileStream);
        fileStream.Close();
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return "";
      }
    }

    private static FileStream GetFileStream(string pathName) => new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    public static void ClearFolder(this DirectoryInfo directory)
    {
      try
      {
        foreach (FileSystemInfo file in directory.GetFiles())
          file.Delete();
        foreach (DirectoryInfo directory1 in directory.GetDirectories())
          directory1.Delete(true);
      }
      catch (Exception ex)
      {
      }
    }

    public static bool DecompressLZMA(string inFile, string outFile)
    {
      try
      {
        Decoder decoder = new Decoder();
        FileStream fileStream1 = new FileStream(inFile, FileMode.Open);
        FileStream fileStream2 = new FileStream(outFile, FileMode.Create);
        byte[] buffer1 = new byte[5];
        fileStream1.Read(buffer1, 0, 5);
        byte[] buffer2 = new byte[8];
        fileStream1.Read(buffer2, 0, 8);
        long int64 = BitConverter.ToInt64(buffer2, 0);
        decoder.SetDecoderProperties(buffer1);
        decoder.Code((Stream) fileStream1, (Stream) fileStream2, fileStream1.Length, int64, (ICodeProgress) null);
        fileStream2.Flush();
        fileStream2.Close();
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return false;
      }
    }
  }
}
