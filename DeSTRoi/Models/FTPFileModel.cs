// DeSTRoi.Models.FTPFileModel
namespace DeSTRoi.Models
{
	public class FTPFileModel : ObservableObject
	{
		private string _ftpPath;

		private string _ftpFile;

		private long _ftpFileSize;

		private SamyINF _inf;

		public string FilePath => _ftpPath;

		public string FileName => _ftpFile;

		public long FileSize
		{
			get
			{
				return _ftpFileSize;
			}
			set
			{
				_ftpFileSize = value;
				RaisePropertyChanged("FileSize");
			}
		}

		public SamyINF INF
		{
			get
			{
				return _inf;
			}
			set
			{
				_inf = value;
				RaisePropertyChanged("INF");
			}
		}

		public FTPFileModel(string ftpPath, string ftpFile)
		{
			_ftpPath = ftpPath;
			_ftpFile = ftpFile;
			_inf = new SamyINF();
		}

		public FTPFileModel(string ftpPath, string ftpFile, SamyINF inf)
		{
			_ftpPath = ftpPath;
			_ftpFile = ftpFile;
			_inf = inf;
		}

		public FTPFileModel(string ftpPath, string ftpFile, long ftpFileSize)
			: this(ftpPath, ftpFile)
		{
			_ftpFileSize = ftpFileSize;
		}

		public FTPFileModel(string ftpPath, string ftpFile, long ftpFileSize, SamyINF inf)
			: this(ftpPath, ftpFile, inf)
		{
			_ftpFileSize = ftpFileSize;
		}
	}
}