// DeSTRoi.Properties.Global
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace DeSTRoi.Properties
{
	[CompilerGenerated]
	public class Global
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("DeSTRoi.Properties.Global", typeof(Global).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		public static string AbortDownload => ResourceManager.GetString("AbortDownload", resourceCulture);

		public static string BtnCancel => ResourceManager.GetString("BtnCancel", resourceCulture);

		public static string BtnNext => ResourceManager.GetString("BtnNext", resourceCulture);

		public static string BtnNo => ResourceManager.GetString("BtnNo", resourceCulture);

		public static string BtnOK => ResourceManager.GetString("BtnOK", resourceCulture);

		public static string BtnSkip => ResourceManager.GetString("BtnSkip", resourceCulture);

		public static string BtnYes => ResourceManager.GetString("BtnYes", resourceCulture);

		public static string CNSMSOF => ResourceManager.GetString("CNSMSOF", resourceCulture);

		public static string ConnectionError => ResourceManager.GetString("ConnectionError", resourceCulture);

		public static string ConnectionLost => ResourceManager.GetString("ConnectionLost", resourceCulture);

		public static string CurrentFile => ResourceManager.GetString("CurrentFile", resourceCulture);

		public static string Decryption => ResourceManager.GetString("Decryption", resourceCulture);

		public static string DecryptionFailed => ResourceManager.GetString("DecryptionFailed", resourceCulture);

		public static string DecryptionInProgress => ResourceManager.GetString("DecryptionInProgress", resourceCulture);

		public static string DLFiles => ResourceManager.GetString("DLFiles", resourceCulture);

		public static string DLMovies => ResourceManager.GetString("DLMovies", resourceCulture);

		public static string DoDeleteDLFile => ResourceManager.GetString("DoDeleteDLFile", resourceCulture);

		public static string DownloadError => ResourceManager.GetString("DownloadError", resourceCulture);

		public static string DRMGetOutputError => ResourceManager.GetString("DRMGetOutputError", resourceCulture);

		public static string DRMGetULFaildMsg => ResourceManager.GetString("DRMGetULFaildMsg", resourceCulture);

		public static string DRMGetULFailed => ResourceManager.GetString("DRMGetULFailed", resourceCulture);

		public static string ExceptionDP => ResourceManager.GetString("ExceptionDP", resourceCulture);

		public static string FatErrContent => ResourceManager.GetString("FatErrContent", resourceCulture);

		public static string FatErrTitle => ResourceManager.GetString("FatErrTitle", resourceCulture);

		public static string FFUnknownDesc => ResourceManager.GetString("FFUnknownDesc", resourceCulture);

		public static string File => ResourceManager.GetString("File", resourceCulture);

		public static string FileFormatUnknown => ResourceManager.GetString("FileFormatUnknown", resourceCulture);

		public static string FurtherInfo => ResourceManager.GetString("FurtherInfo", resourceCulture);

		public static string InvCorFile => ResourceManager.GetString("InvCorFile", resourceCulture);

		public static string KeyRetFailed => ResourceManager.GetString("KeyRetFailed", resourceCulture);

		public static string MCGetKey => ResourceManager.GetString("MCGetKey", resourceCulture);

		public static string MIGettingKey => ResourceManager.GetString("MIGettingKey", resourceCulture);

		public static string MovieDP => ResourceManager.GetString("MovieDP", resourceCulture);

		public static string MovieListError => ResourceManager.GetString("MovieListError", resourceCulture);

		public static string NoDownload => ResourceManager.GetString("NoDownload", resourceCulture);

		public static string NoKeyDetectedMessage => ResourceManager.GetString("NoKeyDetectedMessage", resourceCulture);

		public static string NoKeyDetectedTitle => ResourceManager.GetString("NoKeyDetectedTitle", resourceCulture);

		public static string NotAllFilesKey => ResourceManager.GetString("NotAllFilesKey", resourceCulture);

		public static string NoUploadDRMGet => ResourceManager.GetString("NoUploadDRMGet", resourceCulture);

		public static string of => ResourceManager.GetString("of", resourceCulture);

		public static string OKFilter => ResourceManager.GetString("OKFilter", resourceCulture);

		public static string OKTitle => ResourceManager.GetString("OKTitle", resourceCulture);

		public static string OLFilter => ResourceManager.GetString("OLFilter", resourceCulture);

		public static string OLTitle => ResourceManager.GetString("OLTitle", resourceCulture);

		public static string PlsWait => ResourceManager.GetString("PlsWait", resourceCulture);

		public static string PreparingDL => ResourceManager.GetString("PreparingDL", resourceCulture);

		public static string RetreivingKey => ResourceManager.GetString("RetreivingKey", resourceCulture);

		public static string Seconds => ResourceManager.GetString("Seconds", resourceCulture);

		public static string SkipNoKey => ResourceManager.GetString("SkipNoKey", resourceCulture);

		public static string SyncError => ResourceManager.GetString("SyncError", resourceCulture);

		public static string SyncLost => ResourceManager.GetString("SyncLost", resourceCulture);

		public static string SyncStream => ResourceManager.GetString("SyncStream", resourceCulture);

		public static string UnexpectedErrorMainInst => ResourceManager.GetString("UnexpectedErrorMainInst", resourceCulture);

		public static string UploadDRMGet => ResourceManager.GetString("UploadDRMGet", resourceCulture);

		public static string WaitFor => ResourceManager.GetString("WaitFor", resourceCulture);

		public static string Warning => ResourceManager.GetString("Warning", resourceCulture);

		internal Global()
		{
		}
	}
}