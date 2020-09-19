// WPFLocalizeExtension.Extensions.LocImageExtension
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.Extensions
{
	[MarkupExtensionReturnType(typeof(BitmapSource))]
	public class LocImageExtension : BaseLocalizeExtension<BitmapSource>
	{
		public LocImageExtension()
		{
		}

		public LocImageExtension(string key)
			: base(key)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			object obj = base.ProvideValue(serviceProvider);
			if (obj == null)
			{
				return null;
			}
			if (IsTypeOf(obj.GetType(), typeof(BaseLocalizeExtension<>)))
			{
				return obj;
			}
			if (obj.GetType().Equals(typeof(Bitmap)))
			{
				return FormatOutput(obj);
			}
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of System.Drawing.Bitmap");
		}

		protected override void HandleNewValue()
		{
			object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault());
			SetNewValue(FormatOutput(localizedObject));
		}

		protected override object FormatOutput(object input)
		{
			IntPtr hbitmap = ((Bitmap)input).GetHbitmap();
			BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			bitmapSource.Freeze();
			DeleteObject(hbitmap);
			return bitmapSource;
		}

		[DllImport("gdi32.dll")]
		private static extern int DeleteObject(IntPtr o);
	}
}