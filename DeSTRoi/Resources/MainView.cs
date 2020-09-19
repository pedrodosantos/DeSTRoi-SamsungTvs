using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Text;

namespace DeSTRoi.Properties
{

	public class MainView
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
					ResourceManager resourceManager = resourceMan = new ResourceManager("DeSTRoi.Properties.MainView", typeof(MainView).Assembly);
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

		public static string AboutLabel => ResourceManager.GetString("AboutLabel", resourceCulture);

		public static string AboutTTDesc => ResourceManager.GetString("AboutTTDesc", resourceCulture);

		public static string AboutTTTitle => ResourceManager.GetString("AboutTTTitle", resourceCulture);

		public static string AddKey => ResourceManager.GetString("AddKey", resourceCulture);

		public static string AFLabel => ResourceManager.GetString("AFLabel", resourceCulture);

		public static string AnonFTPLabel => ResourceManager.GetString("AnonFTPLabel", resourceCulture);

		public static string AnonFTPTTDesc => ResourceManager.GetString("AnonFTPTTDesc", resourceCulture);

		public static string AnonFTPTTFooterDesc => ResourceManager.GetString("AnonFTPTTFooterDesc", resourceCulture);

		public static string Decrypt => ResourceManager.GetString("Decrypt", resourceCulture);

		public static string Decryption => ResourceManager.GetString("Decryption", resourceCulture);

		public static string Download => ResourceManager.GetString("Download", resourceCulture);

		public static string DRMGet => ResourceManager.GetString("DRMGet", resourceCulture);

		public static string Encryption => ResourceManager.GetString("Encryption", resourceCulture);

		public static string File => ResourceManager.GetString("File", resourceCulture);

		public static string Filename => ResourceManager.GetString("Filename", resourceCulture);

		public static string Filesize => ResourceManager.GetString("Filesize", resourceCulture);

		public static string FTPPwdLabel => ResourceManager.GetString("FTPPwdLabel", resourceCulture);

		public static string FTPUserLabel => ResourceManager.GetString("FTPUserLabel", resourceCulture);

		public static string HelpBtnTTDesc => ResourceManager.GetString("HelpBtnTTDesc", resourceCulture);

		public static string HelpBtnTTTitle => ResourceManager.GetString("HelpBtnTTTitle", resourceCulture);

		public static string Key => ResourceManager.GetString("Key", resourceCulture);

		public static string LabelDecrypt => ResourceManager.GetString("LabelDecrypt", resourceCulture);

		public static string LabelDownload => ResourceManager.GetString("LabelDownload", resourceCulture);

		public static string LabelKeyRetreival => ResourceManager.GetString("LabelKeyRetreival", resourceCulture);

		public static string LabelOpen => ResourceManager.GetString("LabelOpen", resourceCulture);

		public static string LabelRefreshIPs => ResourceManager.GetString("LabelRefreshIPs", resourceCulture);

		public static string LabelRemove => ResourceManager.GetString("LabelRemove", resourceCulture);

		public static string LabelSelect => ResourceManager.GetString("LabelSelect", resourceCulture);

		public static string MDB => ResourceManager.GetString("MDB", resourceCulture);

		public static string MovieFile => ResourceManager.GetString("MovieFile", resourceCulture);

		public static string Open => ResourceManager.GetString("Open", resourceCulture);

		public static string OutputDirectory => ResourceManager.GetString("OutputDirectory", resourceCulture);

		public static string Settings => ResourceManager.GetString("Settings", resourceCulture);

		public static string Start => ResourceManager.GetString("Start", resourceCulture);

		public static string Title => ResourceManager.GetString("Title", resourceCulture);

		public static string TTAFDesc => ResourceManager.GetString("TTAFDesc", resourceCulture);

		public static string TTAFFooterDesc => ResourceManager.GetString("TTAFFooterDesc", resourceCulture);

		public static string TTAFTitle => ResourceManager.GetString("TTAFTitle", resourceCulture);

		public static string TTDecryptDesc => ResourceManager.GetString("TTDecryptDesc", resourceCulture);

		public static string TTDecryptFooterDesc => ResourceManager.GetString("TTDecryptFooterDesc", resourceCulture);

		public static string TTDecryptGroupDesc => ResourceManager.GetString("TTDecryptGroupDesc", resourceCulture);

		public static string TTDecryptGroupTitle => ResourceManager.GetString("TTDecryptGroupTitle", resourceCulture);

