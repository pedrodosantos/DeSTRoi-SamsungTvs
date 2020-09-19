// DeSTRoi.Properties.FTPDownloadView
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DeSTRoi.Properties
{
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	public class FTPDownloadView
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
					ResourceManager resourceManager = resourceMan = new ResourceManager("DeSTRoi.Properties.FTPDownloadView", typeof(FTPDownloadView).Assembly);
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

		public static string Cancel => ResourceManager.GetString("Cancel", resourceCulture);

		public static string Category => ResourceManager.GetString("Category", resourceCulture);

		public static string Channel => ResourceManager.GetString("Channel", resourceCulture);

		public static string ChannelNumber => ResourceManager.GetString("ChannelNumber", resourceCulture);

		public static string DetInfoLabel => ResourceManager.GetString("DetInfoLabel", resourceCulture);

		public static string Download => ResourceManager.GetString("Download", resourceCulture);

		public static string Duration => ResourceManager.GetString("Duration", resourceCulture);

		public static string FTPDownload => ResourceManager.GetString("FTPDownload", resourceCulture);

		public static string KeyRetMethod => ResourceManager.GetString("KeyRetMethod", resourceCulture);

		public static string Language => ResourceManager.GetString("Language", resourceCulture);

		public static string MovieTitle => ResourceManager.GetString("MovieTitle", resourceCulture);

		public static string OTF => ResourceManager.GetString("OTF", resourceCulture);

		public static string RecTime => ResourceManager.GetString("RecTime", resourceCulture);

		public static string RefreshList => ResourceManager.GetString("RefreshList", resourceCulture);

		public static string Size => ResourceManager.GetString("Size", resourceCulture);

		public static string TVIP => ResourceManager.GetString("TVIP", resourceCulture);

		public FTPDownloadView()
		{
		}
	}
}