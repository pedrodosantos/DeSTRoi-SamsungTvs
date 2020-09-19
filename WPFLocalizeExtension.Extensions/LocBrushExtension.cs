// WPFLocalizeExtension.Extensions.LocBrushExtension
using System;
using System.Windows.Markup;
using System.Windows.Media;
using WPFLocalizeExtension;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.Extensions
{
	[MarkupExtensionReturnType(typeof(Brush))]
	public class LocBrushExtension : BaseLocalizeExtension<Brush>
	{
		public LocBrushExtension()
		{
		}

		public LocBrushExtension(string key)
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
			if (obj.GetType().Equals(typeof(string)))
			{
				return FormatOutput(obj);
			}
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of System.Drawing.Bitmap");
		}

		protected override void HandleNewValue()
		{
			object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault());
			SetNewValue(new BrushConverter().ConvertFromString((string)localizedObject));
		}

		protected override object FormatOutput(object input)
		{
			if (LocalizeDictionary.Instance.GetIsInDesignMode() && base.DesignValue != null)
			{
				try
				{
					return new BrushConverter().ConvertFromString((string)base.DesignValue);
				}
				catch
				{
					return null;
				}
			}
			return new BrushConverter().ConvertFromString((string)input);
		}
	}
}