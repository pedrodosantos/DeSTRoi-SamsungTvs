// DeSTRoi.Models.EncryptedFileModel
using DeSTRoi.Libraries.IO;
using DeSTRoi.Models;
using System.IO;
namespace DeSTRoi.Models
{
	public class EncryptedFileModel : ObservableObject
	{
		private string _filePath = "";

		private long _fileSize;

		private byte[] _fileKey;

		private bool _isFTP;

		private bool _decryptAdaption;

		private SamyINF _inf;

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
				RaisePropertyChanged("FilePath");
				RaisePropertyChanged("FileSize");
			}
		}

		public long FileSize
		{
			get
			{
				return _fileSize;
			}
			set
			{
				_fileSize = value;
				RaisePropertyChanged("FileSize");
				RaisePropertyChanged("FileName");
			}
		}

		public byte[] FileKey
		{
			get
			{
				return _fileKey;
			}
			set
			{
				_fileKey = value;
				RaisePropertyChanged("FileKey");
				RaisePropertyChanged("HasKey");
			}
		}

		public bool IsFTP
		{
			get
			{
				return _isFTP;
			}
			set
			{
				_isFTP = value;
				RaisePropertyChanged("IsFTP");
			}
		}

		public bool DecryptAdaption
		{
			get
			{
				return _decryptAdaption;
			}
			set
			{
				_decryptAdaption = value;
				RaisePropertyChanged("DecryptAdaption");
			}
		}

		public string FileName => Path.GetFileNameWithoutExtension(_filePath);

		public string HRFileSize => FileSizeConverter.ConvertToFileSize(_fileSize);

		public bool HasKey
		{
			get
			{
				if (_fileKey != null && _fileKey.Length == 16)
				{
					return true;
				}
				return false;
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

		public EncryptedFileModel()
		{
			
		}

		public EncryptedFileModel(string filePath)
			: this()
		{
			_filePath = filePath;
			_fileSize = new FileInfo(filePath).Length;
		}

		public EncryptedFileModel(string filePath, SamyINF inf)
			: this()
		{
			_filePath = filePath;
			_fileSize = new FileInfo(filePath).Length;
			_inf = inf;
		}

		public EncryptedFileModel(string filePath, bool isFTP)
			: this(filePath)
		{
			_isFTP = isFTP;
		}

		public EncryptedFileModel(string filePath, bool isFTP, SamyINF inf)
			: this(filePath, inf)
		{
			_isFTP = isFTP;
		}

		public EncryptedFileModel(string filePath, byte[] fileKey)
			: this(filePath)
		{
			_fileKey = fileKey;
		}

		public EncryptedFileModel(string filePath, byte[] fileKey, SamyINF inf)
			: this(filePath, inf)
		{
			_fileKey = fileKey;
		}

		public EncryptedFileModel(string filePath, byte[] fileKey, bool isFTP)
			: this(filePath, fileKey)
		{
			_fileKey = fileKey;
			_isFTP = isFTP;
		}

		public EncryptedFileModel(string filePath, byte[] fileKey, bool isFTP, SamyINF inf)
			: this(filePath, fileKey, inf)
		{
			_fileKey = fileKey;
			_isFTP = isFTP;
		}

		public EncryptedFileModel(string filePath, byte[] fileKey, bool isFTP, bool decryptAdaption)
			: this(filePath, fileKey)
		{
			_fileKey = fileKey;
			_isFTP = isFTP;
			_decryptAdaption = decryptAdaption;
		}

		public EncryptedFileModel(string filePath, byte[] fileKey, bool isFTP, bool decryptAdaption, SamyINF inf)
			: this(filePath, fileKey, inf)
		{
			_fileKey = fileKey;
			_isFTP = isFTP;
			_decryptAdaption = decryptAdaption;
		}
	}
}