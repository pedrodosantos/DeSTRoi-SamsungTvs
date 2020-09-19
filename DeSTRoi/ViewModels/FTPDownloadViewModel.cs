// DeSTRoi.ViewModels.FTPDownloadViewModel
using DeSTRoi.Libraries.IO;
using DeSTRoi.Models;
using DeSTRoi.Properties;
using MinimalisticTelnet;
using Starksoft.Net.Ftp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;

namespace DeSTRoi.ViewModels
{
	public class FTPDownloadViewModel : ObservableObject, IDisposable
	{
		private class FTPRefreshThreadState
		{
			public enum ThreadStateEnum
			{
				Undetermined,
				MovieFound,
				Error
			}

			public enum ThreadErrorEnum
			{
				None,
				Unknown,
				ConnectionLost
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

			public FTPRefreshThreadState()
			{
				State = ThreadStateEnum.Undetermined;
				ErrorType = ThreadErrorEnum.None;
				Error = null;
				Data = null;
			}

			public FTPRefreshThreadState(ThreadStateEnum state)
				: this()
			{
				State = state;
			}

			public FTPRefreshThreadState(ThreadStateEnum state, object[] data)
				: this(state)
			{
				Data = data;
			}

			public FTPRefreshThreadState(ThreadStateEnum state, Exception error)
				: this(state)
			{
				Error = error;
			}

			public FTPRefreshThreadState(ThreadStateEnum state, Exception error, object[] data)
				: this(state, error)
			{
				Data = data;
			}

			public FTPRefreshThreadState(ThreadStateEnum state, Exception error, ThreadErrorEnum errType)
				: this(state)
			{
				Error = error;
				ErrorType = errType;
			}

			public FTPRefreshThreadState(ThreadStateEnum state, Exception error, ThreadErrorEnum errType, object[] data)
				: this(state, error, errType)
			{
				Data = data;
			}
		}

		private class FTPDownloadThreadState
		{
			public enum ThreadStateEnum
			{
				Undetermined,
				Error,
				GetDRMGetKey1,
				GetDRMGetKey2,
				GetDRMGetKey3,
				GetMDBKey1,
				GetMDBKey2,
				NoMDBKeyFound,
				PreparingDL,
				DownloadFile,
				ReportDownloadProgress,
				DownloadCompleted
			}

			public enum ThreadErrorEnum
			{
				None,
				Unknown,
				DRMGetUploadFailed,
				KeyRetFailed,
				DownloadError
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
					if (_error != null && ErrorType != 0)
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

			public FTPDownloadThreadState()
			{
				State = ThreadStateEnum.Undetermined;
				ErrorType = ThreadErrorEnum.None;
				Error = null;
				Data = null;
			}

			public FTPDownloadThreadState(ThreadStateEnum state)
				: this()
			{
				State = state;
			}

			public FTPDownloadThreadState(ThreadStateEnum state, object[] data)
				: this(state)
			{
				Data = data;
			}

			public FTPDownloadThreadState(ThreadStateEnum state, Exception error)
				: this(state)
			{
				Error = error;
			}

			public FTPDownloadThreadState(ThreadStateEnum state, Exception error, object[] data)
				: this(state, error)
			{
				Data = data;
			}

			public FTPDownloadThreadState(ThreadStateEnum state, Exception error, ThreadErrorEnum errType)
				: this(state)
			{
				Error = error;
				ErrorType = errType;
			}

			public FTPDownloadThreadState(ThreadStateEnum state, Exception error, ThreadErrorEnum errType, object[] data)
				: this(state, error, errType)
			{
				Data = data;
			}
		}

		private BackgroundWorker _ftpRefreshThread;

		private string _tvip = "";

		private ObservableCollection<FTPFileModel> _ftpFiles;

		private ObservableCollection<FTPFileModel> _selectedFtpFiles;

		private List<EncryptedFileModel> _downloadedFtpFiles;

		private BackgroundWorker _downloadBGWorker;

		private Window _owner;

		private DialogBox _dbDownload;

		private Semaphore _waitforUserInputSemaphore;

		private Semaphore _threadCanceledSemaphore;

		private Semaphore _dlCompletedSemaphore;

		private DateTime _lastDispUpdateTime = DateTime.MinValue;

		private long _totalDownloaded;

		private string _outputPath = "";

		private string _keyRetMethod = "";

		private bool _otfDecrypt;

		private Queue<byte> _ftpData;

		private bool _isSynchronized;

		private string KeyRetDRMGet
		{
			get
			{
				string resolvedValue = "";
				new LocTextExtension("DeSTRoi:MainView:DRMGet").ResolveLocalizedValue(out resolvedValue);
				return resolvedValue;
			}
		}

