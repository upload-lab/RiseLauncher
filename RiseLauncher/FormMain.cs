
using Ionic.Zip;
using Newtonsoft.Json.Linq;
using RiseLauncher.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace RiseLauncher
{
  public class FormMain : Form
  {
    public static string API_URL = "https://client.craftrise.network/api/launcher/hashs.php";
    public static JObject API_JSON;
    public static JObject MOJANG_JSON;
    private IContainer components = (IContainer) null;
    private Label statusText;
    private PictureBox pictureBox1;

    public FormMain()
    {
      this.FormBorderStyle = FormBorderStyle.None;
      this.InitializeComponent();
    }

    public void updateText(string text)
    {
      try
      {
        if (this.statusText.InvokeRequired)
          this.statusText.Invoke((Delegate) (() => this.statusText.Text = text));
        else
          this.statusText.Text = text;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    public void start()
    {
      string content1 = this.getContent(FormMain.API_URL);
      while (content1 == null || string.IsNullOrEmpty(content1))
      {
        try
        {
          Thread.Sleep(5000);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
        content1 = this.getContent(FormMain.API_URL);
        this.updateText("CraftRise sunucularına bağlantı deneniyor. (001)");
      }
      try
      {
        FormMain.API_JSON = JObject.Parse(content1);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        this.updateText("CraftRise sunucularına bağlanılamıyor. (002)");
        return;
      }
      string content2 = this.getContent(this.getSelectedJavaURL());
      if (content2 == null || string.IsNullOrEmpty(content2))
      {
        this.updateText("CraftRise sunucularına bağlanılamıyor. (003)");
      }
      else
      {
        try
        {
          FormMain.MOJANG_JSON = JObject.Parse(content2);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
          this.updateText("CraftRise sunucularına bağlanılamıyor. (004)");
          return;
        }
        this.updateText("Java sürümü kontrol ediliyor...");
        if (!this.isJavaInstalled())
        {
          this.updateText("Java yükleniyor...");
          if (!this.DownloadJava())
          {
            this.updateText("Java yükleme işlemi başarısız, tekrar deneyin.");
          }
          else
          {
            this.updateText("Java dosyaları çıkartılıyor...");
            string cacheLzmaFile = this.getCacheLZMAFile();
            string cacheLzmaFileZip = this.getCacheLZMAFileZip();
            if (!UtilFile.DecompressLZMA(cacheLzmaFile, cacheLzmaFileZip))
            {
              this.updateText("Java kurulum işlemi başarısız, tekrar deneyin. (005)");
            }
            else
            {
              try
              {
                ZipFile zipFile = new ZipFile(cacheLzmaFileZip);
                zipFile.ExtractAll(UtilJava.getJavaFolderPath(), (ExtractExistingFileAction) 1);
                zipFile.Dispose();
              }
              catch (Exception ex)
              {
                Console.WriteLine(ex.ToString());
                this.updateText("Java kurulum işlemi başarısız, tekrar deneyin. (006)");
                return;
              }
              if (System.IO.File.Exists(this.getCacheLZMAFile()))
                System.IO.File.Delete(this.getCacheLZMAFile());
              if (System.IO.File.Exists(this.getCacheLZMAFileZip()))
                System.IO.File.Delete(this.getCacheLZMAFileZip());
              this.updateText("Java kuruldu, sürüm kontrolü yapılıyor...");
              string currentJavaVersion = UtilJava.getCurrentJavaVersion();
              string mojangJavaVersion = this.getMojangJavaVersion();
              if (currentJavaVersion == null || !currentJavaVersion.Equals(mojangJavaVersion))
                this.updateText("Sürüm kontrolü başarısız, sürüm geçersiz. (007)");
              this.checkLauncherFile();
            }
          }
        }
        else
          this.checkLauncherFile();
      }
    }

    private void checkLauncherFile()
    {
      this.updateText("Launcher için güncelleme kontrolü yapılıyor...");
      string webLauncherHash = this.getWebLauncherHash();
      if (!UtilFile.getHashFile(this.getLauncherFile()).Equals(webLauncherHash) && !this.DownloadLauncherJAR())
        this.updateText("Launcher indirilemedi, tekrar deneyin.");
      else
        this.startLauncher();
    }

    private void startLauncher()
    {
      this.updateText("Launcher başlatılıyor...");
      int num = 512;
      try
      {
        int ram = UtilRAM.getRam();
        if (ram > 2)
        {
          num = ram * 256;
          if (num > 4096)
            num = 4096;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      string str = this.getStartArguments();
      try
      {
        str = str.Replace("%selectedRAM%", num.ToString());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        this.updateText("RAM değerleri güncellenemedi, bir hata mevcut.");
      }
      try
      {
        Process.Start(new ProcessStartInfo()
        {
          FileName = UtilJava.getJavaWExePath(),
          Arguments = str + " -jar launcher.jar launcherStartup",
          WorkingDirectory = UtilJava.getLauncherFolderPath(),
          UseShellExecute = false,
          CreateNoWindow = true
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        this.updateText("Launcher başlatılamadı, tekrar deneyin.");
        return;
      }
      Thread.Sleep(1000);
      Environment.Exit(0);
    }

    private string getWebLauncherHash() => JToken.op_Explicit(((JObject) FormMain.API_JSON.GetValue("MAIN")).GetValue("launcher.jar"));

    private string getWebLauncherURL() => JToken.op_Explicit(((JObject) FormMain.API_JSON.GetValue("WINDOWS_BS")).GetValue("launcherURL"));

    public static string getSelectedJavaType() => JToken.op_Explicit(((JObject) FormMain.API_JSON.GetValue("WINDOWS_BS")).GetValue("javaType"));

    private string getSelectedJavaURL() => JToken.op_Explicit(((JObject) FormMain.API_JSON.GetValue("WINDOWS_BS")).GetValue("javaURL"));

    private string getStartArguments() => JToken.op_Explicit(((JObject) FormMain.API_JSON.GetValue("WINDOWS_BS")).GetValue("startArguments"));

    private string getContent(string URL)
    {
      try
      {
        return new WebClient().DownloadString(URL);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return (string) null;
      }
    }

    private string getMojangJavaURL() => JToken.op_Explicit(((JObject) ((JObject) ((JObject) FormMain.MOJANG_JSON.GetValue("windows")).GetValue(Environment.Is64BitOperatingSystem ? "64" : "32")).GetValue(FormMain.getSelectedJavaType())).GetValue("url"));

    private string getMojangJavaKey() => JToken.op_Explicit(((JObject) ((JObject) ((JObject) FormMain.MOJANG_JSON.GetValue("windows")).GetValue(Environment.Is64BitOperatingSystem ? "64" : "32")).GetValue(FormMain.getSelectedJavaType())).GetValue("sha1"));

    private string getMojangJavaVersion() => JToken.op_Explicit(((JObject) ((JObject) ((JObject) FormMain.MOJANG_JSON.GetValue("windows")).GetValue(Environment.Is64BitOperatingSystem ? "64" : "32")).GetValue(FormMain.getSelectedJavaType())).GetValue("version"));

    private bool isJavaInstalled()
    {
      string mojangJavaVersion = this.getMojangJavaVersion();
      string currentJavaVersion = UtilJava.getCurrentJavaVersion();
      return currentJavaVersion != null && currentJavaVersion.Equals(mojangJavaVersion);
    }

    private string getLauncherFile() => UtilJava.getLauncherFolderPath() + "launcher.jar";

    private string getCacheLZMAFile() => UtilJava.getLauncherFolderPath() + "java.lzma";

    private string getCacheLZMAFileZip() => UtilJava.getLauncherFolderPath() + "java.zip";

    private bool DownloadJava()
    {
      try
      {
        string cacheLzmaFile = this.getCacheLZMAFile();
        try
        {
          if (System.IO.File.Exists(cacheLzmaFile))
            System.IO.File.Delete(cacheLzmaFile);
          if (System.IO.File.Exists(this.getCacheLZMAFileZip()))
            System.IO.File.Delete(this.getCacheLZMAFileZip());
          new DirectoryInfo(UtilJava.getJavaMainFolderPath()).ClearFolder();
          if (!Directory.Exists(UtilJava.getJavaMainFolderPath()))
            Directory.CreateDirectory(UtilJava.getJavaMainFolderPath());
          if (!Directory.Exists(UtilJava.getJavaFolderPath()))
            Directory.CreateDirectory(UtilJava.getJavaFolderPath());
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
        this.updateText("Java indiriliyor...");
        using (WebClient webClient = new WebClient())
        {
          Uri address = new Uri(this.getMojangJavaURL());
          webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.javaDownload);
          webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.WebClientDownloadCompleted);
          webClient.DownloadFile(address, cacheLzmaFile);
          if (!System.IO.File.Exists(cacheLzmaFile))
            return false;
          this.updateText("Java indirildi, dosya kontrol ediliyor...");
          if (UtilFile.getHashFile(this.getCacheLZMAFile()).Equals(this.getMojangJavaKey()))
            return true;
          this.updateText("Java indirilemedi, lütfen tekrar deneyin.");
          return false;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return false;
      }
    }

    private bool DownloadLauncherJAR()
    {
      try
      {
        string launcherFile = this.getLauncherFile();
        if (System.IO.File.Exists(launcherFile))
          System.IO.File.Delete(launcherFile);
        this.updateText("Launcher indiriliyor...");
        using (WebClient webClient = new WebClient())
        {
          Uri address = new Uri(this.getWebLauncherURL());
          webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.LauncherDownload);
          webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.WebClientDownloadCompleted);
          webClient.DownloadFile(address, launcherFile);
          if (!System.IO.File.Exists(launcherFile))
            return false;
          this.updateText("Launcher indirildi, dosya kontrol ediliyor...");
          if (UtilFile.getHashFile(this.getLauncherFile()).Equals(this.getWebLauncherHash()))
            return true;
          this.updateText("Launcher indirilemedi, lütfen tekrar deneyin.");
          return false;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return false;
      }
    }

    private void javaDownload(object sender, DownloadProgressChangedEventArgs e) => this.updateText("Java indiriliyor... (%" + e.ProgressPercentage.ToString() + ")");

    private void LauncherDownload(object sender, DownloadProgressChangedEventArgs e) => this.updateText("Launcher indiriliyor... (%" + e.ProgressPercentage.ToString() + ")");

    private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
    {
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (FormMain));
      this.statusText = new Label();
      this.pictureBox1 = new PictureBox();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.statusText.Font = new Font("Arial", 12f);
      this.statusText.ForeColor = SystemColors.ButtonHighlight;
      this.statusText.Location = new Point(12, 277);
      this.statusText.Name = "statusText";
      this.statusText.Size = new Size(360, 28);
      this.statusText.TabIndex = 0;
      this.statusText.Text = "Java sürümü kontrol ediliyor...";
      this.statusText.TextAlign = ContentAlignment.MiddleCenter;
      this.statusText.UseCompatibleTextRendering = true;
      this.pictureBox1.Image = (Image) Resources.logo_215;
      this.pictureBox1.Location = new Point(112, 61);
      this.pictureBox1.Margin = new Padding(0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(158, 170);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Click += new EventHandler(this.pictureBox1_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 38);
      this.ClientSize = new Size(384, 361);
      this.ControlBox = false;
      this.Controls.Add((Control) this.pictureBox1);
      this.Controls.Add((Control) this.statusText);
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = nameof (FormMain);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "CraftRise Launcher";
      this.TopMost = true;
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
    }
  }
}