		public static string TTDecryptTitle => ResourceManager.GetString("TTDecryptTitle", resourceCulture);

		public static string TTDownloadFTPDesc => ResourceManager.GetString("TTDownloadFTPDesc", resourceCulture);

		public static string TTDownloadFTPFooterDesc => ResourceManager.GetString("TTDownloadFTPFooterDesc", resourceCulture);

		public static string TTDownloadFTPFooterTitle => ResourceManager.GetString("TTDownloadFTPFooterTitle", resourceCulture);

		public static string TTDownloadFTPTitle => ResourceManager.GetString("TTDownloadFTPTitle", resourceCulture);

		public static string TTFileGroupDesc => ResourceManager.GetString("TTFileGroupDesc", resourceCulture);

		public static string TTFileGroupTitle => ResourceManager.GetString("TTFileGroupTitle", resourceCulture);

		public static string TTKeyGroupDesc => ResourceManager.GetString("TTKeyGroupDesc", resourceCulture);

		public static string TTKeyGroupTitle => ResourceManager.GetString("TTKeyGroupTitle", resourceCulture);

		public static string TTKeyRetreivalDesc => ResourceManager.GetString("TTKeyRetreivalDesc", resourceCulture);

		public static string TTKeyRetreivalTitle => ResourceManager.GetString("TTKeyRetreivalTitle", resourceCulture);

		public static string TTOpenFTPFooterTitle => ResourceManager.GetString("TTOpenFTPFooterTitle", resourceCulture);

		public static string TTOpenKeyDesc => ResourceManager.GetString("TTOpenKeyDesc", resourceCulture);

		public static string TTOpenKeyTitle => ResourceManager.GetString("TTOpenKeyTitle", resourceCulture);

		public static string TTOpenLocalDesc => ResourceManager.GetString("TTOpenLocalDesc", resourceCulture);

		public static string TTOpenLocalFooterDesc => ResourceManager.GetString("TTOpenLocalFooterDesc", resourceCulture);

		public static string TTOpenLocalTitle => ResourceManager.GetString("TTOpenLocalTitle", resourceCulture);

		public static string TTOutDirDesc => ResourceManager.GetString("TTOutDirDesc", resourceCulture);

		public static string TTOutDirTBDesc => ResourceManager.GetString("TTOutDirTBDesc", resourceCulture);

		public static string TTOutDirTBFooterDesc => ResourceManager.GetString("TTOutDirTBFooterDesc", resourceCulture);

		public static string TTOutDirTBTitle => ResourceManager.GetString("TTOutDirTBTitle", resourceCulture);

		public static string TTOutDirTitle => ResourceManager.GetString("TTOutDirTitle", resourceCulture);

		public static string TTOutputSettingsDesc => ResourceManager.GetString("TTOutputSettingsDesc", resourceCulture);

		public static string TTOutputSettingsTitle => ResourceManager.GetString("TTOutputSettingsTitle", resourceCulture);

		public static string TTRefreshIPDesc => ResourceManager.GetString("TTRefreshIPDesc", resourceCulture);

		public static string TTRefreshIPTitle => ResourceManager.GetString("TTRefreshIPTitle", resourceCulture);

		public static string TTRemoveFileDesc => ResourceManager.GetString("TTRemoveFileDesc", resourceCulture);

		public static string TTRemoveFileFooterDesc => ResourceManager.GetString("TTRemoveFileFooterDesc", resourceCulture);

		public static string TTRemoveFileTitle => ResourceManager.GetString("TTRemoveFileTitle", resourceCulture);

		public static string TTTVSettingDesc => ResourceManager.GetString("TTTVSettingDesc", resourceCulture);

		public static string TTTVSettingTitle => ResourceManager.GetString("TTTVSettingTitle", resourceCulture);

		public static string TVIP => ResourceManager.GetString("TVIP", resourceCulture);

		public static string TVIPTTDesc => ResourceManager.GetString("TVIPTTDesc", resourceCulture);

		public static string TVIPTTFooter => ResourceManager.GetString("TVIPTTFooter", resourceCulture);

		public static string TVIPTTTitle => ResourceManager.GetString("TVIPTTTitle", resourceCulture);

		public static string TVSettings => ResourceManager.GetString("TVSettings", resourceCulture);

		internal MainView()
		{
		}
	}
}
