// DeSTRoi.Properties.Settings
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace DeSTRoi.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	[CompilerGenerated]
	public sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default => defaultInstance;

		[DefaultSettingValue("")]
		[DebuggerNonUserCode]
		[UserScopedSetting]
		public string OutputDirectory
		{
			get
			{
				return (string)this["OutputDirectory"];
			}
			set
			{
				this["OutputDirectory"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string TVIP
		{
			get
			{
				return (string)this["TVIP"];
			}
			set
			{
				this["TVIP"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("MDB")]
		public string KeyRetMethod
		{
			get
			{
				return (string)this["KeyRetMethod"];
			}
			set
			{
				this["KeyRetMethod"] = value;
			}
		}

		[DefaultSettingValue("")]
		[DebuggerNonUserCode]
		[UserScopedSetting]
		public string FTPUser
		{
			get
			{
				return (string)this["FTPUser"];
			}
			set
			{
				this["FTPUser"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string FTPPwd
		{
			get
			{
				return (string)this["FTPPwd"];
			}
			set
			{
				this["FTPPwd"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool DecryptAF
		{
			get
			{
				return (bool)this["DecryptAF"];
			}
			set
			{
				this["DecryptAF"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool FTPIsAnon
		{
			get
			{
				return (bool)this["FTPIsAnon"];
			}
			set
			{
				this["FTPIsAnon"] = value;
			}
		}
	}
}