// DeSTRoi.Properties.InfoView
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace DeSTRoi.Properties
{
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	public class InfoView
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
					ResourceManager resourceManager = resourceMan = new ResourceManager("DeSTRoi.Properties.InfoView", typeof(InfoView).Assembly);
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

		public static string Category => ResourceManager.GetString("Category", resourceCulture);

		public static string ChannelName => ResourceManager.GetString("ChannelName", resourceCulture);

		public static string ChannelNumber => ResourceManager.GetString("ChannelNumber", resourceCulture);

		public static string Close => ResourceManager.GetString("Close", resourceCulture);

		public static string ContentLanguage => ResourceManager.GetString("ContentLanguage", resourceCulture);

		public static string Duration => ResourceManager.GetString("Duration", resourceCulture);

		public static string Favorite => ResourceManager.GetString("Favorite", resourceCulture);

		public static string Info => ResourceManager.GetString("Info", resourceCulture);

		public static string Language => ResourceManager.GetString("Language", resourceCulture);

		public static string Lock => ResourceManager.GetString("Lock", resourceCulture);

		public static string MovieInformation => ResourceManager.GetString("MovieInformation", resourceCulture);

		public static string RecTime => ResourceManager.GetString("RecTime", resourceCulture);

		public static string TimerEnd => ResourceManager.GetString("TimerEnd", resourceCulture);

		public static string TimerStart => ResourceManager.GetString("TimerStart", resourceCulture);

		public static string Title => ResourceManager.GetString("Title", resourceCulture);

		internal InfoView()
		{
		}
	}
}