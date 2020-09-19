// WPFLocalizeExtension.Extensions.LocDoubleExtension
using System;
using System.Globalization;
using System.Windows.Markup;
using WPFLocalizeExtension;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

namespace WPFLocalizeExtension.Extensions
{
	[MarkupExtensionReturnType(typeof(double))]
	public class LocDoubleExtension : BaseLocalizeExtension<double>
	{
		public LocDoubleExtension()
		{
		}

		public LocDoubleExtension(string key)
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
			throw new NotSupportedException($"ResourceKey '{base.Key}' returns '{obj.GetType().FullName}' which is not type of double");
		}

		protected override void HandleNewValue()
		{
			object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject<object>(base.Assembly, base.Dict, base.Key, GetForcedCultureOrDefault());
			SetNewValue(FormatOutput(localizedObject));
		}

		protected override object FormatOutput(object input)
		{
			if (LocalizeDictionary.Instance.GetIsInDesignMode() && base.DesignValue != null)
			{
				try
				{
					return double.Parse((string)base.DesignValue, new CultureInfo("en-US"));
				}
				catch
				{
					return null;
				}
			}
			return double.Parse((string)input, new CultureInfo("en-US"));
		}
	}
}