
using System;
using System.Diagnostics;
using System.IO;

namespace RiseLauncher
{
  internal class UtilJava
  {
    public static string getJavaMainFolderPath() => UtilJava.getLauncherFolderPath() + Path.DirectorySeparatorChar.ToString() + "java" + Path.DirectorySeparatorChar.ToString();

    public static string getJavaFolderPath()
    {
      string str = UtilJava.getLauncherFolderPath() + Path.DirectorySeparatorChar.ToString() + "java";
      return (!Environment.Is64BitOperatingSystem ? str + Path.DirectorySeparatorChar.ToString() + FormMain.getSelectedJavaType() + "-x32" : str + Path.DirectorySeparatorChar.ToString() + FormMain.getSelectedJavaType() + "-x64") + Path.DirectorySeparatorChar.ToString();
    }

    public static string getJavaExePath()
    {
      string str = UtilJava.getLauncherFolderPath() + Path.DirectorySeparatorChar.ToString() + "java";
      string[] strArray = new string[5]
      {
        !Environment.Is64BitOperatingSystem ? str + Path.DirectorySeparatorChar.ToString() + FormMain.getSelectedJavaType() + "-x32" : str + Path.DirectorySeparatorChar.ToString() + FormMain.getSelectedJavaType() + "-x64",
        null,
        null,
        null,
        null
      };
      char directorySeparatorChar = Path.DirectorySeparatorChar;
      strArray[1] = directorySeparatorChar.ToString();
      strArray[2] = "bin";
      directorySeparatorChar = Path.DirectorySeparatorChar;
      strArray[3] = directorySeparatorChar.ToString();
      strArray[4] = "java.exe";
      return string.Concat(strArray);
    }

    public static string getJavaWExePath()
    {
      string str = UtilJava.getLauncherFolderPath() + Path.DirectorySeparatorChar.ToString() + "java";
      string[] strArray = new string[5]
      {
        !Environment.Is64BitOperatingSystem ? str + Path.DirectorySeparatorChar.ToString() + FormMain.getSelectedJavaType() + "-x32" : str + Path.DirectorySeparatorChar.ToString() + FormMain.getSelectedJavaType() + "-x64",
        null,
        null,
        null,
        null
      };
      char directorySeparatorChar = Path.DirectorySeparatorChar;
      strArray[1] = directorySeparatorChar.ToString();
      strArray[2] = "bin";
      directorySeparatorChar = Path.DirectorySeparatorChar;
      strArray[3] = directorySeparatorChar.ToString();
      strArray[4] = "java.exe";
      return string.Concat(strArray);
    }

    public static string getLauncherFolderPath()
    {
      string str = ".craftrise";
      return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar.ToString() + str + Path.DirectorySeparatorChar.ToString();
    }

    public static string getCurrentJavaVersion()
    {
      try
      {
        string str = (string) null;
        Process process = Process.Start(new ProcessStartInfo()
        {
          FileName = UtilJava.getJavaExePath(),
          Arguments = " -version",
          CreateNoWindow = true,
          RedirectStandardError = true,
          UseShellExecute = false
        });
        while (!process.StandardError.EndOfStream)
        {
          string lower = process.StandardError.ReadLine().ToLower();
          if (lower.StartsWith("java version \""))
          {
            str = lower.Split(' ')[2].Replace("\"", "");
            break;
          }
        }
        process.WaitForExit();
        return str;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }
  }
}
