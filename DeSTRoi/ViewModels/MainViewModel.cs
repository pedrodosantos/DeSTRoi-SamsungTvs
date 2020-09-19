using DeSTRoi.Libraries.IO;
using DeSTRoi.Libraries.Network;
using DeSTRoi.Models;
using DeSTRoi.NonMVVMWindows;
using DeSTRoi.Properties;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;

namespace DeSTRoi.ViewModels
{
  public class MainViewModel : ObservableObject
  {
    public delegate void StringEventHandler(object sender, string data);

    private class DecryptThreadState
    {
      public enum ThreadStateEnum
      {
        Undetermined,
        Error,
        StartDecryption,
        SyncLost,
        DecryptionProgress
      }

      public enum ThreadErrorEnum
      {
        None,
        Unknown,
        InvalidFile,
        DecryptFailed
      }

      private Exception _error;

      public ThreadStateEnum State
      {
        get;
        private set;
      }

      public Exception Error
      {
        get
        {
          return _error;
        }
        private set
        {
          _error = value;
          if (_error != null && ErrorType == ThreadErrorEnum.None)
          {
            ErrorType = ThreadErrorEnum.Unknown;
          }
        }
      }

      public ThreadErrorEnum ErrorType
      {
        get;
        private set;
      }

      public object[] Data
      {
        get;
        private set;
      }

      public object this[int i]
      {
        get
        {
          if (Data == null)
          {
            throw new ArgumentNullException("Data");
          }
          if (i < 0 || i > Data.Length)
          {
            throw new ArgumentOutOfRangeException("i");
          }
          return Data[i];
        }
      }

      public DecryptThreadState()
      {
        State = ThreadStateEnum.Undetermined;
        ErrorType = ThreadErrorEnum.None;
        Error = null;
        Data = null;
      }

      public DecryptThreadState(ThreadStateEnum state)
        : this()
      {
        State = state;
      }

      public DecryptThreadState(ThreadStateEnum state, object[] data)
        : this(state)
      {
        Data = data;
      }

      public DecryptThreadState(ThreadStateEnum state, Exception error)
        : this(state)
      {
        Error = error;
      }

      public DecryptThreadState(ThreadStateEnum state, Exception error, object[] data)
        : this(state, error)
      {
        Data = data;
      }

      public DecryptThreadState(ThreadStateEnum state, Exception error, ThreadErrorEnum errType)
        : this(state)
      {
        Error = error;
        ErrorType = errType;
      }

      public DecryptThreadState(ThreadStateEnum state, Exception error, ThreadErrorEnum errType, object[] data)
        : this(state, error, errType)
      {
        Data = data;
      }
    }

    private Window _owner;

    private string _outputDirectory = "";

    private ObservableCollection<string> _networkIPs;

    private string _selectedIP;

    private ObservableCollection<EncryptedFileModel> _encryptedFiles;

    private ObservableCollection<EncryptedFileModel> _selectedEncryptedFiles;

    private bool _isIPUpdating;

    private BackgroundWorker _bwDecrypt;

    private DialogBox _dbDecrypt;

    private string BtnOk
    {
      get
      {
        string resolvedValue = "";
        new LocTextExtension("DeSTRoi:Global:BtnOK").ResolveLocalizedValue(out resolvedValue);
        return resolvedValue;
      }
    }

    private string BtnCancel
    {
      get
      {
        string resolvedValue = "";
        new LocTextExtension("DeSTRoi:Global:BtnCancel").ResolveLocalizedValue(out resolvedValue);
        return resolvedValue;
      }
    }

    private string BtnYes
    {
      get
      {
        string resolvedValue = "";
        new LocTextExtension("DeSTRoi:Global:BtnYes").ResolveLocalizedValue(out resolvedValue);
        return resolvedValue;
      }
    }

    private string BtnNo
    {
      get
      {
        string resolvedValue = "";
        new LocTextExtension("DeSTRoi:Global:BtnNo").ResolveLocalizedValue(out resolvedValue);
        return resolvedValue;
      }
    }

    private string LocalizedOf
    {
      get
      {
        string resolvedValue = "";
        new LocTextExtension("DeSTRoi:Global:of").ResolveLocalizedValue(out resolvedValue);
        return resolvedValue;
      }
    }

    public Window Owner
    {
      get
      {
        return _owner;
      }
      set
      {
        _owner = value;
      }
    }