		private string KeyRetMDB
		{
			get
			{
				string resolvedValue = "";
				new LocTextExtension("DeSTRoi:MainView:MDB").ResolveLocalizedValue(out resolvedValue);
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

		private string BtnSkip
		{
			get
			{
				string resolvedValue = "";
				new LocTextExtension("DeSTRoi:Global:Btn\u00b4Skip").ResolveLocalizedValue(out resolvedValue);
				return resolvedValue;
			}
		}

		private string BtnNext
		{
			get
			{
				string resolvedValue = "";
				new LocTextExtension("DeSTRoi:Global:BtnNext").ResolveLocalizedValue(out resolvedValue);
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

		public bool DecryptAF
		{
			get;
			set;
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

		public string TVIP
		{
			get
			{
				return _tvip;
			}
			set
			{
				lock (_tvip)
				{
					_tvip = value;
				}
				RefreshFTPListAsync();
				RaisePropertyChanged("TVIP");
			}
		}

		public bool KeyRetIsMDB => KeyRetreivalMethod == KeyRetMDB;

		public string OutputPath
		{
			get
			{
				return _outputPath;
			}
			set
			{
				lock (_outputPath)
				{
					_outputPath = value;
				}
			}
		}

		public string KeyRetreivalMethod
		{
			get
			{
				string text = "";
				lock (_keyRetMethod)
				{
					return _keyRetMethod;
				}
			}
			set
			{
				lock (_keyRetMethod)
				{
					_keyRetMethod = value;
				}
				RaisePropertyChanged("KeyRetreivalMethod");
				RaisePropertyChanged("KeyRetIsMDB");
			}
		}

		public ObservableCollection<FTPFileModel> FTPFiles => _ftpFiles;

		public ObservableCollection<FTPFileModel> SelectedFTPFiles
		{
			get
			{
				return _selectedFtpFiles;
			}
			set
			{
				_selectedFtpFiles = value;
			}
		}

		public bool OnlyOneFileSelected => SelectedFTPFiles.Count == 1;

		public bool OTFDecrypt
		{
			get
			{
				return _otfDecrypt;
			}
			set
			{
				_otfDecrypt = value;
				RaisePropertyChanged("OTFDecrypt");
			}
		}

		public EncryptedFileModel[] DownloadedFTPFiles
		{
			get
			{
				EncryptedFileModel[] array = null;
				lock (_downloadedFtpFiles)
				{
					return _downloadedFtpFiles.ToArray();
				}
			}
		}

		public ICommand ViewClosingCommand => new RelayCommand(ViewClosingCommandExecute);

		public ICommand RefreshListCommand => new RelayCommand(RefreshListCommandExecute, RefreshListCommandCanExecute);

		public ICommand DownloadSelectedCommand => new RelayCommand(DownloadSelectedCommandExecute, DownloadSelectedCommandCanExecute);

		public event EventHandler RequestClose;

		public FTPDownloadViewModel()
		{
			_tvip = "";
			_ftpFiles = new ObservableCollection<FTPFileModel>();
			_selectedFtpFiles = new ObservableCollection<FTPFileModel>();
			_selectedFtpFiles.CollectionChanged += _selectedFtpFiles_CollectionChanged;
			_downloadedFtpFiles = new List<EncryptedFileModel>();
			_ftpRefreshThread = new BackgroundWorker();
			_ftpRefreshThread.WorkerReportsProgress = true;
			_ftpRefreshThread.WorkerSupportsCancellation = true;
			_ftpRefreshThread.DoWork += _ftpRefreshThread_DoWork;
			_ftpRefreshThread.ProgressChanged += _ftpRefreshThread_ProgressChanged;
			_ftpRefreshThread.RunWorkerCompleted += _ftpRefreshThread_RunWorkerCompleted;
			_downloadBGWorker = new BackgroundWorker();
			_downloadBGWorker.WorkerSupportsCancellation = (_downloadBGWorker.WorkerReportsProgress = true);
			_downloadBGWorker.DoWork += _downloadBGWorker_DoWork;
			_downloadBGWorker.ProgressChanged += _downloadBGWorker_ProgressChanged;
			_downloadBGWorker.RunWorkerCompleted += _downloadBGWorker_RunWorkerCompleted;
			_threadCanceledSemaphore = new Semaphore(0, 1);
		}

		public void Dispose()
		{
			ViewClosingCommandExecute();
		}

		private void _selectedFtpFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged("OnlyOneFileSelected");
		}

		protected void RaiseRequestClose()
		{
			if (this.RequestClose != null)
			{
				this.RequestClose(this, new EventArgs());
			}
		}

		private Regex SelectRegex(string input)
		{
			string[] source = new string[5]
			{
			"(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)",
			"(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)",
			"(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)",
			"(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)",
			"(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})(\\s+)(?<size>(\\d+))(\\s+)(?<ctbit>(\\w+\\s\\w+))(\\s+)(?<size2>(\\d+))\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{2}:\\d{2})\\s+(?<name>.+)"
			};
			Regex regex = (from r in source
										 where Regex.IsMatch(input, r)
										 select new Regex(r, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant)).ToList().FirstOrDefault();
			if (regex == null)
			{
				regex = new Regex("");
			}
			return regex;
		}

		private string AddTrailingBackslash(string inp)
		{
			if (inp.Trim().Last() != '\\')
			{
				return inp.Trim() + '\\';
			}
			return inp.Trim();
		}

		private string DecryptFTPPwd(byte[] data)
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
					memoryStream.Write(data, 0, data.Length);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read))
					{
						byte[] array = new byte[data.Length];
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

		private void RefreshFTPListAsync()
		{
			if (_ftpRefreshThread != null && !_ftpRefreshThread.IsBusy)
			{
				FTPFiles.Clear();
				_ftpRefreshThread.RunWorkerAsync(new object[4]
				{
				_tvip,
				Settings.Default.FTPIsAnon,
				Settings.Default.FTPUser,
				DecryptFTPPwd(Settings.Default.FTPPwd.ToByteArray())
				});
			}
		}

		private void _ftpRefreshThread_DoWork(object sender, DoWorkEventArgs e)
		{
			string text = (string)((object[])e.Argument)[0];
			bool flag = (bool)((object[])e.Argument)[1];
			string user = "anonymous";
			string password = "";
			if (!flag)
			{
				user = (string)((object[])e.Argument)[2];
				password = (string)((object[])e.Argument)[3];
			}
			using (Ping ping = new Ping())
			{
				if (ping.Send(text, 100).Status != 0)
				{
					_ftpRefreshThread.ReportProgress(0, new FTPRefreshThreadState(FTPRefreshThreadState.ThreadStateEnum.Error, null, FTPRefreshThreadState.ThreadErrorEnum.ConnectionLost));
				}
			}
			if (_ftpRefreshThread.CancellationPending)
			{
				e.Cancel = true;
				try
				{
					_threadCanceledSemaphore.Release();
				}
				catch
				{
				}
				return;
			}
			Thread tr = new Thread(() =>
			{
				try
				{
					using (FtpClient ftpClient = new FtpClient(text))
					{
						ftpClient.Open(user, password);
						ftpClient.FileTransferType = TransferType.Binary;
						ftpClient.ChangeDirectory("/dtv/usb");
						FtpItemCollection dirList = ftpClient.GetDirList();
						List<string> list = new List<string>();
						foreach (string item in from dd in dirList
																		where dd.ItemType == FtpItemType.Directory && dd.Name.StartsWith("sd")
																		select dd.FullPath)
						{
							if (_ftpRefreshThread.CancellationPending)
							{
								e.Cancel = true;
								try
								{
									_threadCanceledSemaphore.Release();
								}
								catch
								{
								}
								return;
							}
							ftpClient.ChangeDirectory(item);
							if (ftpClient.Exists("CONTENTS"))
							{
								list.Add(item.Trim() + "/CONTENTS");
							}
						}
						int num = 0;
						foreach (string item2 in list)
						{
							if (_ftpRefreshThread.CancellationPending)
							{
								e.Cancel = true;
								try
								{
									_threadCanceledSemaphore.Release();
								}
								catch
								{
								}
								break;
							}
							ftpClient.ChangeDirectory(item2);
							FtpItemCollection dirList2 = ftpClient.GetDirList();
							foreach (FtpItem item3 in dirList2.Where((FtpItem rf) => rf.ItemType == FtpItemType.File && rf.Name.EndsWith("srf")))
							{
								if (_ftpRefreshThread.CancellationPending)
								{
									e.Cancel = true;
									try
									{
										_threadCanceledSemaphore.Release();
									}
									catch
									{
									}
									return;
								}
								using (MemoryStream memoryStream = new MemoryStream())
								{
									ftpClient.GetFile(Path.GetFileNameWithoutExtension(item3.Name) + ".inf", memoryStream, restart: false);
									memoryStream.Seek(0L, SeekOrigin.Begin);
									_ftpRefreshThread.ReportProgress(++num, new FTPRefreshThreadState(FTPRefreshThreadState.ThreadStateEnum.MovieFound, new object[1]
									{
								new FTPFileModel(item2, item3.Name, item3.Size, new SamyINF(memoryStream))
									}));
								}
							}
						}
					}
				}
				catch (Exception error)
				{
					_ftpRefreshThread.ReportProgress(0, new FTPRefreshThreadState(FTPRefreshThreadState.ThreadStateEnum.Error, error, FTPRefreshThreadState.ThreadErrorEnum.Unknown));
				}
			});

			tr.SetApartmentState(ApartmentState.STA);
			tr.Start();
			tr.Join();
		}

		private void _ftpRefreshThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			FTPRefreshThreadState fTPRefreshThreadState = e.UserState as FTPRefreshThreadState;
			try
			{
				if (fTPRefreshThreadState == null)
				{
					return;
				}
				switch (fTPRefreshThreadState.State)
				{
					case FTPRefreshThreadState.ThreadStateEnum.Error:
						switch (fTPRefreshThreadState.ErrorType)
						{
							case FTPRefreshThreadState.ThreadErrorEnum.Unknown:
								{
									string resolvedValue3 = "";
									Exception ex2 = (fTPRefreshThreadState.Error != null) ? fTPRefreshThreadState.Error : new Exception();
									new LocTextExtension("DeSTRoi:Global:MovieListError").ResolveLocalizedValue(out resolvedValue3);
									DialogBox.ShowDialog(Owner, resolvedValue3, ex2.Message, showErrorIcon: true, "Stack Trace:", ex2.StackTrace);
									break;
								}
							case FTPRefreshThreadState.ThreadErrorEnum.ConnectionLost:
								{
									string resolvedValue = "";
									string resolvedValue2 = "";
									new LocTextExtension("DeSTRoi:Global:ConnectionError").ResolveLocalizedValue(out resolvedValue);
									new LocTextExtension("DeSTRoi:Global:ConnectionLost").ResolveLocalizedValue(out resolvedValue2);
									DialogBox.ShowDialog(resolvedValue, resolvedValue2, showErrorIcon: true);
									break;
								}
						}
						break;
					case FTPRefreshThreadState.ThreadStateEnum.MovieFound:
						{
							FTPFileModel fTPFileModel = fTPRefreshThreadState[0] as FTPFileModel;
							if (fTPFileModel != null)
							{
								FTPFiles.Add(fTPFileModel);
							}
							break;
						}
				}
			}
			catch (Exception ex3)
			{
				Exception ex = ex3;
				string locTxt1 = "";
				new LocTextExtension("DeSTRoi:Global:UnexpectedErrorMainInst").ResolveLocalizedValue(out locTxt1);
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					DialogBox.ShowDialog(Owner, locTxt1, ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
				}, null);
			}
		}

		private void _ftpRefreshThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			RaisePropertyChanged("RefreshListCommand");
		}

