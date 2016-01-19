namespace FtpHttpClipboarder
{
  using System;
  using System.Configuration;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Net;
  using System.Windows.Forms;

  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      this.FormBorderStyle = FormBorderStyle.None;
      this.Width = 0;
      this.Height = 0;
      this.Left = -50000;
      this.Top = -50000;
      this.Opacity = 0;
      this.Visible = false;
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
      try
      {
        this.DoWork();
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }

      this.Close();
    }

    private void DoWork()
    {
      var guid = GetNewShortGuid();
      var char0 = guid[0];
      var char1 = guid[1];
      var tmpFilePath = null as string;
      var resourceName = char0 + "/" + char1 + "/";

      // is there any image in clipboard?
      var image = Clipboard.GetImage();
      if (image != null)
      {
        tmpFilePath = GetImageFilePath(guid, image);
        resourceName += guid + ".png";
      }
      else
      {
        tmpFilePath = GetClipboardFilePath();
        resourceName += Path.GetFileName(tmpFilePath) + "." + guid + GetExtension(tmpFilePath);
      }

      UploadFile(resourceName, tmpFilePath);

      // put a link of the file to the clipboard
      var url = GetUrl("http", resourceName);
      Clipboard.SetText(url);
    }

    private string GetExtension(string tmpFilePath)
    {
      var ext = Path.GetExtension(tmpFilePath).ToLowerInvariant();
      switch (ext)
      {
        case ".bat":
        case ".xslt":
        case ".cshtml":
        case ".sln":
        case ".csproj":
        case ".vbproj":
        case ".vsproj":
        case ".sql":
        case ".ascx":
        case ".aspx":
        case ".asmx":
        case ".ashx":
        case ".asax":
        case ".cs":
        case ".vb":
        case ".config":
        case ".xml":
        case ".manifest":
        case ".publishsettings":
        case ".ini":
          return ".txt";

        case ".pdf":
        case ".txt":
        case ".js":
        case ".zip":
        case ".dll":
        case ".7z":
        case ".exe":
        case ".png":
        case ".gif":
        case ".jpg":
        case ".xls":
        case ".xlsx":
        case ".doc":
        case ".docx":
          return ext;

        case ".nupkg":
        case ".mdf":
        case ".ldf":
        case ".saz":
        case ".resx":
        default:
          return ".bin";
      }
    }

    private static string GetClipboardFilePath()
    {
      string tmpFilePath;
      var list = Clipboard.GetFileDropList();
      if (list.Count != 1)
      {
        throw new InvalidOperationException("More than one file is selected");
      }

      tmpFilePath = list[0];

      return tmpFilePath;
    }

    private static string GetImageFilePath(string guid, Image image)
    {
      // save pic to a temp folder
      string tmpFilePath = Path.Combine(Path.GetTempPath(), guid + ".png");
      image.Save(tmpFilePath, ImageFormat.Png);
      return tmpFilePath;
    }

    private void UploadFile(string name, string path)
    {
      var username = ConfigurationManager.AppSettings["username"];
      if (string.IsNullOrWhiteSpace(username))
      {
        MessageBox.Show("The application is not configured, modify the FtpHttpClipboarder.exe.config file to make it work");

        throw new ConfigurationErrorsException("Application is not configured");
      }

      var password = ConfigurationManager.AppSettings["password"];
      if (string.IsNullOrWhiteSpace(password))
      {
        MessageBox.Show("The application is not configured, modify the FtpHttpClipboarder.exe.config file to make it work");

        throw new ConfigurationErrorsException("Application is not configured");
      }

      using (var client = new WebClient())
      {
        client.Credentials = new NetworkCredential(username, password);
        client.UploadFile(GetUrl("ftp", name), "STOR", path);
      }
    }

    private static string GetUrl(string protocol, string name)
    {
      return string.Format("{0}://dl.sitecore.net/updater/share/{1}".Replace(" ", "%20"), protocol, name);
    }

    private static string GetNewShortGuid()
    {
      return Guid.NewGuid().ToString().Replace("-", string.Empty).Trim(new[] { '{', '}' }).ToUpper();
    }

    private void HandleError(Exception ex, string type = null)
    {
      var message = string.Format(
        "{0} {1} {2}{3}{4}{3}",
        DateTime.Now,
        type ?? "ERROR",
        ex.Message,
        Environment.NewLine,
        ex.StackTrace
      );

      File.AppendAllText("FtpHttpClipboarder.log", message);
    }
  }
}