    public string OutputDirectory
    {
      get
      {
        return _outputDirectory;
      }
      set
      {
        _outputDirectory = value;
        Settings.Default.OutputDirectory = _outputDirectory;
        Settings.Default.Save();
        RaisePropertyChanged("OutputDirectory");
      }
    }

    public string SelectedKeyRetreivalMethod
    {
      get
      {
        return Settings.Default.KeyRetMethod;
      }
      set
      {
        Settings.Default.KeyRetMethod = value;
        Settings.Default.Save();
        RaisePropertyChanged("SelectedKeyRetreivalMethod");
      }
    }

    public ObservableCollection<string> NetworkIPs => _networkIPs;

    public string SelectedIP
    {
      get
      {
        return _selectedIP;
      }
      set
      {
        _selectedIP = value;
        Settings.Default.TVIP = _selectedIP;
        Settings.Default.Save();
        RaisePropertyChanged("SelectedIP");
      }
    }

    public bool DecryptAdaptionField
    {
      get
      {
        return Settings.Default.DecryptAF;
      }
      set
      {
        Settings.Default.DecryptAF = value;
        Settings.Default.Save();
        RaisePropertyChanged("DecryptAdaptionField");
      }
    }

    public ObservableCollection<EncryptedFileModel> EncryptedFiles => _encryptedFiles;

    public ObservableCollection<EncryptedFileModel> SelectedEncryptedFiles
    {
      get
      {
        return _selectedEncryptedFiles;
      }
      set
      {
        _selectedEncryptedFiles = value;
      }
    }

    public bool ContextualTabGroupKeyVisible => _selectedEncryptedFiles.Count == 1;

    public string FTPUser
    {
      get
      {
        return Settings.Default.FTPUser;
      }
      set
      {
        Settings.Default.FTPUser = value;
        Settings.Default.Save();
        RaisePropertyChanged("FTPUser");
      }
    }