		private void _downloadBGWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			FTPFileModel[] array = (FTPFileModel[])((object[])e.Argument)[2];
			Hashtable hashtable = new Hashtable();
			new List<EncryptedFileModel>();
			string text = (string)((object[])e.Argument)[0];
			string inp = (string)((object[])e.Argument)[1];
			bool flag = (bool)((object[])e.Argument)[3];
			string user = "anonymous";
			string password = "";
			if (!flag)
			{
				user = (string)((object[])e.Argument)[4];
				password = (string)((object[])e.Argument)[5];
			}
			inp = ((!OTFDecrypt) ? (AddTrailingBackslash(inp) + "tmp\\") : AddTrailingBackslash(inp));
			if (!Directory.Exists(inp))
			{
				Directory.CreateDirectory(inp);
			}
			FtpClient ftpClient = null;
			try
			{
				using (ftpClient = new FtpClient(text))
				{
					if (KeyRetreivalMethod == KeyRetDRMGet)
					{
						TelnetConnection telnetConnection = new TelnetConnection(text, 23);
						Thread.Sleep(100);
						if (!ftpClient.IsConnected)
						{
							ftpClient.Open(user, password);
							ftpClient.FileTransferType = TransferType.Binary;
						}
						try
						{
							ftpClient.ChangeDirectory("/tmp");
							if (!ftpClient.Exists("drmget"))
							{
								using (Stream inputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DeSTRoi.Resources.Apps.drmget"))
								{
									ftpClient.PutFile(inputStream, "drmget", FileAction.Create);
								}
								ftpClient.Close();
								if (_downloadBGWorker.CancellationPending)
								{
									e.Cancel = true;
									return;
								}
								string text2 = "";
								telnetConnection.WriteLine("cd /");
								while (!text2.TrimEnd().EndsWith("#"))
								{
									text2 += telnetConnection.Read();
								}
								text2 = "";
								telnetConnection.Read();
								telnetConnection.WriteLine("chmod 777 /tmp/drmget");
								while (!text2.TrimEnd().EndsWith("#"))
								{
									text2 += telnetConnection.Read();
								}
							}
							if (_downloadBGWorker.CancellationPending)
							{
								e.Cancel = true;
								return;
							}
						}
						catch (Exception error)
						{
							_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, error, FTPDownloadThreadState.ThreadErrorEnum.DRMGetUploadFailed));
						}
						FTPFileModel[] array2 = array;
						string drmgetLine;
						foreach (FTPFileModel fTPFileModel in array2)
						{
							if (_downloadBGWorker.CancellationPending)
							{
								e.Cancel = true;
								return;
							}
							string text3 = fTPFileModel.INF.ChannelName + "_" + fTPFileModel.INF.Title;
							char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
							foreach (char oldChar in invalidFileNameChars)
							{
								text3 = text3.Replace(oldChar, '.');
							}
							try
							{
								string a = "";
								_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.GetDRMGetKey1, new object[1]
								{
								fTPFileModel
								}));
								while (!_waitforUserInputSemaphore.WaitOne(500))
								{
									if (_downloadBGWorker.CancellationPending)
									{
										e.Cancel = true;
										return;
									}
								}
								lock (_dbDownload)
								{
									lock (_dbDownload.SelectedButton)
									{
										a = _dbDownload.SelectedButton;
									}
								}
								if (a == BtnSkip)
								{
									continue;
								}
								if (a == BtnCancel)
								{
									return;
								}
								for (int num = 9; num >= 0; num--)
								{
									if (_downloadBGWorker.CancellationPending)
									{
										e.Cancel = true;
										return;
									}
									_downloadBGWorker.ReportProgress(num, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.GetDRMGetKey2));
									Thread.Sleep(1000);
								}
								_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.GetDRMGetKey3));
								telnetConnection.Read();
								string text4 = "";
								telnetConnection.WriteLine("/tmp/drmget");
								while (!text4.TrimEnd().EndsWith("#"))
								{
									text4 += telnetConnection.Read();
								}
								using (StringReader stringReader = new StringReader(text4))
								{
									drmgetLine = "";
									do
									{
										drmgetLine = stringReader.ReadLine();
									}
									while (drmgetLine != null && !drmgetLine.StartsWith("DRM Key  :"));
									if (drmgetLine == null)
									{
										string resolvedValue = "";
										new LocTextExtension("DeSTRoi:Global:DRMGetOutputError").ResolveLocalizedValue(out resolvedValue);
										throw new FormatException(resolvedValue);
									}
									drmgetLine = drmgetLine.Substring(10).Replace(" ", "").Trim();
									hashtable.Add(fTPFileModel, (from x in Enumerable.Range(0, drmgetLine.Length)
																							 where x % 2 == 0
																							 select Convert.ToByte(drmgetLine.Substring(x, 2), 16)).ToArray());
								}
								File.WriteAllBytes(inp + text3 + ".key", (byte[])hashtable[fTPFileModel]);
							}
							catch (Exception error2)
							{
								try
								{
									File.Delete(inp + text3 + ".key");
								}
								finally
								{
									hashtable.Remove(fTPFileModel);
								}
								_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, error2, FTPDownloadThreadState.ThreadErrorEnum.KeyRetFailed, new object[1]
								{
								fTPFileModel
								}));
							}
						}
					}
					else if (KeyRetreivalMethod == KeyRetMDB)
					{
						_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.GetMDBKey1, new object[1]
						{
						array.Length
						}));
						int num2 = 0;
						if (!ftpClient.IsConnected)
						{
							ftpClient.Open(user, password);
							ftpClient.FileTransferType = TransferType.Binary;
						}
						FTPFileModel[] array3 = array;
						foreach (FTPFileModel fTPFileModel2 in array3)
						{
							if (_downloadBGWorker.CancellationPending)
							{
								e.Cancel = true;
								return;
							}
							string text5 = fTPFileModel2.INF.ChannelName + "_" + fTPFileModel2.INF.Title;
							char[] invalidFileNameChars2 = Path.GetInvalidFileNameChars();
							foreach (char oldChar2 in invalidFileNameChars2)
							{
								text5 = text5.Replace(oldChar2, '.');
							}
							num2++;
							_downloadBGWorker.ReportProgress(num2, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.GetMDBKey2, new object[2]
							{
							fTPFileModel2,
							array.Length
							}));
							try
							{
								string text6 = inp + text5 + ".mdb";
								string remotePath = Path.GetFileNameWithoutExtension(fTPFileModel2.FileName) + ".mdb";
								ftpClient.ChangeDirectory(fTPFileModel2.FilePath);
								ftpClient.GetDirList();
								if (OTFDecrypt)
								{
									using (MemoryStream memoryStream = new MemoryStream())
									{
										ftpClient.GetFile(remotePath, memoryStream, restart: false);
										hashtable.Add(fTPFileModel2, MDB.Parse(memoryStream));
									}
								}
								else
								{
									ftpClient.GetFile(remotePath, text6, FileAction.Create);
									hashtable.Add(fTPFileModel2, MDB.Parse(text6, saveKey: true));
								}
								if (((byte[])hashtable[fTPFileModel2]).All((byte keyByte) => keyByte == 0))
								{
									_downloadBGWorker.ReportProgress(num2, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.NoMDBKeyFound, new object[1]
									{
									fTPFileModel2
									}));
									hashtable[fTPFileModel2] = null;
									try
									{
										File.Delete(inp + text5 + ".key");
									}
									catch
									{
									}
								}
							}
							catch (Exception error3)
							{
								try
								{
									File.Delete(inp + text5 + ".key");
									File.Delete(inp + text5 + ".mdb");
								}
								finally
								{
									try
									{
										hashtable.Remove(fTPFileModel2);
									}
									catch
									{
									}
								}
								_downloadBGWorker.ReportProgress(num2, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, error3, FTPDownloadThreadState.ThreadErrorEnum.KeyRetFailed, new object[1]
								{
								fTPFileModel2
								}));
							}
						}
						ftpClient.Close();
					}
					else
					{
						FTPFileModel[] array2 = array;
						foreach (FTPFileModel key in array2)
						{
							hashtable.Add(key, new byte[16]);
						}
					}
					int num3 = 0;
					_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.PreparingDL));
					if (!ftpClient.IsConnected)
					{
						ftpClient.Open(user, password);
						ftpClient.FileTransferType = TransferType.Binary;
					}
					foreach (FTPFileModel key2 in hashtable.Keys)
					{
						if (_downloadBGWorker.CancellationPending)
						{
							try
							{
								if (ftpClient != null && ftpClient.IsConnected)
								{
									ftpClient.Close();
								}
							}
							catch
							{
							}
							e.Cancel = true;
							return;
						}
						string text7 = key2.INF.ChannelName + "_" + key2.INF.Title;
						char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
						foreach (char oldChar3 in invalidFileNameChars)
						{
							text7 = text7.Replace(oldChar3, '.');
						}
						string text8 = inp + text7 + ".srf";
						if (OTFDecrypt)
						{
							text8 = inp + text7 + ".ts";
						}
						num3++;
						_downloadBGWorker.ReportProgress(num3, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.DownloadFile, new object[2]
						{
						key2,
						hashtable.Count
						}));
						try
						{
							ftpClient.ChangeDirectory(key2.FilePath);
							ftpClient.GetDirList();
							_lastDispUpdateTime = DateTime.Now;
							_totalDownloaded = 0L;
							_dlCompletedSemaphore = new Semaphore(0, 1);
							EventHandler<TransferProgressEventArgs> value = ftpClient_TransferProgress;
							EventHandler<GetFileAsyncCompletedEventArgs> value2 = ftpClient_GetFileAsyncCompleted;
							ftpClient.TransferProgress += value;
							ftpClient.GetFileAsyncCompleted += value2;
							if (OTFDecrypt)
							{
								_isSynchronized = false;
								using (MemoryStream outStream = new MemoryStream())
								{
									using (FileStream fileStream = new FileStream(text8, FileMode.Create, FileAccess.Write))
									{
										ftpClient.GetFileAsync(key2.FileName, outStream, restart: false, new object[3]
										{
										key2,
										fileStream,
										hashtable[key2]
										});
										_dlCompletedSemaphore.WaitOne();
									}
								}
							}
							else
							{
								ftpClient.GetFileAsync(key2.FileName, text8, FileAction.Create, key2);
								_dlCompletedSemaphore.WaitOne();
								using (FileStream fileStream2 = new FileStream(text8, FileMode.Append, FileAccess.Write))
								{
									 
									fileStream2.Seek(0L, SeekOrigin.End);
									fileStream2.Write(new byte[188], 0, 188);
									fileStream2.Write(new byte[7]
									{
									'S'.ToASCII(),
									'a'.ToASCII(),
									'm'.ToASCII(),
									'y'.ToASCII(),
									'I'.ToASCII(),
									'N'.ToASCII(),
									'F'.ToASCII()
									}, 0, new byte[7]
									{
									'S'.ToASCII(),
									'a'.ToASCII(),
									'm'.ToASCII(),
									'y'.ToASCII(),
									'I'.ToASCII(),
									'N'.ToASCII(),
									'F'.ToASCII()
									}.Length);
									GC.Collect();
									key2.INF.Save(fileStream2);
								}
							}
							try
							{
								ftpClient.TransferProgress -= value;
								ftpClient.GetFileAsyncCompleted -= value2;
							}
							catch
							{
							}
							if (_downloadBGWorker.CancellationPending)
							{
								try
								{
									if (ftpClient != null && ftpClient.IsConnected)
									{
										ftpClient.Close();
									}
								}
								catch
								{
								}
								e.Cancel = true;
								return;
							}
							if (!OTFDecrypt)
							{
								lock (_downloadedFtpFiles)
								{
									_downloadBGWorker.ReportProgress(1000, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.DownloadCompleted, new object[1]
									{
									new EncryptedFileModel(text8, (byte[])hashtable[key2], isFTP: true, key2.INF)
									}));
								}
							}
						}
						catch (Exception error4)
						{
							_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, error4, FTPDownloadThreadState.ThreadErrorEnum.DownloadError, new object[1]
							{
							key2
							}));
						}
						finally
						{
							_totalDownloaded = 0L;
						}
					}
				}
			}
			catch (Exception error5)
			{
				_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, error5, FTPDownloadThreadState.ThreadErrorEnum.DownloadError, new object[1]
				{
				new FTPFileModel("", "")
				}));
			}
			finally
			{
				_totalDownloaded = 0L;
			}
			if (_downloadBGWorker.CancellationPending)
			{
				e.Cancel = true;
			}
		}

		private void _downloadBGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			FTPDownloadThreadState fTPDownloadThreadState = e.UserState as FTPDownloadThreadState;
			try
			{
				if (fTPDownloadThreadState == null)
				{
					return;
				}
				switch (fTPDownloadThreadState.State)
				{
					case FTPDownloadThreadState.ThreadStateEnum.Error:
						{
							Exception ex = (fTPDownloadThreadState.Error != null) ? fTPDownloadThreadState.Error : new Exception();
							switch (fTPDownloadThreadState.ErrorType)
							{
								case FTPDownloadThreadState.ThreadErrorEnum.Unknown:
									{
										string resolvedValue5 = "";
										new LocTextExtension("DeSTRoi:Global:UnexpectedErrorMainInst").ResolveLocalizedValue(out resolvedValue5);
										DialogBox.ShowDialog(Owner, resolvedValue5, ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
										break;
									}
								case FTPDownloadThreadState.ThreadErrorEnum.DRMGetUploadFailed:
									{
										string resolvedValue3 = "";
										string resolvedValue4 = "";
										new LocTextExtension("DeSTRoi:Global:DRMGetULFailed").ResolveLocalizedValue(out resolvedValue3);
										new LocTextExtension("DeSTRoi:Global:DRMGetULFaildMsg").ResolveLocalizedValue(out resolvedValue4);
										DialogBox.ShowDialog(Owner, resolvedValue3, resolvedValue4 + "\n" + ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
										break;
									}
								case FTPDownloadThreadState.ThreadErrorEnum.KeyRetFailed:
									{
										FTPFileModel fTPFileModel3 = fTPDownloadThreadState[0] as FTPFileModel;
										if (fTPFileModel3 == null)
										{
											throw new ArgumentNullException("currDl");
										}
										string resolvedValue6 = "";
										new LocTextExtension("DeSTRoi:Global:KeyRetFailed").ResolveLocalizedValue(out resolvedValue6);
										DialogBox.ShowDialog(Owner, resolvedValue6, fTPFileModel3.INF.ChannelName + ": " + fTPFileModel3.INF.Title + "\n" + ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
										break;
									}
								case FTPDownloadThreadState.ThreadErrorEnum.DownloadError:
									{
										FTPFileModel fTPFileModel2 = fTPDownloadThreadState[0] as FTPFileModel;
										if (fTPFileModel2 == null)
										{
											throw new ArgumentNullException("currDl");
										}
										string resolvedValue2 = "";
										new LocTextExtension("DeSTRoi:Global:DownloadError").ResolveLocalizedValue(out resolvedValue2);
										DialogBox.ShowDialog(Owner, resolvedValue2, fTPFileModel2.INF.ChannelName + ": " + fTPFileModel2.INF.Title + "\n" + ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
										break;
									}
							}
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.GetDRMGetKey1:
						{
							string resolvedValue = "";
							FTPFileModel fTPFileModel = fTPDownloadThreadState[0] as FTPFileModel;
							if (fTPFileModel == null)
							{
								throw new ArgumentNullException("currDl");
							}
							new LocTextExtension("DeSTRoi:Global:MIGettingKey").ResolveLocalizedValue(out resolvedValue);
							_dbDownload.MainInstruction = resolvedValue + fTPFileModel.INF.ChannelName + ": " + fTPFileModel.INF.Title;
							new LocTextExtension("DeSTRoi:Global:MCGetKey").SetBinding(_dbDownload, DialogBox.MainContentProperty);
							_dbDownload.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Key_32.png"));
							_dbDownload.IconAnimationEnable = false;
							_dbDownload.IconVisible = true;
							_dbDownload.ProgressBarVisible = false;
							_dbDownload.Buttons = new DialogBox.DialogButton[3]
							{
					new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true),
					new DialogBox.DialogButton(BtnSkip, Key.S, ModifierKeys.Alt, closeDialog: false),
					new DialogBox.DialogButton(BtnNext, Key.Return, ModifierKeys.None, closeDialog: false)
							};
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.GetDRMGetKey2:
						{
							string resolvedValue8 = "";
							string resolvedValue9 = "";
							new LocTextExtension("DeSTRoi:Global:Seconds").ResolveLocalizedValue(out resolvedValue8);
							new LocTextExtension("DeSTRoi:Global:WaitFor").ResolveLocalizedValue(out resolvedValue9);
							_dbDownload.MainContent = resolvedValue9 + e.ProgressPercentage + resolvedValue8;
							_dbDownload.IconAnimationEnable = true;
							_dbDownload.ProgressBarVisible = true;
							_dbDownload.Buttons = new DialogBox.DialogButton[1]
							{
					new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true)
							};
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.GetDRMGetKey3:
						new LocTextExtension("DeSTRoi:Global:RetreivingKey").SetBinding(_dbDownload, DialogBox.MainContentProperty);
						break;
					case FTPDownloadThreadState.ThreadStateEnum.GetMDBKey1:
						{
							int progressBarMax = Convert.ToInt32(fTPDownloadThreadState[0]);
							new LocTextExtension("DeSTRoi:Global:RetreivingKey").SetBinding(_dbDownload, DialogBox.MainInstructionProperty);
							_dbDownload.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Key_32.png"));
							_dbDownload.IconAnimationEnable = true;
							_dbDownload.IconVisible = true;
							_dbDownload.ExpanderVisible = false;
							_dbDownload.ProgressBarIsMarquee = false;
							_dbDownload.ProgressBarVisible = true;
							_dbDownload.ProgressBarMin = 1;
							_dbDownload.ProgressBarMax = progressBarMax;
							_dbDownload.ProgressBarValue = 1;
							_dbDownload.Buttons = new DialogBox.DialogButton[1]
							{
					new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true)
							};
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.GetMDBKey2:
						{
							FTPFileModel fTPFileModel7 = fTPDownloadThreadState[0] as FTPFileModel;
							int num2 = Convert.ToInt32(fTPDownloadThreadState[1]);
							if (fTPFileModel7 == null)
							{
								throw new ArgumentNullException("currDl");
							}
							string resolvedValue13 = "";
							string resolvedValue14 = "";
							new LocTextExtension("DeSTRoi:Global:CurrentFile").ResolveLocalizedValue(out resolvedValue13);
							new LocTextExtension("DeSTRoi:Global:File").ResolveLocalizedValue(out resolvedValue14);
							_dbDownload.MainContent = resolvedValue13 + fTPFileModel7.INF.ChannelName + ": " + fTPFileModel7.INF.Title + "\n" + resolvedValue14 + e.ProgressPercentage + LocalizedOf + num2;
							_dbDownload.ProgressBarValue = e.ProgressPercentage;
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.NoMDBKeyFound:
						{
							FTPFileModel fTPFileModel6 = fTPDownloadThreadState[0] as FTPFileModel;
							string resolvedValue10 = "";
							string resolvedValue11 = "";
							string resolvedValue12 = "";
							new LocTextExtension("DeSTRoi:Global:NoKeyDetectedTitle").ResolveLocalizedValue(out resolvedValue10);
							new LocTextExtension("DeSTRoi:Global:NoKeyDetectedMessage").ResolveLocalizedValue(out resolvedValue11);
							new LocTextExtension("DeSTRoi:Global:FurtherInfo").ResolveLocalizedValue(out resolvedValue12);
							DialogBox.ShowDialog(resolvedValue10, fTPFileModel6.INF.ChannelName + ": " + fTPFileModel6.INF.Title, showErrorIcon: true, resolvedValue12, resolvedValue11);
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.PreparingDL:
						new LocTextExtension("DeSTRoi:Global:DLFiles").SetBinding(_dbDownload, DialogBox.MainInstructionProperty);
						new LocTextExtension("DeSTRoi:Global:PreparingDL").SetBinding(_dbDownload, DialogBox.MainContentProperty);
						_dbDownload.MainIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Download_32.png"));
						_dbDownload.IconAnimationEnable = true;
						_dbDownload.IconVisible = true;
						_dbDownload.ExpanderVisible = true;
						_dbDownload.ExpanderIsExpanded = true;
						_dbDownload.ProgressBarIsMarquee = false;
						_dbDownload.ProgressBarVisible = true;
						_dbDownload.ProgressBarMin = 0;
						_dbDownload.ProgressBarMax = 1000;
						_dbDownload.ProgressBarValue = 0;
						_dbDownload.Buttons = new DialogBox.DialogButton[1]
						{
					new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true)
						};
						break;
					case FTPDownloadThreadState.ThreadStateEnum.DownloadFile:
						{
							FTPFileModel fTPFileModel5 = fTPDownloadThreadState[0] as FTPFileModel;
							if (fTPFileModel5 == null)
							{
								throw new ArgumentNullException("currDl");
							}
							int num = Convert.ToInt32(fTPDownloadThreadState[1]);
							string resolvedValue7 = "";
							new LocTextExtension("DeSTRoi:Global:CurrentFile").ResolveLocalizedValue(out resolvedValue7);
							_dbDownload.MainContent = resolvedValue7 + fTPFileModel5.INF.ChannelName + ": " + fTPFileModel5.INF.Title;
							new LocTextExtension("DeSTRoi:Global:File").ResolveLocalizedValue(out resolvedValue7);
							_dbDownload.ExpanderHeader = resolvedValue7 + e.ProgressPercentage + LocalizedOf + num;
							_dbDownload.ProgressBarValue = 0;
							_dbDownload.Buttons = new DialogBox.DialogButton[1]
							{
					new DialogBox.DialogButton(BtnCancel, DialogBox.DialogButton.SpecialButtonEnum.Cancel, closeDialog: true)
							};
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.ReportDownloadProgress:
						{
							FTPFileModel fTPFileModel4 = fTPDownloadThreadState[0] as FTPFileModel;
							if (fTPFileModel4 != null)
							{
								long source = Convert.ToInt64(fTPDownloadThreadState[1]);
								int source2 = Convert.ToInt32(fTPDownloadThreadState[2]);
								_dbDownload.ProgressBarValue = e.ProgressPercentage;
								try
								{
									_dbDownload.ExpanderContent = FileSizeConverter.ConvertToFileSize(source) + LocalizedOf + FileSizeConverter.ConvertToFileSize(fTPFileModel4.FileSize) + "\n" + FileSizeConverter.ConvertToFileSize(source2) + "/s";
								}
								catch
								{
									_dbDownload.ExpanderContent = FileSizeConverter.ConvertToFileSize(source) + LocalizedOf + FileSizeConverter.ConvertToFileSize(fTPFileModel4.FileSize);
								}
							}
							break;
						}
					case FTPDownloadThreadState.ThreadStateEnum.DownloadCompleted:
						{
							EncryptedFileModel encryptedFileModel = fTPDownloadThreadState[0] as EncryptedFileModel;
							if (encryptedFileModel == null)
							{
								throw new ArgumentNullException("encFile");
							}
							_downloadedFtpFiles.Add(encryptedFileModel);
							break;
						}
				}
			}
			catch (Exception ex2)
			{
				string resolvedValue15 = "";
				new LocTextExtension("DeSTRoi:Global:UnexpectedErrorMainInst").ResolveLocalizedValue(out resolvedValue15);
				DialogBox.ShowDialog(Owner, resolvedValue15, ex2.Message, showErrorIcon: true, "Stack Trace:", ex2.StackTrace);
			}
		}

		private void _downloadBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				_dbDownload.Close();
			}
			finally
			{
				if (!e.Cancelled || e.Error != null)
				{
					Owner.DialogResult = true;
					Owner.Close();
				}
				else if (e.Cancelled)
				{
					Owner.DialogResult = false;
					Owner.Close();
				}
				else
				{
					Owner.IsEnabled = true;
				}
			}
		}

		private void ftpClient_GetFileAsyncCompleted(object sender, GetFileAsyncCompletedEventArgs e)
		{
			try
			{
				if (e.Error != null && !e.Cancelled)
				{
					_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, e.Error, FTPDownloadThreadState.ThreadErrorEnum.DownloadError, new object[1]
					{
					e.UserState
					}));
				}
			}
			catch
			{
			}
			if (_dlCompletedSemaphore != null)
			{
				_dlCompletedSemaphore.Release();
			}
			GC.Collect();
		}

		private void ftpClient_TransferProgress(object sender, TransferProgressEventArgs e)
		{
			try
			{
				if (_downloadBGWorker.CancellationPending)
				{
					((FtpBase)sender).CancelAsync();
					return;
				}
			}
			catch (Exception error)
			{
				_downloadBGWorker.ReportProgress(0, new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.Error, error, FTPDownloadThreadState.ThreadErrorEnum.Unknown));
			}
			FTPFileModel fTPFileModel = null;
			if (OTFDecrypt)
			{
				FileStream fileStream = null;
				byte[] array = null;
				fTPFileModel = (((object[])e.Parameter)[0] as FTPFileModel);
				fileStream = (((object[])e.Parameter)[1] as FileStream);
				array = (((object[])e.Parameter)[2] as byte[]);
				if (_ftpData == null)
				{
					_ftpData = new Queue<byte>();
				}
				_ftpData.Enqueue(e.Buffer);
				while (!_isSynchronized && _ftpData.Count > 1024)
				{
					int num = Decryption.Sync(_ftpData.Peek(1024));
					if (num != -1)
					{
						_isSynchronized = true;
						_ftpData.Dequeue(num);
					}
					else
					{
						_ftpData.Dequeue(1024);
					}
				}
				while (_isSynchronized && _ftpData.Count > 188)
				{
					if (_ftpData.Peek() != 71)
					{
						_isSynchronized = false;
						while (!_isSynchronized && _ftpData.Count > 1024)
						{
							int num2 = Decryption.Sync(_ftpData.Peek(1024));
							if (num2 != -1)
							{
								_isSynchronized = true;
								_ftpData.Dequeue(num2);
							}
							else
							{
								_ftpData.Dequeue(1024);
							}
						}
						if (!_isSynchronized)
						{
							return;
						}
					}
					byte[] array2 = Decryption.DecryptPacket(_ftpData.Dequeue(188), array, DecryptAF);
					fileStream.Write(array2, 0, array2.Length);
				}
			}
			else
			{
				fTPFileModel = (e.Parameter as FTPFileModel);
			}
			_totalDownloaded += e.BytesTransferred;
			if (DateTime.Now - _lastDispUpdateTime > TimeSpan.FromMilliseconds(100.0))
			{
				try
				{
					_downloadBGWorker.ReportProgress((int)(_totalDownloaded * 1000 / fTPFileModel.FileSize), new FTPDownloadThreadState(FTPDownloadThreadState.ThreadStateEnum.ReportDownloadProgress, new object[3]
					{
					fTPFileModel,
					_totalDownloaded,
					e.BytesPerSecond
					}));
				}
				catch
				{
				}
				_lastDispUpdateTime = DateTime.Now;
			}
		}

		private void _dbDownload_ButtonClicked(object sender, RoutedEventArgs e)
		{
			DialogBox.DialogButton dialogButton = sender as DialogBox.DialogButton;
			if (dialogButton.Content == BtnCancel)
			{
				Owner.IsEnabled = true;
				if (_downloadBGWorker != null && _downloadBGWorker.IsBusy)
				{
					_downloadBGWorker.CancelAsync();
				}
			}
			else
			{
				_waitforUserInputSemaphore.Release();
			}
		}

		private void ViewClosingCommandExecute()
		{
			if (_ftpRefreshThread != null && _ftpRefreshThread.IsBusy)
			{
				_ftpRefreshThread.CancelAsync();
				_threadCanceledSemaphore.WaitOne(TimeSpan.FromSeconds(10.0));
			}
			GC.Collect();
		}

		private void RefreshListCommandExecute()
		{
			RefreshFTPListAsync();
		}

		private bool RefreshListCommandCanExecute()
		{
			if (_ftpRefreshThread != null)
			{
				return !_ftpRefreshThread.IsBusy;
			}
			return false;
		}

		private void DownloadSelectedCommandExecute()
		{
			try
			{
				if (SelectedFTPFiles.Count <= 0)
				{
					return;
				}
				lock (_downloadedFtpFiles)
				{
					_downloadedFtpFiles.Clear();
				}
				Owner.IsEnabled = false;
				string resolvedValue = "";
				new LocTextExtension("DeSTRoi:Global:BtnCancel").ResolveLocalizedValue(out resolvedValue);
				_dbDownload = new DialogBox();
				_dbDownload.Owner = _owner;
				new LocTextExtension("DeSTRoi:Global:DLMovies").SetBinding(_dbDownload, Window.TitleProperty);
				if (KeyRetreivalMethod == KeyRetDRMGet)
				{
					new LocTextExtension("DeSTRoi:Global:UploadDRMGet").SetBinding(_dbDownload, DialogBox.MainInstructionProperty);
				}
				else
				{
					if (!(KeyRetreivalMethod == KeyRetMDB))
					{
						throw new ArgumentException("", "KeyRetreivalMethod");
					}
					new LocTextExtension("DeSTRoi:Global:PreparingDL").SetBinding(_dbDownload, DialogBox.MainInstructionProperty);
				}
				new LocTextExtension("DeSTRoi:Global:PlsWait").SetBinding(_dbDownload, DialogBox.MainContentProperty);
				_dbDownload.Buttons = new DialogBox.DialogButton[1]
				{
				new DialogBox.DialogButton(resolvedValue, DialogBox.DialogButton.SpecialButtonEnum.Cancel)
				};
				_dbDownload.ButtonClicked += _dbDownload_ButtonClicked;
				_dbDownload.ProgressBarIsMarquee = true;
				_dbDownload.ProgressBarVisible = true;
				_dbDownload.Show();
				_waitforUserInputSemaphore = new Semaphore(0, 1);
				_downloadBGWorker.RunWorkerAsync(new object[6]
				{
				_tvip,
				_outputPath,
				SelectedFTPFiles.ToArray(),
				Settings.Default.FTPIsAnon,
				Settings.Default.FTPUser,
				DecryptFTPPwd(Settings.Default.FTPPwd.ToByteArray())
				});
			}
			catch (Exception ex)
			{
				string resolvedValue2 = "";
				new LocTextExtension("DeSTRoi:Global:DownloadError").ResolveLocalizedValue(out resolvedValue2);
				DialogBox.ShowDialog(Owner, resolvedValue2, ex.Message, showErrorIcon: true, "Stack Trace:", ex.StackTrace);
			}
		}

		private bool DownloadSelectedCommandCanExecute()
		{
			return _selectedFtpFiles.Count > 0;
		}
	}
}