    public string FTPPwd
    {
      get
      {
        string text = "";
        try
        {
          RijndaelManaged rijndaelManaged = new RijndaelManaged();
          using (MD5 mD = MD5.Create())
          {
            rijndaelManaged.KeySize = 128;
            rijndaelManaged.Key = mD.ComputeHash(Encoding.UTF8.GetBytes(Environment.MachineName + Environment.OSVersion.Platform));
            rijndaelManaged.IV = new byte[16];
            rijndaelManaged.Padding = PaddingMode.Zeros;
          }
          using (MemoryStream memoryStream = new MemoryStream())
          {
            memoryStream.Write(Settings.Default.FTPPwd.ToByteArray(), 0, Settings.Default.FTPPwd.ToByteArray().Length);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read))
            {
              byte[] array = new byte[1024];
              while (cryptoStream.Read(array, 0, array.Length) > 0)
              {
                string str = text;
                string @string = Encoding.UTF8.GetString(array);
                char[] trimChars = new char[1];
                text = str + @string.Trim(trimChars);
                array.Initialize();
              }
              return text;
            }
          }
        }
        catch
        {
          return text;
        }
      }
      set
      {
        if (!(value != FTPPwd))
        {
          return;
        }
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        using (MD5 mD = MD5.Create())
        {
          rijndaelManaged.KeySize = 128;
          rijndaelManaged.Key = mD.ComputeHash(Encoding.UTF8.GetBytes(Environment.MachineName + Environment.OSVersion.Platform));
          rijndaelManaged.IV = new byte[16];
          rijndaelManaged.Padding = PaddingMode.Zeros;
        }
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
          {
            int num = (Encoding.UTF8.GetByteCount(value) % 16 != 0) ? (16 - Encoding.UTF8.GetByteCount(value)) : 0;
            byte[] array = new byte[Encoding.UTF8.GetByteCount(value) + num];
            Encoding.UTF8.GetBytes(value).CopyTo(array, 0);
            cryptoStream.Write(array, 0, array.Length);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            byte[] array2 = new byte[memoryStream.Length];
            memoryStream.Read(array2, 0, array2.Length);
            Settings.Default.FTPPwd = array2.ToHexString();
            Settings.Default.Save();
          }
        }
        RaisePropertyChanged("FTPPwd");
        if (this.PasswordChanged != null)
        {
          this.PasswordChanged(this, value);
        }
      }
    }

    public bool IsAnonymousFTP
    {
      get
      {
        return Settings.Default.FTPIsAnon;
      }
      set
      {
        Settings.Default.FTPIsAnon = value;
        Settings.Default.Save();
        RaisePropertyChanged("IsAnonymousFTP");
      }
    }

    public ICommand ShowAboutBoxCommand => new RelayCommand(ShowAboutBoxCommandExecute);

    public ICommand ExitCommand => new RelayCommand(ExitCommandExecute);

    public ICommand OpenLocalCommand => new RelayCommand<bool?>(OpenLocalCommandExecute);

    public ICommand OpenFTPCommand => new RelayCommand(OpenFTPCommandExecute, OpenFTPCommandCanExecute);

    public ICommand OpenKeyCommand => new RelayCommand(OpenKeyCommandExecute, OpenKeyCommandCanExecute);

    public ICommand RemoveFileCommand => new RelayCommand(RemoveFileCommandExecute, RemoveFileCommandCanExecute);

    public ICommand OpenFolderCommand => new RelayCommand<string>(OpenFolderCommandExecute);

    public ICommand RefreshIPCommand => new RelayCommand(RefreshIPCommandExecute, RefreshIPCommandCanExecute);

    public ICommand DecryptFilesCommand => new RelayCommand(DecryptFilesCommandExecute, DecryptFilesCommandCanExecute);

    public ICommand ShowDetailsCommand => new RelayCommand<EncryptedFileModel>(ShowDetailsCommandExecute, ShowDetailsCommandCanExecute);

    public event StringEventHandler PasswordChanged;

    public MainViewModel()
    {
      _encryptedFiles = new ObservableCollection<EncryptedFileModel>();
      _selectedEncryptedFiles = new ObservableCollection<EncryptedFileModel>();
      _selectedEncryptedFiles.CollectionChanged += _selectedEncryptedFiles_CollectionChanged;
      _networkIPs = new ObservableCollection<string>();
      RefreshIPCommandExecute();
      _outputDirectory = Settings.Default.OutputDirectory;
      _selectedIP = Settings.Default.TVIP;
      _bwDecrypt = new BackgroundWorker();
      _bwDecrypt.WorkerSupportsCancellation = true;
      _bwDecrypt.WorkerReportsProgress = true;
      _bwDecrypt.DoWork += _bwDecrypt_DoWork;
      _bwDecrypt.ProgressChanged += _bwDecrypt_ProgressChanged;
      _bwDecrypt.RunWorkerCompleted += _bwDecrypt_RunWorkerCompleted;
    }

    private void _selectedEncryptedFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      RaisePropertyChanged("ContextualTabGroupKeyVisible");
    }

    private void _ipScanner_NetworkSearchComplete(object sender, LocalIPScanner.NetworkSearchEventArgs e)
    {
      NetworkIPs.Clear();
      foreach (string localIP in e.LocalIPs)
      {
        NetworkIPs.Add(localIP);
      }
      _isIPUpdating = false;
      RaisePropertyChanged("RefreshIPCommand");
    }

    private void _bwDecrypt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      _dbDecrypt.Close();
      Owner.IsEnabled = true;
    }

    private void _bwDecrypt_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      DecryptThreadState decryptThreadState = e.UserState as DecryptThreadState;
      try
      {
        if (decryptThreadState == null)
        {
          return;
        }
        switch (decryptThreadState.State)
        {
          case DecryptThreadState.ThreadStateEnum.Error:
            {
              Exception ex = (decryptThreadState.Error != null) ? decryptThreadState.Error : new Exception();
              switch (decryptThreadState.ErrorType)
              {
                case DecryptThreadState.ThreadErrorEnum.Unknown:
                  {
                    string resolvedValue6 = "";
                    new LocTextExtension("DeSTRoi:Global:UnexpectedErrorMainInst").ResolveLocalizedValue(out resolvedValue6);
                    DialogBox.ShowDialog(Owner, resolvedValue6, ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
                    break;
                  }
                case DecryptThreadState.ThreadErrorEnum.InvalidFile:
                  {
                    EncryptedFileModel encryptedFileModel3 = decryptThreadState[0] as EncryptedFileModel;
                    if (encryptedFileModel3 == null)
                    {
                      throw new ArgumentNullException("currFile");
                    }
                    string resolvedValue4 = "";
                    string resolvedValue5 = "";
                    new LocTextExtension("DeSTRoi:Global:CNSMSOF").ResolveLocalizedValue(out resolvedValue4);
                    new LocTextExtension("DeSTRoi:Global:InvCorFile").ResolveLocalizedValue(out resolvedValue5);
                    DialogBox dialogBox = new DialogBox();
                    new LocTextExtension("DeSTRoi:Global:InvCorFile").SetBinding(dialogBox, DialogBox.MainInstructionProperty);
                    dialogBox.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Delete_32.png"));
                    dialogBox.IconVisible = true;
                    dialogBox.MainContent = resolvedValue4 + encryptedFileModel3.FileName + resolvedValue5;
                    dialogBox.Buttons = new DialogBox.DialogButton[1]
                    {
            new DialogBox.DialogButton(BtnOk, DialogBox.DialogButton.SpecialButtonEnum.Ok, closeDialog: true)
                    };
                    dialogBox.ShowDialog();
                    break;
                  }
                case DecryptThreadState.ThreadErrorEnum.DecryptFailed:
                  {
                    EncryptedFileModel encryptedFileModel2 = decryptThreadState[0] as EncryptedFileModel;
                    if (encryptedFileModel2 == null)
                    {
                      throw new ArgumentNullException("currFile");
                    }
                    string resolvedValue3 = "";
                    new LocTextExtension("DeSTRoi:Global:DecryptionFailed").ResolveLocalizedValue(out resolvedValue3);
                    DialogBox.ShowDialog(Owner, resolvedValue3, encryptedFileModel2.FileName + "\n" + ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
                    break;
                  }
              }
              break;
            }
          case DecryptThreadState.ThreadStateEnum.StartDecryption:
            {
              EncryptedFileModel encryptedFileModel = decryptThreadState[0] as EncryptedFileModel;
              if (encryptedFileModel == null)
              {
                throw new ArgumentNullException("currFile");
              }
              int num = Convert.ToInt32(decryptThreadState[1]);
              string resolvedValue = "";
              string resolvedValue2 = "";
              new LocTextExtension("DeSTRoi:Global:CurrentFile").ResolveLocalizedValue(out resolvedValue);
              new LocTextExtension("DeSTRoi:Global:File").ResolveLocalizedValue(out resolvedValue2);
              _dbDecrypt.MainContent = resolvedValue + encryptedFileModel.FileName;
              _dbDecrypt.ProgressBarValue = 0;
              _dbDecrypt.ExpanderHeader = resolvedValue2 + e.ProgressPercentage + LocalizedOf + num;
              _dbDecrypt.ExpanderIsExpanded = true;
              new LocTextExtension("DeSTRoi:Global:SyncStream").SetBinding(_dbDecrypt, DialogBox.ExpanderContentProperty);
              break;
            }
          case DecryptThreadState.ThreadStateEnum.DecryptionProgress:
            {
              long source = Convert.ToInt64(decryptThreadState[0]);
              long source2 = Convert.ToInt64(decryptThreadState[1]);
              int source3 = Convert.ToInt32(decryptThreadState[2]);
              _dbDecrypt.ProgressBarValue = e.ProgressPercentage;
              _dbDecrypt.ExpanderContent = FileSizeConverter.ConvertToFileSize(source) + LocalizedOf + FileSizeConverter.ConvertToFileSize(source2) + "\n" + FileSizeConverter.ConvertToFileSize(source3) + "/s";
              break;
            }
          case DecryptThreadState.ThreadStateEnum.SyncLost:
            new LocTextExtension("DeSTRoi:Global:SyncLost").SetBinding(_dbDecrypt, DialogBox.ExpanderContentProperty);
            break;
        }
      }
      catch (Exception ex2)
      {
        string resolvedValue7 = "";
        new LocTextExtension("DeSTRoi:Global:UnexpectedErrorMainInst").ResolveLocalizedValue(out resolvedValue7);
        DialogBox.ShowDialog(Owner, resolvedValue7, ex2.Message, showErrorIcon: true, "Stack Trace:", ex2.StackTrace);
      }
    }

    private void _bwDecrypt_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        EncryptedFileModel[] array = (EncryptedFileModel[])((object[])e.Argument)[0];
        string str = (string)((object[])e.Argument)[1];
        int num = 0;
        EncryptedFileModel[] array2 = array;
        foreach (EncryptedFileModel encryptedFileModel in array2)
        {
          try
          {
            if (_bwDecrypt.CancellationPending)
            {
              e.Cancel = true;
              return;
            }
            num++;
            _bwDecrypt.ReportProgress(num, new DecryptThreadState(DecryptThreadState.ThreadStateEnum.StartDecryption, new object[2]
            {
            encryptedFileModel,
            array.Length
            }));
            string path = str + "\\" + encryptedFileModel.FileName + ".ts";
            using (FileStream fileStream = new FileStream(encryptedFileModel.FilePath, FileMode.Open, FileAccess.Read))
            {
              using (FileStream fileStream2 = new FileStream(path, FileMode.Create, FileAccess.Write))
              {
                byte[] array3 = new byte[1024];
                bool flag = false;
                fileStream.Read(array3, 0, array3.Length);
                int num2 = Decryption.Sync(array3);
                if (num2 != -1)
                {
                  flag = true;
                  fileStream.Seek(num2, SeekOrigin.Begin);
                }
                if (flag)
                {
                  long num3 = 0L;
                  DateTime now = DateTime.Now;
                  for (long num4 = 0L; num4 < fileStream.Length; num4 += 188)
                  {
                    if (_bwDecrypt.CancellationPending)
                    {
                      e.Cancel = true;
                      try
                      {
                        fileStream2.Close();
                        File.Delete(path);
                      }
                      catch
                      {
                      }
                      return;
                    }
                    array3 = new byte[188];
                    fileStream.Read(array3, 0, array3.Length);
                    if (array3[0] != 71)
                    {
                      _bwDecrypt.ReportProgress((int)(fileStream.Position * 1000 / fileStream.Length), new DecryptThreadState(DecryptThreadState.ThreadStateEnum.SyncLost));
                      flag = false;
                      while (!flag && fileStream.Position < fileStream.Length)
                      {
                        array3 = new byte[1024];
                        fileStream.Read(array3, 0, array3.Length);
                        num2 = Decryption.Sync(array3);
                        if (num2 != -1)
                        {
                          flag = true;
                          fileStream.Seek(num4 + num2, SeekOrigin.Begin);
                        }
                        num4 = fileStream.Position;
                        if (_bwDecrypt.CancellationPending)
                        {
                          e.Cancel = true;
                          try
                          {
                            fileStream2.Close();
                            File.Delete(path);
                          }
                          catch
                          {
                          }
                          return;
                        }
                      }
                      continue;
                    }
                    byte[] array4 = Decryption.DecryptPacket(array3, encryptedFileModel.FileKey, encryptedFileModel.DecryptAdaption);
                    fileStream2.Write(array4, 0, array4.Length);
                    if (DateTime.Now - now > TimeSpan.FromMilliseconds(100.0))
                    {
                      try
                      {
                        _bwDecrypt.ReportProgress((int)(fileStream.Position * 1000 / fileStream.Length), new DecryptThreadState(DecryptThreadState.ThreadStateEnum.DecryptionProgress, new object[3]
                        {
                        fileStream.Position,
                        fileStream.Length,
                        (int)((double)(fileStream.Position - num3) / (DateTime.Now - now).TotalSeconds)
                        }));
                      }
                      catch
                      {
                      }
                      now = DateTime.Now;
                      num3 = fileStream.Position;
                    }
                  }
                }
                else
                {
                  _bwDecrypt.ReportProgress(0, new DecryptThreadState(DecryptThreadState.ThreadStateEnum.Error, null, DecryptThreadState.ThreadErrorEnum.InvalidFile, new object[1]
                  {
                  encryptedFileModel
                  }));
                  fileStream.Close();
                  fileStream2.Close();
                  try
                  {
                    File.Delete(path);
                  }
                  catch
                  {
                  }
                }
              }
            }
          }
          catch (Exception error)
          {
            _bwDecrypt.ReportProgress(0, new DecryptThreadState(DecryptThreadState.ThreadStateEnum.Error, error, DecryptThreadState.ThreadErrorEnum.DecryptFailed, new object[1]
            {
            encryptedFileModel
            }));
          }
          GC.Collect();
        }
      }
      catch (Exception error2)
      {
        _bwDecrypt.ReportProgress(0, new DecryptThreadState(DecryptThreadState.ThreadStateEnum.Error, error2, DecryptThreadState.ThreadErrorEnum.Unknown));
      }
    }

    private void _dbDecrypt_ButtonClicked(object sender, RoutedEventArgs e)
    {
      DialogBox.DialogButton dialogButton = sender as DialogBox.DialogButton;
      if (dialogButton.Content == BtnCancel)
      {
        _bwDecrypt.CancelAsync();
      }
    }

    private void ShowAboutBoxCommandExecute()
    {
      new DestroiAboutBox(_owner).ShowDialog();
    }

    private void ExitCommandExecute()
    {
      System.Windows.Application.Current.Shutdown();
    }

    private void OpenLocalCommandExecute(bool? isFTP)
    {
      string resolvedValue = "";
      string resolvedValue2 = "";
      new LocTextExtension("DeSTRoi:Global:OLFilter").ResolveLocalizedValue(out resolvedValue);
      new LocTextExtension("DeSTRoi:Global:OLTitle").ResolveLocalizedValue(out resolvedValue2);
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.CheckFileExists = true;
      openFileDialog.DefaultExt = ".srf";
      openFileDialog.Filter = resolvedValue;
      openFileDialog.Title = resolvedValue2;
      openFileDialog.Multiselect = true;
      openFileDialog.AddExtension = true;
      if (openFileDialog.ShowDialog() == true)
      {
        string[] fileNames = openFileDialog.FileNames;
        foreach (string fileName in fileNames)
        {
          OpenFile(fileName, isFTP);
        }
      }
    }

    public void OpenFile(string fileName, bool? isFTP = null)
    {
      if (!File.Exists(fileName))
      {
        return;
      }
      if (Path.GetExtension(fileName).Trim(' ', '.') != "srf" && Path.GetExtension(fileName).Trim(' ', '.') != "ts")
      {
        string resolvedValue = "";
        new LocTextExtension("DeSTRoi:Global:FFUnknownDesc").ResolveLocalizedValue(out resolvedValue);
        DialogBox dialogBox = new DialogBox();
        dialogBox.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Help_32.png"));
        new LocTextExtension("DeSTRoi:Global:FileFormatUnknown").SetBinding(dialogBox, DialogBox.MainInstructionProperty);
        dialogBox.MainContent = Path.GetFileName(fileName) + resolvedValue;
        dialogBox.ExpanderVisible = false;
        dialogBox.Buttons = new DialogBox.DialogButton[2]
        {
        new DialogBox.DialogButton(BtnNo, closeDialog: true),
        new DialogBox.DialogButton(BtnYes, closeDialog: true)
        };
        dialogBox.ShowDialog();
        if (dialogBox.SelectedButton == BtnNo)
        {
          return;
        }
      }
      SamyINF inf = null;
      using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        byte[] array = new byte[7]
        {
        'S'.ToASCII(),
        'a'.ToASCII(),
        'm'.ToASCII(),
        'y'.ToASCII(),
        'I'.ToASCII(),
        'N'.ToASCII(),
        'F'.ToASCII()
        };
        byte[] array2 = new byte[array.Length];
        fileStream.Seek(-1356 - array.Length, SeekOrigin.End);
        fileStream.Read(array2, 0, array.Length);
        if (array2.SequenceEqual(array))
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            byte[] array3 = new byte[1356];
            fileStream.Read(array3, 0, array3.Length);
            memoryStream.Write(array3, 0, array3.Length);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            inf = new SamyINF(memoryStream);
          }
        }
        else
        {
          
          fileStream.Seek(-7464 - array.Length, SeekOrigin.End);
          fileStream.Read(array2, 0, array.Length);
          if (array2.SequenceEqual(array))
          {
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
              byte[] array4 = new byte[7464];
              fileStream.Read(array4, 0, array4.Length);
              memoryStream2.Write(array4, 0, array4.Length);
              memoryStream2.Seek(0L, SeekOrigin.Begin);
              inf = new SamyINF(memoryStream2);
            }
          }
        }
      }
      if (File.Exists(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".key"))
      {
        _encryptedFiles.Add(new EncryptedFileModel(fileName, File.ReadAllBytes(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".key"), isFTP.HasValue && isFTP.Value, Settings.Default.DecryptAF, inf));
      }
      else if (File.Exists(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".mdb"))
      {
        _encryptedFiles.Add(new EncryptedFileModel(fileName, MDB.Parse(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".mdb"), isFTP.HasValue && isFTP.Value, Settings.Default.DecryptAF, inf));
      }
      else
      {
        _encryptedFiles.Add(new EncryptedFileModel(fileName, new byte[16], isFTP.HasValue && isFTP.Value, Settings.Default.DecryptAF, inf));
      }
    }

    private void OpenFTPCommandExecute()
    {
    
      DeSTRoi.Views.FTPDownloadView fTPDownloadView = new DeSTRoi.Views.FTPDownloadView();
      fTPDownloadView.Owner = _owner;
      FTPDownloadViewModel fTPDownloadViewModel = (FTPDownloadViewModel)fTPDownloadView.DataContext;
      fTPDownloadViewModel.OutputPath = _outputDirectory;
      fTPDownloadViewModel.TVIP = _selectedIP;
      fTPDownloadViewModel.KeyRetreivalMethod = SelectedKeyRetreivalMethod;
      fTPDownloadViewModel.DecryptAF = DecryptAdaptionField;
      if (fTPDownloadView.ShowDialog() == true)
      {
        EncryptedFileModel[] downloadedFTPFiles = fTPDownloadViewModel.DownloadedFTPFiles;
        foreach (EncryptedFileModel encryptedFileModel in downloadedFTPFiles)
        {
          encryptedFileModel.DecryptAdaption = Settings.Default.DecryptAF;
          EncryptedFiles.Add(encryptedFileModel);
        }
      }
    }

    private bool OpenFTPCommandCanExecute()
    {
      try
      {
        return Directory.Exists(_outputDirectory) && new Ping().Send(_selectedIP, 100).Status == IPStatus.Success;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private void OpenKeyCommandExecute()
    {
      string resolvedValue = "";
      string resolvedValue2 = "";
      new LocTextExtension("DeSTRoi:Global:OKFilter").ResolveLocalizedValue(out resolvedValue);
      new LocTextExtension("DeSTRoi:Global:OKTitle").ResolveLocalizedValue(out resolvedValue2);
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.CheckFileExists = true;
      openFileDialog.DefaultExt = ".key";
      openFileDialog.Filter = resolvedValue;
      openFileDialog.Title = resolvedValue2;
      openFileDialog.Multiselect = false;
      openFileDialog.AddExtension = true;
      if (openFileDialog.ShowDialog() == true)
      {
        _selectedEncryptedFiles[0].FileKey = File.ReadAllBytes(openFileDialog.FileName);
      }
    }

    private bool OpenKeyCommandCanExecute()
    {
      if (_selectedEncryptedFiles.Count == 1)
      {
        return true;
      }
      return false;
    }

    private void RemoveFileCommandExecute()
    {
      try
      {
        EncryptedFileModel[] array = SelectedEncryptedFiles.ToArray();
        EncryptedFileModel[] array2 = array;
        foreach (EncryptedFileModel encryptedFileModel in array2)
        {
          if (encryptedFileModel.IsFTP)
          {
            string resolvedValue = "";
            new LocTextExtension("DeSTRoi:Global:MovieDP").ResolveLocalizedValue(out resolvedValue);
            DialogBox dialogBox = new DialogBox();
            dialogBox.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Help_32.png"));
            new LocTextExtension("DeSTRoi:Global:DoDeleteDLFile").SetBinding(dialogBox, DialogBox.MainInstructionProperty);
            dialogBox.MainContent = resolvedValue + encryptedFileModel.FileName;
            dialogBox.Buttons = new DialogBox.DialogButton[2]
            {
            new DialogBox.DialogButton(BtnNo, Key.Escape, ModifierKeys.None, closeDialog: true),
            new DialogBox.DialogButton(BtnYes, Key.Return, ModifierKeys.None, closeDialog: true)
            };
            dialogBox.ShowDialog();
            if (dialogBox.SelectedButton == BtnYes)
            {
              File.Delete(encryptedFileModel.FilePath);
              if (File.Exists(Path.GetDirectoryName(encryptedFileModel.FilePath) + "\\" + Path.GetFileNameWithoutExtension(encryptedFileModel.FilePath) + ".key"))
              {
                File.Delete(Path.GetDirectoryName(encryptedFileModel.FilePath) + "\\" + Path.GetFileNameWithoutExtension(encryptedFileModel.FilePath) + ".key");
              }
            }
          }
          _encryptedFiles.Remove(encryptedFileModel);
        }
      }
      catch
      {
      }
    }

    private bool RemoveFileCommandCanExecute()
    {
      return _selectedEncryptedFiles.Count > 0;
    }

    private void OpenFolderCommandExecute(string target)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      folderBrowserDialog.ShowNewFolderButton = true;
      string a;
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK && (a = target) != null && a == "OutputDirectory")
      {
        OutputDirectory = folderBrowserDialog.SelectedPath;
      }
    }

    private void RefreshIPCommandExecute()
    {
      _isIPUpdating = true;
      LocalIPScanner localIPScanner = new LocalIPScanner();
      localIPScanner.NetworkSearchComplete += _ipScanner_NetworkSearchComplete;
      localIPScanner.FindLocalIPsAsync();
    }

    private bool RefreshIPCommandCanExecute()
    {
      return !_isIPUpdating;
    }

    private void DecryptFilesCommandExecute()
    {
      if (EncryptedFiles.Where((EncryptedFileModel hk) => hk.HasKey).Count() != EncryptedFiles.Count)
      {
        DialogBox dialogBox = new DialogBox();
        dialogBox.Owner = Owner;
        new LocTextExtension("DeSTRoi:Global:Warning").SetBinding(dialogBox, Window.TitleProperty);
        dialogBox.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/alert_32.png"));
        new LocTextExtension("DeSTRoi:Global:NotAllFilesKey").SetBinding(dialogBox, DialogBox.MainInstructionProperty);
        new LocTextExtension("DeSTRoi:Global:SkipNoKey").SetBinding(dialogBox, DialogBox.MainContentProperty);
        dialogBox.Buttons = new DialogBox.DialogButton[2]
        {
        new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true),
        new DialogBox.DialogButton("_Ok", DialogBox.DialogButton.SpecialButtonEnum.Ok, closeDialog: true)
        };
        bool progressBarVisible = dialogBox.ExpanderVisible = false;
        dialogBox.ProgressBarVisible = progressBarVisible;
        dialogBox.ShowDialog();
        if (dialogBox.SelectedButton == null || dialogBox.SelectedButton == BtnCancel)
        {
          return;
        }
      }
      _dbDecrypt = new DialogBox();
      _dbDecrypt.Owner = Owner;
      new LocTextExtension("DeSTRoi:Global:Decryption").SetBinding(_dbDecrypt, Window.TitleProperty);
      _dbDecrypt.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Decrypted_32.png"));
      _dbDecrypt.IconAnimationEnable = true;
      _dbDecrypt.ProgressBarIsMarquee = false;
      _dbDecrypt.ProgressBarMin = 0;
      _dbDecrypt.ProgressBarMax = 1000;
      _dbDecrypt.ProgressBarValue = 0;
      _dbDecrypt.ProgressBarVisible = true;
      _dbDecrypt.ExpanderVisible = true;
      new LocTextExtension("DeSTRoi:Global:DecryptionInProgress").SetBinding(_dbDecrypt, DialogBox.MainInstructionProperty);
      _dbDecrypt.Buttons = new DialogBox.DialogButton[1]
      {
      new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true)
      };
      _dbDecrypt.ButtonClicked += _dbDecrypt_ButtonClicked;
      _dbDecrypt.Show();
      Owner.IsEnabled = false;
      _bwDecrypt.RunWorkerAsync(new object[2]
      {
      EncryptedFiles.Where((EncryptedFileModel hk) => hk.HasKey).ToArray(),
      _outputDirectory
      });
    }

    private bool DecryptFilesCommandCanExecute()
    {
      return EncryptedFiles.Count > 0;
    }

    private void ShowDetailsCommandExecute(EncryptedFileModel efm)
    {
      InfoViewModel dataContext;
      dataContext = new InfoViewModel(efm.INF);
      DeSTRoi.Views.InfoView infoView;
      infoView = new DeSTRoi.Views.InfoView();
      infoView.Owner = Owner;
      infoView.DataContext = dataContext;
      infoView.ShowDialog();
    }

    private bool ShowDetailsCommandCanExecute(EncryptedFileModel efm)
    {
      return efm.INF != null;
    }
  }
}
